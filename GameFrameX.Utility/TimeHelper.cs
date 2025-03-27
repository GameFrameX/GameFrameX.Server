namespace GameFrameX.Utility;

/// <summary>
/// 时间帮助工具类
/// </summary>
public static class TimeHelper
{
    /// <summary>
    /// 1970-01-01 00:00:00 本地时间
    /// </summary>
    public static readonly DateTime EpochLocal = TimeZoneInfo.ConvertTime(new DateTime(1970, 1, 1), TimeZoneInfo.Local);

    /// <summary>
    /// 1970-01-01 00:00:00 UTC 时间
    /// </summary>
    public static readonly DateTime EpochUtc = TimeZoneInfo.ConvertTime(new DateTime(1970, 1, 1), TimeZoneInfo.Utc);

    /// <summary>
    /// 返回当前时间的毫秒表示。
    /// </summary>
    /// <returns>当前时间的毫秒数。</returns>
    public static long CurrentTimeMillis()
    {
        return TimeMillis(DateTime.Now);
    }

    /// <summary>
    /// 当前UTC 时间 秒时间戳
    /// </summary>
    /// <returns>当前UTC时间的秒时间戳。</returns>
    public static long UnixTimeSeconds()
    {
        return new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();
    }

    /// <summary>
    /// 当前UTC 时间 毫秒时间戳
    /// </summary>
    /// <returns>当前UTC时间的毫秒时间戳。</returns>
    public static long UnixTimeMilliseconds()
    {
        return new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds();
    }


    /// <summary>
    /// 当前时区时间 毫秒时间戳
    /// </summary>
    /// <returns>当前时区时间的毫秒时间戳。</returns>
    public static long TimeMilliseconds()
    {
        return new DateTimeOffset(DateTime.Now).ToUnixTimeMilliseconds();
    }

    /// <summary>
    /// 获取指定时间距离纪元时间（本地时间或UTC时间）的毫秒数。
    /// </summary>
    /// <param name="time">指定时间。</param>
    /// <param name="utc">是否使用UTC时间。</param>
    /// <returns>距离纪元时间的毫秒数。</returns>
    public static long TimeMillis(DateTime time, bool utc = false)
    {
        if (utc)
        {
            return (long)(time - EpochUtc).TotalMilliseconds;
        }

        return (long)(time - EpochLocal).TotalMilliseconds;
    }

    /// <summary>
    /// 获取指定时间距离纪元时间（本地时间或UTC时间）的秒数。
    /// </summary>
    /// <param name="time">指定时间。</param>
    /// <param name="utc">是否使用UTC时间。</param>
    /// <returns>距离纪元时间的秒数。</returns>
    public static int TimeSecond(DateTime time, bool utc = false)
    {
        if (utc)
        {
            return (int)(time - EpochUtc).TotalSeconds;
        }

        return (int)(time - EpochLocal).TotalSeconds;
    }

    /// <summary>
    /// 当前时区时间 秒时间戳
    /// </summary>
    /// <returns>当前时区时间的秒时间戳。</returns>
    public static long TimeSeconds()
    {
        return new DateTimeOffset(DateTime.Now).ToUnixTimeSeconds();
    }

    /// <summary>
    /// 将Unix时间戳转换为自公元1年1月1日以来的刻度数。
    /// </summary>
    /// <param name="timestampSeconds">Unix时间戳，从1970年1月1日以来的秒数。</param>
    /// <returns>自公元1年1月1日以来的刻度数。</returns>
    public static long TimestampToTicks(long timestampSeconds)
    {
        // 将Unix时间戳转换为刻度数，每秒等于10000000刻度
        // 621355968000000000是公元1年1月1日至1970年1月1日的刻度数差值
        return timestampSeconds * 10000000L + 621355968000000000L;
    }

    /// <summary>
    /// 将Unix毫秒时间戳转换为自公元1年1月1日以来的刻度数。
    /// </summary>
    /// <param name="timestampMillisSeconds">Unix毫秒时间戳，从1970年1月1日以来的毫秒数。</param>
    /// <returns>自公元1年1月1日以来的刻度数。</returns>
    public static long TimestampMillisToTicks(long timestampMillisSeconds)
    {
        // 将Unix毫秒时间戳转换为刻度数，每毫秒等于10000000000刻度
        // 621355968000000000是公元1年1月1日至1970年1月1日的刻度数差值
        return timestampMillisSeconds * 10000L + 621355968000000000L;
    }

