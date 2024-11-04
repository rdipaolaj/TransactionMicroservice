using MediatR;
using ssptb.pe.tdlt.transaction.common.Responses;
using ssptb.pe.tdlt.transaction.dto.Metrics;

namespace ssptb.pe.tdlt.transaction.command.Metrics.Queries;
public class GetSuccessErrorRatioQuery : IRequest<ApiResponse<SuccessErrorRatioResponseDto>>
{
    public Guid UserId { get; }
    public Guid RoleId { get; }

    public GetSuccessErrorRatioQuery(Guid userId, Guid roleId)
    {
        UserId = userId;
        RoleId = roleId;
    }
}
