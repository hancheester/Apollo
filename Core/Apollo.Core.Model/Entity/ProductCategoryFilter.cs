namespace Apollo.Core.Model.Entity
{    
    public class ProductCategoryFilter : BaseEntity
    {
        public int CategoryFilterId { get; set; }
        public int ProductId { get; set; }
    }
}
