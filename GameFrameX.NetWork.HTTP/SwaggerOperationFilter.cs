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


using System.Collections.Concurrent;
using System.ComponentModel;
using System.Reflection;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace GameFrameX.NetWork.HTTP;

/// <summary>
/// 自定义 Swagger 操作过滤器，用于处理动态路由和请求/响应文档。
/// </summary>
/// <remarks>
/// Custom Swagger operation filter for handling dynamic routing and request/response documentation.
/// Automatically generates OpenAPI documentation based on HTTP handler attributes.
/// </remarks>
public sealed class SwaggerOperationFilter : IOperationFilter
{
    /// <summary>
    /// HTTP 处理器列表。
    /// </summary>
    /// <remarks>
    /// List of HTTP handlers for generating documentation.
    /// </remarks>
    private readonly List<BaseHttpHandler> _handlers;

    /// <summary>
    /// 处理器类型到 <see cref="HttpMessageMappingAttribute"/> 的缓存。
    /// </summary>
    /// <remarks>
    /// Cache for mapping handler types to <see cref="HttpMessageMappingAttribute"/>.
    /// </remarks>
    private static readonly ConcurrentDictionary<Type, HttpMessageMappingAttribute> MappingAttributeCache = new();

    /// <summary>
    /// 处理器类型到 <see cref="HttpMessageRequestAttribute"/> 的缓存。
    /// </summary>
    /// <remarks>
    /// Cache for mapping handler types to <see cref="HttpMessageRequestAttribute"/>.
    /// </remarks>
    private static readonly ConcurrentDictionary<Type, HttpMessageRequestAttribute> RequestAttributeCache = new();

    /// <summary>
    /// 处理器类型到 <see cref="HttpMessageResponseAttribute"/> 的缓存。
    /// </summary>
    /// <remarks>
    /// Cache for mapping handler types to <see cref="HttpMessageResponseAttribute"/>.
    /// </remarks>
    private static readonly ConcurrentDictionary<Type, HttpMessageResponseAttribute> ResponseAttributeCache = new();

    /// <summary>
    /// 处理器类型到 <see cref="DescriptionAttribute"/> 的缓存。
    /// </summary>
    /// <remarks>
    /// Cache for mapping handler types to <see cref="DescriptionAttribute"/>.
    /// </remarks>
    private static readonly ConcurrentDictionary<Type, DescriptionAttribute> DescriptionAttributeCache = new();

    /// <summary>
    /// 初始化 <see cref="SwaggerOperationFilter"/> 的新实例。
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of <see cref="SwaggerOperationFilter"/>.
    /// </remarks>
    /// <param name="handlers">HTTP 处理器列表 / HTTP handler list</param>
    public SwaggerOperationFilter(List<BaseHttpHandler> handlers)
    {
        _handlers = handlers;
    }

    /// <summary>
    /// 应用过滤器配置，生成 OpenAPI 操作文档。
    /// </summary>
    /// <remarks>
    /// Applies the filter configuration to generate OpenAPI operation documentation.
    /// </remarks>
    /// <param name="operation">OpenAPI 操作对象 / OpenAPI operation object</param>
    /// <param name="context">操作过滤器上下文 / Operation filter context</param>
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var routeTemplate = context.ApiDescription.RelativePath;
        if (string.IsNullOrEmpty(routeTemplate))
        {
            return;
        }

        operation.Parameters.Clear();

        // 找到匹配的处理器（使用缓存的特性）
        var handler = _handlers.FirstOrDefault(h =>
        {
            var handlerType = h.GetType();
            var mappingAttr = MappingAttributeCache.GetOrAdd(handlerType, t => t.GetCustomAttribute<HttpMessageMappingAttribute>());
            return routeTemplate.EndsWith(mappingAttr?.StandardCmd ?? "", StringComparison.OrdinalIgnoreCase);
        });

        if (handler == null)
        {
            return;
        }

        var handlerType = handler.GetType();

        // 获取请求和响应的消息类型（使用缓存）
        var mappingAttr = MappingAttributeCache.GetOrAdd(handlerType, t => t.GetCustomAttribute<HttpMessageMappingAttribute>());
        var requestAttr = RequestAttributeCache.GetOrAdd(handlerType, t => t.GetCustomAttribute<HttpMessageRequestAttribute>());
        var responseAttr = ResponseAttributeCache.GetOrAdd(handlerType, t => t.GetCustomAttribute<HttpMessageResponseAttribute>());

        // 判断是否为 GET 请求
        var isGetRequest = mappingAttr?.HttpMethod == HttpMethodType.GET;

