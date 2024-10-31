using MediatR;
using ssptb.pe.tdlt.transaction.common.Responses;
using ssptb.pe.tdlt.transaction.dto.Transaction;

namespace ssptb.pe.tdlt.transaction.command.Transaction.Queries;
public class GetTransactionsByIdAndRoleIdQuery : IRequest<ApiResponse<List<TransactionAllResponseDto>>>
{
    public Guid UserId { get; }
    public Guid RoleId { get; }

    public GetTransactionsByIdAndRoleIdQuery(Guid userId, Guid roleId)
    {
        UserId = userId;
        RoleId = roleId;
    }
}
