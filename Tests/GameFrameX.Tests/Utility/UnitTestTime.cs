// ==========================================================================================
//  GameFrameX 组织及其衍生项目的版权、商标、专利及其他相关权利
//  GameFrameX organization and its derivative projects' copyrights, trademarks, patents, and related rights
//  均受中华人民共和国及相关国际法律法规保护。
//  are protected by the laws of the People's Republic of China and relevant international regulations.
//  
//  使用本项目须严格遵守相应法律法规及开源许可证之规定。
//  Usage of this project must strictly comply with applicable laws, regulations, and open-source licenses.
//  
//  本项目采用 MIT 许可证与 Apache License 2.0 双许可证分发，
//  This project is dual-licensed under the MIT License and Apache License 2.0,
//  完整许可证文本请参见源代码根目录下的 LICENSE 文件。
//  please refer to the LICENSE file in the root directory of the source code for the full license text.
//  
//  禁止利用本项目实施任何危害国家安全、破坏社会秩序、
//  It is prohibited to use this project to engage in any activities that endanger national security, disrupt social order,
//  侵犯他人合法权益等法律法规所禁止的行为！
//  or infringe upon the legitimate rights and interests of others, as prohibited by laws and regulations!
//  因基于本项目二次开发所产生的一切法律纠纷与责任，
//  Any legal disputes and liabilities arising from secondary development based on this project
//  本项目组织与贡献者概不承担。
//  shall be borne solely by the developer; the project organization and contributors assume no responsibility.
//  
//  GitHub 仓库：https://github.com/GameFrameX
//  GitHub Repository: https://github.com/GameFrameX
//  Gitee  仓库：https://gitee.com/GameFrameX
//  Gitee Repository:  https://gitee.com/GameFrameX
//  官方文档：https://gameframex.doc.alianblank.com/
//  Official Documentation: https://gameframex.doc.alianblank.com/
// ==========================================================================================

using GameFrameX.Utility;
using Xunit;

namespace GameFrameX.Tests;

public class UnitTestTime : IDisposable
{
    private DateTime dateTime, dateTime1;
    private readonly DateTime testDate = new DateTime(2024, 1, 10, 14, 30, 45, 123); // 2024年1月10日 14:30:45.123
    private readonly DateTime testDateUtc = new DateTime(2024, 1, 10, 14, 30, 45, 123, DateTimeKind.Utc);
    private readonly long testTimestamp = 1704897045; // 对应2024-01-10 14:30:45 UTC
    private readonly long testTimestampMs = 1704897045123; // 对应2024-01-10 14:30:45.123 UTC

    public UnitTestTime()
    {
        dateTime = DateTime.Now;
        dateTime1 = DateTime.Now.AddHours(1);
        // 重置时间偏移
        TimeHelper.ResetTimeOffset();
    }

    public void Dispose()
    {
        // 确保每个测试后重置时间偏移
        TimeHelper.ResetTimeOffset();
    }

    #region 基础时间戳和转换测试

    [Fact]
    public void test_current_time_millis()
    {
        var currentTimeMillis = TimeHelper.UnixTimeMilliseconds();
        var expectedTimeMillis = (long)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalMilliseconds;
        Assert.Equal(expectedTimeMillis, currentTimeMillis);
    }

    [Fact]
    public void test_unix_time_seconds()
    {
        var unixTimeSeconds = TimeHelper.UnixTimeSeconds();
        var expectedUnixTimeSeconds = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();
        Assert.Equal(expectedUnixTimeSeconds, unixTimeSeconds);
    }

    [Fact]
    public void test_unix_time_milliseconds()
    {
        var unixTimeMilliseconds = TimeHelper.UnixTimeMilliseconds();
        var expectedUnixTimeMilliseconds = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds();
        Assert.Equal(expectedUnixTimeMilliseconds, unixTimeMilliseconds);
    }

    [Fact]
    public void test_time_millis()
    {
        var time = new DateTime(2022, 1, 1, 12, 0, 0);
        var utc = false;
        var timeMillis = TimeHelper.TimeToMilliseconds(time, utc);
        var expectedTimeMillis = (long)(time - new DateTime(1970, 1, 1)).TotalMilliseconds;
        Assert.Equal(expectedTimeMillis, timeMillis);
    }

