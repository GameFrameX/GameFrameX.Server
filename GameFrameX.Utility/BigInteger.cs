//************************************************************************************
// BigInteger Class Version 1.03
//
// Copyright (c) 2002 Chew Keong TAN
// All rights reserved.
//
// Permission is hereby granted, free of charge, to any person obtaining a
// copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, provided that the above
// copyright notice(s) and this permission notice appear in all copies of
// the Software and that both the above copyright notice(s) and this
// permission notice appear in supporting documentation.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS
// OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT
// OF THIRD PARTY RIGHTS. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR
// HOLDERS INCLUDED IN THIS NOTICE BE LIABLE FOR ANY CLAIM, OR ANY SPECIAL
// INDIRECT OR CONSEQUENTIAL DAMAGES, OR ANY DAMAGES WHATSOEVER RESULTING
// FROM LOSS OF USE, DATA OR PROFITS, WHETHER IN AN ACTION OF CONTRACT,
// NEGLIGENCE OR OTHER TORTIOUS ACTION, ARISING OUT OF OR IN CONNECTION
// WITH THE USE OR PERFORMANCE OF THIS SOFTWARE.
//
//
// Disclaimer
// ----------
// Although reasonable care has been taken to ensure the correctness of this
// implementation, this code should never be used in any application without
// proper verification and testing.  I disclaim all liability and responsibility
// to any person or entity with respect to any loss or damage caused, or alleged
// to be caused, directly or indirectly, by the use of this BigInteger class.
//
// Comments, bugs and suggestions to
// (http://www.codeproject.com/csharp/biginteger.asp)
//
//
// Overloaded Operators +, -, *, /, %, >>, <<, ==, !=, >, <, >=, <=, &, |, ^, ++, --, ~
//
// Features
// --------
// 1) Arithmetic operations involving large signed integers (2's complement).
// 2) Primality test using Fermat little theorm, Rabin Miller's method,
//    Solovay Strassen's method and Lucas strong pseudoprime.
// 3) Modulo exponential with Barrett's reduction.
// 4) Inverse modulo.
// 5) Pseudo prime generation.
// 6) Co-prime generation.
//
//
// Known Problem
// -------------
// This pseudoprime passes my implementation of
// primality test but failed in JDK's isProbablePrime test.
//
//       byte[] pseudoPrime1 = { (byte)0x00,
//             (byte)0x85, (byte)0x84, (byte)0x64, (byte)0xFD, (byte)0x70, (byte)0x6A,
//             (byte)0x9F, (byte)0xF0, (byte)0x94, (byte)0x0C, (byte)0x3E, (byte)0x2C,
//             (byte)0x74, (byte)0x34, (byte)0x05, (byte)0xC9, (byte)0x55, (byte)0xB3,
//             (byte)0x85, (byte)0x32, (byte)0x98, (byte)0x71, (byte)0xF9, (byte)0x41,
//             (byte)0x21, (byte)0x5F, (byte)0x02, (byte)0x9E, (byte)0xEA, (byte)0x56,
//             (byte)0x8D, (byte)0x8C, (byte)0x44, (byte)0xCC, (byte)0xEE, (byte)0xEE,
//             (byte)0x3D, (byte)0x2C, (byte)0x9D, (byte)0x2C, (byte)0x12, (byte)0x41,
//             (byte)0x1E, (byte)0xF1, (byte)0xC5, (byte)0x32, (byte)0xC3, (byte)0xAA,
//             (byte)0x31, (byte)0x4A, (byte)0x52, (byte)0xD8, (byte)0xE8, (byte)0xAF,
//             (byte)0x42, (byte)0xF4, (byte)0x72, (byte)0xA1, (byte)0x2A, (byte)0x0D,
//             (byte)0x97, (byte)0xB1, (byte)0x31, (byte)0xB3,
//       };
//
//
// Change Log
// ----------
// 1) September 23, 2002 (Version 1.03)
//    - Fixed operator- to give correct data length.
//    - Added Lucas sequence generation.
//    - Added Strong Lucas Primality test.
//    - Added integer square root method.
//    - Added setBit/unsetBit methods.
//    - New isProbablePrime() method which do not require the
//      confident parameter.
//
// 2) August 29, 2002 (Version 1.02)
//    - Fixed bug in the exponentiation of negative numbers.
//    - Faster modular exponentiation using Barrett reduction.
//    - Added getBytes() method.
//    - Fixed bug in ToHexString method.
//    - Added overloading of ^ operator.
//    - Faster computation of Jacobi symbol.
//
// 3) August 19, 2002 (Version 1.01)
//    - Big integer is stored and manipulated as unsigned integers (4 bytes) instead of
//      individual bytes this gives significant performance improvement.
//    - Updated Fermat's Little Theorem test to use a^(p-1) mod p = 1
//    - Added isProbablePrime method.
//    - Updated documentation.
//
// 4) August 9, 2002 (Version 1.0)
//    - Initial Release.
//
//
// References
// [1] D. E. Knuth, "Seminumerical Algorithms", The Art of Computer Programming Vol. 2,
//     3rd Edition, Addison-Wesley, 1998.
//
// [2] K. H. Rosen, "Elementary Number Theory and Its Applications", 3rd Ed,
//     Addison-Wesley, 1993.
//
// [3] B. Schneier, "Applied Cryptography", 2nd Ed, John Wiley & Sons, 1996.
//
// [4] A. Menezes, P. van Oorschot, and S. Vanstone, "Handbook of Applied Cryptography",
//     CRC Press, 1996, www.cacr.math.uwaterloo.ca/hac
//
// [5] A. Bosselaers, R. Govaerts, and J. Vandewalle, "Comparison of Three Modular
//     Reduction Functions," Proc. CRYPTO'93, pp.175-186.
//
// [6] R. Baillie and S. S. Wagstaff Jr, "Lucas Pseudoprimes", Mathematics of Computation,
//     Vol. 35, No. 152, Oct 1980, pp. 1391-1417.
//
// [7] H. C. Williams, "Édouard Lucas and Primality Testing", Canadian Mathematical
//     Society Series of Monographs and Advance Texts, vol. 22, John Wiley & Sons, New York,
//     NY, 1998.
//
// [8] P. Ribenboim, "The new book of prime number records", 3rd edition, Springer-Verlag,
//     New York, NY, 1995.
//
// [9] M. Joye and J.-J. Quisquater, "Efficient computation of full Lucas sequences",
//     Electronics Letters, 32(6), 1996, pp 537-538.
//
//************************************************************************************
namespace GameFrameX.Utility;

/// <summary>
/// 表示一个大整数的类，支持多种数学运算和转换。
/// </summary>
public sealed class BigInteger
{
    // 最大长度，单位为 uint（4 字节）
    // 根据所需的精度级别进行更改。
    private const int maxLength = 70;

    /// <summary>
    /// 小于 2000 的素数数组，用于测试生成的素数。
    /// </summary>
    public static readonly int[] primesBelow2000 =
    {
        2, 3, 5, 7, 11, 13, 17, 19, 23, 29, 31, 37, 41, 43, 47, 53, 59, 61, 67, 71, 73, 79, 83, 89, 97,
        101, 103, 107, 109, 113, 127, 131, 137, 139, 149, 151, 157, 163, 167, 173, 179, 181, 191, 193, 197, 199,
        211, 223, 227, 229, 233, 239, 241, 251, 257, 263, 269, 271, 277, 281, 283, 293,
        307, 311, 313, 317, 331, 337, 347, 349, 353, 359, 367, 373, 379, 383, 389, 397,
        401, 409, 419, 421, 431, 433, 439, 443, 449, 457, 461, 463, 467, 479, 487, 491, 499,
        503, 509, 521, 523, 541, 547, 557, 563, 569, 571, 577, 587, 593, 599,
        601, 607, 613, 617, 619, 631, 641, 643, 647, 653, 659, 661, 673, 677, 683, 691,
        701, 709, 719, 727, 733, 739, 743, 751, 757, 761, 769, 773, 787, 797,
        809, 811, 821, 823, 827, 829, 839, 853, 857, 859, 863, 877, 881, 883, 887,
        907, 911, 919, 929, 937, 941, 947, 953, 967, 971, 977, 983, 991, 997,
        1009, 1013, 1019, 1021, 1031, 1033, 1039, 1049, 1051, 1061, 1063, 1069, 1087, 1091, 1093, 1097,
        1103, 1109, 1117, 1123, 1129, 1151, 1153, 1163, 1171, 1181, 1187, 1193,
        1201, 1213, 1217, 1223, 1229, 1231, 1237, 1249, 1259, 1277, 1279, 1283, 1289, 1291, 1297,
        1301, 1303, 1307, 1319, 1321, 1327, 1361, 1367, 1373, 1381, 1399,
        1409, 1423, 1427, 1429, 1433, 1439, 1447, 1451, 1453, 1459, 1471, 1481, 1483, 1487, 1489, 1493, 1499,
        1511, 1523, 1531, 1543, 1549, 1553, 1559, 1567, 1571, 1579, 1583, 1597,
        1601, 1607, 1609, 1613, 1619, 1621, 1627, 1637, 1657, 1663, 1667, 1669, 1693, 1697, 1699,
        1709, 1721, 1723, 1733, 1741, 1747, 1753, 1759, 1777, 1783, 1787, 1789,
        1801, 1811, 1823, 1831, 1847, 1861, 1867, 1871, 1873, 1877, 1879, 1889,
        1901, 1907, 1913, 1931, 1933, 1949, 1951, 1973, 1979, 1987, 1993, 1997, 1999,
    };

    private readonly uint[] data; // 存储大整数的字节

    /// <summary>
    /// 实际使用的字符数
    /// </summary>
    public int dataLength;

    //***********************************************************************
    // 构造函数（默认值为 0）
    //***********************************************************************

    /// <summary>
    /// 初始化一个新的 BigInteger 实例，默认值为 0。
    /// </summary>
    public BigInteger()
    {
        data = new uint[maxLength];
        dataLength = 1;
    }

    //***********************************************************************
    // 构造函数（使用 long 提供默认值）
    //***********************************************************************

    /// <summary>
    /// 使用指定的 long 值初始化一个新的 BigInteger 实例。
    /// </summary>
    /// <param name="value">要初始化的 long 值。</param>
    /// <exception cref="ArithmeticException">当发生溢出或下溢时引发。</exception>
    public BigInteger(long value)
    {
        data = new uint[maxLength];
        var tempVal = value;

        // 将 long 的字节复制到 BigInteger 中，而不假设 long 数据类型的长度
        dataLength = 0;
        while (value != 0 && dataLength < maxLength)
        {
            data[dataLength] = (uint)(value & 0xFFFFFFFF);
            value >>= 32;
            dataLength++;
        }

        if (tempVal > 0) // 正值溢出检查
        {
            if (value != 0 || (data[maxLength - 1] & 0x80000000) != 0)
            {
                throw new ArithmeticException("构造函数中的正溢出。");
            }
        }
        else if (tempVal < 0) // 负值下溢检查
        {
            if (value != -1 || (data[dataLength - 1] & 0x80000000) == 0)
            {
                throw new ArithmeticException("构造函数中的负下溢。");
            }
        }

        if (dataLength == 0)
        {
            dataLength = 1;
        }
    }

    //***********************************************************************
    // 构造函数（使用 ulong 提供默认值）
    //***********************************************************************

    /// <summary>
    /// 使用指定的 ulong 值初始化一个新的 BigInteger 实例。
    /// </summary>
    /// <param name="value">要初始化的 ulong 值。</param>
    /// <exception cref="ArithmeticException">当发生溢出时引发。</exception>
    public BigInteger(ulong value)
    {
        data = new uint[maxLength];

        // 将 ulong 的字节复制到 BigInteger 中，而不假设 ulong 数据类型的长度
        dataLength = 0;
        while (value != 0 && dataLength < maxLength)
        {
            data[dataLength] = (uint)(value & 0xFFFFFFFF);
            value >>= 32;
            dataLength++;
        }

        if (value != 0 || (data[maxLength - 1] & 0x80000000) != 0)
        {
            throw new ArithmeticException("构造函数中的正溢出。");
        }

        if (dataLength == 0)
        {
            dataLength = 1;
        }
    }

    /// <summary>
    /// 使用指定的 BigInteger 实例初始化一个新的 BigInteger 实例。
    /// </summary>
    /// <param name="bi">要复制的 BigInteger 实例。</param>
    public BigInteger(BigInteger bi)
    {
        data = new uint[maxLength];

        dataLength = bi.dataLength;

        for (var i = 0; i < dataLength; i++)
        {
            data[i] = bi.data[i];
        }
    }

