using Apollo.Core;
using Apollo.Core.Domain;
using Apollo.Core.Logging;
using Apollo.Core.Model.Entity;
using Apollo.Core.Plugins;
using Apollo.Core.Services.Interfaces;
using Apollo.Web.Framework.Controllers;
using System;
using System.Globalization;
using System.Text;
using System.Web.Mvc;

namespace Apollo.Plugin.Widgets.GoogleAnalytics.Controllers
{
    public class WidgetsGoogleAnalyticsController : BasePluginController
    {
        private readonly IWorkContext _workContext;
        private readonly ISettingService _settingService;
        private readonly IOrderService _orderService;        
        private readonly ICategoryService _categoryService;
        private readonly ILogger _logger;

        public WidgetsGoogleAnalyticsController(
            IWorkContext workContext,
            ISettingService settingService,
            IOrderService orderService,
            ICategoryService categoryService,
            ILogBuilder logBuilder)
        {
            _workContext = workContext;
            _settingService = settingService;
            _orderService = orderService;
            _logger = logBuilder.CreateLogger(GetType().FullName);
            _categoryService = categoryService;            
        }

        [ChildActionOnly]
        public ActionResult PublicInfo(string widgetZone, object additionalData = null)
        {
            string globalScript = "";
            var routeData = ((System.Web.UI.Page)HttpContext.CurrentHandler).RouteData;

            try
            {
                var controller = routeData.Values["controller"];
                var action = routeData.Values["action"];

                if (controller == null || action == null)
                    return Content("");

                //Special case, if we are in last step of checkout, we can use order total for conversion value
                if (controller.ToString().Equals("checkout", StringComparison.InvariantCultureIgnoreCase) &&
                    action.ToString().Equals("completed", StringComparison.InvariantCultureIgnoreCase))
                {
                    var lastOrder = GetLastOrder();
                    globalScript += GetEcommerceScript(lastOrder);
                }
                else
                {
                    globalScript += GetTrackingScript();
                }
            }
            catch (Exception ex)
            {
                _logger.InsertLog(LogLevel.Error, "Error creating scripts for Google Analytics tracking", ex.ToString());
            }
            return Content(globalScript);
        }

        private Order GetLastOrder()
        {
            var order = _orderService.GetLastValidOrderByProfileId(_workContext.CurrentProfile.Id, useDefaultCurrency: true);
            return order;
        }

        //<script type="text/javascript"> 

        //var _gaq = _gaq || []; 
        //_gaq.push(['_setAccount', 'UA-XXXXX-X']); 
        //_gaq.push(['_trackPageview']); 

        //(function() { 
        //var ga = document.createElement('script'); ga.type = 'text/javascript'; ga.async = true; 
        //ga.src = ('https:' == document.location.protocol ? 'https://ssl' : 'http://www') + '.google-analytics.com/ga.js'; 
        //var s = document.getElementsByTagName('script')[0]; s.parentNode.insertBefore(ga, s); 
        //})(); 

        //</script>
        private string GetTrackingScript()
        {
            var googleAnalyticsSettings = _settingService.LoadSetting<GoogleAnalyticsSettings>();
            var analyticsTrackingScript = googleAnalyticsSettings.TrackingScript + "\n";
            analyticsTrackingScript = analyticsTrackingScript.Replace("{GOOGLEID}", googleAnalyticsSettings.GoogleId);
            analyticsTrackingScript = analyticsTrackingScript.Replace("{ECOMMERCE}", "");

            return analyticsTrackingScript;
        }

        //<script type="text/javascript"> 

        //var _gaq = _gaq || []; 
        //_gaq.push(['_setAccount', 'UA-XXXXX-X']); 
        //_gaq.push(['_trackPageview']); 
        //_gaq.push(['_addTrans', 
        //'1234',           // order ID - required 
        //'Acme Clothing',  // affiliation or store name 
        //'11.99',          // total - required 
        //'1.29',           // tax 
        //'5',              // shipping 
        //'San Jose',       // city 
        //'California',     // state or province 
        //'USA'             // country 
        //]); 

        //// add item might be called for every item in the shopping cart 
        //// where your ecommerce engine loops through each item in the cart and 
        //// prints out _addItem for each 
        //_gaq.push(['_addItem', 
        //'1234',           // order ID - required 
        //'DD44',           // SKU/code - required 
        //'T-Shirt',        // product name 
        //'Green Medium',   // category or variation 
        //'11.99',          // unit price - required 
        //'1'               // quantity - required 
        //]); 
        //_gaq.push(['_trackTrans']); //submits transaction to the Analytics servers 

