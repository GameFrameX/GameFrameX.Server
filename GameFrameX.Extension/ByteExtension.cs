using System.Buffers.Binary;
using System.Text;

namespace GameFrameX.Extension
{
    public static class ByteExtension
    {
        /// <summary>
        /// 整型变量（32 位）的字节数。
        /// </summary>
        private const int IntSize = sizeof(int);

        /// <summary>
        /// 短整型变量（16 位）的字节数。
        /// </summary>
        private const int ShortSize = sizeof(short);

        /// <summary>
        /// 长整型变量（64 位）的字节数。
        /// </summary>
        private const int LongSize = sizeof(long);

        /// <summary>
        /// 单精度浮点型变量的字节数。
        /// </summary>
        private const int FloatSize = sizeof(float);

        /// <summary>
        /// 双精度浮点型变量的字节数。
        /// </summary>
        private const int DoubleSize = sizeof(double);

        /// <summary>
        /// 字节型变量（8 位）的字节数。
        /// </summary>
        private const int ByteSize = sizeof(byte);

        /// <summary>
        /// 有符号字节类型变量的字节数。
        /// </summary>
        private const int SbyteSize = sizeof(sbyte);

        /// <summary>
        /// 布尔型变量的字节数。
        /// </summary>
        private const int BoolSize = sizeof(bool);

        private const int UIntSize = sizeof(uint);
        private const int UShortSize = sizeof(ushort);
        private const int ULongSize = sizeof(ulong);

        public static string ToHex(this byte b)
        {
            return b.ToString("X2");
        }

        /// <summary>
        /// 将字节数组转换为字符串
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static string ToArrayString(this byte[] bytes)
        {
            StringBuilder stringBuilder = new StringBuilder();
            foreach (byte b in bytes)
            {
                stringBuilder.Append(b + " ");
            }

            return stringBuilder.ToString();
        }

        public static string ToHex(this byte[] bytes)
        {
            StringBuilder stringBuilder = new StringBuilder();
            foreach (byte b in bytes)
            {
                stringBuilder.Append(b.ToString("X2"));
            }

            return stringBuilder.ToString();
        }

        public static string ToHex(this byte[] bytes, string format)
        {
            StringBuilder stringBuilder = new StringBuilder();
            foreach (byte b in bytes)
            {
                stringBuilder.Append(b.ToString(format));
            }

            return stringBuilder.ToString();
        }

        public static string ToHex(this byte[] bytes, int offset, int count)
        {
            StringBuilder stringBuilder = new StringBuilder();
            for (int i = offset; i < offset + count; ++i)
            {
                stringBuilder.Append(bytes[i].ToString("X2"));
            }

            return stringBuilder.ToString();
        }

        public static string ToStr(this byte[] bytes)
        {
            return Encoding.Default.GetString(bytes);
        }

        public static string ToStr(this byte[] bytes, int index, int count)
        {
            return Encoding.Default.GetString(bytes, index, count);
        }

        public static string Utf8ToStr(this byte[] bytes)
        {
            return Encoding.UTF8.GetString(bytes);
        }

        public static string Utf8ToStr(this byte[] bytes, int index, int count)
        {
            return Encoding.UTF8.GetString(bytes, index, count);
        }

        /// <summary>
        /// 将一个32位无符号整数写入指定的缓冲区，并更新偏移量。
        /// </summary>
        /// <param name="buffer">要写入的缓冲区。</param>
        /// <param name="value">要写入的值。</param>
        /// <param name="offset">要写入值的缓冲区中的偏移量。</param>
        public static void WriteUInt(this byte[] buffer, uint value, ref int offset)
        {
            if (offset + UIntSize > buffer.Length)
            {
                offset += UIntSize;
                return;
            }

            BinaryPrimitives.WriteUInt32BigEndian(buffer.AsSpan()[offset..], value);
            offset += UIntSize;
        }

