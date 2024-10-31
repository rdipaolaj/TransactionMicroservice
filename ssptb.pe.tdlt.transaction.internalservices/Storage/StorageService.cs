using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ssptb.pe.tdlt.transaction.common.Responses;
using ssptb.pe.tdlt.transaction.common.Settings;
using ssptb.pe.tdlt.transaction.common.Validations;
using ssptb.pe.tdlt.transaction.dto.Storage;
using ssptb.pe.tdlt.transaction.internalservices.Base;
using System.Net.Http.Json;

namespace ssptb.pe.tdlt.transaction.internalservices.Storage;
internal class StorageService : IStorageService
{
    private readonly IBaseService _baseService;

    /// <summary>
    /// Intefaz de HttpClientFactory para el uso de HttpClients
    /// </summary>
    private readonly IHttpClientFactory _httpClientFactory;

    /// <summary>
    /// Settings para obtener Urls
    /// </summary>
    private readonly IOptions<ApiSettings> _settings;

    private readonly ILogger<StorageService> _logger;

    public StorageService(IBaseService baseService, IHttpClientFactory httpClientFactory, IOptions<ApiSettings> options, ILogger<StorageService> logger)
    {
        _baseService = baseService;
        _httpClientFactory = httpClientFactory;
        _settings = options;
        _logger = logger;
    }
    public async Task<ApiResponse<FileUploadResponseDto>> FileUploadAsync(UploadJsonRequestDto request)
    {
        _logger.LogInformation("Iniciando carga de archivo JSON para el usuario con ID: {RequestId}", request.UserId);

        using HttpClient httpClient = _httpClientFactory.CreateClient("CustomClient");
        string path = GetFileUploadPath();
        httpClient.BaseAddress = new Uri(_settings.Value.UrlMsStorage);
        HttpResponseMessage httpResponse = await _baseService.PostAsJsonAsync(httpClient, path, request);

        if (!CommonHttpValidation.ValidHttpResponse(httpResponse))
        {
            var errorContent = await httpResponse.Content.ReadAsStringAsync();
            _logger.LogError("Error al cargar el archivo. Respuesta HTTP inválida: {StatusCode}, Contenido: {Content}", httpResponse.StatusCode, errorContent);
            return ApiResponseHelper.CreateErrorResponse<FileUploadResponseDto>("Error al cargar el archivo en Storage Service");
        }

        _logger.LogInformation("La carga de archivo fue exitosa, procesando respuesta...");

        var apiResult = await httpResponse.Content.ReadFromJsonAsync<ApiResponse<FileUploadResponseDto>>();

        if (apiResult == null || apiResult.Data == null || !apiResult.Success)
        {
            _logger.LogError("La respuesta del servicio Storage no fue exitosa. Detalles: {ErrorMessage}", apiResult?.Message ?? "No disponible");
            return ApiResponseHelper.CreateErrorResponse<FileUploadResponseDto>("Error en la respuesta del servicio Storage.");
        }

        _logger.LogInformation("Archivo cargado exitosamente con ID: {FileId}", apiResult.Data?.FileId);

        return apiResult;
    }

    #region Private methods

    private static string GetFileUploadPath()
    {
        return "ssptbpetdlt/storage/api/v1/Storage/upload-json";
    }

    #endregion
}
