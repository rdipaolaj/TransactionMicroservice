namespace ssptb.pe.tdlt.transaction.redis.Services;

/// <summary>
/// Interfaz para manejo de servicio redis
/// </summary>
public interface IRedisService
{
    /// <summary>
    /// Obtener información de redis en base a una llave única
    /// </summary>
    /// <param name="key">Llave única para acceso al valor</param>
    /// <returns>Valor registrado en redis</returns>
    public Task<string> GetInformationAsync(string key);

    /// <summary>
    /// Obtener información de redis en base a una llave única
    /// </summary>
    /// <param name="key">Llave única para acceso al valor</param>
    /// <returns>Valor registrado en redis</returns>
    public string GetInformation(string key);

    /// <summary>
    /// Guardar información en redis
    /// </summary>
    /// <param name="key">Llave única para guardar el valor</param>
    /// <param name="value">Valor a guardar</param>
    /// <param name="expiration">Tipo de expiración de valor en redis</param>
    /// <returns>Resultado del registro de la información</returns>
    public Task<bool> SaveInformationAsJsonAsync(string key, object value, TimeSpan expiration);

    /// <summary>
    /// Guardar información en redis
    /// </summary>
    /// <param name="key">Llave única para guardar el valor</param>
    /// <param name="value">Valor a guardar</param>
    /// <param name="expiration">Tipo de expiración de valor en redis</param>
    /// <returns>Resultado del registro de la información</returns>
    public Task<bool> SaveInformationAsync(string key, string value, TimeSpan expiration);

    /// <summary>
    /// Guardar información en redis
    /// </summary>
    /// <param name="key">Llave única para guardar el valor</param>
    /// <param name="value">Valor a guardar</param>
    /// <param name="expiration">Tipo de expiración de valor en redis</param>
    /// <returns>Resultado del registro de la información</returns>
    public bool SaveInformation(string key, string value, TimeSpan expiration);

    public bool DeleteInformation(string key);

    /// <summary>
    /// Eliminar información de redis en base a una llave única de manera asíncrona
    /// </summary>
    /// <param name="key">Llave única para eliminar el valor</param>
    /// <returns>Resultado de la eliminación de la información</returns>
    public Task<bool> DeleteInformationAsync(string key);

    /// <summary>
    /// Obtener información deserializada desde redis
    /// </summary>
    /// <typeparam name="T">Tipo de objeto esperado</typeparam>
    /// <param name="key">Llave única para acceso al valor</param>
    /// <returns>Objeto deserializado desde redis</returns>
    public Task<T?> GetInformationAsync<T>(string key);

    public Task<bool> SaveHashAsync(string key, Dictionary<string, string> hashEntries, TimeSpan expiration);

    public Task<Dictionary<string, string>> GetHashAsync(string key);
}
