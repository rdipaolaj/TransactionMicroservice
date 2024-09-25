using MapsterMapper;
using MediatR;
using ssptb.pe.tdlt.transaction.command.Transaction;
using ssptb.pe.tdlt.transaction.data;
using ssptb.pe.tdlt.transaction.data.Repositories;
using ssptb.pe.tdlt.transaction.entities.Enums;

namespace ssptb.pe.tdlt.transaction.commandhandler.Transaction;
public class ConfirmTransactionCommandHandler : IRequestHandler<ConfirmTransactionCommand, bool>
{
    private readonly ITransactionRepository _transactionRepository;

    public ConfirmTransactionCommandHandler(ITransactionRepository transactionRepository)
    {
        _transactionRepository = transactionRepository;
    }

    public async Task<bool> Handle(ConfirmTransactionCommand request, CancellationToken cancellationToken)
    {
        var transaction = await _transactionRepository.GetTransactionAsync(request.Confirmation.TransactionId);

        if (transaction == null)
            return false;

        transaction.Status = TransactionStatus.Confirmed;

        await _transactionRepository.SaveTransactionAsync(transaction);

        return true;
    }
}
