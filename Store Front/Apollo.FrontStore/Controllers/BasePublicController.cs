using Apollo.Core.Infrastructure;
using Apollo.Web.Framework.Controllers;
using Apollo.Web.Framework.Security;
using System.Web.Mvc;
using System.Web.Routing;

namespace Apollo.FrontStore.Controllers
{
    [ApolloHttpsRequirement(SslRequirement.NoMatter)]
    public abstract partial class BasePublicController : BaseController
    {
        protected virtual ActionResult InvokeHttp404()
        {
            // Call target Controller and pass the routeData.
            IController errorController = EngineContext.Current.Resolve<CommonController>();

            var routeData = new RouteData();
            routeData.Values.Add("controller", "Common");
            routeData.Values.Add("action", "PageNotFound");

            errorController.Execute(new RequestContext(HttpContext, routeData));

            return new EmptyResult();
        }

        protected virtual ActionResult InvokeHttp401()
        {
            // Call target Controller and pass the routeData.
            IController errorController = EngineContext.Current.Resolve<CommonController>();

            var routeData = new RouteData();
            routeData.Values.Add("controller", "Common");
            routeData.Values.Add("action", "Unauthorized");

            errorController.Execute(new RequestContext(HttpContext, routeData));

            return new EmptyResult();
        }

        protected virtual ActionResult InvokeHttp400()
        {
            // Call target Controller and pass the routeData.
            IController errorController = EngineContext.Current.Resolve<CommonController>();

            var routeData = new RouteData();
            routeData.Values.Add("controller", "Common");
            routeData.Values.Add("action", "BadRequest");

            errorController.Execute(new RequestContext(HttpContext, routeData));

            return new EmptyResult();
        }

        protected virtual ActionResult InvokeHttp410()
        {
            // Call target Controller and pass the routeData.
            IController errorController = EngineContext.Current.Resolve<CommonController>();

            var routeData = new RouteData();
            routeData.Values.Add("controller", "Common");
            routeData.Values.Add("action", "Gone");

            errorController.Execute(new RequestContext(HttpContext, routeData));

            return new EmptyResult();
        }
    }
}