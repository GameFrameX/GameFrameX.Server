// ==========================================================================================
//   GameFrameX 组织及其衍生项目的版权、商标、专利及其他相关权利
//   GameFrameX organization and its derivative projects' copyrights, trademarks, patents, and related rights
//   均受中华人民共和国及相关国际法律法规保护。
//   are protected by the laws of the People's Republic of China and relevant international regulations.
//   使用本项目须严格遵守相应法律法规及开源许可证之规定。
//   Usage of this project must strictly comply with applicable laws, regulations, and open-source licenses.
//   本项目采用 MIT 许可证与 Apache License 2.0 双许可证分发，
//   This project is dual-licensed under the MIT License and Apache License 2.0,
//   完整许可证文本请参见源代码根目录下的 LICENSE 文件。
//   please refer to the LICENSE file in the root directory of the source code for the full license text.
//   禁止利用本项目实施任何危害国家安全、破坏社会秩序、
//   It is prohibited to use this project to engage in any activities that endanger national security, disrupt social order,
//   侵犯他人合法权益等法律法规所禁止的行为！
//   or infringe upon the legitimate rights and interests of others, as prohibited by laws and regulations!
//   因基于本项目二次开发所产生的一切法律纠纷与责任，
//   Any legal disputes and liabilities arising from secondary development based on this project
//   本项目组织与贡献者概不承担。
//   shall be borne solely by the developer; the project organization and contributors assume no responsibility.
//   GitHub 仓库：https://github.com/GameFrameX
//   GitHub Repository: https://github.com/GameFrameX
//   Gitee  仓库：https://gitee.com/GameFrameX
//   Gitee Repository:  https://gitee.com/GameFrameX
//   CNB  仓库：https://cnb.cool/GameFrameX
//   CNB Repository:  https://cnb.cool/GameFrameX
//   官方文档：https://gameframex.doc.alianblank.com/
//   Official Documentation: https://gameframex.doc.alianblank.com/
//  ==========================================================================================

#define CPU_SUPPORT_MEMORY_NOT_ALIGN //CPU 是否支持读取非对齐内存

using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;

namespace GameFrameX.Core.Config;

/// <summary>
/// 字节缓冲区类，用于二进制数据的读写操作。
/// </summary>
/// <remarks>
/// Byte buffer class for reading and writing binary data.
/// </remarks>
public sealed class ByteBuf : ICloneable, IEquatable<ByteBuf>
{
    /// <summary>
    /// 默认构造函数，创建一个空的字节缓冲区。
    /// </summary>
    /// <remarks>
    /// Default constructor, creates an empty byte buffer.
    /// </remarks>
    public ByteBuf()
    {
        Bytes = Array.Empty<byte>();
        ReaderIndex = WriterIndex = 0;
    }

    /// <summary>
    /// 使用指定容量创建字节缓冲区。
    /// </summary>
    /// <remarks>
    /// Creates a byte buffer with the specified capacity.
    /// </remarks>
    /// <param name="capacity">初始容量 / Initial capacity</param>
    public ByteBuf(int capacity)
    {
        Bytes = capacity > 0 ? new byte[capacity] : Array.Empty<byte>();
        ReaderIndex = 0;
        WriterIndex = 0;
    }

    /// <summary>
    /// 使用指定字节数组创建字节缓冲区。
    /// </summary>
    /// <remarks>
    /// Creates a byte buffer with the specified byte array.
    /// </remarks>
    /// <param name="bytes">字节数组 / Byte array</param>
    public ByteBuf(byte[] bytes)
    {
        Bytes = bytes;
        ReaderIndex = 0;
        WriterIndex = Capacity;
    }

    /// <summary>
    /// 使用指定字节数组和读写位置创建字节缓冲区。
    /// </summary>
    /// <remarks>
    /// Creates a byte buffer with the specified byte array and read/write positions.
    /// </remarks>
    /// <param name="bytes">字节数组 / Byte array</param>
    /// <param name="readIndex">读取位置 / Read position</param>
    /// <param name="writeIndex">写入位置 / Write position</param>
    public ByteBuf(byte[] bytes, int readIndex, int writeIndex)
    {
        Bytes = bytes;
        ReaderIndex = readIndex;
        WriterIndex = writeIndex;
    }

    /// <summary>
    /// 使用指定容量和释放器创建字节缓冲区。
    /// </summary>
    /// <remarks>
    /// Creates a byte buffer with the specified capacity and releaser.
    /// </remarks>
    /// <param name="capacity">初始容量 / Initial capacity</param>
    /// <param name="releaser">释放器委托 / Releaser delegate</param>
    public ByteBuf(int capacity, Action<ByteBuf> releaser) : this(capacity)
    {
        _releaser = releaser;
    }

    /// <summary>
    /// 包装字节数组为ByteBuf。
    /// </summary>
    /// <remarks>
    /// Wraps a byte array as a ByteBuf.
    /// </remarks>
    /// <param name="bytes">要包装的字节数组 / The byte array to wrap</param>
    /// <returns>包装后的ByteBuf对象 / The wrapped ByteBuf object</returns>
    public static ByteBuf Wrap(byte[] bytes)
    {
        return new ByteBuf(bytes, 0, bytes.Length);
    }

    /// <summary>
    /// 替换内部字节数组。
    /// </summary>
    /// <remarks>
    /// Replaces the internal byte array.
    /// </remarks>
    /// <param name="bytes">新的字节数组 / The new byte array</param>
    public void Replace(byte[] bytes)
    {
        Bytes = bytes;
        ReaderIndex = 0;
        WriterIndex = Capacity;
    }

    /// <summary>
    /// 替换内部字节数组并指定读写位置。
    /// </summary>
    /// <remarks>
    /// Replaces the internal byte array with specified read/write positions.
    /// </remarks>
    /// <param name="bytes">新的字节数组 / The new byte array</param>
    /// <param name="beginPos">起始位置 / Begin position</param>
    /// <param name="endPos">结束位置 / End position</param>
    public void Replace(byte[] bytes, int beginPos, int endPos)
    {
        Bytes = bytes;
        ReaderIndex = beginPos;
        WriterIndex = endPos;
    }

    /// <summary>
    /// 获取或设置当前读取位置。
    /// </summary>
    /// <remarks>
    /// Gets or sets the current read position.
    /// </remarks>
    /// <value>当前读取位置 / The current read position</value>
    public int ReaderIndex { get; set; }

    /// <summary>
    /// 获取或设置当前写入位置。
    /// </summary>
    /// <remarks>
    /// Gets or sets the current write position.
    /// </remarks>
    /// <value>当前写入位置 / The current write position</value>
    public int WriterIndex { get; set; }

    private readonly Action<ByteBuf> _releaser;

    /// <summary>
    /// 获取缓冲区容量。
    /// </summary>
    /// <remarks>
    /// Gets the buffer capacity.
    /// </remarks>
    /// <value>缓冲区容量 / The buffer capacity</value>
    public int Capacity
    {
        get { return Bytes.Length; }
    }

    /// <summary>
    /// 获取当前数据大小。
    /// </summary>
    /// <remarks>
    /// Gets the current data size.
    /// </remarks>
    /// <value>当前数据大小 / The current data size</value>
    public int Size
    {
        get { return WriterIndex - ReaderIndex; }
    }

    /// <summary>
    /// 获取缓冲区是否为空。
    /// </summary>
    /// <remarks>
    /// Gets whether the buffer is empty.
    /// </remarks>
    /// <value>如果缓冲区为空则为 <c>true</c>；否则为 <c>false</c> / <c>true</c> if the buffer is empty; otherwise <c>false</c></value>
    public bool Empty
    {
        get { return WriterIndex <= ReaderIndex; }
    }

    /// <summary>
    /// 获取缓冲区是否非空。
    /// </summary>
    /// <remarks>
    /// Gets whether the buffer is not empty.
    /// </remarks>
    /// <value>如果缓冲区非空则为 <c>true</c>；否则为 <c>false</c> / <c>true</c> if the buffer is not empty; otherwise <c>false</c></value>
    public bool NotEmpty
    {
        get { return WriterIndex > ReaderIndex; }
    }

    /// <summary>
    /// 增加写入位置。
    /// </summary>
    /// <remarks>
    /// Increases the write position.
    /// </remarks>
    /// <param name="add">增加的值 / The value to add</param>
    public void AddWriteIndex(int add)
    {
        WriterIndex += add;
    }

    /// <summary>
    /// 增加读取位置。
    /// </summary>
    /// <remarks>
    /// Increases the read position.
    /// </remarks>
    /// <param name="add">增加的值 / The value to add</param>
    public void AddReadIndex(int add)
    {
        ReaderIndex += add;
    }

#pragma warning disable CA1819 // 属性不应返回数组
    /// <summary>
    /// 获取内部字节数组。
    /// </summary>
    /// <remarks>
    /// Gets the internal byte array.
    /// </remarks>
    /// <value>内部字节数组 / The internal byte array</value>
    public byte[] Bytes { get; private set; }
#pragma warning restore CA1819 // 属性不应返回数组

