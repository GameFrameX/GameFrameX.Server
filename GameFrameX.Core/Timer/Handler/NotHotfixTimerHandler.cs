using GameFrameX.Utility;
using Quartz;

namespace GameFrameX.Core.Timer.Handler
{
    /// <summary>
    /// 非热更程序集的计时器处理器，不需要热更时间更新
    /// </summary>
    public abstract class NotHotfixTimerHandler : IJob
    {
        /// <summary>
        /// 内部计时器处理器调用函数
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public Task Execute(IJobExecutionContext context)
        {
            var param = context.JobDetail.JobDataMap.Get(QuartzTimer.ParamKey) as Param;
            return HandleTimer(param);
        }

        /// <summary>
        /// 计时器处理函数
        /// </summary>
        /// <param name="param">参数</param>
        /// <returns></returns>
        protected abstract Task HandleTimer(Param param);
    }
}