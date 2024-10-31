using Amazon.SecretsManager.Model;
using Amazon.SecretsManager;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ssptb.pe.tdlt.transaction.common.Settings;
using System.Diagnostics;
using System.Text.Json;
using Amazon;
using ssptb.pe.tdlt.transaction.common.Secrets;

namespace ssptb.pe.tdlt.transaction.secretsmanager.Services;
internal class SecretManagerService : ISecretManagerService
{
    private readonly IOptions<SecretManagerSettings> _settings;
    private readonly AmazonSecretsManagerClient _client;
    private readonly ILogger<SecretManagerService> _logger;

    public SecretManagerService(IOptions<SecretManagerSettings> settings, ILogger<SecretManagerService> logger)
    {
        _settings = settings;
        _client = new AmazonSecretsManagerClient(RegionEndpoint.GetBySystemName(_settings.Value.Region));
        _logger = logger;
    }

    public async Task<CouchBaseSecrets?> GetCouchBaseSecrets()
        => await GetSecret<CouchBaseSecrets>(_settings.Value.ArnCouchBaseSecrets);

    public async Task<MongoDbSecrets?> GetMongoDbSecrets()
        => await GetSecret<MongoDbSecrets>(_settings.Value.ArnMongoDBSecrets);

    private async Task<T?> GetSecret<T>(string arn) where T : ISecret
    {
        T? result = default;
        Stopwatch stopwatch = new();
        stopwatch.Start();

        _logger.LogInformation("Obteniendo valores de secret manager con Arn {arn}", arn);

        try
        {
            GetSecretValueResponse response = await _client.GetSecretValueAsync(new GetSecretValueRequest { SecretId = arn });
            result = JsonSerializer.Deserialize<T>(response.SecretString);
            stopwatch.Stop();

            _logger.LogInformation("Valores obtenidos de Arn {arn} satisfactorios, Duración ms : {ElapsedMilliseconds}",
                arn, stopwatch.ElapsedMilliseconds);
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            _logger.LogError("Error al obtener valores de secret manager con Arn {arn}, Duración ms : {ElapsedMilliseconds}, Error : {Message}",
                arn, stopwatch.ElapsedMilliseconds, ex.Message);
        }

        return result;
    }
}
