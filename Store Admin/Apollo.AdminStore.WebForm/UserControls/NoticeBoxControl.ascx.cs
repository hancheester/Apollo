using Apollo.AdminStore.WebForm.Classes;
using System;

namespace Apollo.AdminStore.WebForm.UserControls
{
    public partial class NoticeBoxControl : BaseUserControl
    {
        public string Message
        {
            //get { return ltlMsg.Text; }
            set
            {
                if (!string.IsNullOrEmpty(value))
                    Session["message_type"] = value;

                //if (value != string.Empty)
                //{
                //    phBox.Visible = true;
                //    ltlMsg.Text = value;
                //}
                //else
                //    phBox.Visible = false;
            }
        }

        public string AppendMessage
        {
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    if (Session["message_type"] != null && !string.IsNullOrEmpty(Session["message_type"].ToString()))                    
                        Session["message_type"] += "<br/>" + value;                    
                    else
                        Session["message_type"] = value;
                }

                //if (value != string.Empty)
                //{
                //    phBox.Visible = true;
                //    if (ltlMsg.Text == string.Empty)
                //        ltlMsg.Text = value;
                //    else if (!ltlMsg.Text.Contains(value))
                //        ltlMsg.Text += "<br/>" + value;
                //}
                //else
                //    phBox.Visible = false;
            }
        }

        public int QueryMessageType
        {
            get
            {
                if ((Request.QueryString[QueryKey.MSG_TYPE] != null)
                        && (RegexType.Integer.Match(Request.QueryString[QueryKey.MSG_TYPE]).Success))
                    return Convert.ToInt32(Request.QueryString[QueryKey.MSG_TYPE]);
                else
                    return 0;
            }
        }
      
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                switch ((MessageType)QueryMessageType)
                {
                    case MessageType.AccountNotLoaded:
                        Message = "Account could not be loaded.";
                        break;
                    case MessageType.AccountIdInvalid:
                        Message = "Account ID is invalid.";
                        break;
                    case MessageType.AccountNotFound:
                        Message = "Account could not be found.";
                        break;
                    case MessageType.AccountCreated:
                        Message = "Account was created successfully.";
                        break;
                    case MessageType.OrderNotFound:
                        Message = "Order could not be found.";
                        break;
                    case MessageType.OrderNotLoaded:
                        Message = "Order could not be loaded.";
                        break;
                    case MessageType.OrderCancellationNotFound:
                        Message = "Order cancellation could not be found.";
                        break;
                    case MessageType.OrderCancellationDeleted:
                        Message = "Order cancellation was deleted successfully.";
                        break;
                    case MessageType.OrderRefundNotFound:
                        Message = "Order refund could not be found.";
                        break;
                    case MessageType.OrderRefundDeleted:
                        Message = "Order refund was deleted successfully.";
                        break;
                    case MessageType.OrderDeliveryNotFound:
                        Message = "Order delivery could not be found.";
                        break;
                    case MessageType.OrderShipmentNotFound:
                        Message = "Order shipment could not be found.";
                        break;
                    case MessageType.OrderSaved:
                        Message = "Order was saved successfully.";
                        break;
                    case MessageType.ProductCreated:
                        Message = "Product was created successfully.";
                        break;
                    case MessageType.ProductNotFound:
                        Message = "Product could not be found.";
                        break;
                    case MessageType.ProductDeleted:
                        Message = "Product was deleted successfully.";
                        break;
                    case MessageType.ProductReviewNotFound:
                        Message = "Product review could not be found.";
                        break;
                    case MessageType.ProductReviewDeleted:
                        Message = "Product review was deleted successfully.";
                        break;
                    case MessageType.OfferCreated:
                        Message = "Offer was created successfully.";
                        break;
                    case MessageType.OfferNotFound:
                        Message = "Offer could not be found.";
                        break;
                    case MessageType.CategoryCreated:
                        Message = "Category was created successfully.";
                        break;
                    case MessageType.CategoryNotFound:
                        Message = "Category could not be found.";
                        break;
                    case MessageType.CategoryDeleted:
                        Message = "Category was deleted successfully.";
                        break;
                    case MessageType.CountryCreated:
                        Message = "Country was created successfully.";
                        break;
                    case MessageType.SearchTermDeleted:
                        Message = "Search term was deleted successfully.";
                        break;
                    case MessageType.SearchTermCreated:
                        Message = "Search term was created successfully.";
                        break;
                    case MessageType.CustomDictionaryDeleted:
                        Message = "Custom dictionary word was deleted successfully.";
                        break;
                    case MessageType.CustomDictionaryCreated:
                        Message = "Custom dictionary word was created successfully.";
                        break;
                    case MessageType.ShippingOptionCreated:
                        Message = "Shipping option was created successfully.";
                        break;
                    case MessageType.ShippingOptionDeleted:
                        Message = "Shipping option was deleted successfully.";
                        break;
                    case MessageType.FeaturedItemCreated:
                        Message = "Featured item was created successfully.";
                        break;
                    case MessageType.FeaturedItemDeleted:
                        Message = "Featured item was deleted successfully.";
                        break;
                    case MessageType.FeaturedItemNotFound:
                        Message = "Featured item could not be found.";
                        break;
                    case MessageType.BannerDeleted:
                        Message = "Banner was deleted successfully.";
                        break;
                    case MessageType.BannerCreated:
                        Message = "Banner was created successfully.";
                        break;
                    case MessageType.TestimonialDeleted:
                        Message = "Testimonial was deleted successfully.";
                        break;
                    case MessageType.TestimonialCreated:
                        Message = "Testimonial was created successfully.";
                        break;
                    case MessageType.CurrencyCreated:
                        Message = "Currency was created successfully.";
                        break;
                    case MessageType.PharmOrderUpdated:
                        Message = "Pharmaceutical form was update successfully.";
                        break;
                    case MessageType.BlogPostCreated:
                        Message = "Blog post was created successfully.";
                        break;
                    case MessageType.BlogPostUpdated:
                        Message = "Blog post was update successfully.";
                        break;
                    case MessageType.BlogPostDeleted:
                        Message = "Blog post was deleted successfully.";
                        break;
                    case MessageType.BlogPostNotFound:
                        Message = "Blog post could not be found.";
                        break;
                    case MessageType.None:
                    default:
                        break;
                }
            }
        }

        protected override void OnPreRender(EventArgs e)
        {
            if (Session["message_type"] != null && !string.IsNullOrEmpty(Session["message_type"].ToString()))
            {
                phBox.Visible = true;
                ltlMsg.Text = Session["message_type"].ToString();
                Session["message_type"] = null;
            }
            else
                phBox.Visible = false;
            base.OnPreRender(e);
        }

        public void ClearMessage()
        {
            Session["message_type"] = null;
        }
    }
}