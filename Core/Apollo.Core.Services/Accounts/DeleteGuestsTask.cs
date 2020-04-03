using Apollo.Core.Services.Interfaces;
using Apollo.Core.Services.Tasks;
using System;

namespace Apollo.Core.Services.Accounts
{
    public class DeleteGuestsTask : ITask
    {
        private readonly IAccountService _accountService;

        public DeleteGuestsTask(IAccountService accountService)
        {
            _accountService = accountService;
        }

        public void Execute()
        {
            //60*24*3 = 3 days
            var olderThanMinutes = 4320; //TODO: move to settings
            //Do not delete more than 1000 records per time. This way the system is not slowed down
            _accountService.DeleteGuestCustomers(null, DateTime.Now.AddMinutes(-olderThanMinutes));
        }
    }
}
