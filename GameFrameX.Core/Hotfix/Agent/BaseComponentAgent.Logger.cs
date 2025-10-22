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

using GameFrameX.Core.Components;
using Serilog;
using Serilog.Events;

namespace GameFrameX.Core.Hotfix.Agent;

/// <summary>
/// 基础组件代理类，用于管理组件与Actor之间的交互
/// </summary>
/// <typeparam name="TComponent">具体的组件类型</typeparam>
public abstract partial class BaseComponentAgent<TComponent> where TComponent : BaseComponent
{
    private ILogger _logger;

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    protected virtual ILogger InitLogger()
    {
        return default;
    }

    private ILogger GetLogger()
    {
        if (_logger == default)
        {
            _logger = InitLogger() ?? Serilog.Log.Logger;
        }

        return _logger;
    }

    /// <summary>
    /// Records a debug message with optional format parameters
    /// </summary>
    /// <param name="msg">The debug message to record / 要记录的调试消息</param>
    /// <param name="args">Optional format parameters for the message / 消息的可选格式参数</param>
    /// <remarks>
    /// 用于记录调试级别的日志信息，通常在开发和测试阶段使用
    /// </remarks>
    public void Debug(string msg, params object[] args)
    {
        GetLogger().Debug(msg, args);
    }

    /// <summary>
    /// Records a debug message with exception and optional format parameters
    /// </summary>
    /// <param name="exception">The exception to log / 要记录的异常</param>
    /// <param name="msg">The debug message to record / 要记录的调试消息</param>
    /// <param name="args">Optional format parameters for the message / 消息的可选格式参数</param>
    /// <remarks>
    /// 用于记录带有异常信息的调试级别日志
    /// </remarks>
    public void Debug(Exception exception, string msg, params object[] args)
    {
        GetLogger().Debug(exception, msg, args);
    }

    /// <summary>
    /// Records a simple debug message without parameters
    /// </summary>
    /// <param name="msg">The debug message to record / 要记录的调试消息</param>
    /// <remarks>
    /// 用于记录简单的调试消息，不需要格式化参数
    /// </remarks>
    public void Debug(string msg)
    {
        GetLogger().Debug(msg);
    }

    /// <summary>
    /// Records a debug message with exception only
    /// </summary>
    /// <param name="exception">The exception to log / 要记录的异常</param>
    /// <param name="msg">The debug message to record / 要记录的调试消息</param>
    /// <remarks>
    /// 用于记录带有异常的简单调试消息
    /// </remarks>
    public void Debug(Exception exception, string msg)
    {
        GetLogger().Debug(exception, msg);
    }

    /// <summary>
    /// Records a debug message with single parameter
    /// </summary>
    /// <param name="msg">The debug message template / 调试消息模板</param>
    /// <param name="arg">Single parameter for the message / 消息的单个参数</param>
    /// <remarks>
    /// 用于记录带有单个参数的调试消息
    /// </remarks>
    public void Debug(string msg, object arg)
    {
        GetLogger().Debug(msg, arg);
    }

    /// <summary>
    /// Records a debug message with two parameters
    /// </summary>
    /// <param name="msg">The debug message template / 调试消息模板</param>
    /// <param name="arg1">First parameter for the message / 消息的第一个参数</param>
    /// <param name="arg2">Second parameter for the message / 消息的第二个参数</param>
    /// <remarks>
    /// 用于记录带有两个参数的调试消息
    /// </remarks>
    public void Debug(string msg, object arg1, object arg2)
    {
        GetLogger().Debug(msg, arg1, arg2);
    }

    /// <summary>
    /// Records a debug message with three parameters
    /// </summary>
    /// <param name="msg">The debug message template / 调试消息模板</param>
    /// <param name="arg1">First parameter for the message / 消息的第一个参数</param>
    /// <param name="arg2">Second parameter for the message / 消息的第二个参数</param>
    /// <param name="arg3">Third parameter for the message / 消息的第三个参数</param>
    /// <remarks>
    /// 用于记录带有三个参数的调试消息
    /// </remarks>
    public void Debug(string msg, object arg1, object arg2, object arg3)
    {
        GetLogger().Debug(msg, arg1, arg2, arg3);
    }

    /// <summary>
    /// Records verbose messages with optional format parameters
    /// </summary>
    /// <param name="msg">The verbose message to record / 要记录的详细消息</param>
    /// <param name="args">Optional format parameters for the message / 消息的可选格式参数</param>
    /// <remarks>
    /// 用于记录详细级别的日志信息，比Debug更详细的信息
    /// </remarks>
    public void Verbose(string msg, params object[] args)
    {
        GetLogger().Verbose(msg, args);
    }

    /// <summary>
    /// Records verbose messages with exception and optional format parameters
    /// </summary>
    /// <param name="exception">The exception to log / 要记录的异常</param>
    /// <param name="msg">The verbose message to record / 要记录的详细消息</param>
    /// <param name="args">Optional format parameters for the message / 消息的可选格式参数</param>
    /// <remarks>
    /// 用于记录带有异常信息的详细级别日志
    /// </remarks>
    public void Verbose(Exception exception, string msg, params object[] args)
    {
        GetLogger().Verbose(exception, msg, args);
    }

