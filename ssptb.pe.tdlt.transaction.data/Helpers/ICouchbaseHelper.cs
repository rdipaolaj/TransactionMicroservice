using Couchbase.KeyValue;
using Couchbase;

namespace ssptb.pe.tdlt.transaction.data.Helpers;
public interface ICouchbaseHelper
{
    Task<ICouchbaseCollection> GetCollectionAsync(string scopeName, string collectionName);
    Task<ICluster> GetClusterAsync();
}
