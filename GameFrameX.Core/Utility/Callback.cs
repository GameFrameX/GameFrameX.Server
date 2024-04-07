using GameFrameX.Core.Hotfix.Agent;
using GameFrameX.Utility;

namespace GameFrameX.Core.Utility
{
    public interface IAgentCallback
    {
        Task<bool> Invoke(IComponentAgent agent, Param param = null);

        Type CompAgentType();
    }

    public abstract class AgentCallback<TAgent> : IAgentCallback where TAgent : IComponentAgent
    {
        public Type CompAgentType()
        {
            return typeof(TAgent);
        }

        public Task<bool> Invoke(IComponentAgent agent, Param param = null)
        {
            return OnCall((TAgent) agent, param);
        }

        protected abstract Task<bool> OnCall(TAgent comp, Param param);
    }
}