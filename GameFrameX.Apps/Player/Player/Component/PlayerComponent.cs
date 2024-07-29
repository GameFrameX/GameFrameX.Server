using System.Threading.Tasks;
using GameFrameX.Apps.Player.Player.Entity;
using GameFrameX.Core.Abstractions;
using GameFrameX.Monitor.Player;
using Random = GameFrameX.Utility.Random;

namespace GameFrameX.Apps.Player.Player.Component
{
    [ComponentType(ActorType.Player)]
    public sealed class PlayerComponent : StateComponent<PlayerState>
    {
        public async Task<List<PlayerState>> GetPlayerList(ReqPlayerList reqPlayerList)
        {
            MetricsPlayerRegister.GetPlayerListCounterOptions.Inc();
            return await GameDb.FindListAsync<PlayerState>(m => m.AccountId == reqPlayerList.Id);
        }

        public async Task<PlayerState> OnPlayerCreate(ReqPlayerCreate reqPlayerCreate)
        {
            PlayerState playerState = new PlayerState
                                      {
                                          Id        = ActorIdGenerator.GetActorId(ActorType.Player),
                                          AccountId = reqPlayerCreate.Id,
                                          Name      = reqPlayerCreate.Name,
                                          Level     = 0,
                                          State     = 0,
                                          Avatar    = (uint)Random.GetRandom(1, 50),
                                      };
            MetricsPlayerRegister.CreateCounterOptions.Inc();
            await GameDb.SaveOneAsync<PlayerState>(playerState);
            return playerState;
        }

        public async Task<PlayerState> OnLogin(ReqPlayerLogin reqLogin)
        {
            MetricsPlayerRegister.LoginCounterOptions.Inc();
            return await GameDb.FindAsync<PlayerState>(m => m.Id == reqLogin.Id);
        }
    }
}