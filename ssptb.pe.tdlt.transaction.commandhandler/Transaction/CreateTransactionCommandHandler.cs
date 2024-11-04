using MapsterMapper;
using MediatR;
using ssptb.pe.tdlt.transaction.command.Transaction;
using ssptb.pe.tdlt.transaction.entities.Enums;
using ssptb.pe.tdlt.transaction.data.Repositories;
using ssptb.pe.tdlt.transaction.internalservices.Blockchain;
using Microsoft.Extensions.Logging;
using ssptb.pe.tdlt.transaction.common.Responses;
using ssptb.pe.tdlt.transaction.dto.Transaction;
using ssptb.pe.tdlt.transaction.internalservices.Storage;
using ssptb.pe.tdlt.transaction.dto.Storage;
using ssptb.pe.tdlt.transaction.internalservices.User;
using ssptb.pe.tdlt.transaction.dto.User;

namespace ssptb.pe.tdlt.transaction.commandhandler.Transaction;
public class CreateTransactionCommandHandler : IRequestHandler<CreateTransactionCommand, ApiResponse<TransactionResponseDto>>
{
    private readonly ILogger<CreateTransactionCommandHandler> _logger;
    private readonly ITransactionRepository _transactionRepository;
    private readonly IBlockchainService _blockchainService;
    private readonly IStorageService _storageService;
    private readonly IUserDataService _userDataService;
    private readonly IMapper _mapper;

    public CreateTransactionCommandHandler(
        ILogger<CreateTransactionCommandHandler> logger,
        ITransactionRepository transactionRepository,
        IBlockchainService blockchainService,
        IStorageService storageService,
        IMapper mapper,
        IUserDataService userDataService)
    {
        _logger = logger;
        _transactionRepository = transactionRepository;
        _blockchainService = blockchainService;
        _storageService = storageService;
        _mapper = mapper;
        _userDataService = userDataService;
    }

    public async Task<ApiResponse<TransactionResponseDto>> Handle(CreateTransactionCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Creando transacción...");

        if (request.Transaction == null)
        {
            _logger.LogError("El objeto Transaction en el comando está nulo.");
            return ApiResponseHelper.CreateErrorResponse<TransactionResponseDto>("El objeto Transaction no puede ser nulo.", 400);
        }

        var userCheckRequest = new GetUserByIdRequestDto
        {
            UserId = Guid.Parse(request.Transaction.UserBankTransactionId)
        };

        var userCheckResponse = await _userDataService.GetUserDataClientById(userCheckRequest);

        if (!userCheckResponse.Success || userCheckResponse.Data == null)
        {
            _logger.LogError("El usuario con ID {UserId} no existe en el servicio de clientes.", request.Transaction.UserBankTransactionId);
            return ApiResponseHelper.CreateErrorResponse<TransactionResponseDto>("El usuario no existe en la base de datos.", 404);
        }

        _logger.LogInformation("Usuario con ID {UserId} verificado correctamente.", request.Transaction.UserBankTransactionId);

        var transaction = _mapper.Map<entities.Transaction>(request.Transaction);
        transaction.Id = Guid.NewGuid();
        transaction.Status = TransactionStatus.Pending;

        await _transactionRepository.SaveTransactionAsync(transaction);

        // Enviar al microservicio de Blockchain
        var response = await _blockchainService.RegisterTransactionAsync(transaction);

        if (response.Success && response.Data != null)
        {
            // Obtener el BlockId y actualizar la transacción
            string blockId = response.Data.BlockId;
            transaction.Status = TransactionStatus.SentToBlockchain;
            transaction.BlockId = blockId;

            // Guardar nuevamente la transacción con el BlockId
            await _transactionRepository.SaveTransactionAsync(transaction);

            _logger.LogInformation("Transacción registrada en la blockchain con BlockId: {BlockId}", blockId);

            // Integración con el servicio de almacenamiento
            var storageRequest = new UploadJsonRequestDto
            {
                FileName = $"transaction_{request.Transaction.Tag}",
                JsonContent = request.Transaction.TransactionData.GetRawText(),
                UserId = transaction.Id.ToString(),
            };

            var storageResponse = await _storageService.FileUploadAsync(storageRequest);

            if (!storageResponse.Success)
            {
                _logger.LogError("Error al almacenar el JSON en el servicio de almacenamiento.");

                // Marcar como fallido y actualizar en la base de datos
                transaction.Status = TransactionStatus.Failed;
                await _transactionRepository.SaveTransactionAsync(transaction);

                return ApiResponseHelper.CreateErrorResponse<TransactionResponseDto>("Error al almacenar el JSON en el servicio de almacenamiento.", 500);
            }

            _logger.LogInformation("Archivo JSON almacenado exitosamente en el servicio de almacenamiento.");
            transaction.StorageUrl = storageResponse.Data.PublicUrl;
            await _transactionRepository.SaveTransactionAsync(transaction);

            // Crear el DTO de respuesta
            var responseDto = new TransactionResponseDto
            {
                TransactionId = transaction.Id,
                BlockId = blockId
            };

            // Retornar ApiResponse de éxito con el ID de la transacción y el BlockId
            return ApiResponseHelper.CreateSuccessResponse(responseDto, "Transacción registrada exitosamente.");
        }
        else
        {
            // Si la transacción falló, actualizar su estado a 'Failed'
            transaction.Status = TransactionStatus.Failed;
            await _transactionRepository.SaveTransactionAsync(transaction);

            _logger.LogError("Error al registrar la transacción en la blockchain: {Error}", response.Message);

            // Retornar ApiResponse de error
            return ApiResponseHelper.CreateErrorResponse<TransactionResponseDto>("Error al registrar la transacción en la blockchain", 400, response.Errors);
        }

    }
}