        /// <summary>
        /// 将一个32位整数写入指定的缓冲区，并更新偏移量。
        /// </summary>
        /// <param name="buffer">要写入的缓冲区。</param>
        /// <param name="value">要写入的值。</param>
        /// <param name="offset">要写入值的缓冲区中的偏移量。</param>
        public static void WriteInt(this byte[] buffer, int value, ref int offset)
        {
            if (offset + IntSize > buffer.Length)
            {
                offset += IntSize;
                return;
            }

            BinaryPrimitives.WriteInt32BigEndian(buffer.AsSpan()[offset..], value);
            offset += IntSize;
        }

        /// <summary>
        /// 将一个8位整数写入指定的缓冲区，并更新偏移量。
        /// </summary>
        /// <param name="buffer">要写入的缓冲区。</param>
        /// <param name="value">要写入的值。</param>
        /// <param name="offset">要写入值的缓冲区中的偏移量。</param>
        public static void WriteByte(this byte[] buffer, byte value, ref int offset)
        {
            if (offset + ByteSize > buffer.Length)
            {
                offset += ByteSize;
                return;
            }

            buffer[offset] = value;
            offset += ByteSize;
        }

        /// <summary>
        /// 将一个16位整数写入指定的缓冲区，并更新偏移量。
        /// </summary>
        /// <param name="buffer">要写入的缓冲区。</param>
        /// <param name="value">要写入的值。</param>
        /// <param name="offset">要写入值的缓冲区中的偏移量。</param>
        public static void WriteShort(this byte[] buffer, short value, ref int offset)
        {
            if (offset + ShortSize > buffer.Length)
            {
                offset += ShortSize;
                return;
            }

            BinaryPrimitives.WriteInt16BigEndian(buffer.AsSpan()[offset..], value);
            offset += ShortSize;
        }


        /// <summary>
        /// 将一个16位无符号整数写入指定的缓冲区，并更新偏移量。
        /// </summary>
        /// <param name="buffer">要写入的缓冲区。</param>
        /// <param name="value">要写入的值。</param>
        /// <param name="offset">要写入值的缓冲区中的偏移量。</param>
        public static void WriteUShort(this byte[] buffer, ushort value, ref int offset)
        {
            if (offset + UShortSize > buffer.Length)
            {
                offset += UShortSize;
                return;
            }

            BinaryPrimitives.WriteUInt16BigEndian(buffer.AsSpan()[offset..], value);
            offset += UShortSize;
        }

        /// <summary>
        /// 将一个64位整数写入指定的缓冲区，并更新偏移量。
        /// </summary>
        /// <param name="buffer">要写入的缓冲区。</param>
        /// <param name="value">要写入的值。</param>
        /// <param name="offset">要写入值的缓冲区中的偏移量。</param>
        public static void WriteLong(this byte[] buffer, long value, ref int offset)
        {
            if (offset + LongSize > buffer.Length)
            {
                offset += LongSize;
                return;
            }

            BinaryPrimitives.WriteInt64BigEndian(buffer.AsSpan()[offset..], value);
            offset += LongSize;
        }


        /// <summary>
        /// 将一个64位无符号整数写入指定的缓冲区，并更新偏移量。
        /// </summary>
        /// <param name="buffer">要写入的缓冲区。</param>
        /// <param name="value">要写入的值。</param>
        /// <param name="offset">要写入值的缓冲区中的偏移量。</param>
        public static void WriteULong(this byte[] buffer, ulong value, ref int offset)
        {
            if (offset + ULongSize > buffer.Length)
            {
                offset += ULongSize;
                return;
            }

            BinaryPrimitives.WriteUInt64BigEndian(buffer.AsSpan()[offset..], value);
            offset += ULongSize;
        }

