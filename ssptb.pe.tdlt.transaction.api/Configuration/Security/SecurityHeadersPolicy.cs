namespace ssptb.pe.tdlt.transaction.api.Configuration.Security;

public class SecurityHeadersPolicy
{
    public IDictionary<string, string> SetHeaders { get; } = new Dictionary<string, string>();
    public ISet<string> RemoveHeaders { get; } = new HashSet<string>();
}
