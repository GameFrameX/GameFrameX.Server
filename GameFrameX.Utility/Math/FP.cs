#if FP
namespace GameFrameX.Utility.Math
{
    /// <summary>
    /// 表示 Q31.32 固定点数。
    /// </summary>
    [Serializable]
    public partial struct FP : IEquatable<FP>, IComparable<FP>
    {
        /// <summary>
        /// 原始值
        /// </summary>
        public long _serializedValue;

        /// <summary>
        /// 固定点数的最大值。
        /// </summary>
        public const long MAX_VALUE = long.MaxValue;

        /// <summary>
        /// 固定点数的最小值。
        /// </summary>
        public const long MIN_VALUE = long.MinValue;

        /// <summary>
        /// 固定点数的位数。
        /// </summary>
        public const int NUM_BITS = 64;

        /// <summary>
        /// 小数位数。
        /// </summary>
        public const int FRACTIONAL_PLACES = 32;

        /// <summary>
        /// 表示 1 的固定点数值。
        /// </summary>
        public const long ONE = 1L << FRACTIONAL_PLACES;

        /// <summary>
        /// 表示 10 的固定点数值。
        /// </summary>
        public const long TEN = 10L << FRACTIONAL_PLACES;

        /// <summary>
        /// 表示 0.5 的固定点数值。
        /// </summary>
        public const long HALF = 1L << (FRACTIONAL_PLACES - 1);

        /// <summary>
        /// 表示 2π 的固定点数值。
        /// </summary>
        public const long PI_TIMES_2 = 0x6487ED511;

        /// <summary>
        /// 表示 π 的固定点数值。
        /// </summary>
        public const long PI = 0x3243F6A88;

        /// <summary>
        /// 表示 π/2 的固定点数值。
        /// </summary>
        public const long PI_OVER_2 = 0x1921FB544;

        /// <summary>
        /// 表示自然对数的底数 2 的对数的固定点数值。
        /// </summary>
        public const long LN2 = 0xB17217F7;

        /// <summary>
        /// 表示 2 的最大对数的固定点数值。
        /// </summary>
        public const long LOG2MAX = 0x1F00000000;

        /// <summary>
        /// 表示 2 的最小对数的固定点数值。
        /// </summary>
        public const long LOG2MIN = -0x2000000000;

        /// <summary>
        /// 查找表的大小。
        /// </summary>
        public const int LUT_SIZE = (int)(PI_OVER_2 >> 15);

        // 此类型的精度为 2^-32，即 2,3283064365386962890625E-10
        /// <summary>
        /// 此类型的精度。
        /// </summary>
        public static readonly decimal Precision = (decimal)new FP(1L); //0.00000000023283064365386962890625m;

        /// <summary>
        /// 固定点数的最大值实例。
        /// </summary>
        public static readonly FP MaxValue = new(MAX_VALUE - 1);

        /// <summary>
        /// 固定点数的最小值实例。
        /// </summary>
        public static readonly FP MinValue = new(MIN_VALUE + 2);

        /// <summary>
        /// 表示 1 的固定点数实例。
        /// </summary>
        public static readonly FP One = new(ONE);

        /// <summary>
        /// 表示 10 的固定点数实例。
        /// </summary>
        public static readonly FP Ten = new(TEN);

        /// <summary>
        /// 表示 0.5 的固定点数实例。
        /// </summary>
        public static readonly FP Half = new(HALF);

        /// <summary>
        /// 表示 0 的固定点数实例。
        /// </summary>
        public static readonly FP Zero = new();

        /// <summary>
        /// 表示正无穷大的固定点数实例。
        /// </summary>
        public static readonly FP PositiveInfinity = new(MAX_VALUE);

        /// <summary>
        /// 表示负无穷大的固定点数实例。
        /// </summary>
        public static readonly FP NegativeInfinity = new(MIN_VALUE + 1);

        /// <summary>
        /// 表示 NaN 的固定点数实例。
        /// </summary>
        public static readonly FP NaN = new(MIN_VALUE);

        /// <summary>
        /// 表示 0.1 的固定点数实例。
        /// </summary>
        public static readonly FP EN1 = One / 10;

        /// <summary>
        /// 表示 0.01 的固定点数实例。
        /// </summary>
        public static readonly FP EN2 = One / 100;

        /// <summary>
        /// 表示 0.001 的固定点数实例。
        /// </summary>
        public static readonly FP EN3 = One / 1000;

        /// <summary>
        /// 表示 0.0001 的固定点数实例。
        /// </summary>
        public static readonly FP EN4 = One / 10000;

        /// <summary>
        /// 表示 0.00001 的固定点数实例。
        /// </summary>
        public static readonly FP EN5 = One / 100000;

        /// <summary>
        /// 表示 0.000001 的固定点数实例。
        /// </summary>
        public static readonly FP EN6 = One / 1000000;

        /// <summary>
        /// 表示 0.0000001 的固定点数实例。
        /// </summary>
        public static readonly FP EN7 = One / 10000000;

        /// <summary>
        /// 表示 0.00000001 的固定点数实例。
        /// </summary>
        public static readonly FP EN8 = One / 100000000;

