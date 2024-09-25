using Microsoft.Extensions.DependencyInjection;
using ssptb.pe.tdlt.transaction.internalservices.Base;
using ssptb.pe.tdlt.transaction.internalservices.Blockchain;

namespace ssptb.pe.tdlt.transaction.internalservices;
public static class InternalServicesConfiguration
{
    public static IServiceCollection AddInternalServicesConfiguration(this IServiceCollection services)
    {
        services.AddTransient<IBaseService, BaseService>();
        services.AddTransient<IBlockchainService, BlockchainService>();

        return services;
    }
}