    /// <summary>
    /// 使用指定基数的数字字符串初始化一个新的 BigInteger 实例。
    /// </summary>
    /// <param name="value">表示数字的字符串。</param>
    /// <param name="radix">基数，范围为 2 到 36。</param>
    /// <exception cref="ArithmeticException">当字符串格式无效或发生溢出时引发。</exception>
    /// <remarks>
    /// 示例 (基数 10)
    /// -----------------
    /// 使用默认值 1234 初始化 "a"：
    /// <code>
    /// BigInteger a = new BigInteger("1234", 10);
    /// </code>
    /// 使用默认值 -1234 初始化 "a"：
    /// <code>
    /// BigInteger a = new BigInteger("-1234", 10);
    /// </code>
    /// 示例 (基数 16)
    /// -----------------
    /// 使用默认值 0x1D4F 初始化 "a"：
    /// <code>
    /// BigInteger a = new BigInteger("1D4F", 16);
    /// </code>
    /// 使用默认值 -0x1D4F 初始化 "a"：
    /// <code>
    /// BigInteger a = new BigInteger("-1D4F", 16);
    /// </code>
    /// 注意：字符串值应按照指定格式提供。
    /// </remarks>
    public BigInteger(string value, int radix = 10)
    {
        var multiplier = new BigInteger(1);
        var result = new BigInteger();
        value = value.ToUpper().Trim();
        var limit = 0;

        if (value[0] == '-')
        {
            limit = 1;
        }

        for (var i = value.Length - 1; i >= limit; i--)
        {
            int posVal = value[i];

            if (posVal >= '0' && posVal <= '9')
            {
                posVal -= '0';
            }
            else if (posVal >= 'A' && posVal <= 'Z')
            {
                posVal = posVal - 'A' + 10;
            }
            else
            {
                posVal = 9999999; // arbitrary large
            }

            if (posVal >= radix)
            {
                throw new ArithmeticException("构造函数中的无效字符串。");
            }

            if (value[0] == '-')
            {
                posVal = -posVal;
            }

            result = result + multiplier * posVal;

            if (i - 1 >= limit)
            {
                multiplier = multiplier * radix;
            }
        }

        if (value[0] == '-') // 处理负值
        {
            if ((result.data[maxLength - 1] & 0x80000000) == 0)
            {
                throw new ArithmeticException("构造函数中的负溢出。");
            }
        }
        else // 处理正值
        {
            if ((result.data[maxLength - 1] & 0x80000000) != 0)
            {
                throw new ArithmeticException("构造函数中的正溢出。");
            }
        }

        data = new uint[maxLength];
        for (var i = 0; i < result.dataLength; i++)
        {
            data[i] = result.data[i];
        }

        dataLength = result.dataLength;
    }

    /// <summary>
    /// 使用字节数组初始化一个新的 BigInteger 实例。
    /// </summary>
    /// <param name="inData">包含数字的字节数组。</param>
    /// <exception cref="ArithmeticException">当字节溢出时引发。</exception>
    public BigInteger(byte[] inData)
    {
        dataLength = inData.Length >> 2;

        var leftOver = inData.Length & 0x3;
        if (leftOver != 0) // 长度不是 4 的倍数
        {
            dataLength++;
        }

        if (dataLength > maxLength)
        {
            throw new ArithmeticException("构造函数中的字节溢出。");
        }

        data = new uint[maxLength];

        for (int i = inData.Length - 1, j = 0; i >= 3; i -= 4, j++)
        {
            data[j] = (uint)((inData[i - 3] << 24) + (inData[i - 2] << 16) +
                             (inData[i - 1] << 8) + inData[i]);
        }

        if (leftOver == 1)
        {
            data[dataLength - 1] = inData[0];
        }
        else if (leftOver == 2)
        {
            data[dataLength - 1] = (uint)((inData[0] << 8) + inData[1]);
        }
        else if (leftOver == 3)
        {
            data[dataLength - 1] = (uint)((inData[0] << 16) + (inData[1] << 8) + inData[2]);
        }

        while (dataLength > 1 && data[dataLength - 1] == 0)
        {
            dataLength--;
        }

        //LogHelper.Info("Len = " + dataLength);
    }

    /// <summary>
    /// 使用指定长度的字节数组初始化一个新的 BigInteger 实例。
    /// </summary>
    /// <param name="inData">包含数字的字节数组。</param>
    /// <param name="inLen">字节数组的长度。</param>
    /// <exception cref="ArithmeticException">当字节溢出时引发。</exception>
    public BigInteger(byte[] inData, int inLen)
    {
        dataLength = inLen >> 2;

        var leftOver = inLen & 0x3;
        if (leftOver != 0) // 长度不是 4 的倍数
        {
            dataLength++;
        }

        if (dataLength > maxLength || inLen > inData.Length)
        {
            throw new ArithmeticException("构造函数中的字节溢出。");
        }

        data = new uint[maxLength];

        for (int i = inLen - 1, j = 0; i >= 3; i -= 4, j++)
        {
            data[j] = (uint)((inData[i - 3] << 24) + (inData[i - 2] << 16) +
                             (inData[i - 1] << 8) + inData[i]);
        }

        if (leftOver == 1)
        {
            data[dataLength - 1] = inData[0];
        }
        else if (leftOver == 2)
        {
            data[dataLength - 1] = (uint)((inData[0] << 8) + inData[1]);
        }
        else if (leftOver == 3)
        {
            data[dataLength - 1] = (uint)((inData[0] << 16) + (inData[1] << 8) + inData[2]);
        }

        if (dataLength == 0)
        {
            dataLength = 1;
        }

        while (dataLength > 1 && data[dataLength - 1] == 0)
        {
            dataLength--;
        }

        //LogHelper.Info("Len = " + dataLength);
    }

    /// <summary>
    /// 使用无符号整数数组初始化一个新的 <see cref="BigInteger"/> 实例。
    /// </summary>
    /// <param name="inData">无符号整数数组，表示大整数的值。</param>
    /// <exception cref="ArithmeticException">当输入数据的长度超过最大长度时引发。</exception>
    public BigInteger(uint[] inData)
    {
        dataLength = inData.Length;

        if (dataLength > maxLength)
        {
            throw new ArithmeticException("构造函数中的字节溢出。");
        }

        data = new uint[maxLength];

        for (int i = dataLength - 1, j = 0; i >= 0; i--, j++)
        {
            data[j] = inData[i];
        }

        while (dataLength > 1 && data[dataLength - 1] == 0)
        {
            dataLength--;
        }

        //LogHelper.Info("Len = " + dataLength);
    }


    /// <summary>
    /// 将 <see cref="long"/> 类型的值隐式转换为 <see cref="BigInteger"/>。
    /// </summary>
    /// <param name="value">要转换的 <see cref="long"/> 值。</param>
    /// <returns>转换后的 <see cref="BigInteger"/> 实例。</returns>
    public static implicit operator BigInteger(long value)
    {
        return new BigInteger(value);
    }

    /// <summary>
    /// 将 <see cref="ulong"/> 类型的值隐式转换为 <see cref="BigInteger"/>。
    /// </summary>
    /// <param name="value">要转换的 <see cref="ulong"/> 值。</param>
    /// <returns>转换后的 <see cref="BigInteger"/> 实例。</returns>
    public static implicit operator BigInteger(ulong value)
    {
        return new BigInteger(value);
    }

    /// <summary>
    /// 将 <see cref="int"/> 类型的值隐式转换为 <see cref="BigInteger"/>。
    /// </summary>
    /// <param name="value">要转换的 <see cref="int"/> 值。</param>
    /// <returns>转换后的 <see cref="BigInteger"/> 实例。</returns>
    public static implicit operator BigInteger(int value)
    {
        return new BigInteger(value);
    }

    /// <summary>
    /// 将 <see cref="uint"/> 类型的值隐式转换为 <see cref="BigInteger"/>。
    /// </summary>
    /// <param name="value">要转换的 <see cref="uint"/> 值。</param>
    /// <returns>转换后的 <see cref="BigInteger"/> 实例。</returns>
    public static implicit operator BigInteger(uint value)
    {
        return new BigInteger((ulong)value);
    }


    /// <summary>
    /// 重载加法运算符。
    /// </summary>
    /// <param name="bi1">第一个 <see cref="BigInteger"/> 实例。</param>
    /// <param name="bi2">第二个 <see cref="BigInteger"/> 实例。</param>
    /// <returns>两个 <see cref="BigInteger"/> 实例的和。</returns>
    /// <exception cref="ArithmeticException">当加法溢出时引发。</exception>
    public static BigInteger operator +(BigInteger bi1, BigInteger bi2)
    {
        var result = new BigInteger();

        result.dataLength = bi1.dataLength > bi2.dataLength ? bi1.dataLength : bi2.dataLength;

        long carry = 0;
        for (var i = 0; i < result.dataLength; i++)
        {
            var sum = bi1.data[i] + (long)bi2.data[i] + carry;
            carry = sum >> 32;
            result.data[i] = (uint)(sum & 0xFFFFFFFF);
        }

        if (carry != 0 && result.dataLength < maxLength)
        {
            result.data[result.dataLength] = (uint)carry;
            result.dataLength++;
        }

        while (result.dataLength > 1 && result.data[result.dataLength - 1] == 0)
        {
            result.dataLength--;
        }


        // 溢出检查
        var lastPos = maxLength - 1;
        if ((bi1.data[lastPos] & 0x80000000) == (bi2.data[lastPos] & 0x80000000) &&
            (result.data[lastPos] & 0x80000000) != (bi1.data[lastPos] & 0x80000000))
        {
            throw new ArithmeticException();
        }

        return result;
    }


    /// <summary>
    /// 重载一元自增运算符。
    /// </summary>
    /// <param name="bi1">要自增的 <see cref="BigInteger"/> 实例。</param>
    /// <returns>自增后的 <see cref="BigInteger"/> 实例。</returns>
    /// <exception cref="ArithmeticException">当自增溢出时引发。</exception>
    public static BigInteger operator ++(BigInteger bi1)
    {
        var result = new BigInteger(bi1);

        long val, carry = 1;
        var index = 0;

        while (carry != 0 && index < maxLength)
        {
            val = result.data[index];
            val++;

            result.data[index] = (uint)(val & 0xFFFFFFFF);
            carry = val >> 32;

            index++;
        }

        if (index > result.dataLength)
        {
            result.dataLength = index;
        }
        else
        {
            while (result.dataLength > 1 && result.data[result.dataLength - 1] == 0)
            {
                result.dataLength--;
            }
        }

        // 溢出检查
        var lastPos = maxLength - 1;

        // 如果初始值为正，但自增导致符号变化为负，则发生溢出。
        if ((bi1.data[lastPos] & 0x80000000) == 0 &&
            (result.data[lastPos] & 0x80000000) != (bi1.data[lastPos] & 0x80000000))
        {
            throw new ArithmeticException("自增溢出。");
        }

        return result;
    }

    /// <summary>
    /// 重载减法运算符。
    /// </summary>
    /// <param name="bi1">第一个 <see cref="BigInteger"/> 实例。</param>
    /// <param name="bi2">第二个 <see cref="BigInteger"/> 实例。</param>
    /// <returns>两个 <see cref="BigInteger"/> 实例的差。</returns>
    /// <exception cref="ArithmeticException">当减法溢出时引发。</exception>
    public static BigInteger operator -(BigInteger bi1, BigInteger bi2)
    {
        var result = new BigInteger();

        result.dataLength = bi1.dataLength > bi2.dataLength ? bi1.dataLength : bi2.dataLength;

        long carryIn = 0;
        for (var i = 0; i < result.dataLength; i++)
        {
            long diff;

            diff = bi1.data[i] - (long)bi2.data[i] - carryIn;
            result.data[i] = (uint)(diff & 0xFFFFFFFF);

            if (diff < 0)
            {
                carryIn = 1;
            }
            else
            {
                carryIn = 0;
            }
        }

        // 处理负数溢出
        if (carryIn != 0)
        {
            for (var i = result.dataLength; i < maxLength; i++)
            {
                result.data[i] = 0xFFFFFFFF;
            }

            result.dataLength = maxLength;
        }

        // 修复在 v1.03 中的错误，以正确给出 a - (-b) 的数据长度
        while (result.dataLength > 1 && result.data[result.dataLength - 1] == 0)
        {
            result.dataLength--;
        }

        // 溢出检查
        var lastPos = maxLength - 1;
        if ((bi1.data[lastPos] & 0x80000000) != (bi2.data[lastPos] & 0x80000000) &&
            (result.data[lastPos] & 0x80000000) != (bi1.data[lastPos] & 0x80000000))
        {
            throw new ArithmeticException();
        }

        return result;
    }

