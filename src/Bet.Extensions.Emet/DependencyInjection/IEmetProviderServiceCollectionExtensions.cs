using System;

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
        /// <param name="emetProviderName">The name for the emet provider.</param>
        /// <param name="serviceLifetime">The life time of the service, the default is <see cref="ServiceLifetime.Transient"/>.</param>
        /// <param name="reSettings">Custom Configurations for <see cref="IRulesEngine"/>.</param>
        /// <returns></returns>
        public static IEmetProviderBuilder AddEmetProvider(
            this IServiceCollection services,
            string emetProviderName,
            ServiceLifetime serviceLifetime = ServiceLifetime.Transient,
            Action<ReSettings>? reSettings = null)
        {
            services.AddLogging(builder =>
            {
                builder.AddConsole();
                builder.AddDebug();
            });

            return new EmetProviderBuilder(services, emetProviderName, reSettings, serviceLifetime);
        }
    }
}
