
namespace Apollo.Core.Model.Entity
{
    public class SystemCheck : BaseEntity
    {
        public int OrderId { get; set; }
        public bool BillingPostCodeCheck { get; set; }
        public bool ShippingPostCodeCheck { get; set; }
        public bool EmailCheck { get; set; }
        public bool BillingAddressCheck { get; set; }
        public bool ShippingAddressCheck { get; set; }
        public bool NameCheck { get; set; }
        public bool BillingNameCheck { get; set; }
        public bool ShippingNameCheck { get; set; }
        public bool AvsCheck { get; set; }
    }
}
