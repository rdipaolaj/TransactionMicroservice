using ssptb.pe.tdlt.transaction.entities.Enums;

namespace ssptb.pe.tdlt.transaction.dto.Transaction
{
    public class TransactionConfirmationDto
    {
        public Guid TransactionId { get; set; }
        public string BlockchainTransactionId { get; set; } = string.Empty;
        public TransactionStatus Status { get; set; }
        public DateTime ConfirmationDate { get; set; }
        // Otros campos necesarios...
    }
}
