
namespace Apollo.Core.Model.Entity
{
    public class CartPharmItem : BaseEntity
    {
        public int CartPharmOrderId { get; set; }
        public int ProductId { get; set; }
        public int ProductPriceId { get; set; }
        public string Symptoms { get; set; }
        public string Age { get; set; }
        public bool HasOtherCondMed { get; set; }
        public string OtherCondMed { get; set; }
        public string PersistedInDays { get; set; }
        public string ActionTaken { get; set; }
        public string HasTaken { get; set; }
        public string TakenQuantity { get; set; }
        public string LastTimeTaken { get; set; }
        public string MedForSymptom { get; set; }
    }
}
