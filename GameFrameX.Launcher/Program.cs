using GameFrameX.Entry;
using GameFrameX.NetWork.Message;

namespace GameFrameX.Launcher
{
    internal static class Program
    {
        static async Task Main(string[] args)
        {
            await GameApp.Entry(args, () =>
            {
                CacheStateTypeManager.Init();
                MessageProtoHelper.Init(typeof(MessageProtoHandler).Assembly);
            }, LogAction);
        }

        private static void LogAction(LogOptions options)
        {
            options.IsConsole = false;
        }
    }
}