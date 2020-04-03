using Apollo.AdminStore.WebForm.Classes;
using Apollo.Core.Domain.Media;
using Apollo.Core.Services.Interfaces;
using System;
using System.Globalization;
using System.IO;

namespace Apollo.AdminStore.WebForm.Marketing
{
    public partial class cms_offerbanner_info : BasePage
    {
        public ICampaignService CampaignService { get; set; }
        public MediaSettings MediaSettings { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
                LoadBannerInfo();}
        
        protected void lbDelete_Click(object sender, EventArgs e)
        {
            var item = CampaignService.GetOfferBannerById(QueryId);
            var filePath = MediaSettings.OfferBannerLocalPath + item.MediaFilename;

            if (File.Exists(filePath)) File.Delete(filePath);
            CampaignService.DeleteOfferBanner(item.Id);
            Response.Redirect("/marketing/cms_offerbanner_default.aspx?" + QueryKey.MSG_TYPE + "=" + (int)MessageType.BannerDeleted);
        }

        protected void lbSaveContinue_Click(object sender, EventArgs e)
        {
            var banner = CampaignService.GetOfferBannerById(QueryId);

            banner.Title = txtTitle.Text.Trim();
            banner.Link = txtLink.Text.Trim();
            banner.Enabled = cbEnabled.Checked;
            banner.MediaAlt = txtTitle.Text.Trim();
            banner.Priority = Convert.ToInt32(txtPriority.Text.Trim());

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
                string filePath = MediaSettings.OfferBannerLocalPath + filename;

                if (File.Exists(filePath)) File.Delete(filePath);
                fuBanner.SaveAs(filePath);
                banner.MediaFilename = filename;
            }

            CampaignService.UpdateOfferBanner(banner);
            enbNotice.Message = "Banner was updated successfully.";            
        }

        private void LoadBannerInfo()
        {
            var banner = CampaignService.GetOfferBannerById(QueryId);

            if (banner != null)
            {
                ltlTitle.Text = string.Format("{0} (ID: {1})", banner.Title, QueryId);
                txtTitle.Text = banner.Title;
                txtLink.Text = banner.Link;
                ltlBanner.Text = string.Format("<img src='/get_image_handler.aspx?" + QueryKey.TYPE + "=" + ImageHandlerType.OFFER_BANNER + "&img={0}'/>", banner.MediaFilename);
                cbEnabled.Checked = banner.Enabled;
                txtPriority.Text = banner.Priority.ToString();
                
                if (banner.StartDate.HasValue)
                    txtDateFrom.Text = banner.StartDate.Value.ToString(AppConstant.DATE_FORM1);

                if (banner.EndDate.HasValue)
                    txtDateTo.Text = banner.EndDate.Value.ToString(AppConstant.DATE_FORM1);
            }
        }
    }
}