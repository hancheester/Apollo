using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using Apollo.Core.Model.Entity;

namespace Apollo.AdminStore.WebForm.Classes
{
    public static class EmailHelper
    {
        // TODO: I think we should have this as a task.
        public static string BuildAbandonedCartEmail(string customerName, decimal promoValue, string promoCode, DateTime expires, string additionalHTML, IList<CartItem> cartItems)
        {
            string message = string.Empty;

            var abandonedCartEmailTemplateLocalPath = "";
            using (StreamReader sr = new StreamReader(abandonedCartEmailTemplateLocalPath))
            {
                message = sr.ReadToEnd();
                message = message.Replace("@@PromoValue", string.Format("{0:0}", promoValue) + "%");

                string expiresFormatted;
                if (expires.Subtract(DateTime.Now).TotalDays < 1.0 && !(expires.Hour == 0 && expires.Minute == 0 && expires.Second == 0 && expires.Millisecond == 0))
                {
                    expiresFormatted = "within the next 24 hours";
                }
                else
                {
                    expiresFormatted = "by the end of " + expires.AddDays(-1).DayOfWeek + " " + expires.AddDays(-1).ToLongDateString();
                }

                message = message.Replace("@@Expiry", expiresFormatted);
                message = message.Replace("@@PromoCode", promoCode);
                message = message.Replace("@@CustomerName", customerName);
                message = message.Replace("@@AdditionalHTML", additionalHTML);

                StringBuilder cartItemsHTML = new StringBuilder();

                int numberOfColumnsToRender = 3;
                int numberOfRows = (int)Math.Ceiling(cartItems.Count / ((double)numberOfColumnsToRender));
                int index = 0;
                for (int i = 1; i <= numberOfRows; i++)
                {
                    cartItemsHTML.Append("<tr>");
                    for (int j = 1; j <= numberOfColumnsToRender; j++)
                    {
                        if (j == 2)
                        {
                            cartItemsHTML.Append("<td align=\"center\" width=\"32%\" bgcolor=\"#FFFFFF\" valign=\"top\">");
                        }
                        else
                        {
                            cartItemsHTML.Append("<td align=\"center\" width=\"31%\" bgcolor=\"#FFFFFF\" valign=\"top\">");
                        }

                        if (index < cartItems.Count)
                        {
                            var image = cartItems[index].Product.ProductMedias.Count > 0 ? cartItems[index].Product.ProductMedias[0].ThumbnailFilename : string.Empty;
                            cartItemsHTML.Append("<table cellspacing=\"0\" cellpadding=\"0\"><tr><td height=\"110px\" valign=\"middle\" align=\"center\">");
                            cartItemsHTML.Append("<a href=\"http://www.Apollo.co.uk/" + cartItems[index].Product.UrlRewrite + "?metrics=aban\">");
                            cartItemsHTML.Append("<img src=\"http://www.Apollo.co.uk/" + image.Replace("~/", "") + "\" alt=\"" + cartItems[index].Product.Name + "\" />");
                            cartItemsHTML.Append("</a>");
                            cartItemsHTML.Append("</td></tr><tr><td align=\"center\">");
                            cartItemsHTML.Append("<br />");
                            cartItemsHTML.Append("<font face=\"Arial, Helvetica, sans-serif\" size=\"-1\"><strong>");
                            cartItemsHTML.Append("<a href=\"http://www.Apollo.co.uk/" + cartItems[index].Product.UrlRewrite + "?metrics=aban\"><font color=\"#000000\">");
                            cartItemsHTML.Append(cartItems[index].Product.Name);
                            cartItemsHTML.Append("</font></a>");
                            cartItemsHTML.Append("</strong><br />");
                            cartItemsHTML.Append("<font color=\"#497225\">");
                            cartItemsHTML.Append("&pound;" + (cartItems[index].ProductPrice.OfferPriceExclTax));
                            cartItemsHTML.Append("</font>");
                            cartItemsHTML.Append("</font>");
                            cartItemsHTML.Append("</td></tr></table>");
                        }
                        else
                        {
                            cartItemsHTML.Append("&nbsp;");
                        }

                        cartItemsHTML.Append("</td>");
                        if (j != 3)
                        {
                            cartItemsHTML.Append("<td align=\"center\" width=\"2%\" bgcolor=\"#FFFFFF\"><img src=\"http://www.Apollo.co.uk/Images/emailAssets/vertSep.gif\" /></td>");
                        }
                        index++;
                    }
                    cartItemsHTML.Append("</tr>");
                }
                message = message.Replace("@@CartItems", cartItemsHTML.ToString());
                message = message.Replace(Environment.NewLine, "").Replace("\"", "'");
            }

            return message;
        }
    }
}