using Apollo.Web.Framework.UI.Paging;
using System.Web.Mvc;

namespace Apollo.FrontStore.Extensions
{
    public static class HtmlExtensions
    {
        public static Pager Pager(this HtmlHelper helper, IPageableModel pagination)
        {
            return new Pager(pagination, helper.ViewContext);
        }
    }
}