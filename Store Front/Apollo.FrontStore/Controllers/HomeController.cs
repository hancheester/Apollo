using Apollo.Web.Framework.Security;
using System.Web.Mvc;

namespace Apollo.FrontStore.Controllers
{
#if DEBUG
    [ApolloHttpsRequirement(SslRequirement.No)]
#else
    [ApolloHttpsRequirement(SslRequirement.Yes)]
#endif
    public class HomeController : BasePublicController
    {
        public ActionResult Index()
        {
            return View();
        }
    }
}