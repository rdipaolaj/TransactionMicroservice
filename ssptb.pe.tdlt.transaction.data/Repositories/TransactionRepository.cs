using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using Newtonsoft.Json.Linq;
using ssptb.pe.tdlt.transaction.common.Responses;
using ssptb.pe.tdlt.transaction.data.Helpers;
using ssptb.pe.tdlt.transaction.entities;
using ssptb.pe.tdlt.transaction.entities.Enums;
using System.Text.Json;

namespace ssptb.pe.tdlt.transaction.data.Repositories;
public class TransactionRepository : ITransactionRepository
{
    private readonly IMongoDBHelper _mongoDBHelper;
    private readonly ILogger<TransactionRepository> _logger;

    public TransactionRepository(IMongoDBHelper mongoDBHelper, ILogger<TransactionRepository> logger)
    {
        _mongoDBHelper = mongoDBHelper;
        _logger = logger;
    }

    public async Task<ApiResponse<bool>> SaveTransactionAsync(Transaction transaction)
    {
        try
        {
            // Usa MongoDBHelper para obtener la colección
            var collection = _mongoDBHelper.GetCollection<Transaction>("transaction_data");

            // Asigna la versión de esquema actual al documento
            transaction.SchemaVersion = 1;

            // Inserta o actualiza el documento en la colección
            var filter = Builders<Transaction>.Filter.Eq(t => t.Id, transaction.Id);
            var options = new ReplaceOptions { IsUpsert = true };
            await collection.ReplaceOneAsync(filter, transaction, options);

            _logger.LogInformation($"Transaction {transaction.Id} saved successfully.");

            return ApiResponseHelper.CreateSuccessResponse(true, "Transaction saved successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error saving transaction to MongoDB: {ex.Message}");
            return ApiResponseHelper.CreateErrorResponse<bool>($"Failed to save transaction: {ex.Message}", 500);
        }
    }

    public async Task<ApiResponse<Transaction>> GetTransactionAsync(Guid id)
    {
        try
        {
            var collection = _mongoDBHelper.GetCollection<Transaction>("transaction_data");
            var filter = Builders<Transaction>.Filter.Eq(t => t.Id, id);
            var result = await collection.Find(filter).FirstOrDefaultAsync();

            if (result == null)
            {
                return ApiResponseHelper.CreateErrorResponse<Transaction>("Transaction not found.", 404);
            }

            if (!string.IsNullOrEmpty(result.TransactionDataSave))
            {
                result.TransactionData = JsonDocument.Parse(result.TransactionDataSave).RootElement;
            }

            _logger.LogInformation($"Transaction with ID {id} retrieved from MongoDB.");

            return ApiResponseHelper.CreateSuccessResponse(result, "Transaction retrieved successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error retrieving transaction data from MongoDB: {ex.Message}");
            return ApiResponseHelper.CreateErrorResponse<Transaction>($"Failed to retrieve transaction data: {ex.Message}", 500);
        }
    }

    public async Task<ApiResponse<List<Transaction>>> GetAllTransactionsAsync()
    {
        try
        {
            var collection = _mongoDBHelper.GetCollection<Transaction>("transaction_data");
            var filter = Builders<Transaction>.Filter.Eq(t => t.SchemaVersion, 1);
            var result = await collection.Find(filter).ToListAsync();

            var transactions = new List<Transaction>();

            foreach (var transaction in result)
            {
                if (!string.IsNullOrEmpty(transaction.TransactionDataSave))
                {
                    try
                    {
                        transaction.TransactionData = JsonDocument.Parse(transaction.TransactionDataSave).RootElement;
                    }
                    catch (JsonException ex)
                    {
                        _logger.LogWarning($"Error parsing TransactionDataSave for transaction {transaction.Id}: {ex.Message}, setting TransactionData to null.");
                        transaction.TransactionData = null;
                    }
                }

                transactions.Add(transaction);
            }

            _logger.LogInformation("All transactions retrieved from MongoDB.");

            return ApiResponseHelper.CreateSuccessResponse(transactions, "Transactions retrieved successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error retrieving transactions data from MongoDB: {ex.Message}");
            return ApiResponseHelper.CreateErrorResponse<List<Transaction>>($"Failed to retrieve transactions data: {ex.Message}", 500);
        }
    }

    // Métodos auxiliares
    private bool TryParseGuid(JToken token, out Guid guid)
    {
        guid = Guid.Empty;
        return token != null && Guid.TryParse(token.ToString(), out guid);
    }

    private DateTime ParseDate(JToken token)
    {
        if (token == null) return DateTime.MinValue;
        return DateTimeOffset.TryParse(token.ToString(), out var dto) ? dto.DateTime : DateTime.Parse(token.ToString());
    }

    private TEnum ParseEnum<TEnum>(JToken token, TEnum defaultValue) where TEnum : struct
    {
        if (token == null) return defaultValue;
        return Enum.TryParse(token.ToString(), out TEnum result) ? result : defaultValue;
    }
}
