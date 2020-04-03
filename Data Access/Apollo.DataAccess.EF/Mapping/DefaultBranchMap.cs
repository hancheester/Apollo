using Apollo.Core.Model.Entity;
using System.Data.Entity.ModelConfiguration;

namespace Apollo.DataAccess.EF.Mapping
{
    public class DefaultBranchMap : EntityTypeConfiguration<DefaultBranch>
    {
        public DefaultBranchMap()
        {
            this.ToTable("DefaultBranches");
            this.HasKey(d => d.Id);
        }
    }
}
