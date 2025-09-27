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

using GameFrameX.Core.Abstractions.Agent;

namespace GameFrameX.Core.Abstractions;

/// <summary>
/// IActor 接口定义
/// 定义了一个Actor（参与者）的基本行为和属性，用于管理游戏中的实体对象
/// </summary>
public interface IActor : IWorker
{
    /// <summary>
    /// 获取或设置 IActor 的唯一标识
    /// 用于在系统中唯一标识一个Actor实例
    /// </summary>
    long Id { get; set; }

    /// <summary>
    /// 获取是否自动回收
    /// 标识当前Actor是否可以被系统自动回收释放
    /// </summary>
    bool AutoRecycle { get; }

    /// <summary>
    /// 获取工作 Actor
    /// 返回当前Actor的工作实例，用于处理具体的业务逻辑
    /// </summary>
    IWorkerActor WorkerActor { get; }

    /// <summary>
    /// 获取订阅的哈希列表
    /// 存储当前Actor订阅的所有调度器ID
    /// </summary>
    HashSet<long> ScheduleIdSet { get; }

    /// <summary>
    /// 获取或设置 Actor 类型
    /// 用于标识Actor的具体类型，便于系统进行分类管理
    /// </summary>
    ushort Type { get; set; }

    /// <summary>
    /// 清理全部代理
    /// 清除当前Actor关联的所有组件代理实例
    /// </summary>
    void ClearAgent();

    /// <summary>
    /// 反激活所有组件
    /// 使当前Actor的所有组件进入非激活状态，通常在Actor被回收前调用
    /// </summary>
    /// <returns>一个表示异步操作的任务</returns>
    Task Inactive();

    /// <summary>
    /// 根据组件类型获取对应的 IComponentAgent
    /// 获取指定类型的组件代理实例，如果不存在且isNew为true则创建新实例
    /// </summary>
    /// <param name="agentType">组件类型，指定要获取的组件的具体类型</param>
    /// <param name="isNew">是否当获取为空的时候默认创建，默认值为true</param>
    /// <returns>一个表示异步操作的任务，返回 IComponentAgent 实例</returns>
    Task<IComponentAgent> GetComponentAgent(Type agentType, bool isNew = true);

    /// <summary>
    /// 根据组件类型获取对应的 IComponentAgent
    /// 泛型方法版本，用于获取指定类型的组件代理实例
    /// </summary>
    /// <typeparam name="T">组件类型，必须实现IComponentAgent接口</typeparam>
    /// <param name="isNew">是否当获取为空的时候默认创建，默认值为true</param>
    /// <returns>一个表示异步操作的任务，返回指定类型的 IComponentAgent 实例</returns>
    Task<T> GetComponentAgent<T>(bool isNew = true) where T : IComponentAgent;

    /// <summary>
    /// 获取所有已激活的组件代理实例
    /// </summary>
    /// <remarks>
    /// 遍历组件映射字典(_componentsMap),筛选出所有处于激活状态(IsActive=true)的组件,
    /// 并获取它们对应的代理实例。这个方法通常用于需要批量处理或遍历所有活跃组件的场景。
    /// </remarks>
    /// <returns>返回包含所有已激活组件代理实例的列表</returns>
    List<IComponentAgent> GetActiveComponentAgents();

    /// <summary>
    /// 设置自动回收标记
    /// 配置当前Actor是否允许被系统自动回收
    /// </summary>
    /// <param name="autoRecycle">是否自动回收，true表示允许自动回收，false表示禁止自动回收</param>
    void SetAutoRecycle(bool autoRecycle);

    /// <summary>
    /// Actor 跨天处理
    /// 处理游戏服务器跨天时Actor需要执行的相关逻辑
    /// </summary>
    /// <param name="serverDay">服务器运行天数，表示服务器运行的累计天数</param>
    /// <returns>一个表示异步操作的任务</returns>
    Task CrossDay(int serverDay);

    /// <summary>
    /// 设置Actor的数据
    /// </summary>
    /// <typeparam name="T">要存储的数据类型</typeparam>
    /// <param name="key">数据的键名</param>
    /// <param name="value">要存储的数据值</param>
    /// <remarks>
    /// 用于在Actor中存储任意类型的数据，通过键值对的方式进行管理。
    /// 如果键已存在，则会覆盖原有的值。
    /// </remarks>
    void SetData<T>(string key, T value);

    /// <summary>
    /// 获取Actor中存储的数据
    /// </summary>
    /// <typeparam name="T">要获取的数据类型</typeparam>
    /// <param name="key">数据的键名</param>
    /// <returns>返回指定类型的数据值</returns>
    /// <remarks>
    /// 如果指定的键不存在或类型不匹配，可能会抛出异常。
    /// 使用前建议先确认数据是否存在。
    /// </remarks>
    T GetData<T>(string key);

    /// <summary>
    /// 移除Actor中存储的数据
    /// </summary>
    /// <param name="key">要移除的数据键名</param>
    /// <returns>如果成功移除数据返回true，如果键不存在返回false</returns>
    /// <remarks>
    /// 从Actor的数据存储中移除指定键的数据。
    /// 如果键不存在，则不会产生任何效果。
    /// </remarks>
    bool RemoveData(string key);

    /// <summary>
    /// 清除Actor中存储的所有数据
    /// </summary>
    /// <remarks>
    /// 该方法会清空Actor中所有通过SetData方法存储的数据。
    /// 清除后所有数据将无法恢复，请谨慎使用。
    /// 通常在Actor被回收或重置时调用此方法。
    /// </remarks>
    void ClearData();

    /// <summary>
    /// Actor 回收时的处理方法,该方法在Actor被回收时自动调用，该函数不能由外部调用。只能由ActorManager内部调用
    /// </summary>
    /// <remarks>
    /// 当 Actor 被系统回收时调用此方法。
    /// 用于执行必要的清理工作，如:
    /// - 释放占用的资源
    /// - 清理组件状态
    /// - 保存需要持久化的数据
    /// - 取消订阅的事件
    /// - 断开网络连接等
    /// </remarks>
    /// <returns>表示异步操作的任务</returns>
    Task OnRecycle();

    /// <summary>
    /// 添加一个在Actor回收时执行一次的回调事件
    /// </summary>
    /// <param name="action">要执行的回调方法</param>
    /// <remarks>
    /// 该回调事件只会在Actor被回收时触发一次，之后会自动移除。
    /// 通常用于:
    /// - 执行一次性的清理操作
    /// - 触发状态变更通知
    /// - 记录回收日志等场景
    /// </remarks>
    void AddOnceRecycleCallback(Action action);
}