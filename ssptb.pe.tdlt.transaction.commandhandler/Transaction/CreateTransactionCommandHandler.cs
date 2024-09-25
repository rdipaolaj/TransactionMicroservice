using MapsterMapper;
using MediatR;
using ssptb.pe.tdlt.transaction.command.Transaction;
using ssptb.pe.tdlt.transaction.data;
using ssptb.pe.tdlt.transaction.entities.Enums;
using ssptb.pe.tdlt.transaction.entities;
using ssptb.pe.tdlt.transaction.data.Repositories;
using System.Net.Http.Json;
using ssptb.pe.tdlt.transaction.internalservices.Blockchain;

namespace ssptb.pe.tdlt.transaction.commandhandler.Transaction;
public class CreateTransactionCommandHandler : IRequestHandler<CreateTransactionCommand, Guid>
{
    private readonly ITransactionRepository _transactionRepository;
    private readonly IBlockchainService _blockchainService;
    private readonly IMapper _mapper;

    public CreateTransactionCommandHandler(ITransactionRepository transactionRepository, IBlockchainService blockchainService, IMapper mapper)
    {
        _transactionRepository = transactionRepository;
        _blockchainService = blockchainService;
        _mapper = mapper;
    }

    public async Task<Guid> Handle(CreateTransactionCommand request, CancellationToken cancellationToken)
    {
        var transaction = _mapper.Map<entities.Transaction>(request.Transaction);
        transaction.Id = Guid.NewGuid();
        transaction.Status = TransactionStatus.Pending;

        await _transactionRepository.SaveTransactionAsync(transaction);

        // Enviar al microservicio de Blockchain
        var response = await _blockchainService.RegisterTransactionAsync(transaction);

        if (response.Success)
        {
            transaction.Status = TransactionStatus.SentToBlockchain;
            await _transactionRepository.SaveTransactionAsync(transaction);
        }
        else
        {
            transaction.Status = TransactionStatus.Failed;
            await _transactionRepository.SaveTransactionAsync(transaction);
            // Manejar el error según corresponda
        }

        return transaction.Id;
    }
}
