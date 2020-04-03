using System;
using System.IO;
using System.IO.Compression;
using System.Web;

namespace Apollo.FrontStore.Infrastructure
{
    public class SvgCompressionModule : IHttpModule
    {
        public void Init(HttpApplication application)
        {
            application.BeginRequest += ApplicationOnBeginRequest;
        }

        public void Dispose()
        {
        }

        private void ApplicationOnBeginRequest(object sender, EventArgs eventArgs)
        {
            var app = (HttpApplication)sender;
            if (app == null)
                return;
            var context = app.Context;
            if (!IsSvgRequest(context.Request))
                return;
            context.Response.Filter = new GZipStream(context.Response.Filter, CompressionMode.Compress);
            context.Response.AddHeader("Content-encoding", "gzip");
        }

        protected virtual bool IsSvgRequest(HttpRequest request)
        {
            var path = request.Url.AbsolutePath;
            return Path.HasExtension(path) &&
                   Path.GetExtension(path).Equals(".svg", StringComparison.InvariantCultureIgnoreCase);
        }
    }
}