    /// <summary>
    /// 重载一元自减运算符。
    /// </summary>
    /// <param name="bi1">要自减的 <see cref="BigInteger"/> 实例。</param>
    /// <returns>自减后的 <see cref="BigInteger"/> 实例。</returns>
    /// <exception cref="ArithmeticException">当自减溢出时引发。</exception>
    public static BigInteger operator --(BigInteger bi1)
    {
        var result = new BigInteger(bi1);

        long val;
        var carryIn = true;
        var index = 0;

        while (carryIn && index < maxLength)
        {
            val = result.data[index];
            val--;

            result.data[index] = (uint)(val & 0xFFFFFFFF);

            if (val >= 0)
            {
                carryIn = false;
            }

            index++;
        }

        if (index > result.dataLength)
        {
            result.dataLength = index;
        }

        while (result.dataLength > 1 && result.data[result.dataLength - 1] == 0)
        {
            result.dataLength--;
        }

        // 溢出检查
        var lastPos = maxLength - 1;

        // 如果初始值为负，但自减导致符号变化为正，则发生溢出。
        if ((bi1.data[lastPos] & 0x80000000) != 0 &&
            (result.data[lastPos] & 0x80000000) != (bi1.data[lastPos] & 0x80000000))
        {
            throw new ArithmeticException("自减下溢。");
        }

        return result;
    }

    /// <summary>
    /// 重载乘法运算符。
    /// </summary>
    /// <param name="bi1">第一个 <see cref="BigInteger"/> 实例。</param>
    /// <param name="bi2">第二个 <see cref="BigInteger"/> 实例。</param>
    /// <returns>两个 <see cref="BigInteger"/> 实例的乘积。</returns>
    /// <exception cref="ArithmeticException">当乘法溢出时引发。</exception>
    public static BigInteger operator *(BigInteger bi1, BigInteger bi2)
    {
        var lastPos = maxLength - 1;
        bool bi1Neg = false, bi2Neg = false;

        // 取输入的绝对值
        try
        {
            if ((bi1.data[lastPos] & 0x80000000) != 0) // bi1 为负
            {
                bi1Neg = true;
                bi1 = -bi1;
            }

            if ((bi2.data[lastPos] & 0x80000000) != 0) // bi2 为负
            {
                bi2Neg = true;
                bi2 = -bi2;
            }
        }
        catch (Exception)
        {
        }

        var result = new BigInteger();

        // 乘以绝对值
        try
        {
            for (var i = 0; i < bi1.dataLength; i++)
            {
                if (bi1.data[i] == 0)
                {
                    continue;
                }

                ulong mcarry = 0;
                for (int j = 0, k = i; j < bi2.dataLength; j++, k++)
                {
                    // k = i + j
                    var val = bi1.data[i] * (ulong)bi2.data[j] +
                              result.data[k] + mcarry;

                    result.data[k] = (uint)(val & 0xFFFFFFFF);
                    mcarry = val >> 32;
                }

                if (mcarry != 0)
                {
                    result.data[i + bi2.dataLength] = (uint)mcarry;
                }
            }
        }
        catch (Exception)
        {
            throw new ArithmeticException("乘法溢出。");
        }


        result.dataLength = bi1.dataLength + bi2.dataLength;
        if (result.dataLength > maxLength)
        {
            result.dataLength = maxLength;
        }

        while (result.dataLength > 1 && result.data[result.dataLength - 1] == 0)
        {
            result.dataLength--;
        }

        // 溢出检查（结果为负）
        if ((result.data[lastPos] & 0x80000000) != 0)
        {
            if (bi1Neg != bi2Neg && result.data[lastPos] == 0x80000000) // 符号不同
            {
                // 处理乘法产生最大负数的特殊情况
                if (result.dataLength == 1)
                {
                    return result;
                }

                var isMaxNeg = true;
                for (var i = 0; i < result.dataLength - 1 && isMaxNeg; i++)
                {
                    if (result.data[i] != 0)
                    {
                        isMaxNeg = false;
                    }
                }

                if (isMaxNeg)
                {
                    return result;
                }
            }

            throw new ArithmeticException("乘法溢出。");
        }

        // 如果输入符号不同，则结果为负
        if (bi1Neg != bi2Neg)
        {
            return -result;
        }

        return result;
    }


    /// <summary>
    /// 重载左移运算符。
    /// </summary>
    /// <param name="bi1">要左移的 <see cref="BigInteger"/> 实例。</param>
    /// <param name="shiftVal">左移的位数。</param>
    /// <returns>左移后的 <see cref="BigInteger"/> 实例。</returns>
    public static BigInteger operator <<(BigInteger bi1, int shiftVal)
    {
        var result = new BigInteger(bi1);
        result.dataLength = shiftLeft(result.data, shiftVal);

        return result;
    }


    /// <summary>
    /// 将缓冲区中的位左移指定的位数。
    /// </summary>
    /// <param name="buffer">要左移的缓冲区。</param>
    /// <param name="shiftVal">左移的位数。</param>
    /// <returns>左移后的缓冲区长度。</returns>
    private static int shiftLeft(uint[] buffer, int shiftVal)
    {
        var shiftAmount = 32;
        var bufLen = buffer.Length;

        while (bufLen > 1 && buffer[bufLen - 1] == 0)
        {
            bufLen--;
        }

        for (var count = shiftVal; count > 0;)
        {
            if (count < shiftAmount)
            {
                shiftAmount = count;
            }

            //LogHelper.Info("shiftAmount = {0}", shiftAmount);

            ulong carry = 0;
            for (var i = 0; i < bufLen; i++)
            {
                var val = (ulong)buffer[i] << shiftAmount;
                val |= carry;

                buffer[i] = (uint)(val & 0xFFFFFFFF);
                carry = val >> 32;
            }

            if (carry != 0)
            {
                if (bufLen + 1 <= buffer.Length)
                {
                    buffer[bufLen] = (uint)carry;
                    bufLen++;
                }
            }

            count -= shiftAmount;
        }

        return bufLen;
    }


    /// <summary>
    /// 重载右移运算符。
    /// </summary>
    /// <param name="bi1">要右移的 <see cref="BigInteger"/> 实例。</param>
    /// <param name="shiftVal">右移的位数。</param>
    /// <returns>右移后的 <see cref="BigInteger"/> 实例。</returns>
    public static BigInteger operator >> (BigInteger bi1, int shiftVal)
    {
        var result = new BigInteger(bi1);
        result.dataLength = shiftRight(result.data, shiftVal);


        if ((bi1.data[maxLength - 1] & 0x80000000) != 0) // 负数
        {
            for (var i = maxLength - 1; i >= result.dataLength; i--)
            {
                result.data[i] = 0xFFFFFFFF;
            }

            var mask = 0x80000000;
            for (var i = 0; i < 32; i++)
            {
                if ((result.data[result.dataLength - 1] & mask) != 0)
                {
                    break;
                }

                result.data[result.dataLength - 1] |= mask;
                mask >>= 1;
            }

            result.dataLength = maxLength;
        }

        return result;
    }


    /// <summary>
    /// 将缓冲区中的位右移指定的位数。
    /// </summary>
    /// <param name="buffer">要右移的缓冲区。</param>
    /// <param name="shiftVal">右移的位数。</param>
    /// <returns>右移后的缓冲区长度。</returns>
    private static int shiftRight(uint[] buffer, int shiftVal)
    {
        var shiftAmount = 32;
        var invShift = 0;
        var bufLen = buffer.Length;

        while (bufLen > 1 && buffer[bufLen - 1] == 0)
        {
            bufLen--;
        }

        //LogHelper.Info("bufLen = " + bufLen + " buffer.Length = " + buffer.Length);

        for (var count = shiftVal; count > 0;)
        {
            if (count < shiftAmount)
            {
                shiftAmount = count;
                invShift = 32 - shiftAmount;
            }

            //LogHelper.Info("shiftAmount = {0}", shiftAmount);

            ulong carry = 0;
            for (var i = bufLen - 1; i >= 0; i--)
            {
                var val = (ulong)buffer[i] >> shiftAmount;
                val |= carry;

                carry = (ulong)buffer[i] << invShift;
                buffer[i] = (uint)val;
            }

            count -= shiftAmount;
        }

        while (bufLen > 1 && buffer[bufLen - 1] == 0)
        {
            bufLen--;
        }

        return bufLen;
    }

    /// <summary>
    /// 重载按位取反运算符（1 的补码）。
    /// </summary>
    /// <param name="bi1">要取反的 <see cref="BigInteger"/> 实例。</param>
    /// <returns>取反后的 <see cref="BigInteger"/> 实例。</returns>
    public static BigInteger operator ~(BigInteger bi1)
    {
        var result = new BigInteger(bi1);

        for (var i = 0; i < maxLength; i++)
        {
            result.data[i] = ~bi1.data[i];
        }

        result.dataLength = maxLength;

        while (result.dataLength > 1 && result.data[result.dataLength - 1] == 0)
        {
            result.dataLength--;
        }

        return result;
    }


    /// <summary>
    /// 重载取负运算符（2 的补码）。
    /// </summary>
    /// <param name="bi1">要取负的 <see cref="BigInteger"/> 实例。</param>
    /// <returns>取负后的 <see cref="BigInteger"/> 实例。</returns>
    /// <exception cref="ArithmeticException">当取负溢出时引发。</exception>
    public static BigInteger operator -(BigInteger bi1)
    {
        // 处理零的取负情况，因为这会导致溢出
        if (bi1.dataLength == 1 && bi1.data[0] == 0)
        {
            return new BigInteger();
        }

        var result = new BigInteger(bi1);

        // 1 的补码
        for (var i = 0; i < maxLength; i++)
        {
            result.data[i] = ~bi1.data[i];
        }

        // 对 1 的补码结果加 1
        long val, carry = 1;
        var index = 0;

        while (carry != 0 && index < maxLength)
        {
            val = result.data[index];
            val++;

            result.data[index] = (uint)(val & 0xFFFFFFFF);
            carry = val >> 32;

            index++;
        }

        if ((bi1.data[maxLength - 1] & 0x80000000) == (result.data[maxLength - 1] & 0x80000000))
        {
            throw new ArithmeticException("取负溢出。\n");
        }

        result.dataLength = maxLength;

        while (result.dataLength > 1 && result.data[result.dataLength - 1] == 0)
        {
            result.dataLength--;
        }

        return result;
    }


    /// <summary>
    /// 重载相等运算符。
    /// </summary>
    /// <param name="bi1">第一个 <see cref="BigInteger"/> 实例。</param>
    /// <param name="bi2">第二个 <see cref="BigInteger"/> 实例。</param>
    /// <returns>如果两个 <see cref="BigInteger"/> 实例相等，则返回 true；否则返回 false。</returns>
    public static bool operator ==(BigInteger bi1, BigInteger bi2)
    {
        return bi1.Equals(bi2);
    }

    /// <summary>
    /// 比较当前 <see cref="BigInteger"/> 实例与另一个 <see cref="BigInteger"/> 实例的大小。
    /// </summary>
    /// <param name="other">要比较的另一个 <see cref="BigInteger"/> 实例。</param>
    /// <returns>如果当前实例大于 <paramref name="other"/>，则返回 1；如果小于，则返回 -1；否则返回 0。</returns>
    public int CompareTo(BigInteger other)
    {
        if (this > other)
        {
            return 1;
        }

        if (this < other)
        {
            return -1;
        }

        return 0;
    }


    /// <summary>
    /// 重载不相等运算符。
    /// </summary>
    /// <param name="bi1">第一个 <see cref="BigInteger"/> 实例。</param>
    /// <param name="bi2">第二个 <see cref="BigInteger"/> 实例。</param>
    /// <returns>如果两个 <see cref="BigInteger"/> 实例不相等，则返回 true；否则返回 false。</returns>
    public static bool operator !=(BigInteger bi1, BigInteger bi2)
    {
        return !bi1.Equals(bi2);
    }


