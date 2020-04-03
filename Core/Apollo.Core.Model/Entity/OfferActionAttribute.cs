namespace Apollo.Core.Model.Entity
{
    public class OfferActionAttribute : BaseEntity
    {
        public string Name { get; set; }
        public bool IsCart { get; set; }
        public bool IsCatalog { get; set; }
    }
}
