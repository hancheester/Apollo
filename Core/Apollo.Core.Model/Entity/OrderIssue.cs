
namespace Apollo.Core.Model.Entity
{
    public class OrderIssue : BaseEntity
    {
        public string IssueCode { get; set; }
        public string Issue { get; set; }

        public OrderIssue()
        {
            IssueCode = string.Empty;
            Issue = string.Empty;
        }
    }
}
