using MediatR;
using ssptb.pe.tdlt.transaction.common.Responses;
using ssptb.pe.tdlt.transaction.dto.Metrics;

namespace ssptb.pe.tdlt.transaction.command.Metrics.Queries;
public class GetTransactionsPerHourQuery : IRequest<ApiResponse<TransactionsPerHourResponseDto>>
{
    public Guid UserId { get; }
    public Guid RoleId { get; }

    public GetTransactionsPerHourQuery(Guid userId, Guid roleId)
    {
        UserId = userId;
        RoleId = roleId;
    }
}
