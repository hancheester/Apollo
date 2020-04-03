namespace Apollo.Core.Model.Entity
{
    public class Setting : BaseEntity
    {
        public string Name { get; set; }
        public string Value { get; set; }
        public int StoreId { get; set; }
        public override string ToString()
        {
            return Name;
        }
    }
}