        /// <summary>
        /// 表示最小精度的固定点数实例。
        /// </summary>
        public static readonly FP Epsilon = EN3;

        /// <summary>
        /// 表示 π 的固定点数实例。
        /// </summary>
        public static readonly FP Pi = new(PI);

        /// <summary>
        /// 表示 π/2 的固定点数实例。
        /// </summary>
        public static readonly FP PiOver2 = new(PI_OVER_2);

        /// <summary>
        /// 表示 2π 的固定点数实例。
        /// </summary>
        public static readonly FP PiTimes2 = new(PI_TIMES_2);

        /// <summary>
        /// 表示 π 的倒数的固定点数实例。
        /// </summary>
        public static readonly FP PiInv = (FP)0.3183098861837906715377675267M;

        /// <summary>
        /// 表示 π/2 的倒数的固定点数实例。
        /// </summary>
        public static readonly FP PiOver2Inv = (FP)0.6366197723675813430755350535M;

        /// <summary>
        /// 将角度转换为弧度的固定点数实例。
        /// </summary>
        public static readonly FP Deg2Rad = Pi / new FP(180);

        /// <summary>
        /// 将弧度转换为角度的固定点数实例。
        /// </summary>
        public static readonly FP Rad2Deg = new FP(180) / Pi;

        /// <summary>
        /// 查找表的间隔。
        /// </summary>
        public static readonly FP LutInterval = (LUT_SIZE - 1) / PiOver2;

        /// <summary>
        /// 表示 2 的最大对数的固定点数实例。
        /// </summary>
        public static readonly FP Log2Max = new(LOG2MAX);

        /// <summary>
        /// 表示 2 的最小对数的固定点数实例。
        /// </summary>
        public static readonly FP Log2Min = new(LOG2MIN);

        /// <summary>
        /// 表示自然对数的底数 2 的对数的固定点数实例。
        /// </summary>
        public static readonly FP Ln2 = new(LN2);

        /// <summary>
        /// 返回指定幂的 2 的值。
        /// 提供至少 6 位小数的精度。
        /// </summary>
        internal static FP Pow2(FP x)
        {
            if (x.RawValue == 0)
            {
                return One;
            }

            // 通过利用 exp(-x) = 1/exp(x) 来避免负参数。
            var neg = x.RawValue < 0;
            if (neg)
            {
                x = -x;
            }

            if (x == One)
            {
                return neg ? One / 2 : 2;
            }

            if (x >= Log2Max)
            {
                return neg ? One / MaxValue : MaxValue;
            }

            if (x <= Log2Min)
            {
                return neg ? MaxValue : Zero;
            }

            /* 该算法基于 exp(x) 的幂级数：
             * http://en.wikipedia.org/wiki/Exponential_function#Formal_definition
             *
             * 从第 n 项，我们通过与 x/n 相乘得到第 n+1 项。
             * 当和的项降到零时，我们可以停止求和。
             */

            var integerPart = (int)Floor(x);
            // 获取指数的小数部分
            x = FromRaw(x.RawValue & 0x00000000FFFFFFFF);

            var result = One;
            var term = One;
            var i = 1;
            while (term.RawValue != 0)
            {
                term = FastMul(FastMul(x, term), Ln2) / i;
                result += term;
                i++;
            }

            result = FromRaw(result.RawValue << integerPart);
            if (neg)
            {
                result = One / result;
            }

            return result;
        }

        /// <summary>
        /// 返回指定数字的以 2 为底的对数。
        /// 提供至少 9 位小数的精度。
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">
        /// 参数为非正数。
        /// </exception>
        internal static FP Log2(FP x)
        {
            if (x.RawValue <= 0)
            {
                throw new ArgumentOutOfRangeException("传递给 Ln 的值为非正数", "x");
            }

            // 此实现基于 Clay. S. Turner 的快速二进制对数算法
            // （C. S. Turner， "A Fast Binary Logarithm Algorithm"，IEEE Signal
            //     Processing Mag.，第 124,140 页，2010 年 9 月。）

            long b = 1U << (FRACTIONAL_PLACES - 1);
            long y = 0;

            var rawX = x.RawValue;
            while (rawX < ONE)
            {
                rawX <<= 1;
                y -= ONE;
            }

            while (rawX >= ONE << 1)
            {
                rawX >>= 1;
                y += ONE;
            }

            var z = FromRaw(rawX);

            for (var i = 0; i < FRACTIONAL_PLACES; i++)
            {
                z = FastMul(z, z);
                if (z.RawValue >= ONE << 1)
                {
                    z = FromRaw(z.RawValue >> 1);
                    y += b;
                }

                b >>= 1;
            }

            return FromRaw(y);
        }

        /// <summary>
        /// 返回指定数字的自然对数。
        /// 提供至少 7 位小数的精度。
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">
        /// 参数为非正数。
        /// </exception>
        public static FP Ln(FP x)
        {
            return FastMul(Log2(x), Ln2);
        }

        /// <summary>
        /// 返回 Fix64 数字的符号。
        /// 如果值为正，则返回 1；如果为 0，则返回 0；如果为负，则返回 -1。
        /// </summary>
        public static int Sign(FP value)
        {
            return
                value._serializedValue < 0 ? -1 :
                value._serializedValue > 0 ? 1 :
                0;
        }

