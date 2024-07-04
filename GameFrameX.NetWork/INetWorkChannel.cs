using GameFrameX.NetWork.Messages;
using GameFrameX.SuperSocket.Server.Abstractions.Session;

namespace GameFrameX.NetWork;

/// <summary>
/// 网络通道
/// </summary>
public interface INetWorkChannel
{
    /// <summary>
    /// 应用会话
    /// </summary>
    IGameAppSession AppSession { get; }

    /// <summary>
    /// RPC 会话
    /// </summary>
    IRpcSession RpcSession { get; }

    /// <summary>
    /// 写入消息
    /// </summary>
    /// <param name="messageObject">消息对象</param>
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
    /// 关闭网络
    /// </summary>
    void Close();

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

    /// <summary>
    /// 是否已经关闭
    /// </summary>
    /// <returns></returns>
    bool IsClose();
}