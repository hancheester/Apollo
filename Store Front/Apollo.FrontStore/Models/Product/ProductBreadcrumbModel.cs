using System.Collections.Generic;

namespace Apollo.FrontStore.Models.Product
{
    public class ProductBreadcrumbModel
    {
        public string Name { get; set; }
        public string Url { get; set; }
        public IList<ProductBreadcrumbModel> BreadCrumb { get; set; }

        public ProductBreadcrumbModel()
        {
            BreadCrumb = new List<ProductBreadcrumbModel>();
        }
    }
}