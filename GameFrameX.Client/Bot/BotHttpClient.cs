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

using System.Net.Http.Headers;
using System.Text;
using GameFrameX.NetWork.Abstractions;
using GameFrameX.NetWork.HTTP;
using GameFrameX.NetWork.Messages;
using GameFrameX.ProtoBuf.Net;
using GameFrameX.Foundation.Extensions;
using GameFrameX.Foundation.Logger;

namespace GameFrameX.Client.Bot;

public sealed class BotHttpClient
{
    private readonly HttpClient m_HttpClient = new();

    /// <summary>
    /// 发送Post请求。
    /// </summary>
    /// <param name="url">目标服务器的URL地址。</param>
    /// <param name="message">要发送的消息对象，必须继承自MessageObject。</param>
    /// <typeparam name="T">返回的数据类型，必须继承自MessageObject并且实现IResponseMessage接口。</typeparam>
    /// <returns>返回一个任务对象，该任务完成时将包含从服务器接收到的响应数据，数据类型为T。</returns>
    /// <remarks>
    /// 此方法用于向指定的URL发送POST请求，并接收响应。请求的消息体由参数message提供，而响应则会被解析为指定的泛型类型T。
    /// </remarks>
    public async Task<T> Post<T>(string url, MessageObject message) where T : MessageObject, IResponseMessage, new()
    {
        var webBufferResult = await PostInner(url, message);
        if (webBufferResult.IsSuccessStatusCode)
        {
            var messageObjectHttp = ProtoBufSerializerHelper.Deserialize<MessageHttpObject>(await webBufferResult.Content.ReadAsByteArrayAsync());
            if (messageObjectHttp != null && messageObjectHttp.Id != default)
            {
                var messageType = MessageProtoHelper.GetMessageTypeById(messageObjectHttp.Id);
                if (messageType != typeof(T))
                {
                    LogHelper.Error($"Response message type is invalid. Expected '{typeof(T).FullName}', actual '{messageType.FullName}'.");
                    return default!;
                }

                return ProtoBufSerializerHelper.Deserialize<T>(messageObjectHttp.Body);
            }
        }
        else
        {
            throw new Exception($"Failed to post data. Status code: {webBufferResult.StatusCode}");
        }

        return default!;
    }

    private Task<HttpResponseMessage> PostInner(string url, MessageObject message, object userData = null!)
    {
        url = UrlHandler(url);
        var id = MessageProtoHelper.GetMessageIdByType(message.GetType());
        MessageHttpObject messageHttpObject = new MessageHttpObject
        {
            Id = id,
            UniqueId = message.UniqueId,
            Body = ProtoBufSerializerHelper.Serialize(message),
        };
        var sendData = ProtoBufSerializerHelper.Serialize(messageHttpObject);
        var content = new ByteArrayContent(sendData);
        content.Headers.ContentType = new MediaTypeHeaderValue("application/x-protobuf");
        content.Headers.ContentLength = sendData.Length;
        return m_HttpClient.PostAsync(url, content);
    }

    /// <summary>
    /// URL 标准化
    /// </summary>
    /// <param name="url"></param>
    /// <param name="queryString"></param>
    /// <returns></returns>
    private string UrlHandler(string url, Dictionary<string, string> queryString = null!)
    {
        StringBuilder mStringBuilder = new(256);
        mStringBuilder.Clear();
        mStringBuilder.Append((url));
        if (queryString != null && queryString.Count > 0)
        {
            if (!url.EndsWithFast("?"))
            {
                mStringBuilder.Append("?");
            }

            foreach (var kv in queryString)
            {
                mStringBuilder.AppendFormat("{0}={1}&", kv.Key, kv.Value);
            }

            url = mStringBuilder.ToString(0, mStringBuilder.Length - 1);
            mStringBuilder.Clear();
        }

        return url;
    }
}