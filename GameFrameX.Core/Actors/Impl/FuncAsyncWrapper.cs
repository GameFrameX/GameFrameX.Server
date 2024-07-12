using GameFrameX.Log;

namespace GameFrameX.Core.Actors.Impl;

/// <summary>
/// 异步有返回值的泛型包装器
/// </summary>
/// <typeparam name="T"></typeparam>
public class FuncAsyncWrapper<T> : WorkWrapper
{
    /// <summary>
    /// 工作对象
    /// </summary>
    public Func<Task<T>> Work { private set; get; }

    /// <summary>
    /// 工作等待
    /// </summary>
    public TaskCompletionSource<T> Tcs { private set; get; }

    /// <summary>
    /// 构建有返回值的泛型包装器
    /// </summary>
    /// <param name="work"></param>
    public FuncAsyncWrapper(Func<Task<T>> work)
    {
        Work = work;
        Tcs  = new TaskCompletionSource<T>();
    }

    /// <summary>
    /// 执行
    /// </summary>
    /// <returns></returns>
    public override async Task DoTask()
    {
        T ret = default;
        try
        {
            SetContext();
            ret = await Work();
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