using System.Web.Mvc;
using System.Web.Routing;

namespace Apollo.FrontStore
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            #region Redirect

            routes.MapRoute(
                name: "Old Deal of the Day Link",
                url: "deal-of-the-day",
                defaults: new { controller = "Redirect", action = "RedirectToSpecialOffers" }
            );

            routes.MapRoute(
                name: "Old Just Added Product Link",
                url: "just-added-product",
                defaults: new { controller = "Redirect", action = "RedirectToHome" }
            );

            routes.MapRoute(
                name: "Old Add Review Link",
                url: "add-review",
                defaults: new { controller = "Redirect", action = "RedirectToHome" }
            );

            routes.MapRoute(
                name: "Old Add Item Link",
                url: "add-item",
                defaults: new { controller = "Redirect", action = "RedirectToHome" }
            );

            routes.MapRoute(
                name: "Old Product Link",
                url: "{*urlkey}",
                defaults: new { controller = "Redirect", action = "RedirectToProduct" },
                constraints: new { urlkey = "p-.*" }
            );
            
            routes.MapRoute(
                name: "Old Category Link",
                url: "{*urlkey}",
                defaults: new { controller = "Redirect", action = "RedirectToCategory" },
                constraints: new { urlkey = "c-.*" }
            );

            routes.MapRoute(
                name: "Old Brand Link",
                url: "buy-{urlkey}",
                defaults: new { controller = "Redirect", action = "RedirectToBrand" }
            );

            #endregion

            #region Prescription

            routes.MapRoute(
                name: "Prescriptions",
                url: "category/online-pharmacy/prescriptions",
                defaults: new { controller = "Prescription", action = "Index" }
            );

            routes.MapRoute(
                name: "NHS Prescriptions",
                url: "category/online-pharmacy/prescriptions/nhs-prescriptions",
                defaults: new { controller = "Prescription", action = "NHSPrescriptions" }
            );

            routes.MapRoute(
                name: "NHS Services",
                url: "category/online-pharmacy/prescriptions/nhs-services",
                defaults: new { controller = "Prescription", action = "NHSServices" }
            );

            routes.MapRoute(
                name: "NHS Choices",
                url: "category/online-pharmacy/prescriptions/nhs-choices",
                defaults: new { controller = "Prescription", action = "NHSChoices" }
            );

            routes.MapRoute(
                name: "Private Prescriptions",
                url: "category/online-pharmacy/prescriptions/private-prescriptions",
                defaults: new { controller = "Prescription", action = "PrivatePrescriptions" }
            );

            routes.MapRoute(
                name: "Signposting",
                url: "category/online-pharmacy/prescriptions/signposting",
                defaults: new { controller = "Prescription", action = "Signposting" }
            );

            routes.MapRoute(
                name: "Prescriptions - Healthy Living",
                url: "category/online-pharmacy/prescriptions/prescriptions-healthy-living",
                defaults: new { controller = "Prescription", action = "HealthyLiving" }
            );

            #endregion

            #region Category

            routes.MapRoute(
                name: "Category With Products",
                url: "category/{top}/{second}/{third}",
                defaults: new { controller = "Category", action = "CategoryWithProducts", third = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "Category",
                url: "category/{top}",
                defaults: new { controller = "Category", action = "Category" }
            );

            #endregion

            #region Brand

            routes.MapRoute(
                name: "Shop By Brand",
                url: "brands",
                defaults: new { controller = "Brand", action = "ShopByBrand" }
            );

            routes.MapRoute(
                name: "Brand",
                url: "brand/{urlKey}",
                defaults: new { controller = "Brand", action = "Brand", urlKey = UrlParameter.Optional }
            );
            
            routes.MapRoute(
                name: "Brand With Products",
                url: "brand/{urlKey}/{top}/{second}/{third}",
                defaults: new
                {
                    controller = "Brand",
                    action = "BrandWithProducts",
                    second = UrlParameter.Optional,
                    third = UrlParameter.Optional
                }
            );

            #endregion

            #region Product 

            routes.MapRoute(
                name: "Product",
                url: "product/{urlkey}",
                defaults: new { controller = "Product", action = "ProductDetails" }
            );

            routes.MapRoute(
                name: "Product Not Found",
                url: "productnotfound",
                defaults: new { controller = "Product", action = "ProductNotFound" }
            );

            routes.MapRoute(
                name: "Product With Option",
                url: "product/{urlkey}/{priceid}",
                defaults: new { controller = "Product", action = "ProductDetailsWithOption" },
                constraints: new { priceid = @"\d+" }
            );

            routes.MapRoute(
                name: "Product With Country And Option",
                url: "product/{urlkey}/{priceid}/{countrycode}/{currencycode}",
                defaults: new { controller = "Product", action = "ProductDetailsWithCountryAndOption" },
                constraints: new { priceid = @"\d+" }
            );

            routes.MapRoute(
                name: "Get Product Price Info",
                url: "product/{productId}/getpriceinfo/{productPriceId}/type/{type}",
                defaults: new { controller = "Product", action = "GetPriceInfo" },
                constraints: new { productId = @"\d+", productPriceId = @"\d+", type = @"\d+" }
            );

            routes.MapRoute(
                name: "Add Product Review",
                url: "product/{productId}/addproductreview",
                defaults: new { controller = "Product", action = "AddProductReview" },
                constraints: new { productId = @"\d+" }
            );

            routes.MapRoute(
                name: "Search",
                url: "search",
                defaults: new { controller = "Product", action = "Search" }
            );

            routes.MapRoute(
                name: "Remove Reviewed Product",
                url: "remove-reviewed-product",
                defaults: new { controller = "Product", action = "RemoveReviewedItem" }
            );

            #endregion

            #region Checkout

            routes.MapRoute(
                name: "Checkout Pharm Form",
                url: "checkout/pharmform",
                defaults: new { controller = "Checkout", action = "PharmForm" }
            );

            routes.MapRoute(
                name: "Confirm Pharm Form",
                url: "checkout/confirmpharmform",
                defaults: new { controller = "Checkout", action = "ConfirmPharmForm" }
            );

            routes.MapRoute(
                name: "Checkout Confirm Address",
                url: "checkout/confirmaddress",
                defaults: new { controller = "Checkout", action = "ConfirmAddress" }
            );

            routes.MapRoute(
                name: "Checkout New Address",
                url: "checkout/newaddress/{type}",
                defaults: new { controller = "Checkout", action = "NewAddress" },
                constraints: new { type = @"\d+" }
            );

            routes.MapRoute(
                name: "Checkout Edit Address",
                url: "checkout/editaddress/{type}/{addressid}",
                defaults: new { controller = "Checkout", action = "EditAddress" },
                constraints: new { type = @"\d+", addressid = @"\d+" }
            );

            routes.MapRoute(
                name: "Checkout Display Address",
                url: "checkout/addresses/{type}",
                defaults: new { controller = "Checkout", action = "AddressList" },
                constraints: new { type = @"\d+" }
            );

            routes.MapRoute(
                name: "Checkout Select Address",
                url: "checkout/selectaddress/{type}/{addressid}",
                defaults: new { controller = "Checkout", action = "SelectAddress" },
                constraints: new { type = @"\d+", addressid = @"\d+" }
            );

            routes.MapRoute(
                name: "Checkout Remove Address",
                url: "checkout/removeaddress/{type}/{addressid}",
                defaults: new { controller = "Checkout", action = "RemoveAddress" },
                constraints: new { type = @"\d+", addressid = @"\d+" }
            );

            routes.MapRoute(
                name: "Checkout Confirm Order",
                url: "checkout/confirmorder",
                defaults: new { controller = "Checkout", action = "ConfirmOrder" }
            );

            routes.MapRoute(
                name: "Checkout Payment",
                url: "checkout/payment",
                defaults: new { controller = "Checkout", action = "Payment" }
            );

            routes.MapRoute(
                name: "Checkout Confirm Payment",
                url: "checkout/confirmpayment",
                defaults: new { controller = "Checkout", action = "ConfirmPayment" }
            );

            routes.MapRoute(
                name: "Checkout Completed",
                url: "checkout/completed/{orderid}/{emailinvoiceid}/{hasnhs}",
                defaults: new { controller = "Checkout", action = "Completed", orderid = UrlParameter.Optional, emailinvoiceid = UrlParameter.Optional, hasnhs = UrlParameter.Optional },
                constraints: new { orderid = @"\d+", emailinvoiceid = @"\d+", hasnhs = @"\d+" }
            );
            
            routes.MapRoute(
                name: "SagePay Direct Terminal",
                url: "sagepaydirect/terminal",
                defaults: new { controller = "SagePayDirect", action = "Terminal" }
            );

            routes.MapRoute(
                name: "Checkout Invoice Payment",
                url: "checkout/invoicepayment",
                defaults: new { controller = "Checkout", action = "InvoicePayment" }
            );

            routes.MapRoute(
                name: "Checkout Confirm Invoice Payment",
                url: "checkout/confirminvoicepayment",
                defaults: new { controller = "Checkout", action = "ConfirmInvoicePayment" }
            );

            #endregion

            #region Account

            routes.MapRoute(
                name: "Register",
                url: "register",
                defaults: new { controller = "Customer", action = "Register" }
            );

            routes.MapRoute(
                name: "Register Result",
                url: "register-result/{resultId}",
                defaults: new { controller = "Customer", action = "RegisterResult" },
                constraints: new { resultId = @"\d+" }
            );

            routes.MapRoute(
                name: "Login",
                url: "login",
                defaults: new { controller = "Customer", action = "Login" }
            );

            routes.MapRoute(
                name: "Logout",
                url: "logout",
                defaults: new { controller = "Customer", action = "Logout" }
            );

            routes.MapRoute(
                name: "OAuth 2 Auth",
                url: "externalauth",
                defaults: new { controller = "Customer", action = "ExternalAuthentication" }
            );

            routes.MapRoute(
                name: "OAuth 2 Callback",
                url: "externallogincallback",
                defaults: new { controller = "Customer", action = "ExternalLoginCallback" }
            );

            routes.MapRoute(
                name: "Account",
                url: "account",
                defaults: new { controller = "Customer", action = "Account" }
            );

            routes.MapRoute(
                name: "Account Change Password",
                url: "account/changepassword",
                defaults: new { controller = "Customer", action = "ChangePassword" }
            );

            routes.MapRoute(
                name: "Account Set Password",
                url: "account/setpassword",
                defaults: new { controller = "Customer", action = "SetPassword" }
            );

            routes.MapRoute(
                name: "Account Remove Login",
                url: "account/removelogin",
                defaults: new { controller = "Customer", action = "RemoveLogin" }
            );

            routes.MapRoute(
                name: "Account Primary Address",
                url: "account/primaryaddress",
                defaults: new { controller = "Customer", action = "PrimaryAddress" }
            );

            routes.MapRoute(
                name: "Account Addresses",
                url: "account/addresses",
                defaults: new { controller = "Customer", action = "Addresses" }
            );

            routes.MapRoute(
                name: "Account Set Address",
                url: "account/setaddress/{id}/{type}",
                defaults: new { controller = "Customer", action = "SetAddress" },
                constraints: new { id = @"\d+", type = @"\d+" }
            );

            routes.MapRoute(
                name: "Account New Address",
                url: "account/newaddress",
                defaults: new { controller = "Customer", action = "NewAddress" }
            );

            routes.MapRoute(
                name: "Account Edit Address",
                url: "account/editaddress/{id}",
                defaults: new { controller = "Customer", action = "EditAddress" },
                constraints: new { id = @"\d+" }
            );

            routes.MapRoute(
                name: "Account Remove Address",
                url: "account/removeaddress/{id}",
                defaults: new { controller = "Customer", action = "RemoveAddress" },
                constraints: new { id = @"\d+" }
            );

            routes.MapRoute(
                name: "Forgot Password",
                url: "password-retrieval",
                defaults: new { controller = "Customer", action = "ForgotPassword" }
            );

            routes.MapRoute(
                name: "Forgot Password Result",
                url: "password-retrieval-result/{resultId}",
                defaults: new { controller = "Customer", action = "ForgotPasswordResult" },
                constraints: new { resultId = @"\d+" }
            );

            #endregion

            #region Order

            routes.MapRoute(
                name: "Customer Orders",
                url: "account/orders",
                defaults: new { controller = "Order", action = "Orders" }
            );

            routes.MapRoute(
                name: "Customer Order Details",
                url: "account/orders/{id}",
                defaults: new { controller = "Order", action = "Details" },
                constraints: new { id = @"\d+" }
            );

            routes.MapRoute(
                name: "Customer Order Invoice PDF",
                url: "account/orders/{id}/invoice",
                defaults: new { controller = "Order", action = "DownloadInvoice" },
                constraints: new { id = @"\d+" }
            );

            routes.MapRoute(
                name: "Customer Reward Points",
                url: "account/rewardpoints",
                defaults: new { controller = "Order", action = "RewardPoints" }
            );

            #endregion

            #region Shopping Cart

            routes.MapRoute(
                name: "Update Shipping Option",
                url: "cart/updateshippingoption",
                defaults: new { controller = "ShoppingCart", action = "UpdateShippingOption" }
            );

            routes.MapRoute(
                name: "Apply Code",
                url: "cart/applycode",
                defaults: new { controller = "ShoppingCart", action = "ApplyCode" }
            );

            routes.MapRoute(
                name: "Remove Code",
                url: "cart/removecode",
                defaults: new { controller = "ShoppingCart", action = "RemoveCode" }
            );

            routes.MapRoute(
                name: "Apply Points",
                url: "cart/applypoints",
                defaults: new { controller = "ShoppingCart", action = "ApplyPoint" }
            );

            routes.MapRoute(
                name: "Clear Points",
                url: "cart/clearpoints",
                defaults: new { controller = "ShoppingCart", action = "ClearPoint" }
            );

            routes.MapRoute(
                name: "Load Shipping Options By Country",
                url: "cart/loadshippingoptions",
                defaults: new { controller = "ShoppingCart", action = "LoadShippingOptions" }
            );

            routes.MapRoute(
                name: "Shopping Cart",
                url: "cart",
                defaults: new { controller = "ShoppingCart", action = "Cart" }
            );

            routes.MapRoute(
                name: "Update Item Quantity",
                url: "shop/changequantity",
                defaults: new { controller = "ShoppingCart", action = "ChangeQuantity" }                
            );

            routes.MapRoute(
                name: "Add Product To Cart - Details",
                url: "shop/additem/details/{productid}",
                defaults: new { controller = "ShoppingCart", action = "AddItemWithDetails" },
                constraints: new { productid = @"\d+" }
            );

            routes.MapRoute(
                name: "Add Product To Cart - Catalog",
                url: "shop/additem/catalog/{productid}/{shoppingcarttypeid}/{quantity}/{productpriceid}",
                defaults: new { controller = "ShoppingCart", action = "AddProductToCartFromCatalog", productpriceid = UrlParameter.Optional },
                constraints: new { productid = @"\d+", shoppingcarttypeid = @"\d+", quantity = @"\d+" }
            );
            
            routes.MapRoute(
                name: "Add NHS Prescription",
                url: "shop/additem/prescription",
                defaults: new { controller = "ShoppingCart", action = "AddPrescription" }
            );

            routes.MapRoute(
                name: "Remove Product From Cart",
                url: "shop/removeitem/{cartitemid}",
                defaults: new { controller = "ShoppingCart", action = "RemoveItem" },
                constraints: new { cartitemid = @"\d+" }
            );

            #endregion

            #region Offer

            routes.MapRoute(
                name: "Special Offers",
                url: "special-offers",
                defaults: new { controller = "Common", action = "SpecialOffers" }
            );

            routes.MapRoute(
                name: "Individual Offer",
                url: "special-offers/{urlkey}",
                defaults: new { controller = "Common", action = "IndividualOffer" }
            );

            routes.MapRoute(
                name: "Hide Header Strip Offer",
                url: "hide-header-strip-offer",
                defaults: new { controller = "Common", action = "HeaderStripOfferHide" }
            );

            #endregion
            
            #region Delivery & Currency

            routes.MapRoute(
                name: "Delivery Information",
                url: "delivery-information",
                defaults: new { controller = "Common", action = "DeliveryInformation" }
            );

            routes.MapRoute(
                name: "International Delivery Country",
                url: "international-delivery-country",
                defaults: new { controller = "Common", action = "InternationalDeliveryCountry" }
            );

            routes.MapRoute(
                name: "Change Location Preference",
                url: "changelocationpreference",
                defaults: new { controller = "Common", action = "ChangeLocationPreference" }
            );
            
            routes.MapRoute(
                name: "Change Country Preference",
                url: "changecountrypreference",
                defaults: new { controller = "Common", action = "ChangeCountryPreference" }
            );

            routes.MapRoute(
                name: "Change Currency",
                url: "changecurrency/{customercurrency}",
                defaults: new { controller = "Common", action = "SetCurrency" },
                constraints: new { customercurrency = @"\d+" }
            );

            #endregion

            #region Subscription

            routes.MapRoute(
                name: "Subscription",
                url: "subscription",
                defaults: new { controller = "Common", action = "SubscribeBox" }
            );

            routes.MapRoute(
                name: "Subscription Result",
                url: "subscription-result",
                defaults: new { controller = "Common", action = "SubscribeResult" }
            );
            
            #endregion

            #region Blog

            routes.MapRoute(
                name: "i-Zone",
                url: "blog",
                defaults: new { controller = "Blog", action = "List" }
            );

            routes.MapRoute(
                name: "Blog Post",
                url: "blog/{urlkey}",
                defaults: new { controller = "Blog", action = "BlogPost" }
            );

            routes.MapRoute(
                "Blog By Tag",
                "blog/tag/{tag}",
                new { controller = "Blog", action = "BlogByTag" }
            );

            routes.MapRoute(
                "Blog By Month",
                "blog/month/{month}",
                new { controller = "Blog", action = "BlogByMonth" }
            );

            routes.MapRoute(
                name: "Blog RSS",
                url: "blog/rss",
                defaults: new { controller = "Blog", action = "ListRss" }
            );

            #endregion

            #region Alternative Product

            routes.MapRoute(
                name: "Help Alternative Product",
                url: "help-alternative-product",
                defaults: new { controller = "Common", action = "HelpAlternativeProduct" }
            );

            routes.MapRoute(
                name: "Help Alternative Result",
                url: "help-alternative-product-result",
                defaults: new { controller = "Common", action = "HelpAlternativeProductResult" }
            );

            #endregion

            #region Stock Notification

            routes.MapRoute(
                name: "Help Stock Notification",
                url: "help-stock-notification",
                defaults: new { controller = "Common", action = "HelpStockNotification" }
            );

            routes.MapRoute(
                name: "Quick Stock Notification",
                url: "quick-stock-notification/{productid}",
                defaults: new {controller = "Common", action = "QuickStockNofification" },
                constraints: new { productid = @"\d+" }
            );

            #endregion

            #region AMP

            routes.MapRoute(
                name: "Category With Products AMP",
                url: "amp/category/{top}/{second}/{third}",
                defaults: new { controller = "Amp", action = "CategoryWithProducts", third = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "Category AMP",
                url: "amp/category/{top}",
                defaults: new { controller = "Amp", action = "Category" }
            );

            routes.MapRoute(
                name: "Shop By Brand AMP",
                url: "amp/brands",
                defaults: new { controller = "Amp", action = "ShopByBrand" }
            );

            routes.MapRoute(
                name: "Brand AMP",
                url: "amp/brand/{urlKey}",
                defaults: new { controller = "Amp", action = "Brand", urlKey = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "Brand With Products AMP",
                url: "amp/brand/{urlKey}/{top}/{second}/{third}",
                defaults: new
                {
                    controller = "Amp",
                    action = "BrandWithProducts",
                    second = UrlParameter.Optional,
                    third = UrlParameter.Optional
                }
            );

            routes.MapRoute(
                name: "Product AMP",
                url: "amp/product/{urlkey}",
                defaults: new { controller = "Amp", action = "ProductDetails" }
            );

            routes.MapRoute(
                name: "Subscription AMP",
                url: "amp/subscription",
                defaults: new { controller = "Amp", action = "SubscribeBox" }
            );

            routes.MapRoute(
                name: "Change Location Preference AMP",
                url: "amp/changelocationpreference",
                defaults: new { controller = "Amp", action = "ChangeLocationPreference" }
            );

            routes.MapRoute(
                name: "Home AMP",
                url: "amp/home",
                defaults: new { controller = "Amp", action = "Index" }
            );

            routes.MapRoute(
                name: "Add Product To Cart - Details AMP",
                url: "amp/shop/additem/details/{productid}",
                defaults: new { controller = "Amp", action = "AddItemWithDetails" },
                constraints: new { productid = @"\d+" }
            );

            #endregion

            #region Media

            #endregion

            routes.MapRoute(
                name: "Contact Us",
                url: "contact-us",
                defaults: new { controller = "Common", action = "ContactUs" }
            );

            routes.MapRoute(
                name: "Contact Us Result",
                url: "contact-us-result",
                defaults: new { controller = "Common", action = "ContactUsResult" }
            );

            routes.MapRoute(
                name: "About Us",
                url: "about-us",
                defaults: new { controller = "Common", action = "AboutUs" }
            );
            
            routes.MapRoute(
                name: "Terms And Conditions",
                url: "terms-and-conditions",
                defaults: new { controller = "Common", action = "TermsAndConditions" }
            );

            routes.MapRoute(
                name: "Privacy Policy",
                url: "privacy-policy",
                defaults: new { controller = "Common", action = "PrivacyPolicy" }
            );

            routes.MapRoute(
                name: "Loyalty Scheme",
                url: "loyalty-points-scheme",
                defaults: new { controller = "Common", action = "LoyaltyScheme" }
            );

            routes.MapRoute(
                name: "Accept EU Cookie Law",
                url: "accept-eu-cookie-law",
                defaults: new { controller = "Common", action = "EuCookieLawAccept" }
            );

            routes.MapRoute(
                name: "Careers",
                url: "careers",
                defaults: new { controller = "Common", action = "Careers" }
            );

            routes.MapRoute(
                name: "Home",
                url: "",
                defaults: new { controller = "Home", action = "Index"}
            );

            routes.MapRoute(
                name: "Refresh Cache Notification",
                url: "refresh-cache-notification/{entity}/{token}",
                defaults: new { controller = "Admin", action = "RefreshCache" }
            );

            routes.MapRoute(
                name: "Get Cache Performance",
                url: "get-cache-performance/{token}",
                defaults: new { controller = "Admin", action = "GetCachePerformanceData" }
            );

            routes.MapRoute(
                name: "Get Cache Keys",
                url: "get-cache-keys/{token}",
                defaults: new { controller = "Admin", action = "GetCacheKeys" }
            );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "404",
                url: "{*url}",
                defaults: new { controller = "Common", action = "PageNotFound" }
            );
        }
    }
}