        /// <summary>
        /// 从字节数组中读取16位无符号整数，并将偏移量向前移动。
        /// </summary>
        /// <param name="buffer">要读取的字节数组。</param>
        /// <param name="offset">引用偏移量。</param>
        /// <returns>返回读取的16位无符号整数。</returns>
        public static ushort ReadUShort(this byte[] buffer, ref int offset)
        {
            if (offset > buffer.Length + UShortSize)
            {
                throw new Exception("buffer read out of index");
            }

            var value = BinaryPrimitives.ReadUInt16BigEndian(buffer.AsSpan()[offset..]);
            offset += UShortSize;
            return value;
        }

        /// <summary>
        /// 从字节数组读取16位有符号整数，并将偏移量前移。
        /// </summary>
        /// <param name="buffer">要读取的字节数组。</param>
        /// <param name="offset">引用偏移量。</param>
        /// <returns>返回读取的16位有符号整数。</returns>
        public static short ReadShort(this byte[] buffer, ref int offset)
        {
            if (offset > buffer.Length + ShortSize)
            {
                throw new Exception("buffer read out of index");
            }

            var value = BinaryPrimitives.ReadInt16BigEndian(buffer.AsSpan()[offset..]);
            offset += ShortSize;
            return value;
        }

        /// <summary>
        /// 从字节数组中读取32位无符号整数，并将偏移量向前移动。
        /// </summary>
        /// <param name="buffer">要读取的字节数组。</param>
        /// <param name="offset">引用偏移量。</param>
        /// <returns>返回读取的32位无符号整数。</returns>
        public static uint ReadUInt(this byte[] buffer, ref int offset)
        {
            if (offset > buffer.Length + UIntSize)
            {
                throw new Exception("buffer read out of index");
            }

            var value = BinaryPrimitives.ReadUInt32BigEndian(buffer.AsSpan()[offset..]);
            offset += UIntSize;
            return value;
        }

        /// <summary>
        /// 从字节数组中读取32位有符号整数，并将偏移量向前移动。
        /// </summary>
        /// <param name="buffer">要读取的字节数组。</param>
        /// <param name="offset">引用偏移量。</param>
        /// <returns>返回读取的32位有符号整数。</returns>
        public static int ReadInt(this byte[] buffer, ref int offset)
        {
            if (offset > buffer.Length + IntSize)
            {
                throw new Exception("buffer read out of index");
            }

            var value = BinaryPrimitives.ReadInt32BigEndian(buffer.AsSpan()[offset..]);
            offset += IntSize;
            return value;
        }

        /// <summary>
        /// 从字节数组中读取64位无符号整数，并将偏移量向前移动。
        /// </summary>
        /// <param name="buffer">要读取的字节数组。</param>
        /// <param name="offset">引用偏移量。</param>
        /// <returns>返回读取的64位无符号整数。</returns>    
        public static ulong ReadULong(this byte[] buffer, ref int offset)
        {
            if (offset > buffer.Length + ULongSize)
            {
                throw new Exception("buffer read out of index");
            }

            var value = BinaryPrimitives.ReadUInt64BigEndian(buffer.AsSpan()[offset..]);
            offset += ULongSize;
            return value;
        }

        /// <summary>
        /// 从字节数组中读取64位有符号整数，并将偏移量向前移动。
        /// </summary>
        /// <param name="buffer">要读取的字节数组。</param>
        /// <param name="offset">引用偏移量。</param>
        /// <returns>返回读取的64位有符号整数。</returns>
        public static long ReadLong(this byte[] buffer, ref int offset)
        {
            if (offset > buffer.Length + LongSize)
            {
                throw new Exception("buffer read out of index");
            }

            var value = BinaryPrimitives.ReadInt64BigEndian(buffer.AsSpan()[offset..]);
            offset += LongSize;
            return value;
        }

