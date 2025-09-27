namespace GameFrameX.Hotfix.StartUp;

internal partial class AppStartUpHotfixGame
{
    private readonly NotifyHeartBeat _notifyHeartBeat = new NotifyHeartBeat
    {
        Timestamp = TimeHelper.UnixTimeMilliseconds(),
    };

    /// <summary>
    /// 回复心跳消息
    /// </summary>
    /// <param name="netWorkChannel"></param>
    /// <param name="messageObject"></param>
    private async void ReplyHeartBeat(INetWorkChannel netWorkChannel, MessageObject messageObject)
    {
        if (messageObject is ReqHeartBeat req)
        {
            // LogHelper.Info("收到心跳请求:" + req.Timestamp);
            netWorkChannel.UpdateReceiveMessageTime();
            _notifyHeartBeat.Timestamp = TimeHelper.UnixTimeMilliseconds();
            _notifyHeartBeat.UniqueId = req.UniqueId;
            await netWorkChannel.WriteAsync(_notifyHeartBeat);
        }
    }
}