using System.IO;
using System.Threading;
using System.Threading.Tasks;

using Bet.Extensions.Emet.Options;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using Newtonsoft.Json;

using RulesEngine.Models;

using re = RulesEngine.RulesEngine;

namespace Bet.Extensions.Emet
{
    public class EmetFileStore : IEmetStore
    {
        private readonly ILogger<EmetFileStore> _logger;
        private EmetFileStoreOptions _options;

        public EmetFileStore(
            string providerName,
            IOptionsMonitor<EmetFileStoreOptions> optionsMonitor,
            ILogger<EmetFileStore> logger)
        {
            Name = providerName;
            _options = optionsMonitor.Get(providerName);
            _logger = logger;
        }

        public string Name { get; }

        public Task<WorkflowRules[]> RetrieveAsync(CancellationToken cancellationToken)
        {
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), _options.FileName);
            if (Path.IsPathRooted(_options.FileName))
            {
                filePath = _options.FileName;
            }

            var fileData = File.ReadAllText(filePath);
            var rules = JsonConvert.DeserializeObject<WorkflowRules[]>(fileData);

            return Task.FromResult(rules);
        }

        public Task PersistAsync(WorkflowRules[] rules, CancellationToken cancellationToken)
        {
            var engine = new re(rules);

            // provides with workflow validation
            engine.AddWorkflow(rules);

            var workflows = JsonConvert.SerializeObject(rules);

            var filePath = Path.Combine(Directory.GetCurrentDirectory(), _options.FileName);
            if (Path.IsPathRooted(_options.FileName))
            {
                filePath = _options.FileName;
            }

            File.WriteAllText(filePath, workflows);

            return Task.CompletedTask;
        }
    }
}
