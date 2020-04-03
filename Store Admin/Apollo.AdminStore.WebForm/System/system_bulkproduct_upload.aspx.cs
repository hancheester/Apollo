using Apollo.AdminStore.WebForm.Classes;
using Apollo.Core.Domain.Common;
using Apollo.Core.Domain.Media;
using Apollo.Core.Model.Entity;
using Apollo.Core.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text.RegularExpressions;
using System.Web;

namespace Apollo.AdminStore.WebForm.System
{
    public partial class system_bulkproduct_upload : BasePage
    {
        public IProductService ProductService { get; set; }
        public ExcelUtility ExcelUtility { get; set; }
        public CommonSettings CommonSettings { get; set; }
        public MediaSettings MediaSettings { get; set; }

        protected void lbUploadImages_Click(object sender, EventArgs e)
        {
            FileUpload();

            hfCurrentPanel.Value = "images";
        }
        
        protected void lbFileUpload_Click(object sender, EventArgs e)
        {
            enbNotice.Message = string.Empty;

            if (productFileUpload.HasFile)
            {
                var fileName = productFileUpload.FileName;
                Regex reg = new Regex(@"(?i)\.(xls|xlsx)$");
                bool isMatch = reg.IsMatch(fileName);
                if (!isMatch)
                {
                    enbNotice.Message = fileName + " is not uploaded, only (.xls or .xlsx) file type is allowed.";
                }
                else
                {
                    var saveBulkProductsPath = CommonSettings.BulkProductFileLocalPath;
                    var filePath = saveBulkProductsPath + fileName;

                    productFileUpload.PostedFile.SaveAs(filePath);
                    DataTable dt = ExcelUtility.PrepareDataTable(filePath, "InsertData", Path.GetExtension(productFileUpload.PostedFile.FileName), true);
                    string result = VerifyTableData(dt);

                    if (!string.IsNullOrEmpty(result))
                    {
                        enbNotice.Message = result;
                    }
                    else
                    {
                        IList<Tuple<int, string>> products = new List<Tuple<int, string>>();
                        result = InsertTableData(dt, out products);

                        if (string.IsNullOrEmpty(result))
                        {
                            gvBulkProducts.DataSource = products;
                            gvBulkProducts.DataBind();
                            enbNotice.Message = "Products have been imported successfully. Please see the imported products below.";
                        }
                        else
                        {
                            gvBulkProducts.DataBind();
                            enbNotice.Message = result;
                        }
                    }
                }
            }

            hfCurrentPanel.Value = "products";
        }

        private void FileUpload()
        {
            try
            {
                HttpFileCollection hfc = Request.Files; //file collection

                for (int i = 1; i < hfc.Count; i++)
                {
                    HttpPostedFile hpf = hfc[i];
                    if (hpf.ContentType != "application/octet-stream")
                    {
                        //System validates posted image files for empty size
                        if (hpf.ContentLength > 0)
                        {
                            //saving the files in storefront productmedia folder
                            Regex imageFilenameRegex = new Regex(@"((?=^.{1,}$)(?!.*\s)[0-9a-zA-Z!@#$%*()_+^&]).*(.jpg|.png|.gif)$", RegexOptions.IgnoreCase);
                            bool ismatch = imageFilenameRegex.IsMatch(hpf.FileName);
                            if (!ismatch)
                            {
                                enbNotice.Message = hpf.FileName + " &nbsp is not uploaded, allowed only (.jpg|.png|.gif) files and filename should not contain the space";
                                break;
                            }

                            hpf.SaveAs(MediaSettings.ProductMediaLocalPath + Path.GetFileName(hpf.FileName));
                            enbNotice.Message = "Images are uploaded successfully.";
                        }

                        else
                        {
                            enbNotice.Message = "File size should not be zero";
                            break;
                        }

                    }
                }


            }
            catch (Exception ex)
            {
                enbNotice.Message = ex.Message;
            }

        }

