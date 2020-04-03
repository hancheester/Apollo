using Apollo.AdminStore.WebForm.Classes;
using Apollo.Core.Domain.Media;
using Apollo.Core.Model;
using Apollo.Core.Services.Interfaces;
using System;

namespace Apollo.AdminStore.WebForm.System
{
    public partial class system_settings_media : BasePage
    {
        public IUtilityService UtilityService { get; set; }
        public ISettingService SettingService { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                var setting = SettingService.LoadSetting<MediaSettings>();
                txtBrandMediaPath.Text = setting.BrandMediaPath;
                txtBrandMediaLocalPath.Text = setting.BrandMediaLocalPath;
                txtCategoryMediaPath.Text = setting.CategoryMediaPath;
                txtCategoryMediaLocalPath.Text = setting.CategoryMediaLocalPath;
                txtOfferMediaPath.Text = setting.OfferMediaPath;
                txtOfferMediaLocalPath.Text = setting.OfferMediaLocalPath;
                txtProductMediaPath.Text = setting.ProductMediaPath;
                txtProductMediaLocalPath.Text = setting.ProductMediaLocalPath;
                txtProductColourPath.Text = setting.ProductColourPath;
                txtProductColourLocalPath.Text = setting.ProductColourLocalPath;
                txtMediumBannerPath.Text = setting.MediumBannerPath;
                txtMediumBannerLocalPath.Text = setting.MediumBannerLocalPath;
                txtMiniBannerPath.Text = setting.MiniBannerPath;
                txtMiniBannerLocalPath.Text = setting.MiniBannerLocalPath;
                txtLargeBannerPath.Text = setting.LargeBannerPath;
                txtLargeBannerLocalPath.Text = setting.LargeBannerLocalPath;
                txtOfferBannerPath.Text = setting.OfferBannerPath;
                txtOfferBannerLocalPath.Text = setting.OfferBannerLocalPath;
                txtNoImagePath.Text = setting.NoImagePath;
                txtNoImageLocalPath.Text = setting.NoImageLocalPath;
                txtLargeLogoLink.Text = setting.LargeLogoLink;
            }
        }

        protected void lbUpdate_Click(object sender, EventArgs e)
        {
            var setting = SettingService.LoadSetting<MediaSettings>();
            setting.BrandMediaPath = txtBrandMediaPath.Text;
            setting.BrandMediaLocalPath = txtBrandMediaLocalPath.Text;
            setting.CategoryMediaPath = txtCategoryMediaPath.Text;
            setting.CategoryMediaLocalPath = txtCategoryMediaLocalPath.Text;
            setting.OfferMediaPath = txtOfferMediaPath.Text;
            setting.OfferMediaLocalPath = txtOfferMediaLocalPath.Text;
            setting.ProductMediaPath = txtProductMediaPath.Text;
            setting.ProductMediaLocalPath = txtProductMediaLocalPath.Text;
            setting.ProductColourPath = txtProductColourPath.Text;
            setting.ProductColourLocalPath  = txtProductColourLocalPath.Text;
            setting.MediumBannerPath = txtMediumBannerPath.Text;
            setting.MediumBannerLocalPath = txtMediumBannerLocalPath.Text;
            setting.MiniBannerPath = txtMiniBannerPath.Text;
            setting.MiniBannerLocalPath = txtMiniBannerLocalPath.Text;
            setting.LargeBannerPath = txtLargeBannerPath.Text;
            setting.LargeBannerLocalPath = txtLargeBannerLocalPath.Text;
            setting.OfferBannerPath = txtOfferBannerPath.Text;
            setting.OfferBannerLocalPath = txtOfferBannerLocalPath.Text;
            setting.NoImagePath = txtNoImagePath.Text;
            setting.NoImageLocalPath = txtNoImageLocalPath.Text;
            setting.LargeLogoLink = txtLargeLogoLink.Text;

            SettingService.SaveSetting(setting);

            enbNotice.Message = "Media settings were updated successfully.";
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