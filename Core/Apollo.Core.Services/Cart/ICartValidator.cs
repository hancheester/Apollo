namespace Apollo.Core.Services.Cart
{
    public interface ICartValidator
    {
        string ValidateCartItem(int profileId,
                                int productId,
                                int brandId,
                                string shippingCountryCode,
                                bool isPharmaceutical,
                                bool discontinued,
                                int stock,                                
                                string name,
                                bool productEnforcedStockCount,
                                bool brandEnforcedStockCount,
                                bool enabled,
                                bool productPriceEnabled,                                
                                int quantityInCart,
                                int quantity,
                                int stepQuantity,
                                bool isPhoneOrder,
                                decimal price,
                                bool allowZeroPrice = false);

        string[] ValidateCartAgainstShippingCountry(int profileId, string shippingCountryCode);
    }
}
