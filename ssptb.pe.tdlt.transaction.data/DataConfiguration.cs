using Couchbase.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using ssptb.pe.tdlt.transaction.common.Helpers;
using ssptb.pe.tdlt.transaction.common.Settings;
using ssptb.pe.tdlt.transaction.data.Helpers;
using ssptb.pe.tdlt.transaction.data.Repositories;

namespace ssptb.pe.tdlt.transaction.data;
public static class DataConfiguration
{
    public static IServiceCollection AddDatabaseConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        var serviceProvider = services.BuildServiceProvider();
        var mongoDbSettings = serviceProvider.GetService<IOptions<MongoDbSettings>>()?.Value;

        if (EnvironmentHelper.IsDevelopment())
        {
            mongoDbSettings.ConnectionString = "mongodb://localhost:27017";
        }

        if (mongoDbSettings == null)
        {
            throw new InvalidOperationException("MongoDBSettings not configured properly.");
        }

        // Configurar MongoDB
        services.AddSingleton<IMongoClient, MongoClient>(sp =>
            new MongoClient(mongoDbSettings.ConnectionString));
        services.AddScoped(sp => sp.GetRequiredService<IMongoClient>().GetDatabase("Transaction"));

        services.AddSingleton<MongoDBInitializer>();

        return services;
    }

    public static IServiceCollection AddDataServicesConfiguration(this IServiceCollection services)
    {
        services.AddTransient<IMongoDBHelper, MongoDBHelper>();
        services.AddTransient<ITransactionRepository, TransactionRepository>();

        return services;
    }

    public static void InitializeMongoDatabase(this IServiceProvider services)
    {
        var mongoInitializer = services.GetRequiredService<MongoDBInitializer>();
        mongoInitializer.InitializeDatabase();
    }
}
