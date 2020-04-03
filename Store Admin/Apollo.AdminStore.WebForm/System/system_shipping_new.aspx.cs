using Apollo.AdminStore.WebForm.Classes;
using Apollo.Core.Model.Entity;
using Apollo.Core.Services.Interfaces;
using System;
using System.Web.UI.WebControls;

namespace Apollo.AdminStore.WebForm.System
{
    public partial class system_shipping_new : BasePage
    {
        public IShippingService ShippingService { get; set; }
        
        protected void ddlCountry_Init(object sender, EventArgs e)
        {
            ddlCountry.DataSource = ShippingService.GetActiveCountries();
            ddlCountry.DataTextField = "Name";
            ddlCountry.DataValueField = "Id";
            ddlCountry.DataBind();
            ddlCountry.Items.Insert(0, new ListItem(AppConstant.DEFAULT_SELECT, string.Empty));
        }

        protected void lbSave_Click(object sender, EventArgs e)
        {
            var option = new ShippingOption();
            option.Name = txtName.Text;
            option.Description = txtDescription.Text;
            option.Value = Convert.ToDecimal(txtValue.Text);
            option.FreeThreshold = Convert.ToDecimal(txtFreeThreshold.Text);
            option.SingleItemValue = Convert.ToDecimal(txtSingleItemValue.Text);
            option.UpToOneKg = Convert.ToDecimal(txtUptoOneKG.Text);
            option.UpToOneHalfKg = Convert.ToDecimal(txtUptoOneAndHalfKG.Text);
            option.UpToTwoKg = Convert.ToDecimal(txtUptoTwoKG.Text);
            option.UpToTwoHalfKg = Convert.ToDecimal(txtUptoTwoAndHalfKG.Text);
            option.UpToThreeKg = Convert.ToDecimal(txtUptoThreeKG.Text);
            option.UpToThreeHalfKg = Convert.ToDecimal(txtUptoThreeAndHalfKG.Text);
            option.UpToFourKg = Convert.ToDecimal(txtUptoFourKG.Text);
            option.UpToFourHalfKg = Convert.ToDecimal(txtUptoFourAndHalfKG.Text);
            option.UpToFiveKg = Convert.ToDecimal(txtUptoFiveKG.Text);
            option.HalfKgRate = Convert.ToDecimal(txtHalfKGRate.Text);
            option.Enabled = cbEnabled.Checked;
            option.Priority = Convert.ToInt32(txtPriority.Text);
            option.CountryId = Convert.ToInt32(ddlCountry.SelectedValue);
            option.Timeline = txtTimeline.Text.Trim();

            var id = ShippingService.InsertShippingOption(option);

            Response.Redirect("/system/system_shipping_info.aspx?" + QueryKey.ID + "=" + id + "&" + QueryKey.MSG_TYPE + "=" + (int)MessageType.ShippingOptionCreated);
        }
    }
}