using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using ssptb.pe.tdlt.transaction.api.Configuration;
using ssptb.pe.tdlt.transaction.command.Audit.Command;

namespace ssptb.pe.tdlt.transaction.api.Controllers;

[ApiVersion(1)]
[ApiController]
[Route("ssptbpetdlt/transaction/api/v{v:apiVersion}/[controller]")]
public class AuditController : CustomController
{
    private readonly ILogger<AuditController> _logger;
    private readonly IMediator _mediator;

    public AuditController(IMediator mediator, ILogger<AuditController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Ejecuta una auditoría completa de las transacciones almacenadas en MongoDB contra la Tangle.
    /// </summary>
    /// <returns>Resultados de la auditoría.</returns>
    [HttpPost("full-audit")]
    public async Task<IActionResult> RunFullAudit(Guid userId, Guid roleId)
    {
        _logger.LogInformation("Inicio del proceso de auditoría...");
        var result = await _mediator.Send(new RunFullAuditCommand(userId, roleId));
        return OkorBadRequestValidationApiResponse(result);
    }
}
