using System;
using System.Web.Security;
using System.Web.UI;

namespace Apollo.AdminStore.WebForm
{
    public partial class logout : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            FormsAuthentication.SignOut();
            Response.Redirect("/login.aspx");
        }
    }
}