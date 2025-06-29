using System.Reflection;
using GameFrameX.NetWork.Abstractions;
using GameFrameX.Proto.BuiltIn;

namespace GameFrameX.Launcher;

internal static class Program
{
    private static async Task Main(string[] args)
    {
        await GameApp.Entry(args, () =>
        {
            CacheStateTypeManager.Init();
            MessageProtoHelper.Init(typeof(MessageProtoBuildInTag).Assembly, typeof(MessageProtoHandler).Assembly);
        }, LogConfiguration);
    }

    /// <summary>
    /// 日志配置
    /// </summary>
    /// <param name="options">配置对象</param>
    private static void LogConfiguration(LogOptions options)
    {
        // 发布之后控制台不输出
#if !DEBUG
        options.IsConsole = false;
#endif
    }
}