    /// <summary>
    /// Records information messages with optional format parameters
    /// </summary>
    /// <param name="msg">The information message to record / 要记录的信息消息</param>
    /// <param name="args">Optional format parameters for the message / 消息的可选格式参数</param>
    /// <remarks>
    /// 用于记录信息级别的日志，通常用于记录应用程序的正常运行信息
    /// </remarks>
    public void Information(string msg, params object[] args)
    {
        GetLogger().Information(msg, args);
    }

    /// <summary>
    /// Records information messages with exception and optional format parameters
    /// </summary>
    /// <param name="exception">The exception to log / 要记录的异常</param>
    /// <param name="msg">The information message to record / 要记录的信息消息</param>
    /// <param name="args">Optional format parameters for the message / 消息的可选格式参数</param>
    /// <remarks>
    /// 用于记录带有异常信息的信息级别日志
    /// </remarks>
    public void Information(Exception exception, string msg, params object[] args)
    {
        GetLogger().Information(exception, msg, args);
    }

    /// <summary>
    /// Records warning messages with optional format parameters
    /// </summary>
    /// <param name="msg">The warning message to record / 要记录的警告消息</param>
    /// <param name="args">Optional format parameters for the message / 消息的可选格式参数</param>
    /// <remarks>
    /// 用于记录警告级别的日志，通常用于记录可能的问题或需要注意的情况
    /// </remarks>
    public void Warning(string msg, params object[] args)
    {
        GetLogger().Warning(msg, args);
    }

    /// <summary>
    /// Records warning messages with exception and optional format parameters
    /// </summary>
    /// <param name="exception">The exception to log / 要记录的异常</param>
    /// <param name="msg">The warning message to record / 要记录的警告消息</param>
    /// <param name="args">Optional format parameters for the message / 消息的可选格式参数</param>
    /// <remarks>
    /// 用于记录带有异常信息的警告级别日志
    /// </remarks>
    public void Warning(Exception exception, string msg, params object[] args)
    {
        GetLogger().Warning(exception, msg, args);
    }

    /// <summary>
    /// Records error messages with optional format parameters
    /// </summary>
    /// <param name="msg">The error message to record / 要记录的错误消息</param>
    /// <param name="args">Optional format parameters for the message / 消息的可选格式参数</param>
    /// <remarks>
    /// 用于记录错误级别的日志，通常用于记录应用程序中的错误情况
    /// </remarks>
    public void Error(string msg, params object[] args)
    {
        GetLogger().Error(msg, args);
    }

    /// <summary>
    /// Records error messages with exception and optional format parameters
    /// </summary>
    /// <param name="exception">The exception to log / 要记录的异常</param>
    /// <param name="msg">The error message to record / 要记录的错误消息</param>
    /// <param name="args">Optional format parameters for the message / 消息的可选格式参数</param>
    /// <remarks>
    /// 用于记录带有异常信息的错误级别日志
    /// </remarks>
    public void Error(Exception exception, string msg, params object[] args)
    {
        GetLogger().Error(exception, msg, args);
    }

    /// <summary>
    /// Records fatal messages with optional format parameters
    /// </summary>
    /// <param name="msg">The fatal message to record / 要记录的致命错误消息</param>
    /// <param name="args">Optional format parameters for the message / 消息的可选格式参数</param>
    /// <remarks>
    /// 用于记录致命错误级别的日志，通常用于记录导致应用程序无法继续运行的严重错误
    /// </remarks>
    public void Fatal(string msg, params object[] args)
    {
        GetLogger().Fatal(msg, args);
    }

    /// <summary>
    /// Records fatal messages with exception and optional format parameters
    /// </summary>
    /// <param name="exception">The exception to log / 要记录的异常</param>
    /// <param name="msg">The fatal message to record / 要记录的致命错误消息</param>
    /// <param name="args">Optional format parameters for the message / 消息的可选格式参数</param>
    /// <remarks>
    /// 用于记录带有异常信息的致命错误级别日志
    /// </remarks>
    public void Fatal(Exception exception, string msg, params object[] args)
    {
        GetLogger().Fatal(exception, msg, args);
    }

    /// <summary>
    /// Records log messages with specified log level and optional format parameters
    /// </summary>
    /// <param name="level">The log level to use / 要使用的日志级别</param>
    /// <param name="msg">The log message to record / 要记录的日志消息</param>
    /// <param name="args">Optional format parameters for the message / 消息的可选格式参数</param>
    /// <remarks>
    /// 用于记录指定级别的日志消息，允许动态指定日志级别
    /// </remarks>
    public void Log(LogEventLevel level, string msg, params object[] args)
    {
        GetLogger().Write(level, msg, args);
    }

    /// <summary>
    /// Records log messages with specified log level, exception and optional format parameters
    /// </summary>
    /// <param name="level">The log level to use / 要使用的日志级别</param>
    /// <param name="exception">The exception to log / 要记录的异常</param>
    /// <param name="msg">The log message to record / 要记录的日志消息</param>
    /// <param name="args">Optional format parameters for the message / 消息的可选格式参数</param>
    /// <remarks>
    /// 用于记录带有异常信息的指定级别日志消息
    /// </remarks>
    public void Log(LogEventLevel level, Exception exception, string msg, params object[] args)
    {
        GetLogger().Write(level, exception, msg, args);
    }
}