    /// <summary>
    /// 将给定的时间戳转换为相对于当前时间的 TimeSpan 对象。
    /// </summary>
    /// <param name="timestamp">自某个固定时间点（通常为1970年1月1日午夜）以来经过的毫秒数。</param>
    /// <returns>一个 TimeSpan 对象，表示从给定时间戳到当前时间的间隔。</returns>
    public static TimeSpan TimeSpanWithTimestamp(long timestamp)
    {
        // 计算当前时间与给定时间戳表示的时间之间的差值
        var timeSpan = MillisToDateTime(UnixTimeMilliseconds(), true) - MillisToDateTime(timestamp, true);
        return timeSpan;
    }

    /// <summary>
    /// 毫秒转时间
    /// </summary>
    /// <param name="time">毫秒数。</param>
    /// <param name="utc">是否使用UTC时间。</param>
    /// <returns>转换后的时间。</returns>
    public static DateTime MillisToDateTime(long time, bool utc = false)
    {
        if (utc)
        {
            return EpochUtc.AddMilliseconds(time);
        }

        return EpochLocal.AddMilliseconds(time);
    }

    /// <summary>
    /// 获取从指定日期到当前日期之间跨越的天数。
    /// </summary>
    /// <param name="begin">起始日期。</param>
    /// <param name="hour">小时。</param>
    /// <returns>跨越的天数。</returns>
    public static int GetCrossDays(DateTime begin, int hour = 0)
    {
        return GetCrossDays(begin, DateTime.Now, hour);
    }

    /// <summary>
    /// 获取两个日期之间跨越的天数。
    /// </summary>
    /// <param name="begin">起始日期。</param>
    /// <param name="after">结束日期。</param>
    /// <param name="hour">小时。</param>
    /// <returns>跨越的天数。</returns>
    public static int GetCrossDays(DateTime begin, DateTime after, int hour = 0)
    {
        var days = (int)(after.Date - begin.Date).TotalDays;
        if (begin.Hour < hour)
        {
            days++;
        }

        if (after.Hour < hour)
        {
            days--;
        }

        return days;
    }

    /// <summary>
    /// 判断当前时间是否与指定时间处于同一周。
    /// </summary>
    /// <param name="start">指定时间的起始时间。</param>
    /// <returns>如果当前时间与指定时间处于同一周，则为 true；否则为 false。</returns>
    public static bool IsNowSameWeek(long start)
    {
        return IsNowSameWeek(new DateTime(start));
    }

    /// <summary>
    /// 判断当前时间是否与指定时间处于同一周。
    /// </summary>
    /// <param name="start">指定时间的起始时间。</param>
    /// <returns>如果当前时间与指定时间处于同一周，则为 true；否则为 false。</returns>
    public static bool IsNowSameWeek(DateTime start)
    {
        return IsSameWeek(start, DateTime.Now);
    }

    /// <summary>
    /// 判断两个时间是否处于同一周。
    /// </summary>
    /// <param name="start">起始时间。</param>
    /// <param name="end">结束时间。</param>
    /// <returns>如果两个时间处于同一周，则为 true；否则为 false。</returns>
    public static bool IsSameWeek(DateTime start, DateTime end)
    {
        // 让start是较早的时间
        if (start > end)
        {
            (start, end) = (end, start);
        }

        var dayOfWeek = (int)start.DayOfWeek;
        if (dayOfWeek == (int)DayOfWeek.Sunday)
        {
            dayOfWeek = 7;
        }

        // 获取较早时间所在星期的星期天的0点
        var startsWeekLastDate = start.AddDays(7 - dayOfWeek).Date;
        // 判断end是否在start所在周
        return startsWeekLastDate >= end.Date;
    }

    /// <summary>
    /// 获取指定日期所在星期的时间。
    /// </summary>
    /// <param name="dateTime">指定日期。</param>
    /// <param name="day">星期几。</param>
    /// <returns>指定日期所在星期的时间。</returns>
    public static DateTime GetDayOfWeekTime(DateTime dateTime, DayOfWeek day)
    {
        var dd = (int)day;
        if (dd == 0)
        {
            dd = 7;
        }

        var dayOfWeek = (int)dateTime.DayOfWeek;
        if (dayOfWeek == 0)
        {
            dayOfWeek = 7;
        }

        return dateTime.AddDays(dd - dayOfWeek).Date;
    }

