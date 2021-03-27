using System;

using Azure.Storage.Blobs;

using Bet.Extensions.Emet.Options;

namespace Bet.Extensions.Emet.Azure.Storage.Options
{
    public class EmetAzureStorageStoreOptions : EmetFileStoreOptions
    {
        public Uri? BlobServiceUri { get; set; }

        public string ConnectionString { get; set; } = string.Empty;

        public string ContainerName { get; set; } = string.Empty;

        /// <summary>
        /// Adds Options for blob Client.
        /// </summary>
        internal BlobClientOptions? BlobClientOptions { get; set; }
    }
}
