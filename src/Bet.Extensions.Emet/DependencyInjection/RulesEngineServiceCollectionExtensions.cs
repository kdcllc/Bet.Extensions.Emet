using RulesEngine.Interfaces;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class RulesEngineServiceCollectionExtensions
    {
        public static IServiceCollection AddRulesEngine(this IServiceCollection services)
        {
            services.AddTransient<IRulesEngine, RulesEngine.RulesEngine>(sp =>
            {
                return new RulesEngine.RulesEngine(null);
            });

            return services;
        }
    }
}
