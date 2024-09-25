using MediatR;
using ssptb.pe.tdlt.transaction.common.Responses;
using ssptb.pe.tdlt.transaction.dto.Blockchain;

namespace ssptb.pe.tdlt.transaction.command.Transaction.Queries;
public class GetNodeInfoQuery : IRequest<ApiResponse<NodeStatusDto>>
{
}
