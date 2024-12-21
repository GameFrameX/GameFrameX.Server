using System.Buffers;

namespace GameFrameX.Extension;

/// <summary>
/// 提供对 <see cref="SequenceReader{T}" /> 类的扩展方法，用于从只读内存中读取数据。
/// </summary>
public static class SequenceReaderExtension
{
    /// <summary>
    /// 从只读内存中获取一个字节数据。
    /// </summary>
    /// <param name="reader">只读内存读取器。</param>
    /// <param name="value">结果值。</param>
    /// <returns>读取成功返回 True，否则返回 False。</returns>
    public static bool TryReadBigEndian(this ref SequenceReader<byte> reader, out byte value)
    {
        value = 0;
        if (reader.Remaining < 1L || !reader.TryRead(out var num1))
        {
            return false;
        }

        value = num1;
        return true;
    }

    /// <summary>
    /// 从只读内存中获取指定长度的字节数据，并移动读取位置。
    /// </summary>
    /// <param name="reader">读取器。</param>
    /// <param name="length">读取的长度。</param>
    /// <param name="value">结果值。</param>
    /// <returns>读取成功返回 True，否则返回 False。</returns>
    public static bool TryReadBytes(this ref SequenceReader<byte> reader, int length, out byte[] value)
    {
        value = new byte[length];
        if (reader.Remaining < length || !reader.TryCopyTo(value))
        {
            return false;
        }

        reader.Advance(length);
        return true;
    }

    /// <summary>
    /// 从只读内存中获取一个字节数据，但不移动读取位置。
    /// </summary>
    /// <param name="reader">读取器。</param>
    /// <param name="value">结果值。</param>
    /// <returns>读取成功返回 True，否则返回 False。</returns>
    public static bool TryPeekBigEndian(ref this SequenceReader<byte> reader, out byte value)
    {
        value = 0;
        if (reader.Remaining < 1L || !reader.TryPeek(0, out var num1))
        {
            return false;
        }

        value = num1;
        return true;
    }

    /// <summary>
    /// 从只读内存中获取一个无符号短整型数据，但不移动读取位置。
    /// </summary>
    /// <param name="reader">读取器。</param>
    /// <param name="value">结果值。</param>
    /// <returns>读取成功返回 True，否则返回 False。</returns>
    public static bool TryPeekBigEndian(ref this SequenceReader<byte> reader, out ushort value)
    {
        value = 0;
        if (reader.Remaining < 2L || !reader.TryPeek(0, out var num1) || !reader.TryPeek(1, out var num2))
        {
            return false;
        }

        value = (ushort)(num1 * 256U + num2);
        return true;
    }

    /// <summary>
    /// 从只读内存中获取一个无符号整型数据，但不移动读取位置。
    /// </summary>
    /// <param name="reader">读取器。</param>
    /// <param name="value">结果值。</param>
    /// <returns>读取成功返回 True，否则返回 False。</returns>
    public static bool TryPeekBigEndian(ref this SequenceReader<byte> reader, out uint value)
    {
        value = 0U;
        if (reader.Remaining < 4L)
        {
            return false;
        }

        var num1 = 0;
        var num2 = (int)Math.Pow(256.0, 3.0);
        for (var index = 0; index < 4; ++index)
        {
            if (!reader.TryPeek(index, out var num3))
            {
                return false;
            }

            num1 += num2 * num3;
            num2 /= 256;
        }

        value = (uint)num1;
        return true;
    }

    /// <summary>
    /// 从只读内存中获取一个无符号长整型数据，但不移动读取位置。
    /// </summary>
    /// <param name="reader">读取器。</param>
    /// <param name="value">结果值。</param>
    /// <returns>读取成功返回 True，否则返回 False。</returns>
    public static bool TryPeekBigEndian(ref this SequenceReader<byte> reader, out ulong value)
    {
        value = 0UL;
        if (reader.Remaining < 8L)
        {
            return false;
        }

        long num1 = 0;
        var num2 = (long)Math.Pow(256.0, 7.0);
        for (var index = 0; index < 8; ++index)
        {
            if (!reader.TryPeek(index, out var num3))
            {
                return false;
            }

            num1 += num2 * num3;
            num2 /= 256L;
        }

        value = (ulong)num1;
        return true;
    }
}