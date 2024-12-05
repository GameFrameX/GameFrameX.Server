using System.Buffers.Binary;
using System.Text;

namespace GameFrameX.Extension
{
    /// <summary>
    /// 提供字节和字节数组的扩展方法，用于各种格式的转换和读写操作。
    /// </summary>
    public static class ByteExtension
    {
        /// <summary>
        /// 将字节转换为16进制字符串。
        /// </summary>
        /// <param name="b">要转换的字节。</param>
        /// <returns>16进制字符串。</returns>
        public static string ToHex(this byte b)
        {
            return b.ToString("X2");
        }

        /// <summary>
        /// 将字节数组转换为字符串，每个字节之间用空格分隔。
        /// </summary>
        /// <param name="bytes">要转换的字节数组。</param>
        /// <returns>字符串表示形式。</returns>
        public static string ToArrayString(this byte[] bytes)
        {
            StringBuilder stringBuilder = new StringBuilder();
            foreach (byte b in bytes)
            {
                stringBuilder.Append(b + " ");
            }

            return stringBuilder.ToString();
        }

        /// <summary>
        /// 将字节数组转换为16进制字符串。
        /// </summary>
        /// <param name="bytes">要转换的字节数组。</param>
        /// <returns>16进制字符串。</returns>
        public static string ToHex(this byte[] bytes)
        {
            StringBuilder stringBuilder = new StringBuilder();
            foreach (byte b in bytes)
            {
                stringBuilder.Append(b.ToString("X2"));
            }

            return stringBuilder.ToString();
        }

        /// <summary>
        /// 将字节数组转换为指定格式的字符串。
        /// </summary>
        /// <param name="bytes">要转换的字节数组。</param>
        /// <param name="format">格式化字符串。</param>
        /// <returns>格式化后的字符串。</returns>
        public static string ToHex(this byte[] bytes, string format)
        {
            StringBuilder stringBuilder = new StringBuilder();
            foreach (byte b in bytes)
            {
                stringBuilder.Append(b.ToString(format));
            }

            return stringBuilder.ToString();
        }

        /// <summary>
        /// 将字节数组的指定范围转换为16进制字符串。
        /// </summary>
        /// <param name="bytes">要转换的字节数组。</param>
        /// <param name="offset">起始偏移量。</param>
        /// <param name="count">要转换的字节数。</param>
        /// <returns>16进制字符串。</returns>
        public static string ToHex(this byte[] bytes, int offset, int count)
        {
            StringBuilder stringBuilder = new StringBuilder();
            for (int i = offset; i < offset + count; ++i)
            {
                stringBuilder.Append(bytes[i].ToString("X2"));
            }

            return stringBuilder.ToString();
        }

        /// <summary>
        /// 将字节数组转换为默认编码的字符串。
        /// </summary>
        /// <param name="bytes">要转换的字节数组。</param>
        /// <returns>字符串。</returns>
        public static string ToDefaultString(this byte[] bytes)
        {
            return Encoding.Default.GetString(bytes);
        }

        /// <summary>
        /// 将字节数组的指定范围转换为默认编码的字符串。
        /// </summary>
        /// <param name="bytes">要转换的字节数组。</param>
        /// <param name="index">起始偏移量。</param>
        /// <param name="count">要转换的字节数。</param>
        /// <returns>字符串。</returns>
        public static string ToDefaultString(this byte[] bytes, int index, int count)
        {
            return Encoding.Default.GetString(bytes, index, count);
        }

        /// <summary>
        /// 将字节数组转换为UTF8编码的字符串。
        /// </summary>
        /// <param name="bytes">要转换的字节数组。</param>
        /// <returns>UTF8编码的字符串。</returns>
        public static string ToUtf8String(this byte[] bytes)
        {
            return Encoding.UTF8.GetString(bytes);
        }

        /// <summary>
        /// 将字节数组的指定范围转换为UTF8编码的字符串。
        /// </summary>
        /// <param name="bytes">要转换的字节数组。</param>
        /// <param name="index">起始偏移量。</param>
        /// <param name="count">要转换的字节数。</param>
        /// <returns>UTF8编码的字符串。</returns>
        public static string ToUtf8String(this byte[] bytes, int index, int count)
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
            if (offset + ConstSize.UIntSize > buffer.Length)
            {
                offset += ConstSize.UIntSize;
                return;
            }

