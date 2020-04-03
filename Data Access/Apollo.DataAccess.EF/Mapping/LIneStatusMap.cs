using Apollo.Core.Model.Entity;
using System.Data.Entity.ModelConfiguration;

namespace Apollo.DataAccess.EF.Mapping
{
    public class LineStatusMap : EntityTypeConfiguration<LineStatus>
    {
        public LineStatusMap()
        {
            this.ToTable("LineStatus");
            this.HasKey(l => l.Id);
        }
    }
}
