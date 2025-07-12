using System;
using GameFrameX.Utility;
using Xunit;

namespace GameFrameX.Tests.Utility;

/// <summary>
/// TimeHelper 类的单元测试
/// </summary>
public class TimeHelperTests
{
    /// <summary>
    /// 测试时间常量的正确性
    /// </summary>
    [Fact]
    public void TimeConstants_ShouldHaveCorrectValues()
    {
        // Assert
        Assert.Equal(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Local), TimeHelper.EpochLocal);
        Assert.Equal(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc), TimeHelper.EpochUtc);
        Assert.Equal(0L, TimeHelper.TimeOffsetSeconds);
        Assert.Equal(0L, TimeHelper.TimeOffsetMilliseconds);
    }

    /// <summary>
    /// 测试 SetTimeOffset 方法设置秒级偏移
    /// </summary>
    [Fact]
    public void SetTimeOffset_WithSeconds_ShouldSetCorrectOffset()
    {
        // Arrange
        const long offsetSeconds = 3600; // 1 hour
        
        // Act
        TimeHelper.SetTimeOffset(offsetSeconds, offsetSeconds * 1000);
        
        // Assert
        Assert.Equal(offsetSeconds, TimeHelper.TimeOffsetSeconds);
        Assert.Equal(offsetSeconds * 1000, TimeHelper.TimeOffsetMilliseconds);
        
        // Cleanup
        TimeHelper.ResetTimeOffset();
    }

    /// <summary>
    /// 测试 SetTimeOffset 方法设置毫秒级偏移
    /// </summary>
    [Fact]
    public void SetTimeOffset_WithMilliseconds_ShouldSetCorrectOffset()
    {
        // Arrange
        const long offsetMilliseconds = 3600000; // 1 hour in milliseconds
        
        // Act
        TimeHelper.SetTimeOffset(offsetMilliseconds / 1000, offsetMilliseconds);
        
        // Assert
        Assert.Equal(offsetMilliseconds / 1000, TimeHelper.TimeOffsetSeconds);
        Assert.Equal(offsetMilliseconds, TimeHelper.TimeOffsetMilliseconds);
        
        // Cleanup
        TimeHelper.ResetTimeOffset();
    }

    /// <summary>
    /// 测试 ResetTimeOffset 方法
    /// </summary>
    [Fact]
    public void ResetTimeOffset_ShouldResetToZero()
    {
        // Arrange
        TimeHelper.SetTimeOffset(3600, 0);
        
        // Act
        TimeHelper.ResetTimeOffset();
        
        // Assert
        Assert.Equal(0L, TimeHelper.TimeOffsetSeconds);
        Assert.Equal(0L, TimeHelper.TimeOffsetMilliseconds);
    }

    /// <summary>
    /// 测试 UnixTimeSeconds 方法
    /// </summary>
    [Fact]
    public void UnixTimeSeconds_ShouldReturnCorrectValue()
    {
        // Arrange
        var now = DateTime.UtcNow;
        var expectedUnixTime = (long)(now - TimeHelper.EpochUtc).TotalSeconds;
        
        // Act
        var actualUnixTime = TimeHelper.UnixTimeSeconds();
        
        // Assert
        // 允许1秒的误差，因为测试执行需要时间
        Assert.True(System.Math.Abs(actualUnixTime - expectedUnixTime) <= 1);
    }

    /// <summary>
    /// 测试 UnixTimeMilliseconds 方法
    /// </summary>
    [Fact]
    public void UnixTimeMilliseconds_ShouldReturnCorrectValue()
    {
        // Arrange
        var now = DateTime.UtcNow;
        var expectedUnixTime = (long)(now - TimeHelper.EpochUtc).TotalMilliseconds;
        
        // Act
        var actualUnixTime = TimeHelper.UnixTimeMilliseconds();
        
        // Assert
        // 允许1000毫秒的误差，因为测试执行需要时间
        Assert.True(System.Math.Abs(actualUnixTime - expectedUnixTime) <= 1000);
    }

    /// <summary>
    /// 测试 TimeSeconds 方法
    /// </summary>
    [Fact]
    public void TimeSeconds_ShouldReturnCorrectValue()
    {
        // Arrange
        var now = DateTime.Now;
        var expectedTime = (long)(now - TimeHelper.EpochLocal).TotalSeconds;
        
        // Act
        var actualTime = TimeHelper.TimeSeconds();
        
        // Assert
        // 允许1秒的误差
        Assert.True(System.Math.Abs(actualTime - expectedTime) <= 1);
    }

    /// <summary>
    /// 测试 TimeMilliseconds 方法
    /// </summary>
    [Fact]
    public void TimeMilliseconds_ShouldReturnCorrectValue()
    {
        // Arrange
        var now = DateTime.Now;
        var expectedTime = (long)(now - TimeHelper.EpochLocal).TotalMilliseconds;
        
        // Act
        var actualTime = TimeHelper.TimeMilliseconds();
        
        // Assert
        // 允许1000毫秒的误差
        Assert.True(System.Math.Abs(actualTime - expectedTime) <= 1000);
    }

    /// <summary>
    /// 测试 TimeToMilliseconds 方法转换DateTime
    /// </summary>
    [Fact]
    public void TimeToMilliseconds_WithDateTime_ShouldReturnCorrectValue()
    {
        // Arrange
        var testTime = new DateTime(2023, 1, 1, 12, 0, 0, DateTimeKind.Local);
        var expected = (long)(testTime - TimeHelper.EpochLocal).TotalMilliseconds;
        
        // Act
        var actual = TimeHelper.TimeToMilliseconds(testTime);
        
        // Assert
        Assert.Equal(expected, actual);
    }

    /// <summary>
    /// 测试 TimeToSecond 方法转换DateTime
    /// </summary>
    [Fact]
    public void TimeToSecond_WithDateTime_ShouldReturnCorrectValue()
    {
        // Arrange
        var testTime = new DateTime(2023, 1, 1, 12, 0, 0, DateTimeKind.Local);
        var expected = (long)(testTime - TimeHelper.EpochLocal).TotalSeconds;
        
        // Act
        var actual = TimeHelper.TimeToSecond(testTime);
        
        // Assert
        Assert.Equal(expected, actual);
    }

    /// <summary>
    /// 测试 TimestampToTicks 方法
    /// </summary>
    [Fact]
    public void TimestampToTicks_ShouldReturnCorrectValue()
    {
        // Arrange
        const long timestamp = 1672574400; // 2023-01-01 12:00:00 UTC in seconds
        var expectedDateTime = TimeHelper.EpochUtc.AddSeconds(timestamp);
        var expectedTicks = expectedDateTime.Ticks;
        
        // Act
        var actualTicks = TimeHelper.TimestampToTicks(timestamp);
        
        // Assert
        Assert.Equal(expectedTicks, actualTicks);
    }

    /// <summary>
    /// 测试 TimestampMillisToTicks 方法
    /// </summary>
    [Fact]
    public void TimestampMillisToTicks_ShouldReturnCorrectValue()
    {
        // Arrange
        const long timestampMillis = 1672574400000; // 2023-01-01 12:00:00 UTC in milliseconds
        var expectedDateTime = TimeHelper.EpochUtc.AddMilliseconds(timestampMillis);
        var expectedTicks = expectedDateTime.Ticks;
        
        // Act
        var actualTicks = TimeHelper.TimestampMillisToTicks(timestampMillis);
        
        // Assert
        Assert.Equal(expectedTicks, actualTicks);
    }

    /// <summary>
    /// 测试 TimeSpanWithTimestamp 方法
    /// </summary>
    [Fact]
    public void TimeSpanWithTimestamp_ShouldReturnCorrectTimeSpan()
    {
        // Arrange
        const long timestamp = 1672574400; // 2023-01-01 12:00:00 UTC in seconds
        var expectedDateTime = TimeHelper.EpochUtc.AddSeconds(timestamp);
        var expectedTimeSpan = expectedDateTime - TimeHelper.EpochUtc;
        
        // Act
        var actualTimeSpan = TimeHelper.TimeSpanWithTimestamp(timestamp);
        
        // Assert
        Assert.Equal(expectedTimeSpan, actualTimeSpan);
    }

    /// <summary>
    /// 测试 TimeSpanLocalWithTimestamp 方法
    /// </summary>
    [Fact]
    public void TimeSpanLocalWithTimestamp_ShouldReturnCorrectTimeSpan()
    {
        // Arrange
        const long timestamp = 1672574400; // seconds
        var expectedDateTime = TimeHelper.EpochLocal.AddSeconds(timestamp);
        var expectedTimeSpan = expectedDateTime - TimeHelper.EpochLocal;
        
        // Act
        var actualTimeSpan = TimeHelper.TimeSpanLocalWithTimestamp(timestamp);
        
        // Assert
        Assert.Equal(expectedTimeSpan, actualTimeSpan);
    }

    /// <summary>
    /// 测试 GetTimeDifference 方法比较两个DateTime
    /// </summary>
    [Fact]
    public void GetTimeDifference_WithDateTimes_ShouldReturnCorrectDifference()
    {
        // Arrange
        var startTime = new DateTime(2023, 1, 1, 12, 0, 0);
        var endTime = new DateTime(2023, 1, 1, 13, 30, 45);
        var expectedDifference = endTime - startTime;
        
        // Act
        var actualDifference = TimeHelper.GetTimeDifference(startTime, endTime);
        
        // Assert
        Assert.Equal(expectedDifference, actualDifference);
    }

    /// <summary>
    /// 测试 GetTimeDifference 方法比较两个秒级时间戳
    /// </summary>
    [Fact]
    public void GetTimeDifference_WithSecondTimestamps_ShouldReturnCorrectDifference()
    {
        // Arrange
        const long startTimestamp = 1672574400; // 2023-01-01 12:00:00
        const long endTimestamp = 1672579845;   // 2023-01-01 13:30:45
        var expectedDifference = TimeSpan.FromSeconds(endTimestamp - startTimestamp);
        
        // Act
        var actualDifference = TimeHelper.GetTimeDifference(startTimestamp, endTimestamp);
        
        // Assert
        Assert.Equal(expectedDifference, actualDifference);
    }

    /// <summary>
    /// 测试 GetTimeDifference 方法比较两个毫秒级时间戳
    /// </summary>
    [Fact]
    public void GetTimeDifference_WithMillisecondTimestamps_ShouldReturnCorrectDifference()
    {
        // Arrange
        const long startTimestamp = 1672574400000; // 2023-01-01 12:00:00
        const long endTimestamp = 1672579845000;   // 2023-01-01 13:30:45
        var expectedDifference = TimeSpan.FromMilliseconds(endTimestamp - startTimestamp);
        
        // Act
        var actualDifference = TimeHelper.GetTimeDifference(startTimestamp, endTimestamp, true);
        
        // Assert
        Assert.Equal(expectedDifference, actualDifference);
    }

    /// <summary>
    /// 测试 GetTimeDifferenceFromNow 方法
    /// </summary>
    [Fact]
    public void GetTimeDifferenceFromNow_WithDateTime_ShouldReturnCorrectDifference()
    {
        // Arrange
        var pastTime = DateTime.Now.AddHours(-1);
        
        // Act
        var difference = TimeHelper.GetTimeDifferenceFromNow(pastTime);
        
        // Assert
        Assert.True(difference.TotalHours >= 0.9 && difference.TotalHours <= 1.1); // 允许一些误差
    }

    /// <summary>
    /// 测试 GetSecondsDifference 方法
    /// </summary>
    [Fact]
    public void GetSecondsDifference_ShouldReturnCorrectValue()
    {
        // Arrange
        var startTime = new DateTime(2023, 1, 1, 12, 0, 0);
        var endTime = new DateTime(2023, 1, 1, 12, 1, 30); // 90 seconds later
        
        // Act
        var difference = TimeHelper.GetSecondsDifference(startTime, endTime);
        
        // Assert
        Assert.Equal(90, difference);
    }

    /// <summary>
    /// 测试 GetMillisecondsDifference 方法
    /// </summary>
    [Fact]
    public void GetMillisecondsDifference_ShouldReturnCorrectValue()
    {
        // Arrange
        var startTime = new DateTime(2023, 1, 1, 12, 0, 0, 0);
        var endTime = new DateTime(2023, 1, 1, 12, 0, 1, 500); // 1500 milliseconds later
        
        // Act
        var difference = TimeHelper.GetMillisecondsDifference(startTime, endTime);
        
        // Assert
        Assert.Equal(1500, difference);
    }

    /// <summary>
    /// 测试 GetMinutesDifference 方法
    /// </summary>
    [Fact]
    public void GetMinutesDifference_ShouldReturnCorrectValue()
    {
        // Arrange
        var startTime = new DateTime(2023, 1, 1, 12, 0, 0);
        var endTime = new DateTime(2023, 1, 1, 12, 30, 0); // 30 minutes later
        
        // Act
        var difference = TimeHelper.GetMinutesDifference(startTime, endTime);
        
        // Assert
        Assert.Equal(30, difference);
    }

    /// <summary>
    /// 测试 GetHoursDifference 方法
    /// </summary>
    [Fact]
    public void GetHoursDifference_ShouldReturnCorrectValue()
    {
        // Arrange
        var startTime = new DateTime(2023, 1, 1, 12, 0, 0);
        var endTime = new DateTime(2023, 1, 1, 15, 0, 0); // 3 hours later
        
        // Act
        var difference = TimeHelper.GetHoursDifference(startTime, endTime);
        
        // Assert
        Assert.Equal(3, difference);
    }

    /// <summary>
    /// 测试 GetDaysDifference 方法
    /// </summary>
    [Fact]
    public void GetDaysDifference_ShouldReturnCorrectValue()
    {
        // Arrange
        var startTime = new DateTime(2023, 1, 1, 12, 0, 0);
        var endTime = new DateTime(2023, 1, 4, 12, 0, 0); // 3 days later
        
        // Act
        var difference = TimeHelper.GetDaysDifference(startTime, endTime);
        
        // Assert
        Assert.Equal(3, difference);
    }

    /// <summary>
    /// 测试时间偏移对各种方法的影响
    /// </summary>
    [Fact]
    public void TimeOffset_ShouldAffectTimeMethods()
    {
        // Arrange
        const long offsetSeconds = 3600; // 1 hour
        var originalUnixTime = TimeHelper.UnixTimeSeconds();
        var originalTime = TimeHelper.TimeSeconds();
        
        // Act
        TimeHelper.SetTimeOffset(offsetSeconds, offsetSeconds * 1000);
        var offsetUnixTime = TimeHelper.UnixTimeSeconds();
        var offsetTime = TimeHelper.TimeSeconds();
        
        // Assert
        Assert.Equal((double)offsetSeconds, (double)(offsetUnixTime - originalUnixTime), precision: 1); // 允许1秒误差
        Assert.Equal((double)offsetSeconds, (double)(offsetTime - originalTime), precision: 1); // 允许1秒误差
        
        // Cleanup
        TimeHelper.ResetTimeOffset();
    }

    /// <summary>
    /// 测试边界情况：负时间戳
    /// </summary>
    [Fact]
    public void TimestampToTicks_WithNegativeTimestamp_ShouldReturnCorrectValue()
    {
        // Arrange
        const long negativeTimestamp = -3600; // 1 hour before epoch
        var expectedDateTime = TimeHelper.EpochUtc.AddSeconds(negativeTimestamp);
        var expectedTicks = expectedDateTime.Ticks;
        
        // Act
        var actualTicks = TimeHelper.TimestampToTicks(negativeTimestamp);
        
        // Assert
        Assert.Equal(expectedTicks, actualTicks);
    }

    /// <summary>
    /// 测试边界情况：零时间戳
    /// </summary>
    [Fact]
    public void TimestampToTicks_WithZeroTimestamp_ShouldReturnEpochTicks()
    {
        // Arrange
        const long zeroTimestamp = 0;
        var expectedTicks = TimeHelper.EpochUtc.Ticks;
        
        // Act
        var actualTicks = TimeHelper.TimestampToTicks(zeroTimestamp);
        
        // Assert
        Assert.Equal(expectedTicks, actualTicks);
    }
}