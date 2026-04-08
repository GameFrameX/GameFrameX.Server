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

using GameFrameX.NetWork.Messages;
using GameFrameX.Proto.Proto;
using GameFrameX.Foundation.Logger;
using ErrorEventArgs = GameFrameX.SuperSocket.ClientEngine.ErrorEventArgs;

namespace GameFrameX.Client.Bot;

/// <summary>
/// 机器人客户端类,用于模拟玩家行为进行测试
/// </summary>
public sealed class BotClient
{
    private readonly BotTcpClient m_TcpClient;
    private readonly string m_BotName;
    private readonly BotTcpClientEvent m_BotTcpClientEvent;
    private readonly BotRunOptions _options;
    private int _disconnectScheduled;
    private long _accountId;

    /// <summary>
    /// 初始化机器人客户端
    /// </summary>
    /// <param name="botName">机器人名称</param>
    public BotClient(string botName, BotRunOptions options)
    {
        m_BotName = botName;
        _options = options;
        m_BotTcpClientEvent.OnConnectedCallback += ClientConnectedCallback;
        m_BotTcpClientEvent.OnClosedCallback += ClientClosedCallback;
        m_BotTcpClientEvent.OnErrorCallback += ClientErrorCallback;
        m_BotTcpClientEvent.OnReceiveMsgCallback += ClientReceiveCallback;
        m_TcpClient = new BotTcpClient(m_BotTcpClientEvent, options.TcpHost, options.TcpPort);
    }

    /// <summary>
    /// 启动机器人客户端
    /// </summary>
    /// <returns>异步任务</returns>
    public async Task EntryAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            await m_TcpClient.EntryAsync(cancellationToken);
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
            case RespLogin msg:
                OnAccountLoginSuccess(msg);
                break;
            case RespPlayerList msg:
                OnPlayerListSuccess(msg);
                break;
            case RespPlayerCreate msg:
                OnPlayerCreateSuccess(msg);
                break;
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
        SendAccountLoginMessage();
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
    private void SendAccountLoginMessage()
    {
        try
        {
            var reqLogin = new ReqLogin
            {
                UserName = m_BotName,
                Password = "12312",
                Platform = "LoginPlatform.Custom",
            };
            m_TcpClient.SendToServer(reqLogin);
        }
        catch (Exception e)
        {
            LogHelper.Error($"SendAccountLoginMessage Error: {e.Message}| Thread ID:{Thread.CurrentThread.ManagedThreadId} ");
        }
    }

    #endregion


    #region 消息接收

    private void OnAccountLoginSuccess(RespLogin msg)
    {
        if (msg.ErrorCode != 0)
        {
            LogHelper.Error($"机器人-{m_BotName}账号登录失败，错误码:{msg.ErrorCode}");
            return;
        }

        _accountId = msg.Id;
        LogHelper.Info($"机器人-{m_BotName}账号验证成功,id:{msg.Id}");
        m_TcpClient.SendToServer(new ReqPlayerList { Id = _accountId });
    }

    private void OnPlayerListSuccess(RespPlayerList msg)
    {
        if (msg.ErrorCode != 0)
        {
            LogHelper.Error($"机器人-{m_BotName}请求角色列表失败，错误码:{msg.ErrorCode}");
            return;
        }

        if (msg.PlayerList.Count <= 0)
        {
            LogHelper.Info($"机器人-{m_BotName}角色列表为空，开始创建角色。");
            m_TcpClient.SendToServer(new ReqPlayerCreate
            {
                Id = _accountId,
                Name = m_BotName,
            });
            return;
        }

        var player = msg.PlayerList[0];
        LogHelper.Info($"角色列表 Id:{player.Id}-昵称:{player.Name}-等级:{player.Level}-角色状态:{player.State}");
        m_TcpClient.SendToServer(new ReqPlayerLogin { Id = player.Id });
    }

    private void OnPlayerCreateSuccess(RespPlayerCreate msg)
    {
        if (msg.ErrorCode != 0)
        {
            LogHelper.Error($"机器人-{m_BotName}创建角色失败，错误码:{msg.ErrorCode}");
            return;
        }

        var player = msg.PlayerInfo;
        LogHelper.Info($"创建角色 Id:{player.Id}-昵称:{player.Name}-等级:{player.Level}-角色状态:{player.State}");
        m_TcpClient.SendToServer(new ReqPlayerLogin { Id = player.Id });
    }

    /// <summary>
    /// 处理玩家登录成功的响应
    /// </summary>
    /// <param name="msg">登录成功的响应消息</param>
    private void OnPlayerLoginSuccess(RespPlayerLogin msg)
    {
        LogHelper.Info($"机器人-{m_BotName}登录成功,id:{msg.PlayerInfo.Id}");
        ScheduleDisconnectIfNeeded();
    }

    private void ScheduleDisconnectIfNeeded()
    {
        if (!_options.EnableDisconnectLoop || _options.DisconnectAfterLoginSeconds <= 0)
        {
            return;
        }

        if (Interlocked.CompareExchange(ref _disconnectScheduled, 1, 0) != 0)
        {
            return;
        }

        _ = Task.Run(async () =>
        {
            try
            {
                await Task.Delay(TimeSpan.FromSeconds(_options.DisconnectAfterLoginSeconds));
                LogHelper.Info($"机器人-{m_BotName}主动断开连接，模拟离线。");
                m_TcpClient.Disconnect();
            }
            finally
            {
                Interlocked.Exchange(ref _disconnectScheduled, 0);
            }
        });
    }

    #endregion
}
