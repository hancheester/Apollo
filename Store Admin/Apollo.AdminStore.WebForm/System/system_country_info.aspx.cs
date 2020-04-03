using Apollo.AdminStore.WebForm.Classes;
using Apollo.Core.Services.Interfaces;
using System;

namespace Apollo.AdminStore.WebForm.System
{
    public partial class system_country_info : BasePage
    {
        public IShippingService ShippingService { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
                LoadInfo();
        }
        
        protected void lbSaveContinue_Click(object sender, EventArgs e)
        {
            var country = ShippingService.GetCountryById(QueryId);
            country.Name = txtName.Text.Trim();
            country.ISO3166Code = txtISO3166Code.Text.Trim();
            country.IsEC = cbIsEC.Checked;
            country.Enabled = cbEnabled.Checked;
            country.UpdatedOnDate = DateTime.Now;

            ShippingService.UpdateCountry(country);
            
            enbNotice.Message = "Country was updated successfully.";            
        }
        
        private void LoadInfo()
        {
            var country = ShippingService.GetCountryById(QueryId);

            if (country != null)
            {
                txtName.Text = Convert.ToString(country.Name);
                txtISO3166Code.Text = Convert.ToString(country.ISO3166Code);
                cbIsEC.Checked = country.IsEC;
                cbEnabled.Checked = country.Enabled;
            }
        }
    }
}