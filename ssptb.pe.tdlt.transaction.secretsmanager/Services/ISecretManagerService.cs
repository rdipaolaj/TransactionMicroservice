using ssptb.pe.tdlt.transaction.common.Secrets;

namespace ssptb.pe.tdlt.transaction.secretsmanager.Services;
public interface ISecretManagerService
{
    Task<CouchBaseSecrets?> GetCouchBaseSecrets();
}
