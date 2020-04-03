namespace Apollo.AdminStore.WebForm.Classes
{
    public class SupportedCard
    {
        private string _cardType;
        private bool _hasStartDate;
        private bool _hasIssueNumber;

        public string CardType
        {
            get { return _cardType; }
        }
        public bool HasStartDate
        {
            get { return _hasStartDate; }
        }
        public bool HasIssueNumber
        {
            get { return _hasIssueNumber; }
        }

        public SupportedCard(string cardType, bool hasStartDate, bool hasIssueNumber)
        {
            _cardType = cardType;
            _hasStartDate = hasStartDate;
            _hasIssueNumber = hasIssueNumber;
        }

        public override string ToString()
        {
            return _cardType;
        }
    }
}