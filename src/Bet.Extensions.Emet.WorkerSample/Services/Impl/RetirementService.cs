using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Bet.Extensions.Emet.WorkerSample.Models;

namespace Bet.Extensions.Emet.WorkerSample.Services
{
    public class RetirementService : IRetirementService
    {
        private readonly IEmetProvider _provider;

        public RetirementService(IEmetProvider provider)
        {
            _provider = provider;
        }

        public async Task<bool> IsEligibleAsync(Employee employee, CancellationToken cancellationToken = default)
        {
            var engine = await _provider.RulesEngine.Value;

            var results = await engine.ExecuteAllRulesAsync(_provider.Name, employee);
            return results.FirstOrDefault()?.IsSuccess ?? false;
        }
    }
}
