using MediatR;
using ssptb.pe.tdlt.transaction.command.Metrics.Queries;
using ssptb.pe.tdlt.transaction.common.Responses;
using ssptb.pe.tdlt.transaction.data.Repositories;
using ssptb.pe.tdlt.transaction.dto.Metrics;
using ssptb.pe.tdlt.transaction.dto.User;
using ssptb.pe.tdlt.transaction.internalservices.User;

namespace ssptb.pe.tdlt.transaction.commandhandler.Metrics;
public class GetTransactionsPerHourQueryHandler : IRequestHandler<GetTransactionsPerHourQuery, ApiResponse<TransactionsPerHourResponseDto>>
{
    private readonly ITransactionRepository _transactionRepository;
    private readonly IUserDataService _userDataService;

    // ID de rol administrador
    private readonly Guid _adminRoleId = Guid.Parse("c84b6988-ab74-4a23-81cb-f6aa889ca3d0");

    public GetTransactionsPerHourQueryHandler(ITransactionRepository transactionRepository, IUserDataService userDataService)
    {
        _transactionRepository = transactionRepository;
        _userDataService = userDataService;
    }

    public async Task<ApiResponse<TransactionsPerHourResponseDto>> Handle(GetTransactionsPerHourQuery request, CancellationToken cancellationToken)
    {
        // Obtener datos del usuario
        var userResponse = await _userDataService.GetUserDataClientById(new GetUserByIdRequestDto { UserId = request.UserId });

        if (!userResponse.Success || userResponse.Data == null)
        {
            return ApiResponseHelper.CreateErrorResponse<TransactionsPerHourResponseDto>("Error al obtener datos del usuario");
        }

        // Determinar si el usuario es Admin
        bool isAdmin = userResponse.Data.RoleId == _adminRoleId;

        // Ajustar userId y roleId para que el admin tenga acceso a todas las transacciones
        var userIdToUse = isAdmin ? Guid.Empty : request.UserId;
        var roleIdToUse = isAdmin ? Guid.Empty : userResponse.Data.RoleId;

        // Obtener transacciones por hora (todas si es Admin, o solo las del usuario si no lo es)
        var hourlyTransactionsToday = await _transactionRepository.GetTransactionsPerHourAsync(
            userId: userIdToUse,
            roleId: roleIdToUse,
            date: DateTime.UtcNow
        );

        var hourlyTransactionsYesterday = await _transactionRepository.GetTransactionsPerHourAsync(
            userId: userIdToUse,
            roleId: roleIdToUse,
            date: DateTime.UtcNow.AddDays(-1)
        );

        // Crear la lista de objetos HourlyTransaction con el cambio porcentual por hora
        var hourlyTransactions = new List<HourlyTransaction>();
        for (int hour = 0; hour < 24; hour++)
        {
            hourlyTransactionsToday.TryGetValue(hour, out int todayCount);
            hourlyTransactionsYesterday.TryGetValue(hour, out int yesterdayCount);

            double hourlyChange = CalculatePercentageChange(todayCount, yesterdayCount);
            hourlyTransactions.Add(new HourlyTransaction
            {
                Hour = hour,
                TransactionCount = todayCount,
                HourlyChange = hourlyChange
            });
        }

        // Calcular el cambio porcentual total del día
        double dailyPercentageChange = CalculateAdjustedDailyChange(hourlyTransactionsToday.Values.Sum(), hourlyTransactionsYesterday.Values.Sum());

        // Crear la respuesta con los datos
        var response = new TransactionsPerHourResponseDto
        {
            HourlyTransactions = hourlyTransactions,
            DailyPercentageChange = dailyPercentageChange
        };

        return ApiResponseHelper.CreateSuccessResponse(response, "Transactions per hour retrieved successfully.");
    }

    private double CalculatePercentageChange(int todayCount, int yesterdayCount)
    {
        if (yesterdayCount == 0)
        {
            return todayCount > 0 ? 100 : 0;
        }
        return ((double)(todayCount - yesterdayCount) / yesterdayCount) * 100;
    }

    private double CalculateAdjustedDailyChange(int todayTotal, int yesterdayTotal)
    {
        if (yesterdayTotal == 0 && todayTotal > 0)
        {
            // Si no hay transacciones el día anterior y hay transacciones hoy, lo ajustamos para evitar 100%
            return 100;
        }
        if (yesterdayTotal > 0 && todayTotal == 0)
        {
            // Si hay transacciones el día anterior y ninguna hoy, representamos una disminución total.
            return -100;
        }
        if (yesterdayTotal == 0 && todayTotal == 0)
        {
            // Sin transacciones en ambos días.
            return 0;
        }
        return ((double)(todayTotal - yesterdayTotal) / yesterdayTotal) * 100;
    }
}
