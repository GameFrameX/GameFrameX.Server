using GameFrameX.SuperSocket.Server.Abstractions.Session;

namespace GameFrameX.NetWork.Abstractions;

/// <summary>
/// 网络通道
/// </summary>
public interface INetWorkChannel
{
    /// <summary>
    /// 发送字节长度 - 记录通过此通道发送的总字节数
    /// </summary>
    ulong SendBytesLength { get; }

    /// <summary>
    /// 发送数据包长度 - 记录通过此通道发送的数据包总数
    /// </summary>
    ulong SendPacketLength { get; }

    /// <summary>
    /// 接收字节长度 - 记录通过此通道接收的总字节数
    /// </summary>
    ulong ReceiveBytesLength { get; }

    /// <summary>
    /// 接收数据包长度 - 记录通过此通道接收的数据包总数
    /// </summary>
    ulong ReceivePacketLength { get; }

    /// <summary>
    /// 更新接收数据包字节长度
    /// </summary>
    /// <param name="bufferLength">接收数据包字节长度</param>
    void UpdateReceivePacketBytesLength(ulong bufferLength);

    /// <summary>
    /// 应用会话对象
    /// </summary>
    IGameAppSession GameAppSession { get; }

    /// <summary>
    /// RPC 会话
    /// </summary>
    IRpcSession RpcSession { get; }

    /// <summary>
    /// 异步写入消息到网络通道
    /// </summary>
    /// <param name="msg">要发送的网络消息对象,包含消息内容和相关元数据</param>
    /// <param name="errorCode">错误码,默认为0表示无错误。当发生错误时,可以通过此参数传递错误码</param>
    /// <returns>表示异步操作的Task对象。当消息成功写入时完成,如果发生错误则抛出异常</returns>
    /// <remarks>
    /// 此方法用于将消息异步发送到网络通道。
    /// 如果errorCode不为0,接收方可以根据错误码进行相应的错误处理。
    /// 调用此方法时需要确保网络通道处于打开状态。
    /// </remarks>
    Task WriteAsync(INetworkMessage msg, int errorCode = 0);

    /// <summary>
    /// 关闭网络
    /// </summary>
    void Close();

    /// <summary>
    /// 获取用户数据对象.
    /// 可能会发生转换失败的异常。
    /// 如果数据不存在则返回null
    /// </summary>
    /// <param name="key">数据Key</param>
    /// <typeparam name="T">将要获取的数据类型。</typeparam>
    /// <returns>用户数据对象</returns>
    T GetData<T>(string key);

    /// <summary>
    /// 清除自定义数据
    /// </summary>
    void ClearData();

    /// <summary>
    /// 移除用户数据
    /// </summary>
    /// <param name="key">数据Key</param>
    void RemoveData(string key);

    /// <summary>
    /// 设置用户数据
    /// </summary>
    /// <param name="key">数据Key</param>
    /// <param name="value">数据值</param>
    void SetData(string key, object value);

    /// <summary>
    /// 更新接收消息时间
    /// </summary>
    /// <param name="offsetTicks"></param>
    void UpdateReceiveMessageTime(long offsetTicks = 0);

    /// <summary>
    /// 获取最后一次消息的时间
    /// </summary>
    /// <param name="utcTime">UTC时间</param>
    /// <returns></returns>
    long GetLastMessageTimeSecond(in DateTime utcTime);

    /// <summary>
    /// 网络是否已经关闭
    /// </summary>
    /// <returns>是否已经关闭</returns>
    bool IsClosed();
}