using GameFrameX.Hotfix.Server.Heart.Agent;
using GameFrameX.Core.Net.BaseHandler;
using GameFrameX.NetWork.Messages;
using GameFrameX.Proto.BuiltIn;
using GameFrameX.Utility;

namespace GameFrameX.Hotfix.Server.Heart.Handler;

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
        // LogHelper.Info("收到心跳请求:" + req.Timestamp);
        Comp.OnUpdateHeartBeatTime(req);
        NetWorkChannel.UpdateReceiveMessageTime();
        resp.Timestamp = TimeHelper.UnixTimeSeconds();
        resp.UniqueId = req.UniqueId;
        await NetWorkChannel.WriteAsync(resp);
    }
}