using Apollo.AdminStore.WebForm.Classes;
using Apollo.Core.Domain.Directory;
using Apollo.Core.Model.Entity;
using Apollo.Core.Services.Interfaces;
using System;
using System.Text;

namespace Apollo.AdminStore.WebForm.Customer
{
    public partial class customer_info : BasePage
    {
        public IAccountService AccountService { get; set; }
        public IOrderService OrderService { get; set; }
        public ICartService CartService { get; set; }
        public AdminStoreUtility AdminStoreUtility { get; set; }        
        public CurrencySettings CurrencySettings { get; set; }

        protected override void OnInit(EventArgs e)
        {
            int profileId = QueryUserId;

            if (profileId <= 0)
                Response.Redirect("/customer/customer_default.aspx?" + QueryKey.MSG_TYPE + "=" + (int)MessageType.AccountIdInvalid);

            Account account = AccountService.GetAccountByProfileId(profileId);

            if (account != null)
                LoadAccount(account);
            else
                Response.Redirect("/customer/customer_default.aspx?" + QueryKey.MSG_TYPE + "=" + (int)MessageType.AccountNotFound);
            
            base.OnInit(e);
        }

        private void LoadAccount(Account account)
        {
            ltlTitle.Text = string.Format("{0} (ID: {1}){2}{3}", account.Name, account.Id.ToString(), !account.IsApproved ? " <i class='fa fa-thumbs-down' aria-hidden='true'></i>" : null, account.IsLockedOut ? " <i class='fa fa-lock' aria-hidden='true'></i>" : null);
            ltlLastLoggedIn.Text = account.LastLoginDate.ToLongDateString() + COMMA + account.LastLoginDate.ToLongTimeString();
            ltlLastActivityIn.Text = account.LastActvitityDate.ToLongDateString() + COMMA + account.LastActvitityDate.ToLongTimeString();
            ltlAccCreatedOn.Text = account.CreationDate.ToLongDateString() + COMMA + account.CreationDate.ToLongTimeString();
            ltlCustGroup.Text = string.Join(COMMA, account.Roles);
            ltlNote.Text = account.Note;

            var pointsBalance = AccountService.GetLoyaltyPointsBalanceByAccountId(account.Id);
            ltlLoyaltyPoints.Text = pointsBalance.ToString();
            
            Address primaryBilling = AccountService.GetBillingAddressByAccountId(account.Id);

            if (primaryBilling != null)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append(primaryBilling.Name != string.Empty ? primaryBilling.Name + HtmlElement.BR : string.Empty);
                sb.Append(primaryBilling.AddressLine1 != string.Empty ? primaryBilling.AddressLine1 + HtmlElement.BR : string.Empty);
                sb.Append(primaryBilling.AddressLine2 != string.Empty ? primaryBilling.AddressLine2 + HtmlElement.BR : string.Empty);
                sb.Append(primaryBilling.City).Append(COMMA).Append(HtmlElement.SPACE).Append(primaryBilling.County != string.Empty ? primaryBilling.County + COMMA + HtmlElement.SPACE : string.Empty);
                sb.Append(primaryBilling.PostCode != string.Empty ? primaryBilling.PostCode : string.Empty).Append(HtmlElement.BR);
                sb.Append(primaryBilling.Country.Name);

                ltlBillingAddr.Text = sb.ToString();
            }

            rptOrders.DataSource = OrderService.GetRecentOrderListByProfileId(account.ProfileId, 5);
            rptOrders.DataBind();

            int cartItemQuantity = CartService.GetTotalQuantityCartItemByProfileId(account.ProfileId, false);
            ltlShoppingCart.Text = string.Format("Shopping cart - {0} item(s)", cartItemQuantity.ToString());

            rptShoppingCart.DataSource = CartService.GetCartItemsByProfileId(account.ProfileId, false);
            rptShoppingCart.DataBind();
        }

        protected void ectTogRightNav_ActionOccurred(string message, bool refresh)
        {
            enbNotice.Message = message;
            if (refresh) LoadAccount(AccountService.GetAccountByProfileId(QueryUserId));
        }
    }
}