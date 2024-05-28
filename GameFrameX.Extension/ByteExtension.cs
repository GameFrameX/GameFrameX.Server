using System.Buffers.Binary;
using System.Text;

namespace GameFrameX.Extension
{
    public static class ByteExtension
    {
        private const int IntSize = sizeof(int);
        private const int ShortSize = sizeof(short);
        private const int LongSize = sizeof(long);
        private const int FloatSize = sizeof(float);
        private const int DoubleSize = sizeof(double);
        private const int ByteSize = sizeof(byte);
        private const int SbyteSize = sizeof(sbyte);
        private const int BoolSize = sizeof(bool);

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

        public static void Write(this byte[] bytes, uint num, int offset)
        {
            BinaryPrimitives.WriteUInt32BigEndian(bytes.AsSpan()[offset..], num);
        }

        public static void Write(this byte[] bytes, int num, int offset)
        {
            BinaryPrimitives.WriteInt32BigEndian(bytes.AsSpan()[offset..], num);
        }

        public static void Write(this byte[] bytes, byte num, int offset)
        {
            bytes[offset] = num;
        }

        public static void Write(this byte[] bytes, short num, int offset)
        {
            BinaryPrimitives.WriteInt16BigEndian(bytes.AsSpan().Slice(offset), num);
        }

        public static void Write(this byte[] bytes, ushort num, int offset)
        {
            BinaryPrimitives.WriteUInt16BigEndian(bytes.AsSpan()[offset..], num);
        }

        public static void Write(this byte[] bytes, long num, int offset)
        {
            BinaryPrimitives.WriteInt64BigEndian(bytes.AsSpan()[offset..], num);
        }

        public static int ReadInt(this byte[] bytes, int offset)
        {
            return BinaryPrimitives.ReadInt32BigEndian(bytes.AsSpan()[offset..]);
        }

        public static long ReadLong(this byte[] bytes, int offset)
        {
            return BinaryPrimitives.ReadInt64BigEndian(bytes.AsSpan()[offset..]);
        }

        public static int ReadInt(this Span<byte> bytes, int offset)
        {
            return BinaryPrimitives.ReadInt32BigEndian(bytes[offset..]);
        }

        public static long ReadLong(this Span<byte> bytes, int offset)
        {
            return BinaryPrimitives.ReadInt64BigEndian(bytes[offset..]);
        }

        public static int ReadInt(this ReadOnlySpan<byte> bytes, int offset)
        {
            return BinaryPrimitives.ReadInt32BigEndian(bytes[offset..]);
        }

        public static long ReadLong(this ReadOnlySpan<byte> bytes, int offset)
        {
            return BinaryPrimitives.ReadInt64BigEndian(bytes[offset..]);
        }


        #region Write

        public static unsafe void WriteInt(this byte[] buffer, int value, ref int offset)
        {
            if (offset + IntSize > buffer.Length)
            {
                offset += IntSize;
                return;
            }

            fixed (byte* ptr = buffer)
            {
                *(int*)(ptr + offset) = System.Net.IPAddress.HostToNetworkOrder(value);
                offset += IntSize;
            }
        }

        public static unsafe void WriteShort(this byte[] buffer, short value, ref int offset)
        {
            if (offset + ShortSize > buffer.Length)
            {
                offset += ShortSize;
                return;
            }

            fixed (byte* ptr = buffer)
            {
                *(short*)(ptr + offset) = System.Net.IPAddress.HostToNetworkOrder(value);
                offset += ShortSize;
            }
        }

        public static unsafe void WriteLong(this byte[] buffer, long value, ref int offset)
        {
            if (offset + LongSize > buffer.Length)
            {
                offset += LongSize;
                return;
            }

            fixed (byte* ptr = buffer)
            {
                *(long*)(ptr + offset) = System.Net.IPAddress.HostToNetworkOrder(value);
                offset += LongSize;
            }
        }

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

        public static unsafe void WriteByte(this byte[] buffer, byte value, ref int offset)
        {
            if (offset + ByteSize > buffer.Length)
            {
                offset += ByteSize;
                return;
            }

            fixed (byte* ptr = buffer)
            {
                *(ptr + offset) = value;
                offset += ByteSize;
            }
        }

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

        public static unsafe int ReadInt(this byte[] buffer, ref int offset)
        {
            if (offset > buffer.Length + IntSize)
            {
                throw new Exception("xbuffer read out of index");
            }

            fixed (byte* ptr = buffer)
            {
                var value = *(int*)(ptr + offset);
                offset += IntSize;
                return System.Net.IPAddress.NetworkToHostOrder(value);
            }
        }

        public static unsafe short ReadShort(this byte[] buffer, ref int offset)
        {
            if (offset > buffer.Length + ShortSize)
            {
                throw new Exception("xbuffer read out of index");
            }

            fixed (byte* ptr = buffer)
            {
                var value = *(short*)(ptr + offset);
                offset += ShortSize;
                return System.Net.IPAddress.NetworkToHostOrder(value);
            }
        }

        public static unsafe long ReadLong(this byte[] buffer, ref int offset)
        {
            if (offset > buffer.Length + LongSize)
            {
                throw new Exception("xbuffer read out of index");
            }

            fixed (byte* ptr = buffer)
            {
                var value = *(long*)(ptr + offset);
                offset += LongSize;
                return System.Net.IPAddress.NetworkToHostOrder(value);
            }
        }

        public static unsafe float ReadFloat(this byte[] buffer, ref int offset)
        {
            if (offset > buffer.Length + FloatSize)
            {
                throw new Exception("xbuffer read out of index");
            }

            fixed (byte* ptr = buffer)
            {
                *(int*)(ptr + offset) = System.Net.IPAddress.NetworkToHostOrder(*(int*)(ptr + offset));
                var value = *(float*)(ptr + offset);
                offset += FloatSize;
                return value;
            }
        }

        public static unsafe double ReadDouble(this byte[] buffer, ref int offset)
        {
            if (offset > buffer.Length + DoubleSize)
            {
                throw new Exception("xbuffer read out of index");
            }

            fixed (byte* ptr = buffer)
            {
                *(long*)(ptr + offset) = System.Net.IPAddress.NetworkToHostOrder(*(long*)(ptr + offset));
                var value = *(double*)(ptr + offset);
                offset += DoubleSize;
                return value;
            }
        }

        public static unsafe byte ReadByte(this byte[] buffer, ref int offset)
        {
            if (offset > buffer.Length + ByteSize)
            {
                throw new Exception("xbuffer read out of index");
            }

            fixed (byte* ptr = buffer)
            {
                var value = *(ptr + offset);
                offset += ByteSize;
                return value;
            }
        }

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

        public static unsafe sbyte ReadSByte(this byte[] buffer, ref int offset)
        {
            if (offset > buffer.Length + ByteSize)
            {
                throw new Exception("xbuffer read out of index");
            }

            fixed (byte* ptr = buffer)
            {
                var value = *(sbyte*)(ptr + offset);
                offset += ByteSize;
                return value;
            }
        }

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

        public static unsafe bool ReadBool(this byte[] buffer, ref int offset)
        {
            if (offset > buffer.Length + BoolSize)
            {
                throw new Exception("xbuffer read out of index");
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