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

using System.Text.Json.Serialization;
using GameFrameX.Foundation.Json;
using GameFrameX.Utility.Setting;

namespace GameFrameX.ServerManager;

/// <summary>
/// 游戏服务器信息
/// </summary>
public sealed class GameServiceInfo : IServiceInfo
{
    /// <summary>
    /// 构造游戏服务器信息
    /// </summary>
    /// <param name="type"></param>
    /// <param name="session"></param>
    /// <param name="sessionId"></param>
    /// <param name="serverName"></param>
    /// <param name="serverId"></param>
    /// <param name="minModuleMessageId"></param>
    /// <param name="maxModuleMessageId"></param>
    public GameServiceInfo(ServerType type, object session, string sessionId, string serverName, long serverId, short minModuleMessageId, short maxModuleMessageId)
    {
        Type = type;
        Session = session;
        ServerName = serverName;
        ServerId = serverId;
        MinModuleMessageId = minModuleMessageId;
        MaxModuleMessageId = maxModuleMessageId;
        SessionId = sessionId;
        StatusInfo = new ServiceStatusInfo();
    }

    /// <summary>
    /// 最大模块消息ID
    /// </summary>
    public short MaxModuleMessageId { get; set; }

    /// <summary>
    /// 最小模块消息ID
    /// </summary>
    public short MinModuleMessageId { get; }

    /// <summary>
    /// 会话
    /// </summary>
    [JsonIgnore]
    public object Session { get; }

    /// <summary>
    /// 服务器状态
    /// </summary>
    public ServiceStatusInfo StatusInfo { get; set; }

    /// <summary>
    /// 会话ID
    /// </summary>
    public string SessionId { get; }

    /// <summary>
    /// 服务器类型
    /// </summary>
    public ServerType Type { get; }

    /// <summary>
    /// 服务器名称
    /// </summary>
    public string ServerName { get; }

    /// <summary>
    /// 服务器ID
    /// </summary>
    public long ServerId { get; }

    /// <summary>
    /// 转换为字符串
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
        return JsonHelper.SerializeFormat(this);
    }
}