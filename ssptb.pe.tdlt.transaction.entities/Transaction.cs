using ssptb.pe.tdlt.transaction.entities.Enums;
using System.Text.Json;

namespace ssptb.pe.tdlt.transaction.entities;
public class Transaction
{
    public Guid Id { get; set; }
    public string UserBankTransactionId { get; set; } = string.Empty;
    public string Tag { get; set; } = string.Empty;

    public JsonElement? TransactionData { get; set; } = null;
    public string TransactionDataSave { get; set; } = string.Empty;
    public DateTime TransactionDate { get; set; }
    public TransactionStatus Status { get; set; }
    public int SchemaVersion { get; set; }
    public string? BlockId { get; set; }
    public string? StorageUrl { get; set; }
    // Otros campos necesarios...
}
