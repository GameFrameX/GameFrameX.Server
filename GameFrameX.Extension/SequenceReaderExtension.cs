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
    /// <param name="value">值</param>
    /// <returns>读取成功返回True，否则返回False</returns>
    public static bool TryReadBigEndian(this ref SequenceReader<byte> reader, out byte value)
    {
        value = 0;
        if (reader.Remaining < 1L || !reader.TryRead(out var num1))
        {
            return false;
        }

        value = num1;
        reader.Advance(sizeof(byte));
        return true;
    }

    /// <summary>
    /// 从只读内存中获取字节数据
    /// </summary>
    /// <param name="reader"></param>
    /// <param name="length"></param>
    /// <param name="value"></param>
    /// <returns></returns>
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
    /// <param name="reader"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static bool TryPeekBigEndian(ref this SequenceReader<byte> reader, out ushort value)
    {
        value = 0;
        if (reader.Remaining < 2L || !reader.TryPeek(out var num1) || !reader.TryPeek(out var num2))
        {
            return false;
        }

        value = (ushort)(num1 * 256U + num2);
        reader.Advance(sizeof(ushort));
        return true;
    }
}