        /// <summary>
        /// 从Span字节数组中读取32位无符号整数，并将偏移量向前移动。
        /// </summary>
        /// <param name="buffer">要读取的Span字节数组。</param>
        /// <param name="offset">引用偏移量。</param>
        /// <returns>返回读取的32位无符号整数。</returns>
        public static uint ReadUInt(this Span<byte> buffer, ref int offset)
        {
            if (offset > buffer.Length + UIntSize)
            {
                throw new Exception("buffer read out of index");
            }

            var value = BinaryPrimitives.ReadUInt32BigEndian(buffer[offset..]);
            offset += UIntSize;
            return value;
        }

        /// <summary>
        /// 从Span字节数组中读取32位有符号整数，并将偏移量向前移动。
        /// </summary>
        /// <param name="buffer">要读取的Span字节数组。</param>
        /// <param name="offset">引用偏移量。</param>
        /// <returns>返回读取的32位有符号整数。</returns>
        public static int ReadInt(this Span<byte> buffer, ref int offset)
        {
            if (offset > buffer.Length + IntSize)
            {
                throw new Exception("buffer read out of index");
            }

            var value = BinaryPrimitives.ReadInt32BigEndian(buffer[offset..]);
            offset += IntSize;
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
            if (offset > buffer.Length + ULongSize)
            {
                throw new Exception("buffer read out of index");
            }

            var value = BinaryPrimitives.ReadUInt64BigEndian(buffer[offset..]);
            offset += ULongSize;
            return value;
        }

        /// <summary>
        /// 从字节数组中以指定偏移量读取64位整型。
        /// </summary>
        /// <param name="buffer">要从中读取数据的字节数组。</param>
        /// <param name="offset">读取数据的起始偏移量，此偏移量在读取后会自动增加。</param>
        /// <returns>读取的64位整型，若读取长度小于等于0或偏移量超出数组长度，返回0。</returns>
        public static long ReadLong(this Span<byte> buffer, ref int offset)
        {
            if (offset > buffer.Length + LongSize)
            {
                throw new Exception("buffer read out of index");
            }

            var value = BinaryPrimitives.ReadInt64BigEndian(buffer[offset..]);
            offset += LongSize;
            return value;
        }

        /// <summary>
        /// 从字节数组中以指定偏移量读取无符号整型。
        /// </summary>
        /// <param name="buffer">要从中读取数据的字节数组。</param>
        /// <param name="offset">读取数据的起始偏移量，此偏移量在读取后会自动增加。</param>
        /// <returns>读取的无符号整型，若读取长度小于等于0或偏移量超出数组长度，返回0。</returns>
        public static uint ReadUInt(this ReadOnlySpan<byte> buffer, ref int offset)
        {
            if (offset > buffer.Length + UIntSize)
            {
                throw new Exception("buffer read out of index");
            }

            var value = BinaryPrimitives.ReadUInt32BigEndian(buffer[offset..]);
            offset += UIntSize;
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
            if (offset > buffer.Length + IntSize)
            {
                throw new Exception("buffer read out of index");
            }

            var value = BinaryPrimitives.ReadInt32BigEndian(buffer[offset..]);
            offset += IntSize;
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
            if (offset > buffer.Length + ULongSize)
            {
                throw new Exception("buffer read out of index");
            }

            var value = BinaryPrimitives.ReadUInt64BigEndian(buffer[offset..]);
            offset += ULongSize;
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
            if (offset > buffer.Length + LongSize)
            {
                throw new Exception("buffer read out of index");
            }

            var value = BinaryPrimitives.ReadInt64BigEndian(buffer[offset..]);
            offset += LongSize;
            return value;
        }

        #region Write

        /// <summary>
        /// 将一个单精度浮点数写入指定的缓冲区，并更新偏移量。
        /// </summary>
        /// <param name="buffer">要写入的缓冲区。</param>
        /// <param name="value">要写入的值。</param>
        /// <param name="offset">要写入值的缓冲区中的偏移量。</param>
        public static unsafe void WriteFloat(this byte[] buffer, float value, ref int offset)
        {
            if (offset + FloatSize > buffer.Length)
            {
                offset += FloatSize;
                return;
            }

            fixed (byte* ptr = buffer)
            {
                *(float*)(ptr + offset) = value;
                *(int*)(ptr + offset) = System.Net.IPAddress.HostToNetworkOrder(*(int*)(ptr + offset));
                offset += FloatSize;
            }
        }

