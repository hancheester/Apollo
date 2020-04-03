using Apollo.AdminStore.WebForm.Classes;
using Apollo.Core.Services.Interfaces;
using System;

namespace Apollo.AdminStore.WebForm
{
    public partial class search : BasePage
    {
        public IAccountService AccountService { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            var filter = Request["Filter"];
            var query = Request["Query"];

            switch (filter)
            {
                case "orderid":
                    int orderId = 0;
                    try
                    {
                        orderId = Convert.ToInt32(query.Trim());
                    }
                    catch { Response.Redirect("/sales/order_default.aspx?" + QueryKey.MSG_TYPE + "=" + (int)MessageType.OrderNotFound); }
                    Response.Redirect("/sales/order_info.aspx?orderid=" + orderId);

                    break;

                case "orderaddress":

                    Response.Redirect("/sales/order_default.aspx?orderaddress=" + query.Trim());
                    break;

                case "productid":
                    int productId = 0;
                    try
                    {
                        productId = Convert.ToInt32(query.Trim());
                    }
                    catch { productId = 0; }
                    Response.Redirect("/catalog/product_info.aspx?productid=" + productId);
                    break;

                case "productname":
                    Response.Redirect("/catalog/product_default.aspx?productname=" + query.Trim());
                    break;

                case "email":
                default:
                    string email = query.Trim();
                    var account = AccountService.GetAccountByUsername(email);

                    if (account != null)
                        Response.Redirect("/customer/customer_info.aspx?userid=" + account.ProfileId.ToString());
                    else
                        Response.Redirect("/customer/customer_default.aspx?" + QueryKey.MSG_TYPE + "=" + (int)MessageType.AccountNotFound);

                    break;
            }
        }
    }
}