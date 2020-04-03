using Apollo.Core.Model.Entity;
using System.Data.Entity.ModelConfiguration;

namespace Apollo.DataAccess.EF.Mapping
{
    public class USStateMap : EntityTypeConfiguration<USState>
    {
        public USStateMap()
        {
            this.ToTable("USStates");
            this.HasKey(u => u.Id);
        }
    }
}
