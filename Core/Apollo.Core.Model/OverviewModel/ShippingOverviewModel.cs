namespace Apollo.Core.Model.OverviewModel
{
    public class ShippingOverviewModel
    {
        public int ShippingOptionId { get; set; }
        public int ShippingCountryId { get; set; }
        /// <summary>
        /// Amount in GBP.
        /// </summary>
        public decimal ShippingCost { get; set; }
        public string Packing { get; set; }
    }
}
