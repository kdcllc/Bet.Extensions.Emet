using System.Threading;
using System.Threading.Tasks;

using RulesEngine.Models;

namespace Bet.Extensions.Emet
{
    public interface IRulesEngineStore
    {
        Task<WorkflowRules[]> GetAsync(CancellationToken cancellationToken);

        Task SaveAsync(WorkflowRules[] rules, CancellationToken cancellationToken);
    }
}
