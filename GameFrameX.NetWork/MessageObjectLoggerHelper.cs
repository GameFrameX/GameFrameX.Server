// GameFrameX 组织下的以及组织衍生的项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。使用本项目应遵守相关法律法规和许可证的要求。
// 
// 本项目主要遵循 MIT 许可证和 Apache 许可证（版本 2.0）进行分发和使用。许可证位于源代码树根目录中的 LICENSE 文件。
// 
// 不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目二次开发而产生的一切法律纠纷和责任，我们不承担任何责任！

using System.Text;
using GameFrameX.Foundation.Json;
using GameFrameX.Foundation.Logger;
using GameFrameX.NetWork.Abstractions;
using GameFrameX.NetWork.Messages;
using GameFrameX.Foundation.Extensions;

namespace GameFrameX.NetWork;

/// <summary>
/// 消息对象日志帮助类
/// </summary>
public static class MessageObjectLoggerHelper
{
    /// <summary>
    /// 格式化网络消息为可读的字符串格式
    /// </summary>
    /// <param name="messageId">消息ID</param>
    /// <param name="operationType">操作类型</param>
    /// <param name="uniqueId">唯一标识ID</param>
    /// <param name="messageObject">网络消息对象</param>
    /// <param name="actorId"></param>
    /// <returns>格式化后的消息字符串</returns>
    public static string FormatMessage(int messageId, MessageOperationType operationType, int uniqueId, INetworkMessage messageObject, long actorId)
    {
        try
        {
            if (LogOptions.Default.IsConsole)
            {
                // 使用StringBuilder构建格式化的控制台输出
                var stringBuilder = new StringBuilder();
                stringBuilder.Clear();
                stringBuilder.AppendLine();
                // 向下的箭头
                stringBuilder.AppendLine($"{'\u2193'.RepeatChar(140)}");
                // 消息的头部信息
                // 消息类型
                stringBuilder.Append($"---MessageType:[{messageObject.GetType().Name.CenterAlignedText(30)}]");
                // 消息ID
                stringBuilder.Append($"--MsgId:[{messageId.ToString().CenterAlignedText(11)}]({MessageIdUtility.GetMainId(messageId).ToString().CenterAlignedText(6)},{MessageIdUtility.GetSubId(messageId).ToString().CenterAlignedText(6)})");
                // 操作类型
                stringBuilder.Append($"--OpType:[{operationType.ToString().CenterAlignedText(20)}]");
                // 唯一ID
                stringBuilder.Append($"--UniqueId:[{uniqueId.ToString().CenterAlignedText(13)}]---");
                // 消息的内容 分割
                stringBuilder.AppendLine();
                // 消息的ActorId内容
                stringBuilder.AppendLine(actorId.ToString().CenterAlignedText(140));
                // 消息内容
                stringBuilder.AppendLine($"{messageObject.ToJsonString()}");
                // 向上的箭头
                stringBuilder.AppendLine($"{'\u2191'.RepeatChar(140)}");
                stringBuilder.AppendLine();
                // 从缓存中获取并释放StringBuilder的内容
                return stringBuilder.ToString();
            }

            // 非控制台输出模式下，将消息序列化为JSON格式
            var messageObjectLogObject = new MessageObjectLogObject(messageObject.GetType().Name, messageId, operationType, uniqueId, messageObject, actorId);
            var json = JsonHelper.Serialize(messageObjectLogObject);
            return json;
        }
        catch (Exception e)
        {
            LogHelper.Error(e);
        }

        return string.Empty;
    }
}