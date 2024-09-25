namespace ssptb.pe.tdlt.transaction.internalservices.Base;

/// <summary>
/// Clase de métodos para solicitudes a servicios HTTP
/// </summary>
internal interface IBaseService
{
    /// <summary>
    /// Llamada personalizada a métodos GET
    /// </summary>
    /// <param name="httpClient">Objeto <see cref="HttpClient"/> con la configuración necesaria para la solicitud HTTP</param>
    /// <param name="requestUri">Url de la solicitud</param>
    /// <returns>Retorna un <see cref="HttpResponseMessage"/></returns>
    Task<HttpResponseMessage> GetAsync(HttpClient httpClient, string requestUri);

    /// <summary>
    /// Llamada personalizada a métodos POST donde se envía el cuerpo como Json
    /// </summary>
    /// <typeparam name="TValue">Tipo de objeto a enviar en el cuerpo de la solicitud</typeparam>
    /// <param name="httpClient">Objeto <see cref="HttpClient"/> con la configuración necesaria para la solicitud HTTP</param>
    /// <param name="requestUri">Url de la solicitud</param>
    /// <param name="value">Objeto a enviar en la solicitud HTTP</param>
    /// <returns>Retorna un <see cref="HttpResponseMessage"/></returns>
    Task<HttpResponseMessage> PostAsJsonAsync<TValue>(HttpClient httpClient, string requestUri, TValue value);

    /// <summary>
    /// Llamada personalizada a métodos POST donde se un envía un <see cref="HttpContent"/> como cuerpo de la solicitud
    /// </summary>
    /// <param name="httpClient">Objeto <see cref="HttpClient"/> con la configuración necesaria para la solicitud HTTP</param>
    /// <param name="requestUri">Url de la solicitud</param>
    /// <param name="value">Objeto a enviar en la solicitud HTTP</param>
    /// <returns>Retorna un <see cref="HttpResponseMessage"/></returns>
    Task<HttpResponseMessage> PostAsync(HttpClient httpClient, string requestUri, HttpContent value);
}
