using MapsterMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using ssptb.pe.tdlt.transaction.command.Transaction;
using ssptb.pe.tdlt.transaction.common.Responses;
using ssptb.pe.tdlt.transaction.data.Repositories;
using ssptb.pe.tdlt.transaction.dto.Storage;
using ssptb.pe.tdlt.transaction.dto.Transaction;
using ssptb.pe.tdlt.transaction.dto.User;
using ssptb.pe.tdlt.transaction.entities.Enums;
using ssptb.pe.tdlt.transaction.internalservices.Blockchain;
using ssptb.pe.tdlt.transaction.internalservices.Storage;
using ssptb.pe.tdlt.transaction.internalservices.User;

namespace ssptb.pe.tdlt.transaction.commandhandler.Transaction;

public class CreateTransactionTestCommandHandler : IRequestHandler<CreateTransactionTestCommand, ApiResponse<TransactionTestResponseDto>>
{
    private readonly ILogger<CreateTransactionTestCommandHandler> _logger;
    private readonly ITransactionRepository _transactionRepository;
    private readonly IBlockchainService _blockchainService;
    private readonly IStorageService _storageService;
    private readonly IUserDataService _userDataService;
    private readonly IMapper _mapper;

    public CreateTransactionTestCommandHandler(
        ILogger<CreateTransactionTestCommandHandler> logger,
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

    public async Task<ApiResponse<TransactionTestResponseDto>> Handle(CreateTransactionTestCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Creando transacción...");

        if (request.TransactionTest == null)
        {
            _logger.LogError("El objeto TransactionTest en el comando está nulo.");
            return ApiResponseHelper.CreateErrorResponse<TransactionTestResponseDto>("El objeto TransactionTest no puede ser nulo.", 400);
        }

        var userCheckRequest = new GetUserByIdRequestDto
        {
            UserId = Guid.Parse(request.TransactionTest.UserBankTransactionId)
        };

        var userCheckResponse = await _userDataService.GetUserDataClientById(userCheckRequest);

        if (!userCheckResponse.Success || userCheckResponse.Data == null)
        {
            _logger.LogError("El usuario con ID {UserId} no existe en el servicio de clientes.", request.TransactionTest.UserBankTransactionId);
            return ApiResponseHelper.CreateErrorResponse<TransactionTestResponseDto>("El usuario no existe en la base de datos.", 404);
        }

        _logger.LogInformation("Usuario con ID {UserId} verificado correctamente.", request.TransactionTest.UserBankTransactionId);

        var transaction = _mapper.Map<entities.Transaction>(request.TransactionTest);
        transaction.Id = Guid.NewGuid();
        transaction.Status = TransactionStatus.Pending;

        await _transactionRepository.SaveTransactionAsync(transaction);

        // Enviar al microservicio de Blockchain
        var response = await _blockchainService.RegisterTransactionTestAsync(transaction);

        if (response.Success && response.Data != null)
        {
            // Obtener el BlockId y actualizar la transacción
            string blockId = response.Data.BlockId;
            transaction.Status = TransactionStatus.SentToBlockchain;
            transaction.BlockId = blockId;

            // Guardar nuevamente la transacción con el BlockId
            await _transactionRepository.SaveTransactionAsync(transaction);

            _logger.LogInformation("Transacción registrada en la blockchain TEST con BlockId: {BlockId}", blockId);

            // Integración con el servicio de almacenamiento
            var storageRequest = new UploadJsonRequestDto
            {
                FileName = $"transaction_{request.TransactionTest.Tag}",
                JsonContent = request.TransactionTest.TransactionData.GetRawText(),
                UserId = transaction.Id.ToString(),
            };

            var storageResponse = await _storageService.FileUploadAsync(storageRequest);

            if (!storageResponse.Success)
            {
                _logger.LogError("Error al almacenar el JSON TEST en el servicio de almacenamiento.");

                // Marcar como fallido y actualizar en la base de datos
                transaction.Status = TransactionStatus.Failed;
                await _transactionRepository.SaveTransactionAsync(transaction);

                return ApiResponseHelper.CreateErrorResponse<TransactionTestResponseDto>("Error al almacenar el JSON TEST en el servicio de almacenamiento.", 500);
            }

            _logger.LogInformation("Archivo JSON TEST almacenado exitosamente en el servicio de almacenamiento.");
            transaction.StorageUrl = storageResponse.Data.PublicUrl;
            await _transactionRepository.SaveTransactionAsync(transaction);

            // Crear el DTO de respuesta
            var responseDto = new TransactionTestResponseDto
            {
                TransactionId = transaction.Id,
                BlockId = blockId
            };

            // Retornar ApiResponse de éxito con el ID de la transacción y el BlockId
            return ApiResponseHelper.CreateSuccessResponse(responseDto, "Transacción TEST registrada exitosamente.");
        }
        else
        {
            // Si la transacción falló, actualizar su estado a 'Failed'
            transaction.Status = TransactionStatus.Failed;
            await _transactionRepository.SaveTransactionAsync(transaction);

            _logger.LogError("Error al registrar la transacción TEST en la blockchain: {Error}", response.Message);

            // Retornar ApiResponse de error
            return ApiResponseHelper.CreateErrorResponse<TransactionTestResponseDto>("Error al registrar la transacción TEST en la blockchain", 400, response.Errors);
        }

    }
}
