using Apollo.Core.Model.Entity;
using System.Data.Entity.ModelConfiguration;

namespace Apollo.DataAccess.EF.Mapping
{
    public class RestrictedGroupMappingMap : EntityTypeConfiguration<RestrictedGroupMapping>
    {
        public RestrictedGroupMappingMap()
        {
            this.ToTable("Product_RestrictedGroup_Mapping");
            this.HasKey(p => p.Id);
        }
    }
}
