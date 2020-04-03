using Apollo.Core.Model.Entity;
using System.Data.Entity.ModelConfiguration;

namespace Apollo.DataAccess.EF.Mapping
{
    public class RestrictedGroupMap : EntityTypeConfiguration<RestrictedGroup>
    {
        public RestrictedGroupMap()
        {
            this.ToTable("RestrictedGroups");
            this.HasKey(r => r.Id);
            this.Ignore(r => r.DisplayName);
        }
    }
}
