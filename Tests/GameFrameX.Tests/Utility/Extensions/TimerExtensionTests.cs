using System.Collections.Concurrent;
using GameFrameX.Utility.Extensions;
using Xunit;
using Timer = System.Timers.Timer;

namespace GameFrameX.Tests.Utility.Extensions;

/// <summary>
/// TimerExtension 类的单元测试
/// </summary>
public class TimerExtensionTests : IDisposable
{
    private readonly List<Timer> _timers = new();

    /// <summary>
    /// 创建一个测试用的Timer
    /// </summary>
    private Timer CreateTestTimer(double interval = 100)
    {
        var timer = new Timer(interval);
        _timers.Add(timer);
        return timer;
    }

    /// <summary>
    /// 测试 Reset 方法对已启动的Timer的基本功能
    /// </summary>
    [Fact]
    public void Reset_WithRunningTimer_ShouldStopAndRestart()
    {
        // Arrange
        var timer = CreateTestTimer(50);
        var elapsedCount = 0;
        timer.Elapsed += (sender, e) => Interlocked.Increment(ref elapsedCount);
        
        timer.Start();
        Assert.True(timer.Enabled);

        // Act
        timer.Reset();

        // Assert
        Assert.True(timer.Enabled); // 应该重新启动
    }

    /// <summary>
    /// 测试 Reset 方法对已停止的Timer的功能
    /// </summary>
    [Fact]
    public void Reset_WithStoppedTimer_ShouldStart()
    {
        // Arrange
        var timer = CreateTestTimer(100);
        timer.Stop(); // 确保Timer是停止状态
        Assert.False(timer.Enabled);

        // Act
        timer.Reset();

        // Assert
        Assert.True(timer.Enabled); // 应该启动
    }

    /// <summary>
    /// 测试 Reset 方法对从未启动过的Timer的功能
    /// </summary>
    [Fact]
    public void Reset_WithNeverStartedTimer_ShouldStart()
    {
        // Arrange
        var timer = CreateTestTimer(100);
        Assert.False(timer.Enabled); // 新创建的Timer默认是停止的

        // Act
        timer.Reset();

        // Assert
        Assert.True(timer.Enabled); // 应该启动
    }

    /// <summary>
    /// 测试 Reset 方法在null Timer上抛出异常
    /// </summary>
    [Fact]
    public void Reset_WithNullTimer_ShouldThrowArgumentNullException()
    {
        // Arrange
        Timer? nullTimer = null;

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => nullTimer!.Reset());
    }

    /// <summary>
    /// 测试 Reset 方法重置Timer的计时
    /// </summary>
    [Fact]
    public async Task Reset_ShouldResetTimerInterval()
    {
        // Arrange
        var timer = CreateTestTimer(100); // 100ms间隔
        var elapsedTimes = new List<DateTime>();
        var resetSignal = new ManualResetEventSlim(false);
        
        timer.Elapsed += (sender, e) => 
        {
            elapsedTimes.Add(DateTime.Now);
            if (elapsedTimes.Count >= 2)
            {
                resetSignal.Set();
            }
        };

        timer.Start();
        
        // 等待一段时间后重置
        await Task.Delay(50);
        timer.Reset();

        // 等待Timer触发几次
        var waitResult = resetSignal.Wait(TimeSpan.FromSeconds(1));
        timer.Stop();

        // Assert
        Assert.True(waitResult, "Timer should have elapsed at least twice");
        Assert.True(elapsedTimes.Count >= 2, "Timer should have elapsed at least twice");
    }

    /// <summary>
    /// 测试 Reset 方法保持Timer的配置
    /// </summary>
    [Fact]
    public void Reset_ShouldPreserveTimerConfiguration()
    {
        // Arrange
        var timer = CreateTestTimer(200);
        timer.AutoReset = false; // 设置为不自动重置
        var originalInterval = timer.Interval;
        var originalAutoReset = timer.AutoReset;

        // Act
        timer.Reset();

        // Assert
        Assert.Equal(originalInterval, timer.Interval);
        Assert.Equal(originalAutoReset, timer.AutoReset);
        Assert.True(timer.Enabled);
    }

    /// <summary>
    /// 测试多次连续调用 Reset 方法
    /// </summary>
    [Fact]
    public void Reset_CalledMultipleTimes_ShouldWorkCorrectly()
    {
        // Arrange
        var timer = CreateTestTimer(100);

        // Act & Assert
        for (int i = 0; i < 5; i++)
        {
            timer.Reset();
            Assert.True(timer.Enabled, $"Timer should be enabled after reset #{i + 1}");
        }
    }

    /// <summary>
    /// 测试 Reset 方法在Timer有事件处理器时的行为
    /// </summary>
    [Fact]
    public async Task Reset_WithEventHandlers_ShouldMaintainEventHandlers()
    {
        // Arrange
        var timer = CreateTestTimer(50);
        var elapsedCount = 0;
        var eventFired = new ManualResetEventSlim(false);
        
        timer.Elapsed += (sender, e) => 
        {
            Interlocked.Increment(ref elapsedCount);
            eventFired.Set();
        };

        // Act
        timer.Reset();

        // 等待事件触发
        var eventTriggered = eventFired.Wait(TimeSpan.FromSeconds(1));
        timer.Stop();

        // Assert
        Assert.True(eventTriggered, "Timer elapsed event should have been triggered");
        Assert.True(elapsedCount > 0, "Event handler should have been called");
    }

    /// <summary>
    /// 测试 Reset 方法在不同间隔设置下的行为
    /// </summary>
    [Theory]
    [InlineData(10)]
    [InlineData(50)]
    [InlineData(100)]
    [InlineData(500)]
    public void Reset_WithDifferentIntervals_ShouldWorkCorrectly(double interval)
    {
        // Arrange
        var timer = CreateTestTimer(interval);

        // Act
        timer.Reset();

        // Assert
        Assert.True(timer.Enabled);
        Assert.Equal(interval, timer.Interval);
    }

    /// <summary>
    /// 测试 Reset 方法在Timer已释放后的行为
    /// </summary>
    [Fact]
    public void Reset_WithDisposedTimer_ShouldThrowObjectDisposedException()
    {
        // Arrange
        var timer = CreateTestTimer(100);
        timer.Dispose();

        // Act & Assert
        Assert.Throws<ObjectDisposedException>(() => timer.Reset());
    }

    /// <summary>
    /// 测试 Reset 方法的线程安全性
    /// </summary>
    [Fact]
    public async Task Reset_ConcurrentCalls_ShouldBeSafe()
    {
        // Arrange
        var timer = CreateTestTimer(100);
        var tasks = new List<Task>();
        var exceptions = new ConcurrentBag<Exception>();

        // Act
        for (int i = 0; i < 10; i++)
        {
            tasks.Add(Task.Run(() =>
            {
                try
                {
                    timer.Reset();
                }
                catch (Exception ex)
                {
                    exceptions.Add(ex);
                }
            }));
        }

        await Task.WhenAll(tasks);

        // Assert
        Assert.Empty(exceptions); // 不应该有异常
        Assert.True(timer.Enabled); // Timer应该是启用状态
    }

    /// <summary>
    /// 清理资源
    /// </summary>
    public void Dispose()
    {
        foreach (var timer in _timers)
        {
            try
            {
                timer.Stop();
                timer.Dispose();
            }
            catch
            {
                // 忽略清理时的异常
            }
        }
        _timers.Clear();
    }
}