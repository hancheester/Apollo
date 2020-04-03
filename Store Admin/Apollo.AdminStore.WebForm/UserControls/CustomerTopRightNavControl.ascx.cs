using Apollo.AdminStore.WebForm.Classes;
using Apollo.Core.Model;
using Apollo.Core.Services.Interfaces;
using System;

namespace Apollo.AdminStore.WebForm.UserControls
{
    public partial class UserControls_CustomerTopRightNavControl : BaseUserControl
    {
        public IAccountService AccountService { get; set; }

        public delegate void OrderNavEventHandler(string message, bool refresh);
        public event OrderNavEventHandler ActionOccurred;

        public string SetBackUrl
        {
            set
            {
                lbBack.PostBackUrl = value;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            LoadButtons();

            if (string.IsNullOrEmpty(lbBack.PostBackUrl))
                lbBack.PostBackUrl = "/customer/customer_default.aspx";
        }

        protected int QueryUserId
        {
            get { return ((BasePage)Page).QueryUserId; }
        }

        protected void lbDisapprove_Click(object sender, EventArgs e)
        {
            int profileId = QueryUserId;
            
            if (profileId > 0)
            {
                AccountService.DisableUser(profileId);

                // Display message
                InvokeNewMessage("Account was updated successfully.", true);

                // Load this page again
                LoadButtons();
            }
        }

        protected void lbApprove_Click(object sender, EventArgs e)
        {
            int profileId = QueryUserId;

            if (profileId > 0)
            {
                AccountService.EnableUser(profileId);

                // Display message
                InvokeNewMessage("Account was updated successfully.", true);

                // Load this page again
                LoadButtons();
            }
        }

        protected void lbUnlock_Click(object sender, EventArgs e)
        {
            int profileId = QueryUserId;

            if (profileId > 0)
            {
                AccountService.UnlockUser(profileId);

                // Display message
                InvokeNewMessage("Account was unlocked successfully.", true);

                // Load this page again
                LoadButtons();
            }
        }

        protected void lbBack_Click(object sender, EventArgs e)
        {
            Response.Redirect(lbBack.PostBackUrl);
        }

        private void LoadButtons()
        {
            lbDisapprove.Visible = false;
            lbApprove.Visible = false;

            int profileId = QueryUserId;

            var account = AccountService.GetAccountByProfileId(profileId);

            if (account != null)
            {
                lbDisapprove.Visible = account.IsApproved;
                lbApprove.Visible = !account.IsApproved;
                lbUnlock.Visible = account.IsLockedOut;
                lbUnsubscribe.Visible = account.IsSubscribed;
                lbSubsribe.Visible = !account.IsSubscribed;
            }
        }

        private void InvokeNewMessage(string message, bool refresh)
        {
            OrderNavEventHandler handler = ActionOccurred;
            if (handler != null)
                handler(message, refresh);
        }

        protected void lbUnsubscribe_Click(object sender, EventArgs e)
        {
            int profileId = QueryUserId;

            if (profileId > 0)
            {
                var result = AccountService.ProcessEmailSubsciptionCancellation(profileId);

                switch (result)
                {
                    case SubscriptionResults.Error:
                        InvokeNewMessage("Failed to cancel subscription. Please contact administrator.", true);
                        break;
                    case SubscriptionResults.Successful:
                        InvokeNewMessage("Account unsubscribed successfully.", true);
                        break;
                    case SubscriptionResults.InvalidEmail:
                        InvokeNewMessage("Failed to cancel subscription. Invalid email address. Please contact administrator.", true);
                        break;
                    default:
                        break;
                }                

                // Load this page again
                LoadButtons();

            }
        }

        protected void lbSubscribe_Click(object sender, EventArgs e)
        {
            int profileId = QueryUserId;

            if (profileId > 0)
            {
                var email = AccountService.GetEmailByProfileId(profileId);
                var result = AccountService.ProcessNewEmailSubscription(email);

                switch (result)
                {
                    case SubscriptionResults.Error:
                        InvokeNewMessage("Failed to subscribe. Please contact administrator.", true);
                        break;
                    case SubscriptionResults.Successful:
                        InvokeNewMessage("Account subscribed successfully.", true);
                        break;
                    case SubscriptionResults.InvalidEmail:
                        InvokeNewMessage("Failed to subscribe. Invalid email address. Please contact administrator.", true);
                        break;
                    default:
                        break;
                }

                // Load this page again
                LoadButtons();

            }
        }
    }
}