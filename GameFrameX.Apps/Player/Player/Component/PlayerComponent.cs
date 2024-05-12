using GameFrameX.Apps.Player.Player.Entity;
using GameFrameX.DBServer;
using GameFrameX.Setting;
using GameFrameX.Utility;

namespace GameFrameX.Apps.Player.Player.Component
{
    [ComponentType(ActorType.Player)]
    public sealed class PlayerComponent : StateComponent<PlayerState>
    {
        public async Task<List<PlayerState>?> GetPlayerList(ReqPlayerList reqPlayerList)
        {
            return await GameDb.FindListAsync<PlayerState>(m => m.AccountId == reqPlayerList.Id);
        }

        public async Task<PlayerState> OnPlayerCreate(ReqPlayerCreate reqPlayerCreate)
        {
            PlayerState playerState = new PlayerState
            {
                Id = IdGenerator.GetActorID(ActorType.Player),
                AccountId = reqPlayerCreate.Id,
                Name = reqPlayerCreate.Name,
                Level = 0,
                State = 0
            };
            await GameDb.SaveOneAsync<PlayerState>(playerState);
            return playerState;
        }

        public async Task<PlayerState?> OnLogin(ReqPlayerLogin reqLogin)
        {
            return await GameDb.FindAsync<PlayerState>(m => m.Id == reqLogin.Id);
        }
    }
}