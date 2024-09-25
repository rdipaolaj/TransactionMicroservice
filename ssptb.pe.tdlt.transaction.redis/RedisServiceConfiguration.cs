using Microsoft.Extensions.DependencyInjection;
using ssptb.pe.tdlt.transaction.redis.Services;

namespace ssptb.pe.tdlt.transaction.redis;

/// <summary>
/// Métodos de extensión para configuración de redis
/// </summary>
public static class RedisServiceConfiguration
{
    /// <summary>
    /// Configuración de servicio redis
    /// </summary>
    /// <param name="services"></param>
    /// <returns>Retorna service collection para que funcione como método de extensión</returns>
    public static IServiceCollection AddRedisServiceConfiguration(this IServiceCollection services)
    {
        services.AddSingleton<IRedisService, RedisService>();

        return services;
    }
}
