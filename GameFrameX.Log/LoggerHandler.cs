using Serilog;

namespace GameFrameX.Log;

public static class LoggerHandler
{
    public static bool Start(string serverType = null)
    {
        try
        {
            var logPath = $"./logs/{serverType}_log_.log"; // 日志文件存储的路径
            Console.WriteLine("初始化日志系统配置 开始...");
            Serilog.Log.Logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .MinimumLevel.Debug()
#if DEBUG
                .WriteTo.Console()
#endif
                .WriteTo.File(logPath,
                    rollingInterval: RollingInterval.Hour,
                    rollOnFileSizeLimit: true,
                    fileSizeLimitBytes: 10 * 1024 * 1024)
                .CreateLogger();
            Console.WriteLine("初始化日志系统配置 结束...");
            return true;
        }
        catch (Exception e)
        {
            Serilog.Log.Error($"启动服务器失败,异常:{e}");
            return false;
        }
    }
}