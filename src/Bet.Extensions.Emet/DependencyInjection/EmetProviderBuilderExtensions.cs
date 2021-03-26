using System;

using Bet.Extensions.Emet;
using Bet.Extensions.Emet.Options;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class EmetProviderBuilderExtensions
    {
        /// <summary>
        /// Adds <see cref="EmetFileStore"/> storage with the workflow registration.
        /// </summary>
        /// <param name="builder">The <see cref="IEmetProviderBuilder"/>.</param>
        /// <param name="sectionName"></param>
        /// <param name="configure"></param>
        /// <param name="loaderServiceLifeTime">The service lifetime of the provide, it not specified the lifetime is used of the <see cref="IEmetProvider"/>.</param>
        /// <returns></returns>
        public static IEmetProviderBuilder AddFileLoader(
            this IEmetProviderBuilder builder,
            string sectionName = nameof(EmetFileStoreOptions),
            Action<EmetFileStoreOptions, IConfiguration>? configure = null,
            ServiceLifetime? loaderServiceLifeTime = null)
        {
            // add options that support reload
            builder.Services.AddChangeTokenOptions(sectionName, builder.Name, configure);

            // add file store provider
            builder.Services.Add(new ServiceDescriptor(
                            typeof(IEmetStore),
                            sp =>
                            {
                                var logger = sp.GetRequiredService<ILogger<EmetFileStore>>();
                                var options = sp.GetRequiredService<IOptionsMonitor<EmetFileStoreOptions>>();

                                return new EmetFileStore(builder.Name, options, logger);
                            },
                            loaderServiceLifeTime ?? builder.ServiceLifetime));

            return builder;
        }
    }
}
