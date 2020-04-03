using Apollo.Core.Model.Entity;
using System;
using System.Collections.Generic;

namespace Apollo.Core.Services.Payment.SagePay
{
    public static class SagePayLogsExtension
    {
        public static List<SagePayLog> Append(this List<SagePayLog> list, int orderId, string nv, string status)
        {
            SagePayLog log = new SagePayLog();
            log.OrderId = orderId;
            log.NameValue = nv;
            log.Status = status;
            log.TimeStamp = DateTime.Now;

            // To prevent duplication
            SagePayLog found = list.Find(delegate (SagePayLog queryLog)
            {
                return queryLog.OrderId == log.OrderId
                    && queryLog.NameValue == log.NameValue
                    && queryLog.Status == log.Status
                    && queryLog.TimeStamp.CompareTo(log.TimeStamp) == 0;
            });

            if (found == null)
                list.Add(log);

            return list;
        }
    }
}
