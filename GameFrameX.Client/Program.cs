// See https://aka.ms/new-console-template for more information

using GameFrameX.Bot;
using GameFrameX.NetWork.Abstractions;
using GameFrameX.Proto.Proto;
using GameFrameX.Utility.Log;

internal static class Program
{
    private const int m_BotCount = 100;

    static async Task Main(string[] args)
    {
        var logOption = LogOptions.Default;
        logOption.IsConsole = true;
        logOption.LogEventLevel = Serilog.Events.LogEventLevel.Information;
        LoggerHandler.Start(logOption);
 
        MessageProtoHelper.Init(typeof(ReqLogin).Assembly);

        for (int k = 0; k < m_BotCount; k++)
        {
            string botName = $"BotClient_{k + 1}";
            await Task.Run(() =>
            {
                var client = new BotClient(botName);
                _ = client.EntryAsync();
            });
        }

        Console.ReadKey();
    }
}