using MediatR;
using ssptb.pe.tdlt.transaction.command.Metrics.Queries;
using ssptb.pe.tdlt.transaction.common.Responses;
using ssptb.pe.tdlt.transaction.data.Repositories;
using ssptb.pe.tdlt.transaction.dto.Metrics;
using ssptb.pe.tdlt.transaction.dto.User;
using ssptb.pe.tdlt.transaction.internalservices.User;

namespace ssptb.pe.tdlt.transaction.commandhandler.Metrics;

public class GetSuccessErrorRatioQueryHandler : IRequestHandler<GetSuccessErrorRatioQuery, ApiResponse<SuccessErrorRatioResponseDto>>
{
    private readonly ITransactionRepository _transactionRepository;
    private readonly IUserDataService _userDataService;
    private readonly Guid _adminRoleId = Guid.Parse("c84b6988-ab74-4a23-81cb-f6aa889ca3d0");

    public GetSuccessErrorRatioQueryHandler(ITransactionRepository transactionRepository, IUserDataService userDataService)
    {
        _transactionRepository = transactionRepository;
        _userDataService = userDataService;
    }

    public async Task<ApiResponse<SuccessErrorRatioResponseDto>> Handle(GetSuccessErrorRatioQuery request, CancellationToken cancellationToken)
    {
        // Obtener los datos del usuario
        var userResponse = await _userDataService.GetUserDataClientById(new GetUserByIdRequestDto { UserId = request.UserId });
        if (!userResponse.Success || userResponse.Data == null)
        {
            return ApiResponseHelper.CreateErrorResponse<SuccessErrorRatioResponseDto>("Error al obtener datos del usuario");
        }

        bool isAdmin = userResponse.Data.RoleId == _adminRoleId;
        var userIdToUse = isAdmin ? Guid.Empty : request.UserId;
        var roleIdToUse = userResponse.Data.RoleId;

        // Obtener los conteos de transacciones exitosas y erróneas
        var (successCount, errorCount) = await _transactionRepository.GetSuccessErrorCountAsync(
            userId: userIdToUse,
            roleId: roleIdToUse
        );

        double successPercentage = successCount + errorCount > 0
            ? ((double)successCount / (successCount + errorCount)) * 100
            : 0;

        var response = new SuccessErrorRatioResponseDto
        {
            SuccessCount = successCount,
            ErrorCount = errorCount,
            SuccessPercentage = successPercentage
        };

        return ApiResponseHelper.CreateSuccessResponse(response, "Success-error ratio retrieved successfully.");
    }
}