        /// <summary>
        /// 将一个双精度浮点数写入指定的缓冲区，并更新偏移量。
        /// </summary>
        /// <param name="buffer">要写入的缓冲区。</param>
        /// <param name="value">要写入的值。</param>
        /// <param name="offset">要写入值的缓冲区中的偏移量。</param>
        public static unsafe void WriteDouble(this byte[] buffer, double value, ref int offset)
        {
            if (offset + DoubleSize > buffer.Length)
            {
                offset += DoubleSize;
                return;
            }

            fixed (byte* ptr = buffer)
            {
                *(double*)(ptr + offset) = value;
                *(long*)(ptr + offset) = System.Net.IPAddress.HostToNetworkOrder(*(long*)(ptr + offset));
                offset += DoubleSize;
            }
        }


        /// <summary>
        /// 将一个字节数组写入指定的缓冲区，并更新偏移量。
        /// </summary>
        /// <param name="buffer">要写入的缓冲区。</param>
        /// <param name="value">要写入的值。</param>
        /// <param name="offset">要写入值的缓冲区中的偏移量。</param>
        public static unsafe void WriteBytes(this byte[] buffer, byte[] value, ref int offset)
        {
            if (value == null)
            {
                buffer.WriteInt(0, ref offset);
                return;
            }

            if (offset + value.Length + IntSize > buffer.Length)
            {
                offset += value.Length + IntSize;
                return;
            }

            buffer.WriteInt(value.Length, ref offset);
            System.Array.Copy(value, 0, buffer, offset, value.Length);
            offset += value.Length;
        }

        /// <summary>
        /// 将一个字节数组写入指定的缓冲区，并更新偏移量。
        /// </summary>
        /// <param name="buffer">要写入的缓冲区。</param>
        /// <param name="value">要写入的值。</param>
        /// <param name="offset">要写入值的缓冲区中的偏移量。</param>
        public static unsafe void WriteBytesWithoutLength(this byte[] buffer, byte[] value, ref int offset)
        {
            if (value == null)
            {
                buffer.WriteInt(0, ref offset);
                return;
            }

            if (offset + value.Length > buffer.Length)
            {
                throw new ArgumentException($"buffer write out of index {offset + value.Length + IntSize}, {buffer.Length}");
            }

            fixed (byte* ptr = buffer, valPtr = value)
            {
                Buffer.MemoryCopy(valPtr, ptr + offset, value.Length, value.Length);
                offset += value.Length;
            }
        }

        /// <summary>
        /// 将一个字节写入指定的缓冲区，并更新偏移量。
        /// </summary>
        /// <param name="buffer">要写入的缓冲区。</param>
        /// <param name="value">要写入的值。</param>
        /// <param name="offset">要写入值的缓冲区中的偏移量。</param>
        public static unsafe void WriteSByte(this byte[] buffer, sbyte value, ref int offset)
        {
            if (offset + SbyteSize > buffer.Length)
            {
                offset += SbyteSize;
                return;
            }

            fixed (byte* ptr = buffer)
            {
                *(sbyte*)(ptr + offset) = value;
                offset += SbyteSize;
            }
        }

