using ssptb.pe.tdlt.transaction.common.Settings;
using System.Text.Json.Serialization;

namespace ssptb.pe.tdlt.transaction.common.Secrets;
public class CouchBaseSecrets : ISecret
{
    [JsonPropertyName("ConnectionString")]
    public string ConnectionString { get; set; } = string.Empty;

    [JsonPropertyName("BucketName")]
    public string BucketName { get; set; } = string.Empty;

    [JsonPropertyName("UserName")]
    public string UserName { get; set; } = string.Empty;

    [JsonPropertyName("Password")]
    public string Password { get; set; } = string.Empty;
}
