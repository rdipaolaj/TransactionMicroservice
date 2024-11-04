namespace ssptb.pe.tdlt.transaction.dto.Metrics;
public class TransactionTrendResponseDto
{
    public List<KeyValuePair<DateTime, int>> TrendData { get; set; }
    public double Percentage { get; set; }
}