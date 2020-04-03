using Apollo.AdminStore.WebForm.Classes;
using Apollo.Core.Model.Entity;
using Apollo.Core.Services.Interfaces;
using System;

namespace Apollo.AdminStore.WebForm.System
{
    public partial class system_country_new : BasePage
    {
        public IShippingService ShippingService { get; set; }
      
        protected void lbSave_Click(object sender, EventArgs e)
        {
            var country = new Country
            {
                Name = txtName.Text.Trim(),
                ISO3166Code = txtISO3166Code.Text.Trim(),
                IsEC = cbIsEC.Checked,
                Enabled = cbEnabled.Checked
            };
            
            int id = ShippingService.InsertCountry(country);
            Response.Redirect("/system/system_country_info.aspx?" + QueryKey.MSG_TYPE + "=" + (int)MessageType.CountryCreated + "&" + QueryKey.ID + "=" + id.ToString());
        }
    }
}