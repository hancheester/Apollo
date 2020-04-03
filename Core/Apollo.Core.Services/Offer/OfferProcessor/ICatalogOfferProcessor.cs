using Apollo.Core.Model.Entity;

namespace Apollo.Core.Services.Offer.OfferProcessor
{
    public interface ICatalogOfferProcessor
    {
        Product ProcessCatalog(Product product, int testOfferRuleId = 0);
    }
}
