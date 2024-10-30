using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;

namespace ssptb.pe.tdlt.transaction.data.HealthCheck;
public class MongoDBHealthCheck : IHealthCheck
{
    private readonly IMongoClient _client;
    private readonly ILogger<MongoDBHealthCheck> _logger;

    public MongoDBHealthCheck(IMongoClient client, ILogger<MongoDBHealthCheck> logger)
    {
        _client = client;
        _logger = logger;
    }

    public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            // Intenta obtener una lista de bases de datos para verificar la conexión
            _client.ListDatabaseNames(cancellationToken);
            _logger.LogInformation("MongoDB connection is healthy.");
            return Task.FromResult(HealthCheckResult.Healthy("MongoDB connection is healthy."));
        }
        catch (Exception ex)
        {
            _logger.LogError($"MongoDB connection failed: {ex.Message}");
            return Task.FromResult(HealthCheckResult.Unhealthy($"MongoDB connection failed: {ex.Message}"));
        }
    }
}
