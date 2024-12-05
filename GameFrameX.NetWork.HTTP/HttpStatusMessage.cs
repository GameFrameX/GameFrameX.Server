using GameFrameX.Setting;

namespace GameFrameX.NetWork.HTTP;

/// <summary>
/// HTTP状态消息
/// </summary>
public static class HttpStatusMessage
{
    ///<summary>
    /// 成功
    /// </summary>
    public const string Success = "ok";

    /// <summary>
    /// 未定义的命令
    /// </summary>
    public const string UndefinedCommand = "undefined command";

    /// <summary>
    /// 未找到的命令
    /// </summary>
    public const string NotFoundCommand = "not found command";

    /// <summary>
    /// 验证失败的命令
    /// </summary>
    public const string CheckFailedCommand = "check failed command";

    /// <summary>
    /// 服务器错误
    /// </summary>
    public const string ServerError = "server error";

    /// <summary>
    /// 参数错误
    /// </summary>
    public const string ParamErrorMessage = $"http命令未包含验证参数{GlobalConst.HttpSignKey} 和 {GlobalConst.HttpTimestampKey}";
}