using System;
using System.Linq;

using Bet.Extensions.Emet;

using Microsoft.Extensions.Logging;

using RulesEngine.Interfaces;
using RulesEngine.Models;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class IEmetProviderServiceCollectionExtensions
    {
        /// <summary>
        /// Adds Emet (Truths) Provider to the DI.
        /// It is required to add a store provider.
        /// </summary>
        /// <param name="services">The DI services.</param>
        /// <param name="providerName">The name for the emet provider.</param>
        /// <param name="serviceLifetime">The life time of the service, the default is <see cref="ServiceLifetime.Transient"/>.</param>
        /// <param name="reSettings">Custom Configurations for <see cref="IRulesEngine"/>.</param>
        /// <returns></returns>
        public static IEmetProviderBuilder AddEmetProvider(
            this IServiceCollection services,
            string providerName,
            ServiceLifetime serviceLifetime = ServiceLifetime.Transient,
            Action<ReSettings>? reSettings = null)
        {
            services.AddLogging(builder =>
            {
                builder.AddConsole();
                builder.AddDebug();
            });

            var se = new ReSettings();
            reSettings?.Invoke(se);

            // add iemet provider
            services.Add(
                        new ServiceDescriptor(
                                typeof(IEmetProvider),
                                sp =>
                                {
                                    var logger = sp.GetRequiredService<ILoggerFactory>().CreateLogger(providerName);
                                    var stores = sp.GetServices<IEmetStore>().ToList();

                                    var store = stores.FirstOrDefault(x => x.Name == providerName);
                                    if (store == null)
                                    {
                                        throw new ArgumentNullException(providerName, $"IEmetStore wasn't register");
                                    }

                                    return new EmetProvider(providerName, se, store, logger);
                                },
                                serviceLifetime));

            return new EmetProviderBuilder(services, providerName, reSettings, serviceLifetime);
        }
    }
}
