namespace ssptb.pe.tdlt.transaction.common.Helpers;
public class EnvironmentHelper
{
    public static bool IsDevelopment()
    {
        string environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? string.Empty;
        return environment.Equals("Development", StringComparison.OrdinalIgnoreCase);
    }
}
