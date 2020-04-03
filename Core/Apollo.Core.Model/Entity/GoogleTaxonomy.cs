
namespace Apollo.Core.Model.Entity
{
    public class GoogleTaxonomy : BaseEntity
    {
        public string Name { get; set; }
        public int ParentId { get; set; }
    }
}
