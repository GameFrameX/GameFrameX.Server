namespace GameFrameX.NetWork.Abstractions;

/// <summary>
/// 网络消息
/// </summary>
public interface IMessage
{
    /// <summary>
    /// 获取格式化后的消息字符串
    /// </summary>
    /// <returns></returns>
    string ToFormatMessageString();
}