    /// <summary>
    /// 复制当前数据。
    /// </summary>
    /// <remarks>
    /// Copies the current data.
    /// </remarks>
    /// <returns>复制的字节数组 / The copied byte array</returns>
    public byte[] CopyData()
    {
        var n = Remaining;
        if (n > 0)
        {
            var arr = new byte[n];
            Buffer.BlockCopy(Bytes, ReaderIndex, arr, 0, n);
            return arr;
        }
        else
        {
            return Array.Empty<byte>();
        }
    }

    /// <summary>
    /// 获取剩余可读取的字节数。
    /// </summary>
    /// <remarks>
    /// Gets the number of remaining bytes that can be read.
    /// </remarks>
    /// <value>剩余可读取的字节数 / The number of remaining bytes that can be read</value>
    public int Remaining
    {
        get { return WriterIndex - ReaderIndex; }
    }

    /// <summary>
    /// 丢弃已读取的字节。
    /// </summary>
    /// <remarks>
    /// Discards the bytes that have been read.
    /// </remarks>
    public void DiscardReadBytes()
    {
        WriterIndex -= ReaderIndex;
        Array.Copy(Bytes, ReaderIndex, Bytes, 0, WriterIndex);
        ReaderIndex = 0;
    }

    /// <summary>
    /// 获取未压缩的可写入字节数。
    /// </summary>
    /// <remarks>
    /// Gets the number of bytes that can be written without compaction.
    /// </remarks>
    /// <value>未压缩的可写入字节数 / The number of bytes that can be written without compaction</value>
    public int NotCompactWritable
    {
        get { return Capacity - WriterIndex; }
    }

    /// <summary>
    /// 写入字节数组（不写入大小）。
    /// </summary>
    /// <remarks>
    /// Writes a byte array without writing the size.
    /// </remarks>
    /// <param name="bs">要写入的字节数组 / The byte array to write</param>
    public void WriteBytesWithoutSize(byte[] bs)
    {
        WriteBytesWithoutSize(bs, 0, bs.Length);
    }

    /// <summary>
    /// 写入字节数组的指定部分（不写入大小）。
    /// </summary>
    /// <remarks>
    /// Writes a portion of the byte array without writing the size.
    /// </remarks>
    /// <param name="bs">要写入的字节数组 / The byte array to write</param>
    /// <param name="offset">起始偏移 / The starting offset</param>
    /// <param name="len">长度 / The length</param>
    public void WriteBytesWithoutSize(byte[] bs, int offset, int len)
    {
        EnsureWrite(len);
        Buffer.BlockCopy(bs, offset, Bytes, WriterIndex, len);
        WriterIndex += len;
    }

    /// <summary>
    /// 清空缓冲区。
    /// </summary>
    /// <remarks>
    /// Clears the buffer.
    /// </remarks>
    public void Clear()
    {
        ReaderIndex = WriterIndex = 0;
    }

    /// <summary>
    /// 最小容量
    /// </summary>
    private const int MinCapacity = 16;

    private static int PropSize(int initSize, int needSize)
    {
        for (var i = Math.Max(initSize, MinCapacity);; i <<= 1)
        {
            if (i >= needSize)
            {
                return i;
            }
        }
    }

    private void EnsureWrite0(int size)
    {
        var needSize = WriterIndex + size - ReaderIndex;
        if (needSize < Capacity)
        {
            WriterIndex -= ReaderIndex;
            Array.Copy(Bytes, ReaderIndex, Bytes, 0, WriterIndex);
            ReaderIndex = 0;
        }
        else
        {
            var newCapacity = PropSize(Capacity, needSize);
            var newBytes = new byte[newCapacity];
            WriterIndex -= ReaderIndex;
            Buffer.BlockCopy(Bytes, ReaderIndex, newBytes, 0, WriterIndex);
            ReaderIndex = 0;
            Bytes = newBytes;
        }
    }

