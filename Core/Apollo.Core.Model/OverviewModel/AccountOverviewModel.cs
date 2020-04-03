using System;

namespace Apollo.Core.Model.OverviewModel
{
    public class AccountOverviewModel : BaseOverviewModel
    {
        public string Name { get; set; }        
        public string Email { get; set; }
        public string ContactNumber { get; set; }
        public string DOB { get; set; }
        public DateTime LastActivityDate { get; set; }
        public string Note { get; set; }
        public int ProfileId { get; set; }
    }
}
