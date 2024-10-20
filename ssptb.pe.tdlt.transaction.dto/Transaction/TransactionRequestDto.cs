using System.Text.Json;

namespace ssptb.pe.tdlt.transaction.dto.Transaction;
public class TransactionRequestDto
{
    public string BankTransactionId { get; set; } = string.Empty;
    public string Tag { get; set; } = string.Empty;
    public JsonElement TransactionData { get; set; } = new JsonElement();
    public DateTime TransactionDate { get; set; }
}
