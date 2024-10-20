using ssptb.pe.tdlt.transaction.entities.Enums;

namespace ssptb.pe.tdlt.transaction.dto.Transaction;
public class TransactionAllResponseDto
{
    public Guid Id { get; set; }
    public string BankTransactionId { get; set; } = string.Empty;
    public DateTime TransactionDate { get; set; }
    public TransactionStatus Status { get; set; }
    public string? BlockId { get; set; }
    public string TransactionData { get; set; } = string.Empty;
}
