using System;

using Bet.Extensions.Emet;

using RulesEngine.Models;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// The <see cref="IServiceCollection"/> builder for <see cref="IEmetProvider"/>.
    /// </summary>
    public interface IEmetProviderBuilder
    {
        /// <summary>
        /// The DI collection of the builder.
        /// </summary>
        public IServiceCollection Services { get; }

        /// <summary>
        /// The name of the registered Emet (Truths) Set.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Configuration for <see cref="ReSettings"/>.
        /// </summary>
        Action<ReSettings>? Settings { get; }

        /// <summary>
        /// DI Service Life time.
        /// </summary>
        ServiceLifetime ServiceLifetime { get; }
    }
}
