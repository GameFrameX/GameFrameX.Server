using System.Runtime.InteropServices;

namespace GameFrameX.Extension;

/// <summary>
/// </summary>
public static class ReadOnlyMemoryExtension
{
    /// <summary>
    /// 尝试从只读内存中获取数组段，如果不成功则抛出异常。
    /// </summary>
    /// <param name="memory">源只读内存。</param>
    /// <returns>返回对应的数组段。</returns>
    public static ArraySegment<byte> GetArray(this ReadOnlyMemory<byte> memory)
    {
        if (!MemoryMarshal.TryGetArray(memory, out var result))
        {
            throw new InvalidOperationException("Buffer backed by array was expected");
        }

        return result;
    }
}