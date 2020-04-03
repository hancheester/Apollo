using Apollo.Core.Model.Entity;
using System.Data.Entity.ModelConfiguration;

namespace Apollo.DataAccess.EF.Mapping
{
    public class SysCheckEmailMap : EntityTypeConfiguration<SysCheckEmail>
    {
        public SysCheckEmailMap()
        {
            this.ToTable("SysCheckEmail");
            this.HasKey(s => s.Id);
        }
    }
}
