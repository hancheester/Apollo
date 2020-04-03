namespace Apollo.Core.Model.OverviewModel
{
    public class CategoryFeaturedItemOverviewModel : BaseOverviewModel
    {
        public int CategoryId { get; set; }
        public int FeaturedItemType { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string ProductUrlKey { get; set; }
    }
}
