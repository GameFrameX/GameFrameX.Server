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

namespace GameFrameX.Utility.Setting;

/// <summary>
/// 服务器类型
/// </summary>
[Flags]
public enum ServerType
{
    /// <summary>
    /// 空值
    /// </summary>
    None = -1,

    /// <summary>
    /// 客户端
    /// </summary>
    Client = 0,

    #region 基础服务

    /// <summary>
    /// 日志服
    /// </summary>
    Log = 1 << 1,

    /// <summary>
    /// 数据库
    /// </summary>
    DataBase = Log << 1,

    /// <summary>
    /// 缓存服
    /// </summary>
    Cache = DataBase << 1,

    /// <summary>
    /// 网关服
    /// </summary>
    Gateway = Cache << 1,

    /// <summary>
    /// 账号服
    /// </summary>
    Account = Gateway << 1,

    /// <summary>
    /// 路由
    /// </summary>
    Router = Account << 1,

    /// <summary>
    /// 服务发现中心服，用于发现其他服务器。
    /// </summary>
    DiscoveryCenter = Router << 1,

    /// <summary>
    /// 远程备份
    /// </summary>
    Backup = DiscoveryCenter << 1,

    #endregion

    /// <summary>
    /// 登录服务器
    /// </summary>
    Login = Backup << 1,

    /// <summary>
    /// 游戏服
    /// </summary>
    Game = Login << 1,

    /// <summary>
    /// 匹配服
    /// </summary>
    Match = Game << 1,

    /// <summary>
    /// 充值服
    /// </summary>
    Recharge = Match << 1,

    /// <summary>
    /// 逻辑服
    /// </summary>
    Logic = Recharge << 1,

    /// <summary>
    /// 聊天服
    /// </summary>
    Chat = Logic << 1,

    /// <summary>
    /// 邮件服
    /// </summary>
    Mail = Chat << 1,

    /// <summary>
    /// 公会服
    /// </summary>
    Guild = Mail << 1,

    /// <summary>
    /// 房间服
    /// </summary>
    Room = Guild << 1,

    /// <summary>
    /// 全部
    /// </summary>
    All = Room | Game | Logic | Recharge | Chat | Mail | Guild | Backup | Account | DiscoveryCenter | Gateway | Cache | DataBase | Log,
}