namespace ssptb.pe.tdlt.transaction.dto.Audit;

/// <summary>
/// Resultado de la auditoría completa.
/// </summary>
public class AuditResultResponseDto
{
    public int TotalTransactions { get; set; }
    public int TotalDiscrepancies { get; set; }
    public List<AuditDiscrepancyDto> Discrepancies { get; set; } = new List<AuditDiscrepancyDto>();
    public byte[] ExcelFileData { get; set; } = Array.Empty<byte>(); // Contiene el archivo Excel en memoria
}

/// <summary>
/// Detalles de una discrepancia detectada en la auditoría.
/// </summary>
public class AuditDiscrepancyDto
{
    public string BlockId { get; set; } = string.Empty;
    public string Reason { get; set; } = string.Empty;
    public string LocalData { get; set; } = string.Empty;
    public string TangleData { get; set; } = string.Empty;
    public string Error { get; set; } = string.Empty;
}