using System.Buffers.Binary;
using System.Net;
using System.Text;

namespace GameFrameX.Utility.Extensions;

/// <summary>
/// 提供对Span&lt;byte&gt;的扩展方法，用于高效地读写基本数据类型。
/// </summary>
public static class SpanExtension
{
    #region WriteSpan

    /// <summary>
    /// 将一个32位无符号整数写入指定的缓冲区，并更新偏移量。
    /// </summary>
    /// <param name="buffer">要写入的缓冲区。</param>
    /// <param name="value">要写入的值。</param>
    /// <param name="offset">要写入值的缓冲区中的偏移量。</param>
    public static void WriteUInt(this Span<byte> buffer, uint value, ref int offset)
    {
        if (offset + ConstSize.UIntSize > buffer.Length)
        {
            offset += ConstSize.UIntSize;
            return;
        }

        BinaryPrimitives.WriteUInt32BigEndian(buffer[offset..], value);
        offset += ConstSize.UIntSize;
    }

    /// <summary>
    /// 将一个16位无符号整数写入指定的缓冲区，并更新偏移量。
    /// </summary>
    /// <param name="buffer">要写入的缓冲区。</param>
    /// <param name="value">要写入的值。</param>
    /// <param name="offset">要写入值的缓冲区中的偏移量。</param>
    public static void WriteUShort(this Span<byte> buffer, ushort value, ref int offset)
    {
        if (offset + ConstSize.UShortSize > buffer.Length)
        {
            offset += ConstSize.UShortSize;
            return;
        }

        BinaryPrimitives.WriteUInt16BigEndian(buffer[offset..], value);
        offset += ConstSize.UShortSize;
    }

    /// <summary>
    /// 将整数值写入到指定的字节跨度中。如果指定的偏移量加上整数大小超过了字节跨度的长度，则抛出异常。
    /// 以网络字节顺序存储整数值。
    /// </summary>
    /// <param name="buffer">字节跨度，用于存储整数值。</param>
    /// <param name="value">要写入的整数值。</param>
    /// <param name="offset">写入的起始偏移量，会在调用后增加整数的大小。</param>
    public static unsafe void WriteInt(this Span<byte> buffer, int value, ref int offset)
    {
        if (offset + ConstSize.IntSize > buffer.Length)
        {
            throw new ArgumentException($"buffer write out of index {offset + ConstSize.IntSize}, {buffer.Length}");
        }

        fixed (byte* ptr = buffer)
        {
            *(int*)(ptr + offset) = IPAddress.HostToNetworkOrder(value);
            offset += ConstSize.IntSize;
        }
    }

    /// <summary>
    /// 将长整数值写入到指定的字节跨度中。如果指定的偏移量加上长整数大小超过了字节跨度的长度，则抛出异常。
    /// 以网络字节顺序存储长整数值。
    /// </summary>
    /// <param name="buffer">字节跨度，用于存储长整数值。</param>
    /// <param name="value">要写入的长整数值。</param>
    /// <param name="offset">写入的起始偏移量，会在调用后增加长整数的大小。</param>
    public static unsafe void WriteLong(this Span<byte> buffer, long value, ref int offset)
    {
        if (offset + ConstSize.LongSize > buffer.Length)
        {
            throw new ArgumentException($"buffer write out of index {offset + ConstSize.LongSize}, {buffer.Length}");
        }

        fixed (byte* ptr = buffer)
        {
            *(long*)(ptr + offset) = IPAddress.HostToNetworkOrder(value);
            offset += ConstSize.LongSize;
        }
    }

    /// <summary>
    /// 在给定的偏移量位置，向缓冲区中写入字节序列，不包含长度信息。
    /// </summary>
    /// <param name="buffer">目标字节缓冲区。</param>
    /// <param name="value">需要写入的字节序列。</param>
    /// <param name="offset">字节写入的起始偏移量，写入后更新。</param>
    public static unsafe void WriteBytesWithoutLength(this Span<byte> buffer, byte[] value, ref int offset)
    {
        if (value == null)
        {
            buffer.WriteInt(0, ref offset);
            return;
        }

        if (offset + value.Length > buffer.Length)
        {
            throw new ArgumentException($"buffer write out of index {offset + value.Length}, {buffer.Length}");
        }

        fixed (byte* ptr = buffer, valPtr = value)
        {
            Buffer.MemoryCopy(valPtr, ptr + offset, value.Length, value.Length);
            offset += value.Length;
        }
    }

