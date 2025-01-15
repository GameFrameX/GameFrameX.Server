namespace GameFrameX.Utility.Extensions;

/// <summary>
/// 线程私有random对象
/// </summary>
public static class ThreadLocalRandom
{
    private static int _seed = Environment.TickCount;

    private static readonly ThreadLocal<System.Random> Rng = new(() => new System.Random(Interlocked.Increment(ref _seed)));

    /// <summary>
    /// The current random number seed available to this thread
    /// </summary>
    public static System.Random Current
    {
        get { return Rng.Value; }
    }
}