using ssptb.pe.tdlt.transaction.entities.Enums;

namespace ssptb.pe.tdlt.transaction.dto.Transaction;
public class TransactionResponseDto
{
    public Guid TransactionId { get; set; }
    public string BlockId { get; set; } = string.Empty;
}
