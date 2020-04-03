using Apollo.AdminStore.WebForm.Classes;
using Apollo.Core.Model;
using Apollo.Core.Services.Interfaces;
using System;
using System.Linq;
using System.Web.UI.WebControls;

namespace Apollo.AdminStore.WebForm.Customer
{
    public partial class customer_default : BasePage
    {
        public IAccountService AccountService { get; set; }
        public ICartService CartService { get; set; }
        
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
                LoadUsers();        
        }

        protected void lbResetFilter_Click(object sender, EventArgs e)
        {
            DisposeState(USER_ID_FILTER);
            DisposeState(NAME_FILTER);            
            DisposeState(EMAIL_FILTER);
            DisposeState(CONTACT_NUMBER_FILTER);
            DisposeState(DOB_FILTER);

            LoadUsers();
        }

        protected void lbSearch_Click(object sender, EventArgs e)
        {
            SetState(USER_ID_FILTER, ((TextBox)gvUsers.HeaderRow.FindControl("txtFilterUserId")).Text.Trim());
            SetState(NAME_FILTER, ((TextBox)gvUsers.HeaderRow.FindControl("txtFilterName")).Text.Trim());            
            SetState(EMAIL_FILTER, ((TextBox)gvUsers.HeaderRow.FindControl("txtFilterEmail")).Text.Trim());
            SetState(CONTACT_NUMBER_FILTER, ((TextBox)gvUsers.HeaderRow.FindControl("txtFilterContactNumber")).Text.Trim());
            SetState(DOB_FILTER, ((TextBox)gvUsers.HeaderRow.FindControl("txtDob")).Text.Trim());
            
            LoadUsers();
        }

        protected void btnGoPage_Click(object sender, EventArgs e)
        {
            int gotoIndex = Convert.ToInt32(((TextBox)gvUsers.TopPagerRow.FindControl("txtPageIndex")).Text.Trim()) - 1;

            if ((gvUsers.CustomPageCount > gotoIndex) && (gotoIndex >= 0))
                gvUsers.CustomPageIndex = gotoIndex;

            LoadUsers();
        }

        protected void gvUsers_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvUsers.CustomPageIndex = gvUsers.CustomPageIndex + e.NewPageIndex;

            if (gvUsers.CustomPageIndex < 0)
                gvUsers.CustomPageIndex = 0;

            LoadUsers();
        }

        protected void gvUsers_Sorting(object sender, GridViewSortEventArgs e)
        {
            var orderBy = AccountSortingType.IdAsc;

            switch (e.SortExpression)
            {
                case "Id":
                    orderBy = AccountSortingType.IdDesc;
                    if (e.SortDirection == SortDirection.Ascending)
                        orderBy = AccountSortingType.IdAsc;
                    break;
                case "Name":
                    orderBy = AccountSortingType.NameDesc;
                    if (e.SortDirection == SortDirection.Ascending)
                        orderBy = AccountSortingType.NameAsc;
                    break;
                case "Email":
                    orderBy = AccountSortingType.EmailDesc;
                    if (e.SortDirection == SortDirection.Ascending)
                        orderBy = AccountSortingType.EmailAsc;
                    break;
                case "ContactNumber":
                    orderBy = AccountSortingType.ContactNumberDesc;
                    if (e.SortDirection == SortDirection.Ascending)
                        orderBy = AccountSortingType.ContactNumberAsc;
                    break;
                default:
                    break;
            }

            SetState("OrderBy", (int)orderBy);

            LoadUsers();
        }

        protected void gvUsers_PreRender(object sender, EventArgs e)
        {
            if (gvUsers.TopPagerRow != null)
            {
                gvUsers.TopPagerRow.Visible = true;
                ((TextBox)gvUsers.HeaderRow.FindControl("txtFilterUserId")).Text = GetStringState(USER_ID_FILTER);
                ((TextBox)gvUsers.HeaderRow.FindControl("txtFilterName")).Text = GetStringState(NAME_FILTER);                
                ((TextBox)gvUsers.HeaderRow.FindControl("txtFilterEmail")).Text = GetStringState(EMAIL_FILTER);
                ((TextBox)gvUsers.HeaderRow.FindControl("txtFilterContactNumber")).Text = GetStringState(CONTACT_NUMBER_FILTER);
                ((TextBox)gvUsers.HeaderRow.FindControl("txtDob")).Text = GetStringState(DOB_FILTER);                
            }
        }
        
        private void LoadUsers()
        {
            int[] accountIds = null;
            string name = null;
            string email = null;
            string contactNumber = null;
            string dob = null;
            AccountSortingType orderBy = AccountSortingType.IdDesc;
            
            if (HasState(USER_ID_FILTER))
            {
                string value = GetStringState(USER_ID_FILTER);
                int temp;
                accountIds = value.Split(',')
                    .Where(x => int.TryParse(x.ToString(), out temp))
                    .Select(x => int.Parse(x))
                    .ToArray();
            }

            if (HasState(EMAIL_FILTER)) email = GetStringState(EMAIL_FILTER);
            if (HasState(NAME_FILTER)) name = GetStringState(NAME_FILTER);
            if (HasState(CONTACT_NUMBER_FILTER)) contactNumber = GetStringState(CONTACT_NUMBER_FILTER);
            if (HasState(DOB_FILTER)) dob = GetStringState(DOB_FILTER);
            if (HasState("OrderBy")) orderBy = (AccountSortingType)GetIntState("OrderBy");

            var result = AccountService.GetPagedAccountOverviewModels(
                pageIndex: gvUsers.CustomPageIndex,
                pageSize: gvUsers.PageSize,
                accountIds: accountIds,
                name: name,
                email: email,
                contactNumber: contactNumber,
                dob: dob,
                orderBy: orderBy);

            if (result != null)
            {
                gvUsers.DataSource = result.Items;
                gvUsers.RecordCount = result.TotalCount;
                gvUsers.CustomPageCount = result.TotalPages;
            }

            gvUsers.DataBind();

            if (gvUsers.Rows.Count <= 0) enbNotice.Message = "No records found.";
        }
    }
}