using System;

using Azure.Identity;
using Azure.Storage.Blobs;

using Bet.Extensions.Emet.Options;

namespace Bet.Extensions.Emet.Azure.Storage.Options;

public class EmetAzureStorageStoreOptions : EmetFileStoreOptions
{
    /// <summary>
    /// The full uri path to Azure Storage Blob: https://[name].blob.core.windows.net/.
    /// </summary>
    public Uri? BlobServiceUri { get; set; }

    /// <summary>
    /// <para>
    /// The connection string. If the connection string is empty then <see cref="DefaultAzureCredential"/> are used.
    /// For local development with <see cref="DefaultAzureCredential"/> please use https://github.com/kdcllc/AppAuthentication.
    /// </para>
    /// <para>az storage account show-connection-string --name {account_name} --resource-group {resource_group}.</para>
    /// </summary>
    public string ConnectionString { get; set; } = string.Empty;

    /// <summary>
    /// The name of the Azure Storage Blob container.
    /// </summary>
    public string ContainerName { get; set; } = string.Empty;

    /// <summary>
    /// Adds Options for blob Client.
    /// </summary>
    internal BlobClientOptions? BlobClientOptions { get; set; }
}
