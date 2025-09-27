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
/// 消息操作业务类型
/// </summary>
public enum MessageOperationType : byte
{
    /// <summary>
    /// 空类型
    /// </summary>
    None,

    /// <summary>
    /// 心跳
    /// </summary>
    HeartBeat,

    /// <summary>
    /// 缓存查询
    /// </summary>
    Cache,

    /// <summary>
    /// 数据库查询
    /// </summary>
    Database,

    /// <summary>
    /// 游戏
    /// </summary>
    Game,

    /// <summary>
    /// 游戏管理
    /// </summary>
    GameManager,

    /// <summary>
    /// 禁止
    /// </summary>
    Forbid,

    /// <summary>
    /// 重启
    /// </summary>
    Reboot,

    /// <summary>
    /// 重连
    /// </summary>
    Reconnect,

    /// <summary>
    /// 重载
    /// </summary>
    Reload,

    /// <summary>
    /// 退出
    /// </summary>
    Exit,

    /// <summary>
    /// 踢人
    /// </summary>
    Kick,

    /// <summary>
    /// 通知
    /// </summary>
    Notify,

    /// <summary>
    /// 转发
    /// </summary>
    Forward,

    /// <summary>
    /// 注册
    /// </summary>
    Register,

    /// <summary>
    /// 请求链接服务器信息
    /// </summary>
    RequestConnectServer,
}