    #endregion


    #region ReadSpan

    /// <summary>
    /// 从指定的byte缓冲区和偏移量读取一个int值。
    /// </summary>
    /// <param name="buffer">字节缓冲区。</param>
    /// <param name="offset">开始读取的偏移量，读取后将更新此偏移量。</param>
    /// <returns>读取到的int值。</returns>
    /// <exception cref="System.Exception">当偏移量超出缓冲区大小时，会抛出此异常。</exception>
    public static unsafe int ReadInt(this Span<byte> buffer, ref int offset)
    {
        if (offset > buffer.Length + ConstSize.IntSize)
        {
            throw new Exception("buffer read out of index");
        }

        fixed (byte* ptr = buffer)
        {
            var value = *(int*)(ptr + offset);
            offset += ConstSize.IntSize;
            return IPAddress.NetworkToHostOrder(value);
        }
    }

    /// <summary>
    /// 从给定的字节缓存区读取一个短整型值（16位）。
    /// </summary>
    /// <param name="buffer">字节缓冲区</param>
    /// <param name="offset">偏移量，读取结束后会更新此偏移量。</param>
    /// <returns>从字节缓存区中读取出的短整型值</returns>
    /// <exception cref="Exception">如果读取的位置超出了缓冲区大小范围</exception>
    public static unsafe short ReadShort(this Span<byte> buffer, ref int offset)
    {
        if (offset > buffer.Length + ConstSize.ShortSize)
        {
            throw new Exception("buffer read out of index");
        }

        fixed (byte* ptr = buffer)
        {
            var value = *(short*)(ptr + offset);
            offset += ConstSize.ShortSize;
            return IPAddress.NetworkToHostOrder(value);
        }
    }

    /// <summary>
    /// 从Span字节数组中读取32位无符号整数，并将偏移量向前移动。
    /// </summary>
    /// <param name="buffer">要读取的Span字节数组。</param>
    /// <param name="offset">引用偏移量。</param>
    /// <returns>返回读取的32位无符号整数。</returns>
    public static uint ReadUInt(this Span<byte> buffer, ref int offset)
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
    /// 从Span字节数组中读取64位无符号整数，并将偏移量向前移动。
    /// </summary>
    /// <param name="buffer">要读取的Span字节数组。</param>
    /// <param name="offset">引用偏移量。</param>
    /// <returns>返回读取的64位无符号整数。</returns>
    public static ulong ReadULong(this Span<byte> buffer, ref int offset)
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
    /// 从给定的字节缓存区读取一个长整型值（64位）。
    /// </summary>
    /// <param name="buffer">字节缓冲区</param>
    /// <param name="offset">偏移量，读取结束后会更新此偏移量。</param>
    /// <returns>从字节缓存区中读取出的长整型值</returns>
    /// <exception cref="Exception">如果读取的位置超出了缓冲区大小范围</exception>
    public static unsafe long ReadLong(this Span<byte> buffer, ref int offset)
    {
        if (offset > buffer.Length + ConstSize.LongSize)
        {
            throw new Exception("buffer read out of index");
        }

        fixed (byte* ptr = buffer)
        {
            var value = *(long*)(ptr + offset);
            offset += ConstSize.LongSize;
            return IPAddress.NetworkToHostOrder(value);
        }
    }

    /// <summary>
    /// 从给定的字节缓存区读取一个浮点型值（32位）。
    /// </summary>
    /// <param name="buffer">字节缓冲区</param>
    /// <param name="offset">偏移量，读取结束后会更新此偏移量。</param>
    /// <returns>从字节缓存区中读取出的浮点型值</returns>
    /// <exception cref="Exception">如果读取的位置超出了缓冲区大小范围</exception>
    public static unsafe float ReadFloat(this Span<byte> buffer, ref int offset)
    {
        if (offset > buffer.Length + ConstSize.FloatSize)
        {
            throw new Exception("buffer read out of index");
        }

        fixed (byte* ptr = buffer)
        {
            *(int*)(ptr + offset) = IPAddress.NetworkToHostOrder(*(int*)(ptr + offset));
            var value = *(float*)(ptr + offset);
            offset += ConstSize.FloatSize;
            return value;
        }
    }

