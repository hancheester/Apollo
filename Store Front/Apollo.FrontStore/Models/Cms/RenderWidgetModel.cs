using System.Web.Routing;

namespace Apollo.FrontStore.Models.Cms
{
    public class RenderWidgetModel
    {
        public string ActionName { get; set; }
        public string ControllerName { get; set; }
        public RouteValueDictionary RouteValues { get; set; }
    }
}