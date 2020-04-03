using Apollo.Core.Model.Entity;
using System.Data.Entity.ModelConfiguration;

namespace Apollo.DataAccess.EF.Mapping
{
    public class SubscriberMap : EntityTypeConfiguration<Subscriber>
    {
        public SubscriberMap()
        {
            this.ToTable("Subscribers");
            this.HasKey(s => s.Id);
        }
    }
}