    /// <summary>
    /// 获取当前日期所在星期的时间。
    /// </summary>
    /// <param name="day">星期几。</param>
    /// <returns>当前日期所在星期的时间。</returns>
    public static DateTime GetDayOfWeekTime(DayOfWeek day)
    {
        return GetDayOfWeekTime(DateTime.Now, day);
    }

    /// <summary>
    /// 获取指定星期在中国的对应数字。
    /// </summary>
    /// <param name="day">星期几。</param>
    /// <returns>星期在中国的对应数字。</returns>
    public static int GetChinaDayOfWeek(DayOfWeek day)
    {
        var dayOfWeek = (int)day;
        if (dayOfWeek == 0)
        {
            dayOfWeek = 7;
        }

        return dayOfWeek;
    }

    /// <summary>
    /// 获取当前星期在中国的对应数字。
    /// </summary>
    /// <returns>当前星期在中国的对应数字。</returns>
    public static int GetChinaDayOfWeek()
    {
        return GetChinaDayOfWeek(DateTime.Now.DayOfWeek);
    }

    /// <summary>
    /// 获取当前本地时区的日期，格式为yyyyMMdd的整数
    /// </summary>
    /// <returns>返回一个8位整数，表示当前本地时区的日期。例如：20231225表示2023年12月25日</returns>
    public static int CurrentTimeWithDay()
    {
        return Convert.ToInt32(DateTime.Now.ToString("yyyyMMdd"));
    }

    /// <summary>
    /// 获取当前UTC时区的日期，格式为yyyyMMdd的整数
    /// </summary>
    /// <returns>返回一个8位整数，表示当前UTC时区的日期。例如：20231225表示2023年12月25日</returns>
    public static int CurrentTimeWithUtcDay()
    {
        return Convert.ToInt32(DateTime.UtcNow.ToString("yyyyMMdd"));
    }

    /// <summary>
    /// 获取当前本地时区时间的完整格式字符串
    /// </summary>
    /// <returns>返回格式为"yyyy-MM-dd-HH-mm-ss.fff K"的时间字符串，包含年-月-日-时-分-秒.毫秒 时区偏移。例如："2023-12-25-14-30-45.123 +08:00"</returns>
    public static string CurrentTimeWithFullString()
    {
        return DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss.fff K");
    }

    /// <summary>
    /// 获取当前UTC时区时间的完整格式字符串
    /// </summary>
    /// <returns>返回格式为"yyyy-MM-dd-HH-mm-ss.fff K"的UTC时间字符串，包含年-月-日-时-分-秒.毫秒 时区偏移。例如："2023-12-25-06-30-45.123 +00:00"</returns>
    public static string CurrentTimeWithUtcFullString()
    {
        return DateTime.UtcNow.ToString("yyyy-MM-dd-HH-mm-ss.fff K");
    }

    /// <summary>
    /// UTC 时间戳 转换成UTC时间
    /// </summary>
    /// <param name="utcTimestamp">UTC时间戳,单位秒</param>
    /// <returns>转换后的UTC时间。</returns>
    public static DateTime UtcToUtcDateTime(long utcTimestamp)
    {
        return DateTimeOffset.FromUnixTimeSeconds(utcTimestamp).UtcDateTime;
    }

    /// <summary>
    /// UTC 时间戳 转换成本地时间
    /// </summary>
    /// <param name="utcTimestamp">UTC时间戳,单位秒</param>
    /// <returns>转换后的本地时间。</returns>
    public static DateTime UtcToLocalDateTime(long utcTimestamp)
    {
        return DateTimeOffset.FromUnixTimeSeconds(utcTimestamp).LocalDateTime;
    }

    /// <summary>
    /// 按照UTC时间判断两个时间戳是否是同一天
    /// </summary>
    /// <param name="timestamp1">时间戳1</param>
    /// <param name="timestamp2">时间戳2</param>
    /// <returns>是否是同一天</returns>
    public static bool IsUnixSameDay(long timestamp1, long timestamp2)
    {
        var time1 = UtcToUtcDateTime(timestamp1);
        var time2 = UtcToUtcDateTime(timestamp2);
        return IsSameDay(time1, time2);
    }

    /// <summary>
    /// 获取今天开始时间
    /// </summary>
    /// <returns>今天零点时间</returns>
    public static DateTime GetTodayStartTime()
    {
        return DateTime.Today;
    }

    /// <summary>
    /// 获取今天开始时间戳
    /// </summary>
    /// <returns>今天零点时间戳(秒)</returns>
    public static long GetTodayStartTimestamp()
    {
        return new DateTimeOffset(GetTodayStartTime()).ToUnixTimeSeconds();
    }

