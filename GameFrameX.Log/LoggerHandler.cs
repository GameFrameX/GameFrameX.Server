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
    /// <param name="logOptions"></param>
    /// <returns></returns>
    public static void Start(LogOptions logOptions)
    {
        try
        {
            // 日志文件存储的路径
            var logPath = "./logs/";
            var logFileName = $"{logOptions.ServerType ?? "Server"}_log_.log";
            if (logOptions.LogSavePath != null)
            {
                logPath = Path.Combine(logOptions.LogSavePath, logFileName);
            }
            else
            {
                logPath = Path.Combine(logPath, logFileName);
            }

            Console.WriteLine("日志系统配置 开始");
            Console.WriteLine(logOptions);

            var logger = new LoggerConfiguration()
                         .Enrich.FromLogContext()
                         .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                         .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
                         .WriteTo.File(logPath, rollingInterval: logOptions.RollingInterval, rollOnFileSizeLimit: logOptions.IsFileSizeLimit, fileSizeLimitBytes: logOptions.FileSizeLimitBytes);

            switch (logOptions.LogEventLevel)
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

            if (logOptions.IsConsole)
            {
                logger.WriteTo.Console();
            }

            Serilog.Log.Logger = logger.CreateLogger();
            Console.WriteLine("日志系统配置 结束");
        }
        catch (Exception e)
        {
            Serilog.Log.Error($"启动服务器失败,异常:{e}");
            throw;
        }
    }
}