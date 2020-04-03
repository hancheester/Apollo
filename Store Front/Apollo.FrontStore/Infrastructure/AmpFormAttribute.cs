using System;
using System.Web.Mvc;

namespace Apollo.FrontStore.Infrastructure
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
    public class AmpFormAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            var request = filterContext.HttpContext.Request;
            string baseUrl = request.Url.Scheme + "://" + request.Url.Authority + request.ApplicationPath.TrimEnd('/');
            filterContext.HttpContext.Response.AddHeader("Access-Control-Allow-Origin", baseUrl);
            filterContext.HttpContext.Response.AddHeader("AMP-Access-Control-Allow-Source-Origin", baseUrl);
            base.OnActionExecuted(filterContext);
        }
    }
}