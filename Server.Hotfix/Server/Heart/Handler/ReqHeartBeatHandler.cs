using Server.Core.Net.BaseHandler;
using Server.Hotfix.Server.Heart.Agent;
using Server.NetWork.Messages;

namespace Server.Hotfix.Server.Heart.Handler;

/// <summary>
/// 心跳消息处理器
/// </summary>
[MessageMapping(typeof(ReqHeartBeat))]
internal sealed class ReqHeartBeatHandler : GlobalComponentHandler<HeartBeatComponentAgent>
{
    readonly RespHeartBeat resp = new RespHeartBeat
    {
        Timestamp = TimeHelper.UnixTimeSeconds()
    };

    protected override async Task ActionAsync()
    {
        ReqHeartBeat req = this.Message as ReqHeartBeat;
        LogHelper.Info("收到心跳请求:" + req.Timestamp);
        Channel.UpdateReceiveMessageTime();
        resp.Timestamp = TimeHelper.UnixTimeSeconds();
        await Channel.WriteAsync(resp);
    }
}