    /// <summary>
    /// 重写 <see cref="object.Equals(object?)"/> 方法。
    /// </summary>
    /// <param name="o">要比较的对象。</param>
    /// <returns>如果当前实例与 <paramref name="o"/> 相等，则返回 true；否则返回 false。</returns>
    public override bool Equals(object o)
    {
        var bi = (BigInteger)o;

        if (dataLength != bi.dataLength)
        {
            return false;
        }

        for (var i = 0; i < dataLength; i++)
        {
            if (data[i] != bi.data[i])
            {
                return false;
            }
        }

        return true;
    }


    /// <summary>
    /// 作为默认哈希函数。
    /// </summary>
    /// <returns>当前对象的哈希代码。</returns>
    public override int GetHashCode()
    {
        return ToString().GetHashCode();
    }


    /// <summary>
    /// 重载大于运算符。
    /// </summary>
    /// <param name="bi1">第一个 <see cref="BigInteger"/> 实例。</param>
    /// <param name="bi2">第二个 <see cref="BigInteger"/> 实例。</param>
    /// <returns>如果 <paramref name="bi1"/> 大于 <paramref name="bi2"/>，则返回 true；否则返回 false。</returns>
    public static bool operator >(BigInteger bi1, BigInteger bi2)
    {
        var pos = maxLength - 1;

        // bi1 为负，bi2 为正
        if ((bi1.data[pos] & 0x80000000) != 0 && (bi2.data[pos] & 0x80000000) == 0)
        {
            return false;
        }

        // bi1 为正，bi2 为负
        if ((bi1.data[pos] & 0x80000000) == 0 && (bi2.data[pos] & 0x80000000) != 0)
        {
            return true;
        }

        // 同符号
        var len = bi1.dataLength > bi2.dataLength ? bi1.dataLength : bi2.dataLength;
        for (pos = len - 1; pos >= 0 && bi1.data[pos] == bi2.data[pos]; pos--)
        {
            ;
        }

        if (pos >= 0)
        {
            if (bi1.data[pos] > bi2.data[pos])
            {
                return true;
            }

            return false;
        }

        return false;
    }


    /// <summary>
    /// 重载小于运算符。
    /// </summary>
    /// <param name="bi1">第一个 <see cref="BigInteger"/> 实例。</param>
    /// <param name="bi2">第二个 <see cref="BigInteger"/> 实例。</param>
    /// <returns>如果 <paramref name="bi1"/> 小于 <paramref name="bi2"/>，则返回 true；否则返回 false。</returns>
    public static bool operator <(BigInteger bi1, BigInteger bi2)
    {
        var pos = maxLength - 1;

        // bi1 为负，bi2 为正
        if ((bi1.data[pos] & 0x80000000) != 0 && (bi2.data[pos] & 0x80000000) == 0)
        {
            return true;
        }

        // bi1 为正，bi2 为负
        if ((bi1.data[pos] & 0x80000000) == 0 && (bi2.data[pos] & 0x80000000) != 0)
        {
            return false;
        }

        // 同符号
        var len = bi1.dataLength > bi2.dataLength ? bi1.dataLength : bi2.dataLength;
        for (pos = len - 1; pos >= 0 && bi1.data[pos] == bi2.data[pos]; pos--)
        {
            ;
        }

        if (pos >= 0)
        {
            if (bi1.data[pos] < bi2.data[pos])
            {
                return true;
            }

            return false;
        }

        return false;
    }


    /// <summary>
    /// 重载大于等于运算符。
    /// </summary>
    /// <param name="bi1">第一个 <see cref="BigInteger"/> 实例。</param>
    /// <param name="bi2">第二个 <see cref="BigInteger"/> 实例。</param>
    /// <returns>如果 <paramref name="bi1"/> 大于或等于 <paramref name="bi2"/>，则返回 true；否则返回 false。</returns>
    public static bool operator >=(BigInteger bi1, BigInteger bi2)
    {
        return bi1 == bi2 || bi1 > bi2;
    }


    /// <summary>
    /// 重载小于等于运算符。
    /// </summary>
    /// <param name="bi1">第一个 <see cref="BigInteger"/> 实例。</param>
    /// <param name="bi2">第二个 <see cref="BigInteger"/> 实例。</param>
    /// <returns>如果 <paramref name="bi1"/> 小于或等于 <paramref name="bi2"/>，则返回 true；否则返回 false。</returns>
    public static bool operator <=(BigInteger bi1, BigInteger bi2)
    {
        return bi1 == bi2 || bi1 < bi2;
    }

    //***********************************************************************
    // 私有函数，支持两个数字的除法运算，其中除数为多位数。
    //
    // 算法来源于 [1]
    //***********************************************************************

    private static void MultiByteDivide(BigInteger bi1, BigInteger bi2,
        BigInteger outQuotient, BigInteger outRemainder)
    {
        var result = new uint[maxLength];

        var remainderLen = bi1.dataLength + 1;
        var remainder = new uint[remainderLen];

        var mask = 0x80000000;
        var val = bi2.data[bi2.dataLength - 1];
        int shift = 0, resultPos = 0;

        // 计算除数的最高位的有效位数
        while (mask != 0 && (val & mask) == 0)
        {
            shift++;
            mask >>= 1;
        }

        // 将被除数复制到余数中
        for (var i = 0; i < bi1.dataLength; i++)
        {
            remainder[i] = bi1.data[i];
        }

        // 左移余数以适应除数
        shiftLeft(remainder, shift);
        bi2 = bi2 << shift;

        var j = remainderLen - bi2.dataLength;
        var pos = remainderLen - 1;

        ulong firstDivisorByte = bi2.data[bi2.dataLength - 1];
        ulong secondDivisorByte = bi2.data[bi2.dataLength - 2];

        var divisorLen = bi2.dataLength + 1;
        var dividendPart = new uint[divisorLen];

        // 进行除法运算
        while (j > 0)
        {
            var dividend = ((ulong)remainder[pos] << 32) + remainder[pos - 1];

            var q_hat = dividend / firstDivisorByte;
            var r_hat = dividend % firstDivisorByte;

            var done = false;
            while (!done)
            {
                done = true;

                // 调整 q_hat 以确保不溢出
                if (q_hat == 0x100000000 ||
                    q_hat * secondDivisorByte > (r_hat << 32) + remainder[pos - 2])
                {
                    q_hat--;
                    r_hat += firstDivisorByte;

                    if (r_hat < 0x100000000)
                    {
                        done = false;
                    }
                }
            }

            for (var h = 0; h < divisorLen; h++)
            {
                dividendPart[h] = remainder[pos - h];
            }

            var kk = new BigInteger(dividendPart);
            var ss = bi2 * (long)q_hat;

            // 确保 ss 不大于 kk
            while (ss > kk)
            {
                q_hat--;
                ss -= bi2;
            }

            var yy = kk - ss;

            for (var h = 0; h < divisorLen; h++)
            {
                remainder[pos - h] = yy.data[bi2.dataLength - h];
            }

            result[resultPos++] = (uint)q_hat;

            pos--;
            j--;
        }

        // 设置输出商的长度
        outQuotient.dataLength = resultPos;
        var y = 0;
        for (var x = outQuotient.dataLength - 1; x >= 0; x--, y++)
        {
            outQuotient.data[y] = result[x];
        }

        for (; y < maxLength; y++)
        {
            outQuotient.data[y] = 0;
        }

        // 清理商的多余零
        while (outQuotient.dataLength > 1 && outQuotient.data[outQuotient.dataLength - 1] == 0)
        {
            outQuotient.dataLength--;
        }

        if (outQuotient.dataLength == 0)
        {
            outQuotient.dataLength = 1;
        }

        // 计算余数
        outRemainder.dataLength = shiftRight(remainder, shift);

        for (y = 0; y < outRemainder.dataLength; y++)
        {
            outRemainder.data[y] = remainder[y];
        }

        for (; y < maxLength; y++)
        {
            outRemainder.data[y] = 0;
        }
    }


    //***********************************************************************
    // 私有函数，支持两个数字的除法运算，其中除数为单个数字。
    //***********************************************************************

    private static void SingleByteDivide(BigInteger bi1, BigInteger bi2,
        BigInteger outQuotient, BigInteger outRemainder)
    {
        var result = new uint[maxLength];
        var resultPos = 0;

        // 将被除数复制到余数中
        for (var i = 0; i < maxLength; i++)
        {
            outRemainder.data[i] = bi1.data[i];
        }

        outRemainder.dataLength = bi1.dataLength;

        // 清理余数的多余零
        while (outRemainder.dataLength > 1 && outRemainder.data[outRemainder.dataLength - 1] == 0)
        {
            outRemainder.dataLength--;
        }

        ulong divisor = bi2.data[0];
        var pos = outRemainder.dataLength - 1;
        ulong dividend = outRemainder.data[pos];

        // 如果被除数大于等于除数，进行除法运算
        if (dividend >= divisor)
        {
            var quotient = dividend / divisor;
            result[resultPos++] = (uint)quotient;

            outRemainder.data[pos] = (uint)(dividend % divisor);
        }

        pos--;

        // 继续处理余数
        while (pos >= 0)
        {
            dividend = ((ulong)outRemainder.data[pos + 1] << 32) + outRemainder.data[pos];
            var quotient = dividend / divisor;
            result[resultPos++] = (uint)quotient;

            outRemainder.data[pos + 1] = 0;
            outRemainder.data[pos--] = (uint)(dividend % divisor);
        }

        // 设置输出商的长度
        outQuotient.dataLength = resultPos;
        var j = 0;
        for (var i = outQuotient.dataLength - 1; i >= 0; i--, j++)
        {
            outQuotient.data[j] = result[i];
        }

        for (; j < maxLength; j++)
        {
            outQuotient.data[j] = 0;
        }

        // 清理商的多余零
        while (outQuotient.dataLength > 1 && outQuotient.data[outQuotient.dataLength - 1] == 0)
        {
            outQuotient.dataLength--;
        }

        if (outQuotient.dataLength == 0)
        {
            outQuotient.dataLength = 1;
        }

        // 清理余数的多余零
        while (outRemainder.dataLength > 1 && outRemainder.data[outRemainder.dataLength - 1] == 0)
        {
            outRemainder.dataLength--;
        }
    }


    /// <summary>
    /// 重载除法运算符
    /// </summary>
    /// <param name="bi1">被除数</param>
    /// <param name="bi2">除数</param>
    /// <returns>返回商</returns>
    public static BigInteger operator /(BigInteger bi1, BigInteger bi2)
    {
        var quotient = new BigInteger();
        var remainder = new BigInteger();

        var lastPos = maxLength - 1;
        bool divisorNeg = false, dividendNeg = false;

        if ((bi1.data[lastPos] & 0x80000000) != 0) // bi1 为负数
        {
            bi1 = -bi1;
            dividendNeg = true;
        }

        if ((bi2.data[lastPos] & 0x80000000) != 0) // bi2 为负数
        {
            bi2 = -bi2;
            divisorNeg = true;
        }

        if (bi1 < bi2)
        {
            return quotient; // 如果被除数小于除数，商为0
        }

        if (bi2.dataLength == 1)
        {
            SingleByteDivide(bi1, bi2, quotient, remainder);
        }
        else
        {
            MultiByteDivide(bi1, bi2, quotient, remainder);
        }

        if (dividendNeg != divisorNeg)
        {
            return -quotient; // 如果被除数和除数符号不同，商为负
        }

        return quotient;
    }


    /// <summary>
    /// 重载取模运算符
    /// </summary>
    /// <param name="bi1">被取模数</param>
    /// <param name="bi2">模数</param>
    /// <returns>返回余数</returns>
    public static BigInteger operator %(BigInteger bi1, BigInteger bi2)
    {
        var quotient = new BigInteger();
        var remainder = new BigInteger(bi1);

        var lastPos = maxLength - 1;
        var dividendNeg = false;

        if ((bi1.data[lastPos] & 0x80000000) != 0) // bi1 为负数
        {
            bi1 = -bi1;
            dividendNeg = true;
        }

        if ((bi2.data[lastPos] & 0x80000000) != 0) // bi2 为负数
        {
            bi2 = -bi2;
        }

        if (bi1 < bi2)
        {
            return remainder; // 如果被取模数小于模数，余数为被取模数
        }

        if (bi2.dataLength == 1)
        {
            SingleByteDivide(bi1, bi2, quotient, remainder);
        }
        else
        {
            MultiByteDivide(bi1, bi2, quotient, remainder);
        }

        if (dividendNeg)
        {
            return -remainder; // 如果被取模数为负，余数为负
        }

        return remainder;
    }


