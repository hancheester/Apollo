using Apollo.Core.Model.Entity;
using System.Data.Entity.ModelConfiguration;

namespace Apollo.DataAccess.EF.Mapping
{
    public class EventLogMap : EntityTypeConfiguration<EventLog>
    {
        public EventLogMap()
        {
            this.ToTable("EventLog");
            this.HasKey(e => e.Id);
        }
    }
}
