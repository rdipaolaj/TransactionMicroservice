using MediatR;
using ssptb.pe.tdlt.transaction.command.Metrics.Queries;
using ssptb.pe.tdlt.transaction.common.Responses;
using ssptb.pe.tdlt.transaction.data.Repositories;
using ssptb.pe.tdlt.transaction.dto.Metrics;
using ssptb.pe.tdlt.transaction.dto.User;
using ssptb.pe.tdlt.transaction.internalservices.User;

namespace ssptb.pe.tdlt.transaction.commandhandler.Metrics;
public class GetMonthlyComparisonQueryHandler : IRequestHandler<GetMonthlyComparisonQuery, ApiResponse<MonthlyComparisonResponseDto>>
{
    private readonly ITransactionRepository _transactionRepository;
    private readonly IUserDataService _userDataService;
    private readonly Guid _adminRoleId = Guid.Parse("c84b6988-ab74-4a23-81cb-f6aa889ca3d0");

    public GetMonthlyComparisonQueryHandler(ITransactionRepository transactionRepository, IUserDataService userDataService)
    {
        _transactionRepository = transactionRepository;
        _userDataService = userDataService;
    }

    public async Task<ApiResponse<MonthlyComparisonResponseDto>> Handle(GetMonthlyComparisonQuery request, CancellationToken cancellationToken)
    {
        // Obtener los datos del usuario
        var userResponse = await _userDataService.GetUserDataClientById(new GetUserByIdRequestDto { UserId = request.UserId });
        if (!userResponse.Success || userResponse.Data == null)
        {
            return ApiResponseHelper.CreateErrorResponse<MonthlyComparisonResponseDto>("Error al obtener datos del usuario");
        }

        bool isAdmin = userResponse.Data.RoleId == _adminRoleId;
        var userIdToUse = isAdmin ? Guid.Empty : request.UserId;
        var roleIdToUse = userResponse.Data.RoleId;

        var lastMonthCount = await _transactionRepository.GetTransactionCountByMonthAsync(
            userId: userIdToUse,
            roleId: roleIdToUse,
            referenceDate: DateTime.UtcNow.AddMonths(-1)
        );

        var currentMonthCount = await _transactionRepository.GetTransactionCountByMonthAsync(
            userId: userIdToUse,
            roleId: roleIdToUse,
            referenceDate: DateTime.UtcNow
        );

        double percentageChange = CalculatePercentageChange(lastMonthCount, currentMonthCount);

        var response = new MonthlyComparisonResponseDto
        {
            LastMonthTransactionCount = lastMonthCount,
            CurrentMonthTransactionCount = currentMonthCount,
            PercentageChange = percentageChange
        };

        return ApiResponseHelper.CreateSuccessResponse(response, "Monthly comparison retrieved successfully.");
    }

    private double CalculatePercentageChange(int lastMonthCount, int currentMonthCount)
    {
        if (lastMonthCount == 0)
        {
            return currentMonthCount > 0 ? 100 : 0;
        }
        return ((double)(currentMonthCount - lastMonthCount) / lastMonthCount) * 100;
    }
}
