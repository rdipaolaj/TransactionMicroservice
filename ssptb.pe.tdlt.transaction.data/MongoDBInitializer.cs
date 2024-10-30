using Microsoft.Extensions.Options;
using MongoDB.Driver;
using ssptb.pe.tdlt.transaction.common.Settings;
using ssptb.pe.tdlt.transaction.entities;

namespace ssptb.pe.tdlt.transaction.data;
public class MongoDBInitializer
{
    private readonly IMongoClient _client;
    private readonly MongoDbSettings _settings;

    public MongoDBInitializer(IMongoClient client, IOptions<MongoDbSettings> settings)
    {
        _client = client;
        _settings = settings.Value;
    }

    public void InitializeDatabase()
    {
        var database = _client.GetDatabase("Transaction");

        var collections = database.ListCollectionNames().ToList();
        if (!collections.Contains("transaction_data"))
        {
            // Crear la colección si no existe
            database.CreateCollection("transaction_data");

            // Opcional: Crear índices para mejorar el rendimiento
            var fileMetadataCollection = database.GetCollection<Transaction>("transaction_data");
            var indexKeys = Builders<Transaction>.IndexKeys.Ascending(f => f.UserBankTransactionId);
            fileMetadataCollection.Indexes.CreateOne(new CreateIndexModel<Transaction>(indexKeys));

            Console.WriteLine("La colección 'transaction_data' fue creada en MongoDB.");
        }
        else
        {
            Console.WriteLine("La colección 'transaction_data' ya existe en MongoDB.");
        }
    }
}
