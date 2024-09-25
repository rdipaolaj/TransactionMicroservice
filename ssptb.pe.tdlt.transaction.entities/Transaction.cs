using ssptb.pe.tdlt.transaction.entities.Enums;

namespace ssptb.pe.tdlt.transaction.entities;
public class Transaction
{
    public Guid Id { get; set; }
    public string BankTransactionId { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Currency { get; set; } = string.Empty;
    public string SenderAccount { get; set; } = string.Empty;
    public string ReceiverAccount { get; set; } = string.Empty;
    public DateTime TransactionDate { get; set; }
    public TransactionStatus Status { get; set; }
    // Otros campos necesarios...
}
