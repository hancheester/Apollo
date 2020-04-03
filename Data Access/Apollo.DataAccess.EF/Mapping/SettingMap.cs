using Apollo.Core.Model.Entity;
using System.Data.Entity.ModelConfiguration;

namespace Apollo.DataAccess.EF.Mapping
{
    public class SettingMap : EntityTypeConfiguration<Setting>
    {
        public SettingMap()
        {
            this.ToTable("Setting");
            this.HasKey(x => x.Id);
            this.Property(x => x.Name).IsRequired().HasMaxLength(200);
            this.Property(x => x.Value).IsRequired().HasMaxLength(2000);
        }
    }
}
