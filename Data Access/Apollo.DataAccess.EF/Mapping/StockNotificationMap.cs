using Apollo.Core.Model.Entity;
using System.Data.Entity.ModelConfiguration;

namespace Apollo.DataAccess.EF.Mapping
{
    public class StockNotificationMap : EntityTypeConfiguration<StockNotification>
    {
        public StockNotificationMap()
        {
            this.ToTable("StockNotification");
            this.HasKey(x => x.Id);
        }
    }
}
