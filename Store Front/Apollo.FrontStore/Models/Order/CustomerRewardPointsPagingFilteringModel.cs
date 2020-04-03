namespace Apollo.FrontStore.Models.Order
{
    public class CustomerRewardPointsPagingFilteringModel : BasePageableModel
    {
        public int SelectedAccountId { get; set; }

        public CustomerRewardPointsPagingFilteringModel()
        {
            PageSize = 5; //Default
            PageNumber = 1; //Default
        }
    }
}