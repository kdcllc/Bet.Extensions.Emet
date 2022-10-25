using System;

using Bet.Extensions.Emet;

using RulesEngine.Models;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// The default <see cref="EmetProviderBuilder"/> class to be used with DI registration process.
/// </summary>
public class EmetProviderBuilder : IEmetProviderBuilder
{
    /// <summary>
    /// Initializes a new instance of the <see cref="EmetProviderBuilder"/> class.
    /// Creates a <see cref="EmetProviderBuilder"/>.
    /// </summary>
    /// <param name="services">The DI services from DI container.</param>
    /// <param name="emetName">The name of the registration.</param>
    /// <param name="settings">The settings for <see cref="RulesEngine"/>.</param>
    /// <param name="serviceLifetime">The life time of the <see cref="RulesEngine.RulesEngine"/>. The default is <see cref="ServiceLifetime.Transient"/>.</param>
    /// <exception cref="ArgumentException">The argument Exception is thrown when it emitName is empty.</exception>
    /// <exception cref="ArgumentNullException">DI services can't be null.</exception>
    public EmetProviderBuilder(
        IServiceCollection services,
        string emetName,
        Action<ReSettings>? settings = null,
        ServiceLifetime serviceLifetime = ServiceLifetime.Transient)
    {
        if (string.IsNullOrEmpty(emetName))
        {
            throw new ArgumentException($"'{nameof(emetName)}' cannot be null or empty.", nameof(emetName));
        }

        Services = services ?? throw new ArgumentNullException(nameof(services));
        Name = emetName;
        Settings = settings;
        ServiceLifetime = serviceLifetime;
    }

    /// <summary>
    /// DI registered services.
    /// </summary>
    public IServiceCollection Services { get; }

    /// <summary>
    /// The name of the registered <see cref="EmetProvider"/>.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// The registration action for the settings.
    /// </summary>
    public Action<ReSettings>? Settings { get; }

    /// <summary>
    /// The lifetime for registered DI service.
    /// </summary>
    public ServiceLifetime ServiceLifetime { get; }
}
