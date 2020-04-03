using Apollo.Core.Infrastructure;
using Autofac.Integration.Web;
using log4net;
using System;
using System.Threading;
using System.Web;

[assembly: log4net.Config.XmlConfigurator(Watch = true)]

namespace Apollo.AdminStore.WebForm
{
    public class Global : HttpApplication, IContainerProviderAccessor
    {
        private static ILog _logger = LogManager.GetLogger(typeof(Global).FullName);

        static IContainerProvider _containerProvider;

        public IContainerProvider ContainerProvider
        {
            get { return _containerProvider; }
        }

        protected void Application_Start(object sender, EventArgs e)
        {
            //initialize engine context
            EngineContext.Initialize(false);
        }
        
        protected void Session_Start(object sender, EventArgs e)
        {
            
        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {

        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {

        }

        protected void Application_PreSendRequestHeaders(object sender, EventArgs e)
        {
            Response.Headers.Remove("Server");
            Response.Headers.Remove("X-AspNet-Version");
        }

        protected void Application_Error(object sender, EventArgs e)
        {
            Exception ex = Server.GetLastError();
            if (ex is ThreadAbortException)
                return;
            _logger.Error(string.Format("Application Error, Exception={{{0}}}", ex.Message), ex);

            Server.ClearError();
            Response.Write(string.Format(@"
                            <!DOCTYPE html>
                            <html>
                            <head>
                                <meta charset=""utf-8"">
                                <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
                                <title>Apollo WebAdmin</title>
                                <link href=""/css/inspinia/bootstrap.min.css"" rel=""stylesheet"">
                                <link href=""/css/inspinia/font-awesome.css"" rel=""stylesheet"">
                                <link href=""/css/inspinia/animate.css"" rel=""stylesheet"" >
                                <link href=""/css/inspinia/style.css"" rel=""stylesheet"" >
                            </head>
                            <body class=""gray-bg"">
                                <div class=""middle-box text-center animated fadeInDown"">
                                    <h1>500</h1>
                                    <h3 class=""font-bold"">Internal Server Error</h3>
                                    <div class=""error-desc"">
                                        We apologise. The server encountered something unexpected that didn't allow it to complete the request.<br/>
                                        You can go back to main page: <br/><a href=""/"" class=""btn btn-primary m-t"">Dashboard</a>
                                    </div>
                                    <p></p>
                                    <div class=""alert alert-warning text-left"">
                                        <b>Internal Report</b>
                                        <br/>
                                        <i>URL</i>:<br/>{0}<br/><br/>
                                        <i>Exception Message</i>:<br/>{1}
                                    </div>
                                </div>
                            <script src=""/js/inspinia/jquery-2.1.1.js""></script>
                            <script src=""/js/inspinia/bootstrap.min.js""></script>
                            </body>
                            </html>",
                            Request.Url.ToString(),
                            ex.InnerException != null ? ex.InnerException.Message : ex.Message));
        }

        protected void Session_End(object sender, EventArgs e)
        {

        }

        protected void Application_End(object sender, EventArgs e)
        {

        }        
    }
}