    /// <summary>
    /// 重载按位与运算符
    /// </summary>
    /// <param name="bi1">第一个操作数</param>
    /// <param name="bi2">第二个操作数</param>
    /// <returns>返回按位与的结果</returns>
    public static BigInteger operator &(BigInteger bi1, BigInteger bi2)
    {
        var result = new BigInteger();

        var len = bi1.dataLength > bi2.dataLength ? bi1.dataLength : bi2.dataLength;

        for (var i = 0; i < len; i++)
        {
            var sum = bi1.data[i] & bi2.data[i];
            result.data[i] = sum;
        }

        result.dataLength = maxLength;

        while (result.dataLength > 1 && result.data[result.dataLength - 1] == 0)
        {
            result.dataLength--;
        }

        return result;
    }


    /// <summary>
    /// 重载按位或运算符
    /// </summary>
    /// <param name="bi1">第一个操作数</param>
    /// <param name="bi2">第二个操作数</param>
    /// <returns>返回按位或的结果</returns>
    public static BigInteger operator |(BigInteger bi1, BigInteger bi2)
    {
        var result = new BigInteger();

        var len = bi1.dataLength > bi2.dataLength ? bi1.dataLength : bi2.dataLength;

        for (var i = 0; i < len; i++)
        {
            var sum = bi1.data[i] | bi2.data[i];
            result.data[i] = sum;
        }

        result.dataLength = maxLength;

        while (result.dataLength > 1 && result.data[result.dataLength - 1] == 0)
        {
            result.dataLength--;
        }

        return result;
    }


    /// <summary>
    /// 重载按位异或运算符
    /// </summary>
    /// <param name="bi1">第一个操作数</param>
    /// <param name="bi2">第二个操作数</param>
    /// <returns>返回按位异或的结果</returns>
    public static BigInteger operator ^(BigInteger bi1, BigInteger bi2)
    {
        var result = new BigInteger();

        var len = bi1.dataLength > bi2.dataLength ? bi1.dataLength : bi2.dataLength;

        for (var i = 0; i < len; i++)
        {
            var sum = bi1.data[i] ^ bi2.data[i];
            result.data[i] = sum;
        }

        result.dataLength = maxLength;

        while (result.dataLength > 1 && result.data[result.dataLength - 1] == 0)
        {
            result.dataLength--;
        }

        return result;
    }


    /// <summary>
    /// 返回当前对象和指定对象的较大值
    /// </summary>
    /// <param name="bi">要比较的对象</param>
    /// <returns>返回较大值</returns>
    public BigInteger max(BigInteger bi)
    {
        if (this > bi)
        {
            return new BigInteger(this);
        }

        return new BigInteger(bi);
    }


    /// <summary>
    /// 返回当前对象和指定对象的较小值
    /// </summary>
    /// <param name="bi">要比较的对象</param>
    /// <returns>返回较小值</returns>
    public BigInteger min(BigInteger bi)
    {
        if (this < bi)
        {
            return new BigInteger(this);
        }

        return new BigInteger(bi);
    }


    /// <summary>
    /// 返回当前对象的绝对值
    /// </summary>
    /// <returns>返回绝对值</returns>
    public BigInteger abs()
    {
        if ((data[maxLength - 1] & 0x80000000) != 0)
        {
            return -this; // 如果当前对象为负，返回其相反数
        }

        return new BigInteger(this);
    }


    /// <summary>
    /// 返回表示当前 BigInteger 的十进制字符串
    /// </summary>
    /// <returns>返回十进制字符串</returns>
    public override string ToString()
    {
        return ToString(10);
    }


    //***********************************************************************
    // 返回表示当前 BigInteger 在指定基数下的符号和大小格式的字符串。
    //
    // 示例
    // -------
    // 如果 BigInteger 的值为 -255（十进制），则
    // ToString(16) 返回 "-FF"
    //
    //***********************************************************************

    /// <summary>
    /// 返回表示当前 BigInteger 在指定基数下的字符串
    /// </summary>
    /// <param name="radix">基数，范围在 2 到 36 之间</param>
    /// <returns>返回指定基数的字符串表示</returns>
    /// <exception cref="ArgumentException">当基数不在有效范围内时抛出异常</exception>
    public string ToString(int radix)
    {
        if (radix < 2 || radix > 36)
        {
            throw new ArgumentException("Radix must be >= 2 and <= 36");
        }

        var charSet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        var result = "";

        var a = this;

        var negative = false;
        if ((a.data[maxLength - 1] & 0x80000000) != 0)
        {
            negative = true;
            try
            {
                a = -a; // 取反以处理负数
            }
            catch (Exception)
            {
            }
        }

        var quotient = new BigInteger();
        var remainder = new BigInteger();
        var biRadix = new BigInteger(radix);

        if (a.dataLength == 1 && a.data[0] == 0)
        {
            result = "0"; // 如果值为0，返回"0"
        }
        else
        {
            while (a.dataLength > 1 || (a.dataLength == 1 && a.data[0] != 0))
            {
                SingleByteDivide(a, biRadix, quotient, remainder);

                if (remainder.data[0] < 10)
                {
                    result = remainder.data[0] + result; // 处理小于10的余数
                }
                else
                {
                    result = charSet[(int)remainder.data[0] - 10] + result; // 处理大于等于10的余数
                }

                a = quotient; // 更新被除数
            }

            if (negative)
            {
                result = "-" + result; // 如果是负数，添加负号
            }
        }

        return result;
    }


    //***********************************************************************
    // 返回表示 BigInteger 内容的十六进制字符串
    //
    // 示例
    // -------
    // 1) 如果 BigInteger 的值为 255（十进制），则
    //    ToHexString() 返回 "FF"
    //
    // 2) 如果 BigInteger 的值为 -255（十进制），则
    //    ToHexString() 返回 ".....FFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFFF01"， 
    //    这是 -255 的二进制补码表示。
    //
    //***********************************************************************

    /// <summary>
    /// 返回表示 BigInteger 内容的十六进制字符串
    /// </summary>
    /// <returns>返回十六进制字符串</returns>
    public string ToHexString()
    {
        var result = data[dataLength - 1].ToString("X");

        for (var i = dataLength - 2; i >= 0; i--)
        {
            result += data[i].ToString("X8");
        }

        return result;
    }


    /// <summary>
    /// 模幂运算
    /// </summary>
    /// <param name="exp">指数</param>
    /// <param name="n">模数</param>
    /// <returns>返回模幂运算的结果</returns>
    /// <exception cref="ArithmeticException">当指数为负时抛出异常</exception>
    public BigInteger ModPow(BigInteger exp, BigInteger n)
    {
        if ((exp.data[maxLength - 1] & 0x80000000) != 0)
        {
            throw new ArithmeticException("Positive exponents only."); // 仅支持正指数
        }

        BigInteger resultNum = 1;
        BigInteger tempNum;
        var thisNegative = false;

        if ((data[maxLength - 1] & 0x80000000) != 0) // 当前对象为负数
        {
            tempNum = -this % n;
            thisNegative = true;
        }
        else
        {
            tempNum = this % n; // 确保 (tempNum * tempNum) < b^(2k)
        }

        if ((n.data[maxLength - 1] & 0x80000000) != 0) // 模数为负数
        {
            n = -n;
        }

        // 计算常量 = b^(2k) / m
        var constant = new BigInteger();

        var i = n.dataLength << 1;
        constant.data[i] = 0x00000001;
        constant.dataLength = i + 1;

        constant = constant / n;
        var totalBits = exp.BitCount();
        var count = 0;

        // 执行平方和乘法指数运算
        for (var pos = 0; pos < exp.dataLength; pos++)
        {
            uint mask = 0x01;

            for (var index = 0; index < 32; index++)
            {
                if ((exp.data[pos] & mask) != 0)
                {
                    resultNum = BarrettReduction(resultNum * tempNum, n, constant);
                }

                mask <<= 1;

                tempNum = BarrettReduction(tempNum * tempNum, n, constant);

                if (tempNum.dataLength == 1 && tempNum.data[0] == 1)
                {
                    if (thisNegative && (exp.data[0] & 0x1) != 0) // 奇数指数
                    {
                        return -resultNum;
                    }

                    return resultNum;
                }

                count++;
                if (count == totalBits)
                {
                    break;
                }
            }
        }

        if (thisNegative && (exp.data[0] & 0x1) != 0) // 奇数指数
        {
            return -resultNum;
        }

        return resultNum;
    }


    //***********************************************************************
    // 使用 Barrett 减法快速计算模数减法。
    // 要求 x < b^(2k)，其中 b 是基数。在此情况下，基数为
    // 2^32 (uint)。
    //
    // 参考文献 [4]
    //***********************************************************************

    private BigInteger BarrettReduction(BigInteger x, BigInteger n, BigInteger constant)
    {
        int k = n.dataLength,
            kPlusOne = k + 1,
            kMinusOne = k - 1;

        var q1 = new BigInteger();

        // q1 = x / b^(k-1)
        for (int i = kMinusOne, j = 0; i < x.dataLength; i++, j++)
        {
            q1.data[j] = x.data[i];
        }

        q1.dataLength = x.dataLength - kMinusOne;
        if (q1.dataLength <= 0)
        {
            q1.dataLength = 1;
        }


        var q2 = q1 * constant;
        var q3 = new BigInteger();

        // q3 = q2 / b^(k+1)
        for (int i = kPlusOne, j = 0; i < q2.dataLength; i++, j++)
        {
            q3.data[j] = q2.data[i];
        }

        q3.dataLength = q2.dataLength - kPlusOne;
        if (q3.dataLength <= 0)
        {
            q3.dataLength = 1;
        }


        // r1 = x mod b^(k+1)
        // 即保留最低的 (k+1) 个字
        var r1 = new BigInteger();
        var lengthToCopy = x.dataLength > kPlusOne ? kPlusOne : x.dataLength;
        for (var i = 0; i < lengthToCopy; i++)
        {
            r1.data[i] = x.data[i];
        }

        r1.dataLength = lengthToCopy;


        // r2 = (q3 * n) mod b^(k+1)
        // q3 和 n 的部分乘法

        var r2 = new BigInteger();
        for (var i = 0; i < q3.dataLength; i++)
        {
            if (q3.data[i] == 0)
            {
                continue;
            }

            ulong mcarry = 0;
            var t = i;
            for (var j = 0; j < n.dataLength && t < kPlusOne; j++, t++)
            {
                // t = i + j
                var val = q3.data[i] * (ulong)n.data[j] +
                          r2.data[t] + mcarry;

                r2.data[t] = (uint)(val & 0xFFFFFFFF);
                mcarry = val >> 32;
            }

            if (t < kPlusOne)
            {
                r2.data[t] = (uint)mcarry;
            }
        }

        r2.dataLength = kPlusOne;
        while (r2.dataLength > 1 && r2.data[r2.dataLength - 1] == 0)
        {
            r2.dataLength--;
        }

        r1 -= r2;
        if ((r1.data[maxLength - 1] & 0x80000000) != 0) // 负数
        {
            var val = new BigInteger();
            val.data[kPlusOne] = 0x00000001;
            val.dataLength = kPlusOne + 1;
            r1 += val;
        }

        while (r1 >= n)
        {
            r1 -= n; // 确保 r1 小于 n
        }

        return r1;
    }


    /// <summary>
    /// 返回当前对象和指定对象的最大公约数
    /// </summary>
    /// <param name="bi">要比较的对象</param>
    /// <returns>返回最大公约数</returns>
    public BigInteger Gcd(BigInteger bi)
    {
        BigInteger x;
        BigInteger y;

        if ((data[maxLength - 1] & 0x80000000) != 0) // 当前对象为负数
        {
            x = -this;
        }
        else
        {
            x = this;
        }

        if ((bi.data[maxLength - 1] & 0x80000000) != 0) // 指定对象为负数
        {
            y = -bi;
        }
        else
        {
            y = bi;
        }

        var g = y;

        while (x.dataLength > 1 || (x.dataLength == 1 && x.data[0] != 0))
        {
            g = x;
            x = y % x; // 计算余数
            y = g;
        }

        return g; // 返回最大公约数
    }


