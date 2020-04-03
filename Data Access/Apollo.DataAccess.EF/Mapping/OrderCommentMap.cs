using Apollo.Core.Model.Entity;
using System.Data.Entity.ModelConfiguration;

namespace Apollo.DataAccess.EF.Mapping
{
    public class OrderCommentMap : EntityTypeConfiguration<OrderComment>
    {
        public OrderCommentMap()
        {
            this.ToTable("OrderComment");
            this.HasKey(o => o.Id);
        }
    }
}
