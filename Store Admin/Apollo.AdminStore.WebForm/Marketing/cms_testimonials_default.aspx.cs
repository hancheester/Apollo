using Apollo.AdminStore.WebForm.Classes;
using Apollo.Core.Model;
using Apollo.Core.Services.Interfaces;
using System;
using System.Web.UI.WebControls;

namespace Apollo.AdminStore.WebForm.Marketing
{
    public partial class cms_testimonials_default : BasePage
    {
        public IUtilityService UtilityService { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
                LoadTestimonials();
        }
        
        protected void lbResetFilter_Click(object sender, EventArgs e)
        {
            DisposeState(COMMENT_FILTER);
            DisposeState(NAME_FILTER);         
            LoadTestimonials();
        }

        protected void lbSearch_Click(object sender, EventArgs e)
        {
            SetState(COMMENT_FILTER, ((TextBox)gvTestimonials.HeaderRow.FindControl("txtFilterComment")).Text.Trim());            
            SetState(NAME_FILTER, ((TextBox)gvTestimonials.HeaderRow.FindControl("txtFilterName")).Text.Trim());
            LoadTestimonials();
        }

        protected void btnGoPage_Click(object sender, EventArgs e)
        {
            int gotoIndex = Convert.ToInt32(((TextBox)gvTestimonials.TopPagerRow.FindControl("txtPageIndex")).Text.Trim()) - 1;

            if ((gvTestimonials.CustomPageCount > gotoIndex) && (gotoIndex >= 0))
                gvTestimonials.CustomPageIndex = gotoIndex;

            LoadTestimonials();
        }

        protected void gvTestimonials_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvTestimonials.CustomPageIndex = gvTestimonials.CustomPageIndex + e.NewPageIndex;

            if (gvTestimonials.CustomPageIndex < 0)
                gvTestimonials.CustomPageIndex = 0;

            LoadTestimonials();
        }

        protected void gvTestimonials_Sorting(object sender, GridViewSortEventArgs e)
        {
            TestimonialSortingType orderBy = TestimonialSortingType.IdAsc;

            switch (e.SortExpression)
            {
                case "Name":
                    orderBy = TestimonialSortingType.NameDesc;
                    if (e.SortDirection == SortDirection.Ascending)
                        orderBy = TestimonialSortingType.NameAsc;
                    break;
                case "Comment":
                    orderBy = TestimonialSortingType.CommentDesc;
                    if (e.SortDirection == SortDirection.Ascending)
                        orderBy = TestimonialSortingType.CommentAsc;
                    break;
                case "Id":
                default:
                    orderBy = TestimonialSortingType.IdDesc;
                    if (e.SortDirection == SortDirection.Ascending)
                        orderBy = TestimonialSortingType.IdAsc;
                    break;
            }

            SetState("OrderBy", (int)orderBy);
            LoadTestimonials();
        }

        protected void gvTestimonials_PreRender(object sender, EventArgs e)
        {
            if (gvTestimonials.TopPagerRow != null)
            {
                gvTestimonials.TopPagerRow.Visible = true;                
                ((TextBox)gvTestimonials.HeaderRow.FindControl("txtFilterComment")).Text = GetStringState(COMMENT_FILTER);
                ((TextBox)gvTestimonials.HeaderRow.FindControl("txtFilterName")).Text = GetStringState(NAME_FILTER);                
            }
        }

        private void LoadTestimonials()
        {
            string comment = null;
            string name = null;
            TestimonialSortingType orderBy = TestimonialSortingType.IdAsc;
            
            if (HasState(COMMENT_FILTER)) comment = GetStringState(COMMENT_FILTER);
            if (HasState(NAME_FILTER)) name = GetStringState(NAME_FILTER);
            if (HasState("OrderBy")) orderBy = (TestimonialSortingType)GetIntState("OrderBy");

            var result = UtilityService.GetTestimonialLoadPaged(
                pageIndex: gvTestimonials.CustomPageIndex,
                pageSize: gvTestimonials.PageSize,
                comment: comment,
                name: name,
                orderBy: orderBy);

            if (result != null)
            {
                gvTestimonials.DataSource = result.Items;
                gvTestimonials.RecordCount = result.TotalCount;
                gvTestimonials.CustomPageCount = result.TotalPages;
            }

            gvTestimonials.DataBind();

            if (gvTestimonials.Rows.Count <= 0) enbNotice.Message = "No records found.";
        }
    }
}