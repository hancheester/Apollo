namespace Apollo.Core.Model.Entity
{
    public class BrandCategory : BaseEntity
    {
        public int ParentId { get; set; }
        public string Name { get; set; }
        public int BrandId { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }        
        public string UrlRewrite { get; set; }
        public bool Visible { get; set; }
    }
}
