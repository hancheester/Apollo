
namespace Apollo.Core.Model.Entity
{
    public class spReport_NewVsReturnOrders_Result
    {
        public string Year { get; set; }
        public string Month { get; set; }
        public string Day { get; set; }
        public string Date { get; set; }
        public int NewOrders { get; set; }
        public int ReturningOrders { get; set; }
    }
}
