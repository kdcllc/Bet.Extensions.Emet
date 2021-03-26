using System.Linq;

using Bet.Extensions.Emet;
using Bet.Extensions.Emet.WorkerSample;
using Bet.Extensions.Emet.WorkerSample.Services;

using Microsoft.Extensions.Hosting;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ConsoleServiceCollectionExtensions
    {
        public static void ConfigureServices(HostBuilderContext hostBuilder, IServiceCollection services)
        {
            services.AddScoped<IMain, Main>();

            services.AddTransient<IDiscountService>(
                    sp =>
                    {
                        var providers = sp.GetServices<IEmetProvider>().ToList();
                        var provider = providers.FirstOrDefault(x => x.Name == Constants.DiscountWorkflow);

                        return new DiscountService(provider);
                    });

            services.AddEmetProvider(Constants.DiscountWorkflow)
                    .AddFileLoader(configure: (options, sp) =>
                    {
                        options.FileName = $"Data/{Constants.DiscountWorkflow}.json";
                    });

            services.AddTransient<IRetirementService>(
                    sp =>
                    {
                        var providers = sp.GetServices<IEmetProvider>().ToList();
                        var provider = providers.FirstOrDefault(x => x.Name == Constants.RetirementEligibilityWorkflow);

                        return new RetirementService(provider);
                    });

            services.AddEmetProvider(Constants.RetirementEligibilityWorkflow)
                    .AddFileLoader(configure: (options, sp) =>
                    {
                        options.FileName = $"Data/{Constants.RetirementEligibilityWorkflow}.json";
                    });
        }
    }
}
