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
//  Gitee Repository: https://gitee.com/GameFrameX
//  官方文档：https://gameframex.doc.alianblank.com/
//  Official Documentation: https://gameframex.doc.alianblank.com/
// ==========================================================================================

namespace GameFrameX.DiscoveryCenterManager.Strategy.Models;

/// <summary>
/// 负载均衡上下文
/// </summary>
public class LoadBalancingContext
{
    /// <summary>
    /// 服务器类型
    /// </summary>
    public string ServerType { get; set; } = string.Empty;

    /// <summary>
    /// 可用实例列表
    /// </summary>
    public List<long> AvailableInstances { get; set; } = new();

    /// <summary>
    /// 用于一致性哈希的键（可选）
    /// </summary>
    public string AffinityKey { get; set; } = string.Empty;

    /// <summary>
    /// 请求ID
    /// </summary>
    public string RequestId { get; set; } = System.Guid.NewGuid().ToString();

    /// <summary>
    /// 客户端ID
    /// </summary>
    public string ClientId { get; set; } = string.Empty;

    /// <summary>
    /// 用户ID
    /// </summary>
    public string UserId { get; set; } = string.Empty;

    /// <summary>
    /// 请求时间
    /// </summary>
    public System.DateTime RequestTime { get; set; } = System.DateTime.UtcNow;

    /// <summary>
    /// 请求优先级
    /// </summary>
    public RequestPriority Priority { get; set; } = RequestPriority.Normal;

    
    /// <summary>
    /// 元数据
    /// </summary>
    public Dictionary<string, object> Metadata { get; set; } = new();

    /// <summary>
    /// 超时时间（毫秒）
    /// </summary>
    public int TimeoutMs { get; set; } = 30000;

    /// <summary>
    /// 重试次数
    /// </summary>
    public int RetryCount { get; set; } = 0;

    /// <summary>
    /// 是否启用熔断器
    /// </summary>
    public bool EnableCircuitBreaker { get; set; } = false;

    /// <summary>
    /// 自定义属性
    /// </summary>
    public T GetProperty<T>(string key, T defaultValue = default(T))
    {
        if (Metadata.TryGetValue(key, out var value) && value is T typedValue)
        {
            return typedValue;
        }
        return defaultValue;
    }

    /// <summary>
    /// 设置自定义属性
    /// </summary>
    public void SetProperty<T>(string key, T value)
    {
        Metadata[key] = value;
    }
}