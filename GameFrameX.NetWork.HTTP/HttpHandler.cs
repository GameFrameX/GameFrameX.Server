using System.Text;
using System.Text.Json;
using GameFrameX.Extension;
using GameFrameX.Log;
using GameFrameX.Setting;
using Microsoft.AspNetCore.Http;

namespace GameFrameX.NetWork.HTTP;

/// <summary>
/// HTTP处理器，用于处理HTTP请求
/// </summary>
public static class HttpHandler
{
    private const string ContentType = "application/json; charset=utf-8";

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
        var command = context.Request.Path.ToString().Substring(HttpServer.ApiRootPath.Length);
        var logHeader = $"[HTTPServer] TraceIdentifier:[{context.TraceIdentifier}], 来源[{ip}], url:[{url}]";
        LogHelper.Info($"{logHeader}，请求方式:[{context.Request.Method}]");

        try
        {
            var paramMap = new Dictionary<string, object>();

            // 从查询字符串中提取参数
            foreach (var keyValuePair in context.Request.Query)
            {
                paramMap.Add(keyValuePair.Key, keyValuePair.Value.ToString());
            }

            context.Response.Headers.ContentType = ContentType;

            // 处理POST请求
            if (string.Equals(context.Request.Method, HttpMethod.Post.Method, StringComparison.OrdinalIgnoreCase))
            {
                var headContentType = context.Request.ContentType;
                if (headContentType.IsNullOrWhiteSpace())
                {
                    await context.Response.WriteAsync("http header content type is null");
                    return;
                }

                var isJson = context.Request.HasJsonContentType();
                var isForm = context.Request.HasFormContentType;

                if (isJson)
                {
                    var json = await context.Request.ReadFromJsonAsync<JsonElement>();
                    foreach (var keyValuePair in json.EnumerateObject())
                    {
                        if (!paramMap.TryAdd(keyValuePair.Name, keyValuePair.Value))
                        {
                            // 参数Key发生重复
                            await context.Response.WriteAsync(HttpResult.CreateErrorParam("参数重复了:" + keyValuePair.Name));
                            return;
                        }
                    }
                }
                else if (isForm)
                {
                    foreach (var keyValuePair in context.Request.Form)
                    {
                        if (!paramMap.TryAdd(keyValuePair.Key, keyValuePair.Value.ToString()))
                        {
                            // 参数Key发生重复
                            await context.Response.WriteAsync(HttpResult.CreateErrorParam("参数重复了:" + keyValuePair.Key));
                            return;
                        }
                    }
                }
                else
                {
                    await context.Response.WriteAsync(HttpResult.CreateErrorParam("不支持的Content Type: " + headContentType));
                    return;
                }
            }

            // 记录请求参数
            if (paramMap.Count > 0)
            {
                var str = new StringBuilder();
                str.Append("请求参数:");
                foreach (var parameter in paramMap)
                {
                    if (parameter.Key.IsNullOrEmptyOrWhiteSpace())
                    {
                        continue;
                    }

                    str.Append('\'').Append(parameter.Key).Append("'='").Append(parameter.Value).Append("'  ");
                }

                LogHelper.Info(str.ToString());
            }

            // 检查指令是否有效
            if (command.IsNullOrEmptyOrWhiteSpace())
            {
                await context.Response.WriteAsync(HttpResult.Undefined);
                return;
            }

            if (!GlobalSettings.IsAppRunning)
            {
                await context.Response.WriteAsync(HttpResult.CreateActionFailed("服务器状态错误[正在起/关服]"));
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
                LogHelper.Warn($"http cmd handler 不存在：{command}");
                await context.Response.WriteAsync(HttpResult.NotFound);
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
            var result = await Task.Run(() => { return handler.Action(ip, url, paramMap); });
            LogHelper.Warn($"{logHeader}, 结果: {result}");
            await context.Response.WriteAsync(result);
        }
        catch (Exception e)
        {
            LogHelper.Error($"{logHeader}, 发生异常. {{0}} {{1}}", e.Message, e.StackTrace);
            await context.Response.WriteAsync(HttpResult.Create(HttpStatusCode.ServerError, e.Message));
        }
    }
}