namespace ssptb.pe.tdlt.transaction.dto.Transaction;
public class TransactionRequestDto
{
    public string BankTransactionId { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Currency { get; set; } = string.Empty;
    public string SenderAccount { get; set; } = string.Empty;
    public string ReceiverAccount { get; set; } = string.Empty;
    public DateTime TransactionDate { get; set; }
    // Otros campos necesarios...
}
