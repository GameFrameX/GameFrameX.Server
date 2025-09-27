// ==========================================================================================
//  GameFrameX 组织及其衍生项目的版权、商标、专利及其他相关权利
//  GameFrameX organization and its derivative projects' copyrights, trademarks, patents, and related rights
//  均受中华人民共和国及相关国际法律法规保护。
//  are protected by the laws of the People's Republic of China and relevant international regulations.
//  
//  使用本项目须严格遵守相应法律法规及开源许可证之规定。
//  Usage of this project must strictly comply with applicable laws, regulations, and open-source licenses.
//  
//  本项目采用 MIT 许可证与 Apache License 2.0 双许可证分发，
//  This project is dual-licensed under the MIT License and Apache License 2.0,
//  完整许可证文本请参见源代码根目录下的 LICENSE 文件。
//  please refer to the LICENSE file in the root directory of the source code for the full license text.
//  
//  禁止利用本项目实施任何危害国家安全、破坏社会秩序、
//  It is prohibited to use this project to engage in any activities that endanger national security, disrupt social order,
//  侵犯他人合法权益等法律法规所禁止的行为！
//  or infringe upon the legitimate rights and interests of others, as prohibited by laws and regulations!
//  因基于本项目二次开发所产生的一切法律纠纷与责任，
//  Any legal disputes and liabilities arising from secondary development based on this project
//  本项目组织与贡献者概不承担。
//  shall be borne solely by the developer; the project organization and contributors assume no responsibility.
//  
//  GitHub 仓库：https://github.com/GameFrameX
//  GitHub Repository: https://github.com/GameFrameX
//  Gitee  仓库：https://gitee.com/GameFrameX
//  Gitee Repository:  https://gitee.com/GameFrameX
//  官方文档：https://gameframex.doc.alianblank.com/
//  Official Documentation: https://gameframex.doc.alianblank.com/
// ==========================================================================================

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