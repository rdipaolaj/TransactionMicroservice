using MediatR;
using ssptb.pe.tdlt.transaction.common.Responses;
using ssptb.pe.tdlt.transaction.dto.Audit;

namespace ssptb.pe.tdlt.transaction.command.Audit.Command;

/// <summary>
/// Comando para iniciar una auditoría completa.
/// </summary>
public class RunFullAuditCommand : IRequest<ApiResponse<AuditResultResponseDto>>
{
    public Guid UserId { get; }
    public Guid RoleId { get; }
    public RunFullAuditCommand(Guid userId, Guid roleId)
    {
        UserId = userId;
        RoleId = roleId;
    }
}