using Apollo.Core.Services.Interfaces;
using Apollo.Core.Services.Tasks;
using System;

namespace Apollo.Core.Services.Common
{
    public class ClearGenericAttributeTask : ITask
    {
        private readonly IGenericAttributeService _genericAttributeService;

        public ClearGenericAttributeTask(IGenericAttributeService genericAttributeService)
        {
            _genericAttributeService = genericAttributeService;
        }

        public void Execute()
        {
            //60*24 = 1 day
            var olderThanMinutes = 1440; //TODO: move to settings
            //Do not delete more than 1000 records per time. This way the system is not slowed down
            _genericAttributeService.DeleteOldGenericAttribute(DateTime.Now.AddMinutes(-olderThanMinutes));
        }
    }
}
