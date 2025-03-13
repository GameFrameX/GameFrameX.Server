using GameFrameX.Foundation.Logger;

namespace GameFrameX.Core.Actors.Impl;

/// <summary>
/// 无返回包装器
/// </summary>
public class ActionWrapper : WorkWrapper
{
    /// <summary>
    /// 构建无返回值的包装器
    /// </summary>
    /// <param name="work">工作函数</param>
    public ActionWrapper(Action work)
    {
        Work = work;
        Tcs = new TaskCompletionSource<bool>();
    }

    /// <summary>
    /// 工作对象
    /// </summary>
    public Action Work { get; }

    /// <summary>
    /// 工作等待
    /// </summary>
    public TaskCompletionSource<bool> Tcs { get; }

    /// <summary>
    /// 执行
    /// </summary>
    /// <returns></returns>
    public override Task DoTask()
    {
        try
        {
            SetContext();
            Work();
        }
        catch (Exception e)
        {
            LogHelper.Error(e.ToString());
        }
        finally
        {
            ResetContext();
            Tcs.TrySetResult(true);
        }

        return Task.CompletedTask;
    }

    /// <summary>
    /// 获取调用链
    /// </summary>
    /// <returns></returns>
    public override string GetTrace()
    {
        return Work.Target + "|" + Work.Method.Name;
    }

    /// <summary>
    /// 强制设置结果
    /// </summary>
    public override void ForceSetResult()
    {
        ResetContext();
        Tcs.TrySetResult(false);
    }
}