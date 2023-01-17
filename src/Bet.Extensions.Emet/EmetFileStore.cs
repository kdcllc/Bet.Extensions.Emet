using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

using Bet.Extensions.Emet.Options;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using Newtonsoft.Json;

using RulesEngine.Models;

using re = RulesEngine.RulesEngine;

namespace Bet.Extensions.Emet;

public class EmetFileStore : IEmetStore
{
    private readonly ILogger<EmetFileStore> _logger;
    private EmetFileStoreOptions _options;

    /// <summary>
    /// Initializes a new instance of the <see cref="EmetFileStore"/> class.
    /// </summary>
    /// <param name="providerName"></param>
    /// <param name="optionsMonitor"></param>
    /// <param name="logger"></param>
    public EmetFileStore(
        string providerName,
        IOptionsMonitor<EmetFileStoreOptions> optionsMonitor,
        ILogger<EmetFileStore> logger)
    {
        if (string.IsNullOrEmpty(providerName))
        {
            throw new ArgumentException($"'{nameof(providerName)}' cannot be null or empty.", nameof(providerName));
        }

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

    /// <inheritdoc/>
    public string Name { get; }

    /// <inheritdoc/>
    public Task<Workflow[]> RetrieveAsync(CancellationToken cancellationToken)
    {
        var fileName = string.Empty;
        try
        {
            var result = new List<Workflow>();
            foreach (var workflowName in _options.FileNames)
            {
                fileName = workflowName;

                var filePath = Path.Combine(Directory.GetCurrentDirectory(), workflowName);
                if (Path.IsPathRooted(workflowName))
                {
                    filePath = workflowName;
                }

                var fileData = File.ReadAllText(filePath);
                var workflow = JsonConvert.DeserializeObject<Workflow[]>(fileData);
                if (workflow != null)
                {
                    result.AddRange(workflow);
                }
            }

            return Task.FromResult(result.ToArray());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "{name} failed to retrieve: {fileName}", nameof(EmetFileStore), fileName);
            throw;
        }
    }

    /// <inheritdoc/>
    public Task PersistAsync(Workflow[] workflows, CancellationToken cancellationToken)
    {
        var fileName = string.Empty;
        try
        {
            var engine = new re(workflows);

            // provides with workflow validation
            engine.AddWorkflow(workflows);

            foreach (var workflow in workflows)
            {
                fileName = $"{workflow.WorkflowName}.json";

                var filePath = Path.Combine(Directory.GetCurrentDirectory(), fileName);
                if (Path.IsPathRooted(fileName))
                {
                    filePath = fileName;
                }

                var serWorkflow = JsonConvert.SerializeObject(workflows);
                File.WriteAllText(filePath, serWorkflow);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "{name} failed to persist: {filename}", nameof(EmetFileStore), fileName);
            throw;
        }

        return Task.CompletedTask;
    }
}
