using Apollo.AdminStore.WebForm.Classes;
using Apollo.Core.Model.Entity;
using Apollo.Core.Services.Interfaces;
using System;
using System.Web.UI.WebControls;

namespace Apollo.AdminStore.WebForm.System
{
    public partial class system_currency_info : BasePage
    {
        public IUtilityService UtilityService { get; set; }
        public IShippingService ShippingService { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)            
                LoadInfo();            
        }
       
        protected void lbSaveContinue_Click(object sender, EventArgs e)
        {
            Currency currency = UtilityService.GetCurrency(QueryId);
            currency.CurrencyCode = txtCurrencyCode.Text.Trim();
            currency.HtmlEntity = txtHtmlEntity.Text.Trim();
            currency.ExchangeRate = Convert.ToDecimal(txtExchangeRate.Text.Trim());            
            currency.Symbol = txtSymbol.Text.Trim();

            UtilityService.UpdateCurrency(currency);
            
            enbNotice.Message = "Currency was updated successfully.";
        }
        
        protected string GetCountryImage(int countryId)
        {
            const string FLAG_HTML_FORMAT = "<span class='flag-icon flag-icon-{0}' alt='{1}' title='{1}'></span> - {1}";
            
            var country = ShippingService.GetCountryById(countryId);

            if (country != null)            
                return string.Format(FLAG_HTML_FORMAT, country.ISO3166Code.ToLower(), country.Name);
            
            return string.Empty;
        }

        protected void lbAddCountry_Click(object sender, EventArgs e)
        {
            var item = new CurrencyCountry
            {
                CountryId = Convert.ToInt32(ddlCountry.SelectedValue),
                CurrencyId = QueryId
            };

            UtilityService.InsertCurrencyCountry(item);
            LoadCurrencyCountries();
        }

        protected void rptCountries_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            switch (e.CommandName)
            {
                case "delete":
                    UtilityService.DeleteCurrencyCountry(Convert.ToInt32(e.CommandArgument));
                    break;
                default:
                    break;
            }
            
            LoadCurrencyCountries();
        }

        protected void ddlCountry_Init(object sender, EventArgs e)
        {
            var result = ShippingService.GetPagedCountry();            
            ddlCountry.DataSource = result.Items;
            ddlCountry.DataTextField = "Name";
            ddlCountry.DataValueField = "Id";
            ddlCountry.DataBind();
        }

        private void LoadInfo()
        {
            var currency = UtilityService.GetCurrency(QueryId);

            if (currency != null)
            {
                ltlTitle.Text = string.Format("{0} (ID: {1})", currency.CurrencyCode, QueryId);
                txtCurrencyCode.Text = currency.CurrencyCode;
                txtHtmlEntity.Text = currency.HtmlEntity;
                txtExchangeRate.Text = currency.ExchangeRate.ToString();
                txtSymbol.Text = currency.Symbol;

                LoadCurrencyCountries();
            }
            else
                Response.Redirect("/system/system_currency_default.aspx?msg=currency_not_found");
        }

        private void LoadCurrencyCountries()
        {
            var list = UtilityService.GetCurrencyCountryByCurrencyId(QueryId);
            rptCountries.DataSource = list;
            rptCountries.DataBind();
        }
    }
}