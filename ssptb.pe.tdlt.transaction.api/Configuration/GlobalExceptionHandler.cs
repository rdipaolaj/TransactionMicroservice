using FluentValidation;
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
        else if (exception is ValidationException validationException)
        {
            response = HandleValidationException(validationException, out statusCode);
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
        var errors = new List<ErrorDetail>();

        if (exception.ErrorCode == ApiErrorCode.ValidationError && exception.InnerException is ValidationException validationException)
        {
            errors = validationException.Errors.Select(e => new ErrorDetail
            {
                Code = e.ErrorCode,
                Description = e.ErrorMessage
            }).ToList();

            _logger.LogWarning("ValidationException occurred: {ErrorCode} - {Message}", exception.ErrorCode, exception.Message);
        }
        else
        {
            errors.Add(new ErrorDetail
            {
                Code = exception.ErrorCode.ToString(),
                Description = exception.Message ?? "A custom error occurred."
            });
            _logger.LogWarning("CustomException occurred: {ErrorCode} - {Message}", exception.ErrorCode, exception.Message);
        }

        var apiResponse = ApiResponseHelper.CreateErrorResponse(
            message: exception.Message ?? "An error occurred.",
            statusCode: statusCode,
            errors.ToArray()
        );

        return apiResponse;
    }

    private ApiResponse<string> HandleValidationException(ValidationException exception, out int statusCode)
    {
        statusCode = StatusCodes.Status400BadRequest;
        var errors = exception.Errors.Select(e => new ErrorDetail
        {
            Code = e.ErrorCode,
            Description = e.ErrorMessage
        }).ToArray();

        var apiResponse = ApiResponseHelper.CreateErrorResponse(
            message: "Validation failed.",
            statusCode: statusCode,
            errors
        );

        _logger.LogWarning("ValidationException occurred: {Message}", exception.Message);

        return apiResponse;
    }

    private ApiResponse<string> HandleGeneralException(Exception exception, out int statusCode)
    {
        statusCode = StatusCodes.Status500InternalServerError;
        
        _logger.LogError(exception, "Exception occurred: {Message}, StackTrace: {StackTrace}", exception.Message, exception.StackTrace);

        var apiResponse = ApiResponseHelper.CreateErrorResponse(
            message: "Ocurrió un error inesperado.",
            statusCode: statusCode,
            new ErrorDetail { Code = ApiErrorCode.UnknownError.ToString(), Description = "Ocurrió un error inesperado." }
        );

        _logger.LogError(exception, "Exception occurred: {Message}", exception.Message);

        return apiResponse;
    }
}
