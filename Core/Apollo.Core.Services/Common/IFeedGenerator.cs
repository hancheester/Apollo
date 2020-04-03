using Apollo.Core.Model.Entity;
using System.Collections.Generic;

namespace Apollo.Core.Services.Common
{
    public interface IFeedGenerator
    {
        string BuildFeed(IList<Product> products, string countryCode = "");
    }
}
