using System;
using System.Threading;
using System.Threading.Tasks;

using RulesEngine.Extensions;

namespace Bet.Extensions.Emet.WorkerSample.Services
{
    public class DiscountService : IDiscountService
    {
        private IEmetProvider _provider;

        public DiscountService(IEmetProvider provider)
        {
            _provider = provider;
        }

        public async Task<decimal> CalculateDiscountAsync(dynamic[] inputs, CancellationToken cancellationToken = default)
        {
            var discount = 0m;
            var engine = await _provider.RulesEngine.Value;

            var resultList = await engine.ExecuteAllRulesAsync("Discount", inputs);

            resultList.OnSuccess((eventName) =>
            {
                discount = Convert.ToDecimal(eventName);
            });

            return discount;
        }
    }
}