    /// <summary>
    /// 确保有足够的写入空间。
    /// </summary>
    /// <remarks>
    /// Ensures there is enough space to write.
    /// </remarks>
    /// <param name="size">需要的空间大小 / The required space size</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void EnsureWrite(int size)
    {
        if (WriterIndex + size > Capacity)
        {
            EnsureWrite0(size);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void EnsureRead(int size)
    {
        if (ReaderIndex + size > WriterIndex)
        {
            throw new SerializationException();
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private bool CanRead(int size)
    {
        return ReaderIndex + size <= WriterIndex;
    }

    /// <summary>
    /// 追加一个字节。
    /// </summary>
    /// <remarks>
    /// Appends a single byte.
    /// </remarks>
    /// <param name="x">要追加的字节 / The byte to append</param>
    public void Append(byte x)
    {
        EnsureWrite(1);
        Bytes[WriterIndex++] = x;
    }

    /// <summary>
    /// 写入布尔值。
    /// </summary>
    /// <remarks>
    /// Writes a boolean value.
    /// </remarks>
    /// <param name="b">要写入的布尔值 / The boolean value to write</param>
    public void WriteBool(bool b)
    {
        EnsureWrite(1);
        Bytes[WriterIndex++] = (byte)(b ? 1 : 0);
    }

    /// <summary>
    /// 读取布尔值。
    /// </summary>
    /// <remarks>
    /// Reads a boolean value.
    /// </remarks>
    /// <returns>读取的布尔值 / The read boolean value</returns>
    public bool ReadBool()
    {
        EnsureRead(1);
        return Bytes[ReaderIndex++] != 0;
    }

    /// <summary>
    /// 写入字节。
    /// </summary>
    /// <remarks>
    /// Writes a byte.
    /// </remarks>
    /// <param name="x">要写入的字节 / The byte to write</param>
    public void WriteByte(byte x)
    {
        EnsureWrite(1);
        Bytes[WriterIndex++] = x;
    }

    /// <summary>
    /// 读取字节。
    /// </summary>
    /// <remarks>
    /// Reads a byte.
    /// </remarks>
    /// <returns>读取的字节 / The read byte</returns>
    public byte ReadByte()
    {
        EnsureRead(1);
        return Bytes[ReaderIndex++];
    }

    /// <summary>
    /// 写入短整型。
    /// </summary>
    /// <remarks>
    /// Writes a short integer.
    /// </remarks>
    /// <param name="x">要写入的短整型值 / The short integer value to write</param>
    public void WriteShort(short x)
    {
        if (x >= 0)
        {
            if (x < 0x80)
            {
                EnsureWrite(1);
                Bytes[WriterIndex++] = (byte)x;
                return;
            }
            else if (x < 0x4000)
            {
                EnsureWrite(2);
                Bytes[WriterIndex + 1] = (byte)x;
                Bytes[WriterIndex] = (byte)((x >> 8) | 0x80);
                WriterIndex += 2;
                return;
            }
        }

        EnsureWrite(3);
        Bytes[WriterIndex] = 0xff;
        Bytes[WriterIndex + 2] = (byte)x;
        Bytes[WriterIndex + 1] = (byte)(x >> 8);
        WriterIndex += 3;
    }

    /// <summary>
    /// 读取短整型。
    /// </summary>
    /// <remarks>
    /// Reads a short integer.
    /// </remarks>
    /// <returns>读取的短整型值 / The read short integer value</returns>
    public short ReadShort()
    {
        EnsureRead(1);
        int h = Bytes[ReaderIndex];
        if (h < 0x80)
        {
            ReaderIndex++;
            return (short)h;
        }
        else if (h < 0xc0)
        {
            EnsureRead(2);
            var x = ((h & 0x3f) << 8) | Bytes[ReaderIndex + 1];
            ReaderIndex += 2;
            return (short)x;
        }
        else if (h == 0xff)
        {
            EnsureRead(3);
            var x = (Bytes[ReaderIndex + 1] << 8) | Bytes[ReaderIndex + 2];
            ReaderIndex += 3;
            return (short)x;
        }
        else
        {
            throw new SerializationException();
        }
    }

    /// <summary>
    /// 读取固定长度的短整型。
    /// </summary>
    /// <remarks>
    /// Reads a fixed-length short integer.
    /// </remarks>
    /// <returns>读取的短整型值 / The read short integer value</returns>
    public short ReadFshort()
    {
        EnsureRead(2);
        short x;
#if CPU_SUPPORT_MEMORY_NOT_ALIGN
                unsafe
                {
                    fixed (byte* b = &Bytes[ReaderIndex])
                    {
                        x = *(short*)b;
                    }
                }
#else
        x = (short)((Bytes[ReaderIndex + 1] << 8) | Bytes[ReaderIndex]);

#endif
        ReaderIndex += 2;
        return x;
    }

    /// <summary>
    /// 写入固定长度的短整型。
    /// </summary>
    /// <remarks>
    /// Writes a fixed-length short integer.
    /// </remarks>
    /// <param name="x">要写入的短整型值 / The short integer value to write</param>
    public void WriteFshort(short x)
    {
        EnsureWrite(2);
#if CPU_SUPPORT_MEMORY_NOT_ALIGN
                unsafe
                {
                    fixed (byte* b = &Bytes[WriterIndex])
                    {
                        *(short*)b = x;
                    }
                }
#else
        Bytes[WriterIndex] = (byte)x;
        Bytes[WriterIndex + 1] = (byte)(x >> 8);
#endif
        WriterIndex += 2;
    }

    /// <summary>
    /// 写入整型。
    /// </summary>
    /// <remarks>
    /// Writes an integer.
    /// </remarks>
    /// <param name="x">要写入的整型值 / The integer value to write</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WriteInt(int x)
    {
        WriteUint((uint)x);
    }

    /// <summary>
    /// 读取整型。
    /// </summary>
    /// <remarks>
    /// Reads an integer.
    /// </remarks>
    /// <returns>读取的整型值 / The read integer value</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int ReadInt()
    {
        return (int)ReadUint();
    }

    /// <summary>
    /// 写入无符号整型。
    /// </summary>
    /// <remarks>
    /// Writes an unsigned integer.
    /// </remarks>
    /// <param name="x">要写入的无符号整型值 / The unsigned integer value to write</param>
    public void WriteUint(uint x)
    {
        // 如果有修改，记得也把 EndWriteSegment改了
        // 0 111 1111
        if (x < 0x80)
        {
            EnsureWrite(1);
            Bytes[WriterIndex++] = (byte)x;
        }
        else if (x < 0x4000) // 10 11 1111, -
        {
            EnsureWrite(2);
            Bytes[WriterIndex + 1] = (byte)x;
            Bytes[WriterIndex] = (byte)((x >> 8) | 0x80);
            WriterIndex += 2;
        }
        else if (x < 0x200000) // 110 1 1111, -,-
        {
            EnsureWrite(3);
            Bytes[WriterIndex + 2] = (byte)x;
            Bytes[WriterIndex + 1] = (byte)(x >> 8);
            Bytes[WriterIndex] = (byte)((x >> 16) | 0xc0);
            WriterIndex += 3;
        }
        else if (x < 0x10000000) // 1110 1111,-,-,-
        {
            EnsureWrite(4);
            Bytes[WriterIndex + 3] = (byte)x;
            Bytes[WriterIndex + 2] = (byte)(x >> 8);
            Bytes[WriterIndex + 1] = (byte)(x >> 16);
            Bytes[WriterIndex] = (byte)((x >> 24) | 0xe0);
            WriterIndex += 4;
        }
        else
        {
            EnsureWrite(5);
            Bytes[WriterIndex] = 0xf0;
            Bytes[WriterIndex + 4] = (byte)x;
            Bytes[WriterIndex + 3] = (byte)(x >> 8);
            Bytes[WriterIndex + 2] = (byte)(x >> 16);
            Bytes[WriterIndex + 1] = (byte)(x >> 24);
            WriterIndex += 5;
        }
    }

    /// <summary>
    /// 读取无符号整型。
    /// </summary>
    /// <remarks>
    /// Reads an unsigned integer.
    /// </remarks>
    /// <returns>读取的无符号整型值 / The read unsigned integer value</returns>
    public uint ReadUint()
    {
        // 警告！ 如有修改，记得调整 TryDeserializeInplaceOctets
        EnsureRead(1);
        uint h = Bytes[ReaderIndex];
        if (h < 0x80)
        {
            ReaderIndex++;
            return h;
        }
        else if (h < 0xc0)
        {
            EnsureRead(2);
            var x = ((h & 0x3f) << 8) | Bytes[ReaderIndex + 1];
            ReaderIndex += 2;
            return x;
        }
        else if (h < 0xe0)
        {
            EnsureRead(3);
            var x = ((h & 0x1f) << 16) | ((uint)Bytes[ReaderIndex + 1] << 8) | Bytes[ReaderIndex + 2];
            ReaderIndex += 3;
            return x;
        }
        else if (h < 0xf0)
        {
            EnsureRead(4);
            var x = ((h & 0x0f) << 24) | ((uint)Bytes[ReaderIndex + 1] << 16) | ((uint)Bytes[ReaderIndex + 2] << 8) | Bytes[ReaderIndex + 3];
            ReaderIndex += 4;
            return x;
        }
        else
        {
            EnsureRead(5);
            var x = ((uint)Bytes[ReaderIndex + 1] << 24) | (uint)(Bytes[ReaderIndex + 2] << 16) | ((uint)Bytes[ReaderIndex + 3] << 8) | Bytes[ReaderIndex + 4];
            ReaderIndex += 5;
            return x;
        }
    }

    /// <summary>
    /// 使用不安全代码写入无符号整型。
    /// </summary>
    /// <remarks>
    /// Writes an unsigned integer using unsafe code.
    /// </remarks>
    /// <param name="x">要写入的无符号整型值 / The unsigned integer value to write</param>
    public unsafe void WriteUint_Unsafe(uint x)
    {
        // 0 111 1111
        if (x < 0x80)
        {
            EnsureWrite(1);
            Bytes[WriterIndex++] = (byte)(x << 1);
        }
        else if (x < 0x4000) // 10 11 1111, -
        {
            EnsureWrite(2);

            fixed (byte* wb = &Bytes[WriterIndex])
            {
                *(uint*)wb = (x << 2) | 0b01;
            }

            WriterIndex += 2;
        }
        else if (x < 0x200000) // 110 1 1111, -,-
        {
            EnsureWrite(3);

            fixed (byte* wb = &Bytes[WriterIndex])
            {
                *(uint*)wb = (x << 3) | 0b011;
            }

            WriterIndex += 3;
        }
        else if (x < 0x10000000) // 1110 1111,-,-,-
        {
            EnsureWrite(4);
            fixed (byte* wb = &Bytes[WriterIndex])
            {
                *(uint*)wb = (x << 4) | 0b0111;
            }

            WriterIndex += 4;
        }
        else
        {
            EnsureWrite(5);
            fixed (byte* wb = &Bytes[WriterIndex])
            {
                *(uint*)wb = (x << 5) | 0b01111;
            }

            WriterIndex += 5;
        }
    }

    /// <summary>
    /// 使用不安全代码读取无符号整型。
    /// </summary>
    /// <remarks>
    /// Reads an unsigned integer using unsafe code.
    /// </remarks>
    /// <returns>读取的无符号整型值 / The read unsigned integer value</returns>
    public unsafe uint ReadUint_Unsafe()
    {
        // 警告！ 如有修改，记得调整 TryDeserializeInplaceOctets
        EnsureRead(1);
        uint h = Bytes[ReaderIndex];
        if ((h & 0b1) == 0b0)
        {
            ReaderIndex++;
            return h >> 1;
        }
        else if ((h & 0b11) == 0b01)
        {
            EnsureRead(2);
            fixed (byte* rb = &Bytes[ReaderIndex])
            {
                ReaderIndex += 2;
                return *(uint*)rb >> 2;
            }
        }
        else if ((h & 0b111) == 0b011)
        {
            EnsureRead(3);
            fixed (byte* rb = &Bytes[ReaderIndex])
            {
                ReaderIndex += 3;
                return *(uint*)rb >> 3;
            }
        }
        else if ((h & 0b1111) == 0b0111)
        {
            EnsureRead(4);
            fixed (byte* rb = &Bytes[ReaderIndex])
            {
                ReaderIndex += 4;
                return *(uint*)rb >> 4;
            }
        }
        else
        {
            EnsureRead(5);
            fixed (byte* rb = &Bytes[ReaderIndex])
            {
                ReaderIndex += 5;
                return *(uint*)rb >> 5;
            }
        }
    }

    /// <summary>
    /// 读取固定长度的整型。
    /// </summary>
    /// <remarks>
    /// Reads a fixed-length integer.
    /// </remarks>
    /// <returns>读取的整型值 / The read integer value</returns>
    public int ReadFint()
    {
        EnsureRead(4);
        int x;
#if CPU_SUPPORT_MEMORY_NOT_ALIGN
                unsafe
                {
                    fixed (byte* b = &Bytes[ReaderIndex])
                    {
                        x = *(int*)b;
                    }
                }
#else
        x = (Bytes[ReaderIndex + 3] << 24) | (Bytes[ReaderIndex + 2] << 16) | (Bytes[ReaderIndex + 1] << 8) | Bytes[ReaderIndex];

#endif
        ReaderIndex += 4;
        return x;
    }

    /// <summary>
    /// 写入固定长度的整型。
    /// </summary>
    /// <remarks>
    /// Writes a fixed-length integer.
    /// </remarks>
    /// <param name="x">要写入的整型值 / The integer value to write</param>
    public void WriteFint(int x)
    {
        EnsureWrite(4);
#if CPU_SUPPORT_MEMORY_NOT_ALIGN
                unsafe
                {
                    fixed (byte* b = &Bytes[WriterIndex])
                    {
                        *(int*)b = x;
                    }
                }
#else
        Bytes[WriterIndex] = (byte)x;
        Bytes[WriterIndex + 1] = (byte)(x >> 8);
        Bytes[WriterIndex + 2] = (byte)(x >> 16);
        Bytes[WriterIndex + 3] = (byte)(x >> 24);
#endif
        WriterIndex += 4;
    }

    /// <summary>
    /// 安全地读取固定长度的整型。
    /// </summary>
    /// <remarks>
    /// Safely reads a fixed-length integer.
    /// </remarks>
    /// <returns>读取的整型值 / The read integer value</returns>
    public int ReadFint_Safe()
    {
        EnsureRead(4);
        int x;

        x = (Bytes[ReaderIndex + 3] << 24) | (Bytes[ReaderIndex + 2] << 16) | (Bytes[ReaderIndex + 1] << 8) | Bytes[ReaderIndex];

        ReaderIndex += 4;
        return x;
    }

    /// <summary>
    /// 安全地写入固定长度的整型。
    /// </summary>
    /// <remarks>
    /// Safely writes a fixed-length integer.
    /// </remarks>
    /// <param name="x">要写入的整型值 / The integer value to write</param>
    public void WriteFint_Safe(int x)
    {
        EnsureWrite(4);
        Bytes[WriterIndex] = (byte)x;
        Bytes[WriterIndex + 1] = (byte)(x >> 8);
        Bytes[WriterIndex + 2] = (byte)(x >> 16);
        Bytes[WriterIndex + 3] = (byte)(x >> 24);
        WriterIndex += 4;
    }

    /// <summary>
    /// 写入长整型。
    /// </summary>
    /// <remarks>
    /// Writes a long integer.
    /// </remarks>
    /// <param name="x">要写入的长整型值 / The long integer value to write</param>
    public void WriteLong(long x)
    {
        WriteUlong((ulong)x);
    }

    /// <summary>
    /// 读取长整型。
    /// </summary>
    /// <remarks>
    /// Reads a long integer.
    /// </remarks>
    /// <returns>读取的长整型值 / The read long integer value</returns>
    public long ReadLong()
    {
        return (long)ReadUlong();
    }

    /// <summary>
    /// 将浮点数作为长整型写入。
    /// </summary>
    /// <remarks>
    /// Writes a floating-point number as a long integer.
    /// </remarks>
    /// <param name="x">要写入的浮点数 / The floating-point number to write</param>
    public void WriteNumberAsLong(double x)
    {
        WriteLong((long)x);
    }

    /// <summary>
    /// 读取长整型并转换为浮点数。
    /// </summary>
    /// <remarks>
    /// Reads a long integer and converts it to a floating-point number.
    /// </remarks>
    /// <returns>读取的浮点数 / The read floating-point number</returns>
    public double ReadLongAsNumber()
    {
        return ReadLong();
    }

    private void WriteUlong(ulong x)
    {
        // 0 111 1111
        if (x < 0x80)
        {
            EnsureWrite(1);
            Bytes[WriterIndex++] = (byte)x;
        }
        else if (x < 0x4000) // 10 11 1111, -
        {
            EnsureWrite(2);
            Bytes[WriterIndex + 1] = (byte)x;
            Bytes[WriterIndex] = (byte)((x >> 8) | 0x80);
            WriterIndex += 2;
        }
        else if (x < 0x200000) // 110 1 1111, -,-
        {
            EnsureWrite(3);
            Bytes[WriterIndex + 2] = (byte)x;
            Bytes[WriterIndex + 1] = (byte)(x >> 8);
            Bytes[WriterIndex] = (byte)((x >> 16) | 0xc0);
            WriterIndex += 3;
        }
        else if (x < 0x10000000) // 1110 1111,-,-,-
        {
            EnsureWrite(4);
            Bytes[WriterIndex + 3] = (byte)x;
            Bytes[WriterIndex + 2] = (byte)(x >> 8);
            Bytes[WriterIndex + 1] = (byte)(x >> 16);
            Bytes[WriterIndex] = (byte)((x >> 24) | 0xe0);
            WriterIndex += 4;
        }
        else if (x < 0x800000000L) // 1111 0xxx,-,-,-,-
        {
            EnsureWrite(5);
            Bytes[WriterIndex + 4] = (byte)x;
            Bytes[WriterIndex + 3] = (byte)(x >> 8);
            Bytes[WriterIndex + 2] = (byte)(x >> 16);
            Bytes[WriterIndex + 1] = (byte)(x >> 24);
            Bytes[WriterIndex] = (byte)((x >> 32) | 0xf0);
            WriterIndex += 5;
        }
        else if (x < 0x40000000000L) // 1111 10xx, 
        {
            EnsureWrite(6);
            Bytes[WriterIndex + 5] = (byte)x;
            Bytes[WriterIndex + 4] = (byte)(x >> 8);
            Bytes[WriterIndex + 3] = (byte)(x >> 16);
            Bytes[WriterIndex + 2] = (byte)(x >> 24);
            Bytes[WriterIndex + 1] = (byte)(x >> 32);
            Bytes[WriterIndex] = (byte)((x >> 40) | 0xf8);
            WriterIndex += 6;
        }
        else if (x < 0x200000000000L) // 1111 110x,
        {
            EnsureWrite(7);
            Bytes[WriterIndex + 6] = (byte)x;
            Bytes[WriterIndex + 5] = (byte)(x >> 8);
            Bytes[WriterIndex + 4] = (byte)(x >> 16);
            Bytes[WriterIndex + 3] = (byte)(x >> 24);
            Bytes[WriterIndex + 2] = (byte)(x >> 32);
            Bytes[WriterIndex + 1] = (byte)(x >> 40);
            Bytes[WriterIndex] = (byte)((x >> 48) | 0xfc);
            WriterIndex += 7;
        }
        else if (x < 0x100000000000000L) // 1111 1110
        {
            EnsureWrite(8);
            Bytes[WriterIndex + 7] = (byte)x;
            Bytes[WriterIndex + 6] = (byte)(x >> 8);
            Bytes[WriterIndex + 5] = (byte)(x >> 16);
            Bytes[WriterIndex + 4] = (byte)(x >> 24);
            Bytes[WriterIndex + 3] = (byte)(x >> 32);
            Bytes[WriterIndex + 2] = (byte)(x >> 40);
            Bytes[WriterIndex + 1] = (byte)(x >> 48);
            Bytes[WriterIndex] = 0xfe;
            WriterIndex += 8;
        }
        else // 1111 1111
        {
            EnsureWrite(9);
            Bytes[WriterIndex] = 0xff;
            Bytes[WriterIndex + 8] = (byte)x;
            Bytes[WriterIndex + 7] = (byte)(x >> 8);
            Bytes[WriterIndex + 6] = (byte)(x >> 16);
            Bytes[WriterIndex + 5] = (byte)(x >> 24);
            Bytes[WriterIndex + 4] = (byte)(x >> 32);
            Bytes[WriterIndex + 3] = (byte)(x >> 40);
            Bytes[WriterIndex + 2] = (byte)(x >> 48);
            Bytes[WriterIndex + 1] = (byte)(x >> 56);
            WriterIndex += 9;
        }
    }

    /// <summary>
    /// 读取无符号长整型。
    /// </summary>
    /// <remarks>
    /// Reads an unsigned long integer.
    /// </remarks>
    /// <returns>读取的无符号长整型值 / The read unsigned long integer value</returns>
    public ulong ReadUlong()
    {
        EnsureRead(1);
        uint h = Bytes[ReaderIndex];
        if (h < 0x80)
        {
            ReaderIndex++;
            return h;
        }
        else if (h < 0xc0)
        {
            EnsureRead(2);
            var x = ((h & 0x3f) << 8) | Bytes[ReaderIndex + 1];
            ReaderIndex += 2;
            return x;
        }
        else if (h < 0xe0)
        {
            EnsureRead(3);
            var x = ((h & 0x1f) << 16) | ((uint)Bytes[ReaderIndex + 1] << 8) | Bytes[ReaderIndex + 2];
            ReaderIndex += 3;
            return x;
        }
        else if (h < 0xf0)
        {
            EnsureRead(4);
            var x = ((h & 0x0f) << 24) | ((uint)Bytes[ReaderIndex + 1] << 16) | ((uint)Bytes[ReaderIndex + 2] << 8) | Bytes[ReaderIndex + 3];
            ReaderIndex += 4;
            return x;
        }
        else if (h < 0xf8)
        {
            EnsureRead(5);
            var xl = ((uint)Bytes[ReaderIndex + 1] << 24) | (uint)(Bytes[ReaderIndex + 2] << 16) | ((uint)Bytes[ReaderIndex + 3] << 8) | Bytes[ReaderIndex + 4];
            var xh = h & 0x07;
            ReaderIndex += 5;
            return ((ulong)xh << 32) | xl;
        }
        else if (h < 0xfc)
        {
            EnsureRead(6);
            var xl = ((uint)Bytes[ReaderIndex + 2] << 24) | (uint)(Bytes[ReaderIndex + 3] << 16) | ((uint)Bytes[ReaderIndex + 4] << 8) | Bytes[ReaderIndex + 5];
            var xh = ((h & 0x03) << 8) | Bytes[ReaderIndex + 1];
            ReaderIndex += 6;
            return ((ulong)xh << 32) | xl;
        }
        else if (h < 0xfe)
        {
            EnsureRead(7);
            var xl = ((uint)Bytes[ReaderIndex + 3] << 24) | (uint)(Bytes[ReaderIndex + 4] << 16) | ((uint)Bytes[ReaderIndex + 5] << 8) | Bytes[ReaderIndex + 6];
            var xh = ((h & 0x01) << 16) | ((uint)Bytes[ReaderIndex + 1] << 8) | Bytes[ReaderIndex + 2];
            ReaderIndex += 7;
            return ((ulong)xh << 32) | xl;
        }
        else if (h < 0xff)
        {
            EnsureRead(8);
            var xl = ((uint)Bytes[ReaderIndex + 4] << 24) | (uint)(Bytes[ReaderIndex + 5] << 16) | ((uint)Bytes[ReaderIndex + 6] << 8) | Bytes[ReaderIndex + 7];
            var xh = /*((h & 0x01) << 24) |*/ ((uint)Bytes[ReaderIndex + 1] << 16) | ((uint)Bytes[ReaderIndex + 2] << 8) | Bytes[ReaderIndex + 3];
            ReaderIndex += 8;
            return ((ulong)xh << 32) | xl;
        }
        else
        {
            EnsureRead(9);
            var xl = ((uint)Bytes[ReaderIndex + 5] << 24) | (uint)(Bytes[ReaderIndex + 6] << 16) | ((uint)Bytes[ReaderIndex + 7] << 8) | Bytes[ReaderIndex + 8];
            var xh = ((uint)Bytes[ReaderIndex + 1] << 24) | ((uint)Bytes[ReaderIndex + 2] << 16) | ((uint)Bytes[ReaderIndex + 3] << 8) | Bytes[ReaderIndex + 4];
            ReaderIndex += 9;
            return ((ulong)xh << 32) | xl;
        }
    }

    /// <summary>
    /// 写入固定长度的长整型。
    /// </summary>
    /// <remarks>
    /// Writes a fixed-length long integer.
    /// </remarks>
    /// <param name="x">要写入的长整型值 / The long integer value to write</param>
    public void WriteFlong(long x)
    {
        EnsureWrite(8);
#if CPU_SUPPORT_MEMORY_NOT_ALIGN
                unsafe
                {
                    fixed (byte* b = &Bytes[WriterIndex])
                    {
                        *(long*)b = x;
                    }
                }
#else

        Bytes[WriterIndex] = (byte)x;
        Bytes[WriterIndex + 1] = (byte)(x >> 8);
        Bytes[WriterIndex + 2] = (byte)(x >> 16);
        Bytes[WriterIndex + 3] = (byte)(x >> 24);
        Bytes[WriterIndex + 4] = (byte)(x >> 32);
        Bytes[WriterIndex + 5] = (byte)(x >> 40);
        Bytes[WriterIndex + 6] = (byte)(x >> 48);
        Bytes[WriterIndex + 7] = (byte)(x >> 56);
#endif
        WriterIndex += 8;
    }

    /// <summary>
    /// 读取固定长度的长整型。
    /// </summary>
    /// <remarks>
    /// Reads a fixed-length long integer.
    /// </remarks>
    /// <returns>读取的长整型值 / The read long integer value</returns>
    public long ReadFlong()
    {
        EnsureRead(8);
        long x;
#if CPU_SUPPORT_MEMORY_NOT_ALIGN
                unsafe
                {
                    fixed (byte* b = &Bytes[ReaderIndex])
                    {
                        x = *(long*)b;
                    }
                }
#else
        var xl = (Bytes[ReaderIndex + 3] << 24) | (Bytes[ReaderIndex + 2] << 16) | (Bytes[ReaderIndex + 1] << 8) | Bytes[ReaderIndex];
        var xh = (Bytes[ReaderIndex + 7] << 24) | (Bytes[ReaderIndex + 6] << 16) | (Bytes[ReaderIndex + 5] << 8) | Bytes[ReaderIndex + 4];
        x = ((long)xh << 32) | (long)xl;
#endif
        ReaderIndex += 8;
        return x;
    }

    private static unsafe void Copy8(byte* dst, byte* src)
    {
        dst[0] = src[0];
        dst[1] = src[1];
        dst[2] = src[2];
        dst[3] = src[3];
        dst[4] = src[4];
        dst[5] = src[5];
        dst[6] = src[6];
        dst[7] = src[7];
    }

    private static unsafe void Copy4(byte* dst, byte* src)
    {
        dst[0] = src[0];
        dst[1] = src[1];
        dst[2] = src[2];
        dst[3] = src[3];
    }

    /// <summary>
    /// 写入浮点数。
    /// </summary>
    /// <remarks>
    /// Writes a floating-point number.
    /// </remarks>
    /// <param name="x">要写入的浮点数 / The floating-point number to write</param>
    public void WriteFloat(float x)
    {
        EnsureWrite(4);
        unsafe
        {
            fixed (byte* b = &Bytes[WriterIndex])
            {
#if !CPU_SUPPORT_MEMORY_NOT_ALIGN
                if ((long)b % 4 == 0)
                {
                    *(float*)b = x;
                }
                else
                {
                    Copy4(b, (byte*)&x);
                }
#else
                        *(float*)b = x;
#endif
            }
        }

        //if (!BitConverter.IsLittleEndian)
        //{
        //    Array.Reverse(data, endPos, 4);
        //}
        WriterIndex += 4;
    }

    /// <summary>
    /// 读取浮点数。
    /// </summary>
    /// <remarks>
    /// Reads a floating-point number.
    /// </remarks>
    /// <returns>读取的浮点数 / The read floating-point number</returns>
    public float ReadFloat()
    {
        EnsureRead(4);
        //if (!BitConverter.IsLittleEndian)
        //{
        //    Array.Reverse(data, beginPos, 4);
        //}
        float x;
        unsafe
        {
            fixed (byte* b = &Bytes[ReaderIndex])
            {
#if !CPU_SUPPORT_MEMORY_NOT_ALIGN
                if ((long)b % 4 == 0)
                {
                    x = *(float*)b;
                }
                else
                {
                    *(int*)&x = b[0] | (b[1] << 8) | (b[2] << 16) | (b[3] << 24);
                }
#else
                        x = *(float*)b;
#endif
            }
        }

        ReaderIndex += 4;
        return x;
    }

    /// <summary>
    /// 写入双精度浮点数。
    /// </summary>
    /// <remarks>
    /// Writes a double-precision floating-point number.
    /// </remarks>
    /// <param name="x">要写入的双精度浮点数 / The double-precision floating-point number to write</param>
    public void WriteDouble(double x)
    {
        EnsureWrite(8);
        unsafe
        {
            fixed (byte* b = &Bytes[WriterIndex])
            {
#if !CPU_SUPPORT_MEMORY_NOT_ALIGN
                if ((long)b % 8 == 0)
                {
                    *(double*)b = x;
                }
                else
                {
                    Copy8(b, (byte*)&x);
                }
#else
                        *(double*)b = x;
#endif
            }
            //if (!BitConverter.IsLittleEndian)
            //{
            //    Array.Reverse(data, endPos, 8);
            //}
        }

        WriterIndex += 8;
    }

    /// <summary>
    /// 读取双精度浮点数。
    /// </summary>
    /// <remarks>
    /// Reads a double-precision floating-point number.
    /// </remarks>
    /// <returns>读取的双精度浮点数 / The read double-precision floating-point number</returns>
    public double ReadDouble()
    {
        EnsureRead(8);
        //if (!BitConverter.IsLittleEndian)
        //{
        //    Array.Reverse(data, beginPos, 8);
        //}
        double x;
        unsafe
        {
            fixed (byte* b = &Bytes[ReaderIndex])
            {
#if !CPU_SUPPORT_MEMORY_NOT_ALIGN
                if ((long)b % 8 == 0)
                {
                    x = *(double*)b;
                }
                else
                {
                    var low = b[0] | (b[1] << 8) | (b[2] << 16) | (b[3] << 24);
                    var high = b[4] | (b[5] << 8) | (b[6] << 16) | (b[7] << 24);
                    *(long*)&x = ((long)high << 32) | (uint)low;
                }
#else
                        x = *(double*)b;
#endif
            }
        }

        ReaderIndex += 8;
        return x;
    }

    /// <summary>
    /// 写入大小。
    /// </summary>
    /// <remarks>
    /// Writes a size value.
    /// </remarks>
    /// <param name="n">要写入的大小 / The size to write</param>
    public void WriteSize(int n)
    {
        WriteUint((uint)n);
    }

    /// <summary>
    /// 读取大小。
    /// </summary>
    /// <remarks>
    /// Reads a size value.
    /// </remarks>
    /// <returns>读取的大小 / The read size value</returns>
    public int ReadSize()
    {
        return (int)ReadUint();
    }

    /// <summary>
    /// 写入有符号整型。
    /// </summary>
    /// <remarks>
    /// Writes a signed integer.
    /// </remarks>
    /// <param name="x">要写入的有符号整型值 / The signed integer value to write</param>
    public void WriteSint(int x)
    {
        WriteUint(((uint)x << 1) ^ ((uint)x >> 31));
    }

    /// <summary>
    /// 读取有符号整型。
    /// </summary>
    /// <remarks>
    /// Reads a signed integer.
    /// </remarks>
    /// <returns>读取的有符号整型值 / The read signed integer value</returns>
    public int ReadSint()
    {
        var x = ReadUint();
        return (int)((x >> 1) ^ ((x & 1) << 31));
    }


    /// <summary>
    /// 写入有符号长整型。
    /// </summary>
    /// <remarks>
    /// Writes a signed long integer.
    /// </remarks>
    /// <param name="x">要写入的长整型值 / The long integer value to write</param>
    public void WriteSlong(long x)
    {
        WriteUlong(((ulong)x << 1) ^ ((ulong)x >> 63));
    }

    /// <summary>
    /// 读取有符号长整型。
    /// </summary>
    /// <remarks>
    /// Reads a signed long integer.
    /// </remarks>
    /// <returns>读取的长整型值 / The read long integer value</returns>
    public long ReadSlong()
    {
        var x = ReadLong();
        return (long)((ulong)x >> 1) ^ ((x & 1) << 63);
    }

    /// <summary>
    /// 写入字符串。
    /// </summary>
    /// <remarks>
    /// Writes a string.
    /// </remarks>
    /// <param name="x">要写入的字符串 / The string to write</param>
    public void WriteString(string x)
    {
        var n = x != null ? Encoding.UTF8.GetByteCount(x) : 0;
        WriteSize(n);
        if (n > 0)
        {
            EnsureWrite(n);
            Encoding.UTF8.GetBytes(x, 0, x.Length, Bytes, WriterIndex);
            WriterIndex += n;
        }
    }

    /// <summary>
    /// 获取或设置字符串缓存查找器。
    /// </summary>
    /// <remarks>
    /// Gets or sets the string cache finder.
    /// </remarks>
    /// <value>字符串缓存查找器委托 / The string cache finder delegate</value>
    public static Func<byte[], int, int, string> StringCacheFinder { get; set; }

    /// <summary>
    /// 读取字符串。
    /// </summary>
    /// <remarks>
    /// Reads a string.
    /// </remarks>
    /// <returns>读取的字符串 / The read string</returns>
    public string ReadString()
    {
        var n = ReadSize();
        if (n > 0)
        {
            EnsureRead(n);
            string s;

            if (StringCacheFinder == null)
            {
                s = Encoding.UTF8.GetString(Bytes, ReaderIndex, n);
            }
            else
            {
                // 只缓存比较小的字符串
                s = StringCacheFinder(Bytes, ReaderIndex, n);
            }

            ReaderIndex += n;
            return s;
        }
        else
        {
            return string.Empty;
        }
    }

    /// <summary>
    /// 写入字节数组。
    /// </summary>
    /// <remarks>
    /// Writes a byte array.
    /// </remarks>
    /// <param name="x">要写入的字节数组 / The byte array to write</param>
    public void WriteBytes(byte[] x)
    {
        var n = x != null ? x.Length : 0;
        WriteSize(n);
        if (n > 0)
        {
            EnsureWrite(n);
            x.CopyTo(Bytes, WriterIndex);
            WriterIndex += n;
        }
    }

    /// <summary>
    /// 读取字节数组。
    /// </summary>
    /// <remarks>
    /// Reads a byte array.
    /// </remarks>
    /// <returns>读取的字节数组 / The read byte array</returns>
    public byte[] ReadBytes()
    {
        var n = ReadSize();
        if (n > 0)
        {
            EnsureRead(n);
            var x = new byte[n];
            Buffer.BlockCopy(Bytes, ReaderIndex, x, 0, n);
            ReaderIndex += n;
            return x;
        }
        else
        {
            return Array.Empty<byte>();
        }
    }

    /// <summary>
    /// 写入复数。
    /// </summary>
    /// <remarks>
    /// Writes a complex number.
    /// </remarks>
    /// <param name="x">要写入的复数 / The complex number to write</param>
    public void WriteComplex(Complex x)
    {
        WriteDouble(x.Real);
        WriteDouble(x.Imaginary);
    }

    /// <summary>
    /// 读取复数。
    /// </summary>
    /// <remarks>
    /// Reads a complex number.
    /// </remarks>
    /// <returns>读取的复数 / The read complex number</returns>
    public Complex ReadComplex()
    {
        var x = ReadDouble();
        var y = ReadDouble();
        return new Complex(x, y);
    }

    /// <summary>
    /// 写入二维向量。
    /// </summary>
    /// <remarks>
    /// Writes a 2D vector.
    /// </remarks>
    /// <param name="x">要写入的二维向量 / The 2D vector to write</param>
    public void WriteVector2(Vector2 x)
    {
        WriteFloat(x.X);
        WriteFloat(x.Y);
    }

    /// <summary>
    /// 读取二维向量。
    /// </summary>
    /// <remarks>
    /// Reads a 2D vector.
    /// </remarks>
    /// <returns>读取的二维向量 / The read 2D vector</returns>
    public Vector2 ReadVector2()
    {
        var x = ReadFloat();
        var y = ReadFloat();
        return new Vector2(x, y);
    }

    /// <summary>
    /// 写入三维向量。
    /// </summary>
    /// <remarks>
    /// Writes a 3D vector.
    /// </remarks>
    /// <param name="x">要写入的三维向量 / The 3D vector to write</param>
    public void WriteVector3(Vector3 x)
    {
        WriteFloat(x.X);
        WriteFloat(x.Y);
        WriteFloat(x.Z);
    }

    /// <summary>
    /// 读取三维向量。
    /// </summary>
    /// <remarks>
    /// Reads a 3D vector.
    /// </remarks>
    /// <returns>读取的三维向量 / The read 3D vector</returns>
    public Vector3 ReadVector3()
    {
        var x = ReadFloat();
        var y = ReadFloat();
        var z = ReadFloat();
        return new Vector3(x, y, z);
    }

    /// <summary>
    /// 写入四维向量。
    /// </summary>
    /// <remarks>
    /// Writes a 4D vector.
    /// </remarks>
    /// <param name="x">要写入的四维向量 / The 4D vector to write</param>
    public void WriteVector4(Vector4 x)
    {
        WriteFloat(x.X);
        WriteFloat(x.Y);
        WriteFloat(x.Z);
        WriteFloat(x.W);
    }

    /// <summary>
    /// 读取四维向量。
    /// </summary>
    /// <remarks>
    /// Reads a 4D vector.
    /// </remarks>
    /// <returns>读取的四维向量 / The read 4D vector</returns>
    public Vector4 ReadVector4()
    {
        var x = ReadFloat();
        var y = ReadFloat();
        var z = ReadFloat();
        var w = ReadFloat();
        return new Vector4(x, y, z, w);
    }


    /// <summary>
    /// 写入四元数。
    /// </summary>
    /// <remarks>
    /// Writes a quaternion.
    /// </remarks>
    /// <param name="x">要写入的四元数 / The quaternion to write</param>
    public void WriteQuaternion(Quaternion x)
    {
        WriteFloat(x.X);
        WriteFloat(x.Y);
        WriteFloat(x.Z);
        WriteFloat(x.W);
    }

    /// <summary>
    /// 读取四元数。
    /// </summary>
    /// <remarks>
    /// Reads a quaternion.
    /// </remarks>
    /// <returns>读取的四元数 / The read quaternion</returns>
    public Quaternion ReadQuaternion()
    {
        var x = ReadFloat();
        var y = ReadFloat();
        var z = ReadFloat();
        var w = ReadFloat();
        return new Quaternion(x, y, z, w);
    }


    /// <summary>
    /// 写入4x4矩阵。
    /// </summary>
    /// <remarks>
    /// Writes a 4x4 matrix.
    /// </remarks>
    /// <param name="x">要写入的4x4矩阵 / The 4x4 matrix to write</param>
    public void WriteMatrix4x4(Matrix4x4 x)
    {
        WriteFloat(x.M11);
        WriteFloat(x.M12);
        WriteFloat(x.M13);
        WriteFloat(x.M14);
        WriteFloat(x.M21);
        WriteFloat(x.M22);
        WriteFloat(x.M23);
        WriteFloat(x.M24);
        WriteFloat(x.M31);
        WriteFloat(x.M32);
        WriteFloat(x.M33);
        WriteFloat(x.M34);
        WriteFloat(x.M41);
        WriteFloat(x.M42);
        WriteFloat(x.M43);
        WriteFloat(x.M44);
    }

    /// <summary>
    /// 读取4x4矩阵。
    /// </summary>
    /// <remarks>
    /// Reads a 4x4 matrix.
    /// </remarks>
    /// <returns>读取的4x4矩阵 / The read 4x4 matrix</returns>
    public Matrix4x4 ReadMatrix4x4()
    {
        var m11 = ReadFloat();
        var m12 = ReadFloat();
        var m13 = ReadFloat();
        var m14 = ReadFloat();
        var m21 = ReadFloat();
        var m22 = ReadFloat();
        var m23 = ReadFloat();
        var m24 = ReadFloat();
        var m31 = ReadFloat();
        var m32 = ReadFloat();
        var m33 = ReadFloat();
        var m34 = ReadFloat();
        var m41 = ReadFloat();
        var m42 = ReadFloat();
        var m43 = ReadFloat();
        var m44 = ReadFloat();
        return new Matrix4x4(m11, m12, m13, m14,
                             m21, m22, m23, m24,
                             m31, m32, m33, m34,
                             m41, m42, m43, m44);
    }

    /// <summary>
    /// 跳过字节。
    /// </summary>
    /// <remarks>
    /// Skips bytes.
    /// </remarks>
    internal void SkipBytes()
    {
        var n = ReadSize();
        EnsureRead(n);
        ReaderIndex += n;
    }


    /// <summary>
    /// 写入字节缓冲区（包含大小）。
    /// </summary>
    /// <remarks>
    /// Writes a byte buffer with size.
    /// </remarks>
    /// <param name="o">要写入的字节缓冲区 / The byte buffer to write</param>
    public void WriteByteBufWithSize(ByteBuf o)
    {
        var n = o.Size;
        if (n > 0)
        {
            WriteSize(n);
            WriteBytesWithoutSize(o.Bytes, o.ReaderIndex, n);
        }
        else
        {
            WriteByte(0);
        }
    }

    /// <summary>
    /// 写入字节缓冲区（不包含大小）。
    /// </summary>
    /// <remarks>
    /// Writes a byte buffer without size.
    /// </remarks>
    /// <param name="o">要写入的字节缓冲区 / The byte buffer to write</param>
    public void WriteByteBufWithoutSize(ByteBuf o)
    {
        var n = o.Size;
        if (n > 0)
        {
            WriteBytesWithoutSize(o.Bytes, o.ReaderIndex, n);
        }
    }

    /// <summary>
    /// 尝试读取字节。
    /// </summary>
    /// <remarks>
    /// Tries to read a byte.
    /// </remarks>
    /// <param name="x">要读取的字节 / The byte to read</param>
    /// <returns>是否成功读取 / Whether the read was successful</returns>
    public bool TryReadByte(out byte x)
    {
        if (CanRead(1))
        {
            x = Bytes[ReaderIndex++];
            return true;
        }
        else
        {
            x = 0;
            return false;
        }
    }

    /// <summary>
    /// 尝试反序列化字节缓冲区。
    /// </summary>
    /// <remarks>
    /// Tries to deserialize a byte buffer in place.
    /// </remarks>
    /// <param name="maxSize">最大大小 / The maximum size</param>
    /// <param name="inplaceTempBody">临时字节缓冲区 / The temporary byte buffer</param>
    /// <returns>反序列化错误 / The deserialization error</returns>
    public EDeserializeError TryDeserializeInplaceByteBuf(int maxSize, ByteBuf inplaceTempBody)
    {
        //if (!CanRead(1)) { return EDeserializeError.NOT_ENOUGH; }
        var oldReadIndex = ReaderIndex;
        var commit = false;
        try
        {
            int n;
            int h = Bytes[ReaderIndex];
            if (h < 0x80)
            {
                ReaderIndex++;
                n = h;
            }
            else if (h < 0xc0)
            {
                if (!CanRead(2))
                {
                    return EDeserializeError.NOT_ENOUGH;
                }

                n = ((h & 0x3f) << 8) | Bytes[ReaderIndex + 1];
                ReaderIndex += 2;
            }
            else if (h < 0xe0)
            {
                if (!CanRead(3))
                {
                    return EDeserializeError.NOT_ENOUGH;
                }

                n = ((h & 0x1f) << 16) | (Bytes[ReaderIndex + 1] << 8) | Bytes[ReaderIndex + 2];
                ReaderIndex += 3;
            }
            else if (h < 0xf0)
            {
                if (!CanRead(4))
                {
                    return EDeserializeError.NOT_ENOUGH;
                }

                n = ((h & 0x0f) << 24) | (Bytes[ReaderIndex + 1] << 16) | (Bytes[ReaderIndex + 2] << 8) | Bytes[ReaderIndex + 3];
                ReaderIndex += 4;
            }
            else
            {
                return EDeserializeError.EXCEED_SIZE;
            }

            if (n > maxSize)
            {
                return EDeserializeError.EXCEED_SIZE;
            }

            if (Remaining < n)
            {
                return EDeserializeError.NOT_ENOUGH;
            }

            var inplaceReadIndex = ReaderIndex;
            ReaderIndex += n;

            inplaceTempBody.Replace(Bytes, inplaceReadIndex, ReaderIndex);
            commit = true;
        }
        finally
        {
            if (!commit)
            {
                ReaderIndex = oldReadIndex;
            }
        }

        return EDeserializeError.OK;
    }

    /// <summary>
    /// 写入原始标签（单字节）。
    /// </summary>
    /// <remarks>
    /// Writes a raw tag (single byte).
    /// </remarks>
    /// <param name="b1">要写入的标签 / The tag to write</param>
    public void WriteRawTag(byte b1)
    {
        EnsureWrite(1);
        Bytes[WriterIndex++] = b1;
    }

    /// <summary>
    /// 写入原始标签（双字节）。
    /// </summary>
    /// <remarks>
    /// Writes a raw tag (two bytes).
    /// </remarks>
    /// <param name="b1">要写入的第一个标签 / The first tag to write</param>
    /// <param name="b2">要写入的第二个标签 / The second tag to write</param>
    public void WriteRawTag(byte b1, byte b2)
    {
        EnsureWrite(2);
        Bytes[WriterIndex] = b1;
        Bytes[WriterIndex + 1] = b2;
        WriterIndex += 2;
    }

    /// <summary>
    /// 写入原始标签（三字节）。
    /// </summary>
    /// <remarks>
    /// Writes a raw tag (three bytes).
    /// </remarks>
    /// <param name="b1">要写入的第一个标签 / The first tag to write</param>
    /// <param name="b2">要写入的第二个标签 / The second tag to write</param>
    /// <param name="b3">要写入的第三个标签 / The third tag to write</param>
    public void WriteRawTag(byte b1, byte b2, byte b3)
    {
        EnsureWrite(3);
        Bytes[WriterIndex] = b1;
        Bytes[WriterIndex + 1] = b2;
        Bytes[WriterIndex + 2] = b3;
        WriterIndex += 3;
    }

    #region segment

    /// <summary>
    /// 开始写入段。
    /// </summary>
    /// <remarks>
    /// Begins writing a segment.
    /// </remarks>
    /// <param name="oldSize">旧大小 / The old size</param>
    public void BeginWriteSegment(out int oldSize)
    {
        oldSize = Size;
        EnsureWrite(1);
        WriterIndex += 1;
    }

    /// <summary>
    /// 结束写入段。
    /// </summary>
    /// <remarks>
    /// Ends writing a segment.
    /// </remarks>
    /// <param name="oldSize">旧大小 / The old size</param>
    public void EndWriteSegment(int oldSize)
    {
        var startPos = ReaderIndex + oldSize;
        var segmentSize = WriterIndex - startPos - 1;

        // 0 111 1111
        if (segmentSize < 0x80)
        {
            Bytes[startPos] = (byte)segmentSize;
        }
        else if (segmentSize < 0x4000) // 10 11 1111, -
        {
            EnsureWrite(1);
            Bytes[WriterIndex] = Bytes[startPos + 1];
            Bytes[startPos + 1] = (byte)segmentSize;

            Bytes[startPos] = (byte)((segmentSize >> 8) | 0x80);
            WriterIndex += 1;
        }
        else if (segmentSize < 0x200000) // 110 1 1111, -,-
        {
            EnsureWrite(2);
            Bytes[WriterIndex + 1] = Bytes[startPos + 2];
            Bytes[startPos + 2] = (byte)segmentSize;

            Bytes[WriterIndex] = Bytes[startPos + 1];
            Bytes[startPos + 1] = (byte)(segmentSize >> 8);

            Bytes[startPos] = (byte)((segmentSize >> 16) | 0xc0);
            WriterIndex += 2;
        }
        else if (segmentSize < 0x10000000) // 1110 1111,-,-,-
        {
            EnsureWrite(3);
            Bytes[WriterIndex + 2] = Bytes[startPos + 3];
            Bytes[startPos + 3] = (byte)segmentSize;

            Bytes[WriterIndex + 1] = Bytes[startPos + 2];
            Bytes[startPos + 2] = (byte)(segmentSize >> 8);

            Bytes[WriterIndex] = Bytes[startPos + 1];
            Bytes[startPos + 1] = (byte)(segmentSize >> 16);

            Bytes[startPos] = (byte)((segmentSize >> 24) | 0xe0);
            WriterIndex += 3;
        }
        else
        {
            throw new SerializationException("exceed max segment size");
        }
    }

    /// <summary>
    /// 读取段。
    /// </summary>
    /// <remarks>
    /// Reads a segment.
    /// </remarks>
    /// <param name="startIndex">开始索引 / The start index</param>
    /// <param name="segmentSize">段大小 / The segment size</param>
    public void ReadSegment(out int startIndex, out int segmentSize)
    {
        EnsureRead(1);
        int h = Bytes[ReaderIndex++];

        startIndex = ReaderIndex;

        if (h < 0x80)
        {
            segmentSize = h;
            ReaderIndex += segmentSize;
        }
        else if (h < 0xc0)
        {
            EnsureRead(1);
            segmentSize = ((h & 0x3f) << 8) | Bytes[ReaderIndex];
            var endPos = ReaderIndex + segmentSize;
            Bytes[ReaderIndex] = Bytes[endPos];
            ReaderIndex += segmentSize + 1;
        }
        else if (h < 0xe0)
        {
            EnsureRead(2);
            segmentSize = ((h & 0x1f) << 16) | ((int)Bytes[ReaderIndex] << 8) | Bytes[ReaderIndex + 1];
            var endPos = ReaderIndex + segmentSize;
            Bytes[ReaderIndex] = Bytes[endPos];
            Bytes[ReaderIndex + 1] = Bytes[endPos + 1];
            ReaderIndex += segmentSize + 2;
        }
        else if (h < 0xf0)
        {
            EnsureRead(3);
            segmentSize = ((h & 0x0f) << 24) | ((int)Bytes[ReaderIndex] << 16) | ((int)Bytes[ReaderIndex + 1] << 8) | Bytes[ReaderIndex + 2];
            var endPos = ReaderIndex + segmentSize;
            Bytes[ReaderIndex] = Bytes[endPos];
            Bytes[ReaderIndex + 1] = Bytes[endPos + 1];
            Bytes[ReaderIndex + 2] = Bytes[endPos + 2];
            ReaderIndex += segmentSize + 3;
        }
        else
        {
            throw new SerializationException("exceed max size");
        }

        if (ReaderIndex > WriterIndex)
        {
            throw new SerializationException("segment data not enough");
        }
    }

    /// <summary>
    /// 读取段到指定的字节缓冲区。
    /// </summary>
    /// <remarks>
    /// Reads a segment into the specified byte buffer.
    /// </remarks>
    /// <param name="buf">要读取的段 / The segment to read</param>
    public void ReadSegment(ByteBuf buf)
    {
        ReadSegment(out var startPos, out var size);
        buf.Bytes = Bytes;
        buf.ReaderIndex = startPos;
        buf.WriterIndex = startPos + size;
    }

    /// <summary>
    /// 进入段。
    /// </summary>
    /// <remarks>
    /// Enters a segment.
    /// </remarks>
    /// <param name="saveState">段保存状态 / The segment save state</param>
    public void EnterSegment(out SegmentSaveState saveState)
    {
        ReadSegment(out var startPos, out var size);

        saveState = new SegmentSaveState(ReaderIndex, WriterIndex);
        ReaderIndex = startPos;
        WriterIndex = startPos + size;
    }

    /// <summary>
    /// 离开段。
    /// </summary>
    /// <remarks>
    /// Leaves a segment.
    /// </remarks>
    /// <param name="saveState">段保存状态 / The segment save state</param>
    public void LeaveSegment(SegmentSaveState saveState)
    {
        ReaderIndex = saveState.ReaderIndex;
        WriterIndex = saveState.WriterIndex;
    }

    #endregion

    /// <summary>
    /// 转换为字符串。
    /// </summary>
    /// <remarks>
    /// Converts to a string.
    /// </remarks>
    /// <returns>字符串 / The string</returns>
    public override string ToString()
    {
        var datas = new string[WriterIndex - ReaderIndex];
        for (var i = ReaderIndex; i < WriterIndex; i++)
        {
            datas[i - ReaderIndex] = Bytes[i].ToString("X2");
        }

        return string.Join(".", datas);
    }

    /// <summary>
    /// 比较是否相等。
    /// </summary>
    /// <remarks>
    /// Compares for equality.
    /// </remarks>
    /// <param name="obj">要比较的对象 / The object to compare</param>
    /// <returns>是否相等 / Whether equal</returns>
    public override bool Equals(object obj)
    {
        return obj is ByteBuf other && Equals(other);
    }

    /// <summary>
    /// 比较是否相等。
    /// </summary>
    /// <remarks>
    /// Compares for equality with another ByteBuf.
    /// </remarks>
    /// <param name="other">要比较的对象 / The object to compare</param>
    /// <returns>是否相等 / Whether equal</returns>
    public bool Equals(ByteBuf other)
    {
        if (other == null)
        {
            return false;
        }

        if (Size != other.Size)
        {
            return false;
        }

        for (int i = 0, n = Size; i < n; i++)
        {
            if (Bytes[ReaderIndex + i] != other.Bytes[other.ReaderIndex + i])
            {
                return false;
            }
        }

        return true;
    }

    /// <summary>
    /// 克隆。
    /// </summary>
    /// <remarks>
    /// Clones the buffer.
    /// </remarks>
    /// <returns>克隆的对象 / The cloned object</returns>
    public object Clone()
    {
        return new ByteBuf(CopyData());
    }


    /// <summary>
    /// 从字符串创建字节缓冲区。
    /// </summary>
    /// <remarks>
    /// Creates a byte buffer from a string.
    /// </remarks>
    /// <param name="value">要创建的字符串 / The string to create from</param>
    /// <returns>字节缓冲区 / The byte buffer</returns>
    public static ByteBuf FromString(string value)
    {
        var ss = value.Split(',');
        var data = new byte[ss.Length];
        for (var i = 0; i < data.Length; i++)
        {
            data[i] = byte.Parse(ss[i]);
        }

        return new ByteBuf(data);
    }

    /// <summary>
    /// 获取哈希码。
    /// </summary>
    /// <remarks>
    /// Gets the hash code.
    /// </remarks>
    /// <returns>哈希码 / The hash code</returns>
    public override int GetHashCode()
    {
        var hash = 17;
        for (var i = ReaderIndex; i < WriterIndex; i++)
        {
            hash = hash * 23 + Bytes[i];
        }

        return hash;
    }

    /// <summary>
    /// 释放缓冲区。
    /// </summary>
    /// <remarks>
    /// Releases the buffer.
    /// </remarks>
    public void Release()
    {
        _releaser?.Invoke(this);
    }

#if SUPPORT_PUERTS_ARRAYBUF
        // -- add for puerts
        /// <summary>
        /// 读取数组缓冲区
        /// </summary>
        /// <returns>数组缓冲区</returns>
        public Puerts.ArrayBuffer ReadArrayBuffer()
        {
            return new Puerts.ArrayBuffer(ReadBytes());
        }

        /// <summary>
        /// 写入数组缓冲区
        /// </summary>
        /// <param name="bytes">要写入的数组缓冲区</param>
        public void WriteArrayBuffer(Puerts.ArrayBuffer bytes)
        {
            WriteBytes(bytes.Bytes);
        }
#endif
}