using Apollo.Core.Model.OverviewModel;
using Apollo.Core.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Apollo.Web.Framework.Services.Catalog
{
    public class RecentlyViewedProductsService : IRecentlyViewedProductsService
    {
        private readonly HttpContextBase _httpContext;
        private readonly IProductService _productService;

        public RecentlyViewedProductsService(HttpContextBase httpContext, IProductService productService)
        {
            this._httpContext = httpContext;
            this._productService = productService;
        }
        
        public void AddProductToRecentlyViewedList(int productId)
        {
            var oldProductIds = GetRecentlyViewedProductsIds();
            var newProductIds = new List<int>();
            newProductIds.Add(productId);

            foreach (int oldProductId in oldProductIds)
            {
                if (oldProductId != productId)
                    newProductIds.Add(oldProductId);
            }

            var recentlyViewedCookie = _httpContext.Request.Cookies.Get(SystemCookieNames.RecentlyViewedProducts);
            if (recentlyViewedCookie == null)
            {
                recentlyViewedCookie = new HttpCookie(SystemCookieNames.RecentlyViewedProducts);
                recentlyViewedCookie.HttpOnly = true;
            }

            recentlyViewedCookie.Values.Clear();

            for(int i = 0; i < newProductIds.Count && i < 10; i++)
            {
                recentlyViewedCookie.Values.Add("rvp", newProductIds[i].ToString());
            }
            
            recentlyViewedCookie.Expires = DateTime.Now.AddDays(1);
            _httpContext.Response.Cookies.Set(recentlyViewedCookie);
        }
        
        public IList<ProductOverviewModel> GetRecentlyViewedProducts(int number)
        {
            var products = new List<ProductOverviewModel>();
            var productIds = GetRecentlyViewedProductsIds(number);
            foreach (var productId in productIds)
            {
                var product = _productService.GetProductOverviewModelById(productId);
                if (product != null && product.Enabled && product.VisibleIndividually)
                {
                    products.Add(product);
                }
            }

            return products;
        }

        public void RemoveProductFromRecentlyViewedList(int productId)
        {
            var oldProductIds = GetRecentlyViewedProductsIds();
            var newProductIds = oldProductIds.Where(x => x != productId).ToList();
            
            var recentlyViewedCookie = _httpContext.Request.Cookies.Get(SystemCookieNames.RecentlyViewedProducts);
            if (recentlyViewedCookie == null)
            {
                recentlyViewedCookie = new HttpCookie(SystemCookieNames.RecentlyViewedProducts);
                recentlyViewedCookie.HttpOnly = true;
            }

            recentlyViewedCookie.Values.Clear();

            for (int i = 0; i < newProductIds.Count && i < 10; i++)
            {
                recentlyViewedCookie.Values.Add("rvp", newProductIds[i].ToString());
            }

            recentlyViewedCookie.Expires = DateTime.Now.AddDays(1);
            _httpContext.Response.Cookies.Set(recentlyViewedCookie);
        }

        private IList<int> GetRecentlyViewedProductsIds()
        {
            return GetRecentlyViewedProductsIds(int.MaxValue);
        }

        private IList<int> GetRecentlyViewedProductsIds(int number)
        {
            var productIds = new List<int>();

            var recentlyViewedCookie = _httpContext.Request.Cookies.Get(SystemCookieNames.RecentlyViewedProducts);
            if (recentlyViewedCookie == null) return productIds;

            string[] values = recentlyViewedCookie.Values.GetValues("rvp");
            if (values == null) return productIds;

            return values.Select(x => int.Parse(x))
                .Distinct()
                .Take(number)
                .ToList();
        }
    }
}
