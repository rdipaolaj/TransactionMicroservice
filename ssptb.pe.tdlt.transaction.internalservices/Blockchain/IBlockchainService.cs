using ssptb.pe.tdlt.transaction.common.Responses;
using ssptb.pe.tdlt.transaction.dto.Blockchain;
using ssptb.pe.tdlt.transaction.entities;

namespace ssptb.pe.tdlt.transaction.internalservices.Blockchain;
public interface IBlockchainService
{
    Task<ApiResponse<RegisterTransactionDto>> RegisterTransactionAsync(Transaction transaction);
    Task<ApiResponse<NodeInfoDto>> GetNodeInfoAsync();
    // Otros métodos si los necesitas...
}
