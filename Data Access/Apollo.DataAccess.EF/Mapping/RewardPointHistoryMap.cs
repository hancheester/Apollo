using Apollo.Core.Model.Entity;
using System.Data.Entity.ModelConfiguration;

namespace Apollo.DataAccess.EF.Mapping
{
    public class RewardPointHistoryMap : EntityTypeConfiguration<RewardPointHistory>
    {
        public RewardPointHistoryMap()
        {
            this.ToTable("RewardPointHistory");
            this.HasKey(x => x.Id);
        }
    }
}
