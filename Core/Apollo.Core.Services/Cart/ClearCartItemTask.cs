using Apollo.Core.Services.Interfaces;
using Apollo.Core.Services.Tasks;
using System;

namespace Apollo.Core.Services.Cart
{
    public class ClearCartItemTask : ITask
    {
        private readonly ICartService _cartService;

        public ClearCartItemTask(ICartService cartService)
        {
            _cartService = cartService;
        }

        public void Execute()
        {
            //60*24 = 1 day
            //var olderThanMinutes = 1440; //TODO: move to settings
            var olderThanMinutes = 4320; // 3 days (02/07/2018)
            //Do not delete more than 1000 records per time. This way the system is not slowed down
            _cartService.DeleteOldCartItem(DateTime.Now.AddMinutes(-olderThanMinutes));
        }
    }
}
