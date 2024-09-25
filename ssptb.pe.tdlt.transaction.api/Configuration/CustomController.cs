using Microsoft.AspNetCore.Mvc;
using ssptb.pe.tdlt.transaction.common.Responses;

namespace ssptb.pe.tdlt.transaction.api.Configuration;

public class CustomController : ControllerBase
{
    /// <summary>
    /// Método para devolver un Ok o BadRequest en base a la evaluación del resultado de un api response
    /// </summary>
    /// <typeparam name="T">Tipo de dato de api response</typeparam>
    /// <param name="apiResponse">Objeto ApiResponse</param>
    /// <returns>Retorna IActionResult (Ok o BadRequest)</returns>
    private protected IActionResult OkorBadRequestValidationApiResponse<T>(ApiResponse<T> apiResponse)
        => apiResponse.Success ? Ok(apiResponse) : BadRequest(apiResponse);
}

