using Apollo.Core.Model.Entity;
using System.Data.Entity.ModelConfiguration;

namespace Apollo.DataAccess.EF.Mapping
{
    public class CustomDictionaryMap : EntityTypeConfiguration<CustomDictionary>
    {
        public CustomDictionaryMap()
        {
            this.ToTable("CustomDictionary");
            this.HasKey(c => c.Id);
        }
    }
}
