namespace Apollo.Core.Model.Entity
{
    public class ProductGroupMapping : BaseEntity
    {
        public int ProductId { get; set; }
        public int ProductGroupId { get; set; }
        public int Priority { get; set; }
    }
}
