namespace Apollo.Web.Framework.Services.Catalog
{
    public interface IPriceFormatter
    {
        string FormatPrice(decimal price, bool showCurrency = true, bool showCurrencySymbol = false);
        string FormatValue(decimal value, string currencyCode);
        string FormatTaxRate(decimal taxRate);        
    }
}
