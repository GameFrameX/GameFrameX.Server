using GameFrameX.NetWork.Messages;
using GameFrameX.Proto.Proto;
using GameFrameX.Foundation.Logger;
using GameFrameX.Foundation.Extensions;
using ErrorEventArgs = GameFrameX.SuperSocket.ClientEngine.ErrorEventArgs;

namespace GameFrameX.Client.Bot;

/// <summary>
/// 机器人客户端类,用于模拟玩家行为进行测试
/// </summary>
public sealed class BotClient
{
    private readonly BotTcpClient m_TcpClient;
    private readonly BotHttpClient m_HttpClient;
    private readonly string m_BotName;
    private readonly BotTcpClientEvent m_BotTcpClientEvent;
    private const string m_LoginUrl = "http://127.0.0.1:28080/game/api/";

    /// <summary>
    /// 初始化机器人客户端
    /// </summary>
    /// <param name="botName">机器人名称</param>
    public BotClient(string botName)
    {
        m_BotName = botName;
        m_BotTcpClientEvent.OnConnectedCallback += ClientConnectedCallback;
        m_BotTcpClientEvent.OnClosedCallback += ClientClosedCallback;
        m_BotTcpClientEvent.OnErrorCallback += ClientErrorCallback;
        m_BotTcpClientEvent.OnReceiveMsgCallback += ClientReceiveCallback;
        m_TcpClient = new BotTcpClient(m_BotTcpClientEvent);
        m_HttpClient = new BotHttpClient();
    }

    /// <summary>
    /// 启动机器人客户端
    /// </summary>
    /// <returns>异步任务</returns>
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

    /// <summary>
    /// 处理接收到的消息
    /// </summary>
    /// <param name="messageObject">接收到的消息对象</param>
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

    /// <summary>
    /// 客户端连接成功的回调
    /// </summary>
    private void ClientConnectedCallback()
    {
        SendLoginMessage();
    }

    /// <summary>
    /// 客户端连接关闭的回调
    /// </summary>
    private void ClientClosedCallback()
    {
    }

    /// <summary>
    /// 客户端发生错误的回调
    /// </summary>
    /// <param name="error">错误信息</param>
    private void ClientErrorCallback(ErrorEventArgs error)
    {
    }

    /// <summary>
    /// 客户端接收消息的回调
    /// </summary>
    /// <param name="outerMsg">接收到的消息</param>
    private void ClientReceiveCallback(MessageObject outerMsg)
    {
        OnReceiveMsg(outerMsg);
    }

    #endregion

    #region 消息发送

    /// <summary>
    /// 发送登录消息并处理登录流程
    /// </summary>
    private async void SendLoginMessage()
    {
        try
        {
            //请求登录验证
            var reqLogin = new ReqLogin
            {
                UserName = m_BotName,
                Password = "12312",
                Platform = "LoginPlatform.Custom",
            };

            string respLoginUrl = $"{m_LoginUrl}{nameof(ReqLogin).ConvertToSnakeCase()}";
            var respLogin = await m_HttpClient.Post<RespLogin>(respLoginUrl, reqLogin);
            if (respLogin.ErrorCode != 0)
            {
                LogHelper.Error("请求登录验证，错误信息:" + respLogin.ErrorCode);
                return;
            }

            LogHelper.Info($"机器人-{m_BotName}账号验证成功,id:{respLogin.Id}");

            //请求角色列表
            var reqPlayerList = new ReqPlayerList();
            reqPlayerList.Id = respLogin.Id;
            string reqPlayerListUrl = $"{m_LoginUrl}{nameof(ReqPlayerList).ConvertToSnakeCase()}";
            var respPlayerList = await m_HttpClient.Post<RespPlayerList>(reqPlayerListUrl, reqPlayerList);

            if (respPlayerList.ErrorCode != 0)
            {
                LogHelper.Error("请求角色列表，错误信息:" + respPlayerList.ErrorCode);
                return;
            }

            PlayerInfo player;
            if (respPlayerList.PlayerList.Count == 0)
            {
                LogHelper.Info("角色列表为空");

                //请求创建角色
                var reqCreatePlayer = new ReqPlayerCreate();
                reqCreatePlayer.Id = respLogin.Id;

                string reqCreatePlayerUrl = $"{m_LoginUrl}{nameof(ReqPlayerCreate).ConvertToSnakeCase()}";
                var respPlayerCreator = await m_HttpClient.Post<RespPlayerCreate>(reqCreatePlayerUrl,
                                                                                  reqCreatePlayer);
                if (respPlayerCreator.ErrorCode != 0)
                {
                    LogHelper.Error("请求创建角色，错误信息:" + respPlayerCreator.ErrorCode);
                    return;
                }

                player = respPlayerCreator.PlayerInfo;
                LogHelper.Info($"创建角色 Id:{player.Id}-昵称:{player.Name}-等级:{player.Level}-角色状态:{player.State}");
            }
            else
            {
                player = respPlayerList.PlayerList[0];
                LogHelper.Info($"角色列表 Id:{player.Id}-昵称:{player.Name}-等级:{player.Level}-角色状态:{player.State}");
            }

            var reqPlayerLogin = new ReqPlayerLogin();
            reqPlayerLogin.Id = player.Id;
            // reqPlayerLogin.Name = m_BotName;
            m_TcpClient.SendToServer(reqPlayerLogin);
        }
        catch (Exception e)
        {
            LogHelper.Error($"SendLoginMessage Error: {e.Message}| Thread ID:{Thread.CurrentThread.ManagedThreadId} ");
        }
    }

    #endregion


    #region 消息接收

    /// <summary>
    /// 处理玩家登录成功的响应
    /// </summary>
    /// <param name="msg">登录成功的响应消息</param>
    private void OnPlayerLoginSuccess(RespPlayerLogin msg)
    {
        LogHelper.Info($"机器人-{m_BotName}登录成功,id:{msg.PlayerInfo.Id}");
    }

    #endregion
}