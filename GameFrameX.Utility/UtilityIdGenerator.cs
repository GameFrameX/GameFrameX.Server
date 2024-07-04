namespace GameFrameX.Utility;

/// <summary>
/// ID生成器，生成唯一ID
/// </summary>
public static class UtilityIdGenerator
{
    private static readonly DateTime UtcTimeStart = new(2000, 1, 1, 0, 0, 0, DateTimeKind.Utc);

    // 共享计数器
    private static long _longCounter = (long)(DateTime.UtcNow - UtcTimeStart).TotalSeconds;
    private static int _intCounter = (int)(DateTime.UtcNow - UtcTimeStart).TotalSeconds;

    /// <summary>
    /// 使用Interlocked.Increment生成唯一ID的方法
    /// </summary>
    /// <returns></returns>
    public static int GetNextUniqueIntId()
    {
        // 原子性地递增值
        return Interlocked.Increment(ref _intCounter);
    }

    /// <summary>
    /// 使用Interlocked.Increment生成唯一ID的方法
    /// </summary>
    /// <returns></returns>
    public static long GetNextUniqueId()
    {
        // 原子性地递增值
        return Interlocked.Increment(ref _longCounter);
    }

    /// <summary>
    /// 获取唯一ID字符串
    /// </summary>
    /// <returns></returns>
    public static string GetUniqueIdString()
    {
        return Guid.NewGuid().ToString();
    }
}