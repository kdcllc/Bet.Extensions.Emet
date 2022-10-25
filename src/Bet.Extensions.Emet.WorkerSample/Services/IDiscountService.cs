using System.Threading;
using System.Threading.Tasks;

namespace Bet.Extensions.Emet.WorkerSample.Services;

public interface IDiscountService
{
    Task<decimal> CalculateDiscountAsync(dynamic[] inputs, CancellationToken cancellationToken = default);
}