        /// <summary>
        /// 将一个字符串写入指定的缓冲区，并更新偏移量。
        /// </summary>
        /// <param name="buffer">要写入的缓冲区。</param>
        /// <param name="value">要写入的值。</param>
        /// <param name="offset">要写入值的缓冲区中的偏移量。</param>
        public static unsafe void WriteString(this byte[] buffer, string value, ref int offset)
        {
            if (value == null)
                value = string.Empty;

            int len = System.Text.Encoding.UTF8.GetByteCount(value);

            if (len > short.MaxValue)
            {
                throw new ArgumentException($"string length exceed short.MaxValue {len}, {short.MaxValue}");
            }


            if (offset + len + ShortSize > buffer.Length)
            {
                offset += len + ShortSize;
                return;
            }

            fixed (byte* ptr = buffer)
            {
                System.Text.Encoding.UTF8.GetBytes(value, 0, value.Length, buffer, offset + ShortSize);
                WriteShort(buffer, (short)len, ref offset);
                offset += len;
            }
        }

        /// <summary>
        /// 将一个布尔值写入指定的缓冲区，并更新偏移量。
        /// </summary>
        /// <param name="buffer">要写入的缓冲区。</param>
        /// <param name="value">要写入的值。</param>
        /// <param name="offset">要写入值的缓冲区中的偏移量。</param>
        public static unsafe void WriteBool(this byte[] buffer, bool value, ref int offset)
        {
            if (offset + BoolSize > buffer.Length)
            {
                offset += BoolSize;
                return;
            }

            fixed (byte* ptr = buffer)
            {
                *(bool*)(ptr + offset) = value;
                offset += BoolSize;
            }
        }

        #endregion

        #region Read

        /// <summary>
        /// 从给定的字节缓冲区中读取浮点数，并更新偏移量。
        /// </summary>
        /// <param name="buffer">包含了要读取数据的字节缓冲区。</param>
        /// <param name="offset">读取数据的起始位置，该方法会更新该值。</param>
        /// <returns>从字节缓冲区中读取的浮点数。</returns>
        /// <exception cref="Exception">当尝试读取的位置超出了缓冲区的边界时，会抛出此异常。</exception>
        public static unsafe float ReadFloat(this byte[] buffer, ref int offset)
        {
            if (offset > buffer.Length + FloatSize)
            {
                throw new Exception("buffer read out of index");
            }

            fixed (byte* ptr = buffer)
            {
                *(int*)(ptr + offset) = System.Net.IPAddress.NetworkToHostOrder(*(int*)(ptr + offset));
                var value = *(float*)(ptr + offset);
                offset += FloatSize;
                return value;
            }
        }

        /// <summary>
        /// 从指定偏移量读取 double 类型数据。
        /// </summary>
        /// <param name="buffer">要操作的字节缓冲区。</param>
        /// <param name="offset">操作的起始偏移量，操作完成后，会自动累加双精度浮点数的字节数。</param>
        /// <returns>返回从缓冲区读取的 double 类型数据。</returns>
        public static unsafe double ReadDouble(this byte[] buffer, ref int offset)
        {
            if (offset > buffer.Length + DoubleSize)
            {
                throw new Exception("buffer read out of index");
            }

            fixed (byte* ptr = buffer)
            {
                *(long*)(ptr + offset) = System.Net.IPAddress.NetworkToHostOrder(*(long*)(ptr + offset));
                var value = *(double*)(ptr + offset);
                offset += DoubleSize;
                return value;
            }
        }

        /// <summary>
        /// 从指定偏移量读取 byte 类型数据。
        /// </summary>
        /// <param name="buffer">要操作的字节缓冲区。</param>
        /// <param name="offset">操作的起始偏移量，操作完成后，会自动累加字节的字节数。</param>
        /// <returns>返回从缓冲区读取的 byte 类型数据。</returns>
        public static unsafe byte ReadByte(this byte[] buffer, ref int offset)
        {
            if (offset > buffer.Length + ByteSize)
            {
                throw new Exception("buffer read out of index");
            }

            fixed (byte* ptr = buffer)
            {
                var value = *(ptr + offset);
                offset += ByteSize;
                return value;
            }
        }

