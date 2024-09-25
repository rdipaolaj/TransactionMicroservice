using Couchbase;
using Couchbase.Core.Exceptions;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ssptb.pe.tdlt.transaction.common.Settings;

namespace ssptb.pe.tdlt.transaction.data.HealthCheck;
public class CouchBaseHealthCheck : IHealthCheck
{
    private readonly CouchBaseSettings _couchbaseSettings;
    private readonly ILogger<CouchBaseHealthCheck> _logger;

    public CouchBaseHealthCheck(IOptions<CouchBaseSettings> couchbaseSettings, ILogger<CouchBaseHealthCheck> logger)
    {
        _couchbaseSettings = couchbaseSettings.Value;
        _logger = logger;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        if (_couchbaseSettings == null)
        {
            _logger.LogError("CouchBaseSettings is not configured.");
            return HealthCheckResult.Unhealthy("CouchBaseSettings is not configured.");
        }

        try
        {
            var clusterOptions = new ClusterOptions
            {
                UserName = _couchbaseSettings.UserName,
                Password = _couchbaseSettings.Password
            };
            clusterOptions.ApplyProfile("wan-development");

            // Conectar al clúster
            using var cluster = await Cluster.ConnectAsync(_couchbaseSettings.ConnectionString, clusterOptions);

            // Acceder al bucket
            var bucket = await cluster.BucketAsync(_couchbaseSettings.BucketName);

            // Obtener la colección predeterminada
            var collection = bucket.DefaultCollection();

            // Realizar una operación sencilla para verificar el acceso al bucket
            // Intentaremos obtener un documento que probablemente no exista
            var key = "healthcheck-key";
            var existsResult = await collection.ExistsAsync(key);

            _logger.LogInformation("Couchbase connection and bucket access are healthy.");
            return HealthCheckResult.Healthy("Couchbase connection and bucket access are healthy.");
        }
        catch (BucketNotFoundException ex)
        {
            _logger.LogError($"Bucket '{_couchbaseSettings.BucketName}' not found: {ex.Message}");
            return HealthCheckResult.Unhealthy($"Bucket '{_couchbaseSettings.BucketName}' not found: {ex.Message}");
        }
        catch (AuthenticationFailureException ex)
        {
            _logger.LogError($"Couchbase authentication failed: {ex.Message}");
            return HealthCheckResult.Unhealthy($"Couchbase authentication failed: {ex.Message}");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Couchbase connection failed: {ex.Message}");
            return HealthCheckResult.Unhealthy($"Couchbase connection failed: {ex.Message}");
        }
    }
}
