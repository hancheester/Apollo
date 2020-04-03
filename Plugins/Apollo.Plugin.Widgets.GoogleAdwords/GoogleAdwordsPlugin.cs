using Apollo.Core.Plugins;
using Apollo.Core.Services.Interfaces;
using System.Collections.Generic;
using System.Web.Routing;

namespace Apollo.Plugin.Widgets.GoogleAdwords
{
    public class GoogleAdwordsPlugin : BasePlugin, IWidgetPlugin
    {
        private readonly ISettingService _settingService;

        public GoogleAdwordsPlugin(ISettingService settingService)
        {
            this._settingService = settingService;
        }

        public void GetConfigurationRoute(out string actionName, out string controllerName, out RouteValueDictionary routeValues)
        {
            actionName = "Configure";
            controllerName = "WidgetsGoogleAdwords";
            routeValues = new RouteValueDictionary { { "Namespaces", "Apollo.Plugin.Widgets.GoogleAdwords.Controllers" }, { "area", null } };
        }

        public IList<string> GetWidgetZones()
        {
            return new List<string>
            {
                "body_end_html_tag_before"
            };
        }

        public void GetDisplayWidgetRoute(string widgetZone, out string actionName, out string controllerName, out RouteValueDictionary routeValues)
        {
            actionName = "PublicInfo";
            controllerName = "WidgetsGoogleAdwords";
            routeValues = new RouteValueDictionary
            {
                {"Namespaces", "Apollo.Plugin.Widgets.GoogleAdwords.Controllers"},
                {"area", null},
                {"widgetZone", widgetZone}
            };
        }

        public override void Install()
        {
            var settings = new GoogleAdwordsSettings
            {
                GoogleConversionId = "00000000",
                GoogleConversionLabel = "@@@@@@@@",
                TrackingScript = @"<!-- BEGIN Google Adwords -->
<script type=""text/javascript"">
<!--
var google_conversion_id = {GOOGLECONVERSIONID};
var google_conversion_language = 'en';
var google_conversion_format = '1';
var google_conversion_color = 'ffffff';
var google_conversion_label = '{GOOGLECONVERSIONLABEL}';
var google_conversion_value = 0;
if ({0:0.00}) {{ 
google_conversion_value = {0:0.00};}}
//-->
</script>
<script type=""text/javascript"" src=""https://www.googleadservices.com/pagead/conversion.js"">
</script>
<noscript>
<div style=""display:inline;"">
<img height=""1"" width=""1"" style=""border-style:none;"" alt="""" src=""https://www.googleadservices.com/pagead/conversion/1025552406/?value={0:0.00}&amp;label=AiLWCMaWrAEQluCC6QM&amp;guid=ON&amp;script=0""/>
</div>
</noscript>
<!-- END Google Adwords -->
",
            };
            _settingService.SaveSetting(settings);

            base.Install();
        }

        public override void Uninstall()
        {
            //settings
            _settingService.DeleteSetting<GoogleAdwordsSettings>();

            base.Uninstall();
        }
    }
}
