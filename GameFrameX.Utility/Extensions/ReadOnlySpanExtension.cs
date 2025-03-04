using System.Buffers.Binary;

namespace GameFrameX.Utility.Extensions;

/// <summary>
/// </summary>
public static class ReadOnlySpanExtension
{
    /// <summary>
    /// 从字节数组中以指定偏移量读取无符号整型。
    /// </summary>
    /// <param name="buffer">要从中读取数据的字节数组。</param>
    /// <param name="offset">读取数据的起始偏移量，此偏移量在读取后会自动增加。</param>
    /// <returns>读取的无符号整型，若读取长度小于等于0或偏移量超出数组长度，返回0。</returns>
    public static uint ReadUInt(this ReadOnlySpan<byte> buffer, ref int offset)
    {
        if (offset > buffer.Length + ConstSize.UIntSize)
        {
            throw new Exception("buffer read out of index");
        }

        var value = BinaryPrimitives.ReadUInt32BigEndian(buffer[offset..]);
        offset += ConstSize.UIntSize;
        return value;
    }

    /// <summary>
    /// 从字节数组中以指定偏移量读取整型。
    /// </summary>
    /// <param name="buffer">要从中读取数据的字节数组。</param>
    /// <param name="offset">读取数据的起始偏移量，此偏移量在读取后会自动增加。</param>
    /// <returns>读取的整型，若读取长度小于等于0或偏移量超出数组长度，返回0。</returns>
    public static int ReadInt(this ReadOnlySpan<byte> buffer, ref int offset)
    {
        if (offset > buffer.Length + ConstSize.IntSize)
        {
            throw new Exception("buffer read out of index");
        }

        var value = BinaryPrimitives.ReadInt32BigEndian(buffer[offset..]);
        offset += ConstSize.IntSize;
        return value;
    }

    /// <summary>
    /// 从字节数组中以指定偏移量读取无符号长整型。
    /// </summary>
    /// <param name="buffer">要从中读取数据的字节数组。</param>
    /// <param name="offset">读取数据的起始偏移量，此偏移量在读取后会自动增加。</param>
    /// <returns>读取的无符号长整型，若读取长度小于等于0或偏移量超出数组长度，返回0。</returns>
    public static ulong ReadULong(this ReadOnlySpan<byte> buffer, ref int offset)
    {
        if (offset > buffer.Length + ConstSize.ULongSize)
        {
            throw new Exception("buffer read out of index");
        }

        var value = BinaryPrimitives.ReadUInt64BigEndian(buffer[offset..]);
        offset += ConstSize.ULongSize;
        return value;
    }

    /// <summary>
    /// 从字节数组中以指定偏移量读取长整型。
    /// </summary>
    /// <param name="buffer">要从中读取数据的字节数组。</param>
    /// <param name="offset">读取数据的起始偏移量，此偏移量在读取后会自动增加。</param>
    /// <returns>读取的长整型，若读取长度小于等于0或偏移量超出数组长度，返回0。</returns>
    public static long ReadLong(this ReadOnlySpan<byte> buffer, ref int offset)
    {
        if (offset > buffer.Length + ConstSize.LongSize)
        {
            throw new Exception("buffer read out of index");
        }

        var value = BinaryPrimitives.ReadInt64BigEndian(buffer[offset..]);
        offset += ConstSize.LongSize;
        return value;
    }

    /// <summary>
    /// 从字节数组中以指定偏移量读取无符号短整型。
    /// </summary>
    /// <param name="buffer">要从中读取数据的字节数组。</param>
    /// <param name="offset">读取数据的起始偏移量，此偏移量在读取后会自动增加。</param>
    /// <returns>读取的无符号短整型，若读取长度小于等于0或偏移量超出数组长度，返回0。</returns>
    public static ushort ReadUShort(this ReadOnlySpan<byte> buffer, ref int offset)
    {
        if (offset > buffer.Length + ConstSize.UShortSize)
        {
            throw new Exception("buffer read out of index");
        }

        var value = BinaryPrimitives.ReadUInt16BigEndian(buffer[offset..]);
        offset += ConstSize.UShortSize;
        return value;
    }

    /// <summary>
    /// 从字节数组中以指定偏移量读取短整型。
    /// </summary>
    /// <param name="buffer">要从中读取数据的字节数组。</param>
    /// <param name="offset">读取数据的起始偏移量，此偏移量在读取后会自动增加。</param>
    /// <returns>读取的短整型，若读取长度小于等于0或偏移量超出数组长度，返回0。</returns>
    public static short ReadShort(this ReadOnlySpan<byte> buffer, ref int offset)
    {
        if (offset > buffer.Length + ConstSize.ShortSize)
        {
            throw new Exception("buffer read out of index");
        }

        var value = BinaryPrimitives.ReadInt16BigEndian(buffer[offset..]);
        offset += ConstSize.ShortSize;
        return value;
    }

    /// <summary>
    /// 从字节数组中以指定偏移量读取单精度浮点数。
    /// </summary>
    /// <param name="buffer">要从中读取数据的字节数组。</param>
    /// <param name="offset">读取数据的起始偏移量，此偏移量在读取后会自动增加。</param>
    /// <returns>读取的单精度浮点数，若读取长度小于等于0或偏移量超出数组长度，返回0。</returns>
    public static float ReadFloat(this ReadOnlySpan<byte> buffer, ref int offset)
    {
        if (offset > buffer.Length + ConstSize.FloatSize)
        {
            throw new Exception("buffer read out of index");
        }

        var value = BinaryPrimitives.ReadSingleBigEndian(buffer[offset..]);
        offset += ConstSize.FloatSize;
        return value;
    }

    /// <summary>
    /// 从字节数组中以指定偏移量读取双精度浮点数。
    /// </summary>
    /// <param name="buffer">要从中读取数据的字节数组。</param>
    /// <param name="offset">读取数据的起始偏移量，此偏移量在读取后会自动增加。</param>
    /// <returns>读取的双精度浮点数，若读取长度小于等于0或偏移量超出数组长度，返回0。</returns>
    public static double ReadDouble(this ReadOnlySpan<byte> buffer, ref int offset)
    {
        if (offset > buffer.Length + ConstSize.DoubleSize)
        {
            throw new Exception("buffer read out of index");
        }

        var value = BinaryPrimitives.ReadDoubleBigEndian(buffer[offset..]);
        offset += ConstSize.DoubleSize;
        return value;
    }

}