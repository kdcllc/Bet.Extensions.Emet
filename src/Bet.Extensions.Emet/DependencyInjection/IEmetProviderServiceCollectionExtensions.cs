using System;

using Microsoft.Extensions.Logging;

using RulesEngine.Models;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class IEmetProviderServiceCollectionExtensions
    {
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