    /// <summary>
    /// 用指定数量的随机位填充当前对象
    /// </summary>
    /// <param name="bits">要生成的位数</param>
    /// <param name="rand">随机数生成器</param>
    /// <exception cref="ArithmeticException">当所需位数超过最大长度时抛出异常</exception>
    public void GenRandomBits(int bits, System.Random rand)
    {
        var dwords = bits >> 5;
        var remBits = bits & 0x1F;

        if (remBits != 0)
        {
            dwords++;
        }

        if (dwords > maxLength)
        {
            throw new ArithmeticException("Number of required bits > maxLength."); // 超过最大长度
        }

        for (var i = 0; i < dwords; i++)
        {
            data[i] = (uint)(rand.NextDouble() * 0x100000000);
        }

        for (var i = dwords; i < maxLength; i++)
        {
            data[i] = 0; // 填充剩余部分为0
        }

        if (remBits != 0)
        {
            var mask = (uint)(0x01 << (remBits - 1));
            data[dwords - 1] |= mask;

            mask = 0xFFFFFFFF >> (32 - remBits);
            data[dwords - 1] &= mask;
        }
        else
        {
            data[dwords - 1] |= 0x80000000; // 设置最高位
        }

        dataLength = dwords;

        if (dataLength == 0)
        {
            dataLength = 1; // 确保数据长度至少为1
        }
    }


    //***********************************************************************
    // Returns the position of the most significant bit in the BigInteger.
    //
    // Eg.  The result is 0, if the value of BigInteger is 0...0000 0000
    //      The result is 1, if the value of BigInteger is 0...0000 0001
    //      The result is 2, if the value of BigInteger is 0...0000 0010
    //      The result is 2, if the value of BigInteger is 0...0000 0011
    //
    //***********************************************************************

    /// <summary>
    /// bitCount
    /// </summary>
    /// <returns></returns>
    public int BitCount()
    {
        while (dataLength > 1 && data[dataLength - 1] == 0)
        {
            dataLength--;
        }

        var value = data[dataLength - 1];
        var mask = 0x80000000;
        var bits = 32;

        while (bits > 0 && (value & mask) == 0)
        {
            bits--;
            mask >>= 1;
        }

        bits += (dataLength - 1) << 5;

        return bits;
    }


    //***********************************************************************
    // Probabilistic prime test based on Fermat's little theorem
    //
    // for any a < p (p does not divide a) if
    //      a^(p-1) mod p != 1 then p is not prime.
    //
    // Otherwise, p is probably prime (pseudoprime to the chosen base).
    //
    // Returns
    // -------
    // True if "this" is a pseudoprime to randomly chosen
    // bases.  The number of chosen bases is given by the "confidence"
    // parameter.
    //
    // False if "this" is definitely NOT prime.
    //
    // Note - this method is fast but fails for Carmichael numbers except
    // when the randomly chosen base is a factor of the number.
    //
    //***********************************************************************

    /// <summary>
    /// 概率素数测试
    /// </summary>
    /// <param name="confidence"></param>
    /// <returns></returns>
    public bool FermatLittleTest(int confidence)
    {
        BigInteger thisVal;
        if ((data[maxLength - 1] & 0x80000000) != 0) // negative
        {
            thisVal = -this;
        }
        else
        {
            thisVal = this;
        }

        if (thisVal.dataLength == 1)
        {
            // test small numbers
            if (thisVal.data[0] == 0 || thisVal.data[0] == 1)
            {
                return false;
            }

            if (thisVal.data[0] == 2 || thisVal.data[0] == 3)
            {
                return true;
            }
        }

        if ((thisVal.data[0] & 0x1) == 0) // even numbers
        {
            return false;
        }

        var bits = thisVal.BitCount();
        var a = new BigInteger();
        var p_sub1 = thisVal - new BigInteger(1);
        var rand = new System.Random();

        for (var round = 0; round < confidence; round++)
        {
            var done = false;

            while (!done) // generate a < n
            {
                var testBits = 0;

                // make sure "a" has at least 2 bits
                while (testBits < 2)
                {
                    testBits = (int)(rand.NextDouble() * bits);
                }

                a.GenRandomBits(testBits, rand);

                var byteLen = a.dataLength;

                // make sure "a" is not 0
                if (byteLen > 1 || (byteLen == 1 && a.data[0] != 1))
                {
                    done = true;
                }
            }

            // check whether a factor exists (fix for version 1.03)
            var gcdTest = a.Gcd(thisVal);
            if (gcdTest.dataLength == 1 && gcdTest.data[0] != 1)
            {
                return false;
            }

            // calculate a^(p-1) mod p
            var expResult = a.ModPow(p_sub1, thisVal);

            var resultLen = expResult.dataLength;

            // is NOT prime is a^(p-1) mod p != 1

            if (resultLen > 1 || (resultLen == 1 && expResult.data[0] != 1))
            {
                //LogHelper.Info("a = " + a.ToString());
                return false;
            }
        }

        return true;
    }

    //***********************************************************************
    // 基于拉宾-米勒（Rabin-Miller）算法的概率素数测试
    //
    // 对于任何 p > 0，且 p - 1 = 2^s * t
    //
    // 如果对于任何 a < p，满足以下条件，则 p 可能是素数（强伪素数）：
    // 1) a^t mod p = 1 或
    // 2) a^((2^j)*t) mod p = p-1，对于某些 0 <= j <= s-1
    //
    // 否则，p 是合数。
    //
    // 返回值
    // -------
    // 如果 "this" 是随机选择的基数的强伪素数，则返回 true。
    // 选择的基数数量由 "confidence" 参数给出。
    //
    // 如果 "this" 绝对不是素数，则返回 false。
    //
    //***********************************************************************

    /// <summary>
    /// 拉宾-米勒素数测试
    /// </summary>
    /// <param name="confidence">置信度，表示随机选择的基数数量</param>
    /// <returns>如果 "this" 是强伪素数则返回 true，否则返回 false</returns>
    public bool RabinMillerTest(int confidence)
    {
        BigInteger thisVal;
        if ((data[maxLength - 1] & 0x80000000) != 0) // 负数
        {
            thisVal = -this;
        }
        else
        {
            thisVal = this;
        }

        if (thisVal.dataLength == 1)
        {
            // 测试小数字
            if (thisVal.data[0] == 0 || thisVal.data[0] == 1)
            {
                return false;
            }

            if (thisVal.data[0] == 2 || thisVal.data[0] == 3)
            {
                return true;
            }
        }

        if ((thisVal.data[0] & 0x1) == 0) // 偶数
        {
            return false;
        }

        // 计算 s 和 t 的值
        var p_sub1 = thisVal - new BigInteger(1);
        var s = 0;

        for (var index = 0; index < p_sub1.dataLength; index++)
        {
            uint mask = 0x01;

            for (var i = 0; i < 32; i++)
            {
                if ((p_sub1.data[index] & mask) != 0)
                {
                    index = p_sub1.dataLength; // 退出外层循环
                    break;
                }

                mask <<= 1;
                s++;
            }
        }

        var t = p_sub1 >> s;

        var bits = thisVal.BitCount();
        var a = new BigInteger();
        var rand = new System.Random();

        for (var round = 0; round < confidence; round++)
        {
            var done = false;

            while (!done) // 生成 a < n
            {
                var testBits = 0;

                // 确保 "a" 至少有 2 位
                while (testBits < 2)
                {
                    testBits = (int)(rand.NextDouble() * bits);
                }

                a.GenRandomBits(testBits, rand);

                var byteLen = a.dataLength;

                // 确保 "a" 不为 0
                if (byteLen > 1 || (byteLen == 1 && a.data[0] != 1))
                {
                    done = true;
                }
            }

            // 检查是否存在因子（修复版本 1.03）
            var gcdTest = a.Gcd(thisVal);
            if (gcdTest.dataLength == 1 && gcdTest.data[0] != 1)
            {
                return false;
            }

            var b = a.ModPow(t, thisVal);

            /*
        LogHelper.Info("a = " + a.ToString(10));
        LogHelper.Info("b = " + b.ToString(10));
        LogHelper.Info("t = " + t.ToString(10));
        LogHelper.Info("s = " + s);
        */

            var result = false;

            if (b.dataLength == 1 && b.data[0] == 1) // a^t mod p = 1
            {
                result = true;
            }

            for (var j = 0; result == false && j < s; j++)
            {
                if (b == p_sub1) // a^((2^j)*t) mod p = p-1，对于某些 0 <= j <= s-1
                {
                    result = true;
                    break;
                }

                b = b * b % thisVal;
            }

            if (result == false)
            {
                return false;
            }
        }

        return true;
    }


    //***********************************************************************
    // 基于索洛维-斯特拉森（Solovay-Strassen）算法（欧拉准则）的概率素数测试
    //
    // 如果对于任何 a < p（a 不是 p 的倍数），
    // 则 p 可能是素数：a^((p-1)/2) mod p = J(a, p)
    //
    // 其中 J 是雅可比符号。
    //
    // 否则，p 是合数。
    //
    // 返回值
    // -------
    // 如果 "this" 是随机选择的基数的欧拉伪素数，则返回 true。
    // 选择的基数数量由 "confidence" 参数给出。
    //
    // 如果 "this" 绝对不是素数，则返回 false。
    //
    //***********************************************************************

    /// <summary>
    /// 索洛维-斯特拉森素数测试
    /// </summary>
    /// <param name="confidence">置信度，表示随机选择的基数数量</param>
    /// <returns>如果 "this" 是欧拉伪素数则返回 true，否则返回 false</returns>
    public bool SolovayStrassenTest(int confidence)
    {
        BigInteger thisVal;
        if ((data[maxLength - 1] & 0x80000000) != 0) // 负数
        {
            thisVal = -this;
        }
        else
        {
            thisVal = this;
        }

        if (thisVal.dataLength == 1)
        {
            // 测试小数字
            if (thisVal.data[0] == 0 || thisVal.data[0] == 1)
            {
                return false;
            }

            if (thisVal.data[0] == 2 || thisVal.data[0] == 3)
            {
                return true;
            }
        }

        if ((thisVal.data[0] & 0x1) == 0) // 偶数
        {
            return false;
        }

        var bits = thisVal.BitCount();
        var a = new BigInteger();
        var p_sub1 = thisVal - 1;
        var p_sub1_shift = p_sub1 >> 1;

        var rand = new System.Random();

        for (var round = 0; round < confidence; round++)
        {
            var done = false;

            while (!done) // 生成 a < n
            {
                var testBits = 0;

                // 确保 "a" 至少有 2 位
                while (testBits < 2)
                {
                    testBits = (int)(rand.NextDouble() * bits);
                }

                a.GenRandomBits(testBits, rand);

                var byteLen = a.dataLength;

                // 确保 "a" 不为 0
                if (byteLen > 1 || (byteLen == 1 && a.data[0] != 1))
                {
                    done = true;
                }
            }

            // 检查是否存在因子（修复版本 1.03）
            var gcdTest = a.Gcd(thisVal);
            if (gcdTest.dataLength == 1 && gcdTest.data[0] != 1)
            {
                return false;
            }

            // 计算 a^((p-1)/2) mod p

            var expResult = a.ModPow(p_sub1_shift, thisVal);
            if (expResult == p_sub1)
            {
                expResult = -1;
            }

            // 计算雅可比符号
            BigInteger jacob = Jacobi(a, thisVal);

            //LogHelper.Info("a = " + a.ToString(10) + " b = " + thisVal.ToString(10));
            //LogHelper.Info("expResult = " + expResult.ToString(10) + " Jacob = " + jacob.ToString(10));

            // 如果它们不同，则不是素数
            if (expResult != jacob)
            {
                return false;
            }
        }

        return true;
    }


    //***********************************************************************
    // 实现卢卡斯强伪素数测试。
    //
    // 设 n 为一个奇数，且 gcd(n,D) = 1，n - J(D, n) = 2^s * d
    // 其中 d 为奇数，s >= 0。
    //
    // 如果 Ud mod n = 0 或 V2^r*d mod n = 0，对于某些 0 <= r < s，则 n
    // 是一个强卢卡斯伪素数，参数为 (P, Q)。我们根据 Selfridge 选择
    // P 和 Q。
    //
    // 返回值：如果数字是强卢卡斯伪素数，则返回 true。
    // 否则，返回 false，表示该数字是合数。
    //***********************************************************************

    /// <summary>
    /// 实现卢卡斯强伪素数测试
    /// </summary>
    /// <returns>如果是强卢卡斯伪素数则返回 true，否则返回 false</returns>
    public bool LucasStrongTest()
    {
        BigInteger thisVal;
        if ((data[maxLength - 1] & 0x80000000) != 0) // 负数
        {
            thisVal = -this;
        }
        else
        {
            thisVal = this;
        }

        if (thisVal.dataLength == 1)
        {
            // 测试小数字
            if (thisVal.data[0] == 0 || thisVal.data[0] == 1)
            {
                return false;
            }

            if (thisVal.data[0] == 2 || thisVal.data[0] == 3)
            {
                return true;
            }
        }

        if ((thisVal.data[0] & 0x1) == 0) // 偶数
        {
            return false;
        }

        return LucasStrongTestHelper(thisVal);
    }


