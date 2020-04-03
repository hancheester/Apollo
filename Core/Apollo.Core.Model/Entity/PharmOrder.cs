using System.Collections.Generic;

namespace Apollo.Core.Model.Entity
{
    public class PharmOrder : BaseEntity
    {
        public int OrderId { get; set; }
        public bool TakenByOwner { get; set; }
        public string OwnerAge { get; set; }
        public bool HasOtherCondMed { get; set; }
        public string OtherCondMed { get; set; }
        public string Allergy { get; set; }
        public List<PharmItem> Items { get; set; }

        public PharmOrder()
        {
            this.Items = new List<PharmItem>();
        }
    }
}
