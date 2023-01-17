using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Bet.Extensions.Emet.Options;
using Bet.Extensions.Testing.Logging;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using Xunit;
using Xunit.Abstractions;

namespace Bet.Extensions.Emet.UnitTest;

public class EmetFileStoreUnitTests
{
    private readonly ITestOutputHelper _output;

    public EmetFileStoreUnitTests(ITestOutputHelper output)
    {
        _output = output ?? throw new System.ArgumentNullException(nameof(output));
    }

    [Fact]
    public async Task Test_FileStore_RetreieveAsync()
    {
        var dic = new Dictionary<string, string>
        {
            { "EmetFileStoreOptions:FileNames:0", "workflows/rules1.json" },
        };

        var configBuilder = new ConfigurationBuilder().AddInMemoryCollection(dic);
        var config = configBuilder.Build();

        var services = new ServiceCollection();

        services.AddSingleton<IConfiguration>(config);

        services.AddLogging(x => x.ClearProviders().AddXunit(_output));

        var storeName = "testStoreName";

        services.AddChangeTokenOptions<EmetFileStoreOptions>(sectionName: "EmetFileStoreOptions", optionName: storeName, configureAction: o => { });

        var sp = services.BuildServiceProvider();

        var optionsMonitor = sp.GetRequiredService<IOptionsMonitor<EmetFileStoreOptions>>();
        var logger = sp.GetRequiredService<ILogger<EmetFileStore>>();

        var store = new EmetFileStore(storeName, optionsMonitor, logger);

        var result = await store.RetrieveAsync(CancellationToken.None);
        Assert.NotNull(result);
    }
}
