namespace Apollo.FrontStore.Models.Order
{
    public class CustomerOrderPagingFilteringModel : BasePageableModel
    {
        public int SelectedOrderId { get; set; }
        public string q { get; set; }

        public CustomerOrderPagingFilteringModel()
        {
            PageSize = 5; //Default
            PageNumber = 1; //Default
        }
    }
}