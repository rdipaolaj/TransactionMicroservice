using TimeZoneConverter;

namespace ssptb.pe.tdlt.transaction.common.Helpers;
public static class DatetimeHelper
{
    public const string TimeZone = "SA Pacific Standard Time";

    public static DateTime Now()
    {
        TimeZoneInfo zoneInfo = TZConvert.GetTimeZoneInfo(TimeZone);
        return TimeZoneInfo.ConvertTime(DateTime.Now, zoneInfo);
    }
}
