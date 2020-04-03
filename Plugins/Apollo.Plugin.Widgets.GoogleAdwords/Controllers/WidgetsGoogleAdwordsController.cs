using Apollo.Core;
using Apollo.Core.Logging;
using Apollo.Core.Plugins;
using Apollo.Core.Services.Interfaces;
using Apollo.Web.Framework.Controllers;
using System;
using System.Web.Mvc;
using System.Web.UI;

namespace Apollo.Plugin.Widgets.GoogleAdwords.Controllers
{
    public class WidgetsGoogleAdwordsController : BasePluginController
    {
        private readonly IWorkContext _workContext;
        private readonly ISettingService _settingService;
        private readonly IOrderService _orderService;
        private readonly ILogger _logger;

        public WidgetsGoogleAdwordsController(
            IWorkContext workContext,
            ISettingService settingService,
            IOrderService orderService,
            ILogBuilder logBuilder)
        {
            this._workContext = workContext;
            this._settingService = settingService;
            this._orderService = orderService;
            this._logger = logBuilder.CreateLogger(this.GetType().FullName);
        }

        [ChildActionOnly]
        public ActionResult PublicInfo(string widgetZone, object additionalData = null)
        {
            string globalScript = "";
            var routeData = ((Page)this.HttpContext.CurrentHandler).RouteData;

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
                    var lastOrderTotal = _orderService.CalculateLastValidOrderTotalByProfileId(_workContext.CurrentProfile.Id, useDefaultCurrency:true);
                    globalScript += GetTrackingScript(lastOrderTotal);
                }                
            }
            catch (Exception ex)
            {
                _logger.InsertLog(LogLevel.Error, "Error creating scripts for Google Adwords tracking", ex.ToString());
            }
            return Content(globalScript);
        }

        private string GetTrackingScript(decimal orderTotal)
        {
            var googleAdwordsSettings = _settingService.LoadSetting<GoogleAdwordsSettings>();
            var trackingScript = googleAdwordsSettings.TrackingScript + "\n";
            trackingScript = trackingScript.Replace("{GOOGLECONVERSIONID}", googleAdwordsSettings.GoogleConversionId);
            trackingScript = trackingScript.Replace("{GOOGLECONVERSIONLABEL}", googleAdwordsSettings.GoogleConversionLabel);
            trackingScript = string.Format(trackingScript, orderTotal);
            return trackingScript;
        }        
    }
}
