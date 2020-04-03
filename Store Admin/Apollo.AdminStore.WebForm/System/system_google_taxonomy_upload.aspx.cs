using Apollo.AdminStore.WebForm.Classes;
using Apollo.Core.Domain.Common;
using Apollo.Core.Services.Interfaces;
using System;
using System.IO;
using System.Text.RegularExpressions;

namespace Apollo.AdminStore.WebForm.System
{
    public partial class system_google_taxonomy_upload : BasePage
    {
        public ICategoryService CategoryService { get; set; }
        public ExcelUtility ExcelUtility { get; set; }
        public CommonSettings CommonSettings { get; set; }
        
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void lbFileUpload_Click(object sender, EventArgs e)
        {
            if (prductFileUpload.HasFile)
            {
                var fileName = prductFileUpload.FileName;
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

                    prductFileUpload.PostedFile.SaveAs(filePath);
                    var dt = ExcelUtility.PrepareDataTable(filePath, "GoogleTaxonomy", Path.GetExtension(prductFileUpload.PostedFile.FileName), false);
                    CategoryService.ProcessNewGoogleTaxonomy(dt);

                    enbNotice.Message = "Google taxonomy has been imported successfully.";
                }
            }
        }        
    }        
}