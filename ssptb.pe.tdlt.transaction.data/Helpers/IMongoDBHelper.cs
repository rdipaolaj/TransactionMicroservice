using MongoDB.Driver;

namespace ssptb.pe.tdlt.transaction.data.Helpers;
public interface IMongoDBHelper
{
    IMongoCollection<T> GetCollection<T>(string collectionName);
}
