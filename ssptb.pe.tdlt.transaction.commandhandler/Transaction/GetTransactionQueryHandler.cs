using MapsterMapper;
using MediatR;
using ssptb.pe.tdlt.transaction.command.Transaction.Queries;
using ssptb.pe.tdlt.transaction.data.Repositories;
using ssptb.pe.tdlt.transaction.dto.Transaction;

namespace ssptb.pe.tdlt.transaction.commandhandler.Transaction;
public class GetTransactionQueryHandler : IRequestHandler<GetTransactionQuery, TransactionResponseDto>
{
    private readonly ITransactionRepository _transactionRepository;
    private readonly IMapper _mapper;

    public GetTransactionQueryHandler(ITransactionRepository transactionRepository, IMapper mapper)
    {
        _transactionRepository = transactionRepository;
        _mapper = mapper;
    }

    public async Task<TransactionResponseDto> Handle(GetTransactionQuery request, CancellationToken cancellationToken)
    {
        var transaction = await _transactionRepository.GetTransactionAsync(request.TransactionId);
        return _mapper.Map<TransactionResponseDto>(transaction);
    }
}
