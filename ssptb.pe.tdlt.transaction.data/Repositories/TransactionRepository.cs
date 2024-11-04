using Microsoft.Extensions.Logging;
using MongoDB.Bson;
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

    public async Task<int> GetTotalTransactionsAsync(Guid userId, Guid roleId)
    {
        var collection = _mongoDBHelper.GetCollection<Transaction>("transaction_data");

        // Si el roleId es Guid.Empty, obtenemos todas las transacciones (usuario administrador)
        FilterDefinition<Transaction> filter = roleId == Guid.Empty
            ? Builders<Transaction>.Filter.Empty
            : Builders<Transaction>.Filter.Eq(t => t.UserBankTransactionId, userId.ToString());

        var totalTransactions = await collection.CountDocumentsAsync(filter);
        _logger.LogInformation($"Total transactions calculated: {totalTransactions}");
        return (int)totalTransactions;
    }

    public async Task<double> CalculateMonthlyPercentageChangeAsync(Guid userId, Guid roleId)
    {
        var collection = _mongoDBHelper.GetCollection<Transaction>("transaction_data");

        // Filtrar las transacciones del usuario o todas si es Admin
        FilterDefinition<Transaction> filter = roleId == Guid.Empty
            ? Builders<Transaction>.Filter.Empty
            : Builders<Transaction>.Filter.Eq(t => t.UserBankTransactionId, userId.ToString());

        // Filtrar las transacciones de este mes y del mes pasado
        var currentDate = DateTime.UtcNow;
        var startOfCurrentMonth = new DateTime(currentDate.Year, currentDate.Month, 1);
        var startOfLastMonth = startOfCurrentMonth.AddMonths(-1);

        // Contar transacciones de este mes
        var currentMonthFilter = Builders<Transaction>.Filter.And(
            filter,
            Builders<Transaction>.Filter.Gte(t => t.TransactionDate, startOfCurrentMonth)
        );
        var currentMonthCount = await collection.CountDocumentsAsync(currentMonthFilter);

        // Contar transacciones del mes pasado
        var lastMonthFilter = Builders<Transaction>.Filter.And(
            filter,
            Builders<Transaction>.Filter.Gte(t => t.TransactionDate, startOfLastMonth),
            Builders<Transaction>.Filter.Lt(t => t.TransactionDate, startOfCurrentMonth)
        );
        var lastMonthCount = await collection.CountDocumentsAsync(lastMonthFilter);

        // Calcular el cambio porcentual con lógica mejorada
        double percentageChange;

        if (lastMonthCount == 0 && currentMonthCount > 0)
        {
            // Incremento completo (sin transacciones el mes pasado, pero hay transacciones este mes)
            percentageChange = 100;
        }
        else if (lastMonthCount > 0 && currentMonthCount == 0)
        {
            // Disminución completa (sin transacciones este mes, pero hubo el mes pasado)
            percentageChange = -100;
        }
        else if (lastMonthCount == 0 && currentMonthCount == 0)
        {
            // Sin cambios (ambos meses sin transacciones)
            percentageChange = 0;
        }
        else
        {
            // Cálculo normal
            percentageChange = ((double)(currentMonthCount - lastMonthCount) / lastMonthCount) * 100;
        }

        _logger.LogInformation($"Monthly percentage change calculated: {percentageChange}%");
        return percentageChange;
    }

    public async Task<Dictionary<int, int>> GetTransactionsPerHourAsync(Guid userId, Guid roleId, DateTime date)
    {
        var collection = _mongoDBHelper.GetCollection<Transaction>("transaction_data");

        // Filtrar las transacciones del usuario o todas si es Admin
        FilterDefinition<Transaction> filter = roleId == Guid.Empty
            ? Builders<Transaction>.Filter.Empty
            : Builders<Transaction>.Filter.Eq(t => t.UserBankTransactionId, userId.ToString());

        // Filtrar solo las transacciones del día especificado
        var startOfDay = date.Date;
        var endOfDay = startOfDay.AddDays(1);

        filter = Builders<Transaction>.Filter.And(
            filter,
            Builders<Transaction>.Filter.Gte(t => t.TransactionDate, startOfDay),
            Builders<Transaction>.Filter.Lt(t => t.TransactionDate, endOfDay)
        );

        // Proyectar las horas y agrupar las transacciones por cada hora
        var transactionsByHour = await collection.Aggregate()
            .Match(filter)
            .Group(new BsonDocument
            {
            { "_id", new BsonDocument("$hour", "$TransactionDate") },
            { "count", new BsonDocument("$sum", 1) }
            })
            .ToListAsync();

        // Convertir el resultado a un diccionario
        var result = new Dictionary<int, int>();
        foreach (var item in transactionsByHour)
        {
            var hour = item["_id"].AsInt32;
            var count = item["count"].AsInt32;
            result[hour] = count;
        }

        return result;
    }

    public async Task<List<KeyValuePair<DateTime, int>>> GetCumulativeTransactionTrendAsync(Guid userId, Guid roleId, DateTime startDate, DateTime endDate)
    {
        var collection = _mongoDBHelper.GetCollection<Transaction>("transaction_data");

        FilterDefinition<Transaction> filter;

        if (userId == Guid.Empty)
        {
            // Para el administrador, no aplicar filtro de usuario
            filter = Builders<Transaction>.Filter.Empty;
        }
        else
        {
            // Filtrar por usuario específico si no es admin
            filter = Builders<Transaction>.Filter.Eq(t => t.UserBankTransactionId, userId.ToString());
        }

        // Filtrar también por el rango de fechas
        filter = Builders<Transaction>.Filter.And(
            filter,
            Builders<Transaction>.Filter.Gte(t => t.TransactionDate, startDate),
            Builders<Transaction>.Filter.Lte(t => t.TransactionDate, endDate)
        );

        // Agregar y agrupar las transacciones por día
        var transactionsByDay = await collection.Aggregate()
            .Match(filter)
            .Group(new BsonDocument
            {
            { "_id", new BsonDocument("$dateToString", new BsonDocument { { "format", "%Y-%m-%d" }, { "date", "$TransactionDate" } }) },
            { "count", new BsonDocument("$sum", 1) }
            })
            .Sort(new BsonDocument("_id", 1))  // Ordenar por fecha
            .ToListAsync();

        // Convertir y acumular el resultado en una lista de pares clave-valor
        var result = new List<KeyValuePair<DateTime, int>>();
        int cumulativeTotal = 0;

        foreach (var item in transactionsByDay)
        {
            var date = DateTime.Parse(item["_id"].AsString);
            var count = item["count"].AsInt32;

            cumulativeTotal += count;  // Sumar al total acumulativo
            result.Add(new KeyValuePair<DateTime, int>(date, cumulativeTotal));
        }

        return result;
    }

    public async Task<(int successCount, int errorCount)> GetSuccessErrorCountAsync(Guid userId, Guid roleId)
    {
        var collection = _mongoDBHelper.GetCollection<Transaction>("transaction_data");

        // Filtrar las transacciones del usuario o todas si es Admin
        FilterDefinition<Transaction> filter;

        if (userId == Guid.Empty)
        {
            filter = Builders<Transaction>.Filter.Empty;
        }
        else
        {
            filter = Builders<Transaction>.Filter.Eq(t => t.UserBankTransactionId, userId.ToString());
        }

        // Definir los filtros de éxito y error basados en el campo de estado
        var successFilter = Builders<Transaction>.Filter.And(filter, Builders<Transaction>.Filter.Eq(t => t.Status, TransactionStatus.SentToBlockchain));
        var errorFilter = Builders<Transaction>.Filter.And(filter, Builders<Transaction>.Filter.Eq(t => t.Status, TransactionStatus.Failed));

        // Contar las transacciones exitosas y erróneas
        var successCount = await collection.CountDocumentsAsync(successFilter);
        var errorCount = await collection.CountDocumentsAsync(errorFilter);

        return ((int)successCount, (int)errorCount);
    }

    public async Task<int> GetTransactionCountByMonthAsync(Guid userId, Guid roleId, DateTime referenceDate)
    {
        var collection = _mongoDBHelper.GetCollection<Transaction>("transaction_data");

        // Filtrar las transacciones del usuario o todas si es Admin
        FilterDefinition<Transaction> filter;

        if (userId == Guid.Empty)
        {
            filter = Builders<Transaction>.Filter.Empty;
        }
        else
        {
            filter = Builders<Transaction>.Filter.Eq(t => t.UserBankTransactionId, userId.ToString());
        }

        // Definir el rango de fechas para el mes
        var startOfMonth = new DateTime(referenceDate.Year, referenceDate.Month, 1);
        var startOfNextMonth = startOfMonth.AddMonths(1);

        // Agregar los filtros de rango de fechas
        filter = Builders<Transaction>.Filter.And(
            filter,
            Builders<Transaction>.Filter.Gte(t => t.TransactionDate, startOfMonth),
            Builders<Transaction>.Filter.Lt(t => t.TransactionDate, startOfNextMonth)
        );

        // Contar las transacciones en el rango
        var transactionCount = await collection.CountDocumentsAsync(filter);
        return (int)transactionCount;
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
