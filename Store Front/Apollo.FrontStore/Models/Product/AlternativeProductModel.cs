using System.Collections.Generic;

namespace Apollo.FrontStore.Models.Product
{
    public class AlternativeProductModel
    {
        public int ProductId { get; set; }
        public IList<ProductBoxModel> Products { get; set; }
    }
}