            BinaryPrimitives.WriteUInt32BigEndian(buffer.AsSpan()[offset..], value);
            offset += ConstSize.UIntSize;
        }

        /// <summary>
        /// 将一个32位整数写入指定的缓冲区，并更新偏移量。
        /// </summary>
        /// <param name="buffer">要写入的缓冲区。</param>
        /// <param name="value">要写入的值。</param>
        /// <param name="offset">要写入值的缓冲区中的偏移量。</param>
        public static void WriteInt(this byte[] buffer, int value, ref int offset)
        {
            if (offset + ConstSize.IntSize > buffer.Length)
            {
                offset += ConstSize.IntSize;
                return;
            }

            BinaryPrimitives.WriteInt32BigEndian(buffer.AsSpan()[offset..], value);
            offset += ConstSize.IntSize;
        }

        /// <summary>
        /// 将一个8位整数写入指定的缓冲区，并更新偏移量。
        /// </summary>
        /// <param name="buffer">要写入的缓冲区。</param>
        /// <param name="value">要写入的值。</param>
        /// <param name="offset">要写入值的缓冲区中的偏移量。</param>
        public static void WriteByte(this byte[] buffer, byte value, ref int offset)
        {
            if (offset + ConstSize.ByteSize > buffer.Length)
            {
                offset += ConstSize.ByteSize;
                return;
            }

            buffer[offset] = value;
            offset += ConstSize.ByteSize;
        }

        /// <summary>
        /// 将一个16位整数写入指定的缓冲区，并更新偏移量。
        /// </summary>
        /// <param name="buffer">要写入的缓冲区。</param>
        /// <param name="value">要写入的值。</param>
        /// <param name="offset">要写入值的缓冲区中的偏移量。</param>
        public static void WriteShort(this byte[] buffer, short value, ref int offset)
        {
            if (offset + ConstSize.ShortSize > buffer.Length)
            {
                offset += ConstSize.ShortSize;
                return;
            }

            BinaryPrimitives.WriteInt16BigEndian(buffer.AsSpan()[offset..], value);
            offset += ConstSize.ShortSize;
        }

        /// <summary>
        /// 将一个16位无符号整数写入指定的缓冲区，并更新偏移量。
        /// </summary>
        /// <param name="buffer">要写入的缓冲区。</param>
        /// <param name="value">要写入的值。</param>
        /// <param name="offset">要写入值的缓冲区中的偏移量。</param>
        public static void WriteUShort(this byte[] buffer, ushort value, ref int offset)
        {
            if (offset + ConstSize.UShortSize > buffer.Length)
            {
                offset += ConstSize.UShortSize;
                return;
            }

            BinaryPrimitives.WriteUInt16BigEndian(buffer.AsSpan()[offset..], value);
            offset += ConstSize.UShortSize;
        }

        /// <summary>
        /// 将一个64位整数写入指定的缓冲区，并更新偏移量。
        /// </summary>
        /// <param name="buffer">要写入的缓冲区。</param>
        /// <param name="value">要写入的值。</param>
        /// <param name="offset">要写入值的缓冲区中的偏移量。</param>
        public static void WriteLong(this byte[] buffer, long value, ref int offset)
        {
            if (offset + ConstSize.LongSize > buffer.Length)
            {
                offset += ConstSize.LongSize;
                return;
            }

            BinaryPrimitives.WriteInt64BigEndian(buffer.AsSpan()[offset..], value);
            offset += ConstSize.LongSize;
        }

        /// <summary>
        /// 将一个64位无符号整数写入指定的缓冲区，并更新偏移量。
        /// </summary>
        /// <param name="buffer">要写入的缓冲区。</param>
        /// <param name="value">要写入的值。</param>
        /// <param name="offset">要写入值的缓冲区中的偏移量。</param>
        public static void WriteULong(this byte[] buffer, ulong value, ref int offset)
        {
            if (offset + ConstSize.ULongSize > buffer.Length)
            {
                offset += ConstSize.ULongSize;
                return;
            }

            BinaryPrimitives.WriteUInt64BigEndian(buffer.AsSpan()[offset..], value);
            offset += ConstSize.ULongSize;
        }

