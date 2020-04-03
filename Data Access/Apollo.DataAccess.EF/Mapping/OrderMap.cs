using Apollo.Core.Model.Entity;
using System.Data.Entity.ModelConfiguration;

namespace Apollo.DataAccess.EF.Mapping
{
    public class OrderMap : EntityTypeConfiguration<Order>
    {
        public OrderMap()
        {
            this.ToTable("Orders");
            this.HasKey(o => o.Id);
            this.Property(o => o.DiscountAmount).HasPrecision(10, 4);
            this.Property(o => o.ExchangeRate).HasPrecision(10, 4);
            this.Ignore(o => o.Archieved);
            this.Ignore(o => o.Country);
            this.Ignore(o => o.USState);
            this.Ignore(o => o.ShippingCountry);
            this.Ignore(o => o.ShippingUSState);
            this.Ignore(o => o.ShippingOption);
            this.Ignore(o => o.OrderStatus);
            this.Ignore(o => o.GrandTotal);
            this.Ignore(o => o.Tax);
            this.Ignore(o => o.LineItemCollection);
            this.Ignore(o => o.PharmOrder);
            this.Ignore(o => o.OrderNotes);
            this.Ignore(o => o.OrderComments);
            this.Ignore(o => o.SystemCheck);
        }
    }
}
