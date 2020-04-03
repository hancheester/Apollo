using Apollo.AdminStore.WebForm.Classes;
using Apollo.Core.Domain.Tax;
using Apollo.Core.Model;
using Apollo.Core.Services.Interfaces;
using System;

namespace Apollo.AdminStore.WebForm.System
{
    public partial class system_settings_tax : BasePage
    {
        public IUtilityService UtilityService { get; set; }
        public ISettingService SettingService { get; set; }

        protected override void OnInit(EventArgs e)
        {
            ddlTaxDisplayTypes.Items.AddRange(TaxDisplayType.ExcludingTax.ToListItemArray());
            base.OnInit(e);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                var settings = SettingService.LoadSetting<TaxSettings>();                
                cbPricesIncludeTax.Checked = settings.PricesIncludeTax;
                var type = ddlTaxDisplayTypes.Items.FindByText(settings.TaxDisplayType.ToString());
                if (type != null) type.Selected = true;
            }
        }

        protected void lbUpdate_Click(object sender, EventArgs e)
        {
            var settings = SettingService.LoadSetting<TaxSettings>();
            settings.PricesIncludeTax = cbPricesIncludeTax.Checked;
            settings.TaxDisplayType = (TaxDisplayType)Convert.ToInt32(ddlTaxDisplayTypes.SelectedValue);

            SettingService.SaveSetting(settings);

            enbNotice.Message = "Tax settings were updated successfully.";
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