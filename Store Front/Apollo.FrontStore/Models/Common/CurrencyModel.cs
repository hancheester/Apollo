using Apollo.Web.Framework.Mvc;

namespace Apollo.FrontStore.Models.Common
{
    public class CurrencyModel : BaseEntityModel
    {
        public string Name { get; set; }
        public string CurrencyCode { get; set; }
        public string CurrencySymbol { get; set; }
    }
}