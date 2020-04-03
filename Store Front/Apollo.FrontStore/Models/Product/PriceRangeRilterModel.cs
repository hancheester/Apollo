namespace Apollo.FrontStore.Models.Product
{
    public class PriceRangeFilterModel
    {
        public string Min { get; set; }
        public string Max { get; set; }
        public string From { get; set; }
        public string To { get; set; }

        public PriceRangeFilterModel()
        {
            Min = string.Empty;
            Max = string.Empty;
            From = string.Empty;
            To = string.Empty;
        }
    }
}