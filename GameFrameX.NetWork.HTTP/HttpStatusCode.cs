namespace GameFrameX.NetWork.HTTP;

/// <summary>
/// HTTP状态码
/// </summary>
public enum HttpStatusCode
{
    ///<summary>
    /// 成功
    /// </summary>
    Success = 0,

    ///<summary>
    /// 未定义的命令
    /// </summary>
    Undefined = 11,

    ///<summary>
    /// 非法
    /// </summary>
    Illegal = 12,

    ///<summary>
    /// 参数错误
    /// </summary>
    ParamErr = 13,

    ///<summary>
    /// 验证失败
    /// </summary>
    CheckFailed = 14,

    ///<summary>
    /// 操作失败
    /// </summary>
    ActionFailed = 15,

    ///<summary>
    /// 未找到的命令
    /// </summary>
    NotFound = 16,

    /// <summary>
    /// 服务器错误
    /// </summary>
    ServerError = 17,
}