using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using ssptb.pe.tdlt.transaction.common.Enums;
using ssptb.pe.tdlt.transaction.common.Exceptions;
using ssptb.pe.tdlt.transaction.common.Extensions;

namespace ssptb.pe.tdlt.transaction.infraestructure.Behaviors;
public class ValidatorBehavior<TRequest, TResponse>(
    IEnumerable<IValidator<TRequest>> validators,
    ILogger<ValidatorBehavior<TRequest, TResponse>> logger) :
    IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
{
    private readonly ILogger<ValidatorBehavior<TRequest, TResponse>> _logger = logger;
    private readonly IEnumerable<IValidator<TRequest>> _validators = validators;

    public async Task<TResponse> Handle(TRequest request,
        RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var typeName = request.GetGenericTypeName();

        _logger.LogInformation("Validando command {CommandType}", typeName);

        var failures = _validators
            .Select(v => v.Validate(request))
            .SelectMany(result => result.Errors)
            .Where(error => error != null)
            .ToList();

        if (failures.Count != 0)
        {
            _logger.LogWarning("Errores encontrados para command - {CommandType}", typeName);

            throw new CustomException(
                $"Command Validation Errors for type {typeof(TRequest).Name}",
                ApiErrorCode.ValidationError,
                new ValidationException("Validation exception", failures));
        }

        return await next();
    }
}