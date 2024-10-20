using MapsterMapper;
using MediatR;
using ssptb.pe.tdlt.transaction.command.Transaction;
using ssptb.pe.tdlt.transaction.entities.Enums;
using ssptb.pe.tdlt.transaction.data.Repositories;
using ssptb.pe.tdlt.transaction.internalservices.Blockchain;
using Microsoft.Extensions.Logging;
using ssptb.pe.tdlt.transaction.common.Responses;
using ssptb.pe.tdlt.transaction.dto.Transaction;

namespace ssptb.pe.tdlt.transaction.commandhandler.Transaction;
public class CreateTransactionCommandHandler : IRequestHandler<CreateTransactionCommand, ApiResponse<TransactionResponseDto>>
{
    private readonly ILogger<CreateTransactionCommandHandler> _logger;
    private readonly ITransactionRepository _transactionRepository;
    private readonly IBlockchainService _blockchainService;
    private readonly IMapper _mapper;

    public CreateTransactionCommandHandler(ILogger<CreateTransactionCommandHandler> logger, ITransactionRepository transactionRepository, IBlockchainService blockchainService, IMapper mapper)
    {
        _logger = logger;
        _transactionRepository = transactionRepository;
        _blockchainService = blockchainService;
        _mapper = mapper;
    }

    public async Task<ApiResponse<TransactionResponseDto>> Handle(CreateTransactionCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Creando transacción...");

        if (request.Transaction == null)
        {
            _logger.LogError("El objeto Transaction en el comando está nulo.");
            return ApiResponseHelper.CreateErrorResponse<TransactionResponseDto>("El objeto Transaction no puede ser nulo.", 400);
        }

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
