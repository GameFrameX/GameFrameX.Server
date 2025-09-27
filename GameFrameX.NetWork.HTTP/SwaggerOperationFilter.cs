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