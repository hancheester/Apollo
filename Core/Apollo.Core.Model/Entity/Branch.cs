namespace Apollo.Core.Model.Entity
{
    public class Branch : BaseEntity
    {
        public string Name { get; set; }
        public string VectorBranchId { get; set; }
        public int DisplayOrder { get; set; }
        public bool InterBranchOrderEnabled { get; set; }
    }
}
