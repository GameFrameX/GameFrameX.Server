﻿namespace GameFrameX.Utility;

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
        return TimeMillis(DateTime.Now, false);
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
        int days = (int)(after.Date - begin.Date).TotalDays;
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

        int dayOfWeek = (int)start.DayOfWeek;
        if (dayOfWeek == (int)DayOfWeek.Sunday) dayOfWeek = 7;
        // 获取较早时间所在星期的星期天的0点
        var startsWeekLastDate = start.AddDays(7 - dayOfWeek).Date;
        // 判断end是否在start所在周
        return startsWeekLastDate >= end.Date;
    }

    /// <summary>
    /// 获取指定日期所在星期的时间。
    /// </summary>
    /// <param name="t">指定日期。</param>
    /// <param name="d">星期几。</param>
    /// <returns>指定日期所在星期的时间。</returns>
    public static DateTime GetDayOfWeekTime(DateTime t, DayOfWeek d)
    {
        int dd = (int)d;
        if (dd == 0)
        {
            dd = 7;
        }

        var dayOfWeek = (int)t.DayOfWeek;
        if (dayOfWeek == 0)
        {
            dayOfWeek = 7;
        }

        return t.AddDays(dd - dayOfWeek).Date;
    }

    /// <summary>
    /// 获取当前日期所在星期的时间。
    /// </summary>
    /// <param name="d">星期几。</param>
    /// <returns>当前日期所在星期的时间。</returns>
    public static DateTime GetDayOfWeekTime(DayOfWeek d)
    {
        return GetDayOfWeekTime(DateTime.Now, d);
    }

    /// <summary>
    /// 获取指定星期在中国的对应数字。
    /// </summary>
    /// <param name="d">星期几。</param>
    /// <returns>星期在中国的对应数字。</returns>
    public static int GetChinaDayOfWeek(DayOfWeek d)
    {
        int dayOfWeek = (int)d;
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
    /// 当前时区时间的完整字符串
    /// </summary>
    /// <returns>当前时区时间的完整字符串。</returns>
    public static string CurrentTimeWithFullString()
    {
        return DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss.fff K");
    }

    /// <summary>
    /// UTC时区时间的完整字符串
    /// </summary>
    /// <returns>UTC时区时间的完整字符串。</returns>
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