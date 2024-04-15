using GameFrameX.NetWork.Messages;

namespace GameFrameX.NetWork;

public interface INetChannel
{
    /// <summary>
    /// 应用会话
    /// </summary>
    IGameSession AppSession { get; }

    /// <summary>
    /// 远程地址
    /// </summary>
    string RemoteAddress { get; }

    /// <summary>
    /// 写入消息
    /// </summary>
    /// <param name="messageObject"></param>
    void Write(IMessage messageObject);

    /// <summary>
    /// 异步写入消息
    /// </summary>
    /// <param name="msg"></param>
    /// <param name="uniId"></param>
    /// <param name="code"></param>
    /// <param name="desc"></param>
    /// <returns></returns>
    Task WriteAsync(IMessage msg, int uniId = 0, int code = 0, string desc = "");

    /// <summary>
    /// 获取用户数据对象
    /// </summary>
    /// <param name="key"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    T GetData<T>(string key);

    /// <summary>
    /// 移除用户数据
    /// </summary>
    /// <param name="key"></param>
    void RemoveData(string key);

    /// <summary>
    /// 设置用户数据
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    void SetData(string key, object value);

    /// <summary>
    /// 更新接收消息时间
    /// </summary>
    /// <param name="offsetTicks"></param>
    void UpdateReceiveMessageTime(long offsetTicks = 0);
}