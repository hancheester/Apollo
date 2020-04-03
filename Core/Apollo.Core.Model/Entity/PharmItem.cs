
namespace Apollo.Core.Model.Entity
{
    public class PharmItem : BaseEntity
    {
        public int PharmOrderId { get; set; }
        public int ProductId { get; set; }
        public int ProductPriceId { get; set; }
        public string Symptoms { get; set; }
        public string Age { get; set; }
        public bool HasOtherCondMed { get; set; }
        public string OtherCondMed { get; set; }
        public string PersistedInDays { get; set; }
        public string ActionTaken { get; set; }
        public int Quantity { get; set; }
        public string Name { get; set; }
        public string Option { get; set; }
        public string HasTaken { get; set; }
        public string TakenQuantity { get; set; }
        public string LastTimeTaken { get; set; }
        public string MedForSymptom { get; set; }
    }
}