        /// <summary>
        /// 返回 Fix64 数字的绝对值。
        /// 注意：Abs(Fix64.MinValue) == Fix64.MaxValue。
        /// </summary>
        public static FP Abs(FP value)
        {
            if (value._serializedValue == MIN_VALUE)
            {
                return MaxValue;
            }

            // 无分支实现，参见 http://www.strchr.com/optimized_abs_function
            var mask = value._serializedValue >> 63;
            FP result;
            result._serializedValue = (value._serializedValue + mask) ^ mask;
            return result;
        }

        /// <summary>
        /// 返回 Fix64 数字的绝对值。
        /// FastAbs(Fix64.MinValue) 是未定义的。
        /// </summary>
        public static FP FastAbs(FP value)
        {
            // 无分支实现，参见 http://www.strchr.com/optimized_abs_function
            var mask = value._serializedValue >> 63;
            FP result;
            result._serializedValue = (value._serializedValue + mask) ^ mask;
            return result;
        }

        /// <summary>
        /// 返回小于或等于指定数字的最大整数。
        /// </summary>
        public static FP Floor(FP value)
        {
            // 仅清零小数部分
            FP result;
            result._serializedValue = (long)((ulong)value._serializedValue & 0xFFFFFFFF00000000);
            return result;
        }

        /// <summary>
        /// 返回大于或等于指定数字的最小整数。
        /// </summary>
        public static FP Ceiling(FP value)
        {
            var hasFractionalPart = (value._serializedValue & 0x00000000FFFFFFFF) != 0;
            if (!hasFractionalPart)
                return value;
            
            // 对于正数，Floor + 1；对于负数，也是Floor + 1（因为Floor对负数向下取整）
            return Floor(value) + One;
        }

        /// <summary>
        /// 将值四舍五入到最接近的整数值。
        /// 如果值恰好在偶数和奇数之间，则返回偶数值。
        /// </summary>
        public static FP Round(FP value)
        {
            var fractionalPart = value._serializedValue & 0x00000000FFFFFFFF;
            var integralPart = Floor(value);
            if (fractionalPart < 0x80000000)
            {
                return integralPart;
            }

            if (fractionalPart > 0x80000000)
            {
                return integralPart + One;
            }

            // 如果数字恰好在两个值之间，则四舍五入到最接近的偶数
            // 这是 System.Math.Round() 使用的方法。
            return (integralPart._serializedValue & ONE) == 0
                       ? integralPart
                       : integralPart + One;
        }

        /// <summary>
        /// 返回两个 FP 数字中较大的一个。
        /// </summary>
        public static FP Max(FP left, FP right)
        {
            return left._serializedValue > right._serializedValue ? left : right;
        }

        /// <summary>
        /// 返回两个 FP 数字中较小的一个。
        /// </summary>
        public static FP Min(FP left, FP right)
        {
            return left._serializedValue > right._serializedValue ? right : left;
        }

        /// <summary>
        /// 将指定的值限制在给定的最小值和最大值之间。
        /// </summary>
        public static FP Clamp(FP origin, FP min, FP max)
        {
            return Min(Max(origin, min), max);
        }

        /// <summary>
        /// 将 x 和 y 相加。执行饱和加法，即在溢出情况下，
        /// 根据操作数的符号四舍五入到 MinValue 或 MaxValue。
        /// </summary>
        public static FP operator +(FP x, FP y)
        {
            FP result;
            result._serializedValue = x._serializedValue + y._serializedValue;
            return result;
        }

        /// <summary>
        /// 执行 x 和 y 的加法并进行溢出检查。应由 CLR 内联。
        /// </summary>
        public static FP OverflowAdd(FP x, FP y)
        {
            var xl = x._serializedValue;
            var yl = y._serializedValue;
            var sum = xl + yl;
            // 如果操作数的符号相同且和与 x 的符号不同
            if ((~(xl ^ yl) & (xl ^ sum) & MIN_VALUE) != 0)
            {
                sum = xl > 0 ? MAX_VALUE : MIN_VALUE;
            }

            FP result;
            result._serializedValue = sum;
            return result;
        }

        /// <summary>
        /// 执行 x 和 y 的加法而不进行溢出检查。应由 CLR 内联。
        /// </summary>
        public static FP FastAdd(FP x, FP y)
        {
            FP result;
            result._serializedValue = x._serializedValue + y._serializedValue;
            return result;
        }

        /// <summary>
        /// 从 x 中减去 y。执行饱和减法，即在溢出情况下，
        /// 根据操作数的符号四舍五入到 MinValue 或 MaxValue。
        /// </summary>
        public static FP operator -(FP x, FP y)
        {
            FP result;
            result._serializedValue = x._serializedValue - y._serializedValue;
            return result;
        }

        /// <summary>
        /// 从 x 中减去 y 而不进行溢出检查。应由 CLR 内联。
        /// </summary>
        public static FP OverflowSub(FP x, FP y)
        {
            var xl = x._serializedValue;
            var yl = y._serializedValue;
            var diff = xl - yl;
            // 如果操作数的符号不同且和与 x 的符号不同
            if (((xl ^ yl) & (xl ^ diff) & MIN_VALUE) != 0)
            {
                diff = xl < 0 ? MIN_VALUE : MAX_VALUE;
            }

            FP result;
            result._serializedValue = diff;
            return result;
        }

