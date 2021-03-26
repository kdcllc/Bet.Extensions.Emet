using System;
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
            optionsMonitor.OnChange((o, n) =>
            {
                if (n == providerName)
                {
                    _options = o;
                }
            });

            _logger = logger;
        }

        public string Name { get; }

        public Task<WorkflowRules[]> RetrieveAsync(CancellationToken cancellationToken)
        {
            try
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
            catch (Exception ex)
            {
                _logger.LogError(ex, "EmetFileStore failed to retrieve: {filename}", _options.FileName);
                throw;
            }
        }

        public Task PersistAsync(WorkflowRules[] rules, CancellationToken cancellationToken)
        {
            try
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
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "EmetFileStore failed to persist: {filename}", _options.FileName);
                throw;
            }

            return Task.CompletedTask;
        }
    }
}
