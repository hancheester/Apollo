namespace Apollo.Core.Model.Entity
{
    public class CategoryLargeBannerMapping : BaseEntity
    {
        public int CategoryId { get; set; }
        public int LargeBannerId { get; set; }
        public int DisplayOrder { get; set; }
        public virtual LargeBanner LargeBanner { get; set; }
    }
}
