using Apollo.Core.Model.Entity;
using System.Data.Entity.ModelConfiguration;

namespace Apollo.DataAccess.EF.Mapping
{
    public class ColourMap : EntityTypeConfiguration<Colour>
    {
        public ColourMap()
        {
            this.ToTable("Colours");
            this.HasKey(c => c.Id);
            this.Ignore(c => c.BrandName);
        }
    }
}
