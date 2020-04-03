using Apollo.AdminStore.WebForm.Classes;
using Apollo.Core.Model.Entity;
using Apollo.Core.Services.Interfaces;
using System;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;

namespace Apollo.AdminStore.WebForm
{
    public partial class get_item_handler : BasePage
    {
        public IProductService ProductService { get; set; }
        public IOrderService OrderService { get; set; }

        protected void Page_Load(object sender, EventArgs e)
       {
            HttpContext.Current.Response.Clear();
            HttpContext.Current.Response.ContentType = "application/json";
            HttpContext.Current.Response.Write(GetItem(Request["barcode"], Request["branchid"]));
            HttpContext.Current.Response.End();
        }

        private string GetItem(string barcode, string strBranchId)
        {
            int branchId = Convert.ToInt32(strBranchId);
            string result = "[]";
            int maxQty = 0;

            var price = ProductService.GetProductPriceOverviewModelByBarcode(barcode.Trim());

            if (price != null)
            {
                var foundItems = OrderService.GetBranchAllocationListByBranchIdAndProductPriceId(branchId, price.Id);

                if (foundItems.Count > 0)
                {
                    string name = string.Empty;

                    foundItems.ToList().ForEach(delegate (BranchAllocation item)
                    {
                        var lineItem = OrderService.GetLineItemById(item.LineItemId);
                        maxQty = maxQty + (lineItem.PendingQuantity - lineItem.AllocatedQuantity);

                        if (string.IsNullOrEmpty(name)) name = lineItem.Name;
                    });
                    
                    result = string.Format("[{{\"id\":\"{0}\", \"name\":\"{1}\", \"option\":\"{2}\", \"maxquantity\":\"{3}\"}}]", price.Id, name, price.Option, maxQty);
                }
            }
           
            return result;
        }
    }
}