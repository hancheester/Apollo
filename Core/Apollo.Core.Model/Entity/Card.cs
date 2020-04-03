
namespace Apollo.Core.Model.Entity
{
    public class Card
    {
        public string CardType { get; set; }
        public string CardNumber { get; set; }
        public string HolderName { get; set; }
        public string SecurityCode { get; set; }
        public string ExpiryMonth { get; set; }
        public string ExpiryYear { get; set; }
        public string StartMonth { get; set; }
        public string StartYear { get; set; }
        public string IssueNumber { get; set; }
    }
}
