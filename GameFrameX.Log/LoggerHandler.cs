using Serilog;

namespace GameFrameX.Log;

public static class LoggerHandler
{
    public static bool Start(string serverType = null, string logSavePath = null, bool isDebug = false, RollingInterval rollingInterval = RollingInterval.Hour)
    {
        try
        {
            // 日志文件存储的路径
            string logPath = "./logs/";
            string logFileName = $"{serverType ?? "Server"}_log.log";
            if (logSavePath != null)
            {
                logPath = Path.Combine(logSavePath, logFileName);
            }
            else
            {
                logPath = Path.Combine(logPath, logFileName);
            }

            Console.WriteLine("初始化日志系统配置 开始...");

            var logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .MinimumLevel.Debug()
                .WriteTo.File(logPath,
                    rollingInterval: rollingInterval,
                    rollOnFileSizeLimit: true,
                    fileSizeLimitBytes: 10 * 1024 * 1024);
            if (isDebug)
            {
                logger.WriteTo.Console();
            }

            Serilog.Log.Logger = logger.CreateLogger();
            Console.WriteLine($"初始化日志系统配置 结束...,日志文件路径：{logPath} 存档周期：{rollingInterval}");
            return true;
        }
        catch (Exception e)
        {
            Serilog.Log.Error($"启动服务器失败,异常:{e}");
            return false;
        }
    }
}