namespace ssptb.pe.tdlt.transaction.common.Enums;
public enum ApiErrorCode
{
    // Códigos de éxito
    Success = 0,

    // Códigos de error genéricos
    UnknownError = 1000,
    ValidationError = 1001,
    Unauthorized = 1002,
    Forbidden = 1003,
    NotFound = 1004,
    Conflict = 1005,
    InternalServerError = 1006,

    // Códigos de error específicos
    CustomError = 2000,
    DatabaseError = 2001,
    NetworkError = 2002,

    // Códigos adicionales para posibles escenarios
    Timeout = 2003,
    DependencyFailure = 2004
}