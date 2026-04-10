// ==========================================================================================
//   GameFrameX 组织及其衍生项目的版权、商标、专利及其他相关权利
//   GameFrameX organization and its derivative projects' copyrights, trademarks, patents, and related rights
//   均受中华人民共和国及相关国际法律法规保护。
//   are protected by the laws of the People's Republic of China and relevant international regulations.
//   使用本项目须严格遵守相应法律法规及开源许可证之规定。
//   Usage of this project must strictly comply with applicable laws, regulations, and open-source licenses.
//   本项目采用 MIT 许可证与 Apache License 2.0 双许可证分发，
//   This project is dual-licensed under the MIT License and Apache License 2.0,
//   完整许可证文本请参见源代码根目录下的 LICENSE 文件。
//   please refer to the LICENSE file in the root directory of the source code for the full license text.
//   禁止利用本项目实施任何危害国家安全、破坏社会秩序、
//   It is prohibited to use this project to engage in any activities that endanger national security, disrupt social order,
//   侵犯他人合法权益等法律法规所禁止的行为！
//   or infringe upon the legitimate rights and interests of others, as prohibited by laws and regulations!
//   因基于本项目二次开发所产生的一切法律纠纷与责任，
//   Any legal disputes and liabilities arising from secondary development based on this project
//   本项目组织与贡献者概不承担。
//   shall be borne solely by the developer; the project organization and contributors assume no responsibility.
//   GitHub 仓库：https://github.com/GameFrameX
//   GitHub Repository: https://github.com/GameFrameX
//   Gitee  仓库：https://gitee.com/GameFrameX
//   Gitee Repository:  https://gitee.com/GameFrameX
//   CNB  仓库：https://cnb.cool/GameFrameX
//   CNB Repository:  https://cnb.cool/GameFrameX
//   官方文档：https://gameframex.doc.alianblank.com/
//   Official Documentation: https://gameframex.doc.alianblank.com/
//  ==========================================================================================

using GameFrameX.Client.Bot;
using GameFrameX.NetWork.Abstractions;
using GameFrameX.Proto.Proto;
using GameFrameX.ProtoBuf.Net;
using GameFrameX.Foundation.Logger;

internal static class Program
{
    static async Task Main(string[] args)
    {
        var options = BotRunOptions.Parse(args);
        var logOption = LogOptions.Default;
        logOption.LogType = "ClientBot";
        logOption.IsConsole = true;
        logOption.LogEventLevel = Serilog.Events.LogEventLevel.Information;
        LogHandler.Create(logOption);

        MessageProtoHelper.Init(typeof(ReqLogin).Assembly);
        WarmUpProtoSerializer();

        LogHelper.Info(
            "Bot options: bot-count={botCount}, tcp={tcpHost}:{tcpPort}, scenario={scenario}, disconnect-loop={disconnectLoop}, disconnect-after-login-seconds={disconnectAfterLoginSeconds}, run-seconds={runSeconds}",
            options.BotCount, options.TcpHost, options.TcpPort, options.Scenario, options.EnableDisconnectLoop, options.DisconnectAfterLoginSeconds, options.RunSeconds);

        var cts = new CancellationTokenSource();
        var botTasks = new List<Task>(options.BotCount);
        for (int k = 0; k < options.BotCount; k++)
        {
            string botName = $"{options.BotNamePrefix}_{k + 1}";
            var client = new BotClient(botName, options);
            botTasks.Add(client.EntryAsync(cts.Token));
            if (options.ConnectStaggerMilliseconds > 0)
            {
                await Task.Delay(options.ConnectStaggerMilliseconds, cts.Token);
            }
        }

        if (options.RunSeconds > 0)
        {
            await Task.Delay(TimeSpan.FromSeconds(options.RunSeconds), cts.Token);
            cts.Cancel();
            await Task.WhenAll(botTasks);
            return;
        }

        Console.WriteLine("机器人运行中，按回车结束...");
        Console.ReadLine();
        cts.Cancel();
        await Task.WhenAll(botTasks);
    }

    private static void WarmUpProtoSerializer()
    {
        // 预热常用消息，避免并发首次序列化时的元数据锁竞争。
        _ = ProtoBufSerializerHelper.Serialize(new ReqLogin());
        _ = ProtoBufSerializerHelper.Serialize(new ReqPlayerList());
        _ = ProtoBufSerializerHelper.Serialize(new ReqPlayerCreate());
        _ = ProtoBufSerializerHelper.Serialize(new ReqPlayerLogin());
        _ = ProtoBufSerializerHelper.Serialize(new RespLogin());
        _ = ProtoBufSerializerHelper.Serialize(new RespPlayerList());
        _ = ProtoBufSerializerHelper.Serialize(new RespPlayerCreate());
        _ = ProtoBufSerializerHelper.Serialize(new RespPlayerLogin());
        _ = ProtoBufSerializerHelper.Serialize(new ReqFriendByAdd());
        _ = ProtoBufSerializerHelper.Serialize(new RespFriendByAdd());
        _ = ProtoBufSerializerHelper.Serialize(new ReqDeleteFriend());
        _ = ProtoBufSerializerHelper.Serialize(new RespDeleteFriend());
        _ = ProtoBufSerializerHelper.Serialize(new ReqFriendList());
        _ = ProtoBufSerializerHelper.Serialize(new RespFriendList());
    }
}
