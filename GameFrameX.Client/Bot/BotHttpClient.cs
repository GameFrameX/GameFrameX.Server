using System.Net.Http.Headers;
using System.Text;
using GameFrameX.NetWork.Abstractions;
using GameFrameX.NetWork.Messages;
using GameFrameX.Utility.Log;

namespace GameFrameX.Bot;

public class BotHttpClient
{
    private readonly HttpClient m_HttpClient = new();
    private static readonly StringBuilder m_StringBuilder = new(256);

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
    public async Task<T> Post<T>(string url, MessageObject message) where T : MessageObject, IResponseMessage
    {
        var webBufferResult = await PostInner(url, message);
        if (webBufferResult.IsSuccessStatusCode)
        {
            var messageObjectHttp =
                SerializerHelper.Deserialize<MessageHttpObject>(await webBufferResult.Content.ReadAsByteArrayAsync());
            if (messageObjectHttp != null && messageObjectHttp.Id != default)
            {
                var messageType = MessageProtoHelper.GetMessageTypeById(messageObjectHttp.Id);
                if (messageType != typeof(T))
                {
                    LogHelper.Error(
                        $"Response message type is invalid. Expected '{typeof(T).FullName}', actual '{messageType.FullName}'.");
                    return default!;
                }

                return SerializerHelper.Deserialize<T>(messageObjectHttp.Body);
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
            Body = SerializerHelper.Serialize(message),
        };
        var sendData = SerializerHelper.Serialize(messageHttpObject);
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
        m_StringBuilder.Clear();
        m_StringBuilder.Append((url));
        if (queryString != null && queryString.Count > 0)
        {
            if (!EndsWithFast(url, "?"))
            {
                m_StringBuilder.Append("?");
            }

            foreach (var kv in queryString)
            {
                m_StringBuilder.AppendFormat("{0}={1}&", kv.Key, kv.Value);
            }

            url = m_StringBuilder.ToString(0, m_StringBuilder.Length - 1);
            m_StringBuilder.Clear();
        }

        return url;
    }

    /// <summary>
    /// 判断字符串是否以目标字符串结尾
    /// </summary>
    /// <param name="self"></param>
    /// <param name="target">目标字符串</param>
    /// <returns></returns>
    private bool EndsWithFast(string self, string target)
    {
        int ap = self.Length - 1;
        int bp = target.Length - 1;

        while (ap >= 0 && bp >= 0 && self[ap] == target[bp])
        {
            ap--;
            bp--;
        }

        return (bp < 0);
    }
}