        /// <summary>
        /// 从 x 中减去 y 而不进行溢出检查。应由 CLR 内联。
        /// </summary>
        public static FP FastSub(FP x, FP y)
        {
            return new FP(x._serializedValue - y._serializedValue);
        }

        private static long AddOverflowHelper(long x, long y, ref bool overflow)
        {
            var sum = x + y;
            // x + y 溢出时，如果符号(x) ^ 符号(y) != 符号(sum)
            overflow |= ((x ^ y ^ sum) & MIN_VALUE) != 0;
            return sum;
        }

        /// <summary>
        /// 执行 x 和 y 的乘法。
        /// </summary>
        public static FP operator *(FP x, FP y)
        {
            var xl = x._serializedValue;
            var yl = y._serializedValue;

            var xlo = (ulong)(xl & 0x00000000FFFFFFFF);
            var xhi = xl >> FRACTIONAL_PLACES;
            var ylo = (ulong)(yl & 0x00000000FFFFFFFF);
            var yhi = yl >> FRACTIONAL_PLACES;

            var lolo = xlo * ylo;
            var lohi = (long)xlo * yhi;
            var hilo = xhi * (long)ylo;
            var hihi = xhi * yhi;

            var loResult = lolo >> FRACTIONAL_PLACES;
            var midResult1 = lohi;
            var midResult2 = hilo;
            var hiResult = hihi << FRACTIONAL_PLACES;

            var sum = (long)loResult + midResult1 + midResult2 + hiResult;
            FP result;
            result._serializedValue = sum;
            return result;
        }

        /// <summary>
        /// 执行乘法而不进行溢出检查。
        /// 对于保证不会导致溢出的性能关键代码非常有用。
        /// </summary>
        public static FP OverflowMul(FP x, FP y)
        {
            var xl = x._serializedValue;
            var yl = y._serializedValue;

            var xlo = (ulong)(xl & 0x00000000FFFFFFFF);
            var xhi = xl >> FRACTIONAL_PLACES;
            var ylo = (ulong)(yl & 0x00000000FFFFFFFF);
            var yhi = yl >> FRACTIONAL_PLACES;

            var lolo = xlo * ylo;
            var lohi = (long)xlo * yhi;
            var hilo = xhi * (long)ylo;
            var hihi = xhi * yhi;

            var loResult = lolo >> FRACTIONAL_PLACES;
            var midResult1 = lohi;
            var midResult2 = hilo;
            var hiResult = hihi << FRACTIONAL_PLACES;

            var overflow = false;
            var sum = AddOverflowHelper((long)loResult, midResult1, ref overflow);
            sum = AddOverflowHelper(sum, midResult2, ref overflow);
            sum = AddOverflowHelper(sum, hiResult, ref overflow);

            var opSignsEqual = ((xl ^ yl) & MIN_VALUE) == 0;

            // 如果操作数的符号相同且结果的符号为负，
            // 则乘法正溢出
            // 反之亦然
            if (opSignsEqual)
            {
                if (sum < 0 || (overflow && xl > 0))
                {
                    return MaxValue;
                }
            }
            else
            {
                if (sum > 0)
                {
                    return MinValue;
                }
            }

            // 如果 hihi 的高 32 位（在结果中未使用）既不是全 0 也不是全 1，
            // 则这意味着结果溢出。
            var topCarry = hihi >> FRACTIONAL_PLACES;
            if (topCarry != 0 && topCarry != -1)
            {
                return opSignsEqual ? MaxValue : MinValue;
            }

            // 如果符号不同，且两个操作数的绝对值都大于 1，
            // 且结果大于负操作数，则发生负溢出。
            if (!opSignsEqual)
            {
                long posOp, negOp;
                if (xl > yl)
                {
                    posOp = xl;
                    negOp = yl;
                }
                else
                {
                    posOp = yl;
                    negOp = xl;
                }

                if (sum > negOp && negOp < -ONE && posOp > ONE)
                {
                    return MinValue;
                }
            }

            FP result;
            result._serializedValue = sum;
            return result;
        }

        /// <summary>
        /// 执行乘法而不进行溢出检查。
        /// 对于保证不会导致溢出的性能关键代码非常有用。
        /// </summary>
        public static FP FastMul(FP x, FP y)
        {
            var xl = x._serializedValue;
            var yl = y._serializedValue;

            var xlo = (ulong)(xl & 0x00000000FFFFFFFF);
            var xhi = xl >> FRACTIONAL_PLACES;
            var ylo = (ulong)(yl & 0x00000000FFFFFFFF);
            var yhi = yl >> FRACTIONAL_PLACES;

            var lolo = xlo * ylo;
            var lohi = (long)xlo * yhi;
            var hilo = xhi * (long)ylo;
            var hihi = xhi * yhi;

            var loResult = lolo >> FRACTIONAL_PLACES;
            var midResult1 = lohi;
            var midResult2 = hilo;
            var hiResult = hihi << FRACTIONAL_PLACES;

            var sum = (long)loResult + midResult1 + midResult2 + hiResult;
            FP result;
            result._serializedValue = sum;
            return result;
        }

