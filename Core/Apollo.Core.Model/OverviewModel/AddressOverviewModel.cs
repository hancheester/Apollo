namespace Apollo.Core.Model.OverviewModel
{
    public class AddressOverviewModel
    {
        public string Name { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string City { get; set; }
        public string County { get; set; }
        public int CountryId { get; set; }
        public string CountryName { get; set; }
        public int USStateId { get; set; }
        public string USStateName { get; set; }
        public string PostCode { get; set; }
    }
}
