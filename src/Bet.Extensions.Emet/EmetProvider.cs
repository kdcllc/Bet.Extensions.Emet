using System;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.Hosting;

using RulesEngine.Interfaces;
using RulesEngine.Models;

namespace Bet.Extensions.Emet;

public class EmetProvider : IEmetProvider
{
    private readonly ReSettings _settings;

    public EmetProvider(
        string emetProviderName,
        ReSettings settings,
        IEmetStore store,
        IHostApplicationLifetime hostApplicationLifetime)
    {
        if (string.IsNullOrEmpty(emetProviderName))
        {
            throw new ArgumentException($"'{nameof(emetProviderName)}' cannot be null or empty.", nameof(emetProviderName));
        }

        Name = emetProviderName;

        Store = store ?? throw new ArgumentNullException(nameof(store));
        _settings = settings ?? throw new ArgumentNullException(nameof(settings));

        RulesEngine = new Lazy<Task<IRulesEngine>>(() => GetRulesEngine(hostApplicationLifetime.ApplicationStopping));
    }

    public Lazy<Task<IRulesEngine>> RulesEngine { get; }

    public string Name { get; }

    public IEmetStore Store { get; }

    private async Task<IRulesEngine> GetRulesEngine(CancellationToken cancellationToken)
    {
        // TODO: add cancellation token based on host
        var workflows = await Store.RetrieveAsync(cancellationToken);

        var engine = new RulesEngine.RulesEngine(workflows, _settings);
        return engine;
    }
}
