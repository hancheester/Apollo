using Apollo.AdminStore.WebForm.Classes;
using System.Web.UI;

namespace Apollo.AdminStore.WebForm.UserControls
{
    public partial class UserControls_CustomerNavControl : UserControl
    {
        public enum Menu
        {
            CustomerView,
            AccountInfo,
            Address,
            Order,
            Point,
            ShoppingCart,
            EmailHistory,
            ActivityLog,
            None,
            NewCustomer
        }

        private string ProfileId
        {
            get
            {
                return ((BasePage)this.Page).QueryUserId.ToString();
            }
        }

        public Menu DisabledItem
        {
            set
            {
                ltlCustomerView.Text = "<li><a href='/customer/customer_info.aspx?userid=" + ProfileId + "'>Customer view</a></li>";
                ltlAccInfo.Text = "<li><a href='/customer/customer_account_info.aspx?userid=" + ProfileId + "'>Account information</a></li>";
                ltlAddress.Text = "<li><a href='/customer/customer_address_info.aspx?userid=" + ProfileId + "'>Addresses</a></li>";
                ltlOrder.Text = "<li><a href='/customer/customer_order_info.aspx?userid=" + ProfileId + "'>Orders</a></li>";
                ltlRewardPoint.Text = "<li><a href='/customer/customer_point_info.aspx?userid=" + ProfileId + "'>Loyalty points</a></li>";
                ltlShoppingCart.Text = "<li><a href='/customer/customer_cart_info.aspx?userid=" + ProfileId + "'>Shopping cart</a></li>";
                
                switch (value)
                {
                    case Menu.CustomerView:
                        ltlCustomerView.Text = "<li class='active'><a href='#'>Customer view</a></li>";
                        break;
                    case Menu.AccountInfo:
                        ltlAccInfo.Text = "<li class='active'><a href='#'>Account information</a></li>";
                        break;
                    case Menu.Address:
                        ltlAddress.Text = "<li class='active'><a href='#'>Addresses</a></li>";
                        break;
                    case Menu.Order:
                        ltlOrder.Text = "<li class='active'><a href='#'>Orders</a></li>";
                        break;
                    case Menu.Point:
                        ltlRewardPoint.Text = "<li class='active'><a href='#'>Loyalty points</a></li>";
                        break;
                    case Menu.ShoppingCart:
                        ltlShoppingCart.Text = "<li class='active'><a href='#'>Shopping cart</a></li>";
                        break;
                    case Menu.NewCustomer:
                        ltlCustomerView.Text = "<li class='active'><a href='#'>Customer view</a></li>";
                        ltlAccInfo.Text = "<li><a href='#'>Account information<small class=\"text-navy clearfix\">(please save to continue)</small></a></li>";
                        ltlAddress.Text = "<li><a href='#'>Addresses<small class=\"text-navy clearfix\">(please save to continue)</small></a></li>";
                        ltlOrder.Text = "<li><a href='#'>Orders<small class=\"text-navy clearfix\">(please save to continue)</small></a></li>";
                        ltlRewardPoint.Text = "<li><a href='#'>Loyalty points<small class=\"text-navy clearfix\">(please save to continue)</small></a></li>";
                        ltlShoppingCart.Text = "<li><a href='#'>Shopping cart<small class=\"text-navy clearfix\">(please save to continue)</small></a></li>";
                        break;
                    case Menu.None:
                        break;
                }
            }
        }
    }
}