using System;

using Azure.Storage.Blobs;

using Bet.Extensions.Emet;
using Bet.Extensions.Emet.Azure.Storage;
using Bet.Extensions.Emet.Azure.Storage.Options;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class EmetAzureProviderBuilderExtensions
    {
        /// <summary>
        /// Add <see cref="EmetAzureStorageStore"/> storage provider for the workflow registration.
        /// </summary>
        /// <param name="builder">The <see cref="IEmetProviderBuilder"/>.</param>
        /// <param name="sectionName">The name of the section for the configurations registration.</param>
        /// <param name="configOptions">The <see cref="EmetAzureStorageStoreOptions"/> configuration action.</param>
        /// <param name="configBlobOptions">The <see cref="BlobClientOptions"/> configuration action.</param>
        /// <param name="loaderServiceLifeTime">The service lifetime of the provide, it not specified the lifetime is used of the <see cref="IEmetProvider"/>.</param>
        /// <returns></returns>
        public static IEmetProviderBuilder AddAzureStorageLoader(
            this IEmetProviderBuilder builder,
            string sectionName = nameof(EmetAzureStorageStoreOptions),
            Action<EmetAzureStorageStoreOptions, IConfiguration>? configOptions = null,
            Action<BlobClientOptions>? configBlobOptions = null,
            ServiceLifetime? loaderServiceLifeTime = null)
        {
            builder.Services.AddChangeTokenOptions(sectionName, builder.Name, configOptions);

            var options = new BlobClientOptions();
            configBlobOptions?.Invoke(options);

            builder.Services.Add(new ServiceDescriptor(
                typeof(IEmetStore),
                sp =>
                {
                    var logger = sp.GetRequiredService<ILogger<EmetAzureStorageStore>>();
                    var options = sp.GetRequiredService<IOptionsMonitor<EmetAzureStorageStoreOptions>>();

                    return new EmetAzureStorageStore(builder.Name, options, logger);
                },
                loaderServiceLifeTime ?? builder.ServiceLifetime));

            return builder;
        }
    }
}
