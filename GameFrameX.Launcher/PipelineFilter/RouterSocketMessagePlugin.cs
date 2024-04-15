using GameFrameX.Extension;
using GameFrameX.NetWork.Messages;
using TouchSocket.Sockets;

namespace GameFrameX.Launcher.PipelineFilter;

public sealed partial class RouterSocketMessagePlugin : BaseRouterSocketMessagePlugin, ITcpReceivedPlugin
{
    static readonly MessageRouterEncoderHandler messageEncoderHandler = new MessageRouterEncoderHandler();
    private readonly MessageRouterDecoderHandler messageRouterDecoderHandler = new MessageRouterDecoderHandler();

    public async Task OnTcpReceived(ITcpClientBase socketClient, ReceivedDataEventArgs e)
    {
        //从客户端收到信息
        if (e.ByteBlock.Len < 20)
        {
            LogHelper.Debug($"---收到消息异常：{e.ByteBlock.Buffer.ToArrayString()}");
            return;
        }

        var buffer = new byte[e.ByteBlock.Len];
        e.ByteBlock.Read(buffer);
        var result = await MessageHandler(socketClient.IP, buffer);

        if (!result)
        {
            var outBuffer = new byte[e.ByteBlock.Len];
            _ = e.ByteBlock.Read(outBuffer);
            await socketClient.SendAsync(outBuffer);
        }

        await e.InvokeNext();
    }
}