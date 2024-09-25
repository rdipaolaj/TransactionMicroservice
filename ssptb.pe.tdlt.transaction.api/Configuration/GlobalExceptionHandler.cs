using Microsoft.AspNetCore.Diagnostics;
using ssptb.pe.tdlt.transaction.common.Enums;
using ssptb.pe.tdlt.transaction.common.Exceptions;
using ssptb.pe.tdlt.transaction.common.Responses;

namespace ssptb.pe.tdlt.transaction.api.Configuration;

internal sealed class GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger = logger;

    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        ApiResponse<string> response;
        int statusCode;

        if (exception is CustomException customException)
        {
            response = HandleCustomException(customException, out statusCode);
        }
        else
        {
            response = HandleGeneralException(exception, out statusCode);
        }

        httpContext.Response.StatusCode = statusCode;
        await httpContext.Response.WriteAsJsonAsync(response, cancellationToken);

        return true;
    }

    private ApiResponse<string> HandleCustomException(CustomException exception, out int statusCode)
    {
        statusCode = StatusCodes.Status400BadRequest;

        var apiResponse = ApiResponseHelper.CreateErrorResponse(
            message: exception.Message ?? "An error occurred.",
            statusCode: statusCode,
            new ErrorDetail
            {
                Code = exception.ErrorCode.ToString(), // Se usa el enum como cadena
                Description = exception.Message ?? "A custom error occurred."
            }
        );

        _logger.LogWarning("CustomException occurred: {ErrorCode} - {Message}", exception.ErrorCode, exception.Message);

        return apiResponse;
    }

    private ApiResponse<string> HandleGeneralException(Exception exception, out int statusCode)
    {
        statusCode = StatusCodes.Status500InternalServerError;
        var apiResponse = ApiResponseHelper.CreateErrorResponse(
            message: "Ocurrió un error inesperado.",
            statusCode: statusCode,
            new ErrorDetail { Code = ApiErrorCode.UnknownError.ToString(), Description = "Ocurrió un error inesperado." }
        );

        _logger.LogError(exception, "Exception occurred: {Message}", exception.Message);

        return apiResponse;
    }
}
