using Mapster;
using MapsterMapper;
using MediatR;
using ssptb.pe.tdlt.transaction.command.Transaction.Queries;
using ssptb.pe.tdlt.transaction.common.Responses;
using ssptb.pe.tdlt.transaction.dto.Blockchain;
using ssptb.pe.tdlt.transaction.internalservices.Blockchain;

namespace ssptb.pe.tdlt.transaction.commandhandler.Transaction;
public class GetNodeInfoQueryHandler : IRequestHandler<GetNodeInfoQuery, ApiResponse<NodeStatusDto>>
{
    private readonly IBlockchainService _blockchainService;
    private readonly IMapper _mapper;

    public GetNodeInfoQueryHandler(IBlockchainService blockchainService, IMapper mapper)
    {
        _blockchainService = blockchainService;
        _mapper = mapper;
    }

    public async Task<ApiResponse<NodeStatusDto>> Handle(GetNodeInfoQuery request, CancellationToken cancellationToken)
    {
        var response = await _blockchainService.GetNodeInfoAsync();

        if (!response.Success || response.Data == null)
        {
            return ApiResponseHelper.CreateErrorResponse<NodeStatusDto>(response.Message, 500, response.Errors);
        }

        // Mapear los datos al DTO simplificado
        var nodeStatus = _mapper.Map<NodeStatusDto>(response.Data);

        return ApiResponseHelper.CreateSuccessResponse(nodeStatus, "Node status retrieved successfully.");
    }
}
