using MediatR;
using ssptb.pe.tdlt.transaction.common.Responses;
using ssptb.pe.tdlt.transaction.dto.Transaction;

namespace ssptb.pe.tdlt.transaction.command.Transaction.Queries;
public class GetAllTransactionsQuery : IRequest<ApiResponse<List<TransactionAllResponseDto>>>
{
}
