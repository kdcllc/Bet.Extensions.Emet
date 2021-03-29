using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

using Azure.Identity;
using Azure.Storage.Blobs;

using Bet.Extensions.Emet.Azure.Storage.Options;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using Newtonsoft.Json;

using RulesEngine.Models;

namespace Bet.Extensions.Emet.Azure.Storage
{
    public class EmetAzureStorageStore : IEmetStore
    {
        private static readonly JsonSerializer _serializer = new ();

        private readonly ILogger<EmetAzureStorageStore> _logger;
        private EmetAzureStorageStoreOptions _options;
        private BlobServiceClient _client;

        public EmetAzureStorageStore(
            string providerName,
            IOptionsMonitor<EmetAzureStorageStoreOptions> optionsMonitor,
            ILogger<EmetAzureStorageStore> logger)
        {
            if (string.IsNullOrEmpty(providerName))
            {
                throw new ArgumentException($"'{nameof(providerName)}' cannot be null or empty.", nameof(providerName));
            }

            Name = providerName;

            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            _options = optionsMonitor.Get(Name);

            optionsMonitor.OnChange((options, name) =>
            {
                if (name == Name)
                {
                    _options = options;
                }
            });

            if (string.IsNullOrEmpty(_options.ConnectionString))
            {
                _client = new BlobServiceClient(_options.BlobServiceUri, new DefaultAzureCredential(), _options?.BlobClientOptions ?? new BlobClientOptions());
            }
            else
            {
                _client = new BlobServiceClient(_options.ConnectionString, _options?.BlobClientOptions ?? new BlobClientOptions());
            }
        }

        public string Name {get; }

        public async Task PersistAsync(WorkflowRules[] rules, CancellationToken cancellationToken)
        {
            try
            {
                var result = await _client
                                .GetBlobContainerClient(_options.ContainerName)
                                .CreateIfNotExistsAsync(cancellationToken: cancellationToken);

                var blob = _client.GetBlobContainerClient(_options.ContainerName).GetBlobClient(_options.FileName);

                using var data = new MemoryStream();
                using var jsonStr = new StreamWriter(data);
                using var jsonTextWriter = new JsonTextWriter(jsonStr);
                _serializer.Serialize(jsonTextWriter, rules);
                jsonTextWriter.Flush();
                data.Position = 0;
                await blob.UploadAsync(data, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Azure Storage Blob failed to save the file", _options.FileName);
            }
        }

        public async Task<WorkflowRules[]> RetrieveAsync(CancellationToken cancellationToken)
        {
            try
            {
                var container = await _client
                    .GetBlobContainerClient(_options.ContainerName)
                    .CreateIfNotExistsAsync(cancellationToken: cancellationToken);

                var blob = _client.GetBlobContainerClient(_options.ContainerName).GetBlobClient(_options.FileName);

                var download = await blob.DownloadAsync(cancellationToken);

                using var data = new MemoryStream();
                await download.Value.Content.CopyToAsync(data);
                data.Position = 0;

                using var jsonStr = new StreamReader(data);
                using var jsonTextReader = new JsonTextReader(jsonStr);
                var result = _serializer.Deserialize<WorkflowRules[]>(jsonTextReader);

                return result ?? new WorkflowRules[0];
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Azure Storage Blob failed to retrieve the file", _options.FileName);
                throw;
            }
        }
    }
}
