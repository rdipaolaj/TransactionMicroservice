using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ssptb.pe.tdlt.transaction.common.Responses;
using ssptb.pe.tdlt.transaction.common.Settings;
using ssptb.pe.tdlt.transaction.common.Validations;
using ssptb.pe.tdlt.transaction.dto.User;
using ssptb.pe.tdlt.transaction.internalservices.Base;
using System.Net.Http.Json;

namespace ssptb.pe.tdlt.transaction.internalservices.User;
internal class UserDataService(IHttpClientFactory httpClientFactory, IOptions<ApiSettings> settings, ILogger<UserDataService> logger, IBaseService baseService) : IUserDataService
{
    /// <summary>
    /// Intefaz de HttpClientFactory para el uso de HttpClients
    /// </summary>
    private readonly IHttpClientFactory _httpClientFactory = httpClientFactory;

    /// <summary>
    /// Settings para obtener Urls
    /// </summary>
    private readonly IOptions<ApiSettings> _settings = settings;

    private readonly ILogger<UserDataService> _logger = logger;

    private readonly IBaseService _baseService = baseService;

    public async Task<ApiResponse<GetUserByIdResponseDto>> GetUserDataClientById(GetUserByIdRequestDto request)
    {
        if (string.IsNullOrEmpty(request.UserId.ToString()))
            return ApiResponseHelper.CreateErrorResponse<GetUserByIdResponseDto>("Invalid userId");

        using HttpClient httpClient = _httpClientFactory.CreateClient("CustomClient");
        string path = GetUserByIdPath(request.UserId);

        httpClient.BaseAddress = new Uri(_settings.Value.UrlMsUser);

        HttpResponseMessage httpResponse = await _baseService.GetAsync(httpClient, path);

        if (!CommonHttpValidation.ValidHttpResponse(httpResponse))
        {
            var errorContent = await httpResponse.Content.ReadAsStringAsync();
            _logger.LogError("Error al consumir el servicio de obtener clientes en base al id");
            _logger.LogError("Respuesta HTTP inválida: {StatusCode}, Contenido: {Content}", httpResponse.StatusCode, errorContent);
            return ApiResponseHelper.CreateErrorResponse<GetUserByIdResponseDto>("Error al consumir el servicio de la consulta del cliente en base al id");
        }

        var apiResult = await httpResponse.Content.ReadFromJsonAsync<ApiResponse<GetUserByIdResponseDto>>();

        return apiResult;
    }

    #region Private methods

    private static string GetUserByIdPath(Guid UserId)
    {
        return $"ssptbpetdlt/user/api/v1/User/{UserId}";
    }

    #endregion
}
