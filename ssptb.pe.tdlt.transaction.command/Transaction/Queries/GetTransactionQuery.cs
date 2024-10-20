using MediatR;
using ssptb.pe.tdlt.transaction.common.Responses;
using ssptb.pe.tdlt.transaction.dto.Transaction;

namespace ssptb.pe.tdlt.transaction.command.Transaction.Queries;
public class GetTransactionQuery : IRequest<ApiResponse<TransactionIdResponseDto>>
{
    public Guid TransactionId { get; }

    public GetTransactionQuery(Guid transactionId)
    {
        TransactionId = transactionId;
    }
}
