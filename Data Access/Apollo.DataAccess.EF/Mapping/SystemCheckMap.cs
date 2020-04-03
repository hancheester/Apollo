using Apollo.Core.Model.Entity;
using System.Data.Entity.ModelConfiguration;

namespace Apollo.DataAccess.EF.Mapping
{
    public class SystemCheckMap : EntityTypeConfiguration<SystemCheck>
    {
        public SystemCheckMap()
        {
            this.ToTable("SystemCheck");
            this.HasKey(s => s.Id);
        }
    }
}
