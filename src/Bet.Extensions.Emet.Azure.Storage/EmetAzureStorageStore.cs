using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

using Azure.Storage.Blobs;

using Bet.Extensions.Emet.Azure.Storage.Options;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using Newtonsoft.Json;

using RulesEngine.Models;

namespace Bet.Extensions.Emet.Azure.Storage;

public class EmetAzureStorageStore : IEmetStore
{
    private static readonly JsonSerializer _serializer = new();

    private readonly ILogger<EmetAzureStorageStore> _logger;
    private EmetAzureStorageStoreOptions _options;
    private BlobServiceClient _client;

    /// <summary>
    /// Initializes a new instance of the <see cref="EmetAzureStorageStore"/> class.
    /// </summary>
    /// <param name="providerName"></param>
    /// <param name="optionsMonitor"></param>
    /// <param name="client"></param>
    /// <param name="logger"></param>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="ArgumentNullException"></exception>
    public EmetAzureStorageStore(
        string providerName,
        IOptionsMonitor<EmetAzureStorageStoreOptions> optionsMonitor,
        IEmetBlobClientFactory client,
        ILogger<EmetAzureStorageStore> logger)
    {
        if (string.IsNullOrEmpty(providerName))
        {
            throw new ArgumentException($"'{nameof(providerName)}' cannot be null or empty.", nameof(providerName));
        }

        Name = providerName;

        _client = client.GetClient();

        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        _options = optionsMonitor.Get(Name);

        if (_options == null)
        {
            throw new ArgumentNullException($"'{nameof(providerName)}' options cannot be null", nameof(providerName));
        }

        optionsMonitor.OnChange((options, name) =>
        {
            if (name == Name)
            {
                _options = options;
            }
        });
    }

    /// <inheritdoc/>
    public string Name { get; }

    /// <inheritdoc/>
    public async Task PersistAsync(Workflow[] workflows, CancellationToken cancellationToken)
    {
        var fileName = string.Empty;
        try
        {
            var container = await _client
                            .GetBlobContainerClient(_options!.ContainerName)
                            .CreateIfNotExistsAsync(cancellationToken: cancellationToken);

            foreach (var flow in workflows)
            {
                fileName = $"{flow}.json";

                var blob = _client.GetBlobContainerClient(_options.ContainerName).GetBlobClient(fileName);

                using var data = new MemoryStream();
                using var jsonStr = new StreamWriter(data);
                using var jsonTextWriter = new JsonTextWriter(jsonStr);
                _serializer.Serialize(jsonTextWriter, workflows);
                jsonTextWriter.Flush();
                data.Position = 0;
                await blob.UploadAsync(data, cancellationToken);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "{name} failed to save the file {fileName}.", nameof(EmetAzureStorageStore), fileName);
        }
    }

    /// <inheritdoc/>
    public async Task<Workflow[]> RetrieveAsync(CancellationToken cancellationToken)
    {
        var fileName = string.Empty;

        try
        {
            var result = new List<Workflow>();

            var container = await _client
                .GetBlobContainerClient(_options!.ContainerName)
                .CreateIfNotExistsAsync(cancellationToken: cancellationToken);

            foreach (var file in _options.FileNames)
            {
                fileName = file;

                var blob = _client.GetBlobContainerClient(_options.ContainerName).GetBlobClient(fileName);

                var download = await blob.DownloadAsync(cancellationToken);

                using var data = new MemoryStream();
                await download.Value.Content.CopyToAsync(data);
                data.Position = 0;

                using var jsonStr = new StreamReader(data);
                using var jsonTextReader = new JsonTextReader(jsonStr);
                var workflow = _serializer.Deserialize<Workflow[]>(jsonTextReader);
                if (workflow != null)
                {
                    result.AddRange(workflow);
                }
            }


            return result.ToArray();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "{name} failed to retrieve the file {fileName}", nameof(EmetAzureStorageStore), fileName);
            throw;
        }
    }
}
