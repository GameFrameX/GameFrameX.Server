/*using GameFrameX.Launcher.Common.Session;
using GameFrameX.NetWork;
using GameFrameX.NetWork.TCPSocket;
using GameFrameX.Setting;

namespace GameFrameX.Launcher.Common.Net
{
    public class AppTcpConnectionHandler : TcpConnectionHandler
    {
        protected override void OnDisconnection(BaseNetChannel channel)
        {
            base.OnDisconnection(channel);
            var sessionId = channel.GetData<long>(GlobalConst.SessionIdKey);
            if (sessionId > 0)
            {
                SessionManager.Remove(sessionId);
            }
        }

        // public AppTcpConnectionHandler(Func<int, IMessageHandler> messageHandler, Func<int, Type> typeGetter, Func<Type, int> idGetter) : base(messageHandler, typeGetter, idGetter)
        // {
        // }
    }
}*/