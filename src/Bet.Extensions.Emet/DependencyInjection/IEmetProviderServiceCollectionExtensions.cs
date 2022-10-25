using System;
using System.Linq;

using Bet.Extensions.Emet;

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using RulesEngine.Interfaces;
using RulesEngine.Models;

namespace Microsoft.Extensions.DependencyInjection;

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

        // add emet provider
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
                                    throw new ArgumentNullException(providerName, $"{nameof(IEmetStore)} wasn't register");
                                }

                                var hostApplicationLifeTime = sp.GetRequiredService<IHostApplicationLifetime>();

                                return new EmetProvider(providerName, se, store, hostApplicationLifeTime);
                            },
                            serviceLifetime));

        return new EmetProviderBuilder(services, providerName, reSettings, serviceLifetime);
    }

    /// <summary>
    /// Adds Emet (Truths) Provider to the DI.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="services"></param>
    /// <param name="serviceLifetime"></param>
    /// <param name="reSettings"></param>
    /// <returns></returns>
    public static IEmetProviderBuilder AddEmetProvider<T>(
        this IServiceCollection services,
        ServiceLifetime serviceLifetime = ServiceLifetime.Transient,
        Action<ReSettings>? reSettings = null) where T : class
    {
        var name = typeof(T).Name;

        return services.AddEmetProvider(name, serviceLifetime, reSettings);
    }
}