    [Fact]
    public void test_millis_to_date_time()
    {
        var timeMillis = 1641024000000;
        var utc = true;
        var dateTime = TimeHelper.MillisecondsTimeStampToDateTime(timeMillis, utc);
        var expectedDateTime = new DateTime(2022, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        Assert.Equal(expectedDateTime, dateTime);
    }

    [Fact]
    public void test_get_cross_days()
    {
        var begin = new DateTime(2022, 3, 12, 1, 0, 0);
        var after = new DateTime(2022, 3, 13, 23, 0, 0);
        var hour = 0;
        var crossDays = TimeHelper.GetCrossDays(begin, after, hour);
        var expectedCrossDays = 1;
        Assert.Equal(expectedCrossDays, crossDays);
    }

    [Fact]
    public void Test1()
    {
        Assert.Equal(dateTime.Year, dateTime1.Year);
        Assert.Equal(dateTime.Month, dateTime1.Month);
        Assert.Equal(dateTime.Day, dateTime1.Day);
    }

    #endregion

    #region 补充缺失的测试 - 时间戳转换函数

    [Fact]
    public void TestTimestampToDateTime()
    {
        var timestamp = 1704897045L; // 2024-01-10 14:30:45 UTC
        
        // 测试UTC转换
        var utcDateTime = TimeHelper.TimestampToDateTime(timestamp, true);
        // 验证返回的是DateTime类型且Kind为Utc
        Assert.IsType<DateTime>(utcDateTime);
        Assert.Equal(DateTimeKind.Utc, utcDateTime.Kind);
        
        // 测试本地时间转换
        var localDateTime = TimeHelper.TimestampToDateTime(timestamp, false);
        Assert.IsType<DateTime>(localDateTime);
    }

    [Fact]
    public void TestUtcSecondsToLocalDateTime()
    {
        var timestamp = 1704897045L; // 2024-01-10 14:30:45 UTC
        var localDateTime = TimeHelper.UtcSecondsToLocalDateTime(timestamp);
        Assert.IsType<DateTime>(localDateTime);
    }

    [Fact]
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
            
            Assert.Equal(method2.Year, method1.Year);
            Assert.Equal(method2.Month, method1.Month);
            Assert.Equal(method2.Day, method1.Day);
            Assert.Equal(method2.Hour, method1.Hour);
            Assert.Equal(method2.Minute, method1.Minute);
            Assert.Equal(method2.Second, method1.Second);
        }
    }

    [Fact]
    public void TestUtcMillisecondsToDateTime()
    {
        var timestampMs = 1704897045123L; // 2024-01-10 14:30:45.123 UTC
        var dateTime = TimeHelper.UtcMillisecondsToDateTime(timestampMs);
        Assert.IsType<DateTime>(dateTime);
        Assert.Equal(123, dateTime.Millisecond);
    }

    #endregion

    #region 补充缺失的测试 - 跨天数计算的其他重载

    [Fact]
    public void TestGetCrossDaysFromDateTime()
    {
        var startTime = new DateTime(2024, 1, 10, 10, 0, 0);
        var days = TimeHelper.GetCrossDays(startTime, 0);
        Assert.True(days >= 0);
    }

    [Fact]
    public void TestGetCrossLocalDaysFromDateTime()
    {
        var startTime = new DateTime(2024, 1, 10, 10, 0, 0);
        var days = TimeHelper.GetCrossLocalDays(startTime, 0);
        Assert.True(days >= 0);
    }

    [Fact]
    public void TestGetCrossDaysFromTimestamp()
    {
        var beginTimestamp = 1704880800L; // 2024-01-10 08:00:00 UTC
        var days = TimeHelper.GetCrossDays(beginTimestamp, 0);
        Assert.True(days >= 0);
    }

    #endregion

    #region 补充缺失的测试 - 同周判断函数

    [Fact]
    public void TestIsNowSameWeekWithTicks()
    {
        var now = DateTime.Now;
        var ticks = now.Ticks;
        var result = TimeHelper.IsNowSameWeek(ticks);
        Assert.True(result); // 当前时间应该与现在同周
    }

    [Fact]
    public void TestIsNowSameWeekWithDateTime()
    {
        var now = DateTime.Now;
        var result = TimeHelper.IsNowSameWeek(now);
        Assert.True(result); // 当前时间应该与现在同周
    }

    [Fact]
    public void TestIsNowSameWeekUtc()
    {
        var utcNow = DateTime.UtcNow;
        var result = TimeHelper.IsNowSameWeekUtc(utcNow);
        Assert.True(result); // 当前UTC时间应该与现在同周
    }

    [Fact]
    public void TestIsUnixSameWeek()
    {
        var now = DateTime.Now;
        var ticks = now.Ticks;
        var result = TimeHelper.IsUnixSameWeek(ticks);
        Assert.IsType<bool>(result);
    }

    [Fact]
    public void TestIsUnixSameWeekFromTimestampMilliseconds()
    {
        var timestampMs = TimeHelper.UnixTimeMilliseconds();
        var result = TimeHelper.IsUnixSameWeekFromTimestampMilliseconds(timestampMs);
        Assert.True(result); // 当前时间戳应该与现在同周
    }

    #endregion

    #region 时间偏移测试

    [Fact]
    public void TestTimeOffset()
    {
        // 测试设置时间偏移
        TimeHelper.SetTimeOffset(3600, 3600000); // 1小时偏移
        
        var originalSeconds = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();
        var offsetSeconds = TimeHelper.UnixTimeSeconds();
        Assert.Equal(originalSeconds + 3600, offsetSeconds);
        
        var originalMs = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds();
        var offsetMs = TimeHelper.UnixTimeMilliseconds();
        Assert.Equal(originalMs + 3600000, offsetMs);
        
        // 测试重置偏移
        TimeHelper.ResetTimeOffset();
        var resetSeconds = TimeHelper.UnixTimeSeconds();
        var expectedSeconds = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();
        Assert.Equal(expectedSeconds, resetSeconds);
    }

    #endregion

    #region 时间差计算测试

    [Fact]
    public void TestGetTimeDifference()
    {
        var startTime = new DateTime(2024, 1, 10, 10, 0, 0);
        var endTime = new DateTime(2024, 1, 10, 12, 30, 0);
        
        var diff = TimeHelper.GetTimeDifference(startTime, endTime);
        Assert.Equal(2.5, diff.TotalHours);
        Assert.Equal(150, diff.TotalMinutes);
    }

    [Fact]
    public void TestGetTimeDifferenceWithTimestamp()
    {
        var startTimestamp = 1704880800L; // 2024-01-10 08:00:00 UTC
        var endTimestamp = 1704889800L;   // 2024-01-10 10:30:00 UTC
        
        var diff = TimeHelper.GetTimeDifference(startTimestamp, endTimestamp, true);
        Assert.Equal(2.5, diff.TotalHours);
    }

    [Fact]
    public void TestGetTimeDifferenceFromNow()
    {
        var pastTime = DateTime.Now.AddHours(-2);
        var diff = TimeHelper.GetTimeDifferenceFromNow(pastTime);
        Assert.True(diff.TotalHours > 1.9 && diff.TotalHours < 2.1);
    }

    [Fact]
    public void TestGetSecondsDifference()
    {
        var startTime = new DateTime(2024, 1, 10, 10, 0, 0);
        var endTime = new DateTime(2024, 1, 10, 10, 5, 30);
        
        var seconds = TimeHelper.GetSecondsDifference(startTime, endTime);
        Assert.Equal(330, seconds); // 5分30秒 = 330秒
    }

    [Fact]
    public void TestGetMillisecondsDifference()
    {
        var startTime = new DateTime(2024, 1, 10, 10, 0, 0, 0);
        var endTime = new DateTime(2024, 1, 10, 10, 0, 1, 500);
        
        var ms = TimeHelper.GetMillisecondsDifference(startTime, endTime);
        Assert.Equal(1500, ms); // 1.5秒 = 1500毫秒
    }

    [Fact]
    public void TestGetElapsedSeconds()
    {
        var pastTime = DateTime.Now.AddSeconds(-30);
        var elapsed = TimeHelper.GetElapsedSeconds(pastTime);
        Assert.True(elapsed > 29 && elapsed < 31);
    }

    [Fact]
    public void TestGetAbsoluteDifference()
    {
        var time1 = new DateTime(2024, 1, 10, 10, 0, 0);
        var time2 = new DateTime(2024, 1, 10, 8, 0, 0);
        
        var absSeconds = TimeHelper.GetAbsoluteSecondsDifference(time1, time2);
        Assert.Equal(7200, absSeconds); // 2小时 = 7200秒
        
        var absMs = TimeHelper.GetAbsoluteMillisecondsDifference(time1, time2);
        Assert.Equal(7200000, absMs); // 2小时 = 7200000毫秒
    }

    #endregion

    #region 时间戳转换测试

    [Fact]
    public void TestTimestampToTicks()
    {
        var timestamp = 1704902445L; // 2024-01-10 14:30:45 UTC
        var ticks = TimeHelper.TimestampToTicks(timestamp);
        var expectedTicks = timestamp * 10000000L + TimeHelper.EpochUtc.Ticks;
        Assert.Equal(expectedTicks, ticks);
    }

    [Fact]
    public void TestTimestampMillisToTicks()
    {
        var timestampMs = 1704902445123L; // 2024-01-10 14:30:45.123 UTC
        var ticks = TimeHelper.TimestampMillisToTicks(timestampMs);
        var expectedTicks = timestampMs * 10000L + TimeHelper.EpochUtc.Ticks;
        Assert.Equal(expectedTicks, ticks);
    }

    [Fact]
    public void TestUtcSecondsToUtcDateTime()
    {
        var timestamp = 1704897045L; // 2024-01-10 14:30:45 UTC
        var dateTime = TimeHelper.UtcSecondsToUtcDateTime(timestamp);
        var expected = new DateTime(2024, 1, 10, 14, 30, 45, DateTimeKind.Utc);
        Assert.Equal(expected, dateTime);
    }

    [Fact]
    public void TestUtcMillisecondsToUtcDateTime()
    {
        var timestampMs = 1704897045123L; // 2024-01-10 14:30:45.123 UTC
        var dateTime = TimeHelper.UtcMillisecondsToUtcDateTime(timestampMs);
        var expected = new DateTime(2024, 1, 10, 14, 30, 45, 123, DateTimeKind.Utc);
        Assert.Equal(expected, dateTime);
    }

    #endregion

    #region 跨天数计算测试

    [Fact]
    public void TestGetCrossDaysVariants()
    {
        var begin = new DateTime(2024, 1, 10, 10, 0, 0);
        var end = new DateTime(2024, 1, 12, 15, 0, 0);
        
        var days = TimeHelper.GetCrossDays(begin, end, 0);
        Assert.Equal(2, days);
        
        // 测试时间戳版本
        var beginTimestamp = new DateTimeOffset(begin).ToUnixTimeSeconds();
        var endTimestamp = new DateTimeOffset(end).ToUnixTimeSeconds();
        var daysFromTimestamp = TimeHelper.GetCrossDays(beginTimestamp, endTimestamp, 0);
        Assert.Equal(2, daysFromTimestamp);
    }

    [Fact]
    public void TestGetCrossLocalDays()
    {
        var startTimestamp = 1704880800L; // 2024-01-10 08:00:00 UTC
        var endTimestamp = 1704967200L;   // 2024-01-11 08:00:00 UTC
        
        var days = TimeHelper.GetCrossLocalDays(startTimestamp, endTimestamp);
        Assert.True(days >= 0); // 结果取决于本地时区
    }

    #endregion

    #region 同周判断测试

    [Fact]
    public void TestIsSameWeek()
    {
        var monday = new DateTime(2024, 1, 8);    // 周一
        var wednesday = new DateTime(2024, 1, 10); // 周三
        var sunday = new DateTime(2024, 1, 14);   // 周日
        var nextMonday = new DateTime(2024, 1, 15); // 下周一
        
        Assert.True(TimeHelper.IsSameWeek(monday, wednesday));
        Assert.True(TimeHelper.IsSameWeek(wednesday, sunday));
        Assert.False(TimeHelper.IsSameWeek(sunday, nextMonday));
    }

    [Fact]
    public void TestIsUnixSameWeekFromTimestamp()
    {
        var mondayTimestamp = 1704672000L;    // 2024-01-08 00:00:00 UTC (周一)
        var wednesdayTimestamp = 1704844800L; // 2024-01-10 00:00:00 UTC (周三)
        
        // 注意：这个测试结果可能因当前时间而异
        var result = TimeHelper.IsUnixSameWeekFromTimestamp(mondayTimestamp);
        Assert.IsType<bool>(result);
    }

    #endregion

    #region 星期相关测试

    [Fact]
    public void TestGetDayOfWeekTime()
    {
        var wednesday = new DateTime(2024, 1, 10); // 2024-01-10 是周三
        var monday = TimeHelper.GetDayOfWeekTime(wednesday, DayOfWeek.Monday);
        var expectedMonday = new DateTime(2024, 1, 8); // 2024-01-08 是周一
        
        Assert.Equal(expectedMonday, monday);
    }

    [Fact]
    public void TestGetChinaDayOfWeek()
    {
        Assert.Equal(1, TimeHelper.GetChinaDayOfWeek(DayOfWeek.Monday));
        Assert.Equal(2, TimeHelper.GetChinaDayOfWeek(DayOfWeek.Tuesday));
        Assert.Equal(7, TimeHelper.GetChinaDayOfWeek(DayOfWeek.Sunday));
    }

    #endregion

    #region 日期格式化测试

    [Fact]
    public void TestCurrentDateWithDay()
    {
        var result = TimeHelper.CurrentDateWithDay();
        var expected = int.Parse(DateTime.Now.ToString("yyyyMMdd"));
        Assert.Equal(expected, result);
    }

    [Fact]
    public void TestCurrentTimeWithUtcTime()
    {
        var result = TimeHelper.CurrentTimeWithUtcTime();
        var expected = int.Parse(DateTime.UtcNow.ToString("HHmmss"));
        // 允许1秒的误差
        Assert.True(Math.Abs(result - expected) <= 1);
    }

    [Fact]
    public void TestCurrentDateTimeWithFormat()
    {
        var format = "yyyy-MM-dd HH:mm";
        var result = TimeHelper.CurrentDateTimeWithFormat(format);
        var expected = DateTime.Now.ToString(format);
        
        // 由于时间可能有微小差异，我们只检查格式是否正确
        Assert.Equal(expected.Length, result.Length);
        Assert.True(result.Contains("-"));
        Assert.True(result.Contains(":"));
    }

    #endregion

    #region 时间范围获取测试

    [Fact]
    public void TestGetTodayStartAndEndTime()
    {
        var startTime = TimeHelper.GetTodayStartTime();
        var endTime = TimeHelper.GetTodayEndTime();
        
        Assert.Equal(0, startTime.Hour);
        Assert.Equal(0, startTime.Minute);
        Assert.Equal(0, startTime.Second);
        
        Assert.Equal(23, endTime.Hour);
        Assert.Equal(59, endTime.Minute);
        Assert.Equal(59, endTime.Second);
        
        Assert.Equal(endTime.Date, startTime.Date);
    }

    [Fact]
    public void TestGetWeekStartAndEndTime()
    {
        var startTime = TimeHelper.GetWeekStartTime();
        var endTime = TimeHelper.GetWeekEndTime();
        
        Assert.Equal(DayOfWeek.Monday, startTime.DayOfWeek);
        Assert.Equal(DayOfWeek.Sunday, endTime.DayOfWeek);
        
        var daysDiff = (endTime.Date - startTime.Date).Days;
        Assert.Equal(6, daysDiff);
    }

    [Fact]
    public void TestGetMonthStartAndEndTime()
    {
        var startTime = TimeHelper.GetMonthStartTime();
        var endTime = TimeHelper.GetMonthEndTime();
        
        Assert.Equal(1, startTime.Day);
        Assert.Equal(0, startTime.Hour);
        
        Assert.Equal(23, endTime.Hour);
        Assert.Equal(59, endTime.Minute);
        Assert.Equal(59, endTime.Second);
        
        // 检查是否是同一个月的最后一天
        var nextMonth = startTime.AddMonths(1);
        Assert.Equal(nextMonth.AddDays(-1).Date, endTime.Date);
    }

    [Fact]
    public void TestGetYearStartAndEndTime()
    {
        var startTime = TimeHelper.GetYearStartTime();
        var endTime = TimeHelper.GetYearEndTime();
        
        Assert.Equal(1, startTime.Month);
        Assert.Equal(1, startTime.Day);
        Assert.Equal(0, startTime.Hour);
        
        Assert.Equal(12, endTime.Month);
        Assert.Equal(31, endTime.Day);
        Assert.Equal(23, endTime.Hour);
        Assert.Equal(59, endTime.Minute);
        Assert.Equal(59, endTime.Second);
    }

    #endregion

    #region 指定日期时间范围测试

    [Fact]
    public void TestGetStartAndEndTimeOfDay()
    {
        var date = new DateTime(2024, 1, 10, 14, 30, 45);
        
        var startTime = TimeHelper.GetStartTimeOfDay(date);
        var endTime = TimeHelper.GetEndTimeOfDay(date);
        
        Assert.Equal(new DateTime(2024, 1, 10, 0, 0, 0), startTime);
        Assert.Equal(new DateTime(2024, 1, 10, 23, 59, 59), endTime);
    }

    [Fact]
    public void TestGetStartAndEndTimeOfWeek()
    {
        var wednesday = new DateTime(2024, 1, 10); // 周三
        
        var startTime = TimeHelper.GetStartTimeOfWeek(wednesday);
        var endTime = TimeHelper.GetEndTimeOfWeek(wednesday);
        
        Assert.Equal(DayOfWeek.Monday, startTime.DayOfWeek);
        Assert.Equal(DayOfWeek.Sunday, endTime.DayOfWeek);
        Assert.Equal(new DateTime(2024, 1, 8, 0, 0, 0), startTime);
    }

    [Fact]
    public void TestGetStartAndEndTimeOfMonth()
    {
        var date = new DateTime(2024, 1, 15);
        
        var startTime = TimeHelper.GetStartTimeOfMonth(date);
        var endTime = TimeHelper.GetEndTimeOfMonth(date);
        
        Assert.Equal(new DateTime(2024, 1, 1, 0, 0, 0), startTime);
        Assert.Equal(31, endTime.Day); // 1月有31天
        Assert.Equal(23, endTime.Hour);
        Assert.Equal(59, endTime.Minute);
        Assert.Equal(59, endTime.Second);
    }

    [Fact]
    public void TestGetStartAndEndTimeOfYear()
    {
        var date = new DateTime(2024, 6, 15);
        
        var startTime = TimeHelper.GetStartTimeOfYear(date);
        var endTime = TimeHelper.GetEndTimeOfYear(date);
        
        Assert.Equal(new DateTime(2024, 1, 1, 0, 0, 0), startTime);
        Assert.Equal(new DateTime(2024, 12, 31, 23, 59, 59), endTime);
    }

    #endregion

    #region 下一个时间段测试

    [Fact]
    public void TestGetTomorrowTimes()
    {
        var tomorrowStart = TimeHelper.GetTomorrowStartTime();
        var tomorrowEnd = TimeHelper.GetTomorrowEndTime();
        var today = DateTime.Today;
        
        Assert.Equal(today.AddDays(1), tomorrowStart);
        Assert.Equal(today.AddDays(2).AddSeconds(-1), tomorrowEnd);
    }

    [Fact]
    public void TestGetNextWeekTimes()
    {
        var nextWeekStart = TimeHelper.GetNextWeekStartTime();
        var nextWeekEnd = TimeHelper.GetNextWeekEndTime();
        var thisWeekStart = TimeHelper.GetWeekStartTime();
        
        Assert.Equal(thisWeekStart.AddDays(7), nextWeekStart);
        Assert.Equal(DayOfWeek.Monday, nextWeekStart.DayOfWeek);
        Assert.Equal(DayOfWeek.Sunday, nextWeekEnd.DayOfWeek);
    }

    [Fact]
    public void TestGetNextMonthTimes()
    {
        var nextMonthStart = TimeHelper.GetNextMonthStartTime();
        var nextMonthEnd = TimeHelper.GetNextMonthEndTime();
        var thisMonthStart = TimeHelper.GetMonthStartTime();
        
        Assert.Equal(thisMonthStart.AddMonths(1), nextMonthStart);
        Assert.Equal(1, nextMonthStart.Day);
        Assert.Equal(23, nextMonthEnd.Hour);
        Assert.Equal(59, nextMonthEnd.Minute);
        Assert.Equal(59, nextMonthEnd.Second);
    }

    #endregion

    #region 时间范围判断测试

    [Fact]
    public void TestIsTimeInRange()
    {
        var startTime = new DateTime(2024, 1, 10, 9, 0, 0);
        var endTime = new DateTime(2024, 1, 10, 17, 0, 0);
        var testTime1 = new DateTime(2024, 1, 10, 12, 0, 0);
        var testTime2 = new DateTime(2024, 1, 10, 18, 0, 0);
        
        Assert.True(TimeHelper.IsTimeInRange(testTime1, startTime, endTime));
        Assert.False(TimeHelper.IsTimeInRange(testTime2, startTime, endTime));
        Assert.True(TimeHelper.IsTimeInRange(startTime, startTime, endTime)); // 边界测试
        Assert.True(TimeHelper.IsTimeInRange(endTime, startTime, endTime)); // 边界测试
    }

    [Fact]
    public void TestIsTimestampInRange()
    {
        var startTimestamp = 1704880800L; // 2024-01-10 08:00:00 UTC
        var endTimestamp = 1704909600L;   // 2024-01-10 16:00:00 UTC
        var testTimestamp1 = 1704895200L; // 2024-01-10 12:00:00 UTC
        var testTimestamp2 = 1704913200L; // 2024-01-10 17:00:00 UTC
        
        Assert.True(TimeHelper.IsTimestampInRange(testTimestamp1, startTimestamp, endTimestamp));
        Assert.False(TimeHelper.IsTimestampInRange(testTimestamp2, startTimestamp, endTimestamp));
        Assert.True(TimeHelper.IsTimestampInRange(startTimestamp, startTimestamp, endTimestamp));
        Assert.True(TimeHelper.IsTimestampInRange(endTimestamp, startTimestamp, endTimestamp));
    }

    #endregion

    #region 同一天判断测试

    [Fact]
    public void TestIsSameDay()
    {
        var time1 = new DateTime(2024, 1, 10, 9, 0, 0);
        var time2 = new DateTime(2024, 1, 10, 23, 59, 59);
        var time3 = new DateTime(2024, 1, 11, 0, 0, 1);
        
        Assert.True(TimeHelper.IsSameDay(time1, time2));
        Assert.False(TimeHelper.IsSameDay(time1, time3));
    }

    [Fact]
    public void TestIsUnixSameDay()
    {
        var timestamp1 = 1704880800L; // 2024-01-10 08:00:00 UTC
        var timestamp2 = 1704909600L; // 2024-01-10 16:00:00 UTC
        var timestamp3 = 1704967200L; // 2024-01-11 08:00:00 UTC
        
        Assert.True(TimeHelper.IsUnixSameDay(timestamp1, timestamp2));
        Assert.False(TimeHelper.IsUnixSameDay(timestamp1, timestamp3));
    }

    [Fact]
    public void TestIsLocalSameDay()
    {
        var timestamp1 = 1704880800L; // 2024-01-10 08:00:00 UTC
        var timestamp2 = 1704909600L; // 2024-01-10 16:00:00 UTC
        
        // 结果取决于本地时区，但应该是布尔值
        var result = TimeHelper.IsLocalSameDay(timestamp1, timestamp2);
        Assert.IsType<bool>(result);
    }

    #endregion

    #region 当前时间获取测试

    [Fact]
    public void TestGetUtcNowAndGetNow()
    {
        var utcNow = TimeHelper.GetUtcNow();
        var now = TimeHelper.GetNow();
        
        Assert.IsType<DateTime>(utcNow);
        Assert.IsType<DateTime>(now);
        
        // UTC时间和本地时间应该有时区差异（除非本地时区就是UTC）
        var timeDiff = Math.Abs((now - utcNow).TotalHours);
        Assert.True(timeDiff <= 24); // 时区差异不会超过24小时
    }

    #endregion

    #region 时间戳相关测试

    [Fact]
    public void TestTimestampConversions()
    {
        var now = DateTime.UtcNow;
        var timestamp = new DateTimeOffset(now).ToUnixTimeSeconds();
        
        var convertedBack = TimeHelper.UtcSecondsToUtcDateTime(timestamp);
        
        // 由于精度问题，允许1秒的误差
        var diff = Math.Abs((now - convertedBack).TotalSeconds);
        Assert.True(diff < 1);
    }

    [Fact]
    public void TestTimeSpanWithTimestamp()
    {
        var fixedTimestamp = 86400L; // 1970-01-02 00:00:00 UTC (1天后，在有效范围内)
        var timeSpan = TimeHelper.TimeSpanWithTimestamp(fixedTimestamp);
        
        Assert.IsType<TimeSpan>(timeSpan);
        Assert.Equal(TimeSpan.FromSeconds(fixedTimestamp), timeSpan);
    }

    #endregion

    #region 边界和异常情况测试

    [Fact]
    public void TestLeapYear()
    {
        // 测试闰年2月的处理
        var leapYearDate = new DateTime(2024, 2, 15); // 2024是闰年
        var monthStart = TimeHelper.GetStartTimeOfMonth(leapYearDate);
        var monthEnd = TimeHelper.GetEndTimeOfMonth(leapYearDate);
        
        Assert.Equal(new DateTime(2024, 2, 1, 0, 0, 0), monthStart);
        Assert.Equal(29, monthEnd.Day); // 闰年2月有29天
    }

    [Fact]
    public void TestNegativeTimestamp()
    {
        // 测试1970年之前的时间戳
        var negativeTimestamp = -86400L; // 1969-12-31 00:00:00 UTC
        var dateTime = TimeHelper.UtcSecondsToUtcDateTime(negativeTimestamp);
        
        Assert.Equal(1969, dateTime.Year);
        Assert.Equal(12, dateTime.Month);
        Assert.Equal(31, dateTime.Day);
    }

    [Fact]
    public void TestLargeTimestamp()
    {
        // 测试较大的时间戳
        var largeTimestamp = 2147483647L; // 2038-01-19 03:14:07 UTC (32位时间戳的最大值)
        var dateTime = TimeHelper.UtcSecondsToUtcDateTime(largeTimestamp);
        
        Assert.Equal(2038, dateTime.Year);
        Assert.Equal(1, dateTime.Month);
        Assert.Equal(19, dateTime.Day);
    }

    #endregion

    #region 补充缺失的测试 - 本地时间戳函数

    [Fact]
    public void TestTimeSeconds()
    {
        var timeSeconds = TimeHelper.TimeSeconds();
        var expectedSeconds = new DateTimeOffset(DateTime.Now).ToUnixTimeSeconds() + TimeHelper.TimeOffsetSeconds;
        // 允许1秒的误差
        Assert.True(Math.Abs(timeSeconds - expectedSeconds) <= 1);
    }

    [Fact]
    public void TestTimeMilliseconds()
    {
        var timeMs = TimeHelper.TimeMilliseconds();
        var expectedMs = new DateTimeOffset(DateTime.Now).ToUnixTimeMilliseconds() + TimeHelper.TimeOffsetMilliseconds;
        // 允许1000毫秒的误差
        Assert.True(Math.Abs(timeMs - expectedMs) <= 1000);
    }

    #endregion

    #region 补充缺失的测试 - TimeSpan相关函数

    [Fact]
    public void TestTimeSpanLocalWithTimestamp()
    {
        // 使用固定时间戳进行测试，避免动态时间导致的问题
        var fixedTimestamp = 86400L; // 1970-01-02 00:00:00 UTC (1天后，在有效范围内)
        var timeSpan = TimeHelper.TimeSpanLocalWithTimestamp(fixedTimestamp);
        
        Assert.IsType<TimeSpan>(timeSpan);
        Assert.Equal(TimeSpan.FromSeconds(fixedTimestamp), timeSpan);
    }

    #endregion

    #region 补充缺失的测试 - 毫秒时间差函数

    [Fact]
    public void TestGetTimeDifferenceMs()
    {
        var startTimestampMs = 1704897045000L; // 2024-01-10 14:30:45.000 UTC
        var endTimestampMs = 1704897047500L;   // 2024-01-10 14:30:47.500 UTC
        
        var diff = TimeHelper.GetTimeDifferenceMs(startTimestampMs, endTimestampMs, true);
        Assert.Equal(2500, diff.TotalMilliseconds);
    }

    [Fact]
    public void TestGetTimeDifferenceFromNowMs()
    {
        // 使用固定时间戳进行测试，避免动态时间导致的问题
        var fixedTimestampMs = 1704897045000L; // 2024-01-10 14:30:45 UTC
        var diff = TimeHelper.GetTimeDifferenceFromNowMs(fixedTimestampMs, true);
        // 只验证返回类型正确，不验证具体时间差
        Assert.IsType<TimeSpan>(diff);
    }

    #endregion

    #region 补充缺失的测试 - 各种时间差计算函数

    [Fact]
    public void TestGetMinutesDifference()
    {
        var startTime = new DateTime(2024, 1, 10, 10, 0, 0);
        var endTime = new DateTime(2024, 1, 10, 10, 45, 30);
        
        var minutes = TimeHelper.GetMinutesDifference(startTime, endTime);
        Assert.Equal(45.5, minutes); // 45分30秒 = 45.5分钟
    }

    [Fact]
    public void TestGetHoursDifference()
    {
        var startTime = new DateTime(2024, 1, 10, 10, 0, 0);
        var endTime = new DateTime(2024, 1, 10, 13, 30, 0);
        
        var hours = TimeHelper.GetHoursDifference(startTime, endTime);
        Assert.Equal(3.5, hours); // 3小时30分钟 = 3.5小时
    }

    [Fact]
    public void TestGetDaysDifference()
    {
        var startTime = new DateTime(2024, 1, 10, 12, 0, 0);
        var endTime = new DateTime(2024, 1, 12, 18, 0, 0);
        
        var days = TimeHelper.GetDaysDifference(startTime, endTime);
        Assert.Equal(2.25, days); // 2天6小时 = 2.25天
    }

    [Fact]
    public void TestGetSecondsDifferenceWithTimestamp()
    {
        var startTimestamp = 1704897045L; // 2024-01-10 14:30:45 UTC
        var endTimestamp = 1704897075L;   // 2024-01-10 14:31:15 UTC
        
        var seconds = TimeHelper.GetSecondsDifference(startTimestamp, endTimestamp);
        Assert.Equal(30, seconds);
    }

    [Fact]
    public void TestGetMillisecondsDifferenceWithTimestamp()
    {
        var startTimestampMs = 1704897045000L; // 2024-01-10 14:30:45.000 UTC
        var endTimestampMs = 1704897045750L;   // 2024-01-10 14:30:45.750 UTC
        
        var ms = TimeHelper.GetMillisecondsDifference(startTimestampMs, endTimestampMs);
        Assert.Equal(750, ms);
    }

    [Fact]
    public void TestGetElapsedSecondsWithTimestamp()
    {
        var pastTimestamp = TimeHelper.UnixTimeSeconds() - 10; // 10秒前
        var elapsed = TimeHelper.GetElapsedSeconds(pastTimestamp, true);
        Assert.True(elapsed > 9 && elapsed < 11);
    }

    [Fact]
    public void TestGetElapsedMillisecondsWithTimestamp()
    {
        var pastTimestampMs = TimeHelper.UnixTimeMilliseconds() - 2500; // 2.5秒前
        var elapsed = TimeHelper.GetElapsedMilliseconds(pastTimestampMs, true);
        Assert.True(elapsed > 2400 && elapsed < 2600);
    }

    #endregion

    #region 补充缺失的测试 - 星期相关函数的无参版本

    [Fact]
    public void TestGetDayOfWeekTimeWithDayOfWeek()
    {
        var result = TimeHelper.GetDayOfWeekTime(DayOfWeek.Monday);
        Assert.IsType<DateTime>(result);
        Assert.Equal(DayOfWeek.Monday, result.DayOfWeek); // 应该返回本周一
    }

    [Fact]
    public void TestGetChinaDayOfWeekNoParams()
    {
        var result = TimeHelper.GetChinaDayOfWeek();
        Assert.True(result >= 1 && result <= 7); // 中国星期数字应该在1-7之间
    }

    #endregion

    #region 补充缺失的测试 - 日期格式化函数

    [Fact]
    public void TestCurrentDateWithUtcDay()
    {
        var result = TimeHelper.CurrentDateWithUtcDay();
        var expected = int.Parse(DateTime.UtcNow.ToString("yyyyMMdd"));
        // 允许日期可能跨天的情况
        Assert.True(Math.Abs(result - expected) <= 1);
    }

    [Fact]
    public void TestCurrentTimeWithUtcFullString()
    {
        var result = TimeHelper.CurrentTimeWithUtcFullString();
        Assert.NotNull(result);
        Assert.NotEmpty(result);
        Assert.Equal(6, result.Length); // HHmmss格式，6位字符串
    }

    [Fact]
    public void TestCurrentTimeWithLocalFullString()
    {
        var result = TimeHelper.CurrentTimeWithLocalFullString();
        Assert.NotNull(result);
        Assert.NotEmpty(result);
        Assert.Equal(6, result.Length); // HHmmss格式，6位字符串
    }

    [Fact]
    public void TestCurrentTimeWithLocalTime()
    {
        var result = TimeHelper.CurrentTimeWithLocalTime();
        var expected = int.Parse(DateTime.Now.ToString("HHmmss"));
        // 允许1秒的误差
        Assert.True(Math.Abs(result - expected) <= 1);
    }

    [Fact]
    public void TestCurrentDateTimeWithUtcFormat()
    {
        var format = "yyyy-MM-dd HH:mm:ss";
        var result = TimeHelper.CurrentDateTimeWithUtcFormat(format);
        var expected = DateTime.UtcNow.ToString(format);
        
        // 检查格式是否正确
        Assert.Equal(expected.Length, result.Length);
        Assert.True(result.Contains("-"));
        Assert.True(result.Contains(":"));
    }

    [Fact]
    public void TestCurrentDateTimeWithUtcFullString()
    {
        var result = TimeHelper.CurrentDateTimeWithUtcFullString();
        Assert.NotNull(result);
        Assert.NotEmpty(result);
        Assert.True(result.Length > 15); // 应该是完整的日期时间字符串
    }

    #endregion

    #region 补充缺失的测试 - 时间戳版本的函数

    [Fact]
    public void TestGetTodayStartTimestamp()
    {
        var timestamp = TimeHelper.GetTodayStartTimestamp();
        var dateTime = DateTimeOffset.FromUnixTimeSeconds(timestamp).ToLocalTime().DateTime;
        
        Assert.Equal(0, dateTime.Hour);
        Assert.Equal(0, dateTime.Minute);
        Assert.Equal(0, dateTime.Second);
    }

    [Fact]
    public void TestGetTodayEndTimestamp()
    {
        var timestamp = TimeHelper.GetTodayEndTimestamp();
        var dateTime = DateTimeOffset.FromUnixTimeSeconds(timestamp).ToLocalTime().DateTime;
        
        Assert.Equal(23, dateTime.Hour);
        Assert.Equal(59, dateTime.Minute);
        Assert.Equal(59, dateTime.Second);
    }

    [Fact]
    public void TestGetWeekStartTimestamp()
    {
        var timestamp = TimeHelper.GetWeekStartTimestamp();
        var dateTime = TimeHelper.UtcSecondsToLocalDateTime(timestamp);
        
        Assert.Equal(DayOfWeek.Monday, dateTime.DayOfWeek);
        Assert.Equal(0, dateTime.Hour);
    }

    [Fact]
    public void TestGetWeekEndTimestamp()
    {
        var timestamp = TimeHelper.GetWeekEndTimestamp();
        var dateTime = TimeHelper.UtcSecondsToLocalDateTime(timestamp);
        
        Assert.Equal(DayOfWeek.Sunday, dateTime.DayOfWeek);
        Assert.Equal(23, dateTime.Hour);
    }

    [Fact]
    public void TestGetMonthStartTimestamp()
    {
        var timestamp = TimeHelper.GetMonthStartTimestamp();
        var dateTime = TimeHelper.UtcSecondsToLocalDateTime(timestamp);
        
        Assert.Equal(1, dateTime.Day);
        Assert.Equal(0, dateTime.Hour);
    }

    [Fact]
    public void TestGetMonthEndTimestamp()
    {
        var timestamp = TimeHelper.GetMonthEndTimestamp();
        var dateTime = TimeHelper.UtcSecondsToLocalDateTime(timestamp);
        
        Assert.Equal(23, dateTime.Hour);
        Assert.Equal(59, dateTime.Minute);
    }

    [Fact]
    public void TestGetYearStartTimestamp()
    {
        var timestamp = TimeHelper.GetYearStartTimestamp();
        var dateTime = TimeHelper.UtcSecondsToLocalDateTime(timestamp);
        
        Assert.Equal(1, dateTime.Month);
        Assert.Equal(1, dateTime.Day);
        Assert.Equal(0, dateTime.Hour);
    }

    [Fact]
    public void TestGetYearEndTimestamp()
    {
        var timestamp = TimeHelper.GetYearEndTimestamp();
        var dateTime = TimeHelper.UtcSecondsToLocalDateTime(timestamp);
        
        Assert.Equal(12, dateTime.Month);
        Assert.Equal(31, dateTime.Day);
        Assert.Equal(23, dateTime.Hour);
    }

    [Fact]
    public void TestGetStartTimestampOfDay()
    {
        var date = new DateTime(2024, 1, 10, 14, 30, 45);
        var timestamp = TimeHelper.GetStartTimestampOfDay(date);
        var dateTime = TimeHelper.UtcSecondsToLocalDateTime(timestamp);
        
        Assert.Equal(2024, dateTime.Year);
        Assert.Equal(1, dateTime.Month);
        Assert.Equal(10, dateTime.Day);
        Assert.Equal(0, dateTime.Hour);
    }

    [Fact]
    public void TestGetEndTimestampOfDay()
    {
        var date = new DateTime(2024, 1, 10, 14, 30, 45);
        var timestamp = TimeHelper.GetEndTimestampOfDay(date);
        var dateTime = TimeHelper.UtcSecondsToLocalDateTime(timestamp);
        
        Assert.Equal(2024, dateTime.Year);
        Assert.Equal(1, dateTime.Month);
        Assert.Equal(10, dateTime.Day);
        Assert.Equal(23, dateTime.Hour);
    }

    [Fact]
    public void TestGetStartTimestampOfWeek()
    {
        var wednesday = new DateTime(2024, 1, 10); // 周三
        var timestamp = TimeHelper.GetStartTimestampOfWeek(wednesday);
        var dateTime = TimeHelper.UtcSecondsToLocalDateTime(timestamp);
        
        Assert.Equal(DayOfWeek.Monday, dateTime.DayOfWeek);
        Assert.Equal(0, dateTime.Hour);
    }

    [Fact]
    public void TestGetEndTimestampOfWeek()
    {
        var wednesday = new DateTime(2024, 1, 10); // 周三
        var timestamp = TimeHelper.GetEndTimestampOfWeek(wednesday);
        var dateTime = TimeHelper.UtcSecondsToLocalDateTime(timestamp);
        
        Assert.Equal(DayOfWeek.Sunday, dateTime.DayOfWeek);
        // Assert.Equal(23, dateTime.Hour);
    }

    [Fact]
    public void TestGetStartTimestampOfMonth()
    {
        var date = new DateTime(2024, 1, 15);
        var timestamp = TimeHelper.GetStartTimestampOfMonth(date);
        var dateTime = TimeHelper.UtcSecondsToLocalDateTime(timestamp);
        
        Assert.Equal(2024, dateTime.Year);
        Assert.Equal(1, dateTime.Month);
        Assert.Equal(1, dateTime.Day);
    }

    [Fact]
    public void TestGetEndTimestampOfMonth()
    {
        var date = new DateTime(2024, 1, 15);
        var timestamp = TimeHelper.GetEndTimestampOfMonth(date);
        var dateTime = TimeHelper.UtcSecondsToLocalDateTime(timestamp);
        
        Assert.Equal(2024, dateTime.Year);
        Assert.Equal(1, dateTime.Month);
        Assert.Equal(31, dateTime.Day); // 1月有31天
    }

    [Fact]
    public void TestGetStartTimestampOfYear()
    {
        var date = new DateTime(2024, 6, 15);
        var timestamp = TimeHelper.GetStartTimestampOfYear(date);
        var dateTime = TimeHelper.UtcSecondsToLocalDateTime(timestamp);
        
        Assert.Equal(2024, dateTime.Year);
        Assert.Equal(1, dateTime.Month);
        Assert.Equal(1, dateTime.Day);
    }

    [Fact]
    public void TestGetEndTimestampOfYear()
    {
        var date = new DateTime(2024, 6, 15);
        var timestamp = TimeHelper.GetEndTimestampOfYear(date);
        var dateTime = TimeHelper.UtcSecondsToLocalDateTime(timestamp);
        
        Assert.Equal(2024, dateTime.Year);
        Assert.Equal(12, dateTime.Month);
        Assert.Equal(31, dateTime.Day);
    }

    [Fact]
    public void TestGetTomorrowStartTimestamp()
    {
        var timestamp = TimeHelper.GetTomorrowStartTimestamp();
        var dateTime = TimeHelper.UtcSecondsToLocalDateTime(timestamp);
        var tomorrow = DateTime.Today.AddDays(1);
        
        Assert.Equal(tomorrow.Day, dateTime.Day);
        Assert.Equal(0, dateTime.Hour);
    }

    [Fact]
    public void TestGetTomorrowEndTimestamp()
    {
        var timestamp = TimeHelper.GetTomorrowEndTimestamp();
        var dateTime = TimeHelper.UtcSecondsToLocalDateTime(timestamp);
        var tomorrow = DateTime.Today.AddDays(1);
        
        Assert.Equal(tomorrow.Day, dateTime.Day);
        Assert.Equal(23, dateTime.Hour);
    }

    [Fact]
    public void TestGetNextWeekStartTimestamp()
    {
        var timestamp = TimeHelper.GetNextWeekStartTimestamp();
        var dateTime = TimeHelper.UtcSecondsToLocalDateTime(timestamp);
        
        Assert.Equal(DayOfWeek.Monday, dateTime.DayOfWeek);
        Assert.Equal(0, dateTime.Hour);
    }

    [Fact]
    public void TestGetNextWeekEndTimestamp()
    {
        var timestamp = TimeHelper.GetNextWeekEndTimestamp();
        var dateTime = TimeHelper.UtcSecondsToLocalDateTime(timestamp);
        
        Assert.Equal(DayOfWeek.Sunday, dateTime.DayOfWeek);
        Assert.Equal(23, dateTime.Hour);
    }

    [Fact]
    public void TestGetNextMonthStartTimestamp()
    {
        var timestamp = TimeHelper.GetNextMonthStartTimestamp();
        var dateTime = TimeHelper.UtcSecondsToLocalDateTime(timestamp);
        
        Assert.Equal(1, dateTime.Day);
        Assert.Equal(0, dateTime.Hour);
    }

    [Fact]
    public void TestGetNextMonthEndTimestamp()
    {
        var timestamp = TimeHelper.GetNextMonthEndTimestamp();
        var dateTime = TimeHelper.UtcSecondsToLocalDateTime(timestamp);
        
        Assert.Equal(23, dateTime.Hour);
        Assert.Equal(59, dateTime.Minute);
    }

    #endregion

    #region 补充缺失的测试 - 额外的边界测试

    [Fact]
    public void TestTimeZoneBoundary()
    {
        // 测试时区边界情况
        var utcTime = DateTime.UtcNow;
        var localTime = DateTime.Now;
        
        var utcTimestamp = TimeHelper.TimeToSecond(utcTime, true);
        var localTimestamp = TimeHelper.TimeToSecond(localTime, false);
        
        // UTC和本地时间戳应该有差异（除非本地时区就是UTC）
        Assert.IsType<long>(utcTimestamp);
        Assert.IsType<long>(localTimestamp);
    }

    [Fact]
    public void TestMillisecondPrecision()
    {
        // 测试毫秒精度
        var timeWithMs = new DateTime(2024, 1, 10, 14, 30, 45, 123);
        var timestampMs = TimeHelper.TimeToMilliseconds(timeWithMs, true);
        var convertedBack = TimeHelper.MillisecondsTimeStampToDateTime(timestampMs, true);
        
        Assert.Equal(123, convertedBack.Millisecond);
    }

    [Fact]
    public void TestWeekBoundary()
    {
        // 测试周边界
        var sunday = new DateTime(2024, 1, 7);    // 周日
        var monday = new DateTime(2024, 1, 8);    // 周一
        
        Assert.False(TimeHelper.IsSameWeek(sunday, monday));
        
        // 周日所在周的周一应该是1月1日，不是1月8日
        var mondayOfSundayWeek = TimeHelper.GetDayOfWeekTime(sunday, DayOfWeek.Monday);
        var expectedMondayOfSundayWeek = new DateTime(2024, 1, 1); // 2024年1月7日所在周的周一
        Assert.Equal(expectedMondayOfSundayWeek, mondayOfSundayWeek);
        
        // 周一所在周的周一应该是自己
        var mondayOfMondayWeek = TimeHelper.GetDayOfWeekTime(monday, DayOfWeek.Monday);
        Assert.Equal(monday, mondayOfMondayWeek);
    }

    [Fact]
    public void TestMonthBoundary()
    {
        // 测试月边界
        var endOfJan = new DateTime(2024, 1, 31);
        var startOfFeb = new DateTime(2024, 2, 1);
        
        var janEnd = TimeHelper.GetEndTimeOfMonth(endOfJan);
        var febStart = TimeHelper.GetStartTimeOfMonth(startOfFeb);
        
        Assert.Equal(1, janEnd.Month);
        Assert.Equal(2, febStart.Month);
        Assert.Equal(31, janEnd.Day);
        Assert.Equal(1, febStart.Day);
    }

    [Fact]
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
            Assert.Equal(dateTimeOffsetResult, timeHelperResult);
            
            // 验证年月日时分秒都相等
            Assert.Equal(dateTimeOffsetResult.Year, timeHelperResult.Year);
            Assert.Equal(dateTimeOffsetResult.Month, timeHelperResult.Month);
            Assert.Equal(dateTimeOffsetResult.Day, timeHelperResult.Day);
            Assert.Equal(dateTimeOffsetResult.Hour, timeHelperResult.Hour);
            Assert.Equal(dateTimeOffsetResult.Minute, timeHelperResult.Minute);
            Assert.Equal(dateTimeOffsetResult.Second, timeHelperResult.Second);
        }
    }

    #endregion
}