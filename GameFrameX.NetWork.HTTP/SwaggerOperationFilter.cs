// GameFrameX 组织下的以及组织衍生的项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。使用本项目应遵守相关法律法规和许可证的要求。
// 
// 本项目主要遵循 MIT 许可证和 Apache 许可证（版本 2.0）进行分发和使用。许可证位于源代码树根目录中的 LICENSE 文件。
// 
// 不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目二次开发而产生的一切法律纠纷和责任，我们不承担任何责任！

using System.ComponentModel;
using System.Reflection;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace GameFrameX.NetWork.HTTP;

/// <summary>
/// 自定义 Swagger 操作过滤器,用于处理动态路由和请求/响应文档
/// </summary>
public sealed class SwaggerOperationFilter : IOperationFilter
{
    /// <summary>
    /// HTTP 处理器字典,key为命令ID,value为处理器类型
    /// </summary>
    private readonly List<BaseHttpHandler> _handlers;

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="handlers">HTTP处理器字典</param>
    public SwaggerOperationFilter(List<BaseHttpHandler> handlers)
    {
        _handlers = handlers;
    }

    /// <summary>
    /// 应用过滤器配置
    /// </summary>
    /// <param name="operation">OpenAPI操作对象</param>
    /// <param name="context">操作过滤器上下文</param>
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var routeTemplate = context.ApiDescription.RelativePath;
        if (string.IsNullOrEmpty(routeTemplate))
        {
            return;
        }

        operation.Parameters.Clear();

        // 找到匹配的处理器
        var handler = _handlers.FirstOrDefault(h => routeTemplate.EndsWith(h.GetType().GetCustomAttribute<HttpMessageMappingAttribute>()?.StandardCmd ?? "", StringComparison.OrdinalIgnoreCase));

        if (handler == null)
        {
            return;
        }

        var handlerType = handler.GetType();

        // 获取请求和响应的消息类型
        var requestAttr = handlerType.GetCustomAttribute<HttpMessageRequestAttribute>();
        var responseAttr = handlerType.GetCustomAttribute<HttpMessageResponseAttribute>();

        // 设置请求体
        if (requestAttr?.MessageType != null)
        {
            var requestSchema = context.SchemaGenerator.GenerateSchema(requestAttr.MessageType, context.SchemaRepository);

            // 修正请求Schema中的属性名称
            if (requestSchema.Properties != null)
            {
                var properties = requestAttr.MessageType.GetProperties();
                var correctedProperties = new Dictionary<string, OpenApiSchema>();

                foreach (var property in properties)
                {
                    var lowercaseKey = property.Name.ToLowerInvariant();
                    if (requestSchema.Properties.TryGetValue(lowercaseKey, out var schemaProperty))
                    {
                        correctedProperties[property.Name] = schemaProperty;
                    }
                }

                requestSchema.Properties.Clear();
                foreach (var prop in correctedProperties)
                {
                    requestSchema.Properties[prop.Key] = prop.Value;
                }
            }

            operation.RequestBody = new OpenApiRequestBody
            {
                Required = true,
                Description = "请求参数",
                Content = new Dictionary<string, OpenApiMediaType>
                {
                    ["application/json"] = new OpenApiMediaType
                    {
                        Schema = requestSchema
                    }
                }
            };
        }
        else
        {
            operation.RequestBody = new OpenApiRequestBody
            {
                Required = true,
                Description = "请求参数",
                Content = new Dictionary<string, OpenApiMediaType>
                {
                    ["application/json"] = new OpenApiMediaType
                    {
                        Schema = new OpenApiSchema() { Type = "object", },
                    }
                }
            };
        }

        // 设置成功响应体
        var successResponseSchema = new OpenApiSchema
        {
            Type = "object",
            Properties = new Dictionary<string, OpenApiSchema>
            {
                ["code"] = new OpenApiSchema
                {
                    Type = "integer",
                    Description = "响应状态码",
                    Example = new OpenApiInteger(0),
                },
                ["message"] = new OpenApiSchema
                {
                    Type = "string",
                    Description = "响应消息",
                    Example = new OpenApiString("success")
                }
            }
        };

        // 如果有响应类型，添加到 data 字段
        if (responseAttr?.MessageType != null)
        {
            successResponseSchema.Properties["data"] = context.SchemaGenerator.GenerateSchema(responseAttr.MessageType, context.SchemaRepository);
        }
        else
        {
            successResponseSchema.Properties["data"] = new OpenApiSchema { Type = "object", };
        }

        operation.Responses = new OpenApiResponses
        {
            ["200"] = new OpenApiResponse
            {
                Description = "成功响应",
                Content = new Dictionary<string, OpenApiMediaType>
                {
                    ["application/json"] = new OpenApiMediaType
                    {
                        Schema = successResponseSchema
                    },
                },
            },
        };
        // 添加操作描述
        var descriptionAttr = handlerType.GetCustomAttribute<DescriptionAttribute>();
        operation.Summary = descriptionAttr?.Description ?? handlerType.Name;
        operation.Description = GetTypeDescription(handlerType);
    }

    private string GetTypeDescription(Type type)
    {
        var summaryAttr = type.GetCustomAttribute<DescriptionAttribute>();
        return summaryAttr?.Description ?? type.Name;
    }
}