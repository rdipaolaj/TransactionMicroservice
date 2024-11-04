using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using ssptb.pe.tdlt.transaction.api.Configuration;
using ssptb.pe.tdlt.transaction.command.Metrics.Queries;

namespace ssptb.pe.tdlt.transaction.api.Controllers;

[ApiVersion(1)]
[ApiController]
[Route("ssptbpetdlt/transaction/api/v{v:apiVersion}/[controller]")]
public class MetricsController : CustomController
{
    private readonly ILogger<MetricsController> _logger;
    private readonly IMediator _mediator;

    public MetricsController(IMediator mediator, ILogger<MetricsController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [HttpGet]
    [Route("total-transactions/{id}/{roleId}")]
    public async Task<IActionResult> GetTotalTransactions(Guid id, Guid roleId)
    {
        var query = new GetTotalTransactionsQuery(id, roleId);
        var result = await _mediator.Send(query);
        return OkorBadRequestValidationApiResponse(result);
    }

    [HttpGet]
    [Route("transactions-per-hour/{id}/{roleId}")]
    public async Task<IActionResult> GetTransactionsPerHour(Guid id, Guid roleId)
    {
        var query = new GetTransactionsPerHourQuery(id, roleId);
        var result = await _mediator.Send(query);
        return OkorBadRequestValidationApiResponse(result);
    }

    [HttpGet]
    [Route("transaction-trend/{id}/{roleId}")]
    public async Task<IActionResult> GetTransactionTrend(Guid id, Guid roleId)
    {
        var query = new GetTransactionTrendQuery(id, roleId);
        var result = await _mediator.Send(query);
        return OkorBadRequestValidationApiResponse(result);
    }

    [HttpGet]
    [Route("success-error-ratio/{id}/{roleId}")]
    public async Task<IActionResult> GetSuccessErrorRatio(Guid id, Guid roleId)
    {
        var query = new GetSuccessErrorRatioQuery(id, roleId);
        var result = await _mediator.Send(query);
        return OkorBadRequestValidationApiResponse(result);
    }

    [HttpGet]
    [Route("monthly-comparison/{id}/{roleId}")]
    public async Task<IActionResult> GetMonthlyComparison(Guid id, Guid roleId)
    {
        var query = new GetMonthlyComparisonQuery(id, roleId);
        var result = await _mediator.Send(query);
        return OkorBadRequestValidationApiResponse(result);
    }
}
