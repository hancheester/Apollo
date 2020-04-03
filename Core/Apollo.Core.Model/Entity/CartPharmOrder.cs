using System.Collections.Generic;

namespace Apollo.Core.Model.Entity
{
    public class CartPharmOrder : BaseEntity
    {
        public int ProfileId { get; set; }
        public bool TakenByOwner { get; set; }
        public string OwnerAge { get; set; }
        public bool HasOtherCondMed { get; set; }
        public string OtherCondMed { get; set; }
        public string Allergy { get; set; }
        public List<CartPharmItem> Items { get; set; }

        public CartPharmOrder()
        {
            this.Items = new List<CartPharmItem>();
        }
    }
}
