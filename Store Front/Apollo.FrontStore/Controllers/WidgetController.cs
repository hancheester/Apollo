using Apollo.Core.Caching;
using Apollo.Core.Services.Interfaces.Cms;
using Apollo.FrontStore.Models.Cms;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Routing;

namespace Apollo.FrontStore.Controllers
{
    public class WidgetController : BasePublicController
    {
        #region Constants

        private const string WIDGET_BY_STORE_ID_ZONE = "widget.storeid-{0}.zone-{1}";        

        #endregion

        #region Fields

        private readonly IWidgetService _widgetService;
        //private readonly IStoreContext _storeContext;
        private readonly ICacheManager _cacheManager;

        #endregion

        #region Ctor

        public WidgetController(IWidgetService widgetService, ICacheManager cacheManager)
        {
            _widgetService = widgetService;
            //this._storeContext = storeContext;
            _cacheManager = cacheManager;
        }

        #endregion

        #region Methods

        [ChildActionOnly]
        public ActionResult WidgetsByZone(string widgetZone, object additionalData = null)
        {
            //var cacheKey = string.Format("widget-{0}-{1}", _storeContext.CurrentStore.Id, widgetZone);
            var cacheKey = string.Format(WIDGET_BY_STORE_ID_ZONE, "0", widgetZone);
            var cacheModel = _cacheManager.Get(cacheKey, () =>
            {
                //model
                var model = new List<RenderWidgetModel>();

                //var widgets = _widgetService.LoadActiveWidgetsByWidgetZone(widgetZone, _storeContext.CurrentStore.Id);
                var widgets = _widgetService.LoadActiveWidgetsByWidgetZone(widgetZone);
                foreach (var widget in widgets)
                {
                    var widgetModel = new RenderWidgetModel();

                    string actionName;
                    string controllerName;
                    RouteValueDictionary routeValues;
                    widget.GetDisplayWidgetRoute(widgetZone, out actionName, out controllerName, out routeValues);
                    widgetModel.ActionName = actionName;
                    widgetModel.ControllerName = controllerName;
                    widgetModel.RouteValues = routeValues;

                    model.Add(widgetModel);
                }
                return model;
            });

            //no data?
            if (cacheModel.Count == 0)
                return Content("");

            //"RouteValues" property of widget models depends on "additionalData".
            //We need to clone the cached model before modifications (the updated one should not be cached)
            var clonedModel = new List<RenderWidgetModel>();
            foreach (var widgetModel in cacheModel)
            {
                var clonedWidgetModel = new RenderWidgetModel();
                clonedWidgetModel.ActionName = widgetModel.ActionName;
                clonedWidgetModel.ControllerName = widgetModel.ControllerName;
                if (widgetModel.RouteValues != null)
                    clonedWidgetModel.RouteValues = new RouteValueDictionary(widgetModel.RouteValues);

                if (additionalData != null)
                {
                    if (clonedWidgetModel.RouteValues == null)
                        clonedWidgetModel.RouteValues = new RouteValueDictionary();
                    clonedWidgetModel.RouteValues.Add("additionalData", additionalData);
                }

                clonedModel.Add(clonedWidgetModel);
            }

            return PartialView(clonedModel);
        }

        #endregion
    }
}