        private string VerifyTableData(DataTable dt)
        {
            var error = string.Empty;
            var columnNumber = 0;

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                columnNumber = i + 1;

                #region Verify every column data

                string Name = dt.Rows[i]["name*"].ToString();
                if (Name == string.Empty)
                {
                    error = "name should not be empty.";
                    break;
                }
                string description = dt.Rows[i]["description*"].ToString();
                if (description == string.Empty)
                {
                    error = "description should not be empty.";
                    break;
                }
                string brandId = dt.Rows[i]["brandid*"].ToString();
                if (brandId == string.Empty)
                {
                    error = "brandid should not be empty.";
                    break;
                }
                if (CheckBooleanColumn(dt.Rows[i]["enabled*"].ToString()) == false)
                {
                    error = "enabled should be 1(yes) or 0(no).";
                    break;
                }                
                string urlRewrite = dt.Rows[i]["urlrewrite*"].ToString();
                if (urlRewrite == string.Empty)
                {
                    error = "urlrewrite should not be empty.";
                    break;
                }
                string deliveryId = dt.Rows[i]["deliveryid*"].ToString();
                if (deliveryId == string.Empty)
                {
                    error = "deliveryid should not be empty.";
                    break;
                }                
                string categoryId = dt.Rows[i]["categoryid*"].ToString();
                if (categoryId == string.Empty)
                {
                    error = "categoryid should not be empty.";
                    break;
                }
                string optiontype = dt.Rows[i]["optiontype*"].ToString();
                if (optiontype == string.Empty)
                {
                    error = "optiontype should not be empty.";
                    break;
                }
                if (CheckBooleanColumn(dt.Rows[i]["visibleindividually*"].ToString()) == false)
                {
                    error = "visibleindividually should be 1(yes) or 0(no).";
                    break;
                }
                if (CheckBooleanColumn(dt.Rows[i]["ispharmaceutical*"].ToString()) == false)
                {
                    error = "ispharmaceutical should be 1(yes) or 0(no).";
                    break;
                }
                if (CheckBooleanColumn(dt.Rows[i]["hasfreeWrapping*"].ToString()) == false)
                {
                    error = "hasfreeWrapping should be 1(yes) or 0(no).";
                    break;
                }
                if (CheckBooleanColumn(dt.Rows[i]["openforoffer*"].ToString()) == false)
                {
                    error = "openforoffer should be 1(yes) or 0(no).";
                    break;
                }
                if (CheckBooleanColumn(dt.Rows[i]["discontinued*"].ToString()) == false)
                {
                    error = "discontinued should be 1(yes) or 0(no).";
                    break;
                }
                if (CheckBooleanColumn(dt.Rows[i]["enforcestockcount*"].ToString()) == false)
                {
                    error = "enforcestockcount should be 1(yes) or 0(no).";
                    break;
                }
                if (CheckBooleanColumn(dt.Rows[i]["isgoogleproductsearchdisabled*"].ToString()) == false)
                {
                    error = "isgoogleproductsearchdisabled should be 1(yes) or 0(no).";
                    break;
                }
                if (CheckBooleanColumn(dt.Rows[i]["isphoneorder*"].ToString()) == false)
                {
                    error = "isphoneorder should be 1(yes) or 0(no).";
                    break;
                }
                string taxCategoryId = dt.Rows[i]["taxcategoryid*"].ToString();
                int intTaxCategoryId = 0;
                if (int.TryParse(taxCategoryId, out intTaxCategoryId) != true)
                {
                    error = "taxcategoryid should be an integer.";
                    break;
                }
                string price = dt.Rows[i]["price*"].ToString();
                if (price == string.Empty)
                {
                    error = "price should not be empty.";
                    break;
                }
                string weight = dt.Rows[i]["weight*"].ToString();
                if (weight == string.Empty)
                {
                    error = "weight should not be empty.";
                    break;
                }
                string stepQuantity = dt.Rows[i]["stepquantity*"].ToString();
                int stepquan = 0;
                if (int.TryParse(stepQuantity, out stepquan) != true)
                {
                    error = "stepquantity should be an integer.";
                    break;
                }                
                if (CheckBooleanColumn(dt.Rows[i]["priceenabled*"].ToString()) == false)
                {
                    error = "priceenabled should be 1(yes) or 0(no).";
                    break;
                }
                
                #endregion                
            }

            if (string.IsNullOrEmpty(error) == false)
                return string.Format("Error occured at row number {0}. {1}", columnNumber, error);

            return string.Empty;
        }

        private bool CheckBooleanColumn(string value)
        {
            int intValue = 0;
            if (int.TryParse(value, out intValue) != true || intValue != 0 && intValue != 1)
            {
                return false;
            }

            return true;
        }

        private string InsertTableData(DataTable dt, out IList<Tuple<int, string>> products)
        {
            products = new List<Tuple<int, string>>();

            try
            {
                List<BulkProductsInfo> items = new List<BulkProductsInfo>();
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    var bulkProduct = new BulkProductsInfo();
                    bulkProduct.ConvertRowData(dt.Rows[i]);
                    items.Add(bulkProduct);
                }

                products = ProductService.ProcessBulkProductInsertion(items);
                if (products.Count == 0) return "Failed to insert data into database by query.";

                return string.Empty;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
    }
}