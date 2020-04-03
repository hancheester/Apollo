using Apollo.Core.Model.OverviewModel;
using System.Collections.Generic;

namespace Apollo.Web.Framework.Services.Catalog
{
    public partial interface IRecentlyViewedProductsService
    {
        IList<ProductOverviewModel> GetRecentlyViewedProducts(int number);
        void AddProductToRecentlyViewedList(int productId);
        void RemoveProductFromRecentlyViewedList(int productId);
    }
}
