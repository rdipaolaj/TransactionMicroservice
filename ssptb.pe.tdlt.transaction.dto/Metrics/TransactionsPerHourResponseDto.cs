namespace ssptb.pe.tdlt.transaction.dto.Metrics;
public class TransactionsPerHourResponseDto
{
    public List<HourlyTransaction> HourlyTransactions { get; set; }
    public double DailyPercentageChange { get; set; }  // Cambio porcentual total del día comparado con el día anterior
}

public class HourlyTransaction
{
    public int Hour { get; set; }            // Hora del día (0 - 23)
    public int TransactionCount { get; set; } // Número de transacciones en esa hora
    public double HourlyChange { get; set; }  // Cambio porcentual respecto a la misma hora del día anterior
}