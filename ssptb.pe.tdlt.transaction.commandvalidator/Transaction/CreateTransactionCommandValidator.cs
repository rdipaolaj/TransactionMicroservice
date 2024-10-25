using FluentValidation;
using ssptb.pe.tdlt.transaction.command.Transaction;
using System.Text.Json;

namespace ssptb.pe.tdlt.transaction.commandvalidator.Transaction;
public class CreateTransactionCommandValidator : AbstractValidator<CreateTransactionCommand>
{
    public CreateTransactionCommandValidator()
    {
        RuleFor(x => x.Transaction)
            .NotNull().WithMessage("La transacción no puede ser nula.");

        When(x => x.Transaction != null, () =>
        {
            RuleFor(x => x.Transaction.UserBankTransactionId)
                .NotEmpty().WithMessage("El ID del usuario para la transacción bancaria es obligatorio.")
                .Must(BeValidGuid).WithMessage("El ID del usuario debe tener el formato de un GUID válido."); // Verifica que sea un GUID válido

            RuleFor(x => x.Transaction.Tag)
                .NotEmpty().WithMessage("El Tag es obligatorio.");

            RuleFor(x => x.Transaction.TransactionData)
                .NotEqual(default(JsonElement)).WithMessage("Los datos de la transacción son obligatorios.")
                .Must(HaveValidTransactionData).WithMessage("Los datos de la transacción no pueden estar vacíos.");
        });
    }

    private bool BeValidGuid(string userBankTransactionId)
    {
        return Guid.TryParse(userBankTransactionId, out _); // Verifica si el ID es un GUID válido
    }

    private bool HaveValidTransactionData(JsonElement transactionData)
    {
        return transactionData.ValueKind != JsonValueKind.Undefined && transactionData.ValueKind != JsonValueKind.Null; // Verifica que los datos no estén vacíos
    }
}
