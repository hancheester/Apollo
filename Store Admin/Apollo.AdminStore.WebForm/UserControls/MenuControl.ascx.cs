using System;
using System.Web;
using System.Web.Security;
using System.Web.UI;

namespace Apollo.AdminStore.WebForm.UserControls
{
    public partial class MenuControl : UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            var name = Convert.ToString(HttpContext.Current.Profile.GetPropertyValue("FullName"));
            ltProfileName.Text = name;

            //var roles = Roles.GetRolesForUser();
            //ltRoles.Text = string.Join(",<br/>", roles);
        }

        protected string GenerateMenuItem(string item, string roles)
        {
            string[] arrRoles = roles.Split(',');

            if (roles != string.Empty && arrRoles.Length > 0)
            {
                for (int i = 0; i < arrRoles.Length; i++)
                {
                    if (Roles.IsUserInRole(arrRoles[i]))
                    {
                        return item;
                    }
                }
            }

            return string.Empty;
        }
    }
}