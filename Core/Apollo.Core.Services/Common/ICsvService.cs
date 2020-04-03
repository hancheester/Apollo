using Apollo.Core.Model.Entity;
using System.Collections.Generic;

namespace Apollo.Core.Services.Common
{
    public interface ICsvService
    {
        byte[] ExportOrdersCsv(IList<Order> orders);        
    }
}