    private bool LucasStrongTestHelper(BigInteger thisVal)
    {
        // 执行测试（根据 Selfridge 选择 D）
        // 设 D 为序列的第一个元素
        // 5, -7, 9, -11, 13, ... 使得 J(D,n) = -1
        // 设 P = 1, Q = (1-D) / 4

        long D = 5, sign = -1, dCount = 0;
        var done = false;

        while (!done)
        {
            var Jresult = Jacobi(D, thisVal);

            if (Jresult == -1)
            {
                done = true; // J(D, this) = 1
            }
            else
            {
                if (Jresult == 0 && System.Math.Abs(D) < thisVal) // 找到因子
                {
                    return false;
                }

                if (dCount == 20)
                {
                    // 检查是否为平方数
                    var root = thisVal.Sqrt();
                    if (root * root == thisVal)
                    {
                        return false;
                    }
                }

                //LogHelper.Info(D);
                D = (System.Math.Abs(D) + 2) * sign;
                sign = -sign;
            }

            dCount++;
        }

        var Q = (1 - D) >> 2;

        /*
    LogHelper.Info("D = " + D);
    LogHelper.Info("Q = " + Q);
    LogHelper.Info("(n,D) = " + thisVal.gcd(D));
    LogHelper.Info("(n,Q) = " + thisVal.gcd(Q));
    LogHelper.Info("J(D|n) = " + BigInteger.Jacobi(D, thisVal));
    */

        var p_add1 = thisVal + 1;
        var s = 0;

        for (var index = 0; index < p_add1.dataLength; index++)
        {
            uint mask = 0x01;

            for (var i = 0; i < 32; i++)
            {
                if ((p_add1.data[index] & mask) != 0)
                {
                    index = p_add1.dataLength; // 退出外层循环
                    break;
                }

                mask <<= 1;
                s++;
            }
        }

        var t = p_add1 >> s;

        // 计算常数 = b^(2k) / m
        // 用于 Barrett 减少
        var constant = new BigInteger();

        var nLen = thisVal.dataLength << 1;
        constant.data[nLen] = 0x00000001;
        constant.dataLength = nLen + 1;

        constant = constant / thisVal;

        var lucas = LucasSequenceHelper(1, Q, t, thisVal, constant, 0);
        var isPrime = false;

        if ((lucas[0].dataLength == 1 && lucas[0].data[0] == 0) ||
            (lucas[1].dataLength == 1 && lucas[1].data[0] == 0))
        {
            // u(t) = 0 或 V(t) = 0
            isPrime = true;
        }

        for (var i = 1; i < s; i++)
        {
            if (!isPrime)
            {
                // 指数加倍
                lucas[1] = thisVal.BarrettReduction(lucas[1] * lucas[1], thisVal, constant);
                lucas[1] = (lucas[1] - (lucas[2] << 1)) % thisVal;

                //lucas[1] = ((lucas[1] * lucas[1]) - (lucas[2] << 1)) % thisVal;

                if (lucas[1].dataLength == 1 && lucas[1].data[0] == 0)
                {
                    isPrime = true;
                }
            }

            lucas[2] = thisVal.BarrettReduction(lucas[2] * lucas[2], thisVal, constant); //Q^k
        }

        if (isPrime) // 对合数的额外检查
        {
            // 如果 n 是素数且 gcd(n, Q) == 1，则
            // Q^((n+1)/2) = Q * Q^((n-1)/2) 与 (Q * J(Q, n)) mod n 同余

            var g = thisVal.Gcd(Q);
            if (g.dataLength == 1 && g.data[0] == 1) // gcd(this, Q) == 1
            {
                if ((lucas[2].data[maxLength - 1] & 0x80000000) != 0)
                {
                    lucas[2] += thisVal;
                }

                var temp = Q * Jacobi(Q, thisVal) % thisVal;
                if ((temp.data[maxLength - 1] & 0x80000000) != 0)
                {
                    temp += thisVal;
                }

                if (lucas[2] != temp)
                {
                    isPrime = false;
                }
            }
        }

        return isPrime;
    }


    //***********************************************************************
    // 确定一个数字是否可能是素数，使用拉宾-米勒的测试。
    // 在应用测试之前，数字会被测试是否能被小于 2000 的素数整除。
    //
    // 返回 true 如果数字可能是素数。
    //***********************************************************************

    /// <summary>
    /// 确定一个数字是否可能是素数，使用拉宾-米勒的测试
    /// </summary>
    /// <param name="confidence">置信度，表示随机选择的基数数量</param>
    /// <returns>如果数字可能是素数则返回 true，否则返回 false</returns>
    public bool IsProbablePrime(int confidence)
    {
        BigInteger thisVal;
        if ((data[maxLength - 1] & 0x80000000) != 0) // 负数
        {
            thisVal = -this;
        }
        else
        {
            thisVal = this;
        }

        // 测试是否能被小于 2000 的素数整除
        for (var p = 0; p < primesBelow2000.Length; p++)
        {
            BigInteger divisor = primesBelow2000[p];

            if (divisor >= thisVal)
            {
                break;
            }

            var resultNum = thisVal % divisor;
            if (resultNum.IntValue() == 0)
            {
                /*
LogHelper.Info("Not prime!  Divisible by {0}\n",
                              primesBelow2000[p]);
            */
                return false;
            }
        }

        if (thisVal.RabinMillerTest(confidence))
        {
            return true;
        }

        //LogHelper.Info("Not prime!  Failed primality test\n");
        return false;
    }


    //***********************************************************************
    // 确定这个 BigInteger 是否可能是素数，使用
    // 基于 2 的强伪素数测试和卢卡斯强伪素数测试的组合。
    //
    // 素数测试的顺序如下，
    //
    // 1) 使用小于 2000 的素数进行试除。
    //    如果任何素数整除这个 BigInteger，则它不是素数。
    //
    // 2) 执行基于 2 的强伪素数测试。如果这个 BigInteger 是
    //    基于 2 的强伪素数，则继续进行下一步。
    //
    // 3) 执行强卢卡斯伪素数测试。
    //
    // 如果这个 BigInteger 同时是基于 2 的强伪素数
    // 和强卢卡斯伪素数，则返回 true。
    //
    // 有关此素数测试的详细讨论，请参见 [6]。
    //
    //***********************************************************************

    /// <summary>
    /// 确定这个 BigInteger 是否可能是素数
    /// </summary>
    /// <returns>如果是可能的素数则返回 true，否则返回 false</returns>
    public bool IsProbablePrime()
    {
        BigInteger thisVal;
        if ((data[maxLength - 1] & 0x80000000) != 0) // 负数
        {
            thisVal = -this;
        }
        else
        {
            thisVal = this;
        }

        if (thisVal.dataLength == 1)
        {
            // 测试小数字
            if (thisVal.data[0] == 0 || thisVal.data[0] == 1)
            {
                return false;
            }

            if (thisVal.data[0] == 2 || thisVal.data[0] == 3)
            {
                return true;
            }
        }

        if ((thisVal.data[0] & 0x1) == 0) // 偶数
        {
            return false;
        }

        // 测试是否能被小于 2000 的素数整除
        for (var p = 0; p < primesBelow2000.Length; p++)
        {
            BigInteger divisor = primesBelow2000[p];

            if (divisor >= thisVal)
            {
                break;
            }

            var resultNum = thisVal % divisor;
            if (resultNum.IntValue() == 0)
            {
                //LogHelper.Info("Not prime!  Divisible by {0}\n",
                //                  primesBelow2000[p]);

                return false;
            }
        }

        // 执行基于 2 的拉宾-米勒测试

        // 计算 s 和 t 的值
        var p_sub1 = thisVal - new BigInteger(1);
        var s = 0;

        for (var index = 0; index < p_sub1.dataLength; index++)
        {
            uint mask = 0x01;

            for (var i = 0; i < 32; i++)
            {
                if ((p_sub1.data[index] & mask) != 0)
                {
                    index = p_sub1.dataLength; // 退出外层循环
                    break;
                }

                mask <<= 1;
                s++;
            }
        }

        var t = p_sub1 >> s;

        var bits = thisVal.BitCount();
        BigInteger a = 2;

        // b = a^t mod p
        var b = a.ModPow(t, thisVal);
        var result = false;

        if (b.dataLength == 1 && b.data[0] == 1) // a^t mod p = 1
        {
            result = true;
        }

        for (var j = 0; result == false && j < s; j++)
        {
            if (b == p_sub1) // a^((2^j)*t) mod p = p-1，对于某些 0 <= j <= s-1
            {
                result = true;
                break;
            }

            b = b * b % thisVal;
        }

        // 如果数字是基于 2 的强伪素数，则进行强卢卡斯测试
        if (result)
        {
            result = LucasStrongTestHelper(thisVal);
        }

        return result;
    }


    /// <summary>
    /// 返回 BigInteger 的最低 4 个字节作为 int。
    /// </summary>
    /// <returns>最低 4 个字节的整数值</returns>
    public int IntValue()
    {
        return (int)data[0];
    }


    /// <summary>
    /// 返回 BigInteger 的最低 8 个字节作为 long。
    /// </summary>
    /// <returns>最低 8 个字节的长整型值</returns>
    public long LongValue()
    {
        long val = 0;

        val = data[0];
        try
        {
            // 如果 maxLength = 1 则会抛出异常
            val |= (long)data[1] << 32;
        }
        catch (Exception)
        {
            if ((data[0] & 0x80000000) != 0) // 负数
            {
                val = (int)data[0];
            }
        }

        return val;
    }


    /// <summary>
    /// 计算 a 和 b 的雅可比符号。算法改编自 [3] 和 [4]，并进行了优化
    /// </summary>
    /// <param name="a">第一个参数</param>
    /// <param name="b">第二个参数</param>
    /// <returns>雅可比符号的值</returns>
    /// <exception cref="ArgumentException">如果 b 不是奇数则抛出异常</exception>
    public static int Jacobi(BigInteger a, BigInteger b)
    {
        // 雅可比符号仅定义于奇数
        if ((b.data[0] & 0x1) == 0)
        {
            throw new ArgumentException("雅可比符号仅定义于奇数。");
        }

        if (a >= b)
        {
            a %= b;
        }

        if (a.dataLength == 1 && a.data[0] == 0)
        {
            return 0; // a == 0
        }

        if (a.dataLength == 1 && a.data[0] == 1)
        {
            return 1; // a == 1
        }

        if (a < 0)
        {
            if (((b - 1).data[0] & 0x2) == 0) //if( (((b-1) >> 1).data[0] & 0x1) == 0)
            {
                return Jacobi(-a, b);
            }

            return -Jacobi(-a, b);
        }

        var e = 0;
        for (var index = 0; index < a.dataLength; index++)
        {
            uint mask = 0x01;

            for (var i = 0; i < 32; i++)
            {
                if ((a.data[index] & mask) != 0)
                {
                    index = a.dataLength; // 退出外层循环
                    break;
                }

                mask <<= 1;
                e++;
            }
        }

        var a1 = a >> e;

        var s = 1;
        if ((e & 0x1) != 0 && ((b.data[0] & 0x7) == 3 || (b.data[0] & 0x7) == 5))
        {
            s = -1;
        }

        if ((b.data[0] & 0x3) == 3 && (a1.data[0] & 0x3) == 3)
        {
            s = -s;
        }

        if (a1.dataLength == 1 && a1.data[0] == 1)
        {
            return s;
        }

        return s * Jacobi(b % a1, a1);
    }

    /// <summary>
    /// 生成一个可能是素数的正 BigInteger。
    /// </summary>
    /// <param name="bits">位数</param>
    /// <param name="confidence">置信度</param>
    /// <param name="rand">随机数生成器</param>
    /// <returns>生成的可能是素数的 BigInteger</returns>
    public static BigInteger GenPseudoPrime(int bits, int confidence, System.Random rand)
    {
        var result = new BigInteger();
        var done = false;

        while (!done)
        {
            result.GenRandomBits(bits, rand);
            result.data[0] |= 0x01; // 使其为奇数

            // 素数测试
            done = result.IsProbablePrime(confidence);
        }

        return result;
    }


