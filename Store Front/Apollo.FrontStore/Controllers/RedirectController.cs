using Apollo.Web.Framework.Security;
using System.Web.Mvc;

namespace Apollo.FrontStore.Controllers
{
    [ApolloHttpsRequirement(SslRequirement.No)]
    public class RedirectController : BasePublicController
    {
        public ActionResult RedirectToHome()
        {
            return RedirectPermanent(Url.RouteUrl("Home"));
        }

        public ActionResult RedirectToSpecialOffers()
        {
            return RedirectPermanent(Url.RouteUrl("Special Offers"));
        }

        public ActionResult RedirectToProduct(string urlkey)
        {
            urlkey = urlkey.Remove(0, "p-".Length);
            return RedirectPermanent(Url.RouteUrl("Product", new { @urlkey = urlkey }));
        }

        public ActionResult RedirectToCategory(string urlkey)
        {
            urlkey = urlkey.Remove(0, "c-".Length);
            return RedirectPermanent(Url.RouteUrl("Category", new { @top = urlkey }));
        }

        public ActionResult RedirectToBrand(string urlkey)
        {
            return RedirectPermanent(Url.RouteUrl("Brand", new { @urlKey = urlkey }));
        }
    }
}