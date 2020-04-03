using Apollo.Core.Model.Entity;
using System.Data.Entity.ModelConfiguration;

namespace Apollo.DataAccess.EF.Mapping
{
    public class PharmOrderMap : EntityTypeConfiguration<PharmOrder>
    {
        public PharmOrderMap()
        {
            this.ToTable("PharmOrder");
            this.HasKey(p => p.Id);
            this.Ignore(p => p.Items);
        }
    }
}
