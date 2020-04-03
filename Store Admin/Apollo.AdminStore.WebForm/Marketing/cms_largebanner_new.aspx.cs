using Apollo.AdminStore.WebForm.Classes;
using Apollo.Core.Domain.Media;
using Apollo.Core.Model.Entity;
using Apollo.Core.Services.Interfaces;
using System;
using System.Globalization;
using System.IO;

namespace Apollo.AdminStore.WebForm.Marketing
{
    public partial class cms_largebanner_new : BasePage
    {
        public ICampaignService CampaignService { get; set; }
        public MediaSettings MediaSettings { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
        }

        protected void lbSave_Click(object sender, EventArgs e)
        {
            var newBanner = new LargeBanner {
                Title = txtTitle.Text.Trim(),
                Link = txtLink.Text.Trim(),
                Enabled = cbEnabled.Checked,
                MediaAlt = txtTitle.Text.Trim(),
                Priority = Convert.ToInt32(txtPriority.Text.Trim()),
                DisplayOnHomePage = cbDisplayOnHomepage.Checked,
                DisplayOnOffersPage = cbDisplayOnOffersPage.Checked
            };
            
            if (txtDateFrom.Text.Trim() != string.Empty)
                newBanner.StartDate = DateTime.ParseExact(txtDateFrom.Text, AppConstant.DATE_FORM1, CultureInfo.InvariantCulture);

            if (txtDateTo.Text.Trim() != string.Empty)
                newBanner.EndDate = DateTime.ParseExact(txtDateTo.Text, AppConstant.DATE_FORM1, CultureInfo.InvariantCulture);
            
            newBanner.Id = CampaignService.InsertLargeBanner(newBanner);

            // Save image
            string filename = newBanner.Id.ToString() + Path.GetExtension(fuMedia.FileName).ToLower();
            string filePath = MediaSettings.LargeBannerLocalPath + filename;

            if (File.Exists(filePath)) File.Delete(filePath);
            fuMedia.SaveAs(filePath);

            // Update media
            
            newBanner.MediaFilename = filename;
            CampaignService.UpdateLargeBanner(newBanner);

            Response.Redirect("/marketing/cms_largebanner_info.aspx?" + QueryKey.MSG_TYPE + "=" + (int)MessageType.BannerCreated + "&" + QueryKey.ID + "=" + newBanner.Id.ToString());
        }
    }
}