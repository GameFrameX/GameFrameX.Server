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

namespace GameFrameX.NetWork;

/// <summary>
/// 操作错误码
/// </summary>
public static class OperationErrorCode
{
    /// <summary>
    /// 成功
    /// </summary>
    [System.ComponentModel.Description("成功")]
    public const int Success = 0;

    /// <summary>
    /// 配置表错误
    /// </summary>
    [System.ComponentModel.Description("配置表错误")]
    public const int ConfigErr = 1;

    /// <summary>
    /// 参数错误
    /// </summary>
    [System.ComponentModel.Description("客户端传递参数错误")]
    public const int ParamErr = 2;

    /// <summary>
    /// 消耗不足
    /// </summary>
    [System.ComponentModel.Description("消耗不足")]
    public const int CostNotEnough = 3;

    /// <summary>
    /// 未开通服务
    /// </summary>
    [System.ComponentModel.Description("未开通服务")]
    public const int Forbidden = 4;

    /// <summary>
    /// 不存在
    /// </summary>
    [System.ComponentModel.Description("不存在")]
    public const int NotFound = 5;

    /// <summary>
    /// 已经存在
    /// </summary>
    [System.ComponentModel.Description("已经存在")]
    public const int HasExist = 6;

    /// <summary>
    /// 账号不存在或为空
    /// </summary>
    [System.ComponentModel.Description("账号不存在或为空")]
    public const int AccountCannotBeNull = 7;

    /// <summary>
    /// 无法执行数据库修改
    /// </summary>
    [System.ComponentModel.Description("无法执行数据库修改")]
    public const int Unprocessable = 8;

    /// <summary>
    /// 未知平台
    /// </summary>
    [System.ComponentModel.Description("未知平台")]
    public const int UnknownPlatform = 9;

    /// <summary>
    /// 正常通知
    /// </summary>
    [System.ComponentModel.Description("正常通知")]
    public const int Notice = 10;

    /// <summary>
    /// 功能未开启，主消息屏蔽
    /// </summary>
    [System.ComponentModel.Description("功能未开启，主消息屏蔽")]
    public const int FuncNotOpen = 11;

    /// <summary>
    /// 其他
    /// </summary>
    [System.ComponentModel.Description("其他")]
    public const int Other = 12;

    /// <summary>
    /// 内部服务错误
    /// </summary>
    [System.ComponentModel.Description("内部服务错误")]
    public const int InternalServerError = 13;

    /// <summary>
    /// 通知客户端服务器人数已达上限
    /// </summary>
    [System.ComponentModel.Description("通知客户端服务器人数已达上限")]
    public const int ServerFullyLoaded = 14;

    /// <summary>
    /// 已经过期
    /// </summary>
    [System.ComponentModel.Description("已经过期")]
    public const int HasExpiration = 15;

    /// <summary>
    /// 角色未授权;直接踢下线
    /// </summary>
    [System.ComponentModel.Description("角色未授权;直接踢下线")]
    public const int PlayerUnauthorized = 16;

    /// <summary>
    /// 没有权限
    /// </summary>
    [System.ComponentModel.Description("没有权限")]
    public const int NoPermission = 17;

    /// <summary>
    /// 执行超时 / Execution timeout
    /// </summary>
    [System.ComponentModel.Description("执行超时")]
    public const int TimeOut = 18;
}