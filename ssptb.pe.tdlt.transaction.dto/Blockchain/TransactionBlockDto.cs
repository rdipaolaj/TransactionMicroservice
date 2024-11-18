using System.Text.Json;

namespace ssptb.pe.tdlt.transaction.dto.Blockchain;

/// <summary>
/// DTO para la transacción obtenida por BlockId desde la Blockchain.
/// </summary>
public class TransactionBlockDto
{
    public JsonElement? TransactionData { get; set; } // Almacena los datos dinámicos de la transacción
}
