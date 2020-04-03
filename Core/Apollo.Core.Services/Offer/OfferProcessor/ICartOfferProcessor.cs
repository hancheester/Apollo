using Apollo.Core.Model.Entity;
using Apollo.Core.Model.OverviewModel;
using System.Collections.Generic;

namespace Apollo.Core.Services.Offer.OfferProcessor
{
    public interface ICartOfferProcessor
    {
        CartOffer ProcessCart(int profileId, string shippingCountryCode, string promoCode = "", int testOfferRuleId = 0);
        void RemoveCartOfferByPromoCode(int profileId, string promoCode);
        IList<OfferRuleOverviewModel> FindRelatedOffers(Product product);
    }
}