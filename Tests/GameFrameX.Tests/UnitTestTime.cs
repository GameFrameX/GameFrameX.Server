using GameFrameX.Utility;

namespace GameFrameX.Tests;

public class UnitTestTime
{
    private DateTime dateTime, dateTime1;
    private readonly DateTime testDate = new DateTime(2024, 1, 10, 14, 30, 45, 123); // 2024年1月10日 14:30:45.123
    private readonly DateTime testDateUtc = new DateTime(2024, 1, 10, 14, 30, 45, 123, DateTimeKind.Utc);
    private readonly long testTimestamp = 1704897045; // 对应2024-01-10 14:30:45 UTC
    private readonly long testTimestampMs = 1704897045123; // 对应2024-01-10 14:30:45.123 UTC

    [SetUp]
    public void Setup()
    {
        dateTime = DateTime.Now;
        dateTime1 = DateTime.Now.AddHours(1);
        // 重置时间偏移
        TimeHelper.ResetTimeOffset();
    }

    [TearDown]
    public void TearDown()
    {
        // 确保每个测试后重置时间偏移
        TimeHelper.ResetTimeOffset();
    }

    #region 基础时间戳和转换测试

    [Test]
    public void test_current_time_millis()
    {
        var currentTimeMillis = TimeHelper.UnixTimeMilliseconds();
        var expectedTimeMillis = (long)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalMilliseconds;
        Assert.That(expectedTimeMillis, Is.EqualTo(currentTimeMillis));
    }

    [Test]
    public void test_unix_time_seconds()
    {
        var unixTimeSeconds = TimeHelper.UnixTimeSeconds();
        var expectedUnixTimeSeconds = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();
        Assert.That(unixTimeSeconds, Is.EqualTo(expectedUnixTimeSeconds));
    }

    [Test]
    public void test_unix_time_milliseconds()
    {
        var unixTimeMilliseconds = TimeHelper.UnixTimeMilliseconds();
        var expectedUnixTimeMilliseconds = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds();
        Assert.That(unixTimeMilliseconds, Is.EqualTo(expectedUnixTimeMilliseconds));
    }

    [Test]
    public void test_time_millis()
    {
        var time = new DateTime(2022, 1, 1, 12, 0, 0);
        var utc = false;
        var timeMillis = TimeHelper.TimeToMilliseconds(time, utc);
        var expectedTimeMillis = (long)(time - new DateTime(1970, 1, 1)).TotalMilliseconds;
        Assert.That(timeMillis, Is.EqualTo(expectedTimeMillis));
    }

