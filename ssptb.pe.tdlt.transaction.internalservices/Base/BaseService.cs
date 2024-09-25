using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Net.Http.Json;

namespace ssptb.pe.tdlt.transaction.internalservices.Base;

/// <summary>
/// Clase de métodos para solicitudes a servicios HTTP
/// </summary>
/// <param name="logger">Inyección depdendencias para el acceso a log</param>
internal class BaseService(ILogger<BaseService> logger) : IBaseService
{
    /// <summary>
    /// Llamada personalizada a métodos GET
    /// </summary>
    /// <param name="httpClient">Objeto <see cref="HttpClient"/> con la configuración necesaria para la solicitud HTTP</param>
    /// <param name="requestUri">Url de la solicitud</param>
    /// <returns>Retorna un <see cref="HttpResponseMessage"/></returns>
    public async Task<HttpResponseMessage> GetAsync(HttpClient httpClient, string requestUri)
    {
        string url = FormatUrl(httpClient, requestUri);
        return await ExecuteHttpRequest("GET", url, () => httpClient.GetAsync(requestUri));
    }

    /// <summary>
    /// Llamada personalizada a métodos POST donde se envía el cuerpo como Json
    /// </summary>
    /// <typeparam name="TValue">Tipo de objeto a enviar en el cuerpo de la solicitud</typeparam>
    /// <param name="httpClient">Objeto <see cref="HttpClient"/> con la configuración necesaria para la solicitud HTTP</param>
    /// <param name="requestUri">Url de la solicitud</param>
    /// <param name="value">Objeto a enviar en la solicitud HTTP</param>
    /// <returns>Retorna un <see cref="HttpResponseMessage"/></returns>
    public async Task<HttpResponseMessage> PostAsJsonAsync<TValue>(HttpClient httpClient, string requestUri, TValue value)
    {
        string url = FormatUrl(httpClient, requestUri);
        return await ExecuteHttpRequest("POST", url, () => httpClient.PostAsJsonAsync(requestUri, value));
    }

    /// <summary>
    /// Llamada personalizada a métodos POST donde se un envía un <see cref="HttpContent"/> como cuerpo de la solicitud
    /// </summary>
    /// <param name="httpClient">Objeto <see cref="HttpClient"/> con la configuración necesaria para la solicitud HTTP</param>
    /// <param name="requestUri">Url de la solicitud</param>
    /// <param name="value">Objeto a enviar en la solicitud HTTP</param>
    /// <returns>Retorna un <see cref="HttpResponseMessage"/></returns>
    public async Task<HttpResponseMessage> PostAsync(HttpClient httpClient, string requestUri, HttpContent value)
    {
        string url = FormatUrl(httpClient, requestUri);
        return await ExecuteHttpRequest("POST", url, () => httpClient.PostAsync(requestUri, value));
    }

    #region Private methods

    /// <summary>
    /// Método para envío de la función HTTP a ejecutar la solicitud HTTP
    /// </summary>
    /// <param name="httpMethod">Etiqueta del tipo HTTP que se va ejecutar para el tracing de la solicitud</param>
    /// <param name="requestUri">Url de la solicitud</param>
    /// <param name="httpRequestFunc">Funcion a ejecutar para el retorno de un <see cref="HttpResponseMessage"/></param>
    /// <returns>Retorna un <see cref="HttpResponseMessage"/></returns>
    public async Task<HttpResponseMessage> ExecuteHttpRequest(string httpMethod, string requestUri, Func<Task<HttpResponseMessage>> httpRequestFunc)
    {
        HttpResponseMessage httpResponse;

        logger.LogInformation("Iniciando request {httpMethod} : {requestUri}", httpMethod, requestUri);

        Stopwatch stopwatch = new();
        stopwatch.Start();

        try
        {
            httpResponse = await httpRequestFunc();
            stopwatch.Stop();

            logger.LogInformation("Request {httpMethod}: {requestUri}, Código {HttpStatusCode} - {StatusCode}, Duración {ElapsedMilliseconds} ms",
                httpMethod, requestUri, (int)httpResponse.StatusCode, httpResponse.StatusCode, stopwatch.ElapsedMilliseconds);
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            logger.LogError("Error Request {httpMethod} {requestUri}, Duración {ElapsedMilliseconds} ms, Excepción : {Message}", httpMethod, requestUri, stopwatch.ElapsedMilliseconds, ex.Message);
            throw;
        }

        return httpResponse;
    }

    /// <summary>
    /// Método privado para formatear la Url que servirá para el log de la solicitud HTTP
    /// </summary>
    /// <param name="httpClient">Objeto <see cref="HttpClient"/> con la configuración necesaria para la solicitud HTTP</param>
    /// <param name="requestUri">Url de la solicitud</param>
    /// <returns></returns>
    private static string FormatUrl(HttpClient httpClient, string requestUri)
        => httpClient.BaseAddress is null ? requestUri : $"{httpClient.BaseAddress}{requestUri}";

    #endregion
}