        // 设置请求参数（GET 使用 Query 参数，其他使用 RequestBody）
        if (isGetRequest && requestAttr?.MessageType != null)
        {
            // GET 请求：生成 Query 参数
            var properties = requestAttr.MessageType.GetProperties();
            foreach (var property in properties)
            {
                var paramSchema = GetSchemaForType(property.PropertyType);
                var propDescriptionAttr = property.GetCustomAttribute<DescriptionAttribute>();

                operation.Parameters.Add(new OpenApiParameter
                {
                    Name = property.Name,
                    In = ParameterLocation.Query,
                    Required = false, // 可根据 RequiredAttribute 判断
                    Description = propDescriptionAttr?.Description ?? property.Name,
                    Schema = paramSchema,
                });
            }
        }
        // 设置请求体（非 GET 请求）
        else if (requestAttr?.MessageType != null)
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
                    ["application/json"] = new()
                    {
                        Schema = requestSchema,
                    },
                },
            };
        }
        else if (!isGetRequest)
        {
            operation.RequestBody = new OpenApiRequestBody
            {
                Required = true,
                Description = "请求参数",
                Content = new Dictionary<string, OpenApiMediaType>
                {
                    ["application/json"] = new()
                    {
                        Schema = new OpenApiSchema { Type = "object", },
                    },
                },
            };
        }

        // 设置成功响应体
        var successResponseSchema = new OpenApiSchema
        {
            Type = "object",
            Properties = new Dictionary<string, OpenApiSchema>
            {
                ["code"] = new()
                {
                    Type = "integer",
                    Description = "响应状态码",
                    Example = new OpenApiInteger(0),
                },
                ["message"] = new()
                {
                    Type = "string",
                    Description = "响应消息",
                    Example = new OpenApiString("success"),
                },
            },
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
                    ["application/json"] = new()
                    {
                        Schema = successResponseSchema,
                    },
                },
            },
        };
        // 添加操作描述（使用缓存）
        var descriptionAttr = DescriptionAttributeCache.GetOrAdd(handlerType, t => t.GetCustomAttribute<DescriptionAttribute>());
        operation.Summary = descriptionAttr?.Description ?? handlerType.Name;
        operation.Description = GetTypeDescription(handlerType);
    }

    /// <summary>
    /// 获取类型的描述信息。
    /// </summary>
    /// <remarks>
    /// Gets the description information of the type.
    /// </remarks>
    /// <param name="type">要获取描述的类型 / Type to get description for</param>
    /// <returns>类型的描述信息，如果没有描述特性则返回类型名称 / Type description, returns type name if no description attribute is present</returns>
    private string GetTypeDescription(Type type)
    {
        var summaryAttr = DescriptionAttributeCache.GetOrAdd(type, t => t.GetCustomAttribute<DescriptionAttribute>());
        return summaryAttr?.Description ?? type.Name;
    }

    /// <summary>
    /// 根据类型获取对应的 OpenAPI Schema。
    /// </summary>
    /// <remarks>
    /// Gets the corresponding OpenAPI Schema based on the type.
    /// </remarks>
    /// <param name="type">属性类型 / Property type</param>
    /// <returns>对应的 OpenAPI Schema / Corresponding OpenAPI Schema</returns>
    private static OpenApiSchema GetSchemaForType(Type type)
    {
        if (type == typeof(string))
        {
            return new OpenApiSchema { Type = "string", };
        }

        if (type == typeof(int) || type == typeof(int?))
        {
            return new OpenApiSchema { Type = "integer", Format = "int32", };
        }

        if (type == typeof(long) || type == typeof(long?))
        {
            return new OpenApiSchema { Type = "integer", Format = "int64", };
        }

        if (type == typeof(float) || type == typeof(float?))
        {
            return new OpenApiSchema { Type = "number", Format = "float", };
        }

        if (type == typeof(double) || type == typeof(double?))
        {
            return new OpenApiSchema { Type = "number", Format = "double", };
        }

        if (type == typeof(decimal) || type == typeof(decimal?))
        {
            return new OpenApiSchema { Type = "number", Format = "decimal", };
        }

        if (type == typeof(bool) || type == typeof(bool?))
        {
            return new OpenApiSchema { Type = "boolean", };
        }

        if (type == typeof(DateTime) || type == typeof(DateTime?))
        {
            return new OpenApiSchema { Type = "string", Format = "date-time", };
        }

        if (type == typeof(Guid) || type == typeof(Guid?))
        {
            return new OpenApiSchema { Type = "string", Format = "uuid", };
        }

        if (type == typeof(byte[]))
        {
            return new OpenApiSchema { Type = "string", Format = "byte", };
        }

        if (type.IsArray || (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>)))
        {
            return new OpenApiSchema { Type = "array", };
        }

        return new OpenApiSchema { Type = "object", };
    }
}