        /// <summary>
        /// 计算给定无符号长整型数的前导零位数。
        /// </summary>
        public static int CountLeadingZeroes(ulong x)
        {
            var result = 0;
            while ((x & 0xF000000000000000) == 0)
            {
                result += 4;
                x <<= 4;
            }

            while ((x & 0x8000000000000000) == 0)
            {
                result += 1;
                x <<= 1;
            }

            return result;
        }

        /// <summary>
        /// 执行 x 和 y 的除法。
        /// </summary>
        public static FP operator /(FP x, FP y)
        {
            var xl = x._serializedValue;
            var yl = y._serializedValue;

            if (yl == 0)
            {
                return MAX_VALUE;
            }

            var remainder = (ulong)(xl >= 0 ? xl : -xl);
            var divider = (ulong)(yl >= 0 ? yl : -yl);
            var quotient = 0UL;
            var bitPos = NUM_BITS / 2 + 1;

            // 如果除数可以被 2^n 整除，则利用这一点。
            while ((divider & 0xF) == 0 && bitPos >= 4)
            {
                divider >>= 4;
                bitPos -= 4;
            }

            while (remainder != 0 && bitPos >= 0)
            {
                var shift = CountLeadingZeroes(remainder);
                if (shift > bitPos)
                {
                    shift = bitPos;
                }

                remainder <<= shift;
                bitPos -= shift;

                var div = remainder / divider;
                remainder = remainder % divider;
                quotient += div << bitPos;

                // 检测溢出
                if ((div & ~(0xFFFFFFFFFFFFFFFF >> bitPos)) != 0)
                {
                    return ((xl ^ yl) & MIN_VALUE) == 0 ? MaxValue : MinValue;
                }

                remainder <<= 1;
                --bitPos;
            }

            // 四舍五入
            ++quotient;
            var result = (long)(quotient >> 1);
            if (((xl ^ yl) & MIN_VALUE) != 0)
            {
                result = -result;
            }

            FP r;
            r._serializedValue = result;
            return r;
        }

        /// <summary>
        /// 执行 x 和 y 的取模运算。
        /// </summary>
        public static FP operator %(FP x, FP y)
        {
            FP result;
            result._serializedValue = (x._serializedValue == MIN_VALUE) & (y._serializedValue == -1) ? 0 : x._serializedValue % y._serializedValue;
            return result;
        }

        /// <summary>
        /// 尽可能快速地执行取模运算；如果 x == MinValue 且 y == -1，则抛出异常。
        /// 使用运算符 (%) 进行更可靠但更慢的取模运算。
        /// </summary>
        public static FP FastMod(FP x, FP y)
        {
            FP result;
            result._serializedValue = x._serializedValue % y._serializedValue;
            return result;
        }

        /// <summary>
        /// 返回 x 的相反数。
        /// </summary>
        public static FP operator -(FP x)
        {
            return x._serializedValue == MIN_VALUE ? MaxValue : new FP(-x._serializedValue);
        }

        /// <summary>
        /// 判断两个 FP 数字是否相等。
        /// </summary>
        public static bool operator ==(FP x, FP y)
        {
            return x._serializedValue == y._serializedValue;
        }

        /// <summary>
        /// 判断两个 FP 数字是否不相等。
        /// </summary>
        public static bool operator !=(FP x, FP y)
        {
            return x._serializedValue != y._serializedValue;
        }

        /// <summary>
        /// 判断 x 是否大于 y。
        /// </summary>
        public static bool operator >(FP x, FP y)
        {
            return x._serializedValue > y._serializedValue;
        }

        /// <summary>
        /// 判断 x 是否小于 y。
        /// </summary>
        public static bool operator <(FP x, FP y)
        {
            return x._serializedValue < y._serializedValue;
        }

        /// <summary>
        /// 判断 x 是否大于或等于 y。
        /// </summary>
        public static bool operator >=(FP x, FP y)
        {
            return x._serializedValue >= y._serializedValue;
        }

        /// <summary>
        /// 判断 x 是否小于或等于 y。
        /// </summary>
        public static bool operator <=(FP x, FP y)
        {
            return x._serializedValue <= y._serializedValue;
        }

