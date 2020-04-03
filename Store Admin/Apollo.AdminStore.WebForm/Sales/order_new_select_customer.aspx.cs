using Apollo.AdminStore.WebForm.Classes;
using Apollo.Core.Model;
using Apollo.Core.Model.OverviewModel;
using Apollo.Core.Services.Interfaces;
using System;
using System.Linq;
using System.Web.UI.WebControls;

namespace Apollo.AdminStore.WebForm.Sales
{
    public partial class order_new_select_customer : BasePage
    {
        public IAccountService AccountService { get; set; }
        
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

            LoadUsers();
        }

        protected void lbSearch_Click(object sender, EventArgs e)
        {
            SetState(USER_ID_FILTER, ((TextBox)cgUsers.HeaderRow.FindControl("txtFilterUserId")).Text.Trim());
            SetState(NAME_FILTER, ((TextBox)cgUsers.HeaderRow.FindControl("txtFilterName")).Text.Trim());            
            SetState(EMAIL_FILTER, ((TextBox)cgUsers.HeaderRow.FindControl("txtFilterEmail")).Text.Trim());
            SetState(CONTACT_NUMBER_FILTER, ((TextBox)cgUsers.HeaderRow.FindControl("txtFilterContactNumber")).Text.Trim());

            LoadUsers();
        }

        protected void btnGoPage_Click(object sender, EventArgs e)
        {
            int gotoIndex = Convert.ToInt32(((TextBox)cgUsers.TopPagerRow.FindControl("txtPageIndex")).Text.Trim()) - 1;

            if ((cgUsers.CustomPageCount > gotoIndex) && (gotoIndex >= 0))
                cgUsers.CustomPageIndex = gotoIndex;

            LoadUsers();
        }

        protected void cgUsers_PreRender(object sender, EventArgs e)
        {
            if (cgUsers.TopPagerRow != null)
            {
                cgUsers.TopPagerRow.Visible = true;
                ((TextBox)cgUsers.HeaderRow.FindControl("txtFilterUserId")).Text = GetStringState(USER_ID_FILTER);
                ((TextBox)cgUsers.HeaderRow.FindControl("txtFilterName")).Text = GetStringState(NAME_FILTER);                
                ((TextBox)cgUsers.HeaderRow.FindControl("txtFilterEmail")).Text = GetStringState(EMAIL_FILTER);
                ((TextBox)cgUsers.HeaderRow.FindControl("txtFilterContactNumber")).Text = GetStringState(CONTACT_NUMBER_FILTER);                
            }            
        }

        protected void cgUsers_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            cgUsers.CustomPageIndex = cgUsers.CustomPageIndex + e.NewPageIndex;
            if (cgUsers.CustomPageIndex < 0) cgUsers.CustomPageIndex = 0;

            LoadUsers();
        }

        protected void cgUsers_Sorting(object sender, GridViewSortEventArgs e)
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

        protected void cgUsers_RowCreated(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                if (e.Row.DataItem != null)
                    e.Row.Attributes.Add("onclick", "window.location='/sales/order_new.aspx?userid=" + ((AccountOverviewModel)e.Row.DataItem).ProfileId.ToString() + "'");
                e.Row.Style.Add("cursor", "pointer");
            }
        }

        private void LoadUsers()
        {
            int[] accountIds = null;            
            string name = null;            
            string email = null;
            string contactNumber = null;            
            AccountSortingType orderBy = AccountSortingType.IdDesc;

            if (HasState("OrderBy")) orderBy = (AccountSortingType)GetIntState("OrderBy");
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
            
            var result = AccountService.GetPagedAccountOverviewModels(
                pageIndex: cgUsers.CustomPageIndex,
                pageSize: cgUsers.PageSize,
                accountIds: accountIds,
                name: name,
                email: email,
                contactNumber: contactNumber,
                orderBy: orderBy);

            if (result != null)
            {
                cgUsers.DataSource = result.Items;
                cgUsers.RecordCount = result.TotalCount;
                cgUsers.CustomPageCount = result.TotalPages;
            }

            cgUsers.DataBind();

            if (cgUsers.Rows.Count <= 0) enbNotice.Message = "No records found.";
        }
    }
}