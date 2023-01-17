using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

using Bet.Extensions.Emet.Azure.Storage;
using Bet.Extensions.Emet.Azure.Storage.Options;
using Bet.Extensions.Testing.Logging;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Microsoft.Extensions.Logging;

using Microsoft.Extensions.Options;

using Moq;

using Xunit;
using Xunit.Abstractions;

namespace Bet.Extensions.Emet.UnitTest;

public class EmetAzureStorageStoreUnitTests
{
    private readonly ITestOutputHelper _output;

    public EmetAzureStorageStoreUnitTests(ITestOutputHelper output)
    {
        _output = output ?? throw new System.ArgumentNullException(nameof(output));
    }

    [Fact]
    public async Task Test_AzureEmentStore_RetireveAsync()
    {
        var dic = new Dictionary<string, string>
        {
            { "EmetAzureStorageStoreOptions:BlobServiceUri", "https://blob.com" },
            { "EmetAzureStorageStoreOptions:FileNames:0", "rules1.json" },
        };

        var configBuilder = new ConfigurationBuilder().AddInMemoryCollection(dic);
        var config = configBuilder.Build();

        var services = new ServiceCollection();

        services.AddSingleton<IConfiguration>(config);

        services.AddLogging(x => x.ClearProviders().AddXunit(_output));

        var storeName = "testStoreName";

        services.AddChangeTokenOptions<EmetAzureStorageStoreOptions>(sectionName: "EmetAzureStorageStoreOptions", optionName: storeName, configureAction: o => { });

        var sp = services.BuildServiceProvider();

        var optionsMonitor = sp.GetRequiredService<IOptionsMonitor<EmetAzureStorageStoreOptions>>();
        var logger = sp.GetRequiredService<ILogger<EmetAzureStorageStore>>();

        var blobClintStub = new StubEmetBlobClientFactory(storeName);

        var store = new EmetAzureStorageStore(storeName, optionsMonitor, blobClintStub, logger);

        var result = await store.RetrieveAsync(CancellationToken.None);
        Assert.NotNull(result);
    }

    public class StubEmetBlobClientFactory : IEmetBlobClientFactory
    {
        public StubEmetBlobClientFactory(string name)
        {
            Name = name;
        }

        public string Name { get; }

        public BlobServiceClient GetClient()
        {
            var containerClient = new Mock<BlobContainerClient>();
            var blobServiceClient = new Mock<BlobServiceClient>();

            var blobClient = new Mock<BlobClient>();

            blobServiceClient.Setup(x => x.GetBlobContainerClient(It.IsAny<string>()))
                .Returns(containerClient.Object)
                .Verifiable();

            var mockResponse = new Mock<Response>();

            containerClient.Setup(x => x.CreateIfNotExistsAsync(
                It.IsAny<PublicAccessType>(),
                It.IsAny<Dictionary<string, string>>(),
                It.IsAny<BlobContainerEncryptionScopeOptions>(),
                It.IsAny<CancellationToken>())).ReturnsAsync(Response.FromValue(new Mock<BlobContainerInfo>().Object, mockResponse.Object));

            containerClient.Setup(x => x.GetBlobClient(It.IsAny<string>()))
                .Returns(blobClient.Object)
                .Verifiable();

            blobClient.Setup(x => x.DownloadAsync(It.IsAny<CancellationToken>()))
                   .ReturnsAsync(() =>
                   {
                       var bytes = File.ReadAllBytes(Path.Combine("workflows", "rules1.json"));
                       var b = BlobsModelFactory.BlobDownloadInfo(content: new MemoryStream(bytes));

                       return Response.FromValue(b, mockResponse.Object);
                   })
                    .Verifiable();

            return blobServiceClient.Object;
        }
    }
}
