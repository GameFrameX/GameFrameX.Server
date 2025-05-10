﻿using GameFrameX.NetWork.Abstractions;
using GameFrameX.SuperSocket.Server.Abstractions.Session;
using GameFrameX.Utility.Setting;

namespace GameFrameX.NetWork;

/// <summary>
/// 默认网络通道
/// </summary>
public class DefaultNetWorkChannel : BaseNetWorkChannel
{
    /// <summary>
    /// 初始化
    /// </summary>
    /// <param name="session"></param>
    /// <param name="setting"></param>
    /// <param name="rpcSession"></param>
    /// <param name="isWebSocket"></param>
    public DefaultNetWorkChannel(IGameAppSession session, AppSetting setting, IRpcSession rpcSession = null, bool isWebSocket = false) : base(session, setting, rpcSession, isWebSocket)
    {
    }
}