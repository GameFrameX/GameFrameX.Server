using System.Reflection;
using GameFrameX.GameAnalytics;
using GameFrameX.NetWork.Abstractions;

namespace GameFrameX.Launcher;

internal static class Program
{
    private static async Task Main(string[] args)
    {
        GameAnalyticsHelper.Init("ec33486a135ae80984dbfc66eede1e41", "379e705038d46691c5c567db947af1828f702861", Assembly.GetCallingAssembly().ImageRuntimeVersion);
        await GameApp.Entry(args, () =>
        {
            CacheStateTypeManager.Init();
            MessageProtoHelper.Init(typeof(MessageProtoHandler).Assembly);
        }, LogAction);
    }

    private static void LogAction(LogOptions options)
    {
        // 发布之后控制台不输出
#if !DEBUG
        options.IsConsole = false;
#endif
    }
}