    /// <summary>
    /// 获取今天结束时间
    /// </summary>
    /// <returns>今天23:59:59的时间</returns>
    public static DateTime GetTodayEndTime()
    {
        return DateTime.Today.AddDays(1).AddSeconds(-1);
    }

    /// <summary>
    /// 获取今天结束时间戳
    /// </summary>
    /// <returns>今天23:59:59的时间戳(秒)</returns>
    public static long GetTodayEndTimestamp()
    {
        return new DateTimeOffset(GetTodayEndTime()).ToUnixTimeSeconds();
    }

    /// <summary>
    /// 获取本周开始时间
    /// </summary>
    /// <returns>本周一零点时间</returns>
    public static DateTime GetWeekStartTime()
    {
        var now = DateTime.Now;
        var dayOfWeek = (int)now.DayOfWeek;
        dayOfWeek = dayOfWeek == 0 ? 7 : dayOfWeek;
        return now.AddDays(1 - dayOfWeek).Date;
    }

    /// <summary>
    /// 获取本周开始时间戳
    /// </summary>
    /// <returns>本周一零点时间戳(秒)</returns>
    public static long GetWeekStartTimestamp()
    {
        return new DateTimeOffset(GetWeekStartTime()).ToUnixTimeSeconds();
    }

    /// <summary>
    /// 获取本周结束时间
    /// </summary>
    /// <returns>本周日23:59:59的时间</returns>
    public static DateTime GetWeekEndTime()
    {
        return GetWeekStartTime().AddDays(7).AddSeconds(-1);
    }

    /// <summary>
    /// 获取本周结束时间戳
    /// </summary>
    /// <returns>本周日23:59:59的时间戳(秒)</returns>
    public static long GetWeekEndTimestamp()
    {
        return new DateTimeOffset(GetWeekEndTime()).ToUnixTimeSeconds();
    }

    /// <summary>
    /// 获取本月开始时间
    /// </summary>
    /// <returns>本月1号零点时间</returns>
    public static DateTime GetMonthStartTime()
    {
        return new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
    }

    /// <summary>
    /// 获取本月开始时间戳
    /// </summary>
    /// <returns>本月1号零点时间戳(秒)</returns>
    public static long GetMonthStartTimestamp()
    {
        return new DateTimeOffset(GetMonthStartTime()).ToUnixTimeSeconds();
    }

    /// <summary>
    /// 获取本月结束时间
    /// </summary>
    /// <returns>本月最后一天23:59:59的时间</returns>
    public static DateTime GetMonthEndTime()
    {
        return GetMonthStartTime().AddMonths(1).AddSeconds(-1);
    }

    /// <summary>
    /// 获取本月结束时间戳
    /// </summary>
    /// <returns>本月最后一天23:59:59的时间戳(秒)</returns>
    public static long GetMonthEndTimestamp()
    {
        return new DateTimeOffset(GetMonthEndTime()).ToUnixTimeSeconds();
    }

    /// <summary>
    /// 获取本年开始时间
    /// </summary>
    /// <returns>本年1月1日零点时间</returns>
    public static DateTime GetYearStartTime()
    {
        return new DateTime(DateTime.Now.Year, 1, 1);
    }

    /// <summary>
    /// 获取本年开始时间戳
    /// </summary>
    /// <returns>本年1月1日零点时间戳(秒)</returns>
    public static long GetYearStartTimestamp()
    {
        return new DateTimeOffset(GetYearStartTime()).ToUnixTimeSeconds();
    }

    /// <summary>
    /// 获取本年结束时间
    /// </summary>
    /// <returns>本年12月31日23:59:59的时间</returns>
    public static DateTime GetYearEndTime()
    {
        return GetYearStartTime().AddYears(1).AddSeconds(-1);
    }

    /// <summary>
    /// 获取本年结束时间戳
    /// </summary>
    /// <returns>本年12月31日23:59:59的时间戳(秒)</returns>
    public static long GetYearEndTimestamp()
    {
        return new DateTimeOffset(GetYearEndTime()).ToUnixTimeSeconds();
    }

    /// <summary>
    /// 获取指定日期的开始时间
    /// </summary>
    /// <param name="date">指定日期</param>
    /// <returns>指定日期零点时间</returns>
    public static DateTime GetStartTimeOfDay(DateTime date)
    {
        return date.Date;
    }

