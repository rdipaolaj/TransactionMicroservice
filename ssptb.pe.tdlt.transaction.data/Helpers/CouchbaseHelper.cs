using Couchbase.Extensions.DependencyInjection;
using Couchbase.KeyValue;
using Couchbase;

namespace ssptb.pe.tdlt.transaction.data.Helpers;
public class CouchbaseHelper : ICouchbaseHelper
{
    private readonly INamedBucketProvider _bucketProvider;

    public CouchbaseHelper(INamedBucketProvider bucketProvider)
    {
        _bucketProvider = bucketProvider;
    }

    public async Task<ICouchbaseCollection> GetCollectionAsync(string scopeName, string collectionName)
    {
        // Obtén el bucket
        var bucket = await _bucketProvider.GetBucketAsync();

        // Obtén el scope y la collection especificados
        var scope = bucket.Scope(scopeName);
        var collection = scope.Collection(collectionName);

        return collection;
    }

    public async Task<ICluster> GetClusterAsync()
    {
        var bucket = await _bucketProvider.GetBucketAsync();
        return bucket.Cluster;  // Accedemos al cluster desde el bucket
    }
}
