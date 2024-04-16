/*using GameFrameX.NetWork.Messages;
using TouchSocket.Http.WebSockets;

namespace GameFrameX.Launcher.PipelineFilter;

public sealed class RouterWebSocketMessagePlugin : BaseRouterSocketMessagePlugin, IWebSocketReceivedPlugin<IWebSocket>
{
    static readonly MessageRouterEncoderHandler messageEncoderHandler = new MessageRouterEncoderHandler();
    private readonly MessageRouterDecoderHandler messageRouterDecoderHandler = new MessageRouterDecoderHandler();

    public async Task OnWebSocketReceived(IWebSocket socketClient, WSDataFrameEventArgs e)
    {
        switch (e.DataFrame.Opcode)
        {
            case WSDataType.Cont:
                LogHelper.Info($"收到中间数据，长度为：{e.DataFrame.PayloadLength}");
                return;

            case WSDataType.Text:
                LogHelper.Info(e.DataFrame.ToText());

                if (!socketClient.Client.IsClient)
                {
                    socketClient.Send("我已收到");
                }

                return;

            case WSDataType.Binary:
                if (e.DataFrame.FIN)
                {
                    var result = await MessageHandler(socketClient.Client.IP, e.DataFrame.PayloadData.Buffer);
                    if (result)
                    {
                        LogHelper.Info($"收到二进制数据，长度为：{e.DataFrame.PayloadLength}");
                    }
                    else
                    {
                        var outBuffer = new byte[e.DataFrame.PayloadLength];
                        e.DataFrame.PayloadData.Pos = 0;
                        _ = e.DataFrame.PayloadData.Read(outBuffer);

                        await socketClient.SendAsync(outBuffer);
                    }
                }
                else
                {
                    LogHelper.Info($"收到未结束的二进制数据，长度为：{e.DataFrame.PayloadLength}");
                }

                return;

            case WSDataType.Close:
            {
                LogHelper.Info("远程请求断开");
                socketClient.Close("断开");
            }
                return;

            case WSDataType.Ping:
                break;

            case WSDataType.Pong:
                break;

            default:
                break;
        }

        await e.InvokeNext();
    }
}*/