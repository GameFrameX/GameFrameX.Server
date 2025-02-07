using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace GameFrameX.NetWork.HTTP;

/// <summary>
/// 保持属性名称大小写的 Schema 过滤器
/// 用于在生成 Swagger/OpenAPI 文档时保持属性名称的原始大小写形式
/// </summary>
public sealed class PreservePropertyCasingSchemaFilter : ISchemaFilter
{
    /// <summary>
    /// 应用 Schema 过滤器，处理属性名称的大小写
    /// </summary>
    /// <param name="schema">要修改的 OpenAPI Schema</param>
    /// <param name="context">Schema 过滤器上下文，包含类型信息</param>
    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
        // 如果 schema 为空或没有属性，则直接返回
        if (schema?.Properties == null || schema.Properties.Count == 0)
        {
            return;
        }

        // 获取类型的所有属性
        var properties = context.Type.GetProperties();
        foreach (var property in properties)
        {
            // 检查是否存在小写形式的属性名
            if (schema.Properties.ContainsKey(property.Name.ToLowerInvariant()))
            {
                // 获取属性的 Schema
                var propertySchema = schema.Properties[property.Name.ToLowerInvariant()];
                // 移除小写形式的属性名
                schema.Properties.Remove(property.Name.ToLowerInvariant());
                // 添加原始大小写形式的属性名
                schema.Properties.Add(property.Name, propertySchema);
            }
        }
    }
}