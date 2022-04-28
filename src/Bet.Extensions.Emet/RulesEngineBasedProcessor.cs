using System.Linq;
using System.Threading.Tasks;

using RulesEngine.Models;

namespace Bet.Extensions.Emet;

public class RulesEngineBasedProcessor<TDataModel>
    where TDataModel : class
{
    private Workflow[] _mWorkflows;
    private RulesEngine.RulesEngine _mRulesEngine;

    public RulesEngineBasedProcessor(Workflow[] workflows)
    {
        _mWorkflows = workflows;
        _mRulesEngine = new RulesEngine.RulesEngine(_mWorkflows, null);
    }

    public async Task<bool> MatchesAnyRule(TDataModel targetObject)
    {
        var results = await _mRulesEngine.ExecuteAllRulesAsync("PreprocessorWorkflowChecks", new dynamic[] { targetObject });
        return results.Any(r => r.IsSuccess);
    }
}
