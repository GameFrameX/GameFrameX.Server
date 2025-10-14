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
/// Operation error codes
/// </summary>
/// <remarks>操作错误码</remarks>
public static class OperationErrorCode
{
    /// <summary>
    /// Success
    /// </summary>
    /// <remarks>成功</remarks>
    [System.ComponentModel.Description("Success / 成功")]
    public const int Success = 0;

    /// <summary>
    /// Configuration error
    /// </summary>
    /// <remarks>配置表错误</remarks>
    [System.ComponentModel.Description("Configuration error / 配置表错误")]
    public const int ConfigErr = 1;

    /// <summary>
    /// Parameter error
    /// </summary>
    /// <remarks>参数错误</remarks>
    [System.ComponentModel.Description("Parameter error / 客户端传递参数错误")]
    public const int ParamErr = 2;

    /// <summary>
    /// Insufficient cost
    /// </summary>
    /// <remarks>消耗不足</remarks>
    [System.ComponentModel.Description("Insufficient cost / 消耗不足")]
    public const int CostNotEnough = 3;

    /// <summary>
    /// Service not enabled
    /// </summary>
    /// <remarks>未开通服务</remarks>
    [System.ComponentModel.Description("Service not enabled / 未开通服务")]
    public const int Forbidden = 4;

    /// <summary>
    /// Not found
    /// </summary>
    /// <remarks>不存在</remarks>
    [System.ComponentModel.Description("Not found / 不存在")]
    public const int NotFound = 5;

    /// <summary>
    /// Already exists
    /// </summary>
    /// <remarks>已经存在</remarks>
    [System.ComponentModel.Description("Already exists / 已经存在")]
    public const int HasExist = 6;

    /// <summary>
    /// Account cannot be null or empty
    /// </summary>
    /// <remarks>账号不存在或为空</remarks>
    [System.ComponentModel.Description("Account cannot be null or empty / 账号不存在或为空")]
    public const int AccountCannotBeNull = 7;

    /// <summary>
    /// Unable to execute database modification
    /// </summary>
    /// <remarks>无法执行数据库修改</remarks>
    [System.ComponentModel.Description("Unable to execute database modification / 无法执行数据库修改")]
    public const int Unprocessable = 8;

    /// <summary>
    /// Unknown platform
    /// </summary>
    /// <remarks>未知平台</remarks>
    [System.ComponentModel.Description("Unknown platform / 未知平台")]
    public const int UnknownPlatform = 9;

    /// <summary>
    /// Normal notification
    /// </summary>
    /// <remarks>正常通知</remarks>
    [System.ComponentModel.Description("Normal notification / 正常通知")]
    public const int Notice = 10;

    /// <summary>
    /// Function not enabled, main message blocked
    /// </summary>
    /// <remarks>功能未开启，主消息屏蔽</remarks>
    [System.ComponentModel.Description("Function not enabled, main message blocked / 功能未开启，主消息屏蔽")]
    public const int FuncNotOpen = 11;

    /// <summary>
    /// Other
    /// </summary>
    /// <remarks>其他</remarks>
    [System.ComponentModel.Description("Other / 其他")]
    public const int Other = 12;

    /// <summary>
    /// Internal server error
    /// </summary>
    /// <remarks>内部服务错误</remarks>
    [System.ComponentModel.Description("Internal server error / 内部服务错误")]
    public const int InternalServerError = 13;

    /// <summary>
    /// Server fully loaded
    /// </summary>
    /// <remarks>通知客户端服务器人数已达上限</remarks>
    [System.ComponentModel.Description("Server fully loaded / 通知客户端服务器人数已达上限")]
    public const int ServerFullyLoaded = 14;

    /// <summary>
    /// Has expired
    /// </summary>
    /// <remarks>已经过期</remarks>
    [System.ComponentModel.Description("Has expired / 已经过期")]
    public const int HasExpiration = 15;

    /// <summary>
    /// Player unauthorized, kick offline directly
    /// </summary>
    /// <remarks>角色未授权;直接踢下线</remarks>
    [System.ComponentModel.Description("Player unauthorized, kick offline directly / 角色未授权;直接踢下线")]
    public const int PlayerUnauthorized = 16;

    /// <summary>
    /// No permission
    /// </summary>
    /// <remarks>没有权限</remarks>
    [System.ComponentModel.Description("No permission / 没有权限")]
    public const int NoPermission = 17;

    /// <summary>
    /// Execution timeout
    /// </summary>
    /// <remarks>执行超时</remarks>
    [System.ComponentModel.Description("Execution timeout / 执行超时")]
    public const int TimeOut = 18;
}