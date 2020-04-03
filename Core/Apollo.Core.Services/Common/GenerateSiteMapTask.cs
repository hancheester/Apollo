using Apollo.Core.Services.Interfaces;
using Apollo.Core.Services.Tasks;

namespace Apollo.Core.Services.Common
{
    public class GenerateSiteMapTask : ITask
    {
        private readonly IUtilityService _utilityService;

        public GenerateSiteMapTask(IUtilityService utilityService)
        {
            _utilityService = utilityService;
        }       

        public void Execute()
        {
            _utilityService.GenerateSitemap();
        }
    }
}
