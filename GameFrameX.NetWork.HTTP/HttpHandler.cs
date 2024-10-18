using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using GameFrameX.Extension;
using GameFrameX.Log;
using GameFrameX.Setting;

namespace GameFrameX.NetWork.HTTP
{
    /// <summary>
    /// HTTP处理器
    /// </summary>
    public static class HttpHandler
    {
        private const string ContentType = "application/json; charset=utf-8";

        /// <summary>
        /// 处理HTTP请求
        /// </summary>
        /// <param name="context"></param>
        /// <param name="baseHandler"></param>
        /// <param name="aopHandlerTypes"></param>
        public static async Task HandleRequest(HttpContext context, Func<string, BaseHttpHandler> baseHandler, List<IHttpAopHandler> aopHandlerTypes = null)
        {
            try
            {
                string ip = context.Connection.RemoteIpAddress?.ToString();
                string url = context.Request.PathBase + context.Request.Path;

                string command = context.Request.Path.ToString().Substring(HttpServer.ApiRootPath.Length);
                LogHelper.Info($"收到来自[{ip}]的HTTP请求. 请求url:[{url}],TraceIdentifier:[{context.TraceIdentifier}]");
                Dictionary<string, string> paramMap = new Dictionary<string, string>();

                foreach (var keyValuePair in context.Request.Query)
                {
                    paramMap.Add(keyValuePair.Key, keyValuePair.Value.ToString());
                }

                context.Response.Headers.ContentType = ContentType;
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
                    LogHelper.Info("isJson:" + isJson);
                    if (isJson)
                    {
                        JsonElement json = await context.Request.ReadFromJsonAsync<JsonElement>();
                        foreach (var keyValuePair in json.EnumerateObject())
                        {
                            if (!paramMap.TryAdd(keyValuePair.Name, keyValuePair.Value.GetString()))
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

                var str = new StringBuilder();
                str.Append($"TraceIdentifier:[{context.TraceIdentifier}]:请求参数:");
                foreach (var parameter in paramMap)
                {
                    if (parameter.Key.IsNullOrEmptyOrWhiteSpace())
                    {
                        continue;
                    }

                    str.Append('\'').Append(parameter.Key).Append("'='").Append(parameter.Value).Append("'  ");
                }

                LogHelper.Info(str.ToString());

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

                if (aopHandlerTypes is { Count: > 0 })
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


                var handler = baseHandler(command);
                if (handler == null)
                {
                    LogHelper.Warn($"http cmd handler 不存在：{command}");
                    await context.Response.WriteAsync(HttpResult.NotFound);
                    return;
                }

                //验证
                var checkCode = handler.CheckSign(paramMap);
                if (checkCode.IsNotNullOrEmptyOrWhiteSpace())
                {
                    await context.Response.WriteAsync(HttpResult.CheckFailed);
                    return;
                }

                var result = await Task.Run(() => { return handler.Action(ip, url, paramMap); });
                LogHelper.Warn($"执行http命令：{command}, TraceIdentifier:[{context.TraceIdentifier}]: Results: {result}");
                await context.Response.WriteAsync(result);
            }
            catch (Exception e)
            {
                LogHelper.Error("执行http异常. {0} {1}", e.Message, e.StackTrace);
                await context.Response.WriteAsync(HttpResult.Create(HttpStatusCode.ServerError, e.Message));
            }
        }
    }
}