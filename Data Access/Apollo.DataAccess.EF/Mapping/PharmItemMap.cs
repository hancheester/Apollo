using Apollo.Core.Model.Entity;
using System.Data.Entity.ModelConfiguration;

namespace Apollo.DataAccess.EF.Mapping
{
    public class PharmItemMap : EntityTypeConfiguration<PharmItem>
    {
        public PharmItemMap()
        {
            this.ToTable("PharmItem");
            this.HasKey(p => p.Id);
            this.Ignore(p => p.Name);
            this.Ignore(p => p.Option);
            this.Ignore(p => p.Quantity);
        }
    }
}
