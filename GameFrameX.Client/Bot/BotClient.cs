using GameFrameX.Log;
using GameFrameX.NetWork.Messages;
using GameFrameX.Proto.Proto;
using ErrorEventArgs = GameFrameX.SuperSocket.ClientEngine.ErrorEventArgs;

namespace GameFrameX.Bot;

public class BotClient
{
    private readonly BotTcpClient m_TcpClient;
    private readonly BotHttpClient m_HttpClient;
    private readonly string m_BotName;
    private readonly BotTcpClientEvent m_BotTcpClientEvent;
    private const string m_LoginUrl = "http://127.0.0.1:29200/game/api/";
    private long m_PlayerId;

    public BotClient(string botName)
    {
        m_BotName = botName;
        m_BotTcpClientEvent.OnConnectedCallback = ClientConnectedCallback;
        m_BotTcpClientEvent.OnClosedCallback = ClientClosedCallback;
        m_BotTcpClientEvent.OnErrorCallback = ClientErrorCallback;
        m_BotTcpClientEvent.OnReceiveMsgCallback = ClientReceiveCallback;
        m_TcpClient = new BotTcpClient(m_BotTcpClientEvent);
        m_HttpClient = new BotHttpClient();
    }

    public async Task EntryAsync()
    {
        try
        {
            await m_TcpClient.EntryAsync();
        }
        catch (Exception e)
        {
            LogHelper.Info($"EntryAsync Error: {e.Message}| Thread ID:{Thread.CurrentThread.ManagedThreadId} ");
        }
    }

    private void OnReceiveMsg(MessageObject messageObject)
    {
        switch (messageObject)
        {
            case RespPlayerLogin msg:
                OnPlayerLoginSuccess(msg);
                break;
        }
    }

    #region 连接状态回调

    private void ClientConnectedCallback()
    {
        SendLoginMessage();
    }

    private void ClientClosedCallback()
    {
    }

    private void ClientErrorCallback(ErrorEventArgs error)
    {
    }

    private void ClientReceiveCallback(MessageObject outerMsg)
    {
        OnReceiveMsg(outerMsg);
    }

    #endregion

    #region 消息发送

    private async void SendLoginMessage()
    {
        var req = new ReqLogin
        {
            SdkType = 0,
            SdkToken = "",
            UserName = m_BotName,
            Password = "12312",
            Device = "",
            Platform = "Windows"
        };

        string loginUrl = $"{m_LoginUrl}{nameof(ReqLogin)}";
        var respLogin = await m_HttpClient.Post<RespLogin>(loginUrl, req);
        if (respLogin.ErrorCode != 0)
        {
            LogHelper.Error("登录验证失败，错误信息:" + respLogin.ErrorCode);
            return;
        }

        m_PlayerId = respLogin.Id;
        LogHelper.Info($"机器人-{m_BotName}账号验证成功,id:{respLogin.Id}");
        var reqPlayerLogin = new ReqPlayerLogin();
        reqPlayerLogin.Id = respLogin.Id;
        m_TcpClient.SendToServer(reqPlayerLogin);
    }

    #endregion


    #region 消息接收

    private void OnPlayerLoginSuccess(RespPlayerLogin msg)
    {
        LogHelper.Info($"机器人-{m_BotName}登录成功,id:{m_PlayerId}");
    }

    #endregion
}