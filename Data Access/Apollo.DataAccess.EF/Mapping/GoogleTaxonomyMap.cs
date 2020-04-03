using Apollo.Core.Model.Entity;
using System.Data.Entity.ModelConfiguration;

namespace Apollo.DataAccess.EF.Mapping
{
    public class GoogleTaxonomyMap : EntityTypeConfiguration<GoogleTaxonomy>
    {
        public GoogleTaxonomyMap()
        {
            this.ToTable("GoogleTaxonomy");
            this.HasKey(g => g.Id);
        }
    }
}
