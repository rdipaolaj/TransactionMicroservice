using Microsoft.Extensions.DependencyInjection;
using ssptb.pe.tdlt.transaction.secretsmanager.Services;

namespace ssptb.pe.tdlt.transaction.secretsmanager
{
    public static class SecretManagerConfiguration
    {
        public static IServiceCollection AddSecretManagerConfiguration(this IServiceCollection services)
        {
            services.AddSingleton<ISecretManagerService, SecretManagerService>();

            return services;
        }
    }
}
