﻿using GameFrameX.Core.Net.BaseHandler;
using GameFrameX.Hotfix.Player.Login.Agent;
using GameFrameX.NetWork.Messages;

namespace GameFrameX.Hotfix.Player.Login.Handler
{
    [MessageMapping(typeof(ReqPlayerLogin))]
    internal class ReqPlayerLoginHandler : GlobalComponentHandler<PlayerComponentAgent>
    {
        protected override async Task ActionAsync()
        {
            await Comp.OnLogin(NetWorkChannel, Message as ReqPlayerLogin);
        }
    }
}