using MongoDB.Driver;

namespace ssptb.pe.tdlt.transaction.data.Helpers;
public class MongoDBHelper : IMongoDBHelper
{
    private readonly IMongoDatabase _database;

    public MongoDBHelper(IMongoDatabase database)
    {
        _database = database;
    }

    public IMongoCollection<T> GetCollection<T>(string collectionName)
    {
        return _database.GetCollection<T>(collectionName);
    }
}
