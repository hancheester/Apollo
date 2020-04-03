namespace Apollo.Core.Model.OverviewModel
{
    public class ShippingOptionOverviewModel : BaseOverviewModel
    {
        public string Description { get; set; }
        public string Name { get; set; }
        public bool Enabled { get; set; }
        public int Priority { get; set; }
        public int CountryId { get; set; }
        public decimal Cost { get; set; }
        public decimal FreeThreshold { get; set; }
        public string Timeline { get; set; }
    }
}
