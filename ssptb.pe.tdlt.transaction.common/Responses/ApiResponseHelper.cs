namespace ssptb.pe.tdlt.transaction.common.Responses;
public static class ApiResponseHelper
{
    /// <summary>
    /// Crea una respuesta exitosa con datos.
    /// </summary>
    public static ApiResponse<T> CreateSuccessResponse<T>(T data, string message = "Operation completed successfully.")
    {
        return new ApiResponse<T>
        {
            Success = true,
            StatusCode = 200, // Código de estado para OK
            Message = message,
            Data = data
        };
    }

    /// <summary>
    /// Crea una respuesta exitosa sin datos.
    /// </summary>
    public static ApiResponse<string> CreateSuccessResponse(string message = "Operation completed successfully.")
    {
        return new ApiResponse<string>
        {
            Success = true,
            StatusCode = 200, // Código de estado para OK
            Message = message,
            Data = string.Empty
        };
    }

    /// <summary>
    /// Crea una respuesta de error genérico con un tipo genérico.
    /// </summary>
    public static ApiResponse<T> CreateErrorResponse<T>(string message, int statusCode = 500, List<ErrorDetail>? errors = null)
    {
        return new ApiResponse<T>
        {
            Success = false,
            StatusCode = statusCode, // Código de estado para error interno del servidor
            Message = message,
            Data = default!, // Data será null o su valor por defecto
            Errors = errors ?? new List<ErrorDetail>()
        };
    }

    /// <summary>
    /// Crea una respuesta de error específico con detalles de error.
    /// </summary>
    public static ApiResponse<string> CreateErrorResponse(string message, int statusCode, params ErrorDetail[] errorDetails)
    {
        return new ApiResponse<string>
        {
            Success = false,
            StatusCode = statusCode,
            Message = message,
            Data = string.Empty,
            Errors = errorDetails.ToList()
        };
    }

    /// <summary>
    /// Crea una respuesta personalizada.
    /// </summary>
    public static ApiResponse<T> CreateCustomResponse<T>(bool success, int statusCode, string message, T data, List<ErrorDetail>? errors = null, Dictionary<string, object>? metadata = null)
    {
        return new ApiResponse<T>
        {
            Success = success,
            StatusCode = statusCode,
            Message = message,
            Data = data,
            Errors = errors ?? new List<ErrorDetail>(),
            Metadata = metadata ?? new Dictionary<string, object>()
        };
    }

    /// <summary>
    /// Crea una respuesta de validación de entrada fallida.
    /// </summary>
    public static ApiResponse<string> CreateValidationErrorResponse(List<ErrorDetail> validationErrors, string message = "Validation failed.")
    {
        return new ApiResponse<string>
        {
            Success = false,
            StatusCode = 400, // Código de estado para Bad Request
            Message = message,
            Data = string.Empty,
            Errors = validationErrors
        };
    }
}