using Apollo.Core.Model.Entity;
using System.Data.Entity.ModelConfiguration;

namespace Apollo.DataAccess.EF.Mapping
{
    public class SysCheckPostCodeMap : EntityTypeConfiguration<SysCheckPostCode>
    {
        public SysCheckPostCodeMap()
        {
            this.ToTable("SysCheckPostCode");
            this.HasKey(s => s.Id);
        }
    }
}