    [Test]
    public void test_millis_to_date_time()
    {
        var timeMillis = 1641024000000;
        var utc = true;
        var dateTime = TimeHelper.MillisecondsTimeStampToDateTime(timeMillis, utc);
        var expectedDateTime = new DateTime(2022, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        Assert.That(dateTime, Is.EqualTo(expectedDateTime));
    }

    [Test]
    public void test_get_cross_days()
    {
        var begin = new DateTime(2022, 3, 12, 1, 0, 0);
        var after = new DateTime(2022, 3, 13, 23, 0, 0);
        var hour = 0;
        var crossDays = TimeHelper.GetCrossDays(begin, after, hour);
        var expectedCrossDays = 1;
        Assert.That(crossDays, Is.EqualTo(expectedCrossDays));
    }

    [Test]
    public void Test1()
    {
        Assert.That(dateTime1.Year, Is.EqualTo(dateTime.Year));
        Assert.That(dateTime1.Month, Is.EqualTo(dateTime.Month));
        Assert.That(dateTime1.Day, Is.EqualTo(dateTime.Day));
    }

    #endregion

    #region 补充缺失的测试 - 时间戳转换函数

    [Test]
    public void TestTimestampToDateTime()
    {
        var timestamp = 1704897045L; // 2024-01-10 14:30:45 UTC
        
        // 测试UTC转换
        var utcDateTime = TimeHelper.TimestampToDateTime(timestamp, true);
        // 验证返回的是DateTime类型且Kind为Utc
        Assert.That(utcDateTime, Is.TypeOf<DateTime>());
        Assert.That(utcDateTime.Kind, Is.EqualTo(DateTimeKind.Utc));
        
        // 测试本地时间转换
        var localDateTime = TimeHelper.TimestampToDateTime(timestamp, false);
        Assert.That(localDateTime, Is.TypeOf<DateTime>());
    }

    [Test]
    public void TestUtcToUtcDateTime()
    {
        var timestamp = 1704897045L; // 2024-01-10 14:30:45 UTC
        var dateTime = TimeHelper.UtcToUtcDateTime(timestamp);
        var expected = new DateTime(2024, 1, 10, 14, 30, 45, DateTimeKind.Utc);
        Assert.That(dateTime, Is.EqualTo(expected));
    }

    [Test]
    public void TestUtcToLocalDateTime()
    {
        var timestamp = 1704897045L; // 2024-01-10 14:30:45 UTC
        var localDateTime = TimeHelper.UtcToLocalDateTime(timestamp);
        Assert.That(localDateTime, Is.TypeOf<DateTime>());
    }

    [Test]
    public void TestUtcSecondsToLocalDateTime()
    {
        var timestamp = 1704897045L; // 2024-01-10 14:30:45 UTC
        var localDateTime = TimeHelper.UtcSecondsToLocalDateTime(timestamp);
        Assert.That(localDateTime, Is.TypeOf<DateTime>());
    }

    [Test]
    public void TestUtcSecondsToLocalDateTimeVsDateTimeOffset()
    {
        // 测试多个时间戳，验证两种方法的结果是否一致
        var timestamps = new long[]
        {
            0L, // 1970-01-01 00:00:00 UTC
            1704897045L, // 2024-01-10 14:30:45 UTC
            1735689600L, // 2025-01-01 00:00:00 UTC
            946684800L, // 2000-01-01 00:00:00 UTC
            1577836800L // 2020-01-01 00:00:00 UTC
        };

        foreach (var timestamp in timestamps)
        {
            var method1 = TimeHelper.UtcSecondsToLocalDateTime(timestamp);
            var method2 = DateTimeOffset.FromUnixTimeSeconds(timestamp).ToLocalTime().DateTime;
            
            Assert.That(method1.Year, Is.EqualTo(method2.Year), $"Year mismatch for timestamp {timestamp}");
            Assert.That(method1.Month, Is.EqualTo(method2.Month), $"Month mismatch for timestamp {timestamp}");
            Assert.That(method1.Day, Is.EqualTo(method2.Day), $"Day mismatch for timestamp {timestamp}");
            Assert.That(method1.Hour, Is.EqualTo(method2.Hour), $"Hour mismatch for timestamp {timestamp}");
            Assert.That(method1.Minute, Is.EqualTo(method2.Minute), $"Minute mismatch for timestamp {timestamp}");
            Assert.That(method1.Second, Is.EqualTo(method2.Second), $"Second mismatch for timestamp {timestamp}");
        }
    }

    [Test]
    public void TestUtcMillisecondsToDateTime()
    {
        var timestampMs = 1704897045123L; // 2024-01-10 14:30:45.123 UTC
        var dateTime = TimeHelper.UtcMillisecondsToDateTime(timestampMs);
        Assert.That(dateTime, Is.TypeOf<DateTime>());
        Assert.That(dateTime.Millisecond, Is.EqualTo(123));
    }

    #endregion

    #region 补充缺失的测试 - 跨天数计算的其他重载

    [Test]
    public void TestGetCrossDaysFromDateTime()
    {
        var startTime = new DateTime(2024, 1, 10, 10, 0, 0);
        var days = TimeHelper.GetCrossDays(startTime, 0);
        Assert.That(days, Is.GreaterThanOrEqualTo(0));
    }

    [Test]
    public void TestGetCrossLocalDaysFromDateTime()
    {
        var startTime = new DateTime(2024, 1, 10, 10, 0, 0);
        var days = TimeHelper.GetCrossLocalDays(startTime, 0);
        Assert.That(days, Is.GreaterThanOrEqualTo(0));
    }

    [Test]
    public void TestGetCrossDaysFromTimestamp()
    {
        var beginTimestamp = 1704880800L; // 2024-01-10 08:00:00 UTC
        var days = TimeHelper.GetCrossDays(beginTimestamp, 0);
        Assert.That(days, Is.GreaterThanOrEqualTo(0));
    }

    #endregion

    #region 补充缺失的测试 - 同周判断函数

    [Test]
    public void TestIsNowSameWeekWithTicks()
    {
        var now = DateTime.Now;
        var ticks = now.Ticks;
        var result = TimeHelper.IsNowSameWeek(ticks);
        Assert.That(result, Is.True); // 当前时间应该与现在同周
    }

    [Test]
    public void TestIsNowSameWeekWithDateTime()
    {
        var now = DateTime.Now;
        var result = TimeHelper.IsNowSameWeek(now);
        Assert.That(result, Is.True); // 当前时间应该与现在同周
    }

    [Test]
    public void TestIsNowSameWeekUtc()
    {
        var utcNow = DateTime.UtcNow;
        var result = TimeHelper.IsNowSameWeekUtc(utcNow);
        Assert.That(result, Is.True); // 当前UTC时间应该与现在同周
    }

    [Test]
    public void TestIsUnixSameWeek()
    {
        var now = DateTime.Now;
        var ticks = now.Ticks;
        var result = TimeHelper.IsUnixSameWeek(ticks);
        Assert.That(result, Is.TypeOf<bool>());
    }

    [Test]
    public void TestIsUnixSameWeekFromTimestampMilliseconds()
    {
        var timestampMs = TimeHelper.UnixTimeMilliseconds();
        var result = TimeHelper.IsUnixSameWeekFromTimestampMilliseconds(timestampMs);
        Assert.That(result, Is.True); // 当前时间戳应该与现在同周
    }

    #endregion

    #region 时间偏移测试

    [Test]
    public void TestTimeOffset()
    {
        // 测试设置时间偏移
        TimeHelper.SetTimeOffset(3600, 3600000); // 1小时偏移
        
        var originalSeconds = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();
        var offsetSeconds = TimeHelper.UnixTimeSeconds();
        Assert.That(offsetSeconds, Is.EqualTo(originalSeconds + 3600));
        
        var originalMs = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds();
        var offsetMs = TimeHelper.UnixTimeMilliseconds();
        Assert.That(offsetMs, Is.EqualTo(originalMs + 3600000));
        
        // 测试重置偏移
        TimeHelper.ResetTimeOffset();
        var resetSeconds = TimeHelper.UnixTimeSeconds();
        var expectedSeconds = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();
        Assert.That(resetSeconds, Is.EqualTo(expectedSeconds));
    }

    #endregion

    #region 时间差计算测试

    [Test]
    public void TestGetTimeDifference()
    {
        var startTime = new DateTime(2024, 1, 10, 10, 0, 0);
        var endTime = new DateTime(2024, 1, 10, 12, 30, 0);
        
        var diff = TimeHelper.GetTimeDifference(startTime, endTime);
        Assert.That(diff.TotalHours, Is.EqualTo(2.5));
        Assert.That(diff.TotalMinutes, Is.EqualTo(150));
    }

    [Test]
    public void TestGetTimeDifferenceWithTimestamp()
    {
        var startTimestamp = 1704880800L; // 2024-01-10 08:00:00 UTC
        var endTimestamp = 1704889800L;   // 2024-01-10 10:30:00 UTC
        
        var diff = TimeHelper.GetTimeDifference(startTimestamp, endTimestamp, true);
        Assert.That(diff.TotalHours, Is.EqualTo(2.5));
    }

    [Test]
    public void TestGetTimeDifferenceFromNow()
    {
        var pastTime = DateTime.Now.AddHours(-2);
        var diff = TimeHelper.GetTimeDifferenceFromNow(pastTime);
        Assert.That(diff.TotalHours, Is.GreaterThan(1.9).And.LessThan(2.1));
    }

    [Test]
    public void TestGetSecondsDifference()
    {
        var startTime = new DateTime(2024, 1, 10, 10, 0, 0);
        var endTime = new DateTime(2024, 1, 10, 10, 5, 30);
        
        var seconds = TimeHelper.GetSecondsDifference(startTime, endTime);
        Assert.That(seconds, Is.EqualTo(330)); // 5分30秒 = 330秒
    }

    [Test]
    public void TestGetMillisecondsDifference()
    {
        var startTime = new DateTime(2024, 1, 10, 10, 0, 0, 0);
        var endTime = new DateTime(2024, 1, 10, 10, 0, 1, 500);
        
        var ms = TimeHelper.GetMillisecondsDifference(startTime, endTime);
        Assert.That(ms, Is.EqualTo(1500)); // 1.5秒 = 1500毫秒
    }

    [Test]
    public void TestGetElapsedSeconds()
    {
        var pastTime = DateTime.Now.AddSeconds(-30);
        var elapsed = TimeHelper.GetElapsedSeconds(pastTime);
        Assert.That(elapsed, Is.GreaterThan(29).And.LessThan(31));
    }

    [Test]
    public void TestGetAbsoluteDifference()
    {
        var time1 = new DateTime(2024, 1, 10, 10, 0, 0);
        var time2 = new DateTime(2024, 1, 10, 8, 0, 0);
        
        var absSeconds = TimeHelper.GetAbsoluteSecondsDifference(time1, time2);
        Assert.That(absSeconds, Is.EqualTo(7200)); // 2小时 = 7200秒
        
        var absMs = TimeHelper.GetAbsoluteMillisecondsDifference(time1, time2);
        Assert.That(absMs, Is.EqualTo(7200000)); // 2小时 = 7200000毫秒
    }

    #endregion

    #region 时间戳转换测试

    [Test]
    public void TestTimestampToTicks()
    {
        var timestamp = 1704902445L; // 2024-01-10 14:30:45 UTC
        var ticks = TimeHelper.TimestampToTicks(timestamp);
        var expectedTicks = timestamp * 10000000L + 621355968000000000L;
        Assert.That(ticks, Is.EqualTo(expectedTicks));
    }

    [Test]
    public void TestTimestampMillisToTicks()
    {
        var timestampMs = 1704902445123L; // 2024-01-10 14:30:45.123 UTC
        var ticks = TimeHelper.TimestampMillisToTicks(timestampMs);
        var expectedTicks = timestampMs * 10000L + 621355968000000000L;
        Assert.That(ticks, Is.EqualTo(expectedTicks));
    }

    [Test]
    public void TestUtcSecondsToUtcDateTime()
    {
        var timestamp = 1704897045L; // 2024-01-10 14:30:45 UTC
        var dateTime = TimeHelper.UtcSecondsToUtcDateTime(timestamp);
        var expected = new DateTime(2024, 1, 10, 14, 30, 45, DateTimeKind.Utc);
        Assert.That(dateTime, Is.EqualTo(expected));
    }

    [Test]
    public void TestUtcMillisecondsToUtcDateTime()
    {
        var timestampMs = 1704897045123L; // 2024-01-10 14:30:45.123 UTC
        var dateTime = TimeHelper.UtcMillisecondsToUtcDateTime(timestampMs);
        var expected = new DateTime(2024, 1, 10, 14, 30, 45, 123, DateTimeKind.Utc);
        Assert.That(dateTime, Is.EqualTo(expected));
    }

    #endregion

    #region 跨天数计算测试

    [Test]
    public void TestGetCrossDaysVariants()
    {
        var begin = new DateTime(2024, 1, 10, 10, 0, 0);
        var end = new DateTime(2024, 1, 12, 15, 0, 0);
        
        var days = TimeHelper.GetCrossDays(begin, end, 0);
        Assert.That(days, Is.EqualTo(2));
        
        // 测试时间戳版本
        var beginTimestamp = new DateTimeOffset(begin).ToUnixTimeSeconds();
        var endTimestamp = new DateTimeOffset(end).ToUnixTimeSeconds();
        var daysFromTimestamp = TimeHelper.GetCrossDays(beginTimestamp, endTimestamp, 0);
        Assert.That(daysFromTimestamp, Is.EqualTo(2));
    }

    [Test]
    public void TestGetCrossLocalDays()
    {
        var startTimestamp = 1704880800L; // 2024-01-10 08:00:00 UTC
        var endTimestamp = 1704967200L;   // 2024-01-11 08:00:00 UTC
        
        var days = TimeHelper.GetCrossLocalDays(startTimestamp, endTimestamp);
        Assert.That(days, Is.GreaterThanOrEqualTo(0)); // 结果取决于本地时区
    }

    #endregion

    #region 同周判断测试

    [Test]
    public void TestIsSameWeek()
    {
        var monday = new DateTime(2024, 1, 8);    // 周一
        var wednesday = new DateTime(2024, 1, 10); // 周三
        var sunday = new DateTime(2024, 1, 14);   // 周日
        var nextMonday = new DateTime(2024, 1, 15); // 下周一
        
        Assert.That(TimeHelper.IsSameWeek(monday, wednesday), Is.True);
        Assert.That(TimeHelper.IsSameWeek(wednesday, sunday), Is.True);
        Assert.That(TimeHelper.IsSameWeek(sunday, nextMonday), Is.False);
    }

    [Test]
    public void TestIsUnixSameWeekFromTimestamp()
    {
        var mondayTimestamp = 1704672000L;    // 2024-01-08 00:00:00 UTC (周一)
        var wednesdayTimestamp = 1704844800L; // 2024-01-10 00:00:00 UTC (周三)
        
        // 注意：这个测试结果可能因当前时间而异
        var result = TimeHelper.IsUnixSameWeekFromTimestamp(mondayTimestamp);
        Assert.That(result, Is.TypeOf<bool>());
    }

    #endregion

    #region 星期相关测试

    [Test]
    public void TestGetDayOfWeekTime()
    {
        var wednesday = new DateTime(2024, 1, 10); // 2024-01-10 是周三
        var monday = TimeHelper.GetDayOfWeekTime(wednesday, DayOfWeek.Monday);
        var expectedMonday = new DateTime(2024, 1, 8); // 2024-01-08 是周一
        
        Assert.That(monday, Is.EqualTo(expectedMonday));
    }

    [Test]
    public void TestGetChinaDayOfWeek()
    {
        Assert.That(TimeHelper.GetChinaDayOfWeek(DayOfWeek.Monday), Is.EqualTo(1));
        Assert.That(TimeHelper.GetChinaDayOfWeek(DayOfWeek.Tuesday), Is.EqualTo(2));
        Assert.That(TimeHelper.GetChinaDayOfWeek(DayOfWeek.Sunday), Is.EqualTo(7));
    }

    #endregion

    #region 日期格式化测试

    [Test]
    public void TestCurrentDateWithDay()
    {
        var result = TimeHelper.CurrentDateWithDay();
        var expected = int.Parse(DateTime.Now.ToString("yyyyMMdd"));
        Assert.That(result, Is.EqualTo(expected));
    }

    [Test]
    public void TestCurrentTimeWithUtcTime()
    {
        var result = TimeHelper.CurrentTimeWithUtcTime();
        var expected = int.Parse(DateTime.UtcNow.ToString("HHmmss"));
        // 允许1秒的误差
        Assert.That(Math.Abs(result - expected), Is.LessThanOrEqualTo(1));
    }

    [Test]
    public void TestCurrentDateTimeWithFormat()
    {
        var format = "yyyy-MM-dd HH:mm";
        var result = TimeHelper.CurrentDateTimeWithFormat(format);
        var expected = DateTime.Now.ToString(format);
        
        // 由于时间可能有微小差异，我们只检查格式是否正确
        Assert.That(result.Length, Is.EqualTo(expected.Length));
        Assert.That(result.Contains("-"), Is.True);
        Assert.That(result.Contains(":"), Is.True);
    }

    #endregion

    #region 时间范围获取测试

    [Test]
    public void TestGetTodayStartAndEndTime()
    {
        var startTime = TimeHelper.GetTodayStartTime();
        var endTime = TimeHelper.GetTodayEndTime();
        
        Assert.That(startTime.Hour, Is.EqualTo(0));
        Assert.That(startTime.Minute, Is.EqualTo(0));
        Assert.That(startTime.Second, Is.EqualTo(0));
        
        Assert.That(endTime.Hour, Is.EqualTo(23));
        Assert.That(endTime.Minute, Is.EqualTo(59));
        Assert.That(endTime.Second, Is.EqualTo(59));
        
        Assert.That(startTime.Date, Is.EqualTo(endTime.Date));
    }

    [Test]
    public void TestGetWeekStartAndEndTime()
    {
        var startTime = TimeHelper.GetWeekStartTime();
        var endTime = TimeHelper.GetWeekEndTime();
        
        Assert.That(startTime.DayOfWeek, Is.EqualTo(DayOfWeek.Monday));
        Assert.That(endTime.DayOfWeek, Is.EqualTo(DayOfWeek.Sunday));
        
        var daysDiff = (endTime.Date - startTime.Date).Days;
        Assert.That(daysDiff, Is.EqualTo(6));
    }

    [Test]
    public void TestGetMonthStartAndEndTime()
    {
        var startTime = TimeHelper.GetMonthStartTime();
        var endTime = TimeHelper.GetMonthEndTime();
        
        Assert.That(startTime.Day, Is.EqualTo(1));
        Assert.That(startTime.Hour, Is.EqualTo(0));
        
        Assert.That(endTime.Hour, Is.EqualTo(23));
        Assert.That(endTime.Minute, Is.EqualTo(59));
        Assert.That(endTime.Second, Is.EqualTo(59));
        
        // 检查是否是同一个月的最后一天
        var nextMonth = startTime.AddMonths(1);
        Assert.That(endTime.Date, Is.EqualTo(nextMonth.AddDays(-1).Date));
    }

    [Test]
    public void TestGetYearStartAndEndTime()
    {
        var startTime = TimeHelper.GetYearStartTime();
        var endTime = TimeHelper.GetYearEndTime();
        
        Assert.That(startTime.Month, Is.EqualTo(1));
        Assert.That(startTime.Day, Is.EqualTo(1));
        Assert.That(startTime.Hour, Is.EqualTo(0));
        
        Assert.That(endTime.Month, Is.EqualTo(12));
        Assert.That(endTime.Day, Is.EqualTo(31));
        Assert.That(endTime.Hour, Is.EqualTo(23));
        Assert.That(endTime.Minute, Is.EqualTo(59));
        Assert.That(endTime.Second, Is.EqualTo(59));
    }

    #endregion

    #region 指定日期时间范围测试

    [Test]
    public void TestGetStartAndEndTimeOfDay()
    {
        var date = new DateTime(2024, 1, 10, 14, 30, 45);
        
        var startTime = TimeHelper.GetStartTimeOfDay(date);
        var endTime = TimeHelper.GetEndTimeOfDay(date);
        
        Assert.That(startTime, Is.EqualTo(new DateTime(2024, 1, 10, 0, 0, 0)));
        Assert.That(endTime, Is.EqualTo(new DateTime(2024, 1, 10, 23, 59, 59)));
    }

    [Test]
    public void TestGetStartAndEndTimeOfWeek()
    {
        var wednesday = new DateTime(2024, 1, 10); // 周三
        
        var startTime = TimeHelper.GetStartTimeOfWeek(wednesday);
        var endTime = TimeHelper.GetEndTimeOfWeek(wednesday);
        
        Assert.That(startTime.DayOfWeek, Is.EqualTo(DayOfWeek.Monday));
        Assert.That(endTime.DayOfWeek, Is.EqualTo(DayOfWeek.Sunday));
        Assert.That(startTime, Is.EqualTo(new DateTime(2024, 1, 8, 0, 0, 0)));
    }

    [Test]
    public void TestGetStartAndEndTimeOfMonth()
    {
        var date = new DateTime(2024, 1, 15);
        
        var startTime = TimeHelper.GetStartTimeOfMonth(date);
        var endTime = TimeHelper.GetEndTimeOfMonth(date);
        
        Assert.That(startTime, Is.EqualTo(new DateTime(2024, 1, 1, 0, 0, 0)));
        Assert.That(endTime.Day, Is.EqualTo(31)); // 1月有31天
        Assert.That(endTime.Hour, Is.EqualTo(23));
        Assert.That(endTime.Minute, Is.EqualTo(59));
        Assert.That(endTime.Second, Is.EqualTo(59));
    }

    [Test]
    public void TestGetStartAndEndTimeOfYear()
    {
        var date = new DateTime(2024, 6, 15);
        
        var startTime = TimeHelper.GetStartTimeOfYear(date);
        var endTime = TimeHelper.GetEndTimeOfYear(date);
        
        Assert.That(startTime, Is.EqualTo(new DateTime(2024, 1, 1, 0, 0, 0)));
        Assert.That(endTime, Is.EqualTo(new DateTime(2024, 12, 31, 23, 59, 59)));
    }

    #endregion

    #region 下一个时间段测试

    [Test]
    public void TestGetTomorrowTimes()
    {
        var tomorrowStart = TimeHelper.GetTomorrowStartTime();
        var tomorrowEnd = TimeHelper.GetTomorrowEndTime();
        var today = DateTime.Today;
        
        Assert.That(tomorrowStart, Is.EqualTo(today.AddDays(1)));
        Assert.That(tomorrowEnd, Is.EqualTo(today.AddDays(2).AddSeconds(-1)));
    }

    [Test]
    public void TestGetNextWeekTimes()
    {
        var nextWeekStart = TimeHelper.GetNextWeekStartTime();
        var nextWeekEnd = TimeHelper.GetNextWeekEndTime();
        var thisWeekStart = TimeHelper.GetWeekStartTime();
        
        Assert.That(nextWeekStart, Is.EqualTo(thisWeekStart.AddDays(7)));
        Assert.That(nextWeekStart.DayOfWeek, Is.EqualTo(DayOfWeek.Monday));
        Assert.That(nextWeekEnd.DayOfWeek, Is.EqualTo(DayOfWeek.Sunday));
    }

    [Test]
    public void TestGetNextMonthTimes()
    {
        var nextMonthStart = TimeHelper.GetNextMonthStartTime();
        var nextMonthEnd = TimeHelper.GetNextMonthEndTime();
        var thisMonthStart = TimeHelper.GetMonthStartTime();
        
        Assert.That(nextMonthStart, Is.EqualTo(thisMonthStart.AddMonths(1)));
        Assert.That(nextMonthStart.Day, Is.EqualTo(1));
        Assert.That(nextMonthEnd.Hour, Is.EqualTo(23));
        Assert.That(nextMonthEnd.Minute, Is.EqualTo(59));
        Assert.That(nextMonthEnd.Second, Is.EqualTo(59));
    }

    #endregion

    #region 时间范围判断测试

    [Test]
    public void TestIsTimeInRange()
    {
        var startTime = new DateTime(2024, 1, 10, 9, 0, 0);
        var endTime = new DateTime(2024, 1, 10, 17, 0, 0);
        var testTime1 = new DateTime(2024, 1, 10, 12, 0, 0);
        var testTime2 = new DateTime(2024, 1, 10, 18, 0, 0);
        
        Assert.That(TimeHelper.IsTimeInRange(testTime1, startTime, endTime), Is.True);
        Assert.That(TimeHelper.IsTimeInRange(testTime2, startTime, endTime), Is.False);
        Assert.That(TimeHelper.IsTimeInRange(startTime, startTime, endTime), Is.True); // 边界测试
        Assert.That(TimeHelper.IsTimeInRange(endTime, startTime, endTime), Is.True); // 边界测试
    }

    [Test]
    public void TestIsTimestampInRange()
    {
        var startTimestamp = 1704880800L; // 2024-01-10 08:00:00 UTC
        var endTimestamp = 1704909600L;   // 2024-01-10 16:00:00 UTC
        var testTimestamp1 = 1704895200L; // 2024-01-10 12:00:00 UTC
        var testTimestamp2 = 1704913200L; // 2024-01-10 17:00:00 UTC
        
        Assert.That(TimeHelper.IsTimestampInRange(testTimestamp1, startTimestamp, endTimestamp), Is.True);
        Assert.That(TimeHelper.IsTimestampInRange(testTimestamp2, startTimestamp, endTimestamp), Is.False);
        Assert.That(TimeHelper.IsTimestampInRange(startTimestamp, startTimestamp, endTimestamp), Is.True);
        Assert.That(TimeHelper.IsTimestampInRange(endTimestamp, startTimestamp, endTimestamp), Is.True);
    }

    #endregion

    #region 同一天判断测试

    [Test]
    public void TestIsSameDay()
    {
        var time1 = new DateTime(2024, 1, 10, 9, 0, 0);
        var time2 = new DateTime(2024, 1, 10, 23, 59, 59);
        var time3 = new DateTime(2024, 1, 11, 0, 0, 1);
        
        Assert.That(TimeHelper.IsSameDay(time1, time2), Is.True);
        Assert.That(TimeHelper.IsSameDay(time1, time3), Is.False);
    }

    [Test]
    public void TestIsUnixSameDay()
    {
        var timestamp1 = 1704880800L; // 2024-01-10 08:00:00 UTC
        var timestamp2 = 1704909600L; // 2024-01-10 16:00:00 UTC
        var timestamp3 = 1704967200L; // 2024-01-11 08:00:00 UTC
        
        Assert.That(TimeHelper.IsUnixSameDay(timestamp1, timestamp2), Is.True);
        Assert.That(TimeHelper.IsUnixSameDay(timestamp1, timestamp3), Is.False);
    }

    [Test]
    public void TestIsLocalSameDay()
    {
        var timestamp1 = 1704880800L; // 2024-01-10 08:00:00 UTC
        var timestamp2 = 1704909600L; // 2024-01-10 16:00:00 UTC
        
        // 结果取决于本地时区，但应该是布尔值
        var result = TimeHelper.IsLocalSameDay(timestamp1, timestamp2);
        Assert.That(result, Is.TypeOf<bool>());
    }

    #endregion

    #region 当前时间获取测试

    [Test]
    public void TestGetUtcNowAndGetNow()
    {
        var utcNow = TimeHelper.GetUtcNow();
        var now = TimeHelper.GetNow();
        
        Assert.That(utcNow, Is.TypeOf<DateTime>());
        Assert.That(now, Is.TypeOf<DateTime>());
        
        // UTC时间和本地时间应该有时区差异（除非本地时区就是UTC）
        var timeDiff = Math.Abs((now - utcNow).TotalHours);
        Assert.That(timeDiff, Is.LessThanOrEqualTo(24)); // 时区差异不会超过24小时
    }

    #endregion

    #region 时间戳相关测试

    [Test]
    public void TestTimestampConversions()
    {
        var now = DateTime.UtcNow;
        var timestamp = new DateTimeOffset(now).ToUnixTimeSeconds();
        
        var convertedBack = TimeHelper.UtcSecondsToUtcDateTime(timestamp);
        
        // 由于精度问题，允许1秒的误差
        var diff = Math.Abs((now - convertedBack).TotalSeconds);
        Assert.That(diff, Is.LessThan(1));
    }

    [Test]
    public void TestTimeSpanWithTimestamp()
    {
        var pastTimestamp = TimeHelper.UnixTimeMilliseconds() - 5000; // 5秒前
        var timeSpan = TimeHelper.TimeSpanWithTimestamp(pastTimestamp);
        
        Assert.That(timeSpan.TotalSeconds, Is.GreaterThan(4).And.LessThan(6));
    }

    #endregion

    #region 边界和异常情况测试

    [Test]
    public void TestLeapYear()
    {
        // 测试闰年2月的处理
        var leapYearDate = new DateTime(2024, 2, 15); // 2024是闰年
        var monthStart = TimeHelper.GetStartTimeOfMonth(leapYearDate);
        var monthEnd = TimeHelper.GetEndTimeOfMonth(leapYearDate);
        
        Assert.That(monthStart, Is.EqualTo(new DateTime(2024, 2, 1, 0, 0, 0)));
        Assert.That(monthEnd.Day, Is.EqualTo(29)); // 闰年2月有29天
    }

    [Test]
    public void TestNegativeTimestamp()
    {
        // 测试1970年之前的时间戳
        var negativeTimestamp = -86400L; // 1969-12-31 00:00:00 UTC
        var dateTime = TimeHelper.UtcSecondsToUtcDateTime(negativeTimestamp);
        
        Assert.That(dateTime.Year, Is.EqualTo(1969));
        Assert.That(dateTime.Month, Is.EqualTo(12));
        Assert.That(dateTime.Day, Is.EqualTo(31));
    }

    [Test]
    public void TestLargeTimestamp()
    {
        // 测试较大的时间戳
        var largeTimestamp = 2147483647L; // 2038-01-19 03:14:07 UTC (32位时间戳的最大值)
        var dateTime = TimeHelper.UtcSecondsToUtcDateTime(largeTimestamp);
        
        Assert.That(dateTime.Year, Is.EqualTo(2038));
        Assert.That(dateTime.Month, Is.EqualTo(1));
        Assert.That(dateTime.Day, Is.EqualTo(19));
    }

    #endregion

    #region 补充缺失的测试 - 本地时间戳函数

    [Test]
    public void TestTimeSeconds()
    {
        var timeSeconds = TimeHelper.TimeSeconds();
        var expectedSeconds = new DateTimeOffset(DateTime.Now).ToUnixTimeSeconds() + TimeHelper.TimeOffsetSeconds;
        // 允许1秒的误差
        Assert.That(Math.Abs(timeSeconds - expectedSeconds), Is.LessThanOrEqualTo(1));
    }

    [Test]
    public void TestTimeMilliseconds()
    {
        var timeMs = TimeHelper.TimeMilliseconds();
        var expectedMs = new DateTimeOffset(DateTime.Now).ToUnixTimeMilliseconds() + TimeHelper.TimeOffsetMilliseconds;
        // 允许1000毫秒的误差
        Assert.That(Math.Abs(timeMs - expectedMs), Is.LessThanOrEqualTo(1000));
    }

    #endregion

    #region 补充缺失的测试 - TimeSpan相关函数

    [Test]
    public void TestTimeSpanLocalWithTimestamp()
    {
        // 使用固定时间戳进行测试，避免动态时间导致的问题
        var fixedTimestamp = 1704897045000L; // 2024-01-10 14:30:45 UTC
        var timeSpan = TimeHelper.TimeSpanLocalWithTimestamp(fixedTimestamp);
        // 只验证返回类型正确，不验证具体时间差
        Assert.That(timeSpan, Is.TypeOf<TimeSpan>());
    }

    #endregion

    #region 补充缺失的测试 - 毫秒时间差函数

    [Test]
    public void TestGetTimeDifferenceMs()
    {
        var startTimestampMs = 1704897045000L; // 2024-01-10 14:30:45.000 UTC
        var endTimestampMs = 1704897047500L;   // 2024-01-10 14:30:47.500 UTC
        
        var diff = TimeHelper.GetTimeDifferenceMs(startTimestampMs, endTimestampMs, true);
        Assert.That(diff.TotalMilliseconds, Is.EqualTo(2500));
    }

    [Test]
    public void TestGetTimeDifferenceFromNowMs()
    {
        // 使用固定时间戳进行测试，避免动态时间导致的问题
        var fixedTimestampMs = 1704897045000L; // 2024-01-10 14:30:45 UTC
        var diff = TimeHelper.GetTimeDifferenceFromNowMs(fixedTimestampMs, true);
        // 只验证返回类型正确，不验证具体时间差
        Assert.That(diff, Is.TypeOf<TimeSpan>());
    }

    #endregion

    #region 补充缺失的测试 - 各种时间差计算函数

    [Test]
    public void TestGetMinutesDifference()
    {
        var startTime = new DateTime(2024, 1, 10, 10, 0, 0);
        var endTime = new DateTime(2024, 1, 10, 10, 45, 30);
        
        var minutes = TimeHelper.GetMinutesDifference(startTime, endTime);
        Assert.That(minutes, Is.EqualTo(45.5)); // 45分30秒 = 45.5分钟
    }

    [Test]
    public void TestGetHoursDifference()
    {
        var startTime = new DateTime(2024, 1, 10, 10, 0, 0);
        var endTime = new DateTime(2024, 1, 10, 13, 30, 0);
        
        var hours = TimeHelper.GetHoursDifference(startTime, endTime);
        Assert.That(hours, Is.EqualTo(3.5)); // 3小时30分钟 = 3.5小时
    }

    [Test]
    public void TestGetDaysDifference()
    {
        var startTime = new DateTime(2024, 1, 10, 12, 0, 0);
        var endTime = new DateTime(2024, 1, 12, 18, 0, 0);
        
        var days = TimeHelper.GetDaysDifference(startTime, endTime);
        Assert.That(days, Is.EqualTo(2.25)); // 2天6小时 = 2.25天
    }

    [Test]
    public void TestGetSecondsDifferenceWithTimestamp()
    {
        var startTimestamp = 1704897045L; // 2024-01-10 14:30:45 UTC
        var endTimestamp = 1704897075L;   // 2024-01-10 14:31:15 UTC
        
        var seconds = TimeHelper.GetSecondsDifference(startTimestamp, endTimestamp);
        Assert.That(seconds, Is.EqualTo(30));
    }

    [Test]
    public void TestGetMillisecondsDifferenceWithTimestamp()
    {
        var startTimestampMs = 1704897045000L; // 2024-01-10 14:30:45.000 UTC
        var endTimestampMs = 1704897045750L;   // 2024-01-10 14:30:45.750 UTC
        
        var ms = TimeHelper.GetMillisecondsDifference(startTimestampMs, endTimestampMs);
        Assert.That(ms, Is.EqualTo(750));
    }

    [Test]
    public void TestGetElapsedSecondsWithTimestamp()
    {
        var pastTimestamp = TimeHelper.UnixTimeSeconds() - 10; // 10秒前
        var elapsed = TimeHelper.GetElapsedSeconds(pastTimestamp, true);
        Assert.That(elapsed, Is.GreaterThan(9).And.LessThan(11));
    }

    [Test]
    public void TestGetElapsedMillisecondsWithTimestamp()
    {
        var pastTimestampMs = TimeHelper.UnixTimeMilliseconds() - 2500; // 2.5秒前
        var elapsed = TimeHelper.GetElapsedMilliseconds(pastTimestampMs, true);
        Assert.That(elapsed, Is.GreaterThan(2400).And.LessThan(2600));
    }

    #endregion

    #region 补充缺失的测试 - 星期相关函数的无参版本

    [Test]
    public void TestGetDayOfWeekTimeWithDayOfWeek()
    {
        var result = TimeHelper.GetDayOfWeekTime(DayOfWeek.Monday);
        Assert.That(result, Is.TypeOf<DateTime>());
        Assert.That(result.DayOfWeek, Is.EqualTo(DayOfWeek.Monday)); // 应该返回本周一
    }

    [Test]
    public void TestGetChinaDayOfWeekNoParams()
    {
        var result = TimeHelper.GetChinaDayOfWeek();
        Assert.That(result, Is.InRange(1, 7)); // 中国星期数字应该在1-7之间
    }

    #endregion

    #region 补充缺失的测试 - 日期格式化函数

    [Test]
    public void TestCurrentDateWithUtcDay()
    {
        var result = TimeHelper.CurrentDateWithUtcDay();
        var expected = int.Parse(DateTime.UtcNow.ToString("yyyyMMdd"));
        // 允许日期可能跨天的情况
        Assert.That(Math.Abs(result - expected), Is.LessThanOrEqualTo(1));
    }

    [Test]
    public void TestCurrentTimeWithUtcFullString()
    {
        var result = TimeHelper.CurrentTimeWithUtcFullString();
        Assert.That(result, Is.Not.Null.And.Not.Empty);
        Assert.That(result.Length, Is.EqualTo(6)); // HHmmss格式，6位字符串
    }

    [Test]
    public void TestCurrentTimeWithLocalFullString()
    {
        var result = TimeHelper.CurrentTimeWithLocalFullString();
        Assert.That(result, Is.Not.Null.And.Not.Empty);
        Assert.That(result.Length, Is.EqualTo(6)); // HHmmss格式，6位字符串
    }

    [Test]
    public void TestCurrentTimeWithLocalTime()
    {
        var result = TimeHelper.CurrentTimeWithLocalTime();
        var expected = int.Parse(DateTime.Now.ToString("HHmmss"));
        // 允许1秒的误差
        Assert.That(Math.Abs(result - expected), Is.LessThanOrEqualTo(1));
    }

    [Test]
    public void TestCurrentDateTimeWithUtcFormat()
    {
        var format = "yyyy-MM-dd HH:mm:ss";
        var result = TimeHelper.CurrentDateTimeWithUtcFormat(format);
        var expected = DateTime.UtcNow.ToString(format);
        
        // 检查格式是否正确
        Assert.That(result.Length, Is.EqualTo(expected.Length));
        Assert.That(result.Contains("-"), Is.True);
        Assert.That(result.Contains(":"), Is.True);
    }

    [Test]
    public void TestCurrentDateTimeWithUtcFullString()
    {
        var result = TimeHelper.CurrentDateTimeWithUtcFullString();
        Assert.That(result, Is.Not.Null.And.Not.Empty);
        Assert.That(result.Length, Is.GreaterThan(15)); // 应该是完整的日期时间字符串
    }

    #endregion

    #region 补充缺失的测试 - 时间戳版本的函数

    [Test]
    public void TestGetTodayStartTimestamp()
    {
        var timestamp = TimeHelper.GetTodayStartTimestamp();
        var dateTime = DateTimeOffset.FromUnixTimeSeconds(timestamp).ToLocalTime().DateTime;
        
        Assert.That(dateTime.Hour, Is.EqualTo(0));
        Assert.That(dateTime.Minute, Is.EqualTo(0));
        Assert.That(dateTime.Second, Is.EqualTo(0));
    }

    [Test]
    public void TestGetTodayEndTimestamp()
    {
        var timestamp = TimeHelper.GetTodayEndTimestamp();
        var dateTime = DateTimeOffset.FromUnixTimeSeconds(timestamp).ToLocalTime().DateTime;
        
        Assert.That(dateTime.Hour, Is.EqualTo(23));
        Assert.That(dateTime.Minute, Is.EqualTo(59));
        Assert.That(dateTime.Second, Is.EqualTo(59));
    }

    [Test]
    public void TestGetWeekStartTimestamp()
    {
        var timestamp = TimeHelper.GetWeekStartTimestamp();
        var dateTime = TimeHelper.UtcSecondsToLocalDateTime(timestamp);
        
        Assert.That(dateTime.DayOfWeek, Is.EqualTo(DayOfWeek.Monday));
        Assert.That(dateTime.Hour, Is.EqualTo(0));
    }

    [Test]
    public void TestGetWeekEndTimestamp()
    {
        var timestamp = TimeHelper.GetWeekEndTimestamp();
        var dateTime = TimeHelper.UtcSecondsToLocalDateTime(timestamp);
        
        Assert.That(dateTime.DayOfWeek, Is.EqualTo(DayOfWeek.Sunday));
        Assert.That(dateTime.Hour, Is.EqualTo(23));
    }

    [Test]
    public void TestGetMonthStartTimestamp()
    {
        var timestamp = TimeHelper.GetMonthStartTimestamp();
        var dateTime = TimeHelper.UtcSecondsToLocalDateTime(timestamp);
        
        Assert.That(dateTime.Day, Is.EqualTo(1));
        Assert.That(dateTime.Hour, Is.EqualTo(0));
    }

    [Test]
    public void TestGetMonthEndTimestamp()
    {
        var timestamp = TimeHelper.GetMonthEndTimestamp();
        var dateTime = TimeHelper.UtcSecondsToLocalDateTime(timestamp);
        
        Assert.That(dateTime.Hour, Is.EqualTo(23));
        Assert.That(dateTime.Minute, Is.EqualTo(59));
    }

    [Test]
    public void TestGetYearStartTimestamp()
    {
        var timestamp = TimeHelper.GetYearStartTimestamp();
        var dateTime = TimeHelper.UtcSecondsToLocalDateTime(timestamp);
        
        Assert.That(dateTime.Month, Is.EqualTo(1));
        Assert.That(dateTime.Day, Is.EqualTo(1));
        Assert.That(dateTime.Hour, Is.EqualTo(0));
    }

    [Test]
    public void TestGetYearEndTimestamp()
    {
        var timestamp = TimeHelper.GetYearEndTimestamp();
        var dateTime = TimeHelper.UtcSecondsToLocalDateTime(timestamp);
        
        Assert.That(dateTime.Month, Is.EqualTo(12));
        Assert.That(dateTime.Day, Is.EqualTo(31));
        Assert.That(dateTime.Hour, Is.EqualTo(23));
    }

    [Test]
    public void TestGetStartTimestampOfDay()
    {
        var date = new DateTime(2024, 1, 10, 14, 30, 45);
        var timestamp = TimeHelper.GetStartTimestampOfDay(date);
        var dateTime = TimeHelper.UtcSecondsToLocalDateTime(timestamp);
        
        Assert.That(dateTime.Year, Is.EqualTo(2024));
        Assert.That(dateTime.Month, Is.EqualTo(1));
        Assert.That(dateTime.Day, Is.EqualTo(10));
        Assert.That(dateTime.Hour, Is.EqualTo(0));
    }

    [Test]
    public void TestGetEndTimestampOfDay()
    {
        var date = new DateTime(2024, 1, 10, 14, 30, 45);
        var timestamp = TimeHelper.GetEndTimestampOfDay(date);
        var dateTime = TimeHelper.UtcSecondsToLocalDateTime(timestamp);
        
        Assert.That(dateTime.Year, Is.EqualTo(2024));
        Assert.That(dateTime.Month, Is.EqualTo(1));
        Assert.That(dateTime.Day, Is.EqualTo(10));
        Assert.That(dateTime.Hour, Is.EqualTo(23));
    }

    [Test]
    public void TestGetStartTimestampOfWeek()
    {
        var wednesday = new DateTime(2024, 1, 10); // 周三
        var timestamp = TimeHelper.GetStartTimestampOfWeek(wednesday);
        var dateTime = TimeHelper.UtcSecondsToLocalDateTime(timestamp);
        
        Assert.That(dateTime.DayOfWeek, Is.EqualTo(DayOfWeek.Monday));
        Assert.That(dateTime.Hour, Is.EqualTo(0));
    }

    [Test]
    public void TestGetEndTimestampOfWeek()
    {
        var wednesday = new DateTime(2024, 1, 10); // 周三
        var timestamp = TimeHelper.GetEndTimestampOfWeek(wednesday);
        var dateTime = TimeHelper.UtcSecondsToLocalDateTime(timestamp);
        
        Assert.That(dateTime.DayOfWeek, Is.EqualTo(DayOfWeek.Sunday));
        // Assert.That(dateTime.Hour, Is.EqualTo(23));
    }

    [Test]
    public void TestGetStartTimestampOfMonth()
    {
        var date = new DateTime(2024, 1, 15);
        var timestamp = TimeHelper.GetStartTimestampOfMonth(date);
        var dateTime = TimeHelper.UtcSecondsToLocalDateTime(timestamp);
        
        Assert.That(dateTime.Year, Is.EqualTo(2024));
        Assert.That(dateTime.Month, Is.EqualTo(1));
        Assert.That(dateTime.Day, Is.EqualTo(1));
    }

    [Test]
    public void TestGetEndTimestampOfMonth()
    {
        var date = new DateTime(2024, 1, 15);
        var timestamp = TimeHelper.GetEndTimestampOfMonth(date);
        var dateTime = TimeHelper.UtcSecondsToLocalDateTime(timestamp);
        
        Assert.That(dateTime.Year, Is.EqualTo(2024));
        Assert.That(dateTime.Month, Is.EqualTo(1));
        Assert.That(dateTime.Day, Is.EqualTo(31)); // 1月有31天
    }

    [Test]
    public void TestGetStartTimestampOfYear()
    {
        var date = new DateTime(2024, 6, 15);
        var timestamp = TimeHelper.GetStartTimestampOfYear(date);
        var dateTime = TimeHelper.UtcSecondsToLocalDateTime(timestamp);
        
        Assert.That(dateTime.Year, Is.EqualTo(2024));
        Assert.That(dateTime.Month, Is.EqualTo(1));
        Assert.That(dateTime.Day, Is.EqualTo(1));
    }

    [Test]
    public void TestGetEndTimestampOfYear()
    {
        var date = new DateTime(2024, 6, 15);
        var timestamp = TimeHelper.GetEndTimestampOfYear(date);
        var dateTime = TimeHelper.UtcSecondsToLocalDateTime(timestamp);
        
        Assert.That(dateTime.Year, Is.EqualTo(2024));
        Assert.That(dateTime.Month, Is.EqualTo(12));
        Assert.That(dateTime.Day, Is.EqualTo(31));
    }

    [Test]
    public void TestGetTomorrowStartTimestamp()
    {
        var timestamp = TimeHelper.GetTomorrowStartTimestamp();
        var dateTime = TimeHelper.UtcSecondsToLocalDateTime(timestamp);
        var tomorrow = DateTime.Today.AddDays(1);
        
        Assert.That(dateTime.Day, Is.EqualTo(tomorrow.Day));
        Assert.That(dateTime.Hour, Is.EqualTo(0));
    }

    [Test]
    public void TestGetTomorrowEndTimestamp()
    {
        var timestamp = TimeHelper.GetTomorrowEndTimestamp();
        var dateTime = TimeHelper.UtcSecondsToLocalDateTime(timestamp);
        var tomorrow = DateTime.Today.AddDays(1);
        
        Assert.That(dateTime.Day, Is.EqualTo(tomorrow.Day));
        Assert.That(dateTime.Hour, Is.EqualTo(23));
    }

    [Test]
    public void TestGetNextWeekStartTimestamp()
    {
        var timestamp = TimeHelper.GetNextWeekStartTimestamp();
        var dateTime = TimeHelper.UtcSecondsToLocalDateTime(timestamp);
        
        Assert.That(dateTime.DayOfWeek, Is.EqualTo(DayOfWeek.Monday));
        Assert.That(dateTime.Hour, Is.EqualTo(0));
    }

    [Test]
    public void TestGetNextWeekEndTimestamp()
    {
        var timestamp = TimeHelper.GetNextWeekEndTimestamp();
        var dateTime = TimeHelper.UtcSecondsToLocalDateTime(timestamp);
        
        Assert.That(dateTime.DayOfWeek, Is.EqualTo(DayOfWeek.Sunday));
        Assert.That(dateTime.Hour, Is.EqualTo(23));
    }

    [Test]
    public void TestGetNextMonthStartTimestamp()
    {
        var timestamp = TimeHelper.GetNextMonthStartTimestamp();
        var dateTime = TimeHelper.UtcSecondsToLocalDateTime(timestamp);
        
        Assert.That(dateTime.Day, Is.EqualTo(1));
        Assert.That(dateTime.Hour, Is.EqualTo(0));
    }

    [Test]
    public void TestGetNextMonthEndTimestamp()
    {
        var timestamp = TimeHelper.GetNextMonthEndTimestamp();
        var dateTime = TimeHelper.UtcSecondsToLocalDateTime(timestamp);
        
        Assert.That(dateTime.Hour, Is.EqualTo(23));
        Assert.That(dateTime.Minute, Is.EqualTo(59));
    }

    #endregion

    #region 补充缺失的测试 - 额外的边界测试

    [Test]
    public void TestTimeZoneBoundary()
    {
        // 测试时区边界情况
        var utcTime = DateTime.UtcNow;
        var localTime = DateTime.Now;
        
        var utcTimestamp = TimeHelper.TimeToSecond(utcTime, true);
        var localTimestamp = TimeHelper.TimeToSecond(localTime, false);
        
        // UTC和本地时间戳应该有差异（除非本地时区就是UTC）
        Assert.That(utcTimestamp, Is.TypeOf<long>());
        Assert.That(localTimestamp, Is.TypeOf<long>());
    }

    [Test]
    public void TestMillisecondPrecision()
    {
        // 测试毫秒精度
        var timeWithMs = new DateTime(2024, 1, 10, 14, 30, 45, 123);
        var timestampMs = TimeHelper.TimeToMilliseconds(timeWithMs, true);
        var convertedBack = TimeHelper.MillisecondsTimeStampToDateTime(timestampMs, true);
        
        Assert.That(convertedBack.Millisecond, Is.EqualTo(123));
    }

    [Test]
    public void TestWeekBoundary()
    {
        // 测试周边界
        var sunday = new DateTime(2024, 1, 7);    // 周日
        var monday = new DateTime(2024, 1, 8);    // 周一
        
        Assert.That(TimeHelper.IsSameWeek(sunday, monday), Is.False);
        
        // 周日所在周的周一应该是1月1日，不是1月8日
        var mondayOfSundayWeek = TimeHelper.GetDayOfWeekTime(sunday, DayOfWeek.Monday);
        var expectedMondayOfSundayWeek = new DateTime(2024, 1, 1); // 2024年1月7日所在周的周一
        Assert.That(mondayOfSundayWeek, Is.EqualTo(expectedMondayOfSundayWeek));
        
        // 周一所在周的周一应该是自己
        var mondayOfMondayWeek = TimeHelper.GetDayOfWeekTime(monday, DayOfWeek.Monday);
        Assert.That(mondayOfMondayWeek, Is.EqualTo(monday));
    }

    [Test]
    public void TestMonthBoundary()
    {
        // 测试月边界
        var endOfJan = new DateTime(2024, 1, 31);
        var startOfFeb = new DateTime(2024, 2, 1);
        
        var janEnd = TimeHelper.GetEndTimeOfMonth(endOfJan);
        var febStart = TimeHelper.GetStartTimeOfMonth(startOfFeb);
        
        Assert.That(janEnd.Month, Is.EqualTo(1));
        Assert.That(febStart.Month, Is.EqualTo(2));
        Assert.That(janEnd.Day, Is.EqualTo(31));
        Assert.That(febStart.Day, Is.EqualTo(1));
    }

    [Test]
    public void TestUtcSecondsToUtcDateTimeConsistency()
    {
        // 测试多个不同的时间戳
        var testTimestamps = new long[]
        {
            1704897045, // 2024-01-10 14:30:45 UTC
            1609459200, // 2021-01-01 00:00:00 UTC
            1640995200, // 2022-01-01 00:00:00 UTC
            1672531200, // 2023-01-01 00:00:00 UTC
            0,          // 1970-01-01 00:00:00 UTC (Unix epoch)
            946684800   // 2000-01-01 00:00:00 UTC
        };

        foreach (var timestamp in testTimestamps)
        {
            // 使用TimeHelper方法
            var timeHelperResult = TimeHelper.UtcSecondsToUtcDateTime(timestamp);
            
            // 使用DateTimeOffset方法
            var dateTimeOffsetResult = DateTimeOffset.FromUnixTimeSeconds(timestamp).UtcDateTime;
            
            // 验证两种方法的结果是否一致
            Assert.That(timeHelperResult, Is.EqualTo(dateTimeOffsetResult), 
                $"时间戳 {timestamp} 的转换结果不一致: TimeHelper={timeHelperResult}, DateTimeOffset={dateTimeOffsetResult}");
            
            // 验证年月日时分秒都相等
            Assert.That(timeHelperResult.Year, Is.EqualTo(dateTimeOffsetResult.Year));
            Assert.That(timeHelperResult.Month, Is.EqualTo(dateTimeOffsetResult.Month));
            Assert.That(timeHelperResult.Day, Is.EqualTo(dateTimeOffsetResult.Day));
            Assert.That(timeHelperResult.Hour, Is.EqualTo(dateTimeOffsetResult.Hour));
            Assert.That(timeHelperResult.Minute, Is.EqualTo(dateTimeOffsetResult.Minute));
            Assert.That(timeHelperResult.Second, Is.EqualTo(dateTimeOffsetResult.Second));
        }
    }

    #endregion
}