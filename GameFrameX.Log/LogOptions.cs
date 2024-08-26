using Newtonsoft.Json;
using Serilog;
using Serilog.Events;

namespace GameFrameX.Log;

/// <summary>
/// 日志配置
/// </summary>
public sealed class LogOptions
{
    /// <summary>
    /// 默认配置对象
    /// </summary>
    public static readonly LogOptions Default = new LogOptions();

    /// <summary>
    /// 服务器类型
    /// </summary>
    public string ServerType = null;

    /// <summary>
    /// 日志存储路径,默认为./logs
    /// </summary>
    public string LogSavePath = "./logs/";

    /// <summary>
    /// 是否输出到控制台,默认true
    /// </summary>
    public bool IsConsole = true;

    /// <summary>
    /// 日志滚动间隔,默认为Hour
    /// </summary>
    public RollingInterval RollingInterval = RollingInterval.Hour;

    /// <summary>
    /// 日志输出级别,默认为Debug
    /// </summary>
    public LogEventLevel LogEventLevel = LogEventLevel.Debug;

    /// <summary>
    /// 是否限制单个文件大小
    /// </summary>
    public bool IsFileSizeLimit = true;

    /// <summary>
    /// 日志单个文件大小限制,默认为10MB
    /// 当IsFileSizeLimit为true时有效
    /// </summary>
    public int FileSizeLimitBytes = 10 * 1024 * 1024;

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
        return JsonConvert.SerializeObject(this, Formatting.Indented);
    }
}