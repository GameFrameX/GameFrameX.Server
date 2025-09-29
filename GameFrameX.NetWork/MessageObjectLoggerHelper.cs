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
    public static string FormatMessage(int messageId, byte operationType, int uniqueId, INetworkMessage messageObject, long actorId)
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

                if (actorId != default)
                {
                    // 消息的ActorId内容
                    stringBuilder.AppendLine(actorId.ToString().CenterAlignedText(140));
                }

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