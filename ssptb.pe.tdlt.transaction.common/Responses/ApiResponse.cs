using ssptb.pe.tdlt.transaction.common.Helpers;

namespace ssptb.pe.tdlt.transaction.common.Responses;
public class ApiResponse<T>
{
    /// <summary>
    /// Indica si la solicitud fue exitosa.
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// Código de estado HTTP de la respuesta.
    /// </summary>
    public int StatusCode { get; set; }

    /// <summary>
    /// Mensaje informativo o de error relacionado con la solicitud.
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// Datos devueltos por la solicitud, si corresponde.
    /// </summary>
    public T Data { get; set; } = default!;

    /// <summary>
    /// Identificador único de la transacción o solicitud.
    /// </summary>
    public string TransactionId { get; set; } = Guid.NewGuid().ToString();

    /// <summary>
    /// Timestamp que indica cuándo se generó la respuesta.
    /// </summary>
    public DateTime Timestamp { get; set; } = DatetimeHelper.Now();

    /// <summary>
    /// Lista de errores detallados, si existen.
    /// </summary>
    public List<ErrorDetail> Errors { get; set; } = new List<ErrorDetail>();

    /// <summary>
    /// Metadatos adicionales que pueden ser relevantes para la respuesta.
    /// </summary>
    public Dictionary<string, object> Metadata { get; set; } = new Dictionary<string, object>();

    public ApiResponse() { }

    public ApiResponse(
        bool success, int statusCode, string? message = null,
        T? data = default, string? transactionId = null,
        List<ErrorDetail>? errors = null,
        Dictionary<string, object>? metadata = null)
    {
        Success = success;
        StatusCode = statusCode;
        Message = message ?? string.Empty;
        Data = data ?? default!;
        TransactionId = transactionId ?? Guid.NewGuid().ToString();
        Timestamp = DatetimeHelper.Now();
        Errors = errors ?? new List<ErrorDetail>();
        Metadata = metadata ?? new Dictionary<string, object>();
    }
}

public class ErrorDetail
{
    public string Code { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}