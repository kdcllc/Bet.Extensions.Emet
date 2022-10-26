using Azure.Storage.Blobs;

namespace Bet.Extensions.Emet.Azure.Storage
{
    public interface IEmetBlobClientFactory
    {
        BlobServiceClient GetClient();

        string Name { get; }
    }
}
