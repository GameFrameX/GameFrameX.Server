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
        var timer = CreateTestTimer(200); // 200ms间隔，增加间隔时间
        var elapsedTimes = new List<DateTime>();
        var resetSignal = new ManualResetEventSlim(false);
        
        timer.Elapsed += (sender, e) => 
        {
            lock (elapsedTimes)
            {
                elapsedTimes.Add(DateTime.Now);
                if (elapsedTimes.Count >= 2)
                {
                    resetSignal.Set();
                }
            }
        };

        timer.Start();
        
        // 等待一段时间后重置
        await Task.Delay(100); // 增加等待时间
        timer.Reset();

        // 等待Timer触发几次，增加超时时间
        var waitResult = resetSignal.Wait(TimeSpan.FromSeconds(3));
        timer.Stop();

        // Assert
        Assert.True(waitResult, "Timer should have elapsed at least twice");
        lock (elapsedTimes)
        {
            Assert.True(elapsedTimes.Count >= 2, $"Timer should have elapsed at least twice, but only elapsed {elapsedTimes.Count} times");
        }
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