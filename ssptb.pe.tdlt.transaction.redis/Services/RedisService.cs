using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ssptb.pe.tdlt.transaction.common.Settings;
using StackExchange.Redis;
using System.Diagnostics;
using System.Text.Json;

namespace ssptb.pe.tdlt.transaction.redis.Services;

/// <summary>
/// Clase para manejo de servicio redis
/// </summary>
internal class RedisService : IRedisService
{
    /// <summary>
    /// Interfaz para acceso a la base de datos de redis
    /// </summary>
    private readonly IDatabase _database;

    private readonly ILogger<RedisService> _logger;

    /// <summary>
    /// Constructor de servicio de acceso a Redis
    /// </summary>
    /// <param name="_settigs">Settings de configuracion de Redis</param>
    public RedisService(IOptions<RedisSettings> _settigs, ILogger<RedisService> logger)
    {
        _logger = logger;

        var config = new ConfigurationOptions
        {
            AbortOnConnectFail = false,
            EndPoints = { _settigs.Value.Endpoint ?? string.Empty },
            Ssl = !_settigs.Value.Local,
            AllowAdmin = true,
        };

        _logger.LogInformation("Configuración Redis => EndPoint: {Endpoint}, Ssl: {Ssl}", _settigs.Value.Endpoint, config.Ssl);

        Stopwatch stopwatch = new();
        stopwatch.Start();

        var lazyConnection = new Lazy<ConnectionMultiplexer>(() => ConnectionMultiplexer.Connect(config));
        ConnectionMultiplexer connection = lazyConnection.Value;
        _database = connection.GetDatabase();

        stopwatch.Stop();

        _logger.LogInformation("Configuración Redis terminada => Estado : {IsConnected}, Duración ms : {ElapsedMilliseconds}", connection.IsConnected, stopwatch.ElapsedMilliseconds);
    }

    public bool DeleteInformation(string key) => _database.KeyDelete(key);

    public async Task<bool> DeleteInformationAsync(string key) => await _database.KeyDeleteAsync(key);

    /// <summary>
    /// Obtener información de redis en base a una llave única
    /// </summary>
    /// <param name="key">Llave única para acceso al valor</param>
    /// <returns>Valor registrado en redis</returns>
    public string GetInformation(string key) => _database.StringGet(key);

    /// <summary>
    /// Obtener información de redis en base a una llave única
    /// </summary>
    /// <param name="key">Llave única para acceso al valor</param>
    /// <returns>Valor registrado en redis</returns>
    public async Task<string> GetInformationAsync(string key) => await _database.StringGetAsync(key);

    public async Task<T?> GetInformationAsync<T>(string key)
    {
        var json = await _database.StringGetAsync(key);
        return json.HasValue ? JsonSerializer.Deserialize<T>(json) : default;
    }

    /// <summary>
    /// Guardar información en redis
    /// </summary>
    /// <param name="key">Llave única para guardar el valor</param>
    /// <param name="value">Valor a guardar</param>
    /// <param name="expiration">Tipo de expiración de valor en redis</param>
    /// <returns>Resultado del registro de la información</returns>
    public bool SaveInformation(string key, string value, TimeSpan expiration) => _database.StringSet(key, value, expiration);

    /// <summary>
    /// Guardar información en redis
    /// </summary>
    /// <param name="key">Llave única para guardar el valor</param>
    /// <param name="value">Valor a guardar</param>
    /// <param name="expiration">Tipo de expiración de valor en redis</param>
    /// <returns>Resultado del registro de la información</returns>
    public async Task<bool> SaveInformationAsJsonAsync(string key, object value, TimeSpan expiration)
    {
        string jsonData = JsonSerializer.Serialize(value);

        bool result = await _database.StringSetAsync(key, jsonData, expiration);

        return result;
    }

    public async Task<bool> SaveInformationAsync(string key, string value, TimeSpan expiration) => await _database.StringSetAsync(key, value, expiration);

    public async Task<bool> SaveHashAsync(string key, Dictionary<string, string> hashEntries, TimeSpan expiration)
    {
        var entries = hashEntries.Select(kv => new HashEntry(kv.Key, kv.Value)).ToArray();
        await _database.HashSetAsync(key, entries);
        return await _database.KeyExpireAsync(key, expiration);
    }

    public async Task<Dictionary<string, string>> GetHashAsync(string key)
    {
        var hashEntries = await _database.HashGetAllAsync(key);
        return hashEntries.ToDictionary(entry => entry.Name.ToString(), entry => entry.Value.ToString());
    }
}
