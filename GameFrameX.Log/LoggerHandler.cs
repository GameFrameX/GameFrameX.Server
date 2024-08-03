using Serilog;
using Serilog.Events;

namespace GameFrameX.Log;

/// <summary>
/// 日志对象
/// </summary>
public static class LoggerHandler
{
    /// <summary>
    /// 启动日志对象
    /// </summary>
    /// <param name="serverType">服务器类型</param>
    /// <param name="logSavePath">日志存储地址,默认为./logs</param>
    /// <param name="isConsole">是否是输出到控制台,默认true</param>
    /// <param name="rollingInterval">日志滚动间隔,默认为Hour</param>
    /// <param name="logEventLevel">日志输出级别,默认为Debug</param>
    /// <param name="fileSizeLimitBytes">日志文件大小限制,默认为10MB</param>
    /// <returns></returns>
    public static bool Start(string serverType = null, string logSavePath = null, bool isConsole = true, RollingInterval rollingInterval = RollingInterval.Hour, LogEventLevel logEventLevel = LogEventLevel.Debug, int fileSizeLimitBytes = 10 * 1024 * 1024)
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
                .WriteTo.File(logPath, rollingInterval: rollingInterval, rollOnFileSizeLimit: true, fileSizeLimitBytes: fileSizeLimitBytes);

            switch (logEventLevel)
            {
                case LogEventLevel.Verbose:
                {
                    logger.MinimumLevel.Verbose();
                }
                    break;
                case LogEventLevel.Debug:
                {
                    logger.MinimumLevel.Debug();
                }
                    break;
                case LogEventLevel.Information:
                {
                    logger.MinimumLevel.Information();
                }
                    break;
                case LogEventLevel.Warning:
                {
                    logger.MinimumLevel.Warning();
                }
                    break;
                case LogEventLevel.Error:
                {
                    logger.MinimumLevel.Error();
                }
                    break;
                case LogEventLevel.Fatal:
                {
                    logger.MinimumLevel.Fatal();
                }
                    break;
            }

            if (isConsole)
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