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