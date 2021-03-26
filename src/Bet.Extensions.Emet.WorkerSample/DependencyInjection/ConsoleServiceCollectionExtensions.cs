using System;
using System.Linq;

using Bet.Extensions.Emet;
using Bet.Extensions.Emet.WorkerSample;
using Bet.Extensions.Emet.WorkerSample.Services;

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using RulesEngine.HelperFunctions;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ConsoleServiceCollectionExtensions
    {
        public static void ConfigureServices(HostBuilderContext hostBuilder, IServiceCollection services)
        {
            services.AddScoped<IMain, Main>();

            services.AddCountryWorkflow()
                    .AddDiscountWorkflow()
                    .AddReturenentWorkflow();
        }

        public static IServiceCollection AddReturenentWorkflow(this IServiceCollection services)
        {
            services.AddTransient<IRetirementService>(
                    sp =>
                    {
                        var providers = sp.GetServices<IEmetProvider>().ToList();
                        var provider = providers.FirstOrDefault(x => x.Name == Workflows.RetirementEligibilityWorkflow);

                        return new RetirementService(provider);
                    });

            services.AddEmetProvider(Workflows.RetirementEligibilityWorkflow)
                    .AddFileLoader(configure: (options, sp) =>
                    {
                        options.FileName = $"Data/{Workflows.RetirementEligibilityWorkflow}.json";
                    });

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
                    .AddFileLoader(configure: (options, sp) =>
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
                        settings.CustomTypes = new Type[] { typeof(ExpressionUtils) };
                    })
                    .AddFileLoader(configure: (options, sp) =>
                    {
                        options.FileName = $"Data/{Workflows.CountryWorkflow}.json";
                    });
            return services;
        }
    }
}
