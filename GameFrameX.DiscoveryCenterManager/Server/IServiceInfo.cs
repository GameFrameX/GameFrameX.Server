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

using GameFrameX.Utility.Setting;

namespace GameFrameX.DiscoveryCenterManager.Server;
/// <summary>
/// 服务信息接口
/// </summary>
public interface IServiceInfo
{
    /// <summary>
    /// 会话ID，用于唯一标识当前服务会话
    /// </summary>
    string SessionId { get; }

    /// <summary>
    /// 服务器类型，标识当前服务所属的服务器类别
    /// </summary>
    ServerType Type { get; }

    /// <summary>
    /// 服务器名称，用于显示或区分不同服务实例
    /// </summary>
    string Name { get; }

    /// <summary>
    /// 内网主机地址，服务在内网中的访问地址
    /// </summary>
    string IntranetHost { get; }

    /// <summary>
    /// 内网端口号，服务在内网中的监听端口
    /// </summary>
    ushort IntranetPort { get; }

    /// <summary>
    /// 外网主机地址，服务在外网中的访问地址
    /// </summary>
    string ExternalHost { get; }

    /// <summary>
    /// 外网端口号，服务在外网中的监听端口
    /// </summary>
    ushort ExternalPort { get; }

    /// <summary>
    /// 服务器ID，全局唯一标识该服务器
    /// </summary>
    long ServerId { get; }

    /// <summary>
    /// 服务器实例ID，用于区分同一服务器下的不同实例
    /// </summary>
    long ServerInstanceId { get; }
}