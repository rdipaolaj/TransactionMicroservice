using MediatR;
using ssptb.pe.tdlt.transaction.command.Metrics.Queries;
using ssptb.pe.tdlt.transaction.common.Responses;
using ssptb.pe.tdlt.transaction.data.Repositories;
using ssptb.pe.tdlt.transaction.dto.Metrics;
using ssptb.pe.tdlt.transaction.dto.User;
using ssptb.pe.tdlt.transaction.internalservices.User;

namespace ssptb.pe.tdlt.transaction.commandhandler.Metrics;
public class GetTotalTransactionsQueryHandler : IRequestHandler<GetTotalTransactionsQuery, ApiResponse<TotalTransactionsResponseDto>>
{
    private readonly ITransactionRepository _transactionRepository;
    private readonly IUserDataService _userDataService;

    // Lista de roles permitidos
    private readonly Guid _adminRoleId = Guid.Parse("c84b6988-ab74-4a23-81cb-f6aa889ca3d0");
    private readonly Guid _companyRoleId = Guid.Parse("321ffdf4-4941-4c3b-8188-92fd068e7fba");
    private readonly Guid _userRoleId = Guid.Parse("2231b31f-10e5-461e-9b12-0b8d58255216");

    public GetTotalTransactionsQueryHandler(ITransactionRepository transactionRepository, IUserDataService userDataService)
    {
        _transactionRepository = transactionRepository;
        _userDataService = userDataService;
    }

    public async Task<ApiResponse<TotalTransactionsResponseDto>> Handle(GetTotalTransactionsQuery request, CancellationToken cancellationToken)
    {
        // Obtener los datos del usuario usando el servicio de usuario
        var userResponse = await _userDataService.GetUserDataClientById(new GetUserByIdRequestDto { UserId = request.UserId });

        if (!userResponse.Success || userResponse.Data == null)
        {
            return ApiResponseHelper.CreateErrorResponse<TotalTransactionsResponseDto>("Error al obtener datos del usuario");
        }

        // Determinar si el usuario es Admin
        bool isAdmin = userResponse.Data.RoleId == _adminRoleId;

        // Llamar al repositorio con los parámetros correctos según el rol
        var totalTransactions = await _transactionRepository.GetTotalTransactionsAsync(
            userId: request.UserId,
            roleId: isAdmin ? Guid.Empty : request.UserId
        );

        // Calcular el cambio porcentual mensual
        var percentageChange = await _transactionRepository.CalculateMonthlyPercentageChangeAsync(
            userId: request.UserId,
            roleId: isAdmin ? Guid.Empty : request.UserId
        );

        // Crear la respuesta con los resultados
        var response = new TotalTransactionsResponseDto
        {
            Total = totalTransactions,
            Percentage = percentageChange
        };

        return ApiResponseHelper.CreateSuccessResponse(response, "Total transactions retrieved successfully.");
    }
}
