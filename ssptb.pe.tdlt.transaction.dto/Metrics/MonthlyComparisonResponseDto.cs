namespace ssptb.pe.tdlt.transaction.dto.Metrics;
public class MonthlyComparisonResponseDto
{
    public int LastMonthTransactionCount { get; set; }
    public int CurrentMonthTransactionCount { get; set; }
    public double PercentageChange { get; set; }
}
