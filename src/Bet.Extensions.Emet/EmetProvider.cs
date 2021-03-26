using System;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;

using RulesEngine.Interfaces;
using RulesEngine.Models;

namespace Bet.Extensions.Emet
{
    public class EmetProvider : IEmetProvider
    {
        private readonly ReSettings _settings;
        private readonly ILogger _logger;

        public EmetProvider(
            string emetProviderName,
            ReSettings settings,
            IEmetStore store,
            ILogger logger)
        {
            if (string.IsNullOrEmpty(emetProviderName))
            {
                throw new ArgumentException($"'{nameof(emetProviderName)}' cannot be null or empty.", nameof(emetProviderName));
            }

            Name = emetProviderName;

            Store = store;
            _logger = logger;
            _settings = settings;

            RulesEngine = new Lazy<Task<IRulesEngine>>(() => GetRulesEngine());
        }

        public Lazy<Task<IRulesEngine>> RulesEngine { get; }

        public string Name { get; }

        public IEmetStore Store { get; }

        private async Task<IRulesEngine> GetRulesEngine()
        {
            // TODO: add cancellation token based on host
            var workflows = await Store.RetrieveAsync(CancellationToken.None);

            var engine = new RulesEngine.RulesEngine(workflows, _logger, _settings);
            return engine;
        }
    }
}
