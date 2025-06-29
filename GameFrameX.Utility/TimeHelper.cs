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
    /// 时区偏移秒数。用于调整时间计算的偏移量。
    /// 正值表示向未来偏移,负值表示向过去偏移。
    /// </summary>
    public static long TimeOffsetSeconds { get; private set; } = 0;

    /// <summary>
    /// 时区偏移毫秒数。用于调整时间计算的偏移量。
    /// 正值表示向未来偏移,负值表示向过去偏移。
    /// </summary>
    public static long TimeOffsetMilliseconds { get; private set; } = 0;

    /// <summary>
    /// 设置时区偏移值
    /// </summary>
    /// <param name="offsetSeconds">秒级偏移量</param>
    /// <param name="offsetMilliseconds">毫秒级偏移量</param>
    /// <remarks>
    /// 此方法用于调整时间计算的基准。
    /// 例如要模拟未来时间,可以传入正数;要模拟过去时间,可以传入负数。
    /// 通常用于调试和测试场景。
    /// </remarks>
    public static void SetTimeOffset(long offsetSeconds, long offsetMilliseconds)
    {
        TimeOffsetSeconds = offsetSeconds;
        TimeOffsetMilliseconds = offsetMilliseconds;
    }

    /// <summary>
    /// 重置时区偏移值为默认值(0)
    /// </summary>
    /// <remarks>
    /// 此方法会将秒级和毫秒级的偏移量都重置为0,
    /// 使时间计算恢复到未经调整的状态。
    /// </remarks>
    public static void ResetTimeOffset()
    {
        TimeOffsetSeconds = default;
        TimeOffsetMilliseconds = default;
    }

    /// <summary>
    /// 获取当前UTC时间的秒级时间戳
    /// </summary>
    /// <returns>返回自1970年1月1日 00:00:00 UTC以来经过的秒数,加上时区偏移量</returns>
    /// <remarks>
    /// 此方法:
    /// 1. 获取当前UTC时间
    /// 2. 转换为Unix时间戳(秒)
    /// 3. 加上TimeOffsetSeconds偏移量
    /// 主要用于需要UTC时间戳的场景,如跨时区业务
    /// </remarks>
    public static long UnixTimeSeconds()
    {
        return new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds() + TimeOffsetSeconds;
    }

    /// <summary>
    /// 获取当前UTC时间的毫秒级时间戳
    /// </summary>
    /// <returns>返回自1970年1月1日 00:00:00 UTC以来经过的毫秒数,加上时区偏移量</returns>
    /// <remarks>
    /// 此方法:
    /// 1. 获取当前UTC时间
    /// 2. 转换为Unix时间戳(毫秒)
    /// 3. 加上TimeOffsetMilliseconds偏移量
    /// 相比秒级时间戳提供更高的精度,适用于需要精确时间计算的场景
    /// </remarks>
    public static long UnixTimeMilliseconds()
    {
        return new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds() + TimeOffsetMilliseconds;
    }

    /// <summary>
    /// 获取当前本地时区时间的秒级时间戳
    /// </summary>
    /// <returns>返回自1970年1月1日 00:00:00以来经过的秒数(本地时区),加上时区偏移量</returns>
    /// <remarks>
    /// 此方法:
    /// 1. 获取当前本地时区时间
    /// 2. 转换为Unix时间戳(秒)
    /// 3. 加上TimeOffsetSeconds偏移量
    /// 主要用于需要本地时区时间戳的场景
    /// </remarks>
    public static long TimeSeconds()
    {
        return new DateTimeOffset(DateTime.Now).ToUnixTimeSeconds() + TimeOffsetSeconds;
    }

    /// <summary>
    /// 获取当前本地时区时间的毫秒级时间戳
    /// </summary>
    /// <returns>返回自1970年1月1日 00:00:00以来经过的毫秒数(本地时区),加上时区偏移量</returns>
    /// <remarks>
    /// 此方法:
    /// 1. 获取当前本地时区时间
    /// 2. 转换为Unix时间戳(毫秒)
    /// 3. 加上TimeOffsetMilliseconds偏移量
    /// 相比秒级时间戳提供更高的精度,适用于需要精确时间计算的场景
    /// </remarks>
    public static long TimeMilliseconds()
    {
        return new DateTimeOffset(DateTime.Now).ToUnixTimeMilliseconds() + TimeOffsetMilliseconds;
    }

    /// <summary>
    /// 获取指定时间距离纪元时间（本地时间或UTC时间）的毫秒数。
    /// </summary>
    /// <param name="time">指定时间。</param>
    /// <param name="utc">是否使用UTC时间。</param>
    /// <returns>距离纪元时间的毫秒数。</returns>
    public static long TimeToMilliseconds(DateTime time, bool utc = false)
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
    public static long TimeToSecond(DateTime time, bool utc = false)
    {
        if (utc)
        {
            return (long)(time - EpochUtc).TotalSeconds;
        }

        return (long)(time - EpochLocal).TotalSeconds;
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
        var timeSpan = MillisecondsTimeStampToDateTime(UnixTimeMilliseconds(), true) - MillisecondsTimeStampToDateTime(timestamp, true);
        return timeSpan;
    }

    /// <summary>
    /// 将给定的时间戳转换为相对于当前本地时间的 TimeSpan 对象。
    /// </summary>
    /// <param name="timestamp">自某个固定时间点（通常为1970年1月1日午夜）以来经过的毫秒数。</param>
    /// <returns>一个 TimeSpan 对象，表示从给定时间戳到当前本地时间的间隔。</returns>
    public static TimeSpan TimeSpanLocalWithTimestamp(long timestamp)
    {
        // 计算当前时间与给定时间戳表示的时间之间的差值
        var timeSpan = DateTime.Now - MillisecondsTimeStampToDateTime(timestamp, true);
        return timeSpan;
    }

    /// <summary>
    /// 毫秒转时间
    /// </summary>
    /// <param name="timestamp">毫秒时间戳。</param>
    /// <param name="utc">是否使用UTC时间。</param>
    /// <returns>转换后的时间。</returns>
    public static DateTime MillisecondsTimeStampToDateTime(long timestamp, bool utc = false)
    {
        if (utc)
        {
            return EpochUtc.AddMilliseconds(timestamp);
        }

        return EpochLocal.AddMilliseconds(timestamp);
    }

    /// <summary>
    /// 秒时间戳转时间
    /// </summary>
    /// <param name="timestamp">秒时间戳。</param>
    /// <param name="utc">是否使用UTC时间。</param>
    /// <returns>转换后的时间。</returns>
    public static DateTime TimestampToDateTime(long timestamp, bool utc = false)
    {
        if (utc)
        {
            return EpochUtc.AddSeconds(timestamp);
        }

        return EpochLocal.AddSeconds(timestamp);
    }

    /// <summary>
    /// 获取从指定日期到当前UTC日期之间跨越的天数。
    /// </summary>
    /// <param name="startTime">起始日期。</param>
    /// <param name="hour">小时。</param>
    /// <returns>跨越的天数。</returns>
    public static int GetCrossDays(DateTime startTime, int hour = 0)
    {
        return GetCrossDays(startTime, DateTime.UtcNow, hour);
    }

    /// <summary>
    /// 获取从指定日期到当前本地日期之间跨越的天数。
    /// </summary>
    /// <param name="startTime">起始日期。</param>
    /// <param name="hour">小时。</param>
    /// <returns>跨越的天数。</returns>
    public static int GetCrossLocalDays(DateTime startTime, int hour = 0)
    {
        return GetCrossDays(startTime, DateTime.Now, hour);
    }

    /// <summary>
    /// 获取两个时间戳之间跨越的天数。
    /// </summary>
    /// <param name="beginTimestamp">起始时间戳,从1970年1月1日以来经过的秒数。</param>
    /// <param name="hour">小时。</param>
    /// <returns>跨越的天数。</returns>
    public static int GetCrossDays(long beginTimestamp, int hour = 0)
    {
        var begin = TimestampToDateTime(beginTimestamp);
        return GetCrossDays(begin, hour);
    }

    /// <summary>
    /// 获取两个UTC时间戳之间跨越的天数。
    /// </summary>
    /// <param name="beginTimestamp">开始时间戳(秒)，从1970年1月1日以来经过的秒数。</param>
    /// <param name="afterTimestamp">结束时间戳(秒)，从1970年1月1日以来经过的秒数。</param>
    /// <param name="hour">小时。</param>
    /// <returns>跨越的天数。</returns>
    public static int GetCrossDays(long beginTimestamp, long afterTimestamp, int hour = 0)
    {
        var begin = UtcToUtcDateTime(beginTimestamp);
        var after = UtcToUtcDateTime(afterTimestamp);
        return GetCrossDays(begin, after, hour);
    }

    /// <summary>
    /// 获取两个日期之间跨越的天数。
    /// </summary>
    /// <param name="startTime">起始日期。</param>
    /// <param name="endTime">结束日期。</param>
    /// <param name="hour">小时。</param>
    /// <returns>跨越的天数。</returns>
    public static int GetCrossDays(DateTime startTime, DateTime endTime, int hour = 0)
    {
        var days = (int)(endTime.Date - startTime.Date).TotalDays;
        if (startTime.Hour < hour)
        {
            days++;
        }

        if (endTime.Hour < hour)
        {
            days--;
        }

        return days;
    }

    /// <summary>
    /// 获取两个本地时间戳之间的间隔天数
    /// </summary>
    /// <param name="startTimestamp">开始时间戳(秒),UTC时间戳将被转换为本地时间</param>
    /// <param name="endTimestamp">结束时间戳(秒),UTC时间戳将被转换为本地时间</param>
    /// <returns>间隔天数,如果开始时间晚于结束时间,返回负数</returns>
    /// <remarks>
    /// 此方法会先将UTC时间戳转换为本地时间,然后计算两个本地时间之间的天数差
    /// 计算时会考虑日期的时分秒部分
    /// </remarks>
    public static int GetCrossLocalDays(long startTimestamp, long endTimestamp)
    {
        var startTime = UtcToLocalDateTime(startTimestamp);
        var endTime = UtcToLocalDateTime(endTimestamp);
        return GetCrossDays(startTime, endTime);
    }

    /// <summary>
    /// 判断当前时间是否与指定时间处于同一周。
    /// </summary>
    /// <param name="ticks">指定时间的起始时间(Ticks)。表示自 0001 年 1 月 1 日午夜 00:00:00 以来所经过的时钟周期数</param>
    /// <returns>如果当前时间与指定时间处于同一周，则为 true；否则为 false。</returns>
    /// <remarks>
    /// 此方法将传入的ticks转换为DateTime后与当前时间比较是否在同一周
    /// 使用系统默认的周计算规则(周日为每周第一天)
    /// </remarks>
    public static bool IsNowSameWeek(long ticks)
    {
        return IsNowSameWeek(new DateTime(ticks));
    }

    /// <summary>
    /// 判断当前时间是否与指定时间处于同一周。
    /// 以周一为每周的第一天,周日为每周的最后一天。
    /// 使用本地时间(DateTime.Now)进行比较。
    /// </summary>
    /// <param name="start">指定时间的起始时间。可以是任意DateTime值。</param>
    /// <returns>如果当前时间与指定时间处于同一周，则为 true；否则为 false。</returns>
    /// <remarks>
    /// 此方法将调用IsSameWeek方法进行实际比较。
    /// 使用本地时区时间作为当前时间参考点。
    /// </remarks>
    public static bool IsNowSameWeek(DateTime start)
    {
        return IsSameWeek(start, DateTime.Now);
    }

    /// <summary>
    /// 判断两个时间是否处于同一周。
    /// 以周一为每周的第一天,周日为每周的最后一天。
    /// </summary>
    /// <param name="start">起始时间。可以是任意DateTime值。</param>
    /// <param name="end">结束时间。可以是任意DateTime值。</param>
    /// <returns>如果两个时间处于同一周，则为 true；否则为 false。</returns>
    /// <remarks>
    /// 此方法会自动调整参数顺序,确保start是较早的时间。
    /// 通过计算较早时间所在周的周日时间点,判断另一个时间是否在同一周内。
    /// </remarks>
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
    /// 判断当前UTC时间是否与指定时间处于同一周。
    /// 以周一为每周的第一天,周日为每周的最后一天。
    /// 使用UTC时间(DateTime.UtcNow)进行比较。
    /// </summary>
    /// <param name="start">指定时间的起始时间。可以是任意DateTime值。</param>
    /// <returns>如果当前UTC时间与指定时间处于同一周，则为 true；否则为 false。</returns>
    /// <remarks>
    /// 此方法将调用IsSameWeek方法进行实际比较。
    /// 使用UTC时区时间作为当前时间参考点，避免时区差异影响。
    /// </remarks>
    public static bool IsNowSameWeekUtc(DateTime start)
    {
        return IsSameWeek(start, DateTime.UtcNow);
    }

    /// <summary>
    /// 判断当前UTC时间是否与指定时间戳处于同一周。
    /// </summary>
    /// <param name="ticks">指定时间的起始时间(Ticks)。表示自 0001 年 1 月 1 日午夜 00:00:00 以来所经过的时钟周期数</param>
    /// <returns>如果当前UTC时间与指定时间处于同一周，则为 true；否则为 false。</returns>
    /// <remarks>
    /// 此方法将传入的ticks转换为DateTime后与当前UTC时间比较是否在同一周
    /// 使用UTC时区时间作为当前时间参考点，避免时区差异影响
    /// </remarks>
    public static bool IsUnixSameWeek(long ticks)
    {
        return IsNowSameWeekUtc(new DateTime(ticks));
    }

    /// <summary>
    /// 判断当前UTC时间是否与指定Unix时间戳处于同一周。
    /// </summary>
    /// <param name="timestampSeconds">指定时间的Unix时间戳(秒)。表示自1970年1月1日00:00:00 UTC以来的秒数</param>
    /// <returns>如果当前UTC时间与指定时间处于同一周，则为 true；否则为 false。</returns>
    /// <remarks>
    /// 此方法将传入的Unix时间戳(秒)转换为UTC DateTime后与当前UTC时间比较是否在同一周
    /// 全程使用UTC时间，避免时区差异影响
    /// </remarks>
    public static bool IsUnixSameWeekFromTimestamp(long timestampSeconds)
    {
        var dateTime = UtcSecondsToUtcDateTime(timestampSeconds);
        return IsNowSameWeekUtc(dateTime);
    }

    /// <summary>
    /// 判断当前UTC时间是否与指定Unix时间戳处于同一周。
    /// </summary>
    /// <param name="timestampMilliseconds">指定时间的Unix时间戳(毫秒)。表示自1970年1月1日00:00:00 UTC以来的毫秒数</param>
    /// <returns>如果当前UTC时间与指定时间处于同一周，则为 true；否则为 false。</returns>
    /// <remarks>
    /// 此方法将传入的Unix时间戳(毫秒)转换为UTC DateTime后与当前UTC时间比较是否在同一周
    /// 全程使用UTC时间，避免时区差异影响
    /// </remarks>
    public static bool IsUnixSameWeekFromTimestampMilliseconds(long timestampMilliseconds)
    {
        var dateTime = UtcMillisecondsToUtcDateTime(timestampMilliseconds);
        return IsNowSameWeekUtc(dateTime);
    }

    /// <summary>
    /// 获取指定日期所在星期的时间。
    /// </summary>
    /// <param name="dateTime">指定日期。例如：2024-01-10</param>
    /// <param name="day">星期几。例如：DayOfWeek.Monday 表示星期一，DayOfWeek.Sunday 表示星期日</param>
    /// <returns>返回指定日期所在星期的指定星期几的零点时间。例如：dateTime为2024-01-10(星期三)，day为DayOfWeek.Monday，则返回2024-01-08 00:00:00</returns>
    /// <remarks>
    /// 此方法将星期日(DayOfWeek.Sunday)视为每周的第7天，而不是第0天
    /// 返回的时间总是该日期的零点时间（00:00:00）
    /// </remarks>
    public static DateTime GetDayOfWeekTime(DateTime dateTime, DayOfWeek day)
    {
        // 将星期几转换为数字(1-7)，将星期日从0转换为7
        var dd = (int)day;
        if (dd == 0)
        {
            dd = 7;
        }

        // 获取指定日期是星期几(1-7)，将星期日从0转换为7
        var dayOfWeek = (int)dateTime.DayOfWeek;
        if (dayOfWeek == 0)
        {
            dayOfWeek = 7;
        }

        // 计算目标日期与当前日期的天数差，并返回目标日期的零点时间
        return dateTime.AddDays(dd - dayOfWeek).Date;
    }

    /// <summary>
    /// 获取当前日期所在星期的时间。
    /// </summary>
    /// <param name="day">星期几。例如：DayOfWeek.Monday 表示星期一，DayOfWeek.Sunday 表示星期日。</param>
    /// <returns>返回当前UTC日期所在星期的指定星期几的零点时间。例如：当前是2024-01-10(星期三)，传入DayOfWeek.Monday，则返回2024-01-08 00:00:00。</returns>
    /// <remarks>
    /// 此方法使用UTC时间作为基准计算。
    /// 如果需要使用本地时间，请使用 GetDayOfWeekTime(DateTime.Now, day)。
    /// </remarks>
    public static DateTime GetDayOfWeekTime(DayOfWeek day)
    {
        return GetDayOfWeekTime(DateTime.UtcNow, day);
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
    public static int CurrentDateWithDay()
    {
        return Convert.ToInt32(DateTime.Now.ToString("yyyyMMdd"));
    }

    /// <summary>
    /// 获取当前UTC时区的日期，格式为yyyyMMdd的整数
    /// </summary>
    /// <returns>返回一个8位整数，表示当前UTC时区的日期。例如：20231225表示2023年12月25日</returns>
    public static int CurrentDateWithUtcDay()
    {
        return Convert.ToInt32(DateTime.UtcNow.ToString("yyyyMMdd"));
    }

    /// <summary>
    /// 获取当前UTC时间，格式为HHmmss的字符串
    /// </summary>
    /// <returns>返回一个6位字符串，表示当前UTC时间。例如：143045表示14:30:45</returns>
    public static string CurrentTimeWithUtcFullString()
    {
        return DateTime.UtcNow.ToString("HHmmss");
    }

    /// <summary>
    /// 获取当前本地时间，格式为HHmmss的字符串
    /// </summary>
    /// <returns>返回一个6位字符串，表示当前本地时间。例如：143045表示14:30:45</returns>
    public static string CurrentTimeWithLocalFullString()
    {
        return DateTime.Now.ToString("HHmmss");
    }

    /// <summary>
    /// 获取当前UTC时间，格式为HHmmss的整数
    /// </summary>
    /// <returns>返回一个6位整数，表示当前UTC时间。例如：143045表示14:30:45</returns>
    public static int CurrentTimeWithUtcTime()
    {
        return Convert.ToInt32(CurrentTimeWithUtcFullString());
    }

    /// <summary>
    /// 获取当前本地时间，格式为HHmmss的整数
    /// </summary>
    /// <returns>返回一个6位整数，表示当前本地时间。例如：143045表示14:30:45</returns>
    public static int CurrentTimeWithLocalTime()
    {
        return Convert.ToInt32(CurrentTimeWithLocalFullString());
    }

    /// <summary>
    /// 获取当前本地时区时间的完整格式字符串
    /// </summary>
    /// <returns>返回格式为"yyyy-MM-dd-HH-mm-ss.fff K"的时间字符串，包含年-月-日-时-分-秒.毫秒 时区偏移。例如："2023-12-25-14-30-45.123 +08:00"</returns>
    public static string CurrentDateTimeWithFullString()
    {
        return DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss.fff K");
    }

    /// <summary>
    /// 获取当前本地时区时间的自定义格式字符串
    /// </summary>
    /// <param name="format">时间格式字符串，默认为"yyyy-MM-dd HH:mm:ss.fff K"</param>
    /// <returns>返回指定格式的本地时间字符串。例如默认格式返回："2023-12-25 14:30:45.123 +08:00"</returns>
    public static string CurrentDateTimeWithFormat(string format = "yyyy-MM-dd HH:mm:ss.fff K")
    {
        return DateTime.Now.ToString(format);
    }

    /// <summary>
    /// 获取当前UTC时区时间的自定义格式字符串
    /// </summary>
    /// <param name="format">时间格式字符串，默认为"yyyy-MM-dd HH:mm:ss.fff K"</param>
    /// <returns>返回指定格式的UTC时间字符串。例如默认格式返回："2023-12-25 06:30:45.123 +00:00"</returns>
    public static string CurrentDateTimeWithUtcFormat(string format = "yyyy-MM-dd HH:mm:ss.fff K")
    {
        return DateTime.UtcNow.ToString(format);
    }

    /// <summary>
    /// 获取当前UTC时区时间的完整格式[yyyy-MM-dd-HH-mm-ss.fff K]字符串
    /// </summary>
    /// <returns>返回格式为"yyyy-MM-dd-HH-mm-ss.fff K"的UTC时间字符串，包含年-月-日-时-分-秒.毫秒 时区偏移。例如："2023-12-25-06-30-45.123 +00:00"</returns>
    public static string CurrentDateTimeWithUtcFullString()
    {
        return DateTime.UtcNow.ToString("yyyy-MM-dd-HH-mm-ss.fff K");
    }

    /// <summary>
    /// UTC 时间戳 转换成UTC时间
    /// </summary>
    /// <param name="utcTimestamp">UTC时间戳,单位秒</param>
    /// <returns>转换后的UTC时间。</returns>
    [Obsolete("UtcSecondsToUtcDateTime(long utcTimestampSeconds)代替")]
    public static DateTime UtcToUtcDateTime(long utcTimestamp)
    {
        return UtcSecondsToUtcDateTime(utcTimestamp);
    }

    /// <summary>
    /// UTC 时间戳 转换成UTC时间
    /// </summary>
    /// <param name="utcTimestampSeconds">UTC时间戳,单位秒</param>
    /// <returns>转换后的UTC时间。</returns>
    public static DateTime UtcSecondsToUtcDateTime(long utcTimestampSeconds)
    {
        return DateTimeOffset.FromUnixTimeSeconds(utcTimestampSeconds).UtcDateTime;
    }

    /// <summary>
    /// UTC 毫秒时间戳 转换成UTC时间
    /// </summary>
    /// <param name="utcTimestampMilliseconds">UTC时间戳,单位毫秒</param>
    /// <returns>转换后的UTC时间。</returns>
    public static DateTime UtcMillisecondsToUtcDateTime(long utcTimestampMilliseconds)
    {
        return DateTimeOffset.FromUnixTimeMilliseconds(utcTimestampMilliseconds).UtcDateTime;
    }

    /// <summary>
    /// UTC 时间戳 转换成本地时间
    /// </summary>
    /// <param name="utcTimestamp">UTC时间戳,单位秒</param>
    /// <returns>转换后的本地时间。</returns>
    [Obsolete("UtcSecondsToLocalDateTime(long utcTimestampSeconds) 代替")]
    public static DateTime UtcToLocalDateTime(long utcTimestamp)
    {
        return UtcSecondsToLocalDateTime(utcTimestamp);
    }

    /// <summary>
    /// UTC 时间戳 转换成本地时间
    /// </summary>
    /// <param name="utcTimestamp">UTC时间戳,单位秒</param>
    /// <returns>转换后的本地时间。</returns>
    public static DateTime UtcSecondsToLocalDateTime(long utcTimestamp)
    {
        return DateTimeOffset.FromUnixTimeSeconds(utcTimestamp).LocalDateTime;
    }

    /// <summary>
    /// UTC 毫秒时间戳 转换成本地时间
    /// </summary>
    /// <param name="utcTimestampMilliseconds">UTC时间戳,单位毫秒</param>
    /// <returns>转换后的本地时间。</returns>
    public static DateTime UtcMillisecondsToDateTime(long utcTimestampMilliseconds)
    {
        return DateTimeOffset.FromUnixTimeMilliseconds(utcTimestampMilliseconds).LocalDateTime;
    }

    /// <summary>
    /// 按照UTC时间判断两个时间戳是否是同一天
    /// </summary>
    /// <param name="timestamp1">时间戳1</param>
    /// <param name="timestamp2">时间戳2</param>
    /// <returns>是否是同一天</returns>
    public static bool IsUnixSameDay(long timestamp1, long timestamp2)
    {
        var time1 = UtcSecondsToUtcDateTime(timestamp1);
        var time2 = UtcSecondsToUtcDateTime(timestamp2);
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
    /// <remarks>
    /// 此方法基于UTC时间计算本月开始时间:
    /// 1. 获取当前UTC时间的年份和月份
    /// 2. 创建一个新的DateTime对象,设置为本月1号零点
    /// 3. 返回的时间为UTC时区的时间
    /// 
    /// 示例:
    /// - 当前UTC时间为2024-01-15 14:30:00
    /// - 返回时间为2024-01-01 00:00:00 (UTC)
    /// 
    /// 注意:
    /// - 返回的是UTC时区的时间,如需本地时间请使用TimeZoneInfo.ConvertTimeFromUtc转换
    /// - 返回时间的Hour/Minute/Second/Millisecond均为0
    /// </remarks>
    public static DateTime GetMonthStartTime()
    {
        return new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1);
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
    /// <remarks>
    /// 此方法基于UTC时间计算年份:
    /// 1. 获取当前UTC时间的年份
    /// 2. 返回该年份1月1日零点时间
    /// 
    /// 示例:
    /// - 当前UTC时间为2024-03-15 14:30:00
    /// - 返回2024-01-01 00:00:00
    /// 
    /// 注意:
    /// - 返回的是UTC时间,不考虑本地时区
    /// - 返回时间的时分秒毫秒都为0
    /// - 使用DateTime.UtcNow避免时区转换带来的问题
    /// </remarks>
    public static DateTime GetYearStartTime()
    {
        return new DateTime(DateTime.UtcNow.Year, 1, 1);
    }

    /// <summary>
    /// 获取本年开始时间戳
    /// </summary>
    /// <returns>本年1月1日零点时间戳(秒)</returns>
    /// <remarks>
    /// 此方法返回当前年份1月1日零点的Unix时间戳
    /// 使用本地时区计算时间
    /// 例如:2024年返回2024-01-01 00:00:00的时间戳
    /// </remarks>
    public static long GetYearStartTimestamp()
    {
        return new DateTimeOffset(GetYearStartTime()).ToUnixTimeSeconds();
    }

    /// <summary>
    /// 获取本年结束时间
    /// </summary>
    /// <returns>本年12月31日23:59:59的时间</returns>
    /// <remarks>
    /// 此方法返回当前年份最后一天的最后一秒
    /// 使用本地时区计算时间
    /// 例如:2024年返回2024-12-31 23:59:59
    /// </remarks>
    public static DateTime GetYearEndTime()
    {
        return GetYearStartTime().AddYears(1).AddSeconds(-1);
    }

    /// <summary>
    /// 获取本年结束时间戳
    /// </summary>
    /// <returns>本年12月31日23:59:59的时间戳(秒)</returns>
    /// <remarks>
    /// 此方法返回当前年份最后一天的最后一秒的Unix时间戳
    /// 使用本地时区计算时间
    /// 例如:2024年返回2024-12-31 23:59:59的时间戳
    /// </remarks>
    public static long GetYearEndTimestamp()
    {
        return new DateTimeOffset(GetYearEndTime()).ToUnixTimeSeconds();
    }

    /// <summary>
    /// 获取指定日期的开始时间
    /// </summary>
    /// <param name="date">指定日期</param>
    /// <returns>指定日期零点时间</returns>
    /// <remarks>
    /// 此方法返回指定日期的零点时间(00:00:00)
    /// 例如:输入2024-01-10 14:30:00,返回2024-01-10 00:00:00
    /// 保持原有时区不变
    /// </remarks>
    public static DateTime GetStartTimeOfDay(DateTime date)
    {
        return date.Date;
    }

    /// <summary>
    /// 获取指定日期的开始时间戳
    /// </summary>
    /// <param name="date">指定日期</param>
    /// <returns>指定日期零点时间戳(秒)</returns>
    /// <remarks>
    /// 此方法返回指定日期零点时间的Unix时间戳
    /// 例如:输入2024-01-10 14:30:00,返回2024-01-10 00:00:00的时间戳
    /// 会将时间转换为UTC时间后再计算时间戳
    /// </remarks>
    public static long GetStartTimestampOfDay(DateTime date)
    {
        return new DateTimeOffset(GetStartTimeOfDay(date)).ToUnixTimeSeconds();
    }

    /// <summary>
    /// 获取指定日期的结束时间
    /// </summary>
    /// <param name="date">指定日期</param>
    /// <returns>指定日期23:59:59的时间</returns>
    /// <remarks>
    /// 此方法返回指定日期的最后一秒(23:59:59)
    /// 例如:输入2024-01-10 14:30:00,返回2024-01-10 23:59:59
    /// 保持原有时区不变
    /// </remarks>
    public static DateTime GetEndTimeOfDay(DateTime date)
    {
        return date.Date.AddDays(1).AddSeconds(-1);
    }

    /// <summary>
    /// 获取指定日期的结束时间戳
    /// </summary>
    /// <param name="date">指定日期</param>
    /// <returns>指定日期23:59:59的时间戳(秒)</returns>
    /// <remarks>
    /// 此方法返回指定日期最后一秒的Unix时间戳
    /// 例如:输入2024-01-10 14:30:00,返回2024-01-10 23:59:59的时间戳
    /// 会将时间转换为UTC时间后再计算时间戳
    /// </remarks>
    public static long GetEndTimestampOfDay(DateTime date)
    {
        return new DateTimeOffset(GetEndTimeOfDay(date)).ToUnixTimeSeconds();
    }

    /// <summary>
    /// 获取指定日期所在周的开始时间
    /// </summary>
    /// <param name="date">指定日期</param>
    /// <returns>所在周周一零点时间</returns>
    /// <remarks>
    /// 此方法返回指定日期所在周的周一零点时间
    /// 例如:输入2024-01-10(周三),返回2024-01-08 00:00:00(周一)
    /// 使用周一作为每周的第一天,周日为每周的最后一天
    /// 保持原有时区不变
    /// </remarks>
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
    /// <remarks>
    /// 此方法返回指定日期所在周的周一零点时间的Unix时间戳
    /// 例如:输入2024-01-10(周三),返回2024-01-08 00:00:00(周一)的时间戳
    /// 会将时间转换为UTC时间后再计算时间戳
    /// </remarks>
    public static long GetStartTimestampOfWeek(DateTime date)
    {
        return new DateTimeOffset(GetStartTimeOfWeek(date)).ToUnixTimeSeconds();
    }

    /// <summary>
    /// 获取明天开始时间
    /// </summary>
    /// <returns>明天零点时间</returns>
    /// <remarks>
    /// 此方法返回明天的零点时间
    /// 例如:当前是2024-01-10,返回2024-01-11 00:00:00
    /// 使用本地时区计算时间
    /// </remarks>
    public static DateTime GetTomorrowStartTime()
    {
        return DateTime.Today.AddDays(1);
    }

    /// <summary>
    /// 获取明天开始时间戳
    /// </summary>
    /// <returns>明天零点时间戳(秒)</returns>
    /// <remarks>
    /// 此方法返回明天零点时间的Unix时间戳
    /// 例如:当前是2024-01-10,返回2024-01-11 00:00:00的时间戳
    /// 会将时间转换为UTC时间后再计算时间戳
    /// </remarks>
    public static long GetTomorrowStartTimestamp()
    {
        return new DateTimeOffset(GetTomorrowStartTime()).ToUnixTimeSeconds();
    }

    /// <summary>
    /// 获取明天结束时间
    /// </summary>
    /// <returns>明天23:59:59的时间</returns>
    /// <remarks>
    /// 此方法返回明天的最后一秒
    /// 例如:当前是2024-01-10,返回2024-01-11 23:59:59
    /// 使用本地时区计算时间
    /// </remarks>
    public static DateTime GetTomorrowEndTime()
    {
        return DateTime.Today.AddDays(2).AddSeconds(-1);
    }

    /// <summary>
    /// 获取明天结束时间戳
    /// </summary>
    /// <returns>明天23:59:59的时间戳(秒)</returns>
    /// <remarks>
    /// 此方法返回明天最后一秒的Unix时间戳
    /// 例如:当前是2024-01-10,返回2024-01-11 23:59:59的时间戳
    /// 会将时间转换为UTC时间后再计算时间戳
    /// </remarks>
    public static long GetTomorrowEndTimestamp()
    {
        return new DateTimeOffset(GetTomorrowEndTime()).ToUnixTimeSeconds();
    }

    /// <summary>
    /// 获取下周开始时间
    /// </summary>
    /// <returns>下周一零点时间</returns>
    /// <remarks>
    /// 此方法返回下周一的零点时间
    /// 例如:当前是2024-01-10(周三),返回2024-01-15 00:00:00(下周一)
    /// 使用本地时区计算时间
    /// </remarks>
    public static DateTime GetNextWeekStartTime()
    {
        return GetWeekStartTime().AddDays(7);
    }

    /// <summary>
    /// 获取下周开始时间戳
    /// </summary>
    /// <returns>下周一零点时间戳(秒)</returns>
    /// <remarks>
    /// 此方法返回下周一零点时间的Unix时间戳
    /// 例如:当前是2024-01-10(周三),返回2024-01-15 00:00:00(下周一)的时间戳
    /// 会将时间转换为UTC时间后再计算时间戳
    /// </remarks>
    public static long GetNextWeekStartTimestamp()
    {
        return new DateTimeOffset(GetNextWeekStartTime()).ToUnixTimeSeconds();
    }

    /// <summary>
    /// 获取下周结束时间
    /// </summary>
    /// <returns>下周日23:59:59的时间</returns>
    /// <remarks>
    /// 此方法返回下周日的最后一秒
    /// 例如:当前是2024-01-10(周三),返回2024-01-21 23:59:59(下周日)
    /// 使用本地时区计算时间
    /// </remarks>
    public static DateTime GetNextWeekEndTime()
    {
        return GetNextWeekStartTime().AddDays(7).AddSeconds(-1);
    }

    /// <summary>
    /// 获取下周结束时间戳
    /// </summary>
    /// <returns>下周日23:59:59的时间戳(秒)</returns>
    /// <remarks>
    /// 此方法返回下周日最后一秒的Unix时间戳
    /// 例如:当前是2024-01-10(周三),返回2024-01-21 23:59:59(下周日)的时间戳
    /// 会将时间转换为UTC时间后再计算时间戳
    /// </remarks>
    public static long GetNextWeekEndTimestamp()
    {
        return new DateTimeOffset(GetNextWeekEndTime()).ToUnixTimeSeconds();
    }

    /// <summary>
    /// 获取下月开始时间
    /// </summary>
    /// <returns>下月1号零点时间</returns>
    /// <remarks>
    /// 此方法返回下个月1号的零点时间
    /// 例如:当前是2024-01-10,返回2024-02-01 00:00:00
    /// 使用本地时区计算时间
    /// </remarks>
    public static DateTime GetNextMonthStartTime()
    {
        return GetMonthStartTime().AddMonths(1);
    }

    /// <summary>
    /// 获取下月开始时间戳
    /// </summary>
    /// <returns>下月1号零点时间戳(秒)</returns>
    /// <remarks>
    /// 此方法返回下个月1号零点时间的Unix时间戳
    /// 例如:当前是2024-01-10,返回2024-02-01 00:00:00的时间戳
    /// 会将时间转换为UTC时间后再计算时间戳
    /// </remarks>
    public static long GetNextMonthStartTimestamp()
    {
        return new DateTimeOffset(GetNextMonthStartTime()).ToUnixTimeSeconds();
    }

    /// <summary>
    /// 获取下月结束时间
    /// </summary>
    /// <returns>下月最后一天23:59:59的时间</returns>
    /// <remarks>
    /// 此方法返回下个月最后一天的最后一秒
    /// 例如:当前是2024-01-10,返回2024-02-29 23:59:59
    /// 使用本地时区计算时间
    /// 自动处理大小月份和闰年
    /// </remarks>
    public static DateTime GetNextMonthEndTime()
    {
        return GetNextMonthStartTime().AddMonths(1).AddSeconds(-1);
    }

    /// <summary>
    /// 获取下月结束时间戳
    /// </summary>
    /// <returns>下月最后一天23:59:59的时间戳(秒)</returns>
    /// <remarks>
    /// 此方法返回下个月最后一天最后一秒的Unix时间戳
    /// 例如:当前是2024-01-10,返回2024-02-29 23:59:59的时间戳
    /// 会将时间转换为UTC时间后再计算时间戳
    /// </remarks>
    public static long GetNextMonthEndTimestamp()
    {
        return new DateTimeOffset(GetNextMonthEndTime()).ToUnixTimeSeconds();
    }

    /// <summary>
    /// 获取指定日期所在周的结束时间
    /// </summary>
    /// <param name="date">指定日期</param>
    /// <returns>所在周周日23:59:59的时间</returns>
    /// <remarks>
    /// 此方法返回指定日期所在周的周日最后一秒
    /// 例如:输入2024-01-10(周三),返回2024-01-14 23:59:59(周日)
    /// 使用周一作为每周的第一天,周日为每周的最后一天
    /// 保持原有时区不变
    /// </remarks>
    public static DateTime GetEndTimeOfWeek(DateTime date)
    {
        return GetStartTimeOfWeek(date).AddDays(7).AddSeconds(-1);
    }

    /// <summary>
    /// 获取指定日期所在周的结束时间戳
    /// </summary>
    /// <param name="date">指定日期</param>
    /// <returns>所在周周日23:59:59的时间戳(秒)</returns>
    /// <remarks>
    /// 此方法返回指定日期所在周的周日最后一秒的Unix时间戳
    /// 例如:输入2024-01-10(周三),返回2024-01-14 23:59:59(周日)的时间戳
    /// 会将时间转换为UTC时间后再计算时间戳
    /// </remarks>
    public static long GetEndTimestampOfWeek(DateTime date)
    {
        return new DateTimeOffset(GetEndTimeOfWeek(date)).ToUnixTimeSeconds();
    }

    /// <summary>
    /// 获取指定日期所在月的开始时间
    /// </summary>
    /// <param name="date">指定日期</param>
    /// <returns>所在月1号零点时间</returns>
    /// <remarks>
    /// 此方法返回指定日期所在月份的1号零点时间
    /// 例如:输入2024-01-10,返回2024-01-01 00:00:00
    /// 保持原有时区不变
    /// </remarks>
    public static DateTime GetStartTimeOfMonth(DateTime date)
    {
        return new DateTime(date.Year, date.Month, 1);
    }

    /// <summary>
    /// 获取指定日期所在月的开始时间戳
    /// </summary>
    /// <param name="date">指定日期</param>
    /// <returns>所在月1号零点时间戳(秒)</returns>
    /// <remarks>
    /// 此方法返回指定日期所在月份的1号零点时间的Unix时间戳
    /// 例如:输入2024-01-10,返回2024-01-01 00:00:00的时间戳
    /// 会将时间转换为UTC时间后再计算时间戳
    /// </remarks>
    public static long GetStartTimestampOfMonth(DateTime date)
    {
        return new DateTimeOffset(GetStartTimeOfMonth(date)).ToUnixTimeSeconds();
    }

    /// <summary>
    /// 获取指定日期所在月的结束时间
    /// </summary>
    /// <param name="date">指定日期</param>
    /// <returns>所在月最后一天23:59:59的时间</returns>
    /// <remarks>
    /// 此方法返回指定日期所在月份的最后一天的最后一秒
    /// 例如:输入2024-01-10,返回2024-01-31 23:59:59
    /// 保持原有时区不变
    /// 自动处理大小月份和闰年
    /// </remarks>
    public static DateTime GetEndTimeOfMonth(DateTime date)
    {
        return GetStartTimeOfMonth(date).AddMonths(1).AddSeconds(-1);
    }

    /// <summary>
    /// 获取指定日期所在月的结束时间戳
    /// </summary>
    /// <param name="date">指定日期</param>
    /// <returns>所在月最后一天23:59:59的时间戳(秒)</returns>
    /// <remarks>
    /// 此方法返回指定日期所在月份的最后一天最后一秒的Unix时间戳
    /// 例如:输入2024-01-10,返回2024-01-31 23:59:59的时间戳
    /// 会将时间转换为UTC时间后再计算时间戳
    /// </remarks>
    public static long GetEndTimestampOfMonth(DateTime date)
    {
        return new DateTimeOffset(GetEndTimeOfMonth(date)).ToUnixTimeSeconds();
    }

    /// <summary>
    /// 获取指定日期所在年的开始时间
    /// </summary>
    /// <param name="date">指定日期</param>
    /// <returns>所在年1月1日零点时间</returns>
    /// <remarks>
    /// 此方法返回指定日期所在年份的1月1日零点时间
    /// 例如:输入2024-01-10,返回2024-01-01 00:00:00
    /// 保持原有时区不变
    /// </remarks>
    public static DateTime GetStartTimeOfYear(DateTime date)
    {
        return new DateTime(date.Year, 1, 1);
    }

    /// <summary>
    /// 获取指定日期所在年的开始时间戳
    /// </summary>
    /// <param name="date">指定日期</param>
    /// <returns>所在年1月1日零点时间戳(秒)</returns>
    /// <remarks>
    /// 此方法返回指定日期所在年份的1月1日零点时间的Unix时间戳
    /// 例如:输入2024-01-10,返回2024-01-01 00:00:00的时间戳
    /// 会将时间转换为UTC时间后再计算时间戳
    /// </remarks>
    public static long GetStartTimestampOfYear(DateTime date)
    {
        return new DateTimeOffset(GetStartTimeOfYear(date)).ToUnixTimeSeconds();
    }

    /// <summary>
    /// 获取指定日期所在年的结束时间
    /// </summary>
    /// <param name="date">指定日期</param>
    /// <returns>所在年12月31日23:59:59的时间</returns>
    /// <remarks>
    /// 此方法返回指定日期所在年份的12月31日最后一秒
    /// 例如:输入2024-01-10,返回2024-12-31 23:59:59
    /// 保持原有时区不变
    /// </remarks>
    public static DateTime GetEndTimeOfYear(DateTime date)
    {
        return GetStartTimeOfYear(date).AddYears(1).AddSeconds(-1);
    }

    /// <summary>
    /// 获取指定日期所在年的结束时间戳
    /// </summary>
    /// <param name="date">指定日期</param>
    /// <returns>所在年12月31日23:59:59的时间戳(秒)</returns>
    /// <remarks>
    /// 此方法返回指定日期所在年份的12月31日最后一秒的Unix时间戳
    /// 例如:输入2024-01-10,返回2024-12-31 23:59:59的时间戳
    /// 会将时间转换为UTC时间后再计算时间戳
    /// </remarks>
    public static long GetEndTimestampOfYear(DateTime date)
    {
        return new DateTimeOffset(GetEndTimeOfYear(date)).ToUnixTimeSeconds();
    }

    /// <summary>
    /// 获取当前UTC时间
    /// </summary>
    /// <returns>当前UTC时间</returns>
    /// <remarks>
    /// 此方法返回当前的UTC时间(协调世界时)
    /// 与本地时间相比会有时区偏移
    /// 主要用于需要统一时间标准的场景
    /// </remarks>
    public static DateTime GetUtcNow()
    {
        return DateTime.UtcNow;
    }

    /// <summary>
    /// 获取当前时间
    /// </summary>
    /// <returns>当前时间</returns>
    /// <remarks>
    /// 此方法返回当前的本地时间
    /// 会根据系统设置的时区自动调整
    /// 主要用于需要显示本地时间的场景
    /// </remarks>
    public static DateTime GetNow()
    {
        return DateTime.Now;
    }

    /// <summary>
    /// 获取指定时间是否在指定的时间范围内
    /// </summary>
    /// <param name="time">指定时间。例如：2024-01-10 14:30:00</param>
    /// <param name="startTime">开始时间。例如：2024-01-10 00:00:00</param>
    /// <param name="endTime">结束时间。例如：2024-01-10 23:59:59</param>
    /// <returns>如果指定时间在开始时间和结束时间之间（包含边界），则返回true；否则返回false</returns>
    /// <remarks>
    /// 此方法使用闭区间比较，即time等于startTime或endTime时也返回true
    /// 不会对startTime和endTime的先后顺序做检查，调用方需确保startTime不晚于endTime
    /// </remarks>
    public static bool IsTimeInRange(DateTime time, DateTime startTime, DateTime endTime)
    {
        return time >= startTime && time <= endTime;
    }

    /// <summary>
    /// 获取指定时间戳是否在指定的时间戳范围内
    /// </summary>
    /// <param name="timestamp">指定时间戳（Unix秒级时间戳）。例如：1704857400</param>
    /// <param name="startTimestamp">开始时间戳（Unix秒级时间戳）。例如：1704816000</param>
    /// <param name="endTimestamp">结束时间戳（Unix秒级时间戳）。例如：1704902399</param>
    /// <returns>如果指定时间戳在开始时间戳和结束时间戳之间（包含边界），则返回true；否则返回false</returns>
    /// <remarks>
    /// 此方法使用闭区间比较，即timestamp等于startTimestamp或endTimestamp时也返回true
    /// 不会对startTimestamp和endTimestamp的先后顺序做检查，调用方需确保startTimestamp不大于endTimestamp
    /// 时间戳应为Unix秒级时间戳（自1970年1月1日UTC零点以来的秒数）
    /// </remarks>
    public static bool IsTimestampInRange(long timestamp, long startTimestamp, long endTimestamp)
    {
        return timestamp >= startTimestamp && timestamp <= endTimestamp;
    }

    /// <summary>
    /// 按照本地时间判断两个时间戳是否是同一天
    /// </summary>
    /// <param name="timestamp1">时间戳1（Unix秒级时间戳）。例如：1704857400</param>
    /// <param name="timestamp2">时间戳2（Unix秒级时间戳）。例如：1704859200</param>
    /// <returns>如果两个时间戳转换为本地时间后是同一天，则返回true；否则返回false</returns>
    /// <remarks>
    /// 此方法会先将UTC时间戳转换为本地时间，然后比较是否为同一天
    /// 比较时只考虑年月日，不考虑具体时间
    /// 使用系统默认时区进行UTC到本地时间的转换
    /// </remarks>
    public static bool IsLocalSameDay(long timestamp1, long timestamp2)
    {
        var time1 = UtcSecondsToLocalDateTime(timestamp1);
        var time2 = UtcSecondsToLocalDateTime(timestamp2);
        return IsSameDay(time1, time2);
    }

    /// <summary>
    /// 判断两个时间是否是同一天
    /// </summary>
    /// <param name="time1">时间1。例如：2024-01-10 14:30:00</param>
    /// <param name="time2">时间2。例如：2024-01-10 18:45:00</param>
    /// <returns>如果两个时间是同一天，则返回true；否则返回false</returns>
    /// <remarks>
    /// 此方法只比较年月日是否相同，不考虑具体时间
    /// 比较时会忽略时区差异，直接使用DateTime中存储的日期值进行比较
    /// 使用Date属性确保只比较日期部分
    /// </remarks>
    public static bool IsSameDay(DateTime time1, DateTime time2)
    {
        return time1.Date.Year == time2.Date.Year && time1.Date.Month == time2.Date.Month && time1.Date.Day == time2.Date.Day;
    }
}