        /// <summary>
        /// 返回指定数字的平方根。
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">
        /// 参数为负数。
        /// </exception>
        public static FP Sqrt(FP x)
        {
            var xl = x._serializedValue;
            if (xl < 0)
            {
                // 我们无法像 Single 和 Double 一样表示无穷大，且 Sqrt 在 x < 0 时
                // 数学上是未定义的。因此我们抛出异常。
                throw new ArgumentOutOfRangeException("传递给 Sqrt 的值为负数", "x");
            }

            var num = (ulong)xl;
            var result = 0UL;

            // 次高位
            var bit = 1UL << (NUM_BITS - 2);

            while (bit > num)
            {
                bit >>= 2;
            }

            // 主要部分执行两次，以避免在计算中使用 128 位值。
            for (var i = 0; i < 2; ++i)
            {
                // 首先获取答案的高 48 位。
                while (bit != 0)
                {
                    if (num >= result + bit)
                    {
                        num -= result + bit;
                        result = (result >> 1) + bit;
                    }
                    else
                    {
                        result = result >> 1;
                    }

                    bit >>= 2;
                }

                if (i == 0)
                {
                    // 然后处理它以获取最低的 16 位。
                    if (num > (1UL << (NUM_BITS / 2)) - 1)
                    {
                        // 余数 'num' 太大，无法左移
                        // 32，因此我们必须手动将 1 添加到结果并
                        // 相应地调整 'num'。
                        // num = a - (result + 0.5)^2
                        //       = num + result^2 - (result + 0.5)^2
                        //       = num - result - 0.5
                        num -= result;
                        num = (num << (NUM_BITS / 2)) - 0x80000000UL;
                        result = (result << (NUM_BITS / 2)) + 0x80000000UL;
                    }
                    else
                    {
                        num <<= NUM_BITS / 2;
                        result <<= NUM_BITS / 2;
                    }

                    bit = 1UL << (NUM_BITS / 2 - 2);
                }
            }

            // 最后，如果下一个位应该为 1，则向上四舍五入结果。
            if (num > result)
            {
                ++result;
            }

            FP r;
            r._serializedValue = (long)result;
            return r;
        }

        /// <summary>
        /// 返回 x 的正弦值。
        /// 此函数对小值 x 的精度约为 9 位小数。
        /// 随着 x 值的增大，可能会失去精度。
        /// 性能：在 x64 中比 Math.Sin() 慢约 25%，在 x86 中慢约 200%。
        /// </summary>
        public static FP Sin(FP x)
        {
            bool flipHorizontal, flipVertical;
            var clampedL = ClampSinValue(x._serializedValue, out flipHorizontal, out flipVertical);
            var clamped = new FP(clampedL);

            // 查找 LUT 中的两个最接近的值并执行线性插值
            // 这就是该函数在 x86 上性能下降的原因 - x64 是可以的
            var rawIndex = FastMul(clamped, LutInterval);
            var roundedIndex = Round(rawIndex);
            var indexError = 0;

            var nearestValue = new FP(SinLut[flipHorizontal ? SinLut.Length - 1 - (int)roundedIndex : (int)roundedIndex]);
            var secondNearestValue = new FP(SinLut[flipHorizontal ? SinLut.Length - 1 - (int)roundedIndex - Sign(indexError) : (int)roundedIndex + Sign(indexError)]);

            var delta = FastMul(indexError, FastAbs(FastSub(nearestValue, secondNearestValue)))._serializedValue;
            var interpolatedValue = nearestValue._serializedValue + (flipHorizontal ? -delta : delta);
            var finalValue = flipVertical ? -interpolatedValue : interpolatedValue;

            FP a2;
            a2._serializedValue = finalValue;
            return a2;
        }

        /// <summary>
        /// 返回 x 的粗略近似正弦值。
        /// 这在 x86 上至少比 Sin() 快 3 倍，并且比 Math.Sin() 稍快，
        /// 但是其精度限制在 4-5 位小数，对于足够小的 x 值。
        /// </summary>
        public static FP FastSin(FP x)
        {
            bool flipHorizontal, flipVertical;
            var clampedL = ClampSinValue(x._serializedValue, out flipHorizontal, out flipVertical);

            // 这里我们利用 SinLut 表的条目数量
            // 等于 (PI_OVER_2 >> 15) 直接使用角度索引
            var rawIndex = (uint)(clampedL >> 15);
            if (rawIndex >= LUT_SIZE)
            {
                rawIndex = LUT_SIZE - 1;
            }

            var nearestValue = SinLut[flipHorizontal ? SinLut.Length - 1 - (int)rawIndex : (int)rawIndex];

            FP result;
            result._serializedValue = flipVertical ? -nearestValue : nearestValue;
            return result;
        }

        /// <summary>
        /// 限制角度值在 0 到 2π 之间；使用模运算，速度较慢，但没有更好的方法。
        /// </summary>
        public static long ClampSinValue(long angle, out bool flipHorizontal, out bool flipVertical)
        {
            // 将值限制在 0 - 2*PI 之间；这非常慢，但没有更好的方法
            var clamped2Pi = angle % PI_TIMES_2;
            if (angle < 0)
            {
                clamped2Pi += PI_TIMES_2;
            }

            // LUT 包含 0 - PiOver2 的值；每个其他值必须通过
            // 垂直或水平镜像获得
            flipVertical = clamped2Pi >= PI;
            // 从 (angle % 2PI) 获取 (angle % PI) - 比做另一个模运算快得多
            var clampedPi = clamped2Pi;
            while (clampedPi >= PI)
            {
                clampedPi -= PI;
            }

            flipHorizontal = clampedPi >= PI_OVER_2;
            // 从 (angle % PI) 获取 (angle % PI_OVER_2) - 比做另一个模运算快得多
            var clampedPiOver2 = clampedPi;
            if (clampedPiOver2 >= PI_OVER_2)
            {
                clampedPiOver2 -= PI_OVER_2;
            }

            return clampedPiOver2;
        }

