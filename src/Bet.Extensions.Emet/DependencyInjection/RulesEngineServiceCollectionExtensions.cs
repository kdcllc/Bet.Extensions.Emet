using System;

using Microsoft.Extensions.Logging;

using RulesEngine.Interfaces;
using RulesEngine.Models;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class RulesEngineServiceCollectionExtensions
    {
        public static IServiceCollection AddRulesEngine(
            this IServiceCollection services,
            Action<ReSettings>? reSettings = null)
        {
            services.AddTransient<IRulesEngine, RulesEngine.RulesEngine>(sp =>
            {
                var logger = sp.GetRequiredService<ILogger>();

                var se = new ReSettings();
                reSettings?.Invoke(se);
                return new RulesEngine.RulesEngine(jsonConfig: null, logger: logger, reSettings: se);
            });

            return services;
        }
    }
}
