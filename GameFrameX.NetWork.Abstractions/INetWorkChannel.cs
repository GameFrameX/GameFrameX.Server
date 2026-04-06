// ==========================================================================================
//   GameFrameX 组织及其衍生项目的版权、商标、专利及其他相关权利
//   GameFrameX organization and its derivative projects' copyrights, trademarks, patents, and related rights
//   均受中华人民共和国及相关国际法律法规保护。
//   are protected by the laws of the People's Republic of China and relevant international regulations.
//   使用本项目须严格遵守相应法律法规及开源许可证之规定。
//   Usage of this project must strictly comply with applicable laws, regulations, and open-source licenses.
//   本项目采用 MIT 许可证与 Apache License 2.0 双许可证分发，
//   This project is dual-licensed under the MIT License and Apache License 2.0,
//   完整许可证文本请参见源代码根目录下的 LICENSE 文件。
//   please refer to the LICENSE file in the root directory of the source code for the full license text.
//   禁止利用本项目实施任何危害国家安全、破坏社会秩序、
//   It is prohibited to use this project to engage in any activities that endanger national security, disrupt social order,
//   侵犯他人合法权益等法律法规所禁止的行为！
//   or infringe upon the legitimate rights and interests of others, as prohibited by laws and regulations!
//   因基于本项目二次开发所产生的一切法律纠纷与责任，
//   Any legal disputes and liabilities arising from secondary development based on this project
//   本项目组织与贡献者概不承担。
//   shall be borne solely by the developer; the project organization and contributors assume no responsibility.
//   GitHub 仓库：https://github.com/GameFrameX
//   GitHub Repository: https://github.com/GameFrameX
//   Gitee  仓库：https://gitee.com/GameFrameX
//   Gitee Repository:  https://gitee.com/GameFrameX
//   CNB  仓库：https://cnb.cool/GameFrameX
//   CNB Repository:  https://cnb.cool/GameFrameX
//   官方文档：https://gameframex.doc.alianblank.com/
//   Official Documentation: https://gameframex.doc.alianblank.com/
//  ==========================================================================================

using GameFrameX.SuperSocket.Server.Abstractions.Session;

namespace GameFrameX.NetWork.Abstractions;

/// <summary>
/// 网络通道接口。
/// </summary>
/// <remarks>
/// Network channel interface for managing network communication.
/// </remarks>
public interface INetWorkChannel
{
    /// <summary>
    /// 获取发送字节长度。记录通过此通道发送的总字节数。
    /// </summary>
    /// <remarks>
    /// Gets the total bytes sent through this channel.
    /// </remarks>
    /// <value>发送字节长度 / Total sent bytes length</value>
    ulong SendBytesLength { get; }

    /// <summary>
    /// 获取发送数据包长度。记录通过此通道发送的数据包总数。
    /// </summary>
    /// <remarks>
    /// Gets the total packets sent through this channel.
    /// </remarks>
    /// <value>发送数据包长度 / Total sent packets length</value>
    ulong SendPacketLength { get; }

    /// <summary>
    /// 获取接收字节长度。记录通过此通道接收的总字节数。
    /// </summary>
    /// <remarks>
    /// Gets the total bytes received through this channel.
    /// </remarks>
    /// <value>接收字节长度 / Total received bytes length</value>
    ulong ReceiveBytesLength { get; }

    /// <summary>
    /// 获取接收数据包长度。记录通过此通道接收的数据包总数。
    /// </summary>
    /// <remarks>
    /// Gets the total packets received through this channel.
    /// </remarks>
    /// <value>接收数据包长度 / Total received packets length</value>
    ulong ReceivePacketLength { get; }

    /// <summary>
    /// 更新接收数据包字节长度。
    /// </summary>
    /// <remarks>
    /// Updates the received packet bytes length.
    /// </remarks>
    /// <param name="bufferLength">接收数据包字节长度 / Received packet bytes length</param>
    void UpdateReceivePacketBytesLength(ulong bufferLength);

