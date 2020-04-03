using Apollo.AdminStore.WebForm.Classes;
using Apollo.Core.Domain.Directory;
using Apollo.Core.Model;
using Apollo.Core.Model.Entity;
using Apollo.Core.Model.OverviewModel;
using Apollo.Core.Services.Interfaces;
using System;
using System.Linq;
using System.Web.UI.WebControls;

namespace Apollo.AdminStore.WebForm.Customer
{
    public partial class customer_cart_info : BasePage
    {
        public IAccountService AccountService { get; set; }
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
                ltlTitle.Text = string.Format("{0} (ID: {1}){2}{3}", account.Name, account.Id.ToString(), !account.IsApproved ? " <i class='fa fa-thumbs-down' aria-hidden='true'></i>" : null, account.IsLockedOut ? " <i class='fa fa-lock' aria-hidden='true'></i>" : null);
            else
                Response.Redirect("/customer/customer_default.aspx?" + QueryKey.MSG_TYPE + "=" + (int)MessageType.AccountNotFound);
            
            base.OnInit(e);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadProducts();
            }
        }

        protected void lbResetFilter_Click(object sender, EventArgs e)
        {
            DisposeState(PRODUCT_ID_FILTER);
            DisposeState(NAME_FILTER);

            LoadProducts();
        }

        protected void lbSearch_Click(object sender, EventArgs e)
        {
            SetState(PRODUCT_ID_FILTER, ((TextBox)gvCart.HeaderRow.FindControl("txtFilterId")).Text.Trim());
            SetState(NAME_FILTER, ((TextBox)gvCart.HeaderRow.FindControl("txtFilterName")).Text.Trim());
            
            LoadProducts();
        }

        protected void btnGoPage_Click(object sender, EventArgs e)
        {
            int gotoIndex = Convert.ToInt32(((TextBox)gvCart.TopPagerRow.FindControl("txtPageIndex")).Text.Trim()) - 1;

            if ((gvCart.CustomPageCount > gotoIndex) && (gotoIndex >= 0))
                gvCart.CustomPageIndex = gotoIndex;

            LoadProducts();
        }

        protected void gvCart_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvCart.CustomPageIndex = gvCart.CustomPageIndex + e.NewPageIndex;

            if (gvCart.CustomPageIndex < 0)
                gvCart.CustomPageIndex = 0;

            LoadProducts();
        }

        protected void gvCart_Sorting(object sender, GridViewSortEventArgs e)
        {
            SetState(gvCart.ID + "sortexpression", e.SortExpression);
            LoadProducts();
        }

        protected void gvCart_PreRender(object sender, EventArgs e)
        {
            if (gvCart.TopPagerRow != null)
            {
                gvCart.TopPagerRow.Visible = true;
                ((TextBox)gvCart.HeaderRow.FindControl("txtFilterId")).Text = GetStringState(PRODUCT_ID_FILTER);
                ((TextBox)gvCart.HeaderRow.FindControl("txtFilterName")).Text = GetStringState(NAME_FILTER);
            }
        }

        protected void gvCart_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                if (e.Row.DataItem != null)
                    e.Row.Attributes.Add("onclick", "window.location='/catalog/product_info.aspx?productid=" + ((CartItemOverviewModel)e.Row.DataItem).ProductId.ToString() + "'");
                e.Row.Style.Add("cursor", "pointer");
            }
        }

        protected void gvCart_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            switch (e.CommandName)
            {
                case "remove":
                    CartService.DeleteCartItemsByProfileIdAndPriceId(QueryUserId, Convert.ToInt32(e.CommandArgument), freeItemIncluded: true);
                    LoadProducts();
                    break;
                default:
                    break;
            }
        }

        private void LoadProducts()
        {
            int[] userIds = null;
            int[] productIds = null;
            string name = null;            
            CartItemSortingType orderBy = CartItemSortingType.IdDesc;

            userIds = new int[] { QueryUserId };
            if (HasState("OrderBy")) orderBy = (CartItemSortingType)GetIntState("OrderBy");
            if (HasState(PRODUCT_ID_FILTER))
            {
                string value = GetStringState(PRODUCT_ID_FILTER);
                int temp;
                productIds = value.Split(',')
                    .Where(x => int.TryParse(x.ToString(), out temp))
                    .Select(x => int.Parse(x))
                    .ToArray();
            }            
            if (HasState(NAME_FILTER)) name = GetStringState(BRAND_NAME_FILTER);

            var result = CartService.GetPagedCartItemOverviewModels(
                pageIndex: gvCart.CustomPageIndex,
                pageSize: gvCart.PageSize,
                productIds: productIds,
                userIds: userIds,
                name: name,
                orderBy: orderBy);

            if (result != null)
            {
                gvCart.DataSource = result.Items;
                gvCart.RecordCount = result.TotalCount;
                gvCart.CustomPageCount = result.TotalPages;

                if (result.Items.Count > 0)                
                    phSendEmailAbdnCart.Visible = true;                
                else                
                    phSendEmailAbdnCart.Visible = false;                
            }

            gvCart.DataBind();            
        }

        protected void ectTogRightNav_ActionOccurred(string message, bool refresh)
        {
            enbNotice.Message = message;
            if (refresh) LoadProducts();
        }
        
        //protected void lbPreviewEmail_Click(object sender, EventArgs e)
        //{
        //    decimal promoValue;
        //    DateTime expiry;

        //    if (IsEmailValid(out promoValue, out expiry))
        //    {
        //        int productCount = CartDatatProxy.GetCartCount(Filter);
        //        UserInfo user = AccountAgent.GetUser(QueryUserId);
        //        SortClause sort = new SortClause { Operand = PRODUCT_ID_COLUMN, Operator = AppConstant.ASC };
        //        SortClause sortOpp = new SortClause { Operand = PRODUCT_ID_COLUMN, Operator = AppConstant.DESC };

        //        string email = EmailHelper.BuildAbandonedCartEmail(user.FirstName, promoValue, "XXXXXX", expiry, string.Empty, 
        //             CartDatatProxy.GetCarts(sort, sortOpp, Filter, productCount, productCount));

        //        Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "emailMessage", "<script language=JavaScript>loadHTML(\"" + email + "\");</script>");

        //        SetMessage(string.Empty);
        //    }
        //    else
        //    {
        //        SetMessage("Please check email information.");
        //    }
        //}

        //protected void lbSendEmail_Click(object sender, EventArgs e)
        //{
        //    decimal promoValue;
        //    DateTime expiry;

        //    if (IsEmailValid(out promoValue, out expiry))
        //    {
        //        int productCount = CartDatatProxy.GetCartCount(Filter);
        //        UserInfo user = AccountAgent.GetUser(QueryUserId);
        //        SortClause sort = new SortClause { Operand = PRODUCT_ID_COLUMN, Operator = AppConstant.ASC };
        //        SortClause sortOpp = new SortClause { Operand = PRODUCT_ID_COLUMN, Operator = AppConstant.DESC };

        //        string email = EmailHelper.BuildAbandonedCartEmail(user.FirstName, 
        //                                                           promoValue, 
        //                                                           OfferDataProxy.BuildAbandonedCartPromo(txtOfferRuleName.Text, txtOfferRuleAlias.Text, promoValue, expiry), 
        //                                                           expiry, 
        //                                                           string.Empty,
        //                                                           CartDatatProxy.GetCarts(sort, 
        //                                                                              sortOpp, 
        //                                                                              Filter, 
        //                                                                              productCount, 
        //                                                                              productCount)
        //                                                           );

        //        SetMessage(string.Empty);
        //        EmailMessage emailInfo = new EmailMessage();
        //        emailInfo.EmailAddress = user.Email;
        //        emailInfo.Message = email;
        //        emailInfo.Subject = "There are still items in your cart, get " + string.Format("{0:0}", promoValue) + "% off.";
        //        Apollo.Operational.Email.EmailManager emailMgr = new Apollo.Operational.Email.EmailManager(AppConfig.EmailFrom, AppConfig.EmailDisplayFrom);
        //        Apollo.Operational.Email.EmailManager._emailBcc = AppConfig.EmailBCC;
        //        Apollo.Operational.Email.EmailManager._emailHost = AppConfig.EmailHost;
        //        Apollo.Operational.Email.EmailManager._emailPassword = AppConfig.EmailPassword;
        //        Apollo.Operational.Email.EmailManager._emailUserName = AppConfig.EmailUserName;
        //        emailMgr.SendEmail(user.Email, emailInfo.Subject, email);
        //        //AppMail.SendEmail(user.Email, emailInfo.Subject, email);
        //        AccountAgent.InsertEmailMessage(user.UserId, emailInfo);

        //        SetMessage("Email was successfully sent.");
        //    }
        //    else
        //    {
        //        SetMessage("Email was FAILED to send. Please check email information.");
        //    }
        //}

        //private bool IsEmailValid(out decimal promoValue, out DateTime expiryDateTime)
        //{
        //    bool validA = txtOfferRuleName.Text.Length >= 3 ? true : false;
        //    bool validB = txtOfferRuleAlias.Text.Length >= 3 ? true : false;
        //    bool validC = txtPromoValue.Text.Length >= 0 ? true : false;
        //    bool validD = decimal.TryParse(txtPromoValue.Text, out promoValue);
        //    bool validE = true;
        //    if (radExpSpecifyDate.Checked)
        //    {
        //        validE = DateTime.TryParse(txtExpiry.Text, out expiryDateTime);
        //    }
        //    else
        //    {
        //        expiryDateTime = DateTime.Now.AddDays(1);
        //    }

        //    return validA && validB && validC && validD && validE;
        //}
    }
}