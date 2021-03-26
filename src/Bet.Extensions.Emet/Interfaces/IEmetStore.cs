using System.Threading;
using System.Threading.Tasks;

using RulesEngine.Models;

namespace Bet.Extensions.Emet
{
    /// <summary>
    /// The basic interface that allows for retrieval and storage of <see cref="WorkflowRules"/>.
    /// </summary>
    public interface IEmetStore
    {
        /// <summary>
        /// The name of the store, usually corresponds with the name of <see cref="IEmetProvider.Name"/>.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets <see cref="WorkflowRules"/> that was persisted.
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<WorkflowRules[]> RetrieveAsync(CancellationToken cancellationToken);

        /// <summary>
        /// Persists the <see cref="WorkflowRules"/> to the medium.
        /// </summary>
        /// <param name="rules"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task PersistAsync(WorkflowRules[] rules, CancellationToken cancellationToken);
    }
}
