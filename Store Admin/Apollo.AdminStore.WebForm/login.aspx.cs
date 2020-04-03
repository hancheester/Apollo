using Apollo.AdminStore.WebForm.Classes;
using Apollo.Core.Model;
using Apollo.Core.Services.Interfaces;
using System;
using System.Web.Security;

namespace Apollo.AdminStore.WebForm
{
    public partial class login : BasePage
    {
        public IAccountService AccountService { get; set; }
        
        protected void Page_Load(object sender, EventArgs e)
        {
            if (User.Identity.IsAuthenticated)
            {
                Response.Redirect("/dashboard.aspx");
            }

            if (Request.HttpMethod == "POST")
            {
                string password = Request["password"];
                string email = Request["username"];
                
                var result = AccountService.ValidateUser(email, password, isRestrictedArea: true);
                var authenticated = false;
                switch (result)
                {
                    case CustomerLoginResults.NotPermitted:
                        ltMessage.Text = string.Format("<p class='text-danger'>{0}</p>", "You are not permitted to access this resource, please contact administrator.");
                        break;
                    case CustomerLoginResults.Error:
                        ltMessage.Text = string.Format("<p class='text-danger'>{0}</p>", "System error, please contact administrator.");
                        break;
                    case CustomerLoginResults.Successful:
                        authenticated = true;
                        FormsAuthentication.SetAuthCookie(email, createPersistentCookie: true);                        
                        break;
                    case CustomerLoginResults.MemberNotExists:
                        ltMessage.Text = string.Format("<p class='text-danger'>{0}</p>", "Membership is not found with this username.");
                        break;
                    case CustomerLoginResults.ProfileNotExists:
                        ltMessage.Text = string.Format("<p class='text-danger'>{0}</p>", "Profile is not found with this username.");
                        break;
                    case CustomerLoginResults.AccountNotExists:
                        ltMessage.Text = string.Format("<p class='text-danger'>{0}</p>", "Account is not found with this username.");
                        break;
                    case CustomerLoginResults.WrongPassword:
                        ltMessage.Text = string.Format("<p class='text-danger'>{0}</p>", "Wrong password, please try again later.");
                        break;
                    case CustomerLoginResults.IsLockedOut:
                        ltMessage.Text = string.Format("<p class='text-danger'>{0}</p>", "Your account is locked out, please contact administrator.");
                        break;
                    case CustomerLoginResults.NotApproved:
                        ltMessage.Text = string.Format("<p class='text-danger'>{0}</p>", "Your account is not approved, please contact administrator.");
                        break;
                    default:
                        break;
                }

                if (authenticated)
                {
                    if (string.IsNullOrEmpty(Request.QueryString["ReturnUrl"]))
                        Response.Redirect("/dashboard.aspx");
                    else
                        Response.Redirect(Request.QueryString["ReturnUrl"]);
                }
            }
        }
    }
}