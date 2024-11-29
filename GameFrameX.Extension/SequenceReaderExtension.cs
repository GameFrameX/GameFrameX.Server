using System.Buffers;

namespace GameFrameX.Extension;

/// <summary>
/// 
/// </summary>
public static class SequenceReaderExtension
{
    /// <summary>
    /// 从只读内存中获取字节数据
    /// </summary>
    /// <param name="reader">只读内存</param>
    /// <param name="value">结果值</param>
    /// <returns>读取成功返回True，否则返回False</returns>
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
    /// 从只读内存中获取字节数据,且移动读取位置
    /// </summary>
    /// <param name="reader">读取器</param>
    /// <param name="length">读取的长度</param>
    /// <param name="value">结果值</param>
    /// <returns>读取成功返回True，否则返回False</returns>
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
    /// 从只读内存中获取字节数据. 但是不移动读取位置
    /// </summary>
    /// <param name="reader">读取器</param>
    /// <param name="value">结果值</param>
    /// <returns>读取成功返回True，否则返回False</returns>
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
    /// 从只读内存中获取字节数据. 但是不移动读取位置
    /// </summary>
    /// <param name="reader">读取器</param>
    /// <param name="value">结果值</param>
    /// <returns>读取成功返回True，否则返回False</returns>
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
    /// 从只读内存中获取无符号的整型数据, 但是不移动读取位置
    /// </summary>
    /// <param name="reader">读取器</param>
    /// <param name="value">结果值</param>
    /// <returns>读取成功返回True，否则返回False</returns>
    public static bool TryPeekBigEndian(ref this SequenceReader<byte> reader, out uint value)
    {
        value = 0U;
        if (reader.Remaining < 4L)
        {
            return false;
        }

        int num1 = 0;
        int num2 = (int)Math.Pow(256.0, 3.0);
        for (int index = 0; index < 4; ++index)
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
    /// 从只读内存中获取无符号的长整型数据, 但是不移动读取位置
    /// </summary>
    /// <param name="reader">读取器</param>
    /// <param name="value">结果值</param>
    /// <returns></returns>
    public static bool TryPeekBigEndian(ref this SequenceReader<byte> reader, out ulong value)
    {
        value = 0UL;
        if (reader.Remaining < 8L)
        {
            return false;
        }

        long num1 = 0;
        long num2 = (long)Math.Pow(256.0, 7.0);
        for (int index = 0; index < 8; ++index)
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