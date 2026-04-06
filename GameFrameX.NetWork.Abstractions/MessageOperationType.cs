// ==========================================================================================
//   GameFrameX 组织及其衍生项目的版权、商标、专利及其他相关权利
//   GameFrameX organization and its derivative projects' copyrights, trademarks, patents, and related rights
//   均受中华人民共和国及相关国际法律法规保护。
//   are protected by the laws of the People's Republic of China and relevant international regulations.
//   使用本项目须严格遵守相应法律法规及开源许可证之规定。
//   Usage of this project must strictly comply with applicable laws, regulations, and open-source licenses.
//   本项目采用 MIT 许可证与 Apache License 2.0 双许可证分发，
//   This project is dual-licensed under the MIT License and Apache License 2.0,
//   完整许可证文本请参见源代码根目录下的 LICENSE 文件。
//   please refer to the LICENSE file in the root directory of the source code for the full license text.
//   禁止利用本项目实施任何危害国家安全、破坏社会秩序、
//   It is prohibited to use this project to engage in any activities that endanger national security, disrupt social order,
//   侵犯他人合法权益等法律法规所禁止的行为！
//   or infringe upon the legitimate rights and interests of others, as prohibited by laws and regulations!
//   因基于本项目二次开发所产生的一切法律纠纷与责任，
//   Any legal disputes and liabilities arising from secondary development based on this project
//   本项目组织与贡献者概不承担。
//   shall be borne solely by the developer; the project organization and contributors assume no responsibility.
//   GitHub 仓库：https://github.com/GameFrameX
//   GitHub Repository: https://github.com/GameFrameX
//   Gitee  仓库：https://gitee.com/GameFrameX
//   Gitee Repository:  https://gitee.com/GameFrameX
//   CNB  仓库：https://cnb.cool/GameFrameX
//   CNB Repository:  https://cnb.cool/GameFrameX
//   官方文档：https://gameframex.doc.alianblank.com/
//   Official Documentation: https://gameframex.doc.alianblank.com/
//  ==========================================================================================


﻿// ==========================================================================================
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
/// 消息操作业务类型。
/// </summary>
/// <remarks>
/// Message operation business type enumeration.
/// </remarks>
public enum MessageOperationType : byte
{
    /// <summary>
    /// 空类型。
    /// </summary>
    /// <remarks>
    /// None type.
    /// </remarks>
    None,

    /// <summary>
    /// 心跳。
    /// </summary>
    /// <remarks>
    /// Heartbeat type.
    /// </remarks>
    HeartBeat = 1,

    /// <summary>
    /// 游戏逻辑相关消息。
    /// </summary>
    /// <remarks>
    /// Game logic related messages.
    /// </remarks>
    Game = 4,

    /// <summary>
    /// 服务注册，用于服务实例向中心注册自身信息，声明可提供的服务能力。
    /// </summary>
    /// <remarks>
    /// Service registration, used for service instances to register their information with the center and declare available service capabilities.
    /// </remarks>
    ServiceRegister = 16,

    /// <summary>
    /// 服务注销，用于服务实例从中心移除自身注册信息，停止服务宣告。
    /// </summary>
    /// <remarks>
    /// Service unregistration, used for service instances to remove their registration information from the center and stop service announcements.
    /// </remarks>
    ServiceUnRegister = 17,

    /// <summary>
    /// 服务上线通知，用于服务实例成功启动后向中心广播自身已就绪，可接受调用。
    /// </summary>
    /// <remarks>
    /// Service online notification, used for service instances to broadcast to the center that they are ready to accept calls after successful startup.
    /// </remarks>
    NotifyServiceOnLine = 18,

    /// <summary>
    /// 服务下线通知，用于服务实例即将停止时向中心广播自身将离线，不再接受调用。
    /// </summary>
    /// <remarks>
    /// Service offline notification, used for service instances to broadcast to the center that they will go offline and will no longer accept calls.
    /// </remarks>
    NotifyServiceOffLine = 19,

    /// <summary>
    /// 请求连接服务器信息，客户端向中心获取目标游戏服务器或网关的接入地址与凭证。
    /// </summary>
    /// <remarks>
    /// Request connection server information, used by clients to obtain the access address and credentials of the target game server or gateway from the center.
    /// </remarks>
    ConnectService = 25,

    /// <summary>
    /// 玩家注册，用于玩家会话首次建立时在中心记录玩家与具体游戏服务器的映射关系。
    /// </summary>
    /// <remarks>
    /// Player registration, used to record the mapping relationship between players and specific game servers in the center when the player session is first established.
    /// </remarks>
    PlayerRegister = 32,

    /// <summary>
    /// 玩家注销，用于玩家会话结束时在中心移除玩家与游戏服务器的映射关系。
    /// </summary>
    /// <remarks>
    /// Player unregistration, used to remove the mapping relationship between players and game servers from the center when the player session ends.
    /// </remarks>
    PlayerUnRegister = 33,

    /// <summary>
    /// 玩家上线通知，玩家会话成功建立后向相关服务广播玩家已登录，可推送游戏数据。
    /// </summary>
    /// <remarks>
    /// Player online notification, broadcasts to relevant services that the player has logged in after the player session is successfully established, allowing game data to be pushed.
    /// </remarks>
    NotifyPlayerOnLine = 34,

    /// <summary>
    /// 玩家下线通知，玩家会话断开时向相关服务广播玩家已离线，需清理玩家状态与数据。
    /// </summary>
    /// <remarks>
    /// Player offline notification, broadcasts to relevant services that the player has gone offline when the player session is disconnected, requiring cleanup of player state and data.
    /// </remarks>
    NotifyPlayerOffLine = 35,

    /// <summary>
    /// 最大值，保留位。
    /// </summary>
    /// <remarks>
    /// Maximum value, reserved bit.
    /// </remarks>
    Max = 64,
}