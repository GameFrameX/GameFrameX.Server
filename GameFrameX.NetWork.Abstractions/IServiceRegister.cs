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

namespace GameFrameX.NetWork.Abstractions;

/// <summary>
/// 服务注册接口，用于描述一个可被注册到集群中的服务器节点信息。
/// </summary>
public interface IServiceRegister
{
    /// <summary>
    /// 服务器类型，标识该节点承担的业务角色（如 Gateway、Game、DB 等）。
    /// </summary>
    public string Type { get; set; }

    /// <summary>
    /// 服务器名称，便于运维与监控时快速识别，通常保持全局唯一。
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// 外部访问 IP 地址，客户端或其他服务器通过该地址与本节点通信。
    /// </summary>
    public string Host { get; set; }

    /// <summary>
    /// 外部访问端口，与 <see cref="Host"/> 组合成完整的外部访问地址。
    /// </summary>
    public ushort Port { get; set; }

    /// <summary>
    /// 服务器唯一编号，整个集群生命周期内保持不变，用于路由、日志、监控等场景。
    /// </summary>
    public long ServerId { get; set; }

    /// <summary>
    /// 服务器实例 ID，每次进程启动时重新生成，用于区分同一 ServerId 的不同运行实例。
    /// </summary>
    public long ServerInstanceId { get; set; }
}