using MediatR;
using ssptb.pe.tdlt.transaction.common.Responses;
using ssptb.pe.tdlt.transaction.dto.Transaction;

namespace ssptb.pe.tdlt.transaction.command.Transaction;
public class CreateTransactionCommand : IRequest<ApiResponse<TransactionResponseDto>>
{
    public TransactionRequestDto Transaction { get; }

    public CreateTransactionCommand(TransactionRequestDto transaction)
    {
        Transaction = transaction;
    }
}