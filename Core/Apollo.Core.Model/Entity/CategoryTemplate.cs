namespace Apollo.Core.Model.Entity
{
    public class CategoryTemplate : BaseEntity
    {
        public string Name { get; set; }
        public string ViewPath { get; set; }
        public int DisplayOrder { get; set; }
    }
}
