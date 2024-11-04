using MediatR;
using ssptb.pe.tdlt.transaction.command.Transaction.Queries;
using ssptb.pe.tdlt.transaction.common.Responses;
using ssptb.pe.tdlt.transaction.data.Repositories;
using ssptb.pe.tdlt.transaction.dto.Transaction;
using System.Text.Json;

namespace ssptb.pe.tdlt.transaction.commandhandler.Transaction;
public class GetAllTransactionsQueryHandler : IRequestHandler<GetAllTransactionsQuery, ApiResponse<List<TransactionAllResponseDto>>>
{
    private readonly ITransactionRepository _transactionRepository;

    public GetAllTransactionsQueryHandler(ITransactionRepository transactionRepository)
    {
        _transactionRepository = transactionRepository;
    }

    public async Task<ApiResponse<List<TransactionAllResponseDto>>> Handle(GetAllTransactionsQuery request, CancellationToken cancellationToken)
    {
        var transactions = await _transactionRepository.GetAllTransactionsAsync();

        if (transactions == null || transactions.Data == null || transactions.Data.Count == 0)
        {
            return ApiResponseHelper.CreateErrorResponse<List<TransactionAllResponseDto>>("No transactions found.", 404);
        }

        var responseDto = new List<TransactionAllResponseDto>();

        // Iterar sobre cada transacción y mapearla al DTO
        foreach (var transaction in transactions.Data)
        {
            var dto = new TransactionAllResponseDto
            {
                Id = transaction.Id,
                UserBankTransactionId = transaction.UserBankTransactionId,
                TransactionDate = transaction.TransactionDate,
                Status = transaction.Status,
                BlockId = transaction.BlockId,
                TransactionData = transaction.TransactionData.HasValue && transaction.TransactionData.Value.ValueKind != JsonValueKind.Undefined
                    ? transaction.TransactionData.Value.GetRawText() // Convertir el JsonElement a string
                    : string.Empty,
                StorageUrl = transaction.StorageUrl ?? string.Empty
            };

            responseDto.Add(dto);
        }

        return ApiResponseHelper.CreateSuccessResponse(responseDto, "Transactions retrieved successfully.");
    }
}
