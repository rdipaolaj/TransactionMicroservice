namespace ssptb.pe.tdlt.transaction.common.Settings;
public class SecretManagerSettings
{
    public bool Local { get; set; }
    public string Region { get; set; } = string.Empty;
    public string ArnCouchBaseSecrets { get; set; } = string.Empty;
    public string ArnMongoDBSecrets { get; set; } = string.Empty;
}
