using Apollo.Core.Model;
using Apollo.Core.Model.Entity;
using Apollo.Core.Model.OverviewModel;
using System.Collections.Generic;

namespace Apollo.Core.Services.Interfaces
{
    public interface IOfferService
    {
        void RemoveCartOfferByPromoCode(int profileId, string promoCode);
        IList<OfferTypeOverviewModel> GetActiveOfferTypes();
        int ProcessOfferInsertion(OfferRule rule);
        OfferRule GetOfferRuleByUrlKey(string urlKey);
        PagedList<OfferRule> GetOfferRuleLoadPaged(
            int pageIndex = 0,
            int pageSize = 2147483647,
            IList<int> offerRuleIds = null,
            string name = null,
            string promocode = null,
            string fromDate = null,
            string toDate = null,
            bool? isActive = null,
            bool? isCart = null,
            bool? showInOfferPage = null,
            int? offerTypeId = null,
            OfferRuleSortingType orderBy = OfferRuleSortingType.IdAsc);
        int InsertOfferRelatedItem(OfferRelatedItem relatedItem);
        bool ProcessOfferUpdation(OfferRule rule);
        void UpdateOfferRelatedItem(OfferRelatedItem relatedItem);
        OfferRule GetOfferRuleById(int offerRuleId);
        void DeleteOfferRelatedItem(int offerRelatedItemId);
        IList<OfferType> GetOfferTypes();
        IList<OfferActionAttribute> GetOfferActionAttributes(bool? isCatalog, bool? isCart);
        void RemoveOfferedItemsFromBaskets(int offerRuleId = 0);
        OfferAttribute GetOfferAttribute(int id);
        IList<OfferAttribute> GetOfferAttributeByType(bool isCatalog, bool isCart);
        OfferOperator GetOfferOperator(int id);
        IList<OfferOperator> GetOfferOperatorsByAttribute(int offerAttributeId);
        Product ProcessCatalog(Product product, int testOfferRuleId = 0);        
        CartOffer ProcessCartOfferByProfileId(int profileId, string shippingCountryCode, int testOfferRuleId = 0);
        OfferRule GetOfferRuleOnlyById(int id);
        IList<OfferRuleOverviewModel> FindRelatedOffers(Product product);
    }
}