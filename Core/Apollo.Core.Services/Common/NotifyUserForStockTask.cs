using Apollo.Core.Services.Interfaces;
using Apollo.Core.Services.Tasks;

namespace Apollo.Core.Services.Common
{
    public class NotifyUserForStockTask : ITask
    {
        private readonly IUtilityService _utilityService;

        public NotifyUserForStockTask(IUtilityService utilityService)
        {
            _utilityService = utilityService;
        }
        public void Execute()
        {
            _utilityService.NotifyUserForStock();
        }
    }
}
