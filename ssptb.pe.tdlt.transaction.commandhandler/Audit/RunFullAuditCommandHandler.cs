using MediatR;
using Microsoft.Extensions.Logging;
using ssptb.pe.tdlt.transaction.command.Audit.Command;
using ssptb.pe.tdlt.transaction.common.Responses;
using ssptb.pe.tdlt.transaction.data.Repositories;
using ssptb.pe.tdlt.transaction.dto.Audit;
using ssptb.pe.tdlt.transaction.internalservices.Blockchain;
using ssptb.pe.tdlt.transaction.internalservices.Helpers;
using ssptb.pe.tdlt.transaction.internalservices.Storage;

namespace ssptb.pe.tdlt.transaction.commandhandler.Audit;

/// <summary>
/// Handler para procesar la auditoría completa.
/// </summary>
public class RunFullAuditCommandHandler : IRequestHandler<RunFullAuditCommand, ApiResponse<AuditResultResponseDto>>
{
    private readonly ILogger<RunFullAuditCommandHandler> _logger;
    private readonly ITransactionRepository _transactionRepository;
    private readonly IBlockchainService _blockchainService;

    public RunFullAuditCommandHandler(
        ILogger<RunFullAuditCommandHandler> logger,
        ITransactionRepository transactionRepository,
        IBlockchainService blockchainService)
    {
        _logger = logger;
        _transactionRepository = transactionRepository;
        _blockchainService = blockchainService;
    }

    public async Task<ApiResponse<AuditResultResponseDto>> Handle(RunFullAuditCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Iniciando auditoría completa...");

        var discrepancies = new List<AuditDiscrepancyDto>();
        var transactions = await _transactionRepository.GetTransactionsAsync(request.UserId, request.RoleId);

        foreach (var transaction in transactions)
        {
            try
            {
                var tangleResponse = await _blockchainService.GetTransactionByBlockIdAsync(transaction.BlockId);

                if (tangleResponse == null || !tangleResponse.Success)
                {
                    discrepancies.Add(new AuditDiscrepancyDto
                    {
                        BlockId = transaction.BlockId,
                        Reason = "Transacción no encontrada en la Tangle"
                    });
                    continue;
                }

                if (transaction.TransactionDataSave != tangleResponse.Data?.TransactionData?.ToString())
                {
                    discrepancies.Add(new AuditDiscrepancyDto
                    {
                        BlockId = transaction.BlockId,
                        Reason = "Datos no coinciden",
                        LocalData = transaction.TransactionDataSave,
                        TangleData = tangleResponse.Data?.TransactionData?.ToString() ?? string.Empty
                    });
                }
            }
            catch (Exception ex)
            {
                discrepancies.Add(new AuditDiscrepancyDto
                {
                    BlockId = transaction.BlockId,
                    Reason = "Error al consultar la Tangle",
                    Error = ex.Message
                });
            }
        }

        // Generar archivo Excel en memoria
        _logger.LogInformation("Generando reporte de auditoría en Excel...");
        var excelData = ExcelHelper.GenerateAuditReportExcel(discrepancies);

        _logger.LogInformation("Reporte de auditoría generado exitosamente.");

        var result = new AuditResultResponseDto
        {
            TotalTransactions = transactions.Count,
            Discrepancies = discrepancies,
            TotalDiscrepancies = discrepancies.Count,
            ExcelFileData = excelData // Agregar el archivo Excel al resultado
        };

        return ApiResponseHelper.CreateSuccessResponse(result, "Auditoría completada con reporte en Excel.");
    }
}
