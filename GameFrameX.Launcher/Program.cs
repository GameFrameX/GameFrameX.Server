using GameFrameX.GameAnalytics;
using GameFrameX.NetWork.Abstractions;
using GameFrameX.NetWork.Message;
using GameFrameX.Proto.BuiltIn;

namespace GameFrameX.Launcher
{
    internal static class Program
    {
        static async Task Main(string[] args)
        {
            GameAnalyticsHelper.Init("ec33486a135ae80984dbfc66eede1e41", "379e705038d46691c5c567db947af1828f702861", System.Reflection.Assembly.GetCallingAssembly().ImageRuntimeVersion);
            await GameApp.Entry(args, () =>
            {
                CacheStateTypeManager.Init();
                MessageProtoHelper.Init(typeof(MessageProtoBuildInTag).Assembly);
                MessageProtoHelper.Init(typeof(MessageProtoHandler).Assembly, false);
            }, LogAction);
        }

        private static void LogAction(LogOptions options)
        {
            // options.IsConsole = false;
        }
    }
}