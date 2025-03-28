﻿using GameFrameX.Apps.Common.Event;
using GameFrameX.Apps.Player.Pet.Component;
using GameFrameX.Apps.Player.Pet.Entity;
using GameFrameX.Core.Abstractions.Events;
using GameFrameX.Hotfix.Logic.Server.Server;

namespace GameFrameX.Hotfix.Logic.Role.Pet;

public class PetComponentAgent : StateComponentAgent<PetComponent, PetState>
{
    private async Task OnGotNewPet(OneParam<int> param)
    {
        var serverComp = await ActorManager.GetComponentAgent<ServerComponentAgent>();
        //var level = await serverComp.SendAsync(() => serverComp.GetWorldLevel()); //手动入队的写法
        var level = await serverComp.GetWorldLevel();
        LogHelper.Debug($"PetCompAgent.OnGotNewPet监听到了获得宠物的事件,宠物ID:{param.Value}当前世界等级:{level}");
    }

    [Event(EventId.GotNewPet)]
    private class EL : EventListener<PetComponentAgent>
    {
        protected override async Task HandleEvent(PetComponentAgent agent, GameEventArgs gameEventArgs)
        {
            switch ((EventId)gameEventArgs.EventId)
            {
                case EventId.GotNewPet:
                    await agent.OnGotNewPet((OneParam<int>)gameEventArgs.Data);
                    break;
            }
        }
    }
}