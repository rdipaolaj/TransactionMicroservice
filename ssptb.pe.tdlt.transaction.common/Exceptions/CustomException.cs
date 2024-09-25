using ssptb.pe.tdlt.transaction.common.Enums;

namespace ssptb.pe.tdlt.transaction.common.Exceptions;
public class CustomException : Exception
{
    public ApiErrorCode ErrorCode { get; }

    public CustomException() : this("An error occurred.", ApiErrorCode.UnknownError) { }

    public CustomException(string? message) : this(message, ApiErrorCode.UnknownError) { }

    public CustomException(string? message, ApiErrorCode errorCode) : base(message)
    {
        ErrorCode = errorCode;
    }

    public CustomException(string? message, Exception? innerException) : this(message, ApiErrorCode.UnknownError, innerException) { }

    public CustomException(string? message, ApiErrorCode errorCode, Exception? innerException) : base(message, innerException)
    {
        ErrorCode = errorCode;
    }
}
