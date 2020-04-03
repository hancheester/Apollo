
namespace Apollo.Core.Model.Entity
{
    public class SearchTerm : BaseEntity
    {
        public string Query { get; set; }
        public int NumberOfResults { get; set; }
        public int NumberOfUses { get; set; }
        public string RedirectUrl { get; set; }
    }
}
