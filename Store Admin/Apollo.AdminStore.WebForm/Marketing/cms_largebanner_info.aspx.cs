using Apollo.AdminStore.WebForm.Classes;
using Apollo.Core.Domain.Media;
using Apollo.Core.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Web.UI.WebControls;

namespace Apollo.AdminStore.WebForm.Marketing
{
    public partial class cms_largebanner_info : BasePage
    {
        public ICampaignService CampaignService { get; set; }
        public ICategoryService CategoryService { get; set; }
        public MediaSettings MediaSettings { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
                LoadBannerInfo();
        }
        
        protected void lbDelete_Click(object sender, EventArgs e)
        {
            var item = CampaignService.GetLargeBannerById(QueryId);            
            var filePath = MediaSettings.LargeBannerLocalPath + item.MediaFilename;

            if (File.Exists(filePath)) File.Delete(filePath);
            CampaignService.DeleteLargeBanner(item.Id);
            Response.Redirect("/marketing/cms_largebanner_default.aspx?" + QueryKey.MSG_TYPE + "=" + (int)MessageType.BannerDeleted);
        }

        protected void lbSaveContinue_Click(object sender, EventArgs e)
        {
            var banner = CampaignService.GetLargeBannerById(QueryId);
            
            banner.Title = txtTitle.Text.Trim();
            banner.Link = txtLink.Text.Trim();
            banner.Enabled = cbEnabled.Checked;
            banner.MediaAlt = txtTitle.Text.Trim();
            banner.Priority = Convert.ToInt32(txtPriority.Text.Trim());
            banner.DisplayOnHomePage = cbDisplayOnHomepage.Checked;
            banner.DisplayOnOffersPage = cbDisplayOnOffersPage.Checked;

            if (txtDateFrom.Text.Trim() != string.Empty)
                banner.StartDate = DateTime.ParseExact(txtDateFrom.Text, AppConstant.DATE_FORM1, CultureInfo.InvariantCulture);
            else
                banner.StartDate = null;

            if (txtDateTo.Text.Trim() != string.Empty)
                banner.EndDate = DateTime.ParseExact(txtDateTo.Text, AppConstant.DATE_FORM1, CultureInfo.InvariantCulture);
            else
                banner.EndDate = null;

            if (fuBanner.HasFile)
            {
                // Save image
                string filename = banner.Id.ToString() + Path.GetExtension(fuBanner.FileName).ToLower();
                string filePath = MediaSettings.LargeBannerLocalPath + filename;

                if (File.Exists(filePath)) File.Delete(filePath);
                fuBanner.SaveAs(filePath);
                banner.MediaFilename = filename;
            }

            CampaignService.UpdateLargeBanner(banner);
            LoadBannerInfo();
            enbNotice.Message = "Banner was updated successfully.";
        }

        protected void lbRemoveCategory_Click(object sender, EventArgs e)
        {
            int largeBannerId = QueryId;
            int categoryId = 0;

            var categories = CategoryService.GetCategoriesByLargeBannerId(largeBannerId);

            if (categories.Count <= 1)
            {
                enbNotice.Message = "Sorry, you must have at least one category.";
            }
            else if (int.TryParse(ddlCategorySelection.SelectedValue, out categoryId))
            {
                CategoryService.DeleteCategoryFromLargeBanner(categoryId, largeBannerId);
                enbNotice.Message = "Category was deleted successfully.";
                LoadBannerInfo();
            }
            else
                enbNotice.Message = "Sorry, there is no category to be deleted.";            
        }

        protected void lbSearchNewCategory_Click(object sender, EventArgs e)
        {
            int categoryId = 0;
            int.TryParse(hfCategory.Value, out categoryId);
            var treeList = CategoryService.GetTreeList(categoryId);
            ectCategory.FindSelectedNode(AppConstant.DEFAULT_CATEGORY, treeList);
            ectCategory.Visible = true;

            lbSearchNewCategory.Visible = false;
        }
        
        protected void lbAddNewCategory_Click(object sender, EventArgs e)
        {
            int largeBannerId = QueryId;

            if (hfCategory.Value != string.Empty)
            {
                int categoryId = Convert.ToInt32(hfCategory.Value);
                CategoryService.ProcessCategoryAssignmentForLargeBanner(categoryId, largeBannerId);

                enbNotice.Message = "Category was added successfully.";
            }

            lbSearchNewCategory.Visible = true;
            lbAddNewCategory.Visible = false;
            lbCancelCategory.Visible = false;

            hfCategory.Value = string.Empty;
            ltlCategory.Text = string.Empty;

            LoadBannerInfo();
        }

        protected void lbCancelCategory_Click(object sender, EventArgs e)
        {
            lbSearchNewCategory.Visible = true;
            lbAddNewCategory.Visible = false;
            lbCancelCategory.Visible = false;

            hfCategory.Value = string.Empty;
            ltlCategory.Text = string.Empty;

            LoadBannerInfo();
        }

        protected void ectCategory_TreeChanged(string categoryName, int categoryId)
        {
            LoadBannerInfo();
        }
        
        protected void ectCategory_TreeNodeSelected(string categoryName, int categoryId)
        {
            ltlCategory.Text = categoryName;
            hfCategory.Value = categoryId.ToString();
            ectCategory.Clear();

            lbAddNewCategory.Visible = true;
            lbCancelCategory.Visible = true;
            ectCategory.Visible = false;

            LoadBannerInfo();
        }

        private void LoadBannerInfo()
        {
            var banner = CampaignService.GetLargeBannerById(QueryId);

            if (banner != null)
            {
                ltlTitle.Text = string.Format("{0} (ID: {1})", banner.Title, banner.Id);
                txtTitle.Text = banner.Title;
                txtLink.Text = banner.Link;
                ltlBanner.Text = string.Format("<img src='/get_image_handler.aspx?{0}={1}&id={2}'/>", QueryKey.TYPE, ImageHandlerType.LARGE_BANNER, banner.Id);
                cbEnabled.Checked = banner.Enabled;
                txtPriority.Text = banner.Priority.ToString();
                cbDisplayOnHomepage.Checked = banner.DisplayOnHomePage;
                cbDisplayOnOffersPage.Checked = banner.DisplayOnOffersPage;

                if (banner.StartDate.HasValue)
                    txtDateFrom.Text = banner.StartDate.Value.ToString(AppConstant.DATE_FORM1);

                if (banner.EndDate.HasValue)
                    txtDateTo.Text = banner.EndDate.Value.ToString(AppConstant.DATE_FORM1);
                
                #region Category

                var assignedCategory = CategoryService.GetCategoriesByLargeBannerId(banner.Id);
                var listItems = new List<ListItem>();
                foreach (var item in assignedCategory)
                {
                    StringBuilder sb = new StringBuilder();
                    var tree = CategoryService.GetTreeList(item.Id);

                    foreach (var leaf in tree)
                    {
                        var leafCategory = CategoryService.GetCategory(leaf);
                        if (sb.ToString() != string.Empty) sb.Append(" > ");
                        if (leafCategory != null) sb.Append(Server.HtmlDecode(leafCategory.CategoryName) + (leafCategory.Visible ? null : " (hidden)"));
                    }

                    listItems.Add(new ListItem(Server.HtmlDecode(sb.ToString()), item.Id.ToString()));
                }

                ddlCategorySelection.Items.Clear();
                ddlCategorySelection.Items.AddRange(listItems.ToArray());
                ddlCategorySelection.DataBind();
                ddlCategorySelection.Visible = ddlCategorySelection.Items.Count != 0;
                
                #endregion
            }
        }
    }
}