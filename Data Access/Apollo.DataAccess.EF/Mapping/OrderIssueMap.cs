using Apollo.Core.Model.Entity;
using System.Data.Entity.ModelConfiguration;

namespace Apollo.DataAccess.EF.Mapping
{
    public class OrderIssueMap : EntityTypeConfiguration<OrderIssue>
    {
        public OrderIssueMap()
        {
            this.ToTable("OrderIssue");
            this.HasKey(o => o.Id);
        }
    }
}