        //(function() { 
        //var ga = document.createElement('script'); ga.type = 'text/javascript'; ga.async = true; 
        //ga.src = ('https:' == document.location.protocol ? 'https://ssl' : 'http://www') + '.google-analytics.com/ga.js'; 
        //var s = document.getElementsByTagName('script')[0]; s.parentNode.insertBefore(ga, s); 
        //})(); 

        //</script>
        private string GetEcommerceScript(Order order)
        {
            var googleAnalyticsSettings = _settingService.LoadSetting<GoogleAnalyticsSettings>();
            var storeSettings = _settingService.LoadSetting<StoreInformationSettings>();
            var usCulture = new CultureInfo("en-US");
            var analyticsTrackingScript = googleAnalyticsSettings.TrackingScript + "\n";
            analyticsTrackingScript = analyticsTrackingScript.Replace("{GOOGLEID}", googleAnalyticsSettings.GoogleId);

            if (order != null)
            {
                var analyticsEcommerceScript = googleAnalyticsSettings.EcommerceScript + "\n";
                analyticsEcommerceScript = analyticsEcommerceScript.Replace("{GOOGLEID}", googleAnalyticsSettings.GoogleId);
                analyticsEcommerceScript = analyticsEcommerceScript.Replace("{ORDERID}", order.Id.ToString());                
                analyticsEcommerceScript = analyticsEcommerceScript.Replace("{SITE}", storeSettings.CompanyName);
                analyticsEcommerceScript = analyticsEcommerceScript.Replace("{TOTAL}", order.GrandTotal.ToString("0.00", usCulture));                
                analyticsEcommerceScript = analyticsEcommerceScript.Replace("{TAX}", order.Tax.ToString("0.00", usCulture));
                analyticsEcommerceScript = analyticsEcommerceScript.Replace("{SHIP}", order.ShippingCost.ToString("0.00", usCulture));
                analyticsEcommerceScript = analyticsEcommerceScript.Replace("{CITY}", order.City == null ? "" : FixIllegalJavaScriptChars(order.City));                
                analyticsEcommerceScript = analyticsEcommerceScript.Replace("{STATEPROVINCE}", order.USState == null ? "" : FixIllegalJavaScriptChars(order.USState.State));                
                analyticsEcommerceScript = analyticsEcommerceScript.Replace("{COUNTRY}", order.Country == null ? "" : FixIllegalJavaScriptChars(order.Country.Name));

                var sb = new StringBuilder();
                foreach (var item in order.LineItemCollection)
                {
                    string analyticsEcommerceDetailScript = googleAnalyticsSettings.EcommerceDetailScript;
                    //get category
                    string category = string.Empty;
                    var defaultProductCategory = _categoryService.GetFirstActiveCategoryByProductId(item.ProductId);
                    if (defaultProductCategory != null)
                        category = defaultProductCategory.CategoryName;
                    analyticsEcommerceDetailScript = analyticsEcommerceDetailScript.Replace("{ORDERID}", item.OrderId.ToString());
                    //The SKU code is a required parameter for every item that is added to the transaction                    
                    analyticsEcommerceDetailScript = analyticsEcommerceDetailScript.Replace("{PRODUCTSKU}", item.ProductId.ToString());
                    analyticsEcommerceDetailScript = analyticsEcommerceDetailScript.Replace("{PRODUCTNAME}", FixIllegalJavaScriptChars(item.Name + " " + item.Option));
                    analyticsEcommerceDetailScript = analyticsEcommerceDetailScript.Replace("{CATEGORYNAME}", FixIllegalJavaScriptChars(category));
                    var unitPrice = googleAnalyticsSettings.IncludingTax ? item.PriceInclTax : item.PriceExclTax;                    
                    analyticsEcommerceDetailScript = analyticsEcommerceDetailScript.Replace("{UNITPRICE}", unitPrice.ToString("0.00", usCulture));
                    analyticsEcommerceDetailScript = analyticsEcommerceDetailScript.Replace("{QUANTITY}", item.Quantity.ToString());
                    sb.AppendLine(analyticsEcommerceDetailScript);
                }

                analyticsEcommerceScript = analyticsEcommerceScript.Replace("{DETAILS}", sb.ToString());

                analyticsTrackingScript = analyticsTrackingScript.Replace("{ECOMMERCE}", analyticsEcommerceScript);

            }

            return analyticsTrackingScript;
        }

        private string FixIllegalJavaScriptChars(string text)
        {
            if (string.IsNullOrEmpty(text))
                return text;

            //replace ' with \' (http://stackoverflow.com/questions/4292761/need-to-url-encode-labels-when-tracking-events-with-google-analytics)
            text = text.Replace("'", "\\'");
            text = text.Trim();
            return text;
        }
    }
}