    /// <summary>
    /// 使用指定的偏移量从字节跨度中读取浮点数。
    /// </summary>
    /// <param name="buffer">字节跨度。</param>
    /// <param name="offset">开始读取的偏移量，读取后会自动增加。</param>
    /// <returns>读取的浮点数。</returns>
    /// <exception cref="Exception">当缓冲区读取超出范围时抛出异常。</exception>
    public static unsafe double ReadDouble(this Span<byte> buffer, ref int offset)
    {
        if (offset > buffer.Length + ConstSize.DoubleSize)
        {
            throw new Exception("buffer read out of index");
        }

        fixed (byte* ptr = buffer)
        {
            *(long*)(ptr + offset) = IPAddress.NetworkToHostOrder(*(long*)(ptr + offset));
            var value = *(double*)(ptr + offset);
            offset += ConstSize.DoubleSize;
            return value;
        }
    }

    /// <summary>
    /// 使用指定的偏移量从字节跨度中读取字节。
    /// </summary>
    /// <param name="buffer">字节跨度。</param>
    /// <param name="offset">开始读取的偏移量，读取后会自动增加。</param>
    /// <returns>读取的字节。</returns>
    /// <exception cref="Exception">当缓冲区读取超出范围时抛出异常。</exception>
    public static unsafe byte ReadByte(this Span<byte> buffer, ref int offset)
    {
        if (offset > buffer.Length + ConstSize.ByteSize)
        {
            throw new Exception("buffer read out of index");
        }

        fixed (byte* ptr = buffer)
        {
            var value = *(ptr + offset);
            offset += ConstSize.ByteSize;
            return value;
        }
    }

    /// <summary>
    /// 使用指定的偏移量从字节跨度中读取字节数组。
    /// </summary>
    /// <param name="buffer">字节跨度。</param>
    /// <param name="offset">开始读取的偏移量，读取后会增加对应的字节数组的长度。</param>
    /// <returns>读取的字节数组。如果长度小于或等于0，返回空数组。</returns>
    public static byte[] ReadBytes(this Span<byte> buffer, ref int offset)
    {
        var len = ReadInt(buffer, ref offset);

        if (len <= 0 || offset > buffer.Length + len * ConstSize.ByteSize)
        {
            return Array.Empty<byte>();
        }

        var data = buffer.Slice(offset, len).ToArray();
        offset += len;
        return data;
    }

    /// <summary>
    /// 从给定的字节跨度中读取一个有符号字节并从偏移量处开始更新偏移量。
    /// </summary>
    /// <param name="buffer">要读取的字节跨度。</param>
    /// <param name="offset">开始读取的偏移量。</param>
    /// <returns>返回读取的有符号字节。</returns>
    /// <exception cref="Exception">当buffer读取超出索引时抛出异常。</exception>
    public static unsafe sbyte ReadSByte(this Span<byte> buffer, ref int offset)
    {
        if (offset > buffer.Length + ConstSize.ByteSize)
        {
            throw new Exception("buffer read out of index");
        }

        fixed (byte* ptr = buffer)
        {
            var value = *(sbyte*)(ptr + offset);
            offset += ConstSize.ByteSize;
            return value;
        }
    }

    /// <summary>
    /// 从给定的字节跨度中读取一个字符串并从偏移量处开始更新偏移量。
    /// </summary>
    /// <param name="buffer">要读取的字节跨度。</param>
    /// <param name="offset">开始读取的偏移量。</param>
    /// <returns>返回读取的字符串。</returns>
    public static unsafe string ReadString(this Span<byte> buffer, ref int offset)
    {
        var len = ReadShort(buffer, ref offset);

        if (len <= 0 || offset > buffer.Length + len * ConstSize.ByteSize)
        {
            return string.Empty;
        }

        fixed (byte* ptr = buffer)
        {
            var value = Encoding.UTF8.GetString(ptr + offset, len);
            offset += len;
            return value;
        }
    }

    /// <summary>
    /// 从给定的字节跨度中读取一个布尔值并从偏移量处开始更新偏移量。
    /// </summary>
    /// <param name="buffer">要读取的字节跨度。</param>
    /// <param name="offset">开始读取的偏移量。</param>
    /// <returns>返回读取的布尔值。</returns>
    /// <exception cref="Exception">当buffer读取超出索引时抛出异常。</exception>
    public static unsafe bool ReadBool(this Span<byte> buffer, ref int offset)
    {
        if (offset > buffer.Length + ConstSize.BoolSize)
        {
            throw new Exception("buffer read out of index");
        }

        fixed (byte* ptr = buffer)
        {
            var value = *(bool*)(ptr + offset);
            offset += ConstSize.BoolSize;
            return value;
        }
    }

    #endregion
}