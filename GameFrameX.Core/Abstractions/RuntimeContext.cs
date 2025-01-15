using System.Runtime.CompilerServices;

namespace GameFrameX.Core.Abstractions;

/// <summary>
/// 运行时上下文
/// </summary>
public static class RuntimeContext
{
    /// <summary>
    /// 当前链上下文
    /// </summary>
    private static readonly AsyncLocal<long> ChainContext = new();

    /// <summary>
    /// 当前Actor上下文
    /// </summary>
    private static readonly AsyncLocal<long> ActorContext = new();

    /// <summary>
    /// 当前链ID
    /// </summary>
    public static long CurrentChainId
    {
        get { return ChainContext.Value; }
    }

    /// <summary>
    /// 当前ActorID
    /// </summary>
    public static long CurrentActor
    {
        get { return ActorContext.Value; }
    }


    /// <summary>
    /// 设置上下文
    /// </summary>
    /// <param name="callChainId"></param>
    /// <param name="actorId"></param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void SetContext(long callChainId, long actorId)
    {
        ChainContext.Value = callChainId;
        ActorContext.Value = actorId;
    }

    /// <summary>
    /// 重置上下文
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void ResetContext()
    {
        ChainContext.Value = 0;
        ActorContext.Value = 0;
    }
}