using Apollo.AdminStore.WebForm.Classes;
using Apollo.Core.Services.Interfaces;
using System;

namespace Apollo.AdminStore.WebForm.UserControls
{
    public partial class UserControls_OrderAccountViewControl : BaseUserControl
    {
        public IAccountService AccountService { get; set; }
        public IOrderService OrderService { get; set; }

        public delegate void AccountEventHandler(SysCheckType type, bool verified);

        public event AccountEventHandler Verified;

        private bool _checkPassed;
        private bool _checkNamePassed;

        public int ProfileId
        {
            set
            {
                var account = AccountService.GetAccountOverviewModelByProfileId(value);

                if (account != null)
                {
                    ltlCustName.Text = account.Name;
                    ltlCustEmail.Text = "<a href='mailto:" + account.Email + "'>" + account.Email + "</a>";
                    ltlCustContact.Text = account.ContactNumber;
                    ltlCustDOB.Text = account.DOB;

                    hlProfile.Text = "view";
                    hlProfile.NavigateUrl = "/customer/customer_info.aspx?userid=" + value.ToString();

                    // Get number of paid orders
                    int count = OrderService.GetPaidOrderCountByProfileId(account.ProfileId);

                    ltlNumberOfOrders.Text = count.ToString();

                    DateTime? creationDate = AccountService.GetAccountCreationDateByUsername(account.Email);

                    if (creationDate.HasValue)
                        ltlAccCreatedOn.Text = creationDate.Value.ToLongDateString() + ", " + creationDate.Value.ToLongTimeString();

                    ltlNote.Text = account.Note;
                }
            }
        }

        public int OrderId
        {
            set
            {
                var sysCheck = OrderService.GetSystemCheckByOrderId(value);

                if (sysCheck != null)
                {
                    _checkNamePassed = sysCheck.NameCheck;
                    _checkPassed = sysCheck.EmailCheck;
                }
                else
                {
                    _checkNamePassed = true;
                    _checkPassed = true; // Default value
                }

                if (_checkPassed)
                    phEmail.Visible = false;
                else
                    phEmail.Visible = true;

                if (_checkNamePassed)
                    phName.Visible = false;
                else
                    phName.Visible = true;
            }
        }

        protected bool GetCheckStatus()
        {
            return _checkPassed;
        }

        protected bool GetNameCheckStatus()
        {
            return _checkNamePassed;
        }

        protected void lbVerify_Click(object sender, EventArgs e)
        {
            InvokeVerified(SysCheckType.Email, true);
        }

        protected void lbVerifyName_Click(object sender, EventArgs e)
        {
            InvokeVerified(SysCheckType.Name, true);
        }

        private void InvokeVerified(SysCheckType type, bool verified)
        {
            AccountEventHandler handler = Verified;
            if (handler != null)
                handler(type, verified);
        }
    }
}