using MediatR;
using ssptb.pe.tdlt.transaction.dto.Transaction;

namespace ssptb.pe.tdlt.transaction.command.Transaction;
public class ConfirmTransactionCommand : IRequest<bool>
{
    public TransactionConfirmationDto Confirmation { get; }

    public ConfirmTransactionCommand(TransactionConfirmationDto confirmation)
    {
        Confirmation = confirmation;
    }
}
