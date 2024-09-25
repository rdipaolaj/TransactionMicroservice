using Microsoft.Extensions.DependencyInjection;
using ssptb.pe.tdlt.transaction.commandhandler.Transaction;
using ssptb.pe.tdlt.transaction.infraestructure.Behaviors;
using ssptb.pe.tdlt.transaction.internalservices;
using ssptb.pe.tdlt.transaction.redis;
using ssptb.pe.tdlt.transaction.secretsmanager;

namespace ssptb.pe.tdlt.transaction.infraestructure.Modules;
public static class MediatorModule
{
    public static IServiceCollection AddMediatRAssemblyConfiguration(this IServiceCollection services)
    {
        services.AddMediatR(configuration =>
        {
            configuration.RegisterServicesFromAssemblyContaining(typeof(ConfirmTransactionCommandHandler));
            configuration.RegisterServicesFromAssemblyContaining(typeof(CreateTransactionCommandHandler));

            configuration.AddOpenBehavior(typeof(ValidatorBehavior<,>));
        });

        //services.AddValidatorsFromAssembly(typeof(AffiliateCommandValidator).Assembly);

        return services;
    }
    public static IServiceCollection AddCustomServicesConfiguration(this IServiceCollection services)
    {
        services.AddInternalServicesConfiguration();
        services.AddSecretManagerConfiguration();
        services.AddRedisServiceConfiguration();

        return services;
    }
}
