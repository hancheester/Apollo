using Apollo.AdminStore.WebForm.Classes;
using Apollo.Core.Model.Entity;
using Apollo.Core.Services.Interfaces;
using System;
using System.Web.UI.WebControls;

namespace Apollo.AdminStore.WebForm.System
{
    public partial class system_shipping_info : BasePage
    {
        public IShippingService ShippingService { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
                LoadInfo();
        }
        
        protected string GetCountryImage(int countryId)
        {
            const string FLAG_HTML_FORMAT = "<span class='flag-icon flag-icon-{0}' alt='{1}' title='{1}'></span> - {1}";
            var country = ShippingService.GetCountryById(countryId);

            if (country != null)
                return string.Format(FLAG_HTML_FORMAT, country.ISO3166Code.ToLower(), country.Name);

            return string.Empty;
        }
        
        protected void lbSaveContinue_Click(object sender, EventArgs e)
        {
            var option = new ShippingOption();
            option.Id = Convert.ToInt32(hdnShippingOptionId.Value);
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
            
            ShippingService.UpdateShippingOption(option);
            
            enbNotice.Message = "Shipping option was updated successfully.";
        }
        
        protected void lbDelete_Click(object sender, EventArgs e)
        {
            ShippingService.DeleteShippingOption(QueryId);
            Response.Redirect("/system/system_shipping_default.aspx?" + QueryKey.MSG_TYPE + "=" + (int)MessageType.ShippingOptionDeleted);
        }
        
        protected void ddlCountry_Init(object sender, EventArgs e)
        {
            ddlCountry.DataSource = ShippingService.GetCountries();
            ddlCountry.DataTextField = "Name";
            ddlCountry.DataValueField = "Id";
            ddlCountry.DataBind();
            ddlCountry.Items.Insert(0, new ListItem(AppConstant.DEFAULT_SELECT, string.Empty));
        }

        private void LoadInfo()
        {
            var option = ShippingService.GetShippingOptionById(QueryId);

            if (option != null)
            {
                ltlTitle.Text = string.Format("{0}, {1} (ID: {2})", GetCountryImage(option.CountryId), option.Name, option.Id);

                ddlCountry.SelectedValue = Convert.ToString(option.CountryId);
                txtName.Text = Convert.ToString(option.Name);
                txtDescription.Text = Convert.ToString(option.Description);
                txtValue.Text = Convert.ToString(option.Value);
                txtFreeThreshold.Text = Convert.ToString(option.FreeThreshold);
                txtSingleItemValue.Text = Convert.ToString(option.SingleItemValue);
                txtUptoOneKG.Text = Convert.ToString(option.UpToOneKg);
                txtUptoOneAndHalfKG.Text = Convert.ToString(option.UpToOneHalfKg);
                txtUptoTwoKG.Text = Convert.ToString(option.UpToTwoKg);
                txtUptoTwoAndHalfKG.Text = Convert.ToString(option.UpToTwoHalfKg);
                txtUptoThreeKG.Text = Convert.ToString(option.UpToThreeKg);
                txtUptoThreeAndHalfKG.Text = Convert.ToString(option.UpToThreeHalfKg);
                txtUptoFourKG.Text = Convert.ToString(option.UpToFourKg);
                txtUptoFourAndHalfKG.Text = Convert.ToString(option.UpToFourHalfKg);
                txtUptoFiveKG.Text = Convert.ToString(option.UpToFiveKg);
                txtHalfKGRate.Text = Convert.ToString(option.HalfKgRate);
                cbEnabled.Checked = option.Enabled;
                txtPriority.Text = Convert.ToString(option.Priority);
                txtTimeline.Text = option.Timeline;
                hdnShippingOptionId.Value = Convert.ToString(option.Id);                
            }
        }
    }
}