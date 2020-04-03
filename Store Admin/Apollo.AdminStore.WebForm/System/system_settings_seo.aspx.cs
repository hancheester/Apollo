using Apollo.AdminStore.WebForm.Classes;
using Apollo.Core.Domain.Seo;
using Apollo.Core.Model;
using Apollo.Core.Services.Interfaces;
using System;

namespace Apollo.AdminStore.WebForm.System
{
    public partial class system_settings_seo : BasePage
    {
        public IUtilityService UtilityService { get; set; }
        public ISettingService SettingService { get; set; }

        protected override void OnInit(EventArgs e)
        {
            ddlPageTitleSeoAdjustments.Items.AddRange(PageTitleSeoAdjustment.PagenameAfterStorename.ToListItemArray());
            ddlWwwRequirements.Items.AddRange(WwwRequirement.NoMatter.ToListItemArray());
            
            base.OnInit(e);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                var setting = SettingService.LoadSetting<SeoSettings>();
                txtPageTitleSeparator.Text = setting.PageTitleSeparator;
                var seoAdjust = ddlPageTitleSeoAdjustments.Items.FindByValue(setting.PageTitleSeoAdjustment.ToString());
                if (seoAdjust != null) seoAdjust.Selected = true;
                txtDefaultTitle.Text = setting.DefaultTitle;
                txtDefaultMetaKeywords.Text = setting.DefaultMetaKeywords;
                txtDefaultMetaDescription.Text = setting.DefaultMetaDescription;
                cbGenerateProductMetaDescription.Checked = setting.GenerateProductMetaDescription;
                cbConvertNonWesternChars.Checked = setting.ConvertNonWesternChars;
                cbAllowUnicodeCharsInUrls.Checked = setting.AllowUnicodeCharsInUrls;
                cbCanonicalUrlsEnabled.Checked = setting.CanonicalUrlsEnabled;
                var wwwReq = ddlWwwRequirements.Items.FindByValue(setting.WwwRequirement.ToString());
                if (wwwReq != null) wwwReq.Selected = true;
                cbEnableJsBundling.Checked = setting.EnableJsBundling;
                cbEnableCssBundling.Checked = setting.EnableCssBundling;
                cbTwitterMetaTags.Checked = setting.TwitterMetaTags;
                cbOpenGraphMetaTags.Checked = setting.OpenGraphMetaTags;
            }
        }

        protected void lbUpdate_Click(object sender, EventArgs e)
        {
            var setting = SettingService.LoadSetting<SeoSettings>();
            setting.PageTitleSeparator = txtPageTitleSeparator.Text;
            setting.PageTitleSeoAdjustment = (PageTitleSeoAdjustment)Convert.ToInt32(ddlPageTitleSeoAdjustments.SelectedValue);
            setting.DefaultTitle = txtDefaultTitle.Text.Trim();
            setting.DefaultMetaKeywords = txtDefaultMetaKeywords.Text.Trim();
            setting.DefaultMetaDescription = txtDefaultMetaDescription.Text.Trim();
            setting.GenerateProductMetaDescription = cbGenerateProductMetaDescription.Checked;
            setting.ConvertNonWesternChars = cbConvertNonWesternChars.Checked;
            setting.AllowUnicodeCharsInUrls = cbAllowUnicodeCharsInUrls.Checked;
            setting.CanonicalUrlsEnabled = cbCanonicalUrlsEnabled.Checked;
            setting.WwwRequirement = (WwwRequirement)Convert.ToInt32(ddlWwwRequirements.SelectedValue);
            setting.EnableJsBundling = cbEnableJsBundling.Checked;
            setting.EnableCssBundling = cbEnableCssBundling.Checked;
            setting.TwitterMetaTags = cbTwitterMetaTags.Checked;
            setting.OpenGraphMetaTags = cbOpenGraphMetaTags.Checked;

            SettingService.SaveSetting(setting);

            enbNotice.Message = "SEO settings were updated successfully.";
        }

        protected void lbPublish_Click(object sender, EventArgs e)
        {
            var result = UtilityService.RefreshCache(CacheEntityKey.Setting);

            if (result)
                enbNotice.Message = "All settings related data on store front has been refreshed successfully.";
            else
                enbNotice.Message = "Failed to refresh data on store front. Please contact administrator for help.";
        }
    }
}