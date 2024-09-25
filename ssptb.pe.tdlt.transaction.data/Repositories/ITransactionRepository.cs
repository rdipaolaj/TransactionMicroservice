using ssptb.pe.tdlt.transaction.entities;

namespace ssptb.pe.tdlt.transaction.data.Repositories;
public interface ITransactionRepository
{
    Task SaveTransactionAsync(Transaction transaction);
    Task<Transaction> GetTransactionAsync(Guid id);
    // Otros métodos según tus necesidades...
}
