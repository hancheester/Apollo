using Apollo.AdminStore.WebForm.Classes;
using Apollo.Core.Model;
using Apollo.Core.Model.Entity;
using Apollo.Core.Services.Interfaces;
using System;
using System.Web.UI.WebControls;

namespace Apollo.AdminStore.WebForm.Marketing
{
    public partial class newsletter_subscribers : BasePage
    {
        public IAccountService AccountService { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)            
                LoadSubscribers();
        }

        protected void lbSave_Click(object sender, EventArgs e)
        {
            SaveLastViewedSubscriber();

            if (NotChosenProducts.Count > 0)
                AccountService.UpdateSubscribersStatus(NotChosenProducts, false);
            
            if (ChosenProducts.Count > 0)
                AccountService.UpdateSubscribersStatus(ChosenProducts, true);

            ChosenProducts.Clear();
            NotChosenProducts.Clear();

            enbNotice.Message = "Subscribers were updated successfully.";

            LoadSubscribers();
        }

        protected void lbSearch_Click(object sender, EventArgs e)
        {
            SetState(NAME_FILTER, ((TextBox)gvSubscribers.HeaderRow.FindControl("txtFilterEmail")).Text.Trim());
            SetState(STATUS_FILTER, ((DropDownList)gvSubscribers.HeaderRow.FindControl("ddlActive")).SelectedValue);
            LoadSubscribers();
        }

        protected void lbResetFilter_Click(object sender, EventArgs e)
        {
            DisposeState(NAME_FILTER);
            SetState(STATUS_FILTER, string.Empty);
            LoadSubscribers();
        }

        protected void btnGoPage_Click(object sender, EventArgs e)
        {
            int gotoIndex = Convert.ToInt32(((TextBox)gvSubscribers.TopPagerRow.FindControl("txtPageIndex")).Text.Trim()) - 1;

            if ((gvSubscribers.CustomPageCount > gotoIndex) && (gotoIndex >= 0))
                gvSubscribers.CustomPageIndex = gotoIndex;

            LoadSubscribers();
        }

        protected void ddlFilterChosen_PreRender(object sender, EventArgs e)
        {
            DropDownList ddl = (DropDownList)sender;
            ddl.SelectedIndex = -1;
            ddl.Items.FindByValue(GetStringState(CHOSEN_FILTER)).Selected = true;
        }

        protected void gvSubscribers_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvSubscribers.CustomPageIndex = gvSubscribers.CustomPageIndex + e.NewPageIndex;

            if (gvSubscribers.CustomPageIndex < 0)
                gvSubscribers.CustomPageIndex = 0;

            LoadSubscribers();
        }

        protected void gvSubscribers_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                Subscriber subscriber = e.Row.DataItem as Subscriber;
                CheckBox cb = e.Row.FindControl("cbChosen") as CheckBox;

                if (ChosenProducts.Contains(subscriber.Id))
                    cb.Checked = true;

                SetChosenSubscribers(subscriber.Id, cb.Checked);
            }
        }

        protected void gvSubscribers_Sorting(object sender, GridViewSortEventArgs e)
        {
            SubscriberSortingType orderBy = SubscriberSortingType.IdAsc;

            switch (e.SortExpression)
            {
                case "Email":
                    orderBy = SubscriberSortingType.EmailDesc;
                    if (e.SortDirection == SortDirection.Ascending)
                        orderBy = SubscriberSortingType.EmailAsc;
                    break;
                case "Id":
                default:
                    orderBy = SubscriberSortingType.IdDesc;
                    if (e.SortDirection == SortDirection.Ascending)
                        orderBy = SubscriberSortingType.IdAsc;
                    break;
            }
            SetState("OrderBy", (int)orderBy);
            LoadSubscribers();
        }

        protected void gvSubscribers_PreRender(object sender, EventArgs e)
        {
            if (gvSubscribers.TopPagerRow != null)
            {
                gvSubscribers.TopPagerRow.Visible = true;
                ((TextBox)gvSubscribers.HeaderRow.FindControl("txtFilterEmail")).Text = GetStringState(NAME_FILTER);
                ((DropDownList)gvSubscribers.HeaderRow.FindControl("ddlActive")).Items.FindByValue(GetStringState(STATUS_FILTER)).Selected = true;
            }
        }

        private void LoadSubscribers()
        {
            string email = null;
            bool? isActive = null;
            SubscriberSortingType orderBy = SubscriberSortingType.IdAsc;
            
            if (HasState(NAME_FILTER)) email = GetStringState(NAME_FILTER);
            if (HasState(STATUS_FILTER) && GetStringState(STATUS_FILTER) != ANY) isActive = Convert.ToBoolean(GetStringState(STATUS_FILTER));
            if (HasState("OrderBy")) orderBy = (SubscriberSortingType)GetIntState("OrderBy");

            var result = AccountService.GetSubscriberLoadPaged(
                pageIndex: gvSubscribers.CustomPageIndex,
                pageSize: gvSubscribers.PageSize,
                email: email,
                isActive: isActive,
                orderBy: orderBy);

            if (result != null)
            {
                gvSubscribers.DataSource = result.Items;
                gvSubscribers.RecordCount = result.TotalCount;
                gvSubscribers.CustomPageCount = result.TotalPages;
            }

            gvSubscribers.DataBind();

            if (gvSubscribers.Rows.Count <= 0) enbNotice.Message = "No records found.";
        }

        private void SaveLastViewedSubscriber()
        {
            int subscriberId;

            for (int i = 0; i < gvSubscribers.DataKeys.Count; i++)
            {
                CheckBox cb = gvSubscribers.Rows[i].FindControl("cbChosen") as CheckBox;
                subscriberId = Convert.ToInt32(gvSubscribers.DataKeys[i].Value);

                if (cb != null) SetChosenSubscribers(subscriberId, cb.Checked);
            }
        }

        private void SetChosenSubscribers(int subscriberId, bool chosen)
        {
            if (subscriberId != 0)
            {
                if ((chosen) && !ChosenProducts.Contains(subscriberId))
                {
                    ChosenProducts.Add(subscriberId);
                    NotChosenProducts.Remove(subscriberId);
                }
                else if ((!chosen) && (ChosenProducts.Contains(subscriberId)))
                {
                    ChosenProducts.Remove(subscriberId);
                    NotChosenProducts.Add(subscriberId);
                }
            }
        }
    }
}