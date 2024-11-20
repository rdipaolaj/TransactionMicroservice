using MediatR;
using ssptb.pe.tdlt.transaction.common.Responses;
using ssptb.pe.tdlt.transaction.dto.Transaction;

namespace ssptb.pe.tdlt.transaction.command.Transaction;

public class CreateTransactionTestCommand : IRequest<ApiResponse<TransactionTestResponseDto>>
{
    public TransactionTestRequestDto TransactionTest { get; }

    public CreateTransactionTestCommand(TransactionTestRequestDto transactionTest)
    {
        TransactionTest = transactionTest;
    }
}
