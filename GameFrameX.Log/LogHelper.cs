using System;

namespace GameFrameX.Log;

/// <summary>
/// 日志帮助类
/// </summary>
public static class LogHelper
{
    /// <summary>
    /// 记录带有可选格式参数的调试消息。
    /// </summary>
    /// <param name="msg">要记录的调试消息。</param>
    /// <param name="args">消息的可选格式参数。</param>
    public static void Debug(string msg, params object[] args)
    {
        Serilog.Log.Debug(msg, args);
    }


    // static void StackTrace()
    // {
    //     StackTrace stackTrace = new StackTrace(true);
    //     Serilog.Log.Information("StackTrace Start:");
    //     foreach (var frame in stackTrace.GetFrames())
    //     {
    //         Serilog.Log.Information("Method:" + frame.GetMethod().Name + " file:" + frame.GetFileName() + " Line:" + frame.GetFileLineNumber()); //获取
    //     }
    //
    //     Serilog.Log.Information("StackTrace End: \n");
    // }

    /// <summary>
    /// 记录带有格式参数的信息消息。
    /// </summary>
    /// <param name="msg">要记录的信息消息。</param>
    /// <param name="args">消息的格式参数。</param>
    public static void Info(string msg, params object[] args)
    {
        Serilog.Log.Information(msg, args);
    }

    /// <summary>
    /// 记录信息消息。
    /// </summary>
    /// <param name="msg">要记录的异常对象。</param>
    public static void Info(Exception msg)
    {
        Serilog.Log.Information(msg.Message);
    }

    /// <summary>
    /// 记录信息消息
    /// </summary>
    /// <param name="message">要记录的信息对象</param>
    public static void Info(object message)
    {
        Serilog.Log.Information(message?.ToString() ?? "null object");
    }

    /// <summary>
    /// 记录带有格式参数的警告消息。
    /// </summary>
    /// <param name="msg">要记录的警告消息。</param>
    /// <param name="args">消息的格式参数。</param>
    public static void Warn(string msg, params object[] args)
    {
        Serilog.Log.Warning(msg, args);
    }

    /// <summary>
    /// 记录带有格式参数的错误消息。
    /// </summary>
    /// <param name="msg">要记录的错误消息。</param>
    /// <param name="args">消息的格式参数。</param>
    public static void Error(string msg, params object[] args)
    {
        Serilog.Log.Error(msg, args);
        // StackTrace();
    }

    /// <summary>
    /// 记录严重错误消息。
    /// </summary>
    /// <param name="msg">要记录的严重错误消息。</param>
    public static void Fatal(string msg)
    {
        Serilog.Log.Fatal(msg);
        // StackTrace();
    }

    /// <summary>
    /// 记录严重的异常错误。
    /// </summary>
    /// <param name="msg">要记录的异常对象。</param>
    public static void Fatal(Exception msg)
    {
        Serilog.Log.Fatal(msg, msg.Message);
        // StackTrace();
    }

    /// <summary>
    /// 记录严重的异常错误
    /// </summary>
    /// <param name="exception">异常</param>
    public static void Error(Exception exception)
    {
        Serilog.Log.Error(exception, exception.Message);
    }
}