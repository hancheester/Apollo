using Apollo.AdminStore.WebForm.Classes;
using Apollo.Core.Model.Entity;
using Apollo.Core.Services.Interfaces;
using System;
using System.Collections.Generic;

namespace Apollo.AdminStore.WebForm.Customer
{
    public partial class customer_new : BasePage
    {
        public IAccountService AccountService { get; set; }
        public AdminStoreUtility AdminStoreUtility { get; set; }

        protected override void OnInit(EventArgs e)
        {
            cblRoles.DataSource = AccountService.GetAllRoles();
            cblRoles.DataBind();

            base.OnInit(e);
        }

        protected void lbSave_Click(object sender, EventArgs e)
        {
            Account account = new Account
            {
                Name = txtName.Text.Trim(),
                Username = txtUsername.Text.Trim().ToLower(),
                Email = txtUsername.Text.Trim().ToLower(),
                ContactNumber = txtContact.Text.Trim(),
                DisplayContactNumberInDespatch = cbDisplayContactNumber.Checked,
                DOB = txtDOB.Text.Trim()
            };

            // Get assigned roles if any
            var roles = new List<string>();

            for (int i = 0; i < cblRoles.Items.Count; i++)
                if (cblRoles.Items[i].Selected)
                    roles.Add(cblRoles.Items[i].Value);

            account.Roles = roles.ToArray();
            
            // Get password
            string password;
            if (cbSendAutoPwd.Checked)
                password = AdminStoreUtility.GenerateRandomPasswordGUID(8);
            else
                password = txtPwd.Text.Trim();

            bool sendPassEmailFlag = cbSendAutoPwd.Checked;
            bool sendEmailFlag = sendPassEmailFlag ? false : cbWelcomeEmail.Checked;

            var result = AccountService.ProcessRegistration(account, password, sendEmailFlag, sendPassEmailFlag);

            if (!string.IsNullOrEmpty(result.Message))
            {
                ErrorSummary.AddError(result.Message, "vgNewCustomer", this.Page);
            }
            else
            {
                Response.Redirect("/customer/customer_info.aspx?" + QueryKey.MSG_TYPE + "=" + (int)MessageType.AccountCreated + "&userid=" + result.ProfileId.ToString());                
            }
        }
    }
}