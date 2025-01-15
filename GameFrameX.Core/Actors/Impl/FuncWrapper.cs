using GameFrameX.Utility.Log;

namespace GameFrameX.Core.Actors.Impl;

/// <summary>
/// 有返回值的泛型包装器
/// </summary>
/// <typeparam name="T"></typeparam>
public class FuncWrapper<T> : WorkWrapper
{
    /// <summary>
    /// 构建有返回值的泛型包装器
    /// </summary>
    /// <param name="work">工作单元</param>
    public FuncWrapper(Func<T> work)
    {
        Work = work;
        Tcs = new TaskCompletionSource<T>();
    }

    /// <summary>
    /// 工作对象
    /// </summary>
    public Func<T> Work { get; }

    /// <summary>
    /// 工作等待
    /// </summary>
    public TaskCompletionSource<T> Tcs { get; }

    /// <summary>
    /// 执行
    /// </summary>
    /// <returns></returns>
    public override Task DoTask()
    {
        T ret = default;
        try
        {
            SetContext();
            ret = Work();
        }
        catch (Exception e)
        {
            LogHelper.Error(e.ToString());
        }
        finally
        {
            ResetContext();
            Tcs.TrySetResult(ret);
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
        Tcs.TrySetResult(default);
    }
}