        /// <summary>
        /// 返回 x 的余弦值。
        /// 参见 Sin() 以获取更多详细信息。
        /// </summary>
        public static FP Cos(FP x)
        {
            var xl = x._serializedValue;
            var rawAngle = xl + (xl > 0 ? -PI - PI_OVER_2 : PI_OVER_2);
            var a2 = Sin(new FP(rawAngle));
            return a2;
        }

        /// <summary>
        /// 返回 x 的粗略近似余弦值。
        /// 参见 FastSin 以获取更多详细信息。
        /// </summary>
        public static FP FastCos(FP x)
        {
            var xl = x._serializedValue;
            var rawAngle = xl + (xl > 0 ? -PI - PI_OVER_2 : PI_OVER_2);
            return FastSin(new FP(rawAngle));
        }

        /// <summary>
        /// 返回 x 的正切值。
        /// </summary>
        /// <remarks>
        /// 此函数未经过充分测试，可能会非常不准确。
        /// </remarks>
        public static FP Tan(FP x)
        {
            var clampedPi = x._serializedValue % PI;
            var flip = false;
            if (clampedPi < 0)
            {
                clampedPi = -clampedPi;
                flip = true;
            }

            if (clampedPi > PI_OVER_2)
            {
                flip = !flip;
                clampedPi = PI_OVER_2 - (clampedPi - PI_OVER_2);
            }

            var clamped = new FP(clampedPi);

            // 查找 LUT 中的两个最接近的值并执行线性插值
            var rawIndex = FastMul(clamped, LutInterval);
            var roundedIndex = Round(rawIndex);
            var indexError = FastSub(rawIndex, roundedIndex);

            var nearestValue = new FP(TanLut[(int)roundedIndex]);
            var secondNearestValue = new FP(TanLut[(int)roundedIndex + Sign(indexError)]);

            var delta = FastMul(indexError, FastAbs(FastSub(nearestValue, secondNearestValue)))._serializedValue;
            var interpolatedValue = nearestValue._serializedValue + delta;
            var finalValue = flip ? -interpolatedValue : interpolatedValue;
            var a2 = new FP(finalValue);
            return a2;
        }

        /// <summary>
        /// 返回指定数字的反正切值，使用欧拉级数计算。
        /// 此函数至少具有 7 位小数的精度。
        /// </summary>
        public static FP Atan(FP z)
        {
            if (z.RawValue == 0)
            {
                return Zero;
            }

            // 强制参数为正值
            // Atan(-z) = -Atan(z)。
            var neg = z.RawValue < 0;
            if (neg)
            {
                z = -z;
            }

            FP result;
            var two = (FP)2;
            var three = (FP)3;

            var invert = z > One;
            if (invert)
            {
                z = One / z;
            }

            result = One;
            var term = One;

            var zSq = z * z;
            var zSq2 = zSq * two;
            var zSqPlusOne = zSq + One;
            var zSq12 = zSqPlusOne * two;
            var dividend = zSq2;
            var divisor = zSqPlusOne * three;

            for (var i = 2; i < 30; ++i)
            {
                term *= dividend / divisor;
                result += term;

                dividend += zSq2;
                divisor += zSq12;

                if (term.RawValue == 0)
                {
                    break;
                }
            }

            result = result * z / zSqPlusOne;

            if (invert)
            {
                result = PiOver2 - result;
            }

            if (neg)
            {
                result = -result;
            }

            return result;
        }

        /// <summary>
        /// 返回 y 和 x 的反正切值。
        /// </summary>
        public static FP Atan2(FP y, FP x)
        {
            var yl = y._serializedValue;
            var xl = x._serializedValue;
            if (xl == 0)
            {
                if (yl > 0)
                {
                    return PiOver2;
                }

                if (yl == 0)
                {
                    return Zero;
                }

                return -PiOver2;
            }

            FP atan;
            var z = y / x;

            var sm = EN2 * 28;
            // 处理溢出
            if (One + sm * z * z == MaxValue)
            {
                return y < Zero ? -PiOver2 : PiOver2;
            }

            if (Abs(z) < One)
            {
                atan = z / (One + sm * z * z);
                if (xl < 0)
                {
                    if (yl < 0)
                    {
                        return atan - Pi;
                    }

                    return atan + Pi;
                }
            }
            else
            {
                atan = PiOver2 - z / (z * z + sm);
                if (yl < 0)
                {
                    return atan - Pi;
                }
            }

            return atan;
        }

        /// <summary>
        /// 返回指定数字的反正弦值。
        /// </summary>
        public static FP Asin(FP value)
        {
            return FastSub(PiOver2, Acos(value));
        }

        /// <summary>
        /// 返回指定数字的反余弦值，使用 Atan 和 Sqrt 计算。
        /// 此函数至少具有 7 位小数的精度。
        /// </summary>
        public static FP Acos(FP x)
        {
            if (x < -One || x > One)
            {
                throw new ArgumentOutOfRangeException("必须在 -FP.One 和 FP.One 之间", "x");
            }

            if (x.RawValue == 0)
            {
                return PiOver2;
            }

            var result = Atan(Sqrt(One - x * x) / x);
            return x.RawValue < 0 ? result + Pi : result;
        }

