using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;

using RulesEngine.Extensions;

namespace Bet.Extensions.Emet.WorkerSample.Services
{
    public class CountryService : ICountryService
    {
        private readonly IEmetProvider _provider;
        private readonly ILogger<CountryService> _logger;

        public CountryService(
            IEmetProvider provider,
            ILogger<CountryService> logger)
        {
            _provider = provider ?? throw new System.ArgumentNullException(nameof(provider));
            _logger = logger ?? throw new System.ArgumentNullException(nameof(logger));
        }

        public async Task<bool> IsAcceptableAsync(string countryName, CancellationToken cancellationToken = default)
        {
            var engine = await _provider.RulesEngine.Value;

            var input = new
            {
                country = countryName
            };

            var result = await engine.ExecuteAllRulesAsync(_provider.Name, input);

            result.OnResultFail((errorMessage) =>
            {
                _logger.LogError("{message}", errorMessage);
            });

            return !result.IsFailed();
        }
    }
}