    /// <summary>
    /// 生成一个具有指定位数的随机数，使得 gcd(number, this) = 1
    /// </summary>
    /// <param name="bits">位数</param>
    /// <param name="rand">随机数生成器</param>
    /// <returns>生成的互质的 BigInteger</returns>
    public BigInteger GenCoPrime(int bits, System.Random rand)
    {
        var done = false;
        var result = new BigInteger();

        while (!done)
        {
            result.GenRandomBits(bits, rand);
            //LogHelper.Info(result.ToString(16));

            // gcd 测试
            var g = result.Gcd(this);
            if (g.dataLength == 1 && g.data[0] == 1)
            {
                done = true;
            }
        }

        return result;
    }


    /// <summary>
    /// 返回这个的模逆。如果逆不存在则抛出 ArithmeticException。 
    /// （即 gcd(this, modulus) != 1）
    /// </summary>
    /// <param name="modulus">模数</param>
    /// <returns>模逆</returns>
    /// <exception cref="ArithmeticException">如果逆不存在则抛出异常</exception>
    public BigInteger ModInverse(BigInteger modulus)
    {
        BigInteger[] p = { 0, 1, };
        var q = new BigInteger[2]; // 商
        BigInteger[] r = { 0, 0, }; // 余数

        var step = 0;

        var a = modulus;
        var b = this;

        while (b.dataLength > 1 || (b.dataLength == 1 && b.data[0] != 0))
        {
            var quotient = new BigInteger();
            var remainder = new BigInteger();

            if (step > 1)
            {
                var pval = (p[0] - p[1] * q[0]) % modulus;
                p[0] = p[1];
                p[1] = pval;
            }

            if (b.dataLength == 1)
            {
                SingleByteDivide(a, b, quotient, remainder);
            }
            else
            {
                MultiByteDivide(a, b, quotient, remainder);
            }

            /*
        LogHelper.Info(quotient.dataLength);
        LogHelper.Info("{0} = {1}({2}) + {3}  p = {4}", a.ToString(10),
                          b.ToString(10), quotient.ToString(10), remainder.ToString(10),
                          p[1].ToString(10));
        */

            q[0] = q[1];
            r[0] = r[1];
            q[1] = quotient;
            r[1] = remainder;

            a = b;
            b = remainder;

            step++;
        }

        if (r[0].dataLength > 1 || (r[0].dataLength == 1 && r[0].data[0] != 1))
        {
            throw new ArithmeticException("没有逆！");
        }

        var result = (p[0] - p[1] * q[0]) % modulus;

        if ((result.data[maxLength - 1] & 0x80000000) != 0)
        {
            result += modulus; // 获取最小正模
        }

        return result;
    }


    /// <summary>
    /// 返回 BigInteger 的字节数组。最低索引包含 MSB。
    /// </summary>
    /// <returns>字节数组</returns>
    public byte[] GetBytes()
    {
        var numBits = BitCount();

        var numBytes = numBits >> 3;
        if ((numBits & 0x7) != 0)
        {
            numBytes++;
        }

        var result = new byte[numBytes];

        //LogHelper.Info(result.Length);

        var pos = 0;
        uint tempVal, val = data[dataLength - 1];

        if ((tempVal = (val >> 24) & 0xFF) != 0)
        {
            result[pos++] = (byte)tempVal;
        }

        if ((tempVal = (val >> 16) & 0xFF) != 0)
        {
            result[pos++] = (byte)tempVal;
        }

        if ((tempVal = (val >> 8) & 0xFF) != 0)
        {
            result[pos++] = (byte)tempVal;
        }

        if ((tempVal = val & 0xFF) != 0)
        {
            result[pos++] = (byte)tempVal;
        }

        for (var i = dataLength - 2; i >= 0; i--, pos += 4)
        {
            val = data[i];
            result[pos + 3] = (byte)(val & 0xFF);
            val >>= 8;
            result[pos + 2] = (byte)(val & 0xFF);
            val >>= 8;
            result[pos + 1] = (byte)(val & 0xFF);
            val >>= 8;
            result[pos] = (byte)(val & 0xFF);
        }

        return result;
    }


    /// <summary>
    /// 将指定的位的值设置为 1。最低有效位位置为 0。
    /// </summary>
    /// <param name="bitNum">位编号</param>
    public void SetBit(uint bitNum)
    {
        var bytePos = bitNum >> 5; // 除以 32
        var bitPos = (byte)(bitNum & 0x1F); // 获取最低 5 位

        var mask = (uint)1 << bitPos;
        data[bytePos] |= mask;

        if (bytePos >= dataLength)
        {
            dataLength = (int)bytePos + 1;
        }
    }

    /// <summary>
    /// 将指定的位的值设置为 0。最低有效位位置为 0。
    /// </summary>
    /// <param name="bitNum">位编号</param>
    public void UnsetBit(uint bitNum)
    {
        var bytePos = bitNum >> 5;

        if (bytePos < dataLength)
        {
            var bitPos = (byte)(bitNum & 0x1F);

            var mask = (uint)1 << bitPos;
            var mask2 = 0xFFFFFFFF ^ mask;

            data[bytePos] &= mask2;

            if (dataLength > 1 && data[dataLength - 1] == 0)
            {
                dataLength--;
            }
        }
    }


    //***********************************************************************
    // 返回与 BigInteger 的整数平方根等效的值。
    //
    // BigInteger 的整数平方根定义为最大的整数 n
    // 使得 (n * n) <= this
    //
    //***********************************************************************

    /// <summary>
    /// 返回与整数平方根等效的值
    /// </summary>
    /// <returns>整数平方根的值</returns>
    public BigInteger Sqrt()
    {
        var numBits = (uint)BitCount();

        if ((numBits & 0x1) != 0) // 奇数位数
        {
            numBits = (numBits >> 1) + 1;
        }
        else
        {
            numBits = numBits >> 1;
        }

        var bytePos = numBits >> 5;
        var bitPos = (byte)(numBits & 0x1F);

        uint mask;

        var result = new BigInteger();
        if (bitPos == 0)
        {
            mask = 0x80000000;
        }
        else
        {
            mask = (uint)1 << bitPos;
            bytePos++;
        }

        result.dataLength = (int)bytePos;

        for (var i = (int)bytePos - 1; i >= 0; i--)
        {
            while (mask != 0)
            {
                // 猜测
                result.data[i] ^= mask;

                // 如果其平方大于 this，则撤销猜测
                if (result * result > this)
                {
                    result.data[i] ^= mask;
                }

                mask >>= 1;
            }

            mask = 0x80000000;
        }

        return result;
    }


    //***********************************************************************
    // 返回 Lucas 序列中第 k 个数字，模 n 计算。
    //
    // 使用索引加倍来加速过程。例如，要计算 V(k)，
    // 我们维护序列中的两个数字 V(n) 和 V(n+1)。
    //
    // 要获得 V(2n)，我们使用恒等式
    //      V(2n) = (V(n) * V(n)) - (2 * Q^n)
    // 要获得 V(2n+1)，我们首先将其写为
    //      V(2n+1) = V((n+1) + n)
    // 并使用恒等式
    //      V(m+n) = V(m) * V(n) - Q * V(m-n)
    // 因此，
    //      V((n+1) + n) = V(n+1) * V(n) - Q^n * V((n+1) - n)
    //                   = V(n+1) * V(n) - Q^n * V(1)
    //                   = V(n+1) * V(n) - Q^n * P
    //
    // 我们使用 k 的二进制展开，并对每个
    // 位位置执行索引加倍。对于每个设置的位位置，我们执行
    // 索引加倍后跟索引加法。这意味着对于 V(n)，
    // 我们需要将其更新为 V(2n+1)。对于 V(n+1)，我们需要将其更新为
    // V((2n+1)+1) = V(2*(n+1))
    //
    // 此函数返回
    // [0] = U(k)
    // [1] = V(k)
    // [2] = Q^n
    //
    // 其中 U(0) = 0 % n, U(1) = 1 % n
    //       V(0) = 2 % n, V(1) = P % n
    //***********************************************************************

    /// <summary>
    /// 返回 Lucas 序列中第 k 个数字，模 n 计算
    /// </summary>
    /// <param name="P">参数 P</param>
    /// <param name="Q">参数 Q</param>
    /// <param name="k">索引 k</param>
    /// <param name="n">模数 n</param>
    /// <returns>Lucas 序列的第 k 个数字</returns>
    public static BigInteger[] LucasSequence(BigInteger P, BigInteger Q,
        BigInteger k, BigInteger n)
    {
        if (k.dataLength == 1 && k.data[0] == 0)
        {
            var result = new BigInteger[3];

            result[0] = 0;
            result[1] = 2 % n;
            result[2] = 1 % n;
            return result;
        }

        // 计算常数 = b^(2k) / m
        // 用于 Barrett 减少
        var constant = new BigInteger();

        var nLen = n.dataLength << 1;
        constant.data[nLen] = 0x00000001;
        constant.dataLength = nLen + 1;

        constant = constant / n;

        // 计算 s 和 t 的值
        var s = 0;

        for (var index = 0; index < k.dataLength; index++)
        {
            uint mask = 0x01;

            for (var i = 0; i < 32; i++)
            {
                if ((k.data[index] & mask) != 0)
                {
                    index = k.dataLength; // 退出外层循环
                    break;
                }

                mask <<= 1;
                s++;
            }
        }

        var t = k >> s;

        //LogHelper.Info("s = " + s + " t = " + t);
        return LucasSequenceHelper(P, Q, t, n, constant, s);
    }


    //***********************************************************************
    // 执行 Lucas 序列中第 k 项的计算。
    // 有关算法的详细信息，请参见参考文献 [9]。
    //
    // k 必须是奇数，即 LSB == 1
    //***********************************************************************

    private static BigInteger[] LucasSequenceHelper(BigInteger P, BigInteger Q,
        BigInteger k, BigInteger n,
        BigInteger constant, int s)
    {
        var result = new BigInteger[3];

        if ((k.data[0] & 0x00000001) == 0)
        {
            throw new ArgumentException("参数 k 必须是奇数。");
        }

        var numbits = k.BitCount();
        var mask = (uint)0x1 << ((numbits & 0x1F) - 1);

        // v = v0, v1 = v1, u1 = u1, Q_k = Q^0

        BigInteger v = 2 % n,
                   Q_k = 1 % n,
                   v1 = P % n,
                   u1 = Q_k;
        var flag = true;

        for (var i = k.dataLength - 1; i >= 0; i--) // 遍历 k 的二进制展开
        {
            //LogHelper.Info("round");
            while (mask != 0)
            {
                if (i == 0 && mask == 0x00000001) // 最后一位
                {
                    break;
                }

                if ((k.data[i] & mask) != 0) // 位被设置
                {
                    // 索引加倍并加法

                    u1 = u1 * v1 % n;

                    v = (v * v1 - P * Q_k) % n;
                    v1 = n.BarrettReduction(v1 * v1, n, constant);
                    v1 = (v1 - ((Q_k * Q) << 1)) % n;

                    if (flag)
                    {
                        flag = false;
                    }
                    else
                    {
                        Q_k = n.BarrettReduction(Q_k * Q_k, n, constant);
                    }

                    Q_k = Q_k * Q % n;
                }
                else
                {
                    // 索引加倍
                    u1 = (u1 * v - Q_k) % n;

                    v1 = (v * v1 - P * Q_k) % n;
                    v = n.BarrettReduction(v * v, n, constant);
                    v = (v - (Q_k << 1)) % n;

                    if (flag)
                    {
                        Q_k = Q % n;
                        flag = false;
                    }
                    else
                    {
                        Q_k = n.BarrettReduction(Q_k * Q_k, n, constant);
                    }
                }

                mask >>= 1;
            }

            mask = 0x80000000;
        }

        // 此时 u1 = u(n+1) 和 v = v(n)
        // 由于最后一位始终为 1，我们需要将 u1 转换为 u(2n+1) 和 v 转换为 v(2n+1)

        u1 = (u1 * v - Q_k) % n;
        v = (v * v1 - P * Q_k) % n;
        if (flag)
        {
            flag = false;
        }
        else
        {
            Q_k = n.BarrettReduction(Q_k * Q_k, n, constant);
        }

        Q_k = Q_k * Q % n;

        for (var i = 0; i < s; i++)
        {
            // 索引加倍
            u1 = u1 * v % n;
            v = (v * v - (Q_k << 1)) % n;

            if (flag)
            {
                Q_k = Q % n;
                flag = false;
            }
            else
            {
                Q_k = n.BarrettReduction(Q_k * Q_k, n, constant);
            }
        }

        result[0] = u1;
        result[1] = v;
        result[2] = Q_k;

        return result;
    }
}