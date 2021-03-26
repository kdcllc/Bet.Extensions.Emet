using System.Threading;
using System.Threading.Tasks;

namespace Bet.Extensions.Emet.WorkerSample.Services
{
    public interface ICountryService
    {
        Task<bool> IsAcceptableAsync(string countryName, CancellationToken cancellationToken = default);
    }
}
