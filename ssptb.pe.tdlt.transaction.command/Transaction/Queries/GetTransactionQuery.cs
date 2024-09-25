using MediatR;
using ssptb.pe.tdlt.transaction.dto.Transaction;

namespace ssptb.pe.tdlt.transaction.command.Transaction.Queries;
public class GetTransactionQuery : IRequest<TransactionResponseDto>
{
    public Guid TransactionId { get; }

    public GetTransactionQuery(Guid transactionId)
    {
        TransactionId = transactionId;
    }
}
