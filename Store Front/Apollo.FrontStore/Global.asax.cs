using Apollo.Core.Caching;
using Apollo.Core.Infrastructure;
using Apollo.FrontStore.Infrastructure;
using Apollo.Web.Framework;
using FluentValidation.Mvc;
using log4net;
using System;
using System.Threading;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
[assembly: log4net.Config.XmlConfigurator(Watch = true)]

namespace Apollo.FrontStore
{
    public class MvcApplication : HttpApplication
    {
        private static ILog _logger = LogManager.GetLogger(typeof(MvcApplication).FullName);

        private const string APPLICATION_ERROR_HTML = @"<html>
                                                        <head>
                                                            <title>Apollo | Server Error</title>
                                                            <link rel=""stylesheet"" href=""https://maxcdn.bootstrapcdn.com/bootstrap/3.3.6/css/bootstrap.min.css"" integrity=""sha384-1q8mTJOASx8j1Au+a5WDVnPi2lkFfwwEAa8hDDdjZlpLegxhjVME1fgjWPGmkzs7"" crossorigin=""anonymous"">
                                                        </head>
                                                        <body class=""container"">
                                                            <div class=""row"">
                                                                <div class=""text-center"" style=""margin-top: 20px;"">
                                                                    <span class=""glyphicon glyphicon-alert"" style=""font-size: 120px; color: #fd5555;""></span>
                                                                    <div class=""col-sm-12""><h3>OOPS! Something wrong with our website.</h3></div>
                                                                    <p style =""width: 100%; font-size: 20px; font-family: 'Archivo Narrow', sans-serif; display: inline-block; text-transform: uppercase;"">We're really sorry.</p>
                                                                    <p style =""width: 100%; font-size: 15px; font-family: 'Archivo Narrow', sans-serif; display: inline-block; text-transform: uppercase;"">Please bear with us as we scramble to resolve the issue as quickly as possible. Stay tuned, we'll be restoring our regular service as soon as possible. Thanks for your patience.</p>
                                                                    <p style =""width: 100%; font-size: 15px; font-family: 'Archivo Narrow', sans-serif; display: inline-block; text-transform: uppercase;"">In the meantime, check out <a href=""https://twitter.com/Apollo"">our twitter account</a> for updates on the situation.</p>
                                                                </div>
                                                            </div>
                                                        </body>
                                                        </html>";
        protected void Application_Start()
        {
            //suppress the header but configure it in web.config
            AntiForgeryConfig.SuppressXFrameOptionsHeader = true;

            //initialize engine context
            EngineContext.Initialize(false);

            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            //fluent validation
            DataAnnotationsModelValidatorProvider.AddImplicitRequiredAttributeForValueTypes = false;
            ModelValidatorProviders.Providers.Add(new FluentValidationModelValidatorProvider(new ApolloValidatorFactory()));

            MvcHandler.DisableMvcResponseHeader = true;
            
            //register Custom Memory Cache for Child Action Method Caching
            var cacheManager = EngineContext.Current.ContainerManager.ResolveNamed<ICacheManager>("Apollo_cache_static");
            OutputCacheAttribute.ChildActionCache = new CustomMemoryCache("Apollo_child_action_cache", cacheManager);
        }

        protected void Application_PreSendRequestHeaders(object sender, EventArgs e)
        {
            HttpContext.Current.Response.Headers.Remove("Server");
            HttpContext.Current.Response.Headers.Remove("X-Powered-By");
        }

        protected void Application_Error(object sender, EventArgs e)
        {
            Exception ex = Server.GetLastError();
            if (ex is ThreadAbortException)
                return;
            _logger.Error(string.Format("Application Error, User Agent={{{2}}}, Http Method={{{1}}}, Exception={{{0}}}", 
                ex.Message, Request.HttpMethod, Request.UserAgent), ex);

            Server.ClearError();

            Response.Write(APPLICATION_ERROR_HTML);
        }        
    }
}
