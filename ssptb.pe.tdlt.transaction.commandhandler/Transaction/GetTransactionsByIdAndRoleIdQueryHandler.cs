using MediatR;
using Microsoft.Extensions.Logging;
using ssptb.pe.tdlt.transaction.command.Transaction.Queries;
using ssptb.pe.tdlt.transaction.common.Responses;
using ssptb.pe.tdlt.transaction.data.Repositories;
using ssptb.pe.tdlt.transaction.dto.Transaction;
using ssptb.pe.tdlt.transaction.dto.User;
using ssptb.pe.tdlt.transaction.internalservices.User;
using System.Collections.Generic;
using System.Text.Json;

namespace ssptb.pe.tdlt.transaction.commandhandler.Transaction;
public class GetTransactionsByIdAndRoleIdQueryHandler : IRequestHandler<GetTransactionsByIdAndRoleIdQuery, ApiResponse<List<TransactionAllResponseDto>>>
{
    private readonly ILogger<GetTransactionsByIdAndRoleIdQueryHandler> _logger;
    private readonly ITransactionRepository _transactionRepository;
    private readonly IUserDataService _userDataService;

    // Lista de roles permitidos
    private readonly Guid _adminRoleId = Guid.Parse("b0069c1d-f7f7-408d-b32f-155d80bab376");
    private readonly Guid _companyRoleId = Guid.Parse("25f5393f-62ff-4f4a-95e7-656233239abd");
    private readonly Guid _userRoleId = Guid.Parse("edd62b2f-5976-4498-b4e6-c870b4e96b2a");


    public GetTransactionsByIdAndRoleIdQueryHandler(ILogger<GetTransactionsByIdAndRoleIdQueryHandler> logger, ITransactionRepository transactionRepository, IUserDataService userDataService)
    {
        _logger = logger;
        _transactionRepository = transactionRepository;
        _userDataService = userDataService;
    }

    public async Task<ApiResponse<List<TransactionAllResponseDto>>> Handle(GetTransactionsByIdAndRoleIdQuery request, CancellationToken cancellationToken)
    {
        var userCheckRequest = new GetUserByIdRequestDto
        {
            UserId = request.UserId
        };

        var userCheckResponse = await _userDataService.GetUserDataClientById(userCheckRequest);

        if (!userCheckResponse.Success || userCheckResponse.Data == null)
        {
            _logger.LogError("El usuario con ID {UserId} no existe en el servicio de clientes.", request.UserId);
            return ApiResponseHelper.CreateErrorResponse<List<TransactionAllResponseDto>>("El usuario no existe en la base de datos.", 404);
        }

        _logger.LogInformation("Usuario con ID {UserId} verificado correctamente.", request.UserId);

        // Validar si el RoleId es válido
        var roleId = request.RoleId;

        if (roleId != _adminRoleId && roleId != _companyRoleId && roleId != _userRoleId)
        {
            _logger.LogError("El RoleId proporcionado no es válido. RoleId: {RoleId}", roleId);
            return ApiResponseHelper.CreateErrorResponse<List<TransactionAllResponseDto>>("El RoleId proporcionado no es válido.", 400);
        }

        var transactions = await _transactionRepository.GetAllTransactionsAsync();

        if (transactions == null || transactions.Data == null || transactions.Data.Count == 0)
        {
            return ApiResponseHelper.CreateErrorResponse<List<TransactionAllResponseDto>>("No transactions found.", 404);
        }

        List<TransactionAllResponseDto> responseDto;

        // Lógica de filtrado según el RoleId
        if (roleId == _adminRoleId)  // Admin
        {
            // Si el rol es Admin, retornar todas las transacciones
            responseDto = transactions.Data.Select(transaction => MapToDto(transaction)).ToList();
        }
        else
        {
            // Si el rol es Company o User, filtrar por UserBankTransactionId
            responseDto = transactions.Data
                .Where(transaction => transaction.UserBankTransactionId == request.UserId.ToString())
                .Select(transaction => MapToDto(transaction))
                .ToList();
        }

        return ApiResponseHelper.CreateSuccessResponse(responseDto, "Transactions retrieved successfully.");
    }

    private TransactionAllResponseDto MapToDto(entities.Transaction transaction)
    {
        return new TransactionAllResponseDto
        {
            Id = transaction.Id,
            UserBankTransactionId = transaction.UserBankTransactionId,
            TransactionDate = transaction.TransactionDate,
            Status = transaction.Status,
            BlockId = transaction.BlockId,
            TransactionData = transaction.TransactionData.HasValue && transaction.TransactionData.Value.ValueKind != JsonValueKind.Undefined
                ? transaction.TransactionData.Value.GetRawText()
                : string.Empty
        };
    }

}
