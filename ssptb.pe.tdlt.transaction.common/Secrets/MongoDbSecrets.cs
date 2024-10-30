using ssptb.pe.tdlt.transaction.common.Settings;
using System.Text.Json.Serialization;

namespace ssptb.pe.tdlt.transaction.common.Secrets;
public class MongoDbSecrets : ISecret
{
    [JsonPropertyName("ConnectionString")]
    public string ConnectionString { get; set; } = string.Empty;
}