        /// <summary>
        /// 从字节数组中读取16位无符号整数，并将偏移量向前移动。
        /// </summary>
        /// <param name="buffer">要读取的字节数组。</param>
        /// <param name="offset">引用偏移量。</param>
        /// <returns>返回读取的16位无符号整数。</returns>
        public static ushort ReadUShort(this byte[] buffer, ref int offset)
        {
            if (offset > buffer.Length + ConstSize.UShortSize)
            {
                throw new Exception("buffer read out of index");
            }

            var value = BinaryPrimitives.ReadUInt16BigEndian(buffer.AsSpan()[offset..]);
            offset += ConstSize.UShortSize;
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
            if (offset > buffer.Length + ConstSize.ShortSize)
            {
                throw new Exception("buffer read out of index");
            }

            var value = BinaryPrimitives.ReadInt16BigEndian(buffer.AsSpan()[offset..]);
            offset += ConstSize.ShortSize;
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
            if (offset > buffer.Length + ConstSize.UIntSize)
            {
                throw new Exception("buffer read out of index");
            }

            var value = BinaryPrimitives.ReadUInt32BigEndian(buffer.AsSpan()[offset..]);
            offset += ConstSize.UIntSize;
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
            if (offset > buffer.Length + ConstSize.IntSize)
            {
                throw new Exception("buffer read out of index");
            }

            var value = BinaryPrimitives.ReadInt32BigEndian(buffer.AsSpan()[offset..]);
            offset += ConstSize.IntSize;
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
            if (offset > buffer.Length + ConstSize.ULongSize)
            {
                throw new Exception("buffer read out of index");
            }

            var value = BinaryPrimitives.ReadUInt64BigEndian(buffer.AsSpan()[offset..]);
            offset += ConstSize.ULongSize;
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
            if (offset > buffer.Length + ConstSize.LongSize)
            {
                throw new Exception("buffer read out of index");
            }

            var value = BinaryPrimitives.ReadInt64BigEndian(buffer.AsSpan()[offset..]);
            offset += ConstSize.LongSize;
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
            if (offset + ConstSize.FloatSize > buffer.Length)
            {
                offset += ConstSize.FloatSize;
                return;
            }

            fixed (byte* ptr = buffer)
            {
                *(float*)(ptr + offset) = value;
                *(int*)(ptr + offset) = System.Net.IPAddress.HostToNetworkOrder(*(int*)(ptr + offset));
                offset += ConstSize.FloatSize;
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
            if (offset + ConstSize.DoubleSize > buffer.Length)
            {
                offset += ConstSize.DoubleSize;
                return;
            }

            fixed (byte* ptr = buffer)
            {
                *(double*)(ptr + offset) = value;
                *(long*)(ptr + offset) = System.Net.IPAddress.HostToNetworkOrder(*(long*)(ptr + offset));
                offset += ConstSize.DoubleSize;
            }
        }


        /// <summary>
        /// 将一个字节数组写入指定的缓冲区，并更新偏移量。
        /// </summary>
        /// <param name="buffer">要写入的缓冲区。</param>
        /// <param name="value">要写入的值。</param>
        /// <param name="offset">要写入值的缓冲区中的偏移量。</param>
        public static void WriteBytes(this byte[] buffer, byte[] value, ref int offset)
        {
            if (value == null)
            {
                buffer.WriteInt(0, ref offset);
                return;
            }

            if (offset + value.Length + ConstSize.IntSize > buffer.Length)
            {
                offset += value.Length + ConstSize.IntSize;
                return;
            }

            buffer.WriteInt(value.Length, ref offset);
            Array.Copy(value, 0, buffer, offset, value.Length);
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
                throw new ArgumentException($"buffer write out of index {offset + value.Length}, {buffer.Length}");
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
            if (offset + ConstSize.SbyteSize > buffer.Length)
            {
                offset += ConstSize.SbyteSize;
                return;
            }

            fixed (byte* ptr = buffer)
            {
                *(sbyte*)(ptr + offset) = value;
                offset += ConstSize.SbyteSize;
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
            {
                value = string.Empty;
            }

            int len = Encoding.UTF8.GetByteCount(value);

            if (len > short.MaxValue)
            {
                throw new ArgumentException($"string length exceed short.MaxValue {len}, {short.MaxValue}");
            }


            if (offset + len + ConstSize.ShortSize > buffer.Length)
            {
                offset += len + ConstSize.ShortSize;
                return;
            }

            fixed (byte* ptr = buffer)
            {
                Encoding.UTF8.GetBytes(value, 0, value.Length, buffer, offset + ConstSize.ShortSize);
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
            if (offset + ConstSize.BoolSize > buffer.Length)
            {
                offset += ConstSize.BoolSize;
                return;
            }

            fixed (byte* ptr = buffer)
            {
                *(bool*)(ptr + offset) = value;
                offset += ConstSize.BoolSize;
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
            if (offset > buffer.Length + ConstSize.FloatSize)
            {
                throw new Exception("buffer read out of index");
            }

            fixed (byte* ptr = buffer)
            {
                *(int*)(ptr + offset) = System.Net.IPAddress.NetworkToHostOrder(*(int*)(ptr + offset));
                var value = *(float*)(ptr + offset);
                offset += ConstSize.FloatSize;
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
            if (offset > buffer.Length + ConstSize.DoubleSize)
            {
                throw new Exception("buffer read out of index");
            }

            fixed (byte* ptr = buffer)
            {
                *(long*)(ptr + offset) = System.Net.IPAddress.NetworkToHostOrder(*(long*)(ptr + offset));
                var value = *(double*)(ptr + offset);
                offset += ConstSize.DoubleSize;
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
        /// 从指定偏移量开始读取指定长度的字节数组。
        /// </summary>
        /// <param name="buffer">要操作的字节缓冲区。</param>
        /// <param name="offset">操作的起始偏移量。</param>
        /// <param name="len">需要读取的字节数组长度。</param>
        /// <returns>返回从缓冲区读取的 byte[] 类型数据。</returns>
        public static byte[] ReadBytes(this byte[] buffer, int offset, int len)
        {
            if (len <= 0 || offset > buffer.Length + len * ConstSize.ByteSize)
            {
                return Array.Empty<byte>();
            }

            var data = new byte[len];
            Array.Copy(buffer, offset, data, 0, len);
            return data;
        }

        /// <summary>
        /// 从指定偏移量开始读取指定长度的字节数组。
        /// </summary>
        /// <param name="buffer">要操作的字节缓冲区。</param>
        /// <param name="offset">操作的起始偏移量。</param>
        /// <param name="len">需要读取的字节数组长度。</param>
        /// <returns>返回从缓冲区读取的 byte[] 类型数据。</returns>
        public static byte[] ReadBytes(this byte[] buffer, ref int offset, int len)
        {
            if (len <= 0 || offset > buffer.Length + len * ConstSize.ByteSize)
            {
                return Array.Empty<byte>();
            }

            var data = new byte[len];
            Array.Copy(buffer, offset, data, 0, len);
            offset += len;
            return data;
        }

        /// <summary>
        /// 从指定偏移量开始读取指定长度的字节数组，长度作为 int 类型数据在字节数组的开头。
        /// </summary>
        /// <param name="buffer">要操作的字节缓冲区。</param>
        /// <param name="offset">操作的起始偏移量，操作完成后，会自动累加读取的字节长度以及 int 类型长度。</param>
        /// <returns>返回从缓冲区读取的 byte[] 类型数据。</returns>
        public static byte[] ReadBytes(this byte[] buffer, ref int offset)
        {
            var len = ReadInt(buffer, ref offset);

            if (len <= 0 || offset > buffer.Length + len * ConstSize.ByteSize)
            {
                return Array.Empty<byte>();
            }

            var data = new byte[len];
            Array.Copy(buffer, offset, data, 0, len);
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

                if (len <= 0 || offset > buffer.Length + len * ConstSize.ByteSize)
                {
                    return string.Empty;
                }

                var value = Encoding.UTF8.GetString(buffer, offset, len);
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
}