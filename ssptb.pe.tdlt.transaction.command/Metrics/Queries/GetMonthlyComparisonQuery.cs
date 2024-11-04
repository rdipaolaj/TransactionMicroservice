using MediatR;
using ssptb.pe.tdlt.transaction.common.Responses;
using ssptb.pe.tdlt.transaction.dto.Metrics;

namespace ssptb.pe.tdlt.transaction.command.Metrics.Queries;
public class GetMonthlyComparisonQuery : IRequest<ApiResponse<MonthlyComparisonResponseDto>>
{
    public Guid UserId { get; }
    public Guid RoleId { get; }

    public GetMonthlyComparisonQuery(Guid userId, Guid roleId)
    {
        UserId = userId;
        RoleId = roleId;
    }
}
