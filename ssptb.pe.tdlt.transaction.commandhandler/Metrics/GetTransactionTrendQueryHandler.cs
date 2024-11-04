using MediatR;
using ssptb.pe.tdlt.transaction.command.Metrics.Queries;
using ssptb.pe.tdlt.transaction.common.Responses;
using ssptb.pe.tdlt.transaction.data.Repositories;
using ssptb.pe.tdlt.transaction.dto.Metrics;
using ssptb.pe.tdlt.transaction.dto.User;
using ssptb.pe.tdlt.transaction.internalservices.User;

namespace ssptb.pe.tdlt.transaction.commandhandler.Metrics;
public class GetTransactionTrendQueryHandler : IRequestHandler<GetTransactionTrendQuery, ApiResponse<TransactionTrendResponseDto>>
{
    private readonly ITransactionRepository _transactionRepository;
    private readonly IUserDataService _userDataService;
    private readonly Guid _adminRoleId = Guid.Parse("c84b6988-ab74-4a23-81cb-f6aa889ca3d0");

    public GetTransactionTrendQueryHandler(ITransactionRepository transactionRepository, IUserDataService userDataService)
    {
        _transactionRepository = transactionRepository;
        _userDataService = userDataService;
    }

    public async Task<ApiResponse<TransactionTrendResponseDto>> Handle(GetTransactionTrendQuery request, CancellationToken cancellationToken)
    {
        // Obtener los datos del usuario
        var userResponse = await _userDataService.GetUserDataClientById(new GetUserByIdRequestDto { UserId = request.UserId });
        if (!userResponse.Success || userResponse.Data == null)
        {
            return ApiResponseHelper.CreateErrorResponse<TransactionTrendResponseDto>("Error al obtener datos del usuario");
        }

        // Determinar si el usuario es Admin
        bool isAdmin = userResponse.Data.RoleId == _adminRoleId;
        var userIdToUse = isAdmin ? Guid.Empty : request.UserId;
        var roleIdToUse = userResponse.Data.RoleId;

        // Obtener la tendencia acumulativa de transacciones por día para los últimos 30 días
        var trendData = await _transactionRepository.GetCumulativeTransactionTrendAsync(
            userId: userIdToUse,
            roleId: roleIdToUse,
            startDate: DateTime.UtcNow.AddDays(-30),
            endDate: DateTime.UtcNow
        );

        // Calcular el cambio porcentual entre el inicio y el final del período
        double percentageChange = CalculateCumulativePercentageChange(trendData);

        // Crear la respuesta con los datos acumulativos
        var response = new TransactionTrendResponseDto
        {
            TrendData = trendData,
            Percentage = percentageChange
        };

        return ApiResponseHelper.CreateSuccessResponse(response, "Cumulative transaction trend retrieved successfully.");
    }

    private double CalculateCumulativePercentageChange(List<KeyValuePair<DateTime, int>> trendData)
    {
        if (trendData.Count < 2) return 0;

        // Obtener el valor inicial y final en el período
        int initialCount = trendData.First().Value;
        int finalCount = trendData.Last().Value;

        if (initialCount == 0)
        {
            return finalCount > 0 ? 100 : 0;
        }

        return ((double)(finalCount - initialCount) / initialCount) * 100;
    }
}
