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

namespace GameFrameX.NetWork.Abstractions;

/// <summary>
/// 网络消息包接口，包含消息数据、消息头及相关操作方法。
/// </summary>
/// <remarks>
/// Defines the interface for a network message package, including message data, header, and related operations.
/// </remarks>
public interface INetworkMessagePackage : IMessage
{
    /// <summary>
    /// 获取包含在消息包中的消息类型。
    /// </summary>
    /// <remarks>
    /// The type of the message contained in the package.
    /// </remarks>
    /// <value>消息类型 / Message type</value>
    Type MessageType { get; }

    /// <summary>
    /// 获取消息数据，通常为序列化后的二进制内容。
    /// </summary>
    /// <remarks>
    /// The message data, usually serialized as a byte array.
    /// </remarks>
    /// <value>消息数据 / Message data</value>
    byte[] MessageData { get; }

    /// <summary>
    /// 获取消息头对象，包含消息标识、长度、类型等元数据。
    /// </summary>
    /// <remarks>
    /// The message header object, containing metadata such as message ID, length, and type.
    /// </remarks>
    /// <value>消息头对象 / Message header object</value>
    INetworkMessageHeader Header { get; }

    /// <summary>
    /// 将消息数据反序列化为消息对象。
    /// </summary>
    /// <remarks>
    /// Deserializes the message data into a message object.
    /// </remarks>
    /// <returns>反序列化后的消息对象 / The deserialized message object</returns>
    INetworkMessage DeserializeMessageObject();

    /// <summary>
    /// 设置消息数据，通常用于接收或构建消息包时赋值。
    /// </summary>
    /// <remarks>
    /// Sets the message data, typically used when receiving or constructing a message package.
    /// </remarks>
    /// <param name="messageData">要设置的消息数据字节数组 / The byte array of message data to set</param>
    void SetMessageData(byte[] messageData);
}