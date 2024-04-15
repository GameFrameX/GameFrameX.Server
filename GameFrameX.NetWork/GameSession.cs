using TouchSocket.Sockets;

namespace GameFrameX.NetWork;

public class GameSession : IGameSession
{
    public string SessionId { get; }
    public string RemoteEndPoint { get; }
    public ISocketClient SocketClient { get; }

    public async void Send(byte[] bytes)
    {
        await SendAsync(bytes);
    }

    public Task SendAsync(byte[] bytes)
    {
        return SocketClient.SendAsync(bytes);
    }

    public GameSession(string remoteEndPoint, ISocketClient socketClient)
    {
        RemoteEndPoint = remoteEndPoint;
        SocketClient = socketClient;
    }

    public GameSession()
    {
        SessionId = Guid.NewGuid().ToString("N");
    }
}