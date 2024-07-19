using System.Threading.Tasks;
using GameFrameX.Apps.Account.Login.Entity;
using GameFrameX.Core.Abstractions;
using GameFrameX.Monitor.Account;

namespace GameFrameX.Apps.Account.Login.Component
{
    [ComponentType(ActorType.Account)]
    public sealed class LoginComponent : StateComponent<LoginState>
    {
        public async Task<LoginState> OnLogin(ReqLogin reqLogin)
        {
            MetricsAccountRegister.LoginCounterOptions.Inc();
            return await GameDb.FindAsync<LoginState>(m => m.UserName == reqLogin.UserName && m.Password == reqLogin.Password);
        }

        public async Task<LoginState> Register(long accountId, ReqLogin reqLogin)
        {
            MetricsAccountRegister.RegisterCounterOptions.Inc();
            LoginState loginState = new LoginState() { Id = accountId, UserName = reqLogin.UserName, Password = reqLogin.Password };
            await GameDb.SaveOneAsync<LoginState>(loginState);
            return loginState;
        }
    }
}