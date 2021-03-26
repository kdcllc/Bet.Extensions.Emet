using System.Threading;
using System.Threading.Tasks;

using Bet.Extensions.Emet.WorkerSample.Models;

namespace Bet.Extensions.Emet.WorkerSample.Services
{
    public interface IRetirementService
    {
        Task<bool> IsEligibleAsync(Employee employee, CancellationToken cancellationToken = default);
    }
}
