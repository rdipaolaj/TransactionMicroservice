namespace ssptb.pe.tdlt.transaction.dto.Blockchain;
public class NodeStatusDto
{
    public bool IsHealthy { get; set; }
    public string Version { get; set; } = string.Empty;
    public string NetworkName { get; set; } = string.Empty;
    // Agrega otras propiedades que consideres necesarias
}
