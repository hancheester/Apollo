using Apollo.Core.Model.Entity;
using System.Data.Entity.ModelConfiguration;

namespace Apollo.DataAccess.EF.Mapping
{
    public class SagePayLogMap : EntityTypeConfiguration<SagePayLog>
    {
        public SagePayLogMap()
        {
            this.ToTable("SagePayLog");
            this.HasKey(s => s.Id);
        }
    }
}
