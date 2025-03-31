using GameFrameX.Utility;

namespace GameFrameX.Tests;

public class UnitTestTime
{
    private DateTime dateTime, dateTime1;

    [SetUp]
    public void Setup()
    {
        dateTime = DateTime.Now;
        dateTime1 = DateTime.Now.AddHours(1);
    }

    [Test]
    public void test_current_time_millis()
    {
        var currentTimeMillis = TimeHelper.UnixTimeMilliseconds();
        var expectedTimeMillis = (long)(DateTime.Now - new DateTime(1970, 1, 1)).TotalMilliseconds;
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
}