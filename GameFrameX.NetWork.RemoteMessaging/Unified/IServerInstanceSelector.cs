// ==========================================================================================
//   GameFrameX 组织及其衍生项目的版权、商标、专利及其他相关权利
//   均受中华人民共和国及相关国际法律法规保护。
//   使用本项目须严格遵守相应法律法规及开源许可证之规定。
//   本项目采用 MIT 许可证与 Apache License 2.0 双许可证分发，
//   完整许可证文本请参见源代码根目录下的 LICENSE 文件。
//   禁止利用本项目实施任何危害国家安全、破坏社会秩序、
//   侵犯他人合法权益等法律法规所禁止的行为！
//   因基于本项目二次开发所产生的一切法律纠纷与责任，
//   本项目组织与贡献者概不承担。
//   GitHub 仓库：https://github.com/GameFrameX
//   Gitee  仓库：https://gitee.com/GameFrameX
//   CNB  仓库：https://cnb.cool/GameFrameX
//   官方文档：https://gameframex.doc.alianblank.com/
//  ==========================================================================================

namespace GameFrameX.NetWork.RemoteMessaging.Unified;

/// <summary>
/// 服务实例选择结果。
/// </summary>
/// <remarks>
/// Service instance selection result.
/// </remarks>
public sealed class InstanceSelection
{
    /// <summary>
    /// 选中的实例标识
    /// </summary>
    /// <remarks>
    /// Selected instance identifier.
    /// </remarks>
    public string InstanceId { get; init; }

    /// <summary>
    /// 对应的服务名
    /// </summary>
    /// <remarks>
    /// Corresponding service name.
    /// </remarks>
    public string ServiceName { get; init; }

    /// <summary>
    /// 是否命中
    /// </summary>
    /// <remarks>
    /// Whether an instance was found.
    /// </remarks>
    public bool HasInstance
    {
        get { return !string.IsNullOrEmpty(InstanceId); }
    }

    /// <summary>
    /// 创建选中结果
    /// </summary>
    /// <remarks>
    /// Creates a selection result.
    /// </remarks>
    /// <param name="serviceName">服务名 / Service name</param>
    /// <param name="instanceId">实例ID / Instance ID</param>
    /// <returns>实例选择结果 / Instance selection result</returns>
    public static InstanceSelection Selected(string serviceName, string instanceId)
    {
        return new InstanceSelection
        {
            ServiceName = serviceName,
            InstanceId = instanceId,
        };
    }

    /// <summary>
    /// 无可用实例
    /// </summary>
    /// <remarks>
    /// No available instance.
    /// </remarks>
    /// <param name="serviceName">服务名 / Service name</param>
    /// <returns>空选择结果 / Empty selection result</returns>
    public static InstanceSelection None(string serviceName)
    {
        return new InstanceSelection
        {
            ServiceName = serviceName,
        };
    }
}

/// <summary>
/// 服务实例选择器接口。负责在同名多实例下进行一致性选路。
/// </summary>
/// <remarks>
/// Server instance selector interface. Responsible for consistent routing among multiple instances with the same name.
/// </remarks>
public interface IServerInstanceSelector
{
    /// <summary>
    /// 选择一个目标实例。有 routeKey 时一致性哈希，无 routeKey 时轮询或随机。
    /// </summary>
    /// <remarks>
    /// Selects a target instance. Uses consistent hash when routeKey is provided, round-robin or random otherwise.
    /// </remarks>
    /// <param name="serviceName">服务名 / Service name</param>
    /// <param name="routeKey">路由键（可选）/ Route key (optional)</param>
    /// <returns>实例选择结果 / Instance selection result</returns>
    InstanceSelection Select(string serviceName, string routeKey = null);

    /// <summary>
    /// 刷新可用实例列表（服务上下线时调用）。
    /// </summary>
    /// <remarks>
    /// Refreshes the list of available instances (called when services go online or offline).
    /// </remarks>
    /// <param name="serviceName">服务名 / Service name</param>
    /// <param name="instanceIds">可用实例ID列表 / Available instance IDs</param>
    void RefreshInstances(string serviceName, IReadOnlyList<string> instanceIds);
}
