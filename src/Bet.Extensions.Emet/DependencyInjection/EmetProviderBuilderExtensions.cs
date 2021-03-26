using System;
using System.Linq;

using Bet.Extensions.Emet;
using Bet.Extensions.Emet.Options;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using RulesEngine.Models;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class EmetProviderBuilderExtensions
    {
        /// <summary>
        /// Adds <see cref="EmetFileStore"/>.
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="sectionName"></param>
        /// <param name="configure"></param>
        /// <returns></returns>
        public static IEmetProviderBuilder AddFileLoader(
            this IEmetProviderBuilder builder,
            string sectionName = nameof(EmetFileStoreOptions),
            Action<EmetFileStoreOptions, IServiceProvider>? configure = null)
        {
            // add options that support reload
            builder.Services.AddChangeTokenOptions(sectionName, builder.Name, configure);

            builder.Services.Add(new ServiceDescriptor(
                            typeof(IEmetStore),
                            sp =>
                            {
                                var logger = sp.GetRequiredService<ILogger<EmetFileStore>>();
                                var options = sp.GetRequiredService<IOptionsMonitor<EmetFileStoreOptions>>();

                                return new EmetFileStore(builder.Name, options, logger);
                            },
                            builder.ServiceLifetime));

            builder.Services.Add(
                            new ServiceDescriptor(
                                typeof(IEmetProvider),
                                sp =>
                                {
                                    var logger = sp.GetRequiredService<ILoggerFactory>().CreateLogger(builder.Name);
                                    var stores = sp.GetServices<IEmetStore>().ToList();

                                    var store = stores.FirstOrDefault(x => x.Name == builder.Name);

                                    var se = new ReSettings();
                                    builder.Settings?.Invoke(se);

                                    return new EmetProvider(builder.Name, se, store, logger);
                                },
                                builder.ServiceLifetime));

            return builder;
        }
    }
}
