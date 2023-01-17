using System;

using Azure.Identity;
using Azure.Storage.Blobs;

using Bet.Extensions.Emet.Azure.Storage.Options;

using Microsoft.Extensions.Options;

namespace Bet.Extensions.Emet.Azure.Storage;

public class EmetBlobClientFactory : IEmetBlobClientFactory
{
    private EmetAzureStorageStoreOptions _options;

    public EmetBlobClientFactory(
        string name,
        IOptionsMonitor<EmetAzureStorageStoreOptions> optionsMonitor)
    {
        if (string.IsNullOrEmpty(name))
        {
            throw new ArgumentException($"'{nameof(name)}' cannot be null or empty.", nameof(name));
        }

        Name = name;

        _options = optionsMonitor.Get(Name);

        if (_options == null)
        {
            throw new ArgumentNullException($"'{nameof(name)}' options cannot be null", nameof(name));
        }

        optionsMonitor.OnChange((options, name) =>
        {
            if (name == Name)
            {
                _options = options;
            }
        });
    }

    public string Name { get; }

    public BlobServiceClient GetClient()
    {

        if (string.IsNullOrEmpty(_options.ConnectionString))
        {
            return new BlobServiceClient(_options.BlobServiceUri, new DefaultAzureCredential(), _options?.BlobClientOptions ?? new BlobClientOptions());
        }

        return new BlobServiceClient(_options.ConnectionString, _options?.BlobClientOptions ?? new BlobClientOptions());
    }
}
