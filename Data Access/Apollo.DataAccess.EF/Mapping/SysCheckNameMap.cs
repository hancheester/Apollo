using Apollo.Core.Model.Entity;
using System.Data.Entity.ModelConfiguration;

namespace Apollo.DataAccess.EF.Mapping
{
    public class SysCheckNameMap : EntityTypeConfiguration<SysCheckName>
    {
        public SysCheckNameMap()
        {
            this.ToTable("SysCheckName");
            this.HasKey(s => s.Id);
        }
    }
}
