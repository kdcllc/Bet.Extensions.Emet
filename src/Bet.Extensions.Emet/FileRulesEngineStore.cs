using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;

using RulesEngine.Models;

using re = RulesEngine.RulesEngine;

namespace Bet.Extensions.Emet
{
    public class FileRulesEngineStore : IRulesEngineStore
    {
        private readonly ILogger<FileRulesEngineStore> _logger;

        public FileRulesEngineStore(ILogger<FileRulesEngineStore> logger)
        {
            _logger = logger;
        }

        public Task<WorkflowRules[]> GetAsync(CancellationToken cancellationToken)
        {
            return null;
        }

        public Task SaveAsync(WorkflowRules[] rules, CancellationToken cancellationToken)
        {
            var engine = new re(null);

            // provides with workflow validation
            engine.AddWorkflow(rules);

            return Task.CompletedTask;
        }
    }
}
