using GameFrameX.Core.BaseHandler;
using GameFrameX.NetWork.Messages;

namespace GameFrameX.Hotfix.Logic.Server.Heart;

/// <summary>
/// 心跳消息处理器
/// </summary>
[MessageMapping(typeof(ReqHeartBeat))]
internal sealed class ReqHeartBeatHandler : GlobalComponentHandler<HeartBeatComponentAgent>
{
    readonly NotifyHeartBeat resp = new NotifyHeartBeat
    {
        Timestamp = TimeHelper.UnixTimeMilliseconds()
    };

    protected override async Task ActionAsync()
    {
        ReqHeartBeat req = this.Message as ReqHeartBeat;
        // LogHelper.Info("收到心跳请求:" + req.Timestamp);
        ComponentAgent.OnUpdateHeartBeatTime(req);
        NetWorkChannel.UpdateReceiveMessageTime();
        resp.Timestamp = TimeHelper.UnixTimeMilliseconds();
        resp.UniqueId = req.UniqueId;
        await NetWorkChannel.WriteAsync(resp);
    }
}