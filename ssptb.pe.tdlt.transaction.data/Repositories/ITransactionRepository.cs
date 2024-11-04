using ssptb.pe.tdlt.transaction.common.Responses;
using ssptb.pe.tdlt.transaction.entities;

namespace ssptb.pe.tdlt.transaction.data.Repositories;
public interface ITransactionRepository
{
    Task<ApiResponse<bool>> SaveTransactionAsync(Transaction transaction);
    Task<ApiResponse<Transaction>> GetTransactionAsync(Guid id);

    Task<ApiResponse<List<Transaction>>> GetAllTransactionsAsync();

    Task<int> GetTotalTransactionsAsync(Guid userId, Guid roleId);
    Task<double> CalculateMonthlyPercentageChangeAsync(Guid userId, Guid roleId);
    Task<Dictionary<int, int>> GetTransactionsPerHourAsync(Guid userId, Guid roleId, DateTime date);
    Task<List<KeyValuePair<DateTime, int>>> GetCumulativeTransactionTrendAsync(Guid userId, Guid roleId, DateTime startDate, DateTime endDate);
    Task<(int successCount, int errorCount)> GetSuccessErrorCountAsync(Guid userId, Guid roleId);
    Task<int> GetTransactionCountByMonthAsync(Guid userId, Guid roleId, DateTime referenceDate);
    // Otros métodos según tus necesidades...
}
