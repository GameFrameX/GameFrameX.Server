using GameFrameX.NetWork.Message;
using GameFrameX.Proto.BuiltIn;

namespace GameFrameX.Launcher
{
    internal static class Program
    {
        static async Task Main(string[] args)
        {
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