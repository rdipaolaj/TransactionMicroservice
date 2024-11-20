using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ssptb.pe.tdlt.transaction.common.Responses;
using ssptb.pe.tdlt.transaction.common.Settings;
using ssptb.pe.tdlt.transaction.common.Validations;
using ssptb.pe.tdlt.transaction.dto.Blockchain;
using ssptb.pe.tdlt.transaction.entities;
using ssptb.pe.tdlt.transaction.internalservices.Base;
using System.Net.Http.Json;

namespace ssptb.pe.tdlt.transaction.internalservices.Blockchain;
internal class BlockchainService : IBlockchainService
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

    private readonly ILogger<BlockchainService> _logger;

    public BlockchainService(IBaseService baseService, IHttpClientFactory httpClientFactory, IOptions<ApiSettings> options, ILogger<BlockchainService> logger)
    {
        _baseService = baseService;
        _httpClientFactory = httpClientFactory;
        _settings = options;
        _logger = logger;
    }

    public async Task<ApiResponse<RegisterTransactionDto>> RegisterTransactionAsync(Transaction transaction)
    {
        using HttpClient httpClient = _httpClientFactory.CreateClient("CustomClient");
        string path = GetRegisterTransactionPath();

        httpClient.BaseAddress = new Uri(_settings.Value.UrlMsBlockchain);

        HttpResponseMessage httpResponse = await _baseService.PostAsJsonAsync(httpClient, path, transaction);

        if (!CommonHttpValidation.ValidHttpResponse(httpResponse))
        {
            var errorContent = await httpResponse.Content.ReadAsStringAsync();
            _logger.LogError("Error al registrar la transacción en Blockchain Service");
            _logger.LogError("Respuesta HTTP inválida: {StatusCode}, Contenido: {Content}", httpResponse.StatusCode, errorContent);
            return ApiResponseHelper.CreateErrorResponse<RegisterTransactionDto>("Error al registrar la transacción en Blockchain Service");
        }

        var apiResult = await httpResponse.Content.ReadFromJsonAsync<ApiResponse<RegisterTransactionDto>>();

        return apiResult;
    }

    public async Task<ApiResponse<RegisterTransactionDto>> RegisterTransactionTestAsync(Transaction transaction)
    {
        using HttpClient httpClient = _httpClientFactory.CreateClient("CustomClient");
        string path = GetRegisterTransactionTestPath();

        httpClient.BaseAddress = new Uri(_settings.Value.UrlMsBlockchain);

        HttpResponseMessage httpResponse = await _baseService.PostAsJsonAsync(httpClient, path, transaction);

        if (!CommonHttpValidation.ValidHttpResponse(httpResponse))
        {
            var errorContent = await httpResponse.Content.ReadAsStringAsync();
            _logger.LogError("Error al registrar la transacción TEST en Blockchain Service");
            _logger.LogError("Respuesta HTTP inválida: {StatusCode}, Contenido: {Content}", httpResponse.StatusCode, errorContent);
            return ApiResponseHelper.CreateErrorResponse<RegisterTransactionDto>("Error al registrar la transacción TEST en Blockchain Service");
        }

        var apiResult = await httpResponse.Content.ReadFromJsonAsync<ApiResponse<RegisterTransactionDto>>();

        return apiResult;
    }

    public async Task<ApiResponse<NodeInfoDto>> GetNodeInfoAsync()
    {
        using HttpClient httpClient = _httpClientFactory.CreateClient("CustomClient");
        string path = GetNodeInfoPath();
        httpClient.BaseAddress = new Uri(_settings.Value.UrlMsBlockchain);
        HttpResponseMessage httpResponse = await _baseService.GetAsync(httpClient, path);

        if (!CommonHttpValidation.ValidHttpResponse(httpResponse))
        {
            var errorContent = await httpResponse.Content.ReadAsStringAsync();
            _logger.LogError("Error al obtener la información del nodo de Blockchain Service");
            _logger.LogError("Respuesta HTTP inválida: {StatusCode}, Contenido: {Content}", httpResponse.StatusCode, errorContent);
            return ApiResponseHelper.CreateErrorResponse<NodeInfoDto>("Error al obtener la información del nodo de Blockchain Service");
        }

        var apiResult = await httpResponse.Content.ReadFromJsonAsync<ApiResponse<NodeInfoDto>>();

        return apiResult;
    }

    public async Task<ApiResponse<TransactionBlockDto>> GetTransactionByBlockIdAsync(string blockId)
    {
        using HttpClient httpClient = _httpClientFactory.CreateClient("CustomClient");
        string path = GetTransactionByBlockIdPath(blockId);
        httpClient.BaseAddress = new Uri(_settings.Value.UrlMsBlockchain);

        HttpResponseMessage httpResponse = await _baseService.GetAsync(httpClient, path);

        if (!CommonHttpValidation.ValidHttpResponse(httpResponse))
        {
            var errorContent = await httpResponse.Content.ReadAsStringAsync();
            _logger.LogError("Error al obtener la transacción por BlockId en Blockchain Service");
            _logger.LogError("Respuesta HTTP inválida: {StatusCode}, Contenido: {Content}", httpResponse.StatusCode, errorContent);
            return ApiResponseHelper.CreateErrorResponse<TransactionBlockDto>("Error al obtener la transacción por BlockId en Blockchain Service");
        }

        // Leer la respuesta como JSON
        var apiResult = await httpResponse.Content.ReadFromJsonAsync<ApiResponse<TransactionBlockDto>>();
        if (apiResult == null)
        {
            _logger.LogError("No se pudo deserializar la respuesta de la API Blockchain.");
            return ApiResponseHelper.CreateErrorResponse<TransactionBlockDto>("No se pudo procesar la respuesta del servicio Blockchain.");
        }

        // Verificar si la API retornó éxito
        if (!apiResult.Success)
        {
            _logger.LogError("La API Blockchain devolvió un error: {Message}", apiResult.Message);
            return ApiResponseHelper.CreateErrorResponse<TransactionBlockDto>(apiResult.Message, apiResult.StatusCode, apiResult.Errors);
        }

        _logger.LogInformation("Transacción recuperada exitosamente para BlockId: {BlockId}", blockId);
        return apiResult;
    }

    #region Private methods

    private static string GetRegisterTransactionPath()
    {
        return "blockchain/register-transaction";
    }

    private static string GetRegisterTransactionTestPath()
    {
        return "blockchain/register-test-transaction";
    }

    private static string GetNodeInfoPath()
    {
        return "blockchain/node-info";
    }

    private static string GetTransactionByBlockIdPath(string blockId)
    {
        return $"blockchain/retrieve-transaction?blockId={blockId}";
    }

    #endregion
}
