using System;
using System.Linq;

using Bet.Extensions.Emet;
using Bet.Extensions.Emet.WorkerSample;
using Bet.Extensions.Emet.WorkerSample.Services;

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ConsoleServiceCollectionExtensions
    {
        public static void ConfigureServices(HostBuilderContext hostBuilder, IServiceCollection services)
        {
            services.AddScoped<IMain, Main>();

            services.AddCountryWorkflow()
                    .AddDiscountWorkflow()
                    .AddRetirementEligibilityWorkflow(EmetStoreEnum.AzureStorage);
        }

        public static IServiceCollection AddRetirementEligibilityWorkflow(this IServiceCollection services, EmetStoreEnum storeType)
        {
            var workflowName = storeType switch
            {
                EmetStoreEnum.FileSystem => Workflows.RetirementEligibilityWorkflow,
                EmetStoreEnum.AzureStorage => Workflows.AzureRetirementEligibilityWorkflow,
                _ => throw new NotImplementedException()
            };

            services.AddTransient<IRetirementService>(
                    sp =>
                    {
                        var providers = sp.GetServices<IEmetProvider>().ToList();
                        var provider = providers.FirstOrDefault(x => x.Name == workflowName);
                        if (provider == null)
                        {
                            throw new ArgumentNullException(workflowName, $"IEmetProvider wasn't register");
                        }

                        return new RetirementService(provider);
                    });

            var builder = services.AddEmetProvider(workflowName);

            if (storeType == EmetStoreEnum.FileSystem)
            {
                builder.AddFileLoader(configure: (options, config) =>
                {
                    options.FileName = $"Data/{Workflows.RetirementEligibilityWorkflow}.json";
                });
            }
            else if (storeType == EmetStoreEnum.AzureStorage)
            {
                builder.AddAzureStorageLoader(
                    workflowName,
                    configOptions: (options, config) =>
                    {
                        options.BlobServiceUri = new Uri(config["SharedBlobServiceUri"]);
                    });
            }

            return services;
        }

        public static IServiceCollection AddDiscountWorkflow(this IServiceCollection services)
        {
            services.AddTransient<IDiscountService>(
                    sp =>
                    {
                        var providers = sp.GetServices<IEmetProvider>().ToList();
                        var provider = providers.FirstOrDefault(x => x.Name == Workflows.DiscountWorkflow);

                        return new DiscountService(provider);
                    });

            services.AddEmetProvider(Workflows.DiscountWorkflow)
                    .AddFileLoader(configure: (options, config) =>
                    {
                        options.FileName = $"Data/{Workflows.DiscountWorkflow}.json";
                    });

            return services;
        }

        public static IServiceCollection AddCountryWorkflow(this IServiceCollection services)
        {
            services.AddTransient<ICountryService>(
                    sp =>
                    {
                        var providers = sp.GetServices<IEmetProvider>().ToList();
                        var logger = sp.GetRequiredService<ILogger<CountryService>>();
                        var provider = providers.FirstOrDefault(x => x.Name == Workflows.CountryWorkflow);

                        return new CountryService(provider, logger);
                    });

            services.AddEmetProvider(
                    Workflows.CountryWorkflow,
                    reSettings: settings =>
                    {
                        settings.CustomTypes = new Type[] { typeof(ExpressionHelper) };
                    })
                    .AddFileLoader(configure: (options, sp) =>
                    {
                        options.FileName = $"Data/{Workflows.CountryWorkflow}.json";
                    });
            return services;
        }
    }
}
