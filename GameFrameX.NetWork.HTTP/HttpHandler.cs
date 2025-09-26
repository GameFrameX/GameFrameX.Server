using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Reflection;
using GameFrameX.Foundation.Extensions;
using GameFrameX.Foundation.Http.Normalization;
using GameFrameX.Foundation.Json;
using GameFrameX.NetWork.Abstractions;
using GameFrameX.NetWork.Messages;
using GameFrameX.ProtoBuf.Net;
using GameFrameX.Foundation.Logger;
using GameFrameX.Utility.Setting;
using Microsoft.AspNetCore.Http;

namespace GameFrameX.NetWork.HTTP;

/// <summary>
/// HTTP处理器，用于处理HTTP请求
/// </summary>
public static class HttpHandler
{
    private const string JsonContentType = "application/json; charset=utf-8";
    private const string ProtoBufContentType = "application/x-protobuf";

    /// <summary>
    /// 处理HTTP请求
    /// </summary>
    /// <param name="context">HTTP上下文</param>
    /// <param name="baseHandler">基础HTTP处理器工厂方法</param>
    /// <param name="aopHandlerTypes">AOP处理器列表，可选</param>
    public static async Task HandleRequest(HttpContext context, Func<string, BaseHttpHandler> baseHandler, List<IHttpAopHandler> aopHandlerTypes = null)
    {
        var ip = context.Connection.RemoteIpAddress?.ToString();
        string url = context.Request.PathBase + context.Request.Path;
        var command = context.Request.Path.ToString().Substring(GlobalSettings.CurrentSetting.HttpUrl.Length);
        var logHeader = $"[HTTPServer] TraceIdentifier:[{context.TraceIdentifier}], 来源[{ip}], url:[{url}]";
        LogHelper.Debug($"{logHeader}，请求方式:[{context.Request.Method}]");

        try
        {
            var paramMap = new Dictionary<string, object>();

            // 从查询字符串中提取参数
            foreach (var keyValuePair in context.Request.Query)
            {
                paramMap.Add(keyValuePair.Key, keyValuePair.Value.ToString());
            }

            context.Response.Headers.ContentType = JsonContentType;
            MessageObject message = null;
            // 处理POST请求
            // if (string.Equals(context.Request.Method, HttpMethod.Post.Method, StringComparison.OrdinalIgnoreCase))
            // {
            var headContentType = context.Request.ContentType;
            if (headContentType.IsNullOrWhiteSpace())
            {
                await context.Response.WriteAsync("http header content type is null");
                return;
            }

            bool isProtoBuf = headContentType.Equals(ProtoBufContentType, StringComparison.OrdinalIgnoreCase);

            if (isProtoBuf)
            {
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    await context.Request.Body.CopyToAsync(memoryStream);
                    var buffer = memoryStream.ToArray();
                    var messageObjectHttp = ProtoBufSerializerHelper.Deserialize<MessageHttpObject>(buffer);
                    var messageType = MessageProtoHelper.GetMessageTypeById(messageObjectHttp.Id);
                    message = (MessageObject)ProtoBufSerializerHelper.Deserialize(messageObjectHttp.Body, messageType);
                    message.SetMessageId(messageObjectHttp.Id);
                    message.SetUniqueId(messageObjectHttp.UniqueId);
                }
            }
            else
            {
                var isJson = context.Request.HasJsonContentType();

                if (isJson)
                {
                    StreamReader streamReader = new StreamReader(context.Request.Body);
                    var jsonBody = await streamReader.ReadToEndAsync();
                    var jsonKv = JsonHelper.Deserialize<Dictionary<string, object>>(jsonBody);
                    foreach (var keyValuePair in jsonKv)
                    {
                        if (!paramMap.TryAdd(keyValuePair.Key, keyValuePair.Value))
                        {
                            // 参数Key发生重复
                            await context.Response.WriteAsync(HttpJsonResult.ErrorString(HttpStatusCode.ParamErr, "参数重复了:" + keyValuePair.Key));
                            return;
                        }
                    }
                }
                else
                {
                    await context.Response.WriteAsync(HttpJsonResult.ErrorString(HttpStatusCode.ParamErr, "不支持的Content Type: " + headContentType));
                    return;
                }
            }
            // }

            // 记录请求参数
            if (paramMap.Count > 0)
            {
                LogHelper.Debug("请求参数:" + JsonHelper.Serialize(paramMap));
            }

            // 检查指令是否有效
            if (command.IsNullOrEmptyOrWhiteSpace())
            {
                await context.Response.WriteAsync(HttpJsonResult.ErrorString(HttpStatusCode.Undefined, HttpStatusMessage.UndefinedCommand));
                return;
            }

            if (!GlobalSettings.IsAppRunning)
            {
                await context.Response.WriteAsync(HttpJsonResult.ErrorString(HttpStatusCode.ActionFailed, "服务器状态错误[正在起/关服]"));
                return;
            }

            #region AOP

            // 执行AOP处理器
            if (aopHandlerTypes is { Count: > 0, })
            {
                foreach (var httpAopHandler in aopHandlerTypes)
                {
                    if (!httpAopHandler.Run(context, ip, url, paramMap))
                    {
                        return;
                    }
                }
            }

            #endregion

            // 获取并执行对应的HTTP处理器
            var handler = baseHandler(command);
            if (handler == null)
            {
                LogHelper.Warning($"http cmd handler 不存在：{command}");
                await context.Response.WriteAsync(HttpJsonResult.NotFoundString());
                return;
            }

            // 验证签名
            var isChecked = handler.CheckSign(paramMap, out var error);
            if (isChecked == false)
            {
                await context.Response.WriteAsync(error);
                return;
            }


            // 执行处理器逻辑
            if (isProtoBuf)
            {
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();
                var result = await handler.Action(ip, url, paramMap, message);
                stopwatch.Stop();
                LogHelper.Debug($"{logHeader},执行时间：{stopwatch.ElapsedMilliseconds}ms, 结果: {result}");
                if (result.IsNotNull())
                {
                    try
                    {
                        ReadOnlyMemory<byte> body = ProtoBufSerializerHelper.Serialize(result);
                        MessageHttpObject messageHttpObject = new MessageHttpObject { Id = MessageProtoHelper.GetMessageIdByType(result), UniqueId = message.UniqueId, Body = body.ToArray(), };
                        var resultResponse = ProtoBufSerializerHelper.Serialize(messageHttpObject);
                        context.Response.ContentLength = resultResponse.Length;
                        await context.Response.BodyWriter.WriteAsync(resultResponse);
                    }
                    catch (Exception e)
                    {
                        LogHelper.Error("消息对象编码异常,请检查错误日志");
                        LogHelper.Error(e);
                    }
                }
            }
            else
            {
                var httpRequestAttr = handler.GetType().GetCustomAttribute<HttpMessageRequestAttribute>();
                if (httpRequestAttr != null)
                {
                    ArgumentNullException.ThrowIfNull(httpRequestAttr.MessageType, nameof(httpRequestAttr.MessageType));
                    var httpMessageRequestBase = (HttpMessageRequestBase)JsonHelper.Deserialize(JsonHelper.Serialize(paramMap), httpRequestAttr.MessageType);
                    var validationResults = new List<ValidationResult>();

                    var validationContext = new ValidationContext(httpMessageRequestBase, null, null);

                    var isValid = Validator.TryValidateObject(httpMessageRequestBase, validationContext, validationResults, true);
                    if (isValid)
                    {
                        Stopwatch stopwatch = new Stopwatch();
                        stopwatch.Start();
                        var result = await handler.Action(ip, url, httpMessageRequestBase);
                        stopwatch.Stop();
                        LogHelper.Debug($"{logHeader}, 执行时间：{stopwatch.ElapsedMilliseconds}ms, 结果: {result}");
                        await context.Response.WriteAsync(result);
                    }
                    else
                    {
                        if (validationResults.Count > 0)
                        {
                            await context.Response.WriteAsync(HttpJsonResult.ErrorString(400, validationResults[0].ErrorMessage));
                        }
                        else
                        {
                            await context.Response.WriteAsync(HttpJsonResult.ErrorString(400, "data verification failed"));
                        }
                    }
                }
                else
                {
                    Stopwatch stopwatch = new Stopwatch();
                    stopwatch.Start();
                    var result = await handler.Action(ip, url, paramMap);
                    stopwatch.Stop();
                    LogHelper.Debug($"{logHeader}, 执行时间：{stopwatch.ElapsedMilliseconds}ms, 结果: {result}");
                    await context.Response.WriteAsync(result);
                }
            }
        }
        catch (Exception e)
        {
            LogHelper.Error($"{logHeader}, 发生异常. {{0}} {{1}}", e.Message, e.StackTrace);
            await context.Response.WriteAsync(HttpJsonResult.FailString(e.Message));
        }
    }
}