        /// <summary>
        /// 从指定偏移量开始读取指定长度的字节数组。
        /// </summary>
        /// <param name="buffer">要操作的字节缓冲区。</param>
        /// <param name="offset">操作的起始偏移量。</param>
        /// <param name="len">需要读取的字节数组长度。</param>
        /// <returns>返回从缓冲区读取的 byte[] 类型数据。</returns>
        public static unsafe byte[] ReadBytes(this byte[] buffer, int offset, int len)
        {
            if (len <= 0 || offset > buffer.Length + len * ByteSize)
            {
                return Array.Empty<byte>();
            }

            var data = new byte[len];
            System.Array.Copy(buffer, offset, data, 0, len);
            offset += len;
            return data;
        }

        /// <summary>
        /// 从指定偏移量开始读取指定长度的字节数组，长度作为 int 类型数据在字节数组的开头。
        /// </summary>
        /// <param name="buffer">要操作的字节缓冲区。</param>
        /// <param name="offset">操作的起始偏移量，操作完成后，会自动累加读取的字节长度以及 int 类型长度。</param>
        /// <returns>返回从缓冲区读取的 byte[] 类型数据。</returns>
        public static unsafe byte[] ReadBytes(this byte[] buffer, ref int offset)
        {
            var len = ReadInt(buffer, ref offset);

            if (len <= 0 || offset > buffer.Length + len * ByteSize)
            {
                return Array.Empty<byte>();
            }

            var data = new byte[len];
            System.Array.Copy(buffer, offset, data, 0, len);
            offset += len;
            return data;
        }

        /// <summary>
        /// 从字节数组中以指定偏移量读取有符号字节。
        /// </summary>
        /// <param name="buffer">要从中读取数据的字节数组。</param>
        /// <param name="offset">读取数据的起始偏移量，此偏移量在读取后会自动增加。</param>
        /// <returns>读取的有符号字节。</returns>
        /// <exception cref="Exception">当偏移量超过数组长度时，将抛出异常。</exception>
        public static unsafe sbyte ReadSByte(this byte[] buffer, ref int offset)
        {
            if (offset > buffer.Length + ByteSize)
            {
                throw new Exception("buffer read out of index");
            }

            fixed (byte* ptr = buffer)
            {
                var value = *(sbyte*)(ptr + offset);
                offset += ByteSize;
                return value;
            }
        }

        /// <summary>
        /// 从字节数组中以指定偏移量读取字符串。
        /// </summary>
        /// <param name="buffer">要从中读取数据的字节数组。</param>
        /// <param name="offset">读取数据的起始偏移量，此偏移量在读取后会自动增加。</param>
        /// <returns>读取的字符串，若读取长度小于等于0或偏移量超出数组长度，返回空字符串。</returns>
        public static unsafe string ReadString(this byte[] buffer, ref int offset)
        {
            fixed (byte* ptr = buffer)
            {
                var len = ReadShort(buffer, ref offset);

                if (len <= 0 || offset > buffer.Length + len * ByteSize)
                {
                    return string.Empty;
                }

                var value = System.Text.Encoding.UTF8.GetString(buffer, offset, len);
                offset += len;
                return value;
            }
        }

        /// <summary>
        /// 从字节数组中以指定偏移量读取布尔值。
        /// </summary>
        /// <param name="buffer">要从中读取数据的字节数组。</param>
        /// <param name="offset">读取数据的起始偏移量，此偏移量在读取后会自动增加。</param>
        /// <returns>读取的布尔值。</returns>
        /// <exception cref="Exception">当偏移量超过数组长度时，将抛出异常。</exception>
        public static unsafe bool ReadBool(this byte[] buffer, ref int offset)
        {
            if (offset > buffer.Length + BoolSize)
            {
                throw new Exception("buffer read out of index");
            }

            fixed (byte* ptr = buffer)
            {
                var value = *(bool*)(ptr + offset);
                offset += BoolSize;
                return value;
            }
        }

        #endregion
    }
}