using System;

using RulesEngine.Models;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// The default <see cref="EmetProviderBuilder"/> class to be used with DI registration process.
    /// </summary>
    public class EmetProviderBuilder : IEmetProviderBuilder
    {
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

        public IServiceCollection Services { get; }

        public string Name { get; }

        public Action<ReSettings>? Settings { get; }

        public ServiceLifetime ServiceLifetime { get; }
    }
}
