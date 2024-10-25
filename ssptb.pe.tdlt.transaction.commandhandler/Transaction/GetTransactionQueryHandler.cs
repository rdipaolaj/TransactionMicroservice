using MapsterMapper;
using MediatR;
using ssptb.pe.tdlt.transaction.command.Transaction.Queries;
using ssptb.pe.tdlt.transaction.common.Responses;
using ssptb.pe.tdlt.transaction.data.Repositories;
using ssptb.pe.tdlt.transaction.dto.Transaction;
using System.Text.Json;

namespace ssptb.pe.tdlt.transaction.commandhandler.Transaction;
public class GetTransactionQueryHandler : IRequestHandler<GetTransactionQuery, ApiResponse<TransactionIdResponseDto>>
{
    private readonly ITransactionRepository _transactionRepository;
    private readonly IMapper _mapper;

    public GetTransactionQueryHandler(ITransactionRepository transactionRepository, IMapper mapper)
    {
        _transactionRepository = transactionRepository;
        _mapper = mapper;
    }

    public async Task<ApiResponse<TransactionIdResponseDto>> Handle(GetTransactionQuery request, CancellationToken cancellationToken)
    {
        // Recuperamos la transacción
        var transactionResponse = await _transactionRepository.GetTransactionAsync(request.TransactionId);

        if (transactionResponse == null || transactionResponse.Data == null)
        {
            // Utilizamos el helper para crear la respuesta de error
            return ApiResponseHelper.CreateErrorResponse<TransactionIdResponseDto>("Transaction not found.", 404);
        }

        // Creamos un DTO y asignamos los valores recuperados
        var responseDto = new TransactionIdResponseDto
        {
            Id = transactionResponse.Data.Id,
            UserBankTransactionId = transactionResponse.Data.UserBankTransactionId,
            TransactionDate = transactionResponse.Data.TransactionDate,
            Status = transactionResponse.Data.Status,
            BlockId = transactionResponse.Data.BlockId,
            TransactionData = transactionResponse.Data.TransactionData.HasValue && transactionResponse.Data.TransactionData.Value.ValueKind != JsonValueKind.Undefined
                ? transactionResponse.Data.TransactionData.Value.GetRawText()
                : string.Empty
        };

        // Utilizamos el helper para crear una respuesta exitosa con el DTO
        return ApiResponseHelper.CreateSuccessResponse(responseDto, "Transaction retrieved successfully.");
    }
}