        /// <summary>
        /// 将长整型值转换为 FP。
        /// </summary>
        public static implicit operator FP(long value)
        {
            FP result;
            result._serializedValue = value * ONE;
            return result;
        }

        /// <summary>
        /// 将 FP 转换为长整型值。
        /// </summary>
        public static explicit operator long(FP value)
        {
            return value._serializedValue >> FRACTIONAL_PLACES;
        }

        /// <summary>
        /// 将 FP 转换为浮点型值。
        /// </summary>
        public static explicit operator float(FP value)
        {
            return (float)value._serializedValue / ONE;
        }

        /// <summary>
        /// 将十进制值转换为 FP。
        /// </summary>
        public static explicit operator FP(decimal value)
        {
            FP result;
            result._serializedValue = (long)(value * ONE);
            return result;
        }

        /// <summary>
        /// 将整型值转换为 FP。
        /// </summary>
        public static implicit operator FP(int value)
        {
            FP result;
            result._serializedValue = value * ONE;
            return result;
        }

        /// <summary>
        /// 将 FP 转换为十进制值。
        /// </summary>
        public static explicit operator decimal(FP value)
        {
            return (decimal)value._serializedValue / ONE;
        }

        /// <summary>
        /// 将 FP 转换为浮点型。
        /// </summary>
        public float AsFloat()
        {
            return (float)this;
        }

        /// <summary>
        /// 将 FP 转换为整型。
        /// </summary>
        public int AsInt()
        {
            return (int)this;
        }

        /// <summary>
        /// 将 FP 转换为无符号整型。
        /// </summary>
        public uint AsUInt()
        {
            return (uint)this;
        }

        /// <summary>
        /// 将 FP 转换为长整型。
        /// </summary>
        public long AsLong()
        {
            return (long)this;
        }

        /// <summary>
        /// 将 FP 转换为双精度浮点型。
        /// </summary>
        public double AsDouble()
        {
            return (double)this;
        }

        /// <summary>
        /// 将 FP 转换为十进制型。
        /// </summary>
        public decimal AsDecimal()
        {
            return (decimal)this;
        }

        /// <summary>
        /// 将 FP 转换为浮点型。
        /// </summary>
        public static float ToFloat(FP value)
        {
            return (float)value;
        }

        /// <summary>
        /// 将 FP 转换为整型。
        /// </summary>
        public static int ToInt(FP value)
        {
            return (int)value;
        }

        /// <summary>
        /// 将 FP 转换为无符号整型。
        /// </summary>
        public static uint ToUInt(FP value)
        {
            return (uint)value;
        }

        /// <summary>
        /// 判断 FP 值是否为无穷大。
        /// </summary>
        public static bool IsInfinity(FP value)
        {
            return value == NegativeInfinity || value == PositiveInfinity;
        }

        /// <summary>
        /// 判断 FP 值是否为 NaN。
        /// </summary>
        public static bool IsNaN(FP value)
        {
            return value == NaN;
        }

        /// <summary>
        /// 判断两个 FP 对象是否相等。
        /// </summary>
        public override bool Equals(object obj)
        {
            return obj is FP && ((FP)obj)._serializedValue == _serializedValue;
        }

        /// <summary>
        /// 返回 FP 对象的哈希代码。
        /// </summary>
        public override int GetHashCode()
        {
            return _serializedValue.GetHashCode();
        }

        /// <summary>
        /// 判断两个 FP 对象是否相等。
        /// </summary>
        public bool Equals(FP other)
        {
            return _serializedValue == other._serializedValue;
        }

        /// <summary>
        /// 比较两个 FP 对象的大小。
        /// </summary>
        public int CompareTo(FP other)
        {
            return _serializedValue.CompareTo(other._serializedValue);
        }

        /// <summary>
        /// 返回 FP 对象的字符串表示形式。
        /// </summary>
        public override string ToString()
        {
            return ((float)this).ToString();
        }

        /// <summary>
        /// 返回 FP 对象的字符串表示形式，使用指定的格式提供程序。
        /// </summary>
        public string ToString(IFormatProvider provider)
        {
            return ((float)this).ToString(provider);
        }

        /// <summary>
        /// 返回 FP 对象的字符串表示形式，使用指定的格式。
        /// </summary>
        public string ToString(string format)
        {
            return ((float)this).ToString(format);
        }

        /// <summary>
        /// 从原始值创建 FP 对象。
        /// </summary>
        public static FP FromRaw(long rawValue)
        {
            return new FP(rawValue);
        }

        /// <summary>
        /// 获取底层整数表示。
        /// </summary>
        public long RawValue
        {
            get { return _serializedValue; }
        }

        /// <summary>
        /// 这是从原始值构造的构造函数；只能在内部使用。
        /// </summary>
        /// <param name="rawValue"></param>
        private FP(long rawValue)
        {
            _serializedValue = rawValue;
        }

        /// <summary>
        /// 从整型值创建 FP 对象。
        /// </summary>
        public FP(int value)
        {
            _serializedValue = value * ONE;
        }
    }
}
#endif