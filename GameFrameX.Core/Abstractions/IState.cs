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

namespace GameFrameX.Core.Abstractions;

/// <summary>
/// 状态接口
/// 用于管理和维护对象的状态信息
/// </summary>
public interface IState
{
    /// <summary>
    /// 读取状态
    /// </summary>
    /// <returns>一个表示异步操作的任务，该任务在状态读取完成时完成</returns>
    /// <remarks>
    /// 此方法用于异步读取对象的当前状态信息
    /// 通常用于从持久化存储（如数据库、文件等）中加载状态
    /// 在对象初始化或需要刷新状态时调用
    /// 实现此方法时应考虑异常处理和并发访问的情况
    /// </remarks>
    public Task ReadStateAsync();

    /// <summary>
    /// 更新状态
    /// </summary>
    /// <returns>一个表示异步操作的任务，该任务在状态更新完成时完成</returns>
    /// <remarks>
    /// 此方法用于异步更新对象的状态信息
    /// 在状态发生变化时应调用此方法以保持状态的同步
    /// 负责将当前内存中的状态持久化到存储介质中
    /// 建议在以下情况调用此方法：
    /// 1. 状态数据发生重要变更时
    /// 2. 定期保存检查点时
    /// 3. 系统关闭前的状态保存
    /// 实现时需要注意：
    /// - 确保数据一致性
    /// - 处理并发写入情况
    /// - 考虑性能影响，适当使用缓存策略
    /// </remarks>
    public Task WriteStateAsync();
}