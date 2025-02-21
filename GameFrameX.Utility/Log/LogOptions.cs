using GameFrameX.Foundation.Json;
using Serilog;
using Serilog.Events;

namespace GameFrameX.Utility.Log;

/// <summary>
/// 日志配置类，用于配置日志的相关选项。
/// </summary>
public sealed class LogOptions
{
    /// <summary>
    /// 默认配置对象，提供一个默认的日志配置实例。
    /// </summary>
    public static readonly LogOptions Default = new();

    /// <summary>
    /// 服务器类型，用于标识日志来源的服务器类型。
    /// </summary>
    public string ServerType { get; set; } = null;

    /// <summary>
    /// 日志存储路径，默认为 ./logs。
    /// </summary>
    public string LogSavePath { get; set; } = "./logs/";

    /// <summary>
    /// 是否输出到控制台，默认为 true。
    /// </summary>
    public bool IsConsole { get; set; } = true;

    /// <summary>
    /// 日志滚动间隔，默认为每小时（Hour）。
    /// </summary>
    public RollingInterval RollingInterval { get; set; } = RollingInterval.Hour;

    /// <summary>
    /// 日志输出级别，默认为 Debug。
    /// </summary>
    public LogEventLevel LogEventLevel { get; set; } = LogEventLevel.Debug;

    /// <summary>
    /// 是否限制单个文件大小，默认为 true。
    /// </summary>
    public bool IsFileSizeLimit { get; set; } = true;

    /// <summary>
    /// 日志单个文件大小限制，默认为 10MB。
    /// 当 IsFileSizeLimit 为 true 时有效。
    /// </summary>
    public int FileSizeLimitBytes { get; set; } = 10 * 1024 * 1024;

    /// <summary>
    /// 日志文件保留数量限制 默认为 31 个文件,即 31 天的日志文件
    /// 当 设置值为 null 时不限制文件数量
    /// </summary>
    public int? RetainedFileCountLimit { get; set; } = 31;

    /// <summary>
    /// 返回日志配置对象的 JSON 字符串表示形式。
    /// </summary>
    /// <returns>JSON 字符串表示形式。</returns>
    public override string ToString()
    {
        return JsonHelper.SerializeFormat(this);
    }
}