    /// <summary>
    /// 获取指定日期的开始时间戳
    /// </summary>
    /// <param name="date">指定日期</param>
    /// <returns>指定日期零点时间戳(秒)</returns>
    public static long GetStartTimestampOfDay(DateTime date)
    {
        return new DateTimeOffset(GetStartTimeOfDay(date)).ToUnixTimeSeconds();
    }

    /// <summary>
    /// 获取指定日期的结束时间
    /// </summary>
    /// <param name="date">指定日期</param>
    /// <returns>指定日期23:59:59的时间</returns>
    public static DateTime GetEndTimeOfDay(DateTime date)
    {
        return date.Date.AddDays(1).AddSeconds(-1);
    }

    /// <summary>
    /// 获取指定日期的结束时间戳
    /// </summary>
    /// <param name="date">指定日期</param>
    /// <returns>指定日期23:59:59的时间戳(秒)</returns>
    public static long GetEndTimestampOfDay(DateTime date)
    {
        return new DateTimeOffset(GetEndTimeOfDay(date)).ToUnixTimeSeconds();
    }

    /// <summary>
    /// 获取指定日期所在周的开始时间
    /// </summary>
    /// <param name="date">指定日期</param>
    /// <returns>所在周周一零点时间</returns>
    public static DateTime GetStartTimeOfWeek(DateTime date)
    {
        var dayOfWeek = (int)date.DayOfWeek;
        dayOfWeek = dayOfWeek == 0 ? 7 : dayOfWeek;
        return date.AddDays(1 - dayOfWeek).Date;
    }

    /// <summary>
    /// 获取指定日期所在周的开始时间戳
    /// </summary>
    /// <param name="date">指定日期</param>
    /// <returns>所在周周一零点时间戳(秒)</returns>
    public static long GetStartTimestampOfWeek(DateTime date)
    {
        return new DateTimeOffset(GetStartTimeOfWeek(date)).ToUnixTimeSeconds();
    }

    /// <summary>
    /// 获取指定日期所在周的结束时间
    /// </summary>
    /// <param name="date">指定日期</param>
    /// <returns>所在周周日23:59:59的时间</returns>
    public static DateTime GetEndTimeOfWeek(DateTime date)
    {
        return GetStartTimeOfWeek(date).AddDays(7).AddSeconds(-1);
    }

    /// <summary>
    /// 获取指定日期所在周的结束时间戳
    /// </summary>
    /// <param name="date">指定日期</param>
    /// <returns>所在周周日23:59:59的时间戳(秒)</returns>
    public static long GetEndTimestampOfWeek(DateTime date)
    {
        return new DateTimeOffset(GetEndTimeOfWeek(date)).ToUnixTimeSeconds();
    }

    /// <summary>
    /// 获取指定日期所在月的开始时间
    /// </summary>
    /// <param name="date">指定日期</param>
    /// <returns>所在月1号零点时间</returns>
    public static DateTime GetStartTimeOfMonth(DateTime date)
    {
        return new DateTime(date.Year, date.Month, 1);
    }

    /// <summary>
    /// 获取指定日期所在月的开始时间戳
    /// </summary>
    /// <param name="date">指定日期</param>
    /// <returns>所在月1号零点时间戳(秒)</returns>
    public static long GetStartTimestampOfMonth(DateTime date)
    {
        return new DateTimeOffset(GetStartTimeOfMonth(date)).ToUnixTimeSeconds();
    }

    /// <summary>
    /// 获取指定日期所在月的结束时间
    /// </summary>
    /// <param name="date">指定日期</param>
    /// <returns>所在月最后一天23:59:59的时间</returns>
    public static DateTime GetEndTimeOfMonth(DateTime date)
    {
        return GetStartTimeOfMonth(date).AddMonths(1).AddSeconds(-1);
    }

    /// <summary>
    /// 获取指定日期所在月的结束时间戳
    /// </summary>
    /// <param name="date">指定日期</param>
    /// <returns>所在月最后一天23:59:59的时间戳(秒)</returns>
    public static long GetEndTimestampOfMonth(DateTime date)
    {
        return new DateTimeOffset(GetEndTimeOfMonth(date)).ToUnixTimeSeconds();
    }

    /// <summary>
    /// 获取指定日期所在年的开始时间
    /// </summary>
    /// <param name="date">指定日期</param>
    /// <returns>所在年1月1日零点时间</returns>
    public static DateTime GetStartTimeOfYear(DateTime date)
    {
        return new DateTime(date.Year, 1, 1);
    }

    /// <summary>
    /// 获取指定日期所在年的开始时间戳
    /// </summary>
    /// <param name="date">指定日期</param>
    /// <returns>所在年1月1日零点时间戳(秒)</returns>
    public static long GetStartTimestampOfYear(DateTime date)
    {
        return new DateTimeOffset(GetStartTimeOfYear(date)).ToUnixTimeSeconds();
    }

    /// <summary>
    /// 获取指定日期所在年的结束时间
    /// </summary>
    /// <param name="date">指定日期</param>
    /// <returns>所在年12月31日23:59:59的时间</returns>
    public static DateTime GetEndTimeOfYear(DateTime date)
    {
        return GetStartTimeOfYear(date).AddYears(1).AddSeconds(-1);
    }

    /// <summary>
    /// 获取指定日期所在年的结束时间戳
    /// </summary>
    /// <param name="date">指定日期</param>
    /// <returns>所在年12月31日23:59:59的时间戳(秒)</returns>
    public static long GetEndTimestampOfYear(DateTime date)
    {
        return new DateTimeOffset(GetEndTimeOfYear(date)).ToUnixTimeSeconds();
    }

    /// <summary>
    /// 获取两个时间之间的间隔天数
    /// </summary>
    /// <param name="startTime">开始时间</param>
    /// <param name="endTime">结束时间</param>
    /// <returns>间隔天数</returns>
    public static int GetDaysBetween(DateTime startTime, DateTime endTime)
    {
        return (int)(endTime.Date - startTime.Date).TotalDays;
    }

    /// <summary>
    /// 获取两个UTC时间戳之间的间隔天数
    /// </summary>
    /// <param name="startTimestamp">开始时间戳(秒)</param>
    /// <param name="endTimestamp">结束时间戳(秒)</param>
    /// <returns>间隔天数</returns>
    public static int GetDaysBetween(long startTimestamp, long endTimestamp)
    {
        var startTime = UtcToUtcDateTime(startTimestamp);
        var endTime = UtcToUtcDateTime(endTimestamp);
        return GetDaysBetween(startTime, endTime);
    }

    /// <summary>
    /// 获取两个本地时间戳之间的间隔天数
    /// </summary>
    /// <param name="startTimestamp">开始时间戳(秒)</param>
    /// <param name="endTimestamp">结束时间戳(秒)</param>
    /// <returns>间隔天数</returns>
    public static int GetLocalDaysBetween(long startTimestamp, long endTimestamp)
    {
        var startTime = UtcToLocalDateTime(startTimestamp);
        var endTime = UtcToLocalDateTime(endTimestamp);
        return GetDaysBetween(startTime, endTime);
    }

    /// <summary>
    /// 获取指定时间是否在指定的时间范围内
    /// </summary>
    /// <param name="time">指定时间</param>
    /// <param name="startTime">开始时间</param>
    /// <param name="endTime">结束时间</param>
    /// <returns>是否在范围内</returns>
    public static bool IsTimeInRange(DateTime time, DateTime startTime, DateTime endTime)
    {
        return time >= startTime && time <= endTime;
    }

    /// <summary>
    /// 获取指定时间戳是否在指定的时间戳范围内
    /// </summary>
    /// <param name="timestamp">指定时间戳</param>
    /// <param name="startTimestamp">开始时间戳</param>
    /// <param name="endTimestamp">结束时间戳</param>
    /// <returns>是否在范围内</returns>
    public static bool IsTimestampInRange(long timestamp, long startTimestamp, long endTimestamp)
    {
        return timestamp >= startTimestamp && timestamp <= endTimestamp;
    }

    /// <summary>
    /// 按照本地时间判断两个时间戳是否是同一天
    /// </summary>
    /// <param name="timestamp1">时间戳1</param>
    /// <param name="timestamp2">时间戳2</param>
    /// <returns>是否是同一天</returns>
    public static bool IsLocalSameDay(long timestamp1, long timestamp2)
    {
        var time1 = UtcToLocalDateTime(timestamp1);
        var time2 = UtcToLocalDateTime(timestamp2);
        return IsSameDay(time1, time2);
    }

    /// <summary>
    /// 判断两个时间是否是同一天
    /// </summary>
    /// <param name="time1">时间1</param>
    /// <param name="time2">时间2</param>
    /// <returns>是否是同一天</returns>
    public static bool IsSameDay(DateTime time1, DateTime time2)
    {
        return time1.Date.Year == time2.Date.Year && time1.Date.Month == time2.Date.Month && time1.Date.Day == time2.Date.Day;
    }
}