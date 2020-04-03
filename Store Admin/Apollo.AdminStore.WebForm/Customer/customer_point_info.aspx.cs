using Apollo.AdminStore.WebForm.Classes;
using Apollo.Core.Model.Entity;
using Apollo.Core.Services.Interfaces;
using System;
using System.Web.UI.WebControls;

namespace Apollo.AdminStore.WebForm.Customer
{
    public partial class customer_point_info : BasePage
    {
        public IAccountService AccountService { get; set; }

        protected override void OnInit(EventArgs e)
        {
            int profileId = QueryUserId;

            if (profileId <= 0)
                Response.Redirect("/customer/customer_default.aspx?" + QueryKey.MSG_TYPE + "=" + (int)MessageType.AccountIdInvalid);

            Account account = AccountService.GetAccountByProfileId(profileId);

            if (account != null)
            {
                ltlTitle.Text = string.Format("{0} (ID: {1}){2}{3}", account.Name, account.Id.ToString(), !account.IsApproved ? " <i class='fa fa-thumbs-down' aria-hidden='true'></i>" : null, account.IsLockedOut ? " <i class='fa fa-lock' aria-hidden='true'></i>" : null);
                LoadHistory(profileId);
            }
            else
                Response.Redirect("/customer/customer_default.aspx?" + QueryKey.MSG_TYPE + "=" + (int)MessageType.AccountNotFound);

            base.OnInit(e);
        }

        protected void ectTogRightNav_ActionOccurred(string message, bool refresh)
        {
            enbNotice.Message = message;
            if (refresh)
            {
                int profileId = QueryUserId;
                LoadHistory(profileId);
            }
        }

        protected void btnGoPage_Click(object sender, EventArgs e)
        {
            int gotoIndex = Convert.ToInt32(((TextBox)gvPoints.TopPagerRow.FindControl("txtPageIndex")).Text.Trim()) - 1;

            if ((gvPoints.CustomPageCount > gotoIndex) && (gotoIndex >= 0))
                gvPoints.CustomPageIndex = gotoIndex;

            int profileId = QueryUserId;
            LoadHistory(profileId);
        }

        protected void gvPoints_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvPoints.CustomPageIndex = gvPoints.CustomPageIndex + e.NewPageIndex;

            if (gvPoints.CustomPageIndex < 0)
                gvPoints.CustomPageIndex = 0;

            int profileId = QueryUserId;
            LoadHistory(profileId);
        }

        private void LoadHistory(int profileId)
        {
            Account account = AccountService.GetAccountByProfileId(profileId);
            
            var result = AccountService.GetPagedRewardPointHistory(
                pageIndex: gvPoints.CustomPageIndex,
                pageSize: gvPoints.PageSize,
                accountId: account.Id);

            if (result != null)
            {
                gvPoints.DataSource = result.Items;
                gvPoints.RecordCount = result.TotalCount;
                gvPoints.CustomPageCount = result.TotalPages;
            }

            gvPoints.DataBind();

            if (gvPoints.Rows.Count <= 0) enbNotice.Message = "No records found.";
        }
    }
}