using Couchbase.Extensions.DependencyInjection;
using Couchbase.KeyValue;
using Microsoft.Extensions.Logging;
using ssptb.pe.tdlt.transaction.entities;

namespace ssptb.pe.tdlt.transaction.data.Repositories;
public class TransactionRepository : ITransactionRepository
{
    private readonly ILogger<TransactionRepository> _logger;
    private readonly ICouchbaseCollection _collection;

    public TransactionRepository(INamedBucketProvider bucketProvider, ILogger<TransactionRepository> logger)
    {
        _logger = logger;
        var bucket = bucketProvider.GetBucketAsync().Result;
        _collection = bucket.DefaultCollection();
    }

    public async Task SaveTransactionAsync(Transaction transaction)
    {
        try
        {
            await _collection.UpsertAsync(transaction.Id.ToString(), transaction);
            _logger.LogInformation($"Transaction {transaction.Id} saved successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error saving transaction: {ex.Message}");
            throw;
        }
    }

    public async Task<Transaction> GetTransactionAsync(Guid id)
    {
        try
        {
            var result = await _collection.GetAsync(id.ToString());
            return result.ContentAs<Transaction>();
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error retrieving transaction: {ex.Message}");
            throw;
        }
    }
}
