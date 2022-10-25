using System;
using System.Linq;

using Bet.Extensions.Emet;
using Bet.Extensions.Emet.WorkerSample;
using Bet.Extensions.Emet.WorkerSample.Services;

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using RulesEngine.Models;

namespace Microsoft.Extensions.DependencyInjection;

public static class ConsoleServiceCollectionExtensions
{
    public static void ConfigureServices(HostBuilderContext hostBuilder, IServiceCollection services)
    {
        services.AddScoped<IMain, Main>();

        services
                .AddWorkflow<IRetirementService, RetirementService>(
                    Workflows.RetirementEligibilityWorkflow,
                    EmetStoreEnum.AzureStorage,
                    (provider, logger) => new RetirementService(provider))
                .AddWorkflow<IDiscountService, DiscountService>(
                    Workflows.DiscountWorkflow,
                    EmetStoreEnum.FileSystem,
                    (provider, logger) => new DiscountService(provider))
                .AddWorkflow<ICountryService, CountryService>(
                    Workflows.CountryWorkflow,
                    EmetStoreEnum.FileSystem,
                    (provider, logger) => new CountryService(provider, logger),
                    settings => settings.CustomTypes = new Type[] { typeof(ExpressionHelper) });
    }

    public static IServiceCollection AddWorkflow<T, TService>(
                this IServiceCollection services,
                string workflowName,
                EmetStoreEnum storeType,
                Func<IEmetProvider, ILogger<TService>, TService> configure,
                Action<ReSettings>? settings = null)
        where T : class
        where TService : T
    {
        services.AddTransient<T>(
                sp =>
                {
                    var providers = sp.GetServices<IEmetProvider>().ToList();
                    var provider = providers.FirstOrDefault(x => x.Name == workflowName);

                    if (provider == null)
                    {
                        throw new ArgumentNullException(workflowName, $"{nameof(IEmetProvider)} wasn't register");
                    }

                    var logger = sp.GetRequiredService<ILogger<TService>>();

                    return configure(provider, logger);
                });

        IEmetProviderBuilder builder;

        if (settings == null)
        {
            builder = services.AddEmetProvider(workflowName);
        }
        else
        {
            builder = services.AddEmetProvider(workflowName, reSettings: settings);
        }

        if (storeType == EmetStoreEnum.FileSystem)
        {
            builder.AddFileLoader(
                sectionName: $"FileWorkflows:{workflowName}",
                configure: (options, config) => { });
        }
        else if (storeType == EmetStoreEnum.AzureStorage)
        {
            builder.AddAzureStorageLoader(
                sectionName: $"AzureWorkflows:{workflowName}",
                configOptions: (options, config) =>
                {
                    options.BlobServiceUri = new Uri(config["AzureWorkflows:SharedBlobServiceUri"]);
                },
                loaderServiceLifeTime: ServiceLifetime.Singleton);
        }

        return services;
    }
}
