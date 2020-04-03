namespace Apollo.AdminStore.WebForm.Classes
{
    public class LineItemLite
    {
        public int LineItemId { get; set; }
        public int OrderId { get; set; }
        public int PendingQuantity { get; set; }
        public int AllocatedQuantity { get; set; }
        public int ShippedQuantity { get; set; }
        public string StatusCode { get; set; }
        public string Name { get; set; }
        public string Option { get; set; }
        
        public string Comment { get; set; }
        public bool UpdateOrderLastActivityDateFlag { get; set; }

        public LineItemLite()
        {
            Comment = string.Empty;
        }
    }
}