using System.Text.Json.Serialization;

namespace ssptb.pe.tdlt.transaction.dto.Blockchain;
public class NodeInfoDto
{
    [JsonPropertyName("nodeInfo")]
    public NodeInfo NodeInfo { get; set; } = new NodeInfo();

    [JsonPropertyName("url")]
    public string Url { get; set; } = string.Empty;
}

public class BaseToken
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("tickerSymbol")]
    public string TickerSymbol { get; set; } = string.Empty;

    [JsonPropertyName("unit")]
    public string Unit { get; set; } = string.Empty;

    [JsonPropertyName("subunit")]
    public string Subunit { get; set; } = string.Empty;

    [JsonPropertyName("decimals")]
    public int Decimals { get; set; }

    [JsonPropertyName("useMetricPrefix")]
    public bool UseMetricPrefix { get; set; }
}

public class ConfirmedMilestone
{
    [JsonPropertyName("index")]
    public int Index { get; set; }

    [JsonPropertyName("timestamp")]
    public int Timestamp { get; set; }

    [JsonPropertyName("milestoneId")]
    public string MilestoneId { get; set; } = string.Empty;
}

public class LatestMilestone
{
    [JsonPropertyName("index")]
    public int Index { get; set; }

    [JsonPropertyName("timestamp")]
    public int Timestamp { get; set; }

    [JsonPropertyName("milestoneId")]
    public string MilestoneId { get; set; } = string.Empty;
}

public class Metrics
{
    [JsonPropertyName("blocksPerSecond")]
    public double BlocksPerSecond { get; set; }

    [JsonPropertyName("referencedMessagesPerSecond")]
    public double ReferencedBlocksPerSecond { get; set; }

    [JsonPropertyName("referencedRate")]
    public double ReferencedRate { get; set; }
}

public class NodeInfo
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("version")]
    public string Version { get; set; } = string.Empty;

    [JsonPropertyName("status")]
    public Status Status { get; set; } = new Status();

    [JsonPropertyName("supportedProtocolVersions")]
    public List<int> SupportedProtocolVersions { get; set; } = new List<int>();

    [JsonPropertyName("protocol")]
    public Protocol Protocol { get; set; } = new Protocol();

    [JsonPropertyName("pendingProtocolParameters")]
    public List<object> PendingProtocolParameters { get; set; } = new List<object>();

    [JsonPropertyName("baseToken")]
    public BaseToken BaseToken { get; set; } = new BaseToken();

    [JsonPropertyName("metrics")]
    public Metrics Metrics { get; set; } = new Metrics();

    [JsonPropertyName("features")]
    public List<string> Features { get; set; } = new List<string>();
}

public class Protocol
{
    [JsonPropertyName("version")]
    public int Version { get; set; }

    [JsonPropertyName("networkName")]
    public string NetworkName { get; set; } = string.Empty;

    [JsonPropertyName("bech32HRP")]
    public string Bech32Hrp { get; set; } = string.Empty;

    [JsonPropertyName("minPowScore")]
    public int MinPowScore { get; set; }

    [JsonPropertyName("belowMaxDepth")]
    public int BelowMaxDepth { get; set; }

    [JsonPropertyName("rentStructure")]
    public RentStructure RentStructure { get; set; } = new RentStructure();

    [JsonPropertyName("tokenSupply")]
    public string TokenSupply { get; set; } = string.Empty;
}

public class RentStructure
{
    [JsonPropertyName("vByteCost")]
    public int VByteCost { get; set; }

    [JsonPropertyName("vByteFactorKey")]
    public int VByteFactorKey { get; set; }

    [JsonPropertyName("vByteFactorData")]
    public int VByteFactorData { get; set; }
}

public class Status
{
    [JsonPropertyName("isHealthy")]
    public bool IsHealthy { get; set; }

    [JsonPropertyName("latestMilestone")]
    public LatestMilestone LatestMilestone { get; set; } = new LatestMilestone();

    [JsonPropertyName("confirmedMilestone")]
    public ConfirmedMilestone ConfirmedMilestone { get; set; } = new ConfirmedMilestone();

    [JsonPropertyName("pruningIndex")]
    public int PruningIndex { get; set; }
}

