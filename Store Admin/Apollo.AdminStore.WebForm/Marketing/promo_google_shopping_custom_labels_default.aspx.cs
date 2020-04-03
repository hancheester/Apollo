using Apollo.AdminStore.WebForm.Classes;
using Apollo.Core.Model.Entity;
using Apollo.Core.Model.OverviewModel;
using Apollo.Core.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Apollo.AdminStore.WebForm.Marketing
{
    public partial class promo_google_shopping_custom_labels_default : BasePage
    {
        public IProductService ProductService { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
                LoadGroups();
        }

        protected void lbSearch_Click(object sender, EventArgs e)
        {
            SetState(PRODUCT_ID_FILTER, ((TextBox)gvCustomLabels.HeaderRow.FindControl("txtFilterProductId")).Text.Trim());
            SetState(NAME_FILTER, ((TextBox)gvCustomLabels.HeaderRow.FindControl("txtFilterName")).Text.Trim());
            SetState("customlabel1filter", ((TextBox)gvCustomLabels.HeaderRow.FindControl("txtFilterCustomLabel1")).Text.Trim());
            SetState("customlabel2filter", ((TextBox)gvCustomLabels.HeaderRow.FindControl("txtFilterCustomLabel2")).Text.Trim());
            SetState("customlabel3filter", ((TextBox)gvCustomLabels.HeaderRow.FindControl("txtFilterCustomLabel3")).Text.Trim());
            SetState("customlabel4filter", ((TextBox)gvCustomLabels.HeaderRow.FindControl("txtFilterCustomLabel4")).Text.Trim());
            SetState("customlabel5filter", ((TextBox)gvCustomLabels.HeaderRow.FindControl("txtFilterCustomLabel5")).Text.Trim());
            SetState("value1filter", ((TextBox)gvCustomLabels.HeaderRow.FindControl("txtFilterValue1")).Text.Trim());
            SetState("value2filter", ((TextBox)gvCustomLabels.HeaderRow.FindControl("txtFilterValue2")).Text.Trim());
            SetState("value3filter", ((TextBox)gvCustomLabels.HeaderRow.FindControl("txtFilterValue3")).Text.Trim());
            SetState("value4filter", ((TextBox)gvCustomLabels.HeaderRow.FindControl("txtFilterValue4")).Text.Trim());
            SetState("value5filter", ((TextBox)gvCustomLabels.HeaderRow.FindControl("txtFilterValue5")).Text.Trim());

            enbNotice.Message = string.Empty;
            LoadGroups();
        }

        protected void lbReset_Click(object sender, EventArgs e)
        {
            DisposeState(PRODUCT_ID_FILTER);
            DisposeState(NAME_FILTER);
            DisposeState("customlabel1filter");
            DisposeState("customlabel2filter");
            DisposeState("customlabel3filter");
            DisposeState("customlabel4filter");
            DisposeState("customlabel1filter");
            DisposeState("value1filter");
            DisposeState("value2filter");
            DisposeState("value3filter");
            DisposeState("value4filter");
            DisposeState("value5filter");

            enbNotice.Message = string.Empty;
            LoadGroups();
        }

        protected void lbUpdateSelected_Click(object sender, EventArgs e)
        {
            SaveLastViewedGroups();

            // Convert to entity objects
            var overviewGroups = ChosenCustomLabelGroups;
            var groups = new List<ProductGoogleCustomLabelGroupMapping>();
            foreach (var item in overviewGroups)
            {
                groups.Add(new ProductGoogleCustomLabelGroupMapping
                {
                    ProductId = item.ProductId,
                    CustomLabel1 = item.CustomLabel1,
                    CustomLabel2 = item.CustomLabel2,
                    CustomLabel3 = item.CustomLabel3,
                    CustomLabel4 = item.CustomLabel4,
                    CustomLabel5 = item.CustomLabel5,
                    Value1 = item.Value1,
                    Value2 = item.Value2,
                    Value3 = item.Value3,
                    Value4 = item.Value4,
                    Value5 = item.Value5
                });
            }

            ProductService.SaveProductGoogleCustomLabelGroups(groups);
            enbNotice.Message = "Items were updated successfully";
            ChosenCustomLabelGroups.Clear();
            NotChosenCustomLabelGroups.Clear();
            LoadGroups();
        }

        protected void ddlFilterChosen_PreRender(object sender, EventArgs e)
        {
            DropDownList ddl = (DropDownList)sender;
            ddl.SelectedIndex = -1;
        }

        protected void btnGoPage_Click(object sender, EventArgs e)
        {
            int gotoIndex = Convert.ToInt32(((TextBox)gvCustomLabels.TopPagerRow.FindControl("txtPageIndex")).Text.Trim()) - 1;

            if ((gvCustomLabels.CustomPageCount > gotoIndex) && (gotoIndex >= 0))
                gvCustomLabels.CustomPageIndex = gotoIndex;

            enbNotice.Message = string.Empty;
            LoadGroups();
        }

        protected void gvCustomLabels_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                var group = (ProductGoogleCustomLabelGroupMappingOverviewModel)e.Row.DataItem;
                ProductGoogleCustomLabelGroupMappingOverviewModel chosenGroup = null;

                if (ExistInChosenGroups(group))
                {
                    chosenGroup = ChosenCustomLabelGroups.Find(delegate (ProductGoogleCustomLabelGroupMappingOverviewModel item)
                    {
                        return item.ProductId == group.ProductId;
                    });
                }

                CheckBox cb = e.Row.FindControl("cbChosen") as CheckBox;

                if (chosenGroup != null)
                {
                    TextBox txtCustomLabel1 = e.Row.FindControl("txtCustomLabel1") as TextBox;
                    TextBox txtCustomLabel2 = e.Row.FindControl("txtCustomLabel2") as TextBox;
                    TextBox txtCustomLabel3 = e.Row.FindControl("txtCustomLabel3") as TextBox;
                    TextBox txtCustomLabel4 = e.Row.FindControl("txtCustomLabel4") as TextBox;
                    TextBox txtCustomLabel5 = e.Row.FindControl("txtCustomLabel5") as TextBox;

                    TextBox txtValue1 = e.Row.FindControl("txtValue1") as TextBox;
                    TextBox txtValue2 = e.Row.FindControl("txtValue2") as TextBox;
                    TextBox txtValue3 = e.Row.FindControl("txtValue3") as TextBox;
                    TextBox txtValue4 = e.Row.FindControl("txtValue4") as TextBox;
                    TextBox txtValue5 = e.Row.FindControl("txtValue5") as TextBox;

                    cb.Checked = true;

                    txtCustomLabel1.Text = chosenGroup.CustomLabel1;
                    txtCustomLabel2.Text = chosenGroup.CustomLabel2;
                    txtCustomLabel3.Text = chosenGroup.CustomLabel3;
                    txtCustomLabel4.Text = chosenGroup.CustomLabel4;
                    txtCustomLabel5.Text = chosenGroup.CustomLabel5;

                    txtValue1.Text = chosenGroup.Value1;
                    txtValue2.Text = chosenGroup.Value2;
                    txtValue3.Text = chosenGroup.Value3;
                    txtValue4.Text = chosenGroup.Value4;
                    txtValue5.Text = chosenGroup.Value5;
                }
            }
        }

        protected void gvCustomLabels_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            int productId = 0;
            switch (e.CommandName)
            {
                default:
                case "save":
                    productId = Convert.ToInt32(e.CommandArgument);
                    var row = ((GridViewRow)((Control)e.CommandSource).NamingContainer);
                    var txtCustomLabel1 = row.FindControl("txtCustomLabel1") as TextBox;
                    var txtCustomLabel2 = row.FindControl("txtCustomLabel2") as TextBox;
                    var txtCustomLabel3 = row.FindControl("txtCustomLabel3") as TextBox;
                    var txtCustomLabel4 = row.FindControl("txtCustomLabel4") as TextBox;
                    var txtCustomLabel5 = row.FindControl("txtCustomLabel5") as TextBox;

                    var txtValue1 = row.FindControl("txtValue1") as TextBox;
                    var txtValue2 = row.FindControl("txtValue2") as TextBox;
                    var txtValue3 = row.FindControl("txtValue3") as TextBox;
                    var txtValue4 = row.FindControl("txtValue4") as TextBox;
                    var txtValue5 = row.FindControl("txtValue5") as TextBox;

                    var updatedGroup = new ProductGoogleCustomLabelGroupMapping
                    {
                        ProductId = productId,
                        CustomLabel1 = string.IsNullOrEmpty(txtCustomLabel1.Text.Trim()) ? null : txtCustomLabel1.Text.Trim(),
                        CustomLabel2 = string.IsNullOrEmpty(txtCustomLabel2.Text.Trim()) ? null : txtCustomLabel2.Text.Trim(),
                        CustomLabel3 = string.IsNullOrEmpty(txtCustomLabel3.Text.Trim()) ? null : txtCustomLabel3.Text.Trim(),
                        CustomLabel4 = string.IsNullOrEmpty(txtCustomLabel4.Text.Trim()) ? null : txtCustomLabel4.Text.Trim(),
                        CustomLabel5 = string.IsNullOrEmpty(txtCustomLabel5.Text.Trim()) ? null : txtCustomLabel5.Text.Trim(),

                        Value1 = string.IsNullOrEmpty(txtValue1.Text.Trim()) ? null : txtValue1.Text.Trim(),
                        Value2 = string.IsNullOrEmpty(txtValue2.Text.Trim()) ? null : txtValue2.Text.Trim(),
                        Value3 = string.IsNullOrEmpty(txtValue3.Text.Trim()) ? null : txtValue3.Text.Trim(),
                        Value4 = string.IsNullOrEmpty(txtValue4.Text.Trim()) ? null : txtValue4.Text.Trim(),
                        Value5 = string.IsNullOrEmpty(txtValue5.Text.Trim()) ? null : txtValue5.Text.Trim()
                    };

                    ProductService.SaveProductGoogleCustomLabelGroup(updatedGroup);

                    enbNotice.Message = "The row was updated successfully.";
                    break;
                case "clear":
                    var emptyGroup = new ProductGoogleCustomLabelGroupMapping
                    {
                        ProductId = Convert.ToInt32(e.CommandArgument)
                    };

                    ProductService.SaveProductGoogleCustomLabelGroup(emptyGroup);

                    enbNotice.Message = "The row was updated successfully.";
                    break;
            }

            LoadGroups();
        }

        protected void gvCustomLabels_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            SaveLastViewedGroups();

            gvCustomLabels.CustomPageIndex = gvCustomLabels.CustomPageIndex + e.NewPageIndex;

            if (gvCustomLabels.CustomPageIndex < 0)
                gvCustomLabels.CustomPageIndex = 0;

            LoadGroups();
        }

        protected void gvCustomLabels_PreRender(object sender, EventArgs e)
        {
            if (gvCustomLabels.TopPagerRow != null)
            {
                gvCustomLabels.TopPagerRow.Visible = true;
                ((TextBox)gvCustomLabels.HeaderRow.FindControl("txtFilterProductId")).Text = GetStringState(PRODUCT_ID_FILTER);
                ((TextBox)gvCustomLabels.HeaderRow.FindControl("txtFilterName")).Text = GetStringState(NAME_FILTER);
                ((TextBox)gvCustomLabels.HeaderRow.FindControl("txtFilterCustomLabel1")).Text = GetStringState("customelabel1filter");
                ((TextBox)gvCustomLabels.HeaderRow.FindControl("txtFilterCustomLabel2")).Text = GetStringState("customelabel2filter");
                ((TextBox)gvCustomLabels.HeaderRow.FindControl("txtFilterCustomLabel3")).Text = GetStringState("customelabel3filter");
                ((TextBox)gvCustomLabels.HeaderRow.FindControl("txtFilterCustomLabel4")).Text = GetStringState("customelabel4filter");
                ((TextBox)gvCustomLabels.HeaderRow.FindControl("txtFilterCustomLabel5")).Text = GetStringState("customelabel5filter");
            }

            for (int i = 0; i < gvCustomLabels.Rows.Count; i++)
            {
                CheckBox cb = gvCustomLabels.Rows[i].FindControl("cbChosen") as CheckBox;
                var group = BuildGroup(Convert.ToInt32(gvCustomLabels.DataKeys[i].Value), gvCustomLabels.Rows[i]);

                if (cb != null) SetChosenGroups(group, cb.Checked);
            }

            if (gvCustomLabels.Rows.Count == 1 && (int)gvCustomLabels.DataKeys[0].Value == 0 && gvCustomLabels.TopPagerRow != null)
            {
                gvCustomLabels.TopPagerRow.FindControl(PH_RECORD_FOUND).Visible = false;
                gvCustomLabels.TopPagerRow.FindControl(PH_RECORD_NOT_FOUND).Visible = true;
            }
        }

        private void SaveLastViewedGroups()
        {
            for (int i = 0; i < gvCustomLabels.DataKeys.Count; i++)
            {
                CheckBox cb = gvCustomLabels.Rows[i].FindControl("cbChosen") as CheckBox;

                var group = BuildGroup(Convert.ToInt32(gvCustomLabels.DataKeys[i].Value), gvCustomLabels.Rows[i]);
                
                if (cb != null) SetChosenGroups(group, cb.Checked);
            }
        }

        private bool ExistInChosenGroups(ProductGoogleCustomLabelGroupMappingOverviewModel group)
        {
            bool exist = false;

            for (int j = 0; j < ChosenCustomLabelGroups.Count; j++)
            {
                exist = IsEqual(ChosenCustomLabelGroups[j], group);

                if (!exist) exist = ChosenCustomLabelGroups[j].ProductId == group.ProductId;

                if (exist) break;
            }

            return exist;
        }

        private bool ExistInNotChosenGroups(ProductGoogleCustomLabelGroupMappingOverviewModel group)
        {
            bool exist = false;

            for (int j = 0; j < NotChosenCustomLabelGroups.Count; j++)
            {
                exist = IsEqual(NotChosenCustomLabelGroups[j], group);

                if (!exist) exist = NotChosenCustomLabelGroups[j].ProductId == group.ProductId;

                if (exist) break;
            }

            return exist;
        }

        private bool IsEqual(ProductGoogleCustomLabelGroupMappingOverviewModel item1, ProductGoogleCustomLabelGroupMappingOverviewModel item2)
        {
            PropertyInfo[] properties1 = item1.GetType().GetProperties();
            PropertyInfo[] properties2 = item2.GetType().GetProperties();
            object value1;
            object value2;

            Type type = item1.GetType();
            bool isEqual = false;

            for (int i = 0; i < properties1.Length; i++)
            {
                value1 = properties1[i].GetValue(item1, null);
                value2 = properties2[i].GetValue(item2, null);

                if (value1 != null && value2 != null)
                    isEqual = value1.Equals(value2);

                if (!isEqual)
                    break;
            }

            return isEqual;
        }

        private void SetChosenGroups(ProductGoogleCustomLabelGroupMappingOverviewModel group, bool chosen)
        {
            if (group != null)
            {
                if (chosen)
                {
                    if (ExistInChosenGroups(group))
                    {
                        ChosenCustomLabelGroups.RemoveAll(delegate (ProductGoogleCustomLabelGroupMappingOverviewModel arg)
                        {
                            return arg.ProductId == group.ProductId;
                        });
                    }

                    ChosenCustomLabelGroups.Add(group);
                    NotChosenCustomLabelGroups.RemoveAll(delegate (ProductGoogleCustomLabelGroupMappingOverviewModel arg)
                    {
                        return arg.ProductId == group.ProductId;
                    });
                }
                else
                {
                    ChosenCustomLabelGroups.RemoveAll(delegate (ProductGoogleCustomLabelGroupMappingOverviewModel arg)
                    {
                        return arg.ProductId == group.ProductId;
                    });

                    if (ExistInNotChosenGroups(group))
                    {
                        NotChosenCustomLabelGroups.RemoveAll(delegate (ProductGoogleCustomLabelGroupMappingOverviewModel arg)
                        {
                            return arg.ProductId == group.ProductId;
                        });
                    }

                    NotChosenCustomLabelGroups.Add(group);
                }
            }
        }

        private ProductGoogleCustomLabelGroupMappingOverviewModel BuildGroup(int productId, GridViewRow row)
        {
            TextBox txtCustomLabel1 = row.FindControl("txtCustomLabel1") as TextBox;
            TextBox txtCustomLabel2 = row.FindControl("txtCustomLabel2") as TextBox;
            TextBox txtCustomLabel3 = row.FindControl("txtCustomLabel3") as TextBox;
            TextBox txtCustomLabel4 = row.FindControl("txtCustomLabel4") as TextBox;
            TextBox txtCustomLabel5 = row.FindControl("txtCustomLabel5") as TextBox;

            TextBox txtValue1 = row.FindControl("txtValue1") as TextBox;
            TextBox txtValue2 = row.FindControl("txtValue2") as TextBox;
            TextBox txtValue3 = row.FindControl("txtValue3") as TextBox;
            TextBox txtValue4 = row.FindControl("txtValue4") as TextBox;
            TextBox txtValue5 = row.FindControl("txtValue5") as TextBox;

            var overviewGroup = new ProductGoogleCustomLabelGroupMappingOverviewModel();

            overviewGroup.ProductId = productId;

            if (string.IsNullOrEmpty(txtCustomLabel1.Text.Trim()) == false)
                overviewGroup.CustomLabel1 = txtCustomLabel1.Text.Trim();

            if (string.IsNullOrEmpty(txtCustomLabel2.Text.Trim()) == false)
                overviewGroup.CustomLabel2 = txtCustomLabel2.Text.Trim();

            if (string.IsNullOrEmpty(txtCustomLabel3.Text.Trim()) == false)
                overviewGroup.CustomLabel3 = txtCustomLabel3.Text.Trim();

            if (string.IsNullOrEmpty(txtCustomLabel4.Text.Trim()) == false)
                overviewGroup.CustomLabel4 = txtCustomLabel4.Text.Trim();

            if (string.IsNullOrEmpty(txtCustomLabel5.Text.Trim()) == false)
                overviewGroup.CustomLabel5 = txtCustomLabel5.Text.Trim();

            if (string.IsNullOrEmpty(txtValue1.Text.Trim()) == false)
                overviewGroup.Value1 = txtValue1.Text.Trim();
                
            if (string.IsNullOrEmpty(txtValue2.Text.Trim()) == false)
                overviewGroup.Value2 = txtValue2.Text.Trim();

            if (string.IsNullOrEmpty(txtValue3.Text.Trim()) == false)
                overviewGroup.Value3 = txtValue3.Text.Trim();

            if (string.IsNullOrEmpty(txtValue4.Text.Trim()) == false)
                overviewGroup.Value4 = txtValue4.Text.Trim();

            if (string.IsNullOrEmpty(txtValue5.Text.Trim()) == false)
                overviewGroup.Value5 = txtValue5.Text.Trim();

            return overviewGroup;
        }

        private void LoadGroups()
        {
            int[] productIds = null;
            string name = null;
            string customLabel1 = null;
            string customLabel2 = null;
            string customLabel3 = null;
            string customLabel4 = null;
            string customLabel5 = null;
            string value1 = null;
            string value2 = null;
            string value3 = null;
            string value4 = null;
            string value5 = null;

            if (HasState(PRODUCT_ID_FILTER))
            {
                string value = GetStringState(PRODUCT_ID_FILTER);
                int temp;
                productIds = value.Split(',')
                      .Where(x => int.TryParse(x.ToString(), out temp))
                      .Select(x => int.Parse(x))
                      .ToArray();
            }
            if (HasState(NAME_FILTER)) name = GetStringState(NAME_FILTER);
            if (HasState("customlabel1filter")) customLabel1 = GetStringState("customlabel1filter");
            if (HasState("customlabel2filter")) customLabel2 = GetStringState("customlabel2filter");
            if (HasState("customlabel3filter")) customLabel3 = GetStringState("customlabel3filter");
            if (HasState("customlabel4filter")) customLabel4 = GetStringState("customlabel4filter");
            if (HasState("customlabel5filter")) customLabel5 = GetStringState("customlabel5filter");

            if (HasState("value1filter")) value1 = GetStringState("value1filter");
            if (HasState("value2filter")) value2 = GetStringState("value2filter");
            if (HasState("value3filter")) value3 = GetStringState("value3filter");
            if (HasState("value4filter")) value4 = GetStringState("value4filter");
            if (HasState("value5filter")) value5 = GetStringState("value5filter");
            
            var result = ProductService.GetProductGoogleCustomLabelGroupLoadPaged(
                pageIndex: gvCustomLabels.CustomPageIndex,
                pageSize: gvCustomLabels.PageSize,
                productIds: productIds,
                name: name,
                customLabel1: customLabel1,
                customLabel2: customLabel2,
                customLabel3: customLabel3,
                customLabel4: customLabel4,
                customLabel5: customLabel5,
                value1: value1,
                value2: value2,
                value3: value3,
                value4: value4,
                value5: value5);

            if (result != null)
            {
                gvCustomLabels.DataSource = result.Items;
                gvCustomLabels.RecordCount = result.TotalCount;
                gvCustomLabels.CustomPageCount = result.TotalPages;
            }

            gvCustomLabels.DataBind();

            if (gvCustomLabels.Rows.Count <= 0) enbNotice.Message = "No records found.";
        }        
    }
}