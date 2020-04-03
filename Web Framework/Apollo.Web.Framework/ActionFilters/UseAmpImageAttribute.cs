using HtmlAgilityPack;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;

namespace Apollo.Web.Framework.ActionFilters
{
    public class UseAmpImageAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (filterContext.IsChildAction == false)
            {
                var stringBuilder = new StringBuilder();
                var stringWriter = new StringWriter(stringBuilder);
                var htmlTextWriter = new HtmlTextWriter(stringWriter);
                var output = (HttpWriter)filterContext.RequestContext.HttpContext.Response.Output;
                filterContext.RequestContext.HttpContext.Response.Output = htmlTextWriter;
                filterContext.HttpContext.Items["__private_value__sb"] = stringBuilder;
                filterContext.HttpContext.Items["__private_value__output"] = output;
            }
            else
            {
                base.OnActionExecuting(filterContext);
            }            
        }

        public override void OnResultExecuted(ResultExecutedContext filterContext)
        {
            if (filterContext.IsChildAction == false)
            {
                var stringBuilder = (StringBuilder)filterContext.HttpContext.Items["__private_value__sb"];
                var response = stringBuilder.ToString();
                response = UpdateAmpImages(response);

                var output = (HttpWriter)filterContext.HttpContext.Items["__private_value__output"];
                output.Write(response);
            }
            else
            {
                base.OnResultExecuted(filterContext);
            }
        }

        private string UpdateAmpImages(string response)
        {
            // Use HtmlAgilityPack (install-package HtmlAgilityPack)
            var doc = GetHtmlDocument(response);
            var imageList = doc.DocumentNode.Descendants("img");

            const string ampImage = "amp-img";

            if (!imageList.Any()) return response;

            if (!HtmlNode.ElementsFlags.ContainsKey("amp-img"))
            {
                HtmlNode.ElementsFlags.Add("amp-img", HtmlElementFlag.Closed);
            }

            foreach (var imgTag in imageList)
            {
                var original = imgTag.OuterHtml;
                var replacement = imgTag.Clone();
                replacement.Name = ampImage;
                response = response.Replace(original, replacement.OuterHtml);
            }

            return response;
        }

        private HtmlDocument GetHtmlDocument(string htmlContent)
        {
            var doc = new HtmlDocument
            {
                OptionOutputAsXml = true,
                OptionDefaultStreamEncoding = Encoding.UTF8
            };
            doc.LoadHtml(htmlContent);
            return doc;
        }
    }
}
