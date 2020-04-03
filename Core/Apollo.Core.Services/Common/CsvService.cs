using Apollo.Core.Model.Entity;
using Apollo.Core.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;

namespace Apollo.Core.Services.Common
{
    public class CsvService : ICsvService
    {
        private readonly IAccountService _accountService;
        
        public CsvService(IAccountService accountService)
        {
            _accountService = accountService;
        }

        public byte[] ExportOrdersCsv(IList<Order> orders)
        {
            if (orders == null) throw new ArgumentNullException("orders");
            if (orders.Count == 0) return null;

            byte[] bytes;

            using (var stream = new MemoryStream())
            {
                var sw = new StreamWriter(stream);

                sw.WriteLine("OrderID,Order on,Last Activity,Email,Payment,Payment Reference,Grand Total,Shipping,Country,Status,Last Updated By");

                foreach (var order in orders)
                {
                    string lastUpdatedBy = string.Empty;
                    string lastActivityDate = string.Empty;
                    string email = _accountService.GetEmailByProfileId(order.ProfileId);

                    if (order.OrderComments.Count > 0)
                        lastUpdatedBy = order.OrderComments[0].FullName.ToString();

                    if (order.LastActivityDate.HasValue)
                        lastActivityDate = Convert.ToDateTime(order.LastActivityDate).ToString("dd/MM/yyyy HH:mm:ss");

                    sw.WriteLine(order.Id + "," +
                                Convert.ToDateTime(order.OrderPlaced).ToString("dd/MM/yyyy HH:mm:ss") + "," +
                                lastActivityDate + "," +
                                email + "," +
                                order.PaymentMethod + "," +
                                order.PaymentRef + "," +
                                (order.GrandTotal > 0 ? order.CurrencyCode + " " + order.GrandTotal + ","  : null) +
                                order.ShippingOption.Name + "," +
                                order.ShippingCountry.Name + "," +
                                order.StatusCode + "," +
                                lastUpdatedBy);
                }

                sw.Close();

                bytes = stream.ToArray();
            }
            return bytes;
        }
    }
}
