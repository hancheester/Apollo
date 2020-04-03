using Apollo.Core.Model;
using Apollo.Core.Model.Entity;
using Apollo.Core.Model.OverviewModel;
using System;
using System.Collections.Generic;

namespace Apollo.Core.Services.Interfaces
{
    public interface ICartService
    {
        IList<CartItem> GetCartItemsByProfileId(int profileId, bool autoRemovePhoneOrderItems, bool? isPharmaceutical = null);
        int GetTotalQuantityCartItemByProfileId(int profileId, bool autoRemovePhoneOrderItems = true);
        OrderTotals CalculateOrderTotals(int profileId, bool autoRemovePhoneOrderItems = true, int testOfferRuleId = 0);
        int DeleteOldCartItem(DateTime? lastUpdateOnDateFromUtc);
        void DeleteCartItemsByProfileIdAndPriceId(int profileId, int productPriceId, bool freeItemIncluded = false);
        IList<ShippingOptionOverviewModel> GetCustomerShippingOptionByCountryAndPriority(int profileId, bool autoRemovePhoneOrderItems = true);
        void UpdateLineQuantityByProfileIdAndProductPriceId(int profileId, int productPriceId, int quantity);
        string[] ProcessPostalRestrictionRules(int profileId, string shippingCountryCode);
        void ProcessCartPharmOrder(CartPharmOrder cartPharmOrder);
        int CalculateEarnedLoyaltyPointsFromCart(int profileId);
        int InsertCartItem(CartItem item);
        bool HasPharmItem(int profileId);
        void DeleteCartItemsByProfileId(int profileId);
        void DeleteCartItemsByProfileIdAndCartItemId(int profileId, int cartItemId);
        bool CheckIfNeedPharmForm(int profileId, bool autoRemovePhoneOrderItems = true);
        AllocatedPointResult ProcessAllocatedPoints(int profileId, int allocatedPoints, bool autoRemovePhoneOrderItems = true);
        bool HasOnlyFreeNHSPrescriptionItem(int profileId);
        void ClearCart(int profileId);
        void MigrateShoppingCart(int fromProfileId, int toProfileId, string shippingCountryCode, bool autoRemovePhoneOrderItems = true);
        string ProcessPrescriptionUpdate(int profileId, int productPriceId, int quantity);
        IList<CartItemOverviewModel> GetCartItemOverviewModelByProfileId(int profileId, bool autoRemovePhoneOrderItems = true, bool? isPharmaceutical = null);
        string ProcessItemQuantityUpdate(
            int profileId,
            string shippingCountryCode,
            int cartItemId,
            int quantity);
        PagedList<CartItemOverviewModel> GetPagedCartItemOverviewModels(
             int pageIndex = 0,
             int pageSize = 2147483647,
             IList<int> productIds = null,
             IList<int> userIds = null,
             string name = null,
             CartItemSortingType orderBy = CartItemSortingType.IdAsc);
        string ProcessItemAddition(
            int profileId,
            int productId,
            int productPriceId,
            string shippingCountryCode,
            int quantity,
            int testOfferRuleId = 0,
            bool disablePhoneOrderCheck = false);
    }
}
