using Apollo.Core;
using Apollo.Core.Model.Entity;
using System;

namespace Apollo.Web.Framework.Services.Catalog
{
    public class PriceFormatter : IPriceFormatter
    {
        #region Fields

        private readonly IWorkContext _workContext;
        
        #endregion

        #region Constructors

        public PriceFormatter(IWorkContext workContext)
        {
            this._workContext = workContext;
        }

        #endregion

        #region Utilities

        protected string GetCurrencyString(decimal amount)
        {
            return GetCurrencyString(amount, _workContext.WorkingCurrency, true);
        }

        protected string GetCurrencyString(decimal amount, Currency targetCurrency, bool showCurrency, bool showCurrencySymbol = false)
        {
            if (targetCurrency == null) throw new ArgumentNullException("targetCurrency");

            var prefix = amount < 0 ? "-" : string.Empty;
            amount = amount < 0 ? amount * -1 : amount;
            amount = Math.Round(targetCurrency.ExchangeRate * amount, 2, MidpointRounding.AwayFromZero);

            if (showCurrency)            
                return string.Format("{0}{1}{2:0.00}", prefix, showCurrencySymbol ? targetCurrency.Symbol : targetCurrency.HtmlEntity, amount);                
            
             return string.Format("{0:0.00}", amount);            
        }

        #endregion

        #region Methods

        public virtual string FormatPrice(decimal price, bool showCurrency = true, bool showCurrencySymbol = false)
        {
            return GetCurrencyString(price, _workContext.WorkingCurrency, showCurrency, showCurrencySymbol);
        }
        
        public virtual string FormatValue(decimal value, string currencyCode)
        {
            value = Math.Round(value, 2);
            return string.Format("{0} {1}", currencyCode, value);
        }

        public virtual string FormatTaxRate(decimal taxRate)
        {
            //return taxRate.ToString("G29");
            //return FormatPrice(taxRate, false, _workContext.WorkingCurrency);

            //var targetCurrency = _workContext.WorkingCurrency;
            //taxRate = taxRate < 0M ? taxRate * -1 : taxRate;

            return string.Format("{0}{1}", _workContext.WorkingCurrency.HtmlEntity, taxRate.ToString("N"));            
        }

        #endregion
    }
}
