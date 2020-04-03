using Autofac;
using Autofac.Integration.Web;
using System;
using System.Web;
using System.Web.UI;

namespace Apollo.AdminStore.WebForm.Classes
{
    public class BaseUserControl : UserControl
    {
        #region ========== Query Handler ==========

        protected string GetStringQuery(string key)
        {
            if (Request.QueryString[key] != null)
                return Request.QueryString[key];
            else
                return string.Empty;
        }

        protected int GetIntQuery(string key)
        {
            if ((Request.QueryString[key] != null)
                        && (RegexType.Integer.Match(Request.QueryString[key]).Success))
                return Convert.ToInt32(Request.QueryString[key]);
            else
                return 0;
        }

        #endregion

        public BaseUserControl()
        {
            var cpa = (IContainerProviderAccessor)HttpContext.Current.ApplicationInstance;
            var cp = cpa.ContainerProvider;
            cp.RequestLifetime.InjectProperties(this);
        }
    }
}