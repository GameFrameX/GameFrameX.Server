using GameFrameX.NetWork.Messages;
using ErrorEventArgs = GameFrameX.SuperSocket.ClientEngine.ErrorEventArgs;

namespace GameFrameX.Bot;

public struct BotTcpClientEvent
{
    public Action OnConnectedCallback = null;
    public Action OnClosedCallback = null;
    public Action<ErrorEventArgs> OnErrorCallback = null;
    public Action<MessageObject> OnReceiveMsgCallback = null;

    public BotTcpClientEvent()
    {
    }
}