    /// <summary>
    /// 获取应用会话对象。
    /// </summary>
    /// <remarks>
    /// Gets the application session object.
    /// </remarks>
    /// <value>应用会话对象 / Application session object</value>
    IGameAppSession GameAppSession { get; }

    /// <summary>
    /// 获取 RPC 会话。
    /// </summary>
    /// <remarks>
    /// Gets the RPC session.
    /// </remarks>
    /// <value>RPC 会话 / RPC session</value>
    IRpcSession RpcSession { get; }

    /// <summary>
    /// 异步写入消息到网络通道。
    /// </summary>
    /// <remarks>
    /// Asynchronously writes a message to the network channel.
    /// This method is used to send messages asynchronously to the network channel.
    /// If errorCode is not 0, the receiver can handle errors based on the error code.
    /// Ensure the network channel is open when calling this method.
    /// </remarks>
    /// <param name="msg">要发送的网络消息对象，包含消息内容和相关元数据 / The network message object to send, containing message content and related metadata</param>
    /// <param name="errorCode">错误码，默认为0表示无错误。当发生错误时，可以通过此参数传递错误码 / Error code, default is 0 for no error. When an error occurs, this parameter can pass the error code</param>
    /// <returns>表示异步操作的Task对象。当消息成功写入时完成，如果发生错误则抛出异常 / A Task representing the asynchronous operation. Completes when the message is successfully written, throws an exception if an error occurs</returns>
    Task WriteAsync(INetworkMessage msg, int errorCode = 0);

    /// <summary>
    /// 关闭网络通道。
    /// </summary>
    /// <remarks>
    /// Closes the network channel.
    /// </remarks>
    void Close();

    /// <summary>
    /// 获取用户数据对象。
    /// </summary>
    /// <remarks>
    /// Gets the user data object. May throw an exception if conversion fails. Returns null if the data does not exist.
    /// </remarks>
    /// <param name="key">数据Key / Data key</param>
    /// <typeparam name="T">将要获取的数据类型 / The type of data to retrieve</typeparam>
    /// <returns>用户数据对象 / User data object</returns>
    T GetData<T>(string key);

    /// <summary>
    /// 清除自定义数据。
    /// </summary>
    /// <remarks>
    /// Clears all custom data.
    /// </remarks>
    void ClearData();

    /// <summary>
    /// 移除用户数据。
    /// </summary>
    /// <remarks>
    /// Removes user data by key.
    /// </remarks>
    /// <param name="key">数据Key / Data key</param>
    void RemoveData(string key);

    /// <summary>
    /// 设置用户数据。
    /// </summary>
    /// <remarks>
    /// Sets user data by key.
    /// </remarks>
    /// <param name="key">数据Key / Data key</param>
    /// <param name="value">数据值 / Data value</param>
    void SetData(string key, object value);

    /// <summary>
    /// 更新接收消息时间。
    /// </summary>
    /// <remarks>
    /// Updates the last received message time.
    /// </remarks>
    /// <param name="offsetTicks">偏移刻度数 / Offset ticks</param>
    void UpdateReceiveMessageTime(long offsetTicks = 0);

    /// <summary>
    /// 获取最后一次消息的时间。
    /// </summary>
    /// <remarks>
    /// Gets the time of the last message in seconds.
    /// </remarks>
    /// <param name="utcTime">UTC时间 / UTC time</param>
    /// <returns>最后一次消息的时间（秒） / The time of the last message in seconds</returns>
    long GetLastMessageTimeSecond(in DateTime utcTime);

    /// <summary>
    /// 检查网络是否已经关闭。
    /// </summary>
    /// <remarks>
    /// Checks whether the network channel is closed.
    /// </remarks>
    /// <returns>是否已经关闭 / <c>true</c> if closed; otherwise <c>false</c></returns>
    bool IsClosed();
}