using ssptb.pe.tdlt.transaction.common.Responses;
using ssptb.pe.tdlt.transaction.dto.User;

namespace ssptb.pe.tdlt.transaction.internalservices.User;

/// <summary>
/// Interfaz de ms servicio de user data
/// </summary>
public interface IUserDataService
{
    /// <summary>
    /// Llamada a servicio para obtener la información del cliente en base a su id
    /// </summary>
    /// <param name="request">Request para consulta de cliente</param>
    /// <returns>Información de cliente en base a su id</returns>
    Task<ApiResponse<GetUserByIdResponseDto>> GetUserDataClientById(GetUserByIdRequestDto request);
}
