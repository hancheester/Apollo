using Apollo.AdminStore.WebForm.Classes;
using Apollo.Core.Model;
using Apollo.Core.Model.Entity;
using Apollo.Core.Model.OverviewModel;
using Apollo.Core.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.UI.WebControls;

namespace Apollo.AdminStore.WebForm.FulFillment
{
    public partial class order_line_item_status : BasePage
    {
        public IOrderService OrderService { get; set; }
        public AdminStoreUtility AdminStoreUtility { get; set; }
        
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadProducts();
            }
        }

        protected void gvProducts_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            SaveLastViewedItems();

            gvProducts.CustomPageIndex = gvProducts.CustomPageIndex + e.NewPageIndex;

            if (gvProducts.CustomPageIndex < 0)
                gvProducts.CustomPageIndex = 0;

            LoadProducts();
        }

        protected void gvProducts_Sorting(object sender, GridViewSortEventArgs e)
        {
            var orderBy = LineItemSortingType.OrderIdAsc;

            switch (e.SortExpression)
            {
                default:
                case "OrderId":
                    orderBy = LineItemSortingType.OrderIdDesc;
                    if (e.SortDirection == SortDirection.Ascending)
                        orderBy = LineItemSortingType.OrderIdAsc;
                    break;
                case "ProductId":
                    orderBy = LineItemSortingType.ProductIdDesc;
                    if (e.SortDirection == SortDirection.Ascending)
                        orderBy = LineItemSortingType.ProductIdAsc;
                    break;
                case "Name":
                    orderBy = LineItemSortingType.NameDesc;
                    if (e.SortDirection == SortDirection.Ascending)
                        orderBy = LineItemSortingType.NameAsc;
                    break;
                case "Quantity":
                    orderBy = LineItemSortingType.QuantityDesc;
                    if (e.SortDirection == SortDirection.Ascending)
                        orderBy = LineItemSortingType.QuantityAsc;
                    break;
                case "StatusCode":
                    orderBy = LineItemSortingType.StatusDesc;
                    if (e.SortDirection == SortDirection.Ascending)
                        orderBy = LineItemSortingType.StatusAsc;
                    break;                
            }

            SetState("OrderBy", (int)orderBy);
            LoadProducts();
        }

        protected void gvProducts_PreRender(object sender, EventArgs e)
        {
            if (gvProducts.TopPagerRow != null)
            {
                gvProducts.TopPagerRow.Visible = true;
                ((TextBox)gvProducts.HeaderRow.FindControl("txtFilterOrderId")).Text = GetStringState(ORDER_ID_FILTER);
                ((TextBox)gvProducts.HeaderRow.FindControl("txtFilterProductId")).Text = GetStringState(PRODUCT_ID_FILTER);
                ((TextBox)gvProducts.HeaderRow.FindControl("txtFilterName")).Text = GetStringState(PRODUCT_NAME_FILTER);                
                ((TextBox)gvProducts.HeaderRow.FindControl("txtFilterOption")).Text = GetStringState(PRODUCT_SIZE_FILTER);                
                ((TextBox)gvProducts.HeaderRow.FindControl("txtFilterBarcode")).Text = GetStringState(BARCODE_FILTER);

                DropDownList ddl = ((DropDownList)gvProducts.HeaderRow.FindControl("ddlStatus"));

                ListItem item = ddl.Items.FindByValue(GetStringState(STATUS_CODE_FILTER));

                if (item != null)
                {
                    ddl.SelectedIndex = -1;
                    item.Selected = true;
                }
            }

            for (int i = 0; i < gvProducts.Rows.Count; i++)
            {
                CheckBox cb = gvProducts.Rows[i].FindControl("cbChosen") as CheckBox;
                CheckBox cbLastActivity = gvProducts.Rows[i].FindControl("cbLastActivity") as CheckBox;

                LineItemLite item = BuildItem(Convert.ToInt32(gvProducts.DataKeys[i].Value), gvProducts.Rows[i]);

                if (cb != null) SetChosenItems(item, cb.Checked, cbLastActivity.Checked);
            }
        }

        protected void gvProducts_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                DropDownList ddlItemStatus = AdminStoreUtility.FindControlRecursive(e.Row, "ddlItemStatus") as DropDownList;

                Literal ltlStatusCode = AdminStoreUtility.FindControlRecursive(e.Row, "ltlStatusCode") as Literal;

                // binding selected text to dropdown
                if (ltlStatusCode.Text != string.Empty)
                {
                    var foundItem = ddlItemStatus.Items.FindByValue(ltlStatusCode.Text);
                    if (foundItem != null) foundItem.Selected = true;
                }

                var item = (LineItemOverviewModel)e.Row.DataItem;
                LineItemLite itemLite = BuildItem(item, e.Row);
                LineItemLite chosenItem = null;

                if (ExistInChosenItems(itemLite))
                {
                    chosenItem = SessionFacade.ChosenLineItemLites.Find(delegate (LineItemLite arg)
                    {
                        return itemLite.LineItemId == arg.LineItemId;
                    });
                }

                CheckBox cb = e.Row.FindControl("cbChosen") as CheckBox;

                if (chosenItem != null)
                {
                    TextBox txtComment = e.Row.FindControl("txtComment") as TextBox;
                    txtComment.Text = chosenItem.Comment;
                    ltlStatusCode.Text = chosenItem.StatusCode;

                    ddlItemStatus.SelectedIndex = -1;
                    ddlItemStatus.Items.FindByValue(ltlStatusCode.Text).Selected = true;
                    cb.Checked = true;
                }
            }
        }

        protected void lbResetProductFilter_Click(object sender, EventArgs e)
        {
            DisposeState(PRODUCT_ID_FILTER);            
            DisposeState(PRODUCT_NAME_FILTER);
            DisposeState(PRODUCT_SIZE_FILTER);
            DisposeState(PRODUCT_QUANITITY_FILTER);            
            DisposeState(ORDER_ID_FILTER);
            DisposeState(BARCODE_FILTER);

            SetState(STATUS_CODE_FILTER, string.Empty);

            LoadProducts();
        }

        protected void lbSearchProduct_Click(object sender, EventArgs e)
        {
            SetState(ORDER_ID_FILTER, ((TextBox)gvProducts.HeaderRow.FindControl("txtFilterOrderId")).Text.Trim());
            SetState(PRODUCT_ID_FILTER, ((TextBox)gvProducts.HeaderRow.FindControl("txtFilterProductId")).Text.Trim());
            SetState(PRODUCT_NAME_FILTER, ((TextBox)gvProducts.HeaderRow.FindControl("txtFilterName")).Text.Trim());
            SetState(PRODUCT_SIZE_FILTER, ((TextBox)gvProducts.HeaderRow.FindControl("txtFilterOption")).Text.Trim());            
            SetState(BARCODE_FILTER, ((TextBox)gvProducts.HeaderRow.FindControl("txtFilterBarcode")).Text.Trim());
            SetState(STATUS_CODE_FILTER, ((DropDownList)gvProducts.HeaderRow.FindControl("ddlStatus")).SelectedValue);

            gvProducts.CustomPageIndex = 0;

            LoadProducts();
        }

        protected void lbUpdateSelected_Click(object sender, EventArgs e)
        {
            SaveLastViewedItems();

            List<int> orderList = new List<int>();

            var profileId = Convert.ToInt32(HttpContext.Current.Profile.GetPropertyValue("ProfileId"));
            var fullName = Convert.ToString(HttpContext.Current.Profile.GetPropertyValue("FullName"));

            foreach (var item in SessionFacade.ChosenLineItemLites)
            {
                // Update pending quantity
                //OrderAgent.UpdateLineItemPendingQuantity(item.LineItemId, item.PendingQuantity);

                // Update allocated quantity
                //OrderAgent.UpdateLineItemAllocatedQuantity(item.LineItemId, item.AllocatedQuantity);

                // Update shipped quantity
                //OrderAgent.UpdateShippedQuantity(item.LineItemId, item.ShippedQuantity);

                // Update line item status
                OrderService.UpdateLineItemStatusCodeByLineItemId(item.LineItemId, item.StatusCode);

                // Delete branch item allocation
                if (CanGoForBranchAllocation(item.StatusCode))
                {
                    OrderService.ResetLineItemForBranchAllocation(item.LineItemId, item.PendingQuantity);
                }

                // Update last activity date for the order of the line item
                if (item.UpdateOrderLastActivityDateFlag)
                {
                    OrderService.UpdateOrderLastActivityDateByOrderId(item.OrderId, DateTime.Now);
                }

                // Add comment
                string name = item.Name + (item.Option != string.Empty ? " " + item.Option : string.Empty);
                string message = "Status for " + name + " was updated to <i>" + OrderService.GetLineStatusByCode(item.StatusCode) + "</i>.";
                //message = "<br/>Pending quantity for" + name + " was updated to " + item.PendingQuantity;
                //message = "<br/>Allocated quantity for" + name + " was updated to " + item.AllocatedQuantity;
                //message = "<br/>Shipped quantity for" + name + " was updated to " + item.ShippedQuantity;

                string comment = item.Comment;
                item.Comment = string.Empty;

                OrderService.ProcessOrderCommentInsertion(item.OrderId,
                                                        profileId,
                                                        fullName,
                                                        "Line Item",
                                                        message,
                                                        comment);

                if (!orderList.Contains(item.OrderId))
                    orderList.Add(item.OrderId);
            }

            // Process order status with current line item status
            OrderService.ResetOrderStatusAccordingToLineStatusByOrderIds(orderList, profileId, fullName);
            
            enbNotice.Message = "Lines were updated successfully";
            ChosenToAddItems.Clear();
            NotChosenToAddItems.Clear();
            LoadProducts();
        }

        protected void btnProductGoPage_Click(object sender, EventArgs e)
        {
            int gotoIndex = Convert.ToInt32(((TextBox)gvProducts.TopPagerRow.FindControl("txtPageIndex")).Text.Trim()) - 1;

            if ((gvProducts.CustomPageCount > gotoIndex) && (gotoIndex >= 0))
                gvProducts.CustomPageIndex = gotoIndex;

            LoadProducts();
        }

        protected void ddlStatus_Init(object sender, EventArgs e)
        {
            DropDownList ddl = (DropDownList)sender;
            LoadStatus(ddl);
        }

        protected void ddlItemStatus_Init(object sender, EventArgs e)
        {
            DropDownList ddl = (DropDownList)sender;
            LoadStatus(ddl);
        }
        
        private void LoadStatus(DropDownList ddl)
        {
            var list = OrderService.GetLineStatusList().ToList();
            list.Insert(0, new LineStatus { Status = AppConstant.DEFAULT_SELECT });
            
            ddl.DataTextField = "Status";
            ddl.DataValueField = "StatusCode";
            ddl.DataSource = list;
            ddl.DataBind();
        }

        private void LoadProducts()
        {
            int[] orderIds = null;
            int[] productIds = null;
            string name = null;
            string barcode = null;            
            string option = null;
            string statusCode = null;
            LineItemSortingType orderBy = LineItemSortingType.OrderIdAsc;
            
            if (HasState(ORDER_ID_FILTER))
            {
                string value = GetStringState(ORDER_ID_FILTER);
                int temp;
                orderIds = value.Split(',')
                    .Where(x => int.TryParse(x.ToString(), out temp))
                    .Select(x => int.Parse(x))
                    .ToArray();
            }

            if (HasState(PRODUCT_ID_FILTER))
            {
                string value = GetStringState(PRODUCT_ID_FILTER);
                int temp;
                productIds = value.Split(',')
                    .Where(x => int.TryParse(x.ToString(), out temp))
                    .Select(x => int.Parse(x))
                    .ToArray();
            }

            if (HasState(PRODUCT_NAME_FILTER)) name = GetStringState(PRODUCT_NAME_FILTER);
            if (HasState(BARCODE_FILTER)) barcode = GetStringState(BARCODE_FILTER);
            if (HasState(PRODUCT_SIZE_FILTER)) option = GetStringState(PRODUCT_SIZE_FILTER);
            if (HasState(STATUS_CODE_FILTER)) statusCode = GetStringState(STATUS_CODE_FILTER);            
            if (HasState("OrderBy")) orderBy = (LineItemSortingType)GetIntState("OrderBy");

            var result = OrderService.GetPagedLineItemOverviewModel(
                pageIndex: gvProducts.CustomPageIndex,
                pageSize: gvProducts.PageSize,
                orderIds: orderIds,
                productIds: productIds,
                name: name,
                barcode: barcode,
                option: option,
                statusCode: statusCode,
                orderBy: orderBy);

            if (result != null)
            {
                gvProducts.DataSource = result.Items;
                gvProducts.RecordCount = result.TotalCount;
                gvProducts.CustomPageCount = result.TotalPages;
            }

            gvProducts.DataBind();

            if (gvProducts.Rows.Count <= 0) enbNotice.Message = "No records found.";
        }

        private void SaveLastViewedItems()
        {
            for (int i = 0; i < gvProducts.DataKeys.Count; i++)
            {
                CheckBox cb = gvProducts.Rows[i].FindControl("cbChosen") as CheckBox;
                CheckBox cbLastActivity = gvProducts.Rows[i].FindControl("cbLastActivity") as CheckBox;
                int lineItemId = Convert.ToInt32(gvProducts.DataKeys[i].Value);

                var item = BuildItem(lineItemId, gvProducts.Rows[i]);

                if (cb != null) SetChosenItems(item, cb.Checked, cbLastActivity.Checked);
            }
        }

        private LineItemLite BuildItem(int lineItemId, GridViewRow row)
        {
            var lineItem = OrderService.GetLineItemOverviewModel(lineItemId);

            return BuildItem(lineItem, row);
        }

        private LineItemLite BuildItem(LineItemOverviewModel lineItem, GridViewRow row)
        {
            var newItem = new LineItemLite();
            newItem.LineItemId = lineItem.Id;
            newItem.OrderId = lineItem.OrderId;
            newItem.Name = lineItem.Name;
            newItem.Option = lineItem.Option;
            newItem.ShippedQuantity = lineItem.ShippedQuantity;
            newItem.PendingQuantity = lineItem.PendingQuantity;
            newItem.AllocatedQuantity = lineItem.AllocatedQuantity;
            newItem.StatusCode = lineItem.StatusCode;

            //Fetch comment
            TextBox txtComment = row.FindControl("txtComment") as TextBox;
            if (txtComment.Text.Trim() != string.Empty)
                newItem.Comment = txtComment.Text.Trim();

            // Fetch status          
            DropDownList ddlItemStatus = (DropDownList)row.FindControl("ddlItemStatus");
            if (ddlItemStatus.SelectedValue != string.Empty)
                newItem.StatusCode = ddlItemStatus.SelectedValue;
            
            return newItem;
        }

        private void SetChosenItems(LineItemLite item, bool chosen, bool updateLastActivityDate)
        {
            if (item != null)
            {
                if (chosen)
                {
                    item.UpdateOrderLastActivityDateFlag = updateLastActivityDate;

                    if (ExistInChosenItems(item))
                    {
                        SessionFacade.ChosenLineItemLites.RemoveAll(delegate (LineItemLite arg)
                        {
                            return arg.LineItemId == item.LineItemId;
                        });
                    }

                    SessionFacade.ChosenLineItemLites.Add(item);
                    SessionFacade.NotChosenLineItemLites.RemoveAll(delegate (LineItemLite arg)
                    {
                        return arg.LineItemId == item.LineItemId;
                    });
                }
                else
                {
                    SessionFacade.ChosenLineItemLites.RemoveAll(delegate (LineItemLite arg)
                    {
                        return arg.LineItemId == item.LineItemId;
                    });

                    if (ExistInNotChosenItems(item))
                    {
                        SessionFacade.NotChosenLineItemLites.RemoveAll(delegate (LineItemLite arg)
                        {
                            return arg.LineItemId == item.LineItemId;
                        });
                    }

                    SessionFacade.NotChosenLineItemLites.Add(item);
                }
            }
        }

        private bool ExistInChosenItems(LineItemLite item)
        {
            bool exist = false;

            for (int j = 0; j < SessionFacade.ChosenLineItemLites.Count; j++)
            {
                exist = IsEqual(SessionFacade.ChosenLineItemLites[j], item);

                if (!exist) exist = SessionFacade.ChosenLineItemLites[j].LineItemId == item.LineItemId;

                if (exist) break;
            }

            return exist;
        }

        private bool ExistInNotChosenItems(LineItemLite item)
        {
            bool exist = false;

            for (int j = 0; j < SessionFacade.NotChosenLineItemLites.Count; j++)
            {
                exist = IsEqual(SessionFacade.NotChosenLineItemLites[j], item);

                if (!exist) exist = SessionFacade.NotChosenLineItemLites[j].LineItemId == item.LineItemId;

                if (exist) break;
            }

            return exist;
        }

        private bool IsEqual(LineItemLite item1, LineItemLite item2)
        {
            PropertyInfo[] properties1 = item1.GetType().GetProperties();
            PropertyInfo[] properties2 = item2.GetType().GetProperties();
            object value1;
            object value2;

            Type type = item1.GetType();
            bool isEqual = false;

            for (int i = 0; i < properties1.Length; i++)
            {
                value1 = properties1[i].GetValue(item1, null);
                value2 = properties2[i].GetValue(item2, null);

                if (value1 != null && value2 != null)
                    isEqual = value1.Equals(value2);

                if (!isEqual)
                    break;
            }

            return isEqual;
        }
        
        private bool CanGoForBranchAllocation(string statusCode)
        {
            if (statusCode == LineStatusCode.ORDERED
                || statusCode == LineStatusCode.PICK_IN_PROGRESS
                || statusCode == LineStatusCode.CANCELLED
                || statusCode == LineStatusCode.DESPATCHED
                || statusCode == LineStatusCode.PARTIAL_SHIPPING
                || statusCode == LineStatusCode.PENDING
                || statusCode == LineStatusCode.SCHEDULED_FOR_CANCEL
                || statusCode == LineStatusCode.GOODS_ALLOCATED)
                return true;

            return false;
        }
    }
}