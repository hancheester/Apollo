using Apollo.AdminStore.WebForm.Classes;
using Apollo.Core.Model;
using Apollo.Core.Services.Interfaces;
using System;
using System.Web.UI.WebControls;

namespace Apollo.AdminStore.WebForm.Sales
{
    public partial class order_comments_default : BasePage
    {
        public IOrderService OrderService { get; set; }
        
        protected override void OnInit(EventArgs e)
        {
            if (QueryOrderId <= 0)
                Response.Redirect("/sales/order_default.aspx");
            else
            {
                ltlTitle.Text = string.Format("<h3>Order # {0}</h3>", QueryOrderId.ToString());
                LoadComments();
            }

            base.OnInit(e);
        }
        
        protected void lbResetFilter_Click(object sender, EventArgs e)
        {
            DisposeState(FROM_DATE_FILTER);
            DisposeState(TO_DATE_FILTER);
            DisposeState(COMMENT_FILTER);
            DisposeState(NAME_FILTER);

            gvComments.CustomPageIndex = 0;

            LoadComments();
        }

        protected void lbSearch_Click(object sender, EventArgs e)
        {
            SetState(FROM_DATE_FILTER, ((TextBox)gvComments.HeaderRow.FindControl("txtFromDate")).Text.Trim());
            SetState(TO_DATE_FILTER, ((TextBox)gvComments.HeaderRow.FindControl("txtToDate")).Text.Trim());
            SetState(COMMENT_FILTER, ((TextBox)gvComments.HeaderRow.FindControl("txtFilterComment")).Text.Trim());
            SetState(NAME_FILTER, ((TextBox)gvComments.HeaderRow.FindControl("txtFilterFullName")).Text.Trim());

            gvComments.CustomPageIndex = 0;

            LoadComments();
        }

        protected void btnGoPage_Click(object sender, EventArgs e)
        {
            int gotoIndex = Convert.ToInt32(((TextBox)gvComments.TopPagerRow.FindControl("txtPageIndex")).Text.Trim()) - 1;

            if ((gvComments.CustomPageCount > gotoIndex) && (gotoIndex >= 0))
                gvComments.CustomPageIndex = gotoIndex;

            LoadComments();
        }

        protected void gvComments_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvComments.CustomPageIndex = gvComments.CustomPageIndex + e.NewPageIndex;

            if (gvComments.CustomPageIndex < 0)
                gvComments.CustomPageIndex = 0;

            LoadComments();
        }

        protected void gvComments_Sorting(object sender, GridViewSortEventArgs e)
        {
            var orderBy = OrderCommentSortingType.IdAsc;

            switch (e.SortExpression)
            {
                case "DateStamp":
                    orderBy = OrderCommentSortingType.DateStampDesc;
                    if (e.SortDirection == SortDirection.Ascending)
                        orderBy = OrderCommentSortingType.DateStampAsc;
                    break;
                case "CommentText":
                    orderBy = OrderCommentSortingType.CommentTextDesc;
                    if (e.SortDirection == SortDirection.Ascending)
                        orderBy = OrderCommentSortingType.CommentTextAsc;
                    break;
                case "FullName":
                    orderBy = OrderCommentSortingType.FullNameDesc;
                    if (e.SortDirection == SortDirection.Ascending)
                        orderBy = OrderCommentSortingType.FullNameAsc;
                    break;               
                default:
                    break;
            }

            SetState("OrderBy", (int)orderBy);
            LoadComments();
        }

        protected void gvComments_PreRender(object sender, EventArgs e)
        {
            if (gvComments.TopPagerRow != null)
            {
                gvComments.TopPagerRow.Visible = true;                
                ((TextBox)gvComments.HeaderRow.FindControl("txtFilterComment")).Text = GetStringState(COMMENT_FILTER);
                ((TextBox)gvComments.HeaderRow.FindControl("txtFilterFullName")).Text = GetStringState(NAME_FILTER);
                ((TextBox)gvComments.HeaderRow.FindControl("txtFromDate")).Text = GetStringState(FROM_DATE_FILTER);
                ((TextBox)gvComments.HeaderRow.FindControl("txtToDate")).Text = GetStringState(TO_DATE_FILTER);
            }
        }

        private void LoadComments()
        {
            int[] orderIds = new int[] { QueryOrderId };
            string name = null;
            string fromDateStamp = null;
            string toDateStamp = null;
            string commentText = null;
            OrderCommentSortingType orderBy = OrderCommentSortingType.IdDesc;
            
            if (HasState(NAME_FILTER)) name = GetStringState(NAME_FILTER);
            if (HasState(FROM_DATE_FILTER)) fromDateStamp = GetStringState(FROM_DATE_FILTER);
            if (HasState(TO_DATE_FILTER)) toDateStamp = GetStringState(TO_DATE_FILTER);
            if (HasState(COMMENT_FILTER)) commentText = GetStringState(COMMENT_FILTER);
            if (HasState("OrderBy")) orderBy = (OrderCommentSortingType)GetIntState("OrderBy");

            var result = OrderService.GetOrderCommentLoadPaged(
                pageIndex: gvComments.CustomPageIndex,
                pageSize: gvComments.PageSize,
                orderIds: orderIds,
                fromDateStamp: fromDateStamp,
                toDateStamp: toDateStamp,
                commentText: commentText,
                orderBy: orderBy);

            if (result != null)
            {
                gvComments.DataSource = result.Items;
                gvComments.RecordCount = result.TotalCount;
                gvComments.CustomPageCount = result.TotalPages;
            }

            gvComments.DataBind();

            if (gvComments.Rows.Count <= 0) enbNotice.Message = "No records found.";
        }
    }
}