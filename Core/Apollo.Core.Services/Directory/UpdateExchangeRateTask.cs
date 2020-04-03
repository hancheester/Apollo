using Apollo.Core.Services.Tasks;

namespace Apollo.Core.Services.Directory
{
    public class UpdateExchangeRateTask : ITask
    {
        private readonly ICurrencyService _currencyService;

        public UpdateExchangeRateTask(ICurrencyService currencyService)
        {
            _currencyService = currencyService;
        }

        public void Execute()
        {
            _currencyService.UpdateCurrencyLiveRates();
        }
    }
}
