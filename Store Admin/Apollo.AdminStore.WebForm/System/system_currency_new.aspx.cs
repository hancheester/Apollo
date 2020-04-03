using Apollo.AdminStore.WebForm.Classes;
using Apollo.Core.Model.Entity;
using Apollo.Core.Services.Interfaces;
using System;

namespace Apollo.AdminStore.WebForm.System
{
    public partial class system_currency_new : BasePage
    {
        public IUtilityService UtilityService { get; set; }

        protected void lbSave_Click(object sender, EventArgs e)
        {
            Currency currency = new Currency();
            currency.CurrencyCode = txtCurrencyCode.Text.Trim();
            currency.HtmlEntity = txtHtmlEntity.Text.Trim();
            currency.ExchangeRate = Convert.ToDecimal(txtExchangeRate.Text.Trim());
            currency.Symbol = txtSymbol.Text.Trim();

            int id = UtilityService.InsertCurrency(currency);
            
            Response.Redirect("/system/system_currency_info.aspx?" + QueryKey.MSG_TYPE + "=" + (int)MessageType.CurrencyCreated + "&" + QueryKey.ID + "=" + currency.Id.ToString());
        }
    }
}