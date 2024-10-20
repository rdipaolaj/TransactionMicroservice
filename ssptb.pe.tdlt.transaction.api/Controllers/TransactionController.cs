using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using ssptb.pe.tdlt.transaction.api.Configuration;
using ssptb.pe.tdlt.transaction.command.Transaction;
using ssptb.pe.tdlt.transaction.command.Transaction.Queries;
using ssptb.pe.tdlt.transaction.dto.Transaction;

namespace ssptb.pe.tdlt.transaction.api.Controllers;

[ApiVersion(1)]
[ApiController]
[Route("ssptbpetdlt/transaction/api/v{v:apiVersion}/[controller]")]
public class TransactionController : CustomController
{
    private readonly ILogger<TransactionController> _logger;
    private readonly IMediator _mediator;

    public TransactionController(IMediator mediator, ILogger<TransactionController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [HttpGet]
    [Route("node-info")]
    public async Task<IActionResult> GetNodeInfo()
    {
        var query = new GetNodeInfoQuery();
        var result = await _mediator.Send(query);
        return OkorBadRequestValidationApiResponse(result);
    }

    [HttpPost]
    [Route("create")]
    public async Task<IActionResult> CreateTransaction([FromBody] TransactionRequestDto request)
    {
        var command = new CreateTransactionCommand(request);
        var result = await _mediator.Send(command);
        return OkorBadRequestValidationApiResponse(result);
    }

    [HttpGet]
    [Route("{id}")]
    public async Task<IActionResult> GetTransaction(Guid id)
    {
        var query = new GetTransactionQuery(id);
        var result = await _mediator.Send(query);
        return OkorBadRequestValidationApiResponse(result);
    }

    [HttpGet]
    [Route("all")]
    public async Task<IActionResult> GetAllTransactions()
    {
        var query = new GetAllTransactionsQuery();
        var result = await _mediator.Send(query);
        return OkorBadRequestValidationApiResponse(result);
    }

    [HttpPost]
    [Route("confirm")]
    public async Task<IActionResult> ConfirmTransaction([FromBody] TransactionConfirmationDto confirmation)
    {
        var command = new ConfirmTransactionCommand(confirmation);
        var result = await _mediator.Send(command);
        return Ok(result);
    }
}
