using System.Reflection;
using GameFrameX.GameAnalytics;
using GameFrameX.NetWork.Abstractions;
using GameFrameX.Proto.BuiltIn;
using GameFrameX.Foundation.Logger;

namespace GameFrameX.Launcher;

internal static class Program
{
    private static async Task Main(string[] args)
    {
        GameAnalyticsHelper.Init("ec33486a135ae80984dbfc66eede1e41", "379e705038d46691c5c567db947af1828f702861", Assembly.GetCallingAssembly().ImageRuntimeVersion);
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