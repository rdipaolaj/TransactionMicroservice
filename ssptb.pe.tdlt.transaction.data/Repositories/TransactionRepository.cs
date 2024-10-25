using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using ssptb.pe.tdlt.transaction.common.Responses;
using ssptb.pe.tdlt.transaction.data.Helpers;
using ssptb.pe.tdlt.transaction.entities;
using ssptb.pe.tdlt.transaction.entities.Enums;
using System.Text.Json;

namespace ssptb.pe.tdlt.transaction.data.Repositories;
public class TransactionRepository : ITransactionRepository
{
    private readonly ICouchbaseHelper _couchbaseHelper;
    private readonly ILogger<TransactionRepository> _logger;

    public TransactionRepository(ICouchbaseHelper couchbaseHelper, ILogger<TransactionRepository> logger)
    {
        _couchbaseHelper = couchbaseHelper;
        _logger = logger;
    }

    public async Task<ApiResponse<bool>> SaveTransactionAsync(Transaction transaction)
    {
        try
        {
            // Usa CouchbaseHelper para obtener la collection
            var collection = await _couchbaseHelper.GetCollectionAsync("transaction_app", "transaction_data");

            // Asigna la versión de esquema actual al documento
            transaction.SchemaVersion = 1;

            // Inserta o actualiza los metadatos en la collection
            await collection.UpsertAsync(transaction.Id.ToString(), transaction);
            _logger.LogInformation($"Transaction {transaction.Id} saved successfully.");

            return ApiResponseHelper.CreateSuccessResponse(true, "Transaction saved successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error saving transaction to Couchbase: {ex.Message}");
            return ApiResponseHelper.CreateErrorResponse<bool>($"Failed to save transaction: {ex.Message}", 500);
        }
    }

    public async Task<ApiResponse<Transaction>> GetTransactionAsync(Guid id)
    {
        try
        {
            var collection = await _couchbaseHelper.GetCollectionAsync("transaction_app", "transaction_data");
            var result = await collection.GetAsync(id.ToString());
            var transaction = result.ContentAs<Transaction>();

            if (!string.IsNullOrEmpty(transaction.TransactionDataSave))
            {
                transaction.TransactionData = JsonDocument.Parse(transaction.TransactionDataSave).RootElement;
            }

            _logger.LogInformation($"Transaction with ID {id} retrieved from Couchbase.");

            return ApiResponseHelper.CreateSuccessResponse(transaction, "Transaction retrieved successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error retrieving transaction data from Couchbase: {ex.Message}");
            return ApiResponseHelper.CreateErrorResponse<Transaction>($"Failed to retrieve transaction data: {ex.Message}", 500);
        }
    }

    public async Task<ApiResponse<List<Transaction>>> GetAllTransactionsAsync()
    {
        try
        {
            // Definir la consulta N1QL para obtener todas las transacciones
            var query = "SELECT t.* FROM `travel-sample`.`transaction_app`.`transaction_data` AS t " +
                    "WHERE t.schemaVersion = 1";

            // Obtener el cluster usando CouchbaseHelper
            var cluster = await _couchbaseHelper.GetClusterAsync();
            var result = await cluster.QueryAsync<JObject>(query); // Utilizamos JObject para un manejo explícito de JSON

            var transactions = new List<Transaction>();

            await foreach (var row in result.Rows)
            {
                var transaction = new Transaction();

                // Validar ID
                if (!TryParseGuid(row["id"], out var parsedGuid))
                {
                    _logger.LogWarning($"Invalid GUID format for transaction ID: {row["id"]}");
                    continue;
                }
                transaction.Id = parsedGuid;

                // Asignar propiedades de la transacción
                transaction.UserBankTransactionId = row["userBankTransactionId"]?.ToString() ?? string.Empty;
                transaction.Tag = row["tag"]?.ToString() ?? string.Empty;
                transaction.TransactionDate = ParseDate(row["transactionDate"]);
                transaction.Status = ParseEnum<TransactionStatus>(row["status"], TransactionStatus.Pending);
                transaction.SchemaVersion = row["schemaVersion"] != null ? (int)row["schemaVersion"] : 1;
                transaction.BlockId = row["blockId"]?.ToString();

                // Manejar el campo 'transactionDataSave'
                if (row["transactionDataSave"] != null)
                {
                    try
                    {
                        transaction.TransactionDataSave = row["transactionDataSave"].ToString();
                        transaction.TransactionData = JsonDocument.Parse(transaction.TransactionDataSave).RootElement;
                    }
                    catch (JsonException ex)
                    {
                        _logger.LogWarning($"Error parsing TransactionDataSave: {ex.Message}, setting TransactionData to null.");
                        transaction.TransactionData = null;
                    }
                }

                transactions.Add(transaction);
            }

            _logger.LogInformation($"All transactions retrieved from Couchbase.");

            return ApiResponseHelper.CreateSuccessResponse(transactions, "Transactions retrieved successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error retrieving transactions data from Couchbase: {ex.Message}");
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
