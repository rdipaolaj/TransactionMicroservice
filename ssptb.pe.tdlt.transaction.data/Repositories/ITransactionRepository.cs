using ssptb.pe.tdlt.transaction.common.Responses;
using ssptb.pe.tdlt.transaction.entities;

namespace ssptb.pe.tdlt.transaction.data.Repositories;
public interface ITransactionRepository
{
    Task<ApiResponse<bool>> SaveTransactionAsync(Transaction transaction);
    Task<ApiResponse<Transaction>> GetTransactionAsync(Guid id);

    Task<ApiResponse<List<Transaction>>> GetAllTransactionsAsync();
    // Otros métodos según tus necesidades...
}
