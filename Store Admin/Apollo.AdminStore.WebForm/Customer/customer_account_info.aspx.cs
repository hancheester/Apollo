using Apollo.AdminStore.WebForm.Classes;
using Apollo.Core.Model;
using Apollo.Core.Model.Entity;
using Apollo.Core.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Security;
using System.Web.UI.WebControls;

namespace Apollo.AdminStore.WebForm.Customer
{
    public partial class customer_account_info : BasePage
    {
        private static readonly log4net.ILog _logger = log4net.LogManager.GetLogger(typeof(customer_account_info).FullName);

        public IAccountService AccountService { get; set; }
        public AdminStoreUtility AdminStoreUtility { get; set; }

        protected override void OnInit(EventArgs e)
        {
            int profileId = QueryUserId;

            if (profileId <= 0)
                Response.Redirect("/customer/customer_default.aspx?" + QueryKey.MSG_TYPE + "=" + (int)MessageType.AccountIdInvalid);
            
            var account = AccountService.GetAccountByProfileId(profileId);

            if (account != null)
            {
                cblRoles.DataSource = AccountService.GetAllRoles();
                cblRoles.DataBind();

                LoadUserInfo(account);
            }
            else
                Response.Redirect("/customer/customer_default.aspx?" + QueryKey.MSG_TYPE + "=" + (int)MessageType.AccountNotFound);
            
            base.OnInit(e);
        }

        private void LoadUserInfo(Account account)
        {
            ltlTitle.Text = string.Format("{0} (ID: {1}){2}{3}", account.Name, account.Id.ToString(), !account.IsApproved ? " <i class='fa fa-thumbs-down' aria-hidden='true'></i>" : null, account.IsLockedOut ? " <i class='fa fa-lock' aria-hidden='true'></i>" : null);
            txtName.Text = account.Name;
            txtPhone.Text = account.ContactNumber;
            cbDisplayContactNumber.Checked = account.DisplayContactNumberInDespatch;
            txtNote.Text = account.Note;

            var pointsBalance = AccountService.GetLoyaltyPointsBalanceByAccountId(account.Id);
            txtLoyaltyPoint.Text = pointsBalance.ToString();
            
            txtUsername.Text = account.Email;
            txtDOB.Text = account.DOB;

            var isAdministrator = Roles.IsUserInRole("Administrator");

            for (int i = 0; i < cblRoles.Items.Count; i++)
            {
                if (account.Roles.Contains(cblRoles.Items[i].Value))
                    cblRoles.Items[i].Selected = true;
            }

            cblRoles.Enabled = isAdministrator;

            var hasPassword = AccountService.HasPassword(account.Username);
            phHasNoPassword.Visible = !hasPassword;

            var logins = AccountService.GetLogins(account.Username);
            rptCredentials.DataSource = logins;
            rptCredentials.DataBind();
            phCredentials.Visible = logins.Count > 0;
        }

        protected void lbSave_Click(object sender, EventArgs e)
        {
            var account = AccountService.GetAccountByProfileId(QueryUserId);

            if (account == null)
            {
                _logger.ErrorFormat("Account could not be loaded. Account ID={{{0}}}", QueryUserId);
                Response.Redirect("/customer/customer_default.aspx?" + QueryKey.MSG_TYPE + "=" + (int)MessageType.AccountNotLoaded);
            }

            account.Name = txtName.Text.Trim();
            account.DOB = txtDOB.Text.Trim();
            account.ContactNumber = txtPhone.Text.Trim();
            account.DisplayContactNumberInDespatch = cbDisplayContactNumber.Checked;
            account.Note = txtNote.Text.Trim();
            account.Email = txtUsername.Text.Trim();

            // Set roles
            var isAdministrator = Roles.IsUserInRole("Administrator");
            if (isAdministrator)
            {
                List<string> roles = new List<string>();
                for (int i = 0; i < cblRoles.Items.Count; i++)
                {
                    if (cblRoles.Items[i].Selected)
                    {
                        roles.Add(cblRoles.Items[i].Value);
                    }
                }

                account.Roles = roles.ToArray();
            }
            
            string newPassword = string.Empty;

            // TODO: Need to change for setting/adding password
            // Set new password
            if (txtPwd.Text.Trim() != string.Empty && !cbSendAutoPwd.Checked)
            {
                newPassword = txtPwd.Text.Trim();
            }

            // TODO: Need to change for setting/adding password
            // Auto generate password
            if (cbSendAutoPwd.Checked)
            {
                newPassword = AdminStoreUtility.GenerateRandomPasswordGUID(8);
            }

            var points = Convert.ToInt32(txtLoyaltyPoint.Text.Trim());

            var result = AccountService.ProcessAccountUpdate(account, points, cbNewUsername.Checked);
            var message = string.Empty;

            switch (result)
            {
                case AccountUpdateResults.Successful:
                    message = "Account was updated successfully.";
                    break;
                case AccountUpdateResults.MemberNotExist:
                    message = "Account was failed to update. Membership could not be found.";
                    break;
                case AccountUpdateResults.ExistingEmail:
                    message = "Account was failed to update. Another existing membership with same email is found.";
                    break;
                default:
                    break;
            }

            txtPwd.Text = string.Empty;
            cbSendAutoPwd.Checked = false;
            cbNotificationEmail.Checked = false;
            
            enbNotice.Message = message;
        }

        protected void ectTogRightNav_ActionOccurred(string message, bool refresh)
        {
            enbNotice.Message = message;
            if (refresh) LoadUserInfo(AccountService.GetAccountByProfileId(QueryUserId));
        }

        protected void rptCredentials_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "remove")
            {
                var account = AccountService.GetAccountByProfileId(QueryUserId);
                var login = e.CommandArgument.ToString().Split('|');
                AccountService.RemoveLoginAndReturnIdentity(account.Username, login[0], login[1]);

                var logins = AccountService.GetLogins(account.Username);
                rptCredentials.DataSource = logins;
                rptCredentials.DataBind();
                phCredentials.Visible = logins.Count > 0;

                enbNotice.Message = "Credential was removed successfully.";
            }
        }

        protected void lbUpdatePassword_Click(object sender, EventArgs e)
        {
            var account = AccountService.GetAccountByProfileId(QueryUserId);
            var hasPassword = AccountService.HasPassword(account.Username);

            if (cbSendAutoPwd.Checked)
            {
                AccountService.ProcessPasswordReset(account.Email, cbNotificationEmail.Checked);
            }
            else
            {
                if (hasPassword)
                {
                    AccountService.ChangePassword(account.Username, txtPwd.Text.Trim(), cbNotificationEmail.Checked);
                }
                else
                {
                    AccountService.SetPassword(account.Username, txtPwd.Text.Trim(), cbNotificationEmail.Checked);
                }
            }

            var logins = AccountService.GetLogins(account.Username);
            rptCredentials.DataSource = logins;
            rptCredentials.DataBind();
            phCredentials.Visible = logins.Count > 0;
            phHasNoPassword.Visible = false;

            enbNotice.Message = "Password was updated successfully.";
        }
    }
}