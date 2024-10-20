using System.Text.Json.Serialization;

namespace ssptb.pe.tdlt.transaction.dto.Blockchain;
public class RegisterTransactionDto
{
    [JsonPropertyName("blockId")]
    public string BlockId { get; set; } = string.Empty;

    [JsonPropertyName("block")]
    public Block Block { get; set; } = new Block();
}

public class Block
{
    [JsonPropertyName("protocolVersion")]
    public int ProtocolVersion { get; set; }

    [JsonPropertyName("parents")]
    public List<string> Parents { get; set; } = new List<string>();

    [JsonPropertyName("payload")]
    public Payload Payload { get; set; } = new Payload();

    [JsonPropertyName("nonce")]
    public string Nonce { get; set; } = string.Empty;
}

public class Payload
{
    [JsonPropertyName("type")]
    public int Type { get; set; }

    [JsonPropertyName("tag")]
    public string Tag { get; set; } = string.Empty;

    [JsonPropertyName("data")]
    public string Data { get; set; } = string.Empty;
}