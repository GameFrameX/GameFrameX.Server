// ==========================================================================================
//  GameFrameX 组织及其衍生项目的版权、商标、专利及其他相关权利
//  GameFrameX organization and its derivative projects' copyrights, trademarks, patents, and related rights
//  均受中华人民共和国及相关国际法律法规保护。
//  are protected by the laws of the People's Republic of China and relevant international regulations.
//  
//  使用本项目须严格遵守相应法律法规及开源许可证之规定。
//  Usage of this project must strictly comply with applicable laws, regulations, and open-source licenses.
//  
//  本项目采用 MIT 许可证与 Apache License 2.0 双许可证分发，
//  This project is dual-licensed under the MIT License and Apache License 2.0,
//  完整许可证文本请参见源代码根目录下的 LICENSE 文件。
//  please refer to the LICENSE file in the root directory of the source code for the full license text.
//  
//  禁止利用本项目实施任何危害国家安全、破坏社会秩序、
//  It is prohibited to use this project to engage in any activities that endanger national security, disrupt social order,
//  侵犯他人合法权益等法律法规所禁止的行为！
//  or infringe upon the legitimate rights and interests of others, as prohibited by laws and regulations!
//  因基于本项目二次开发所产生的一切法律纠纷与责任，
//  Any legal disputes and liabilities arising from secondary development based on this project
//  本项目组织与贡献者概不承担。
//  shall be borne solely by the developer; the project organization and contributors assume no responsibility.
//  
//  GitHub 仓库：https://github.com/GameFrameX
//  GitHub Repository: https://github.com/GameFrameX
//  Gitee  仓库：https://gitee.com/GameFrameX
//  Gitee Repository:  https://gitee.com/GameFrameX
//  官方文档：https://gameframex.doc.alianblank.com/
//  Official Documentation: https://gameframex.doc.alianblank.com/
// ==========================================================================================

#if !FP
using System;
using System.IO;

namespace GameFrameX.Utility.Math
{
    /// <summary>
    /// 表示一个浮点数的结构体，提供高精度的数学运算。
    /// </summary>
    public partial struct FP : IEquatable<GameFrameX.Utility.Math.FP>, IComparable<GameFrameX.Utility.Math.FP>
    {
        private float rawvalue;

        /// <summary>
        /// 获取原始浮点值。
        /// </summary>
        public float RawValue => rawvalue;

        /// <summary>
        /// 表示浮点数的最大值。
        /// </summary>
        public const float MAX_VALUE = float.MaxValue;

        /// <summary>
        /// 表示浮点数的最小值。
        /// </summary>
        public const float MIN_VALUE = float.MinValue;

        /// <summary>
        /// 表示浮点数的值为1。
        /// </summary>
        public const float ONE = 1f;

        /// <summary>
        /// 表示浮点数的值为10。
        /// </summary>
        public const float TEN = 10f;

        /// <summary>
        /// 表示浮点数的值为0.5。
        /// </summary>
        public const float HALF = 0.5f;

        /// <summary>
        /// 表示2π的值。
        /// </summary>
        public const float PI_TIMES_2 = 6.28318530718f;

        /// <summary>
        /// 表示π的值。
        /// </summary>
        public const float PI = 3.141592653589793f;

        /// <summary>
        /// 表示π/2的值。
        /// </summary>
        public const float PI_OVER_2 = 1.5707963267948966f;

        /// <summary>
        /// 表示浮点数的最大值实例。
        /// </summary>
        public static readonly GameFrameX.Utility.Math.FP MaxValue = new GameFrameX.Utility.Math.FP(MAX_VALUE - 1);

        /// <summary>
        /// 表示浮点数的最小值实例。
        /// </summary>
        public static readonly GameFrameX.Utility.Math.FP MinValue = new GameFrameX.Utility.Math.FP(MIN_VALUE + 2);

        /// <summary>
        /// 表示浮点数的值为1的实例。
        /// </summary>
        public static readonly GameFrameX.Utility.Math.FP One = new GameFrameX.Utility.Math.FP(ONE);

        /// <summary>
        /// 表示浮点数的值为10的实例。
        /// </summary>
        public static readonly GameFrameX.Utility.Math.FP Ten = new GameFrameX.Utility.Math.FP(TEN);

        /// <summary>
        /// 表示浮点数的值为0.5的实例。
        /// </summary>
        public static readonly GameFrameX.Utility.Math.FP Half = new GameFrameX.Utility.Math.FP(HALF);

        /// <summary>
        /// 表示浮点数的值为0的实例。
        /// </summary>
        public static readonly GameFrameX.Utility.Math.FP Zero = new GameFrameX.Utility.Math.FP();

        /// <summary>
        /// 表示正无穷大的浮点数实例。
        /// </summary>
        public static readonly GameFrameX.Utility.Math.FP PositiveInfinity = new GameFrameX.Utility.Math.FP(MAX_VALUE);

        /// <summary>
        /// 表示负无穷大的浮点数实例。
        /// </summary>
        public static readonly GameFrameX.Utility.Math.FP NegativeInfinity = new GameFrameX.Utility.Math.FP(MIN_VALUE + 1);

        /// <summary>
        /// 表示非数字（NaN）的浮点数实例。
        /// </summary>
        public static readonly GameFrameX.Utility.Math.FP NaN = new GameFrameX.Utility.Math.FP(MIN_VALUE);

        /// <summary>
        /// 表示1/10的浮点数实例。
        /// </summary>
        public static readonly GameFrameX.Utility.Math.FP EN1 = One / 10;

        /// <summary>
        /// 表示1/100的浮点数实例。
        /// </summary>
        public static readonly GameFrameX.Utility.Math.FP EN2 = One / 100;

        /// <summary>
        /// 表示1/1000的浮点数实例。
        /// </summary>
        public static readonly GameFrameX.Utility.Math.FP EN3 = One / 1000;

        /// <summary>
        /// 表示1/10000的浮点数实例。
        /// </summary>
        public static readonly GameFrameX.Utility.Math.FP EN4 = One / 10000;

        /// <summary>
        /// 表示1/100000的浮点数实例。
        /// </summary>
        public static readonly GameFrameX.Utility.Math.FP EN5 = One / 100000;

        /// <summary>
        /// 表示1/1000000的浮点数实例。
        /// </summary>
        public static readonly GameFrameX.Utility.Math.FP EN6 = One / 1000000;

        /// <summary>
        /// 表示1/10000000的浮点数实例。
        /// </summary>
        public static readonly GameFrameX.Utility.Math.FP EN7 = One / 10000000;

        /// <summary>
        /// 表示1/100000000的浮点数实例。
        /// </summary>
        public static readonly GameFrameX.Utility.Math.FP EN8 = One / 100000000;

        /// <summary>
        /// 表示一个非常小的浮点数（Epsilon）。
        /// </summary>
        public static readonly GameFrameX.Utility.Math.FP Epsilon = EN3;

        /// <summary>
        /// 表示π的浮点数实例。
        /// </summary>
        public static readonly GameFrameX.Utility.Math.FP Pi = new GameFrameX.Utility.Math.FP(PI);

        /// <summary>
        /// 表示π/2的浮点数实例。
        /// </summary>
        public static readonly GameFrameX.Utility.Math.FP PiOver2 = new GameFrameX.Utility.Math.FP(PI_OVER_2);

        /// <summary>
        /// 表示2π的浮点数实例。
        /// </summary>
        public static readonly GameFrameX.Utility.Math.FP PiTimes2 = new GameFrameX.Utility.Math.FP(PI_TIMES_2);

        /// <summary>
        /// 表示π的倒数的浮点数实例。
        /// </summary>
        public static readonly GameFrameX.Utility.Math.FP PiInv = (GameFrameX.Utility.Math.FP)0.3183098861837906715377675267M;

        /// <summary>
        /// 表示π/2的倒数的浮点数实例。
        /// </summary>
        public static readonly GameFrameX.Utility.Math.FP PiOver2Inv = (GameFrameX.Utility.Math.FP)0.6366197723675813430755350535M;

        /// <summary>
        /// 将角度转换为弧度的浮点数实例。
        /// </summary>
        public static readonly GameFrameX.Utility.Math.FP Deg2Rad = Pi / new GameFrameX.Utility.Math.FP(180);

        /// <summary>
        /// 将弧度转换为角度的浮点数实例。
        /// </summary>
        public static readonly GameFrameX.Utility.Math.FP Rad2Deg = new GameFrameX.Utility.Math.FP(180) / Pi;

        /// <summary>
        /// 返回2的指定幂。
        /// 提供至少6位小数的精度。
        /// </summary>
        internal static GameFrameX.Utility.Math.FP Pow2(GameFrameX.Utility.Math.FP x)
        {
            return new GameFrameX.Utility.Math.FP((float)System.Math.Pow(2, x.rawvalue));
        }
        
        /// <summary>
        /// 返回指定数字的以2为底的对数。
        /// 提供至少9位小数的精度。
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">
        /// 参数为非正数时抛出此异常。
        /// </exception>
        public static GameFrameX.Utility.Math.FP Log2(GameFrameX.Utility.Math.FP x)
        {
            return new GameFrameX.Utility.Math.FP(float.Log2(x.rawvalue));
        }

        /// <summary>
        /// 返回Fix64数字的符号。
        /// 如果值为正，返回1；如果值为0，返回0；如果值为负，返回-1。
        /// </summary>
        public static int Sign(GameFrameX.Utility.Math.FP value)
        {
            return value.rawvalue < 0 ? -1 : value.rawvalue > 0 ? 1 : 0;
        }

        /// <summary>
        /// 返回Fix64数字的绝对值。
        /// 注意：Abs(Fix64.MinValue) == Fix64.MaxValue。
        /// </summary>
        public static GameFrameX.Utility.Math.FP Abs(GameFrameX.Utility.Math.FP value)
        {
            return new GameFrameX.Utility.Math.FP(System.Math.Abs(value.rawvalue));
        }

        /// <summary>
        /// 返回Fix64数字的绝对值。
        /// FastAbs(Fix64.MinValue) 是未定义的。
        /// </summary>
        public static GameFrameX.Utility.Math.FP FastAbs(GameFrameX.Utility.Math.FP value)
        {
            return Abs(value);
        }

        /// <summary>
        /// 返回小于或等于指定数字的最大整数。
        /// </summary>
        public static GameFrameX.Utility.Math.FP Floor(GameFrameX.Utility.Math.FP value)
        {
            return new GameFrameX.Utility.Math.FP(float.Floor(value.rawvalue));
        }

        /// <summary>
        /// 返回大于或等于指定数字的最小整数。
        /// </summary>
        public static GameFrameX.Utility.Math.FP Ceiling(GameFrameX.Utility.Math.FP value)
        {
            return new GameFrameX.Utility.Math.FP(float.Ceiling(value.rawvalue));
        }

        /// <summary>
        /// 将值四舍五入到最接近的整数值。
        /// 如果值恰好在偶数和奇数之间，则返回偶数值。
        /// </summary>
        public static GameFrameX.Utility.Math.FP Round(GameFrameX.Utility.Math.FP value)
        {
            return new GameFrameX.Utility.Math.FP(float.Round(value.rawvalue));
        }

        /// <summary>
        /// 返回两个Fix64数字中的较大者。
        /// </summary>
        public static GameFrameX.Utility.Math.FP Max(GameFrameX.Utility.Math.FP left, GameFrameX.Utility.Math.FP right)
        {
            return new GameFrameX.Utility.Math.FP(float.Max(left.rawvalue, right.rawvalue));
        }

        /// <summary>
        /// 返回两个Fix64数字中的较小者。
        /// </summary>
        public static GameFrameX.Utility.Math.FP Min(GameFrameX.Utility.Math.FP left, GameFrameX.Utility.Math.FP right)
        {
            return new GameFrameX.Utility.Math.FP(float.Min(left.rawvalue, right.rawvalue));
        }

        /// <summary>
        /// 限制指定值在给定的最小值和最大值之间。
        /// </summary>
        public static GameFrameX.Utility.Math.FP Clamp(GameFrameX.Utility.Math.FP origin, GameFrameX.Utility.Math.FP min, GameFrameX.Utility.Math.FP max)
        {
            return Min(Max(origin, min), max);
        }

        /// <summary>
        /// 将x和y相加。执行饱和加法，即在溢出的情况下，
        /// 根据操作数的符号四舍五入到MinValue或MaxValue。
        /// </summary>
        public static GameFrameX.Utility.Math.FP operator +(GameFrameX.Utility.Math.FP x, GameFrameX.Utility.Math.FP y)
        {
            return new GameFrameX.Utility.Math.FP(x.rawvalue + y.rawvalue);
        }

        /// <summary>
        /// 从x中减去y。执行饱和减法，即在溢出的情况下，
        /// 根据操作数的符号四舍五入到MinValue或MaxValue。
        /// </summary>
        public static GameFrameX.Utility.Math.FP operator -(GameFrameX.Utility.Math.FP x, GameFrameX.Utility.Math.FP y)
        {
            return new GameFrameX.Utility.Math.FP(x.rawvalue - y.rawvalue);
        }

        /// <summary>
        /// 从x中减去y而不执行溢出检查。应由CLR内联。
        /// </summary>
        public static GameFrameX.Utility.Math.FP FastSub(GameFrameX.Utility.Math.FP x, GameFrameX.Utility.Math.FP y)
        {
            return x - y;
        }

        /// <summary>
        /// 执行Fix64数字的乘法。
        /// </summary>
        public static GameFrameX.Utility.Math.FP operator *(GameFrameX.Utility.Math.FP x, GameFrameX.Utility.Math.FP y)
        {
            return new GameFrameX.Utility.Math.FP(x.rawvalue * y.rawvalue);
        }

        /// <summary>
        /// 执行乘法而不检查溢出。
        /// 对于保证不会导致溢出的性能关键代码非常有用。
        /// </summary>
        public static GameFrameX.Utility.Math.FP FastMul(GameFrameX.Utility.Math.FP x, GameFrameX.Utility.Math.FP y)
        {
            return x * y;
        }

        /// <summary>
        /// 计算给定数字的前导零位数。
        /// </summary>
        public static int CountLeadingZeroes(ulong x)
        {
            int result = 0;
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
        /// 执行Fix64数字的除法。
        /// </summary>
        public static GameFrameX.Utility.Math.FP operator /(GameFrameX.Utility.Math.FP x, GameFrameX.Utility.Math.FP y)
        {
            return new GameFrameX.Utility.Math.FP(x.rawvalue / y.rawvalue);
        }

        /// <summary>
        /// 执行Fix64数字的取模运算。
        /// </summary>
        public static GameFrameX.Utility.Math.FP operator %(GameFrameX.Utility.Math.FP x, GameFrameX.Utility.Math.FP y)
        {
            return new GameFrameX.Utility.Math.FP(x.rawvalue % y.rawvalue);
        }

        /// <summary>
        /// 返回x的相反数。
        /// </summary>
        public static GameFrameX.Utility.Math.FP operator -(GameFrameX.Utility.Math.FP x)
        {
            return new GameFrameX.Utility.Math.FP(-x.rawvalue);
        }

        /// <summary>
        /// 判断两个Fix64数字是否相等。
        /// </summary>
        public static bool operator ==(GameFrameX.Utility.Math.FP x, GameFrameX.Utility.Math.FP y)
        {
            return x.rawvalue.Equals(y.rawvalue);
        }

        /// <summary>
        /// 判断两个Fix64数字是否不相等。
        /// </summary>
        public static bool operator !=(GameFrameX.Utility.Math.FP x, GameFrameX.Utility.Math.FP y)
        {
            return false == x.rawvalue.Equals(y.rawvalue);
        }

        /// <summary>
        /// 判断x是否大于y。
        /// </summary>
        public static bool operator >(GameFrameX.Utility.Math.FP x, GameFrameX.Utility.Math.FP y)
        {
            return x.rawvalue > y.rawvalue;
        }

        /// <summary>
        /// 判断x是否小于y。
        /// </summary>
        public static bool operator <(GameFrameX.Utility.Math.FP x, GameFrameX.Utility.Math.FP y)
        {
            return x.rawvalue < y.rawvalue;
        }

        /// <summary>
        /// 判断x是否大于或等于y。
        /// </summary>
        public static bool operator >=(GameFrameX.Utility.Math.FP x, GameFrameX.Utility.Math.FP y)
        {
            return x.rawvalue >= y.rawvalue;
        }

        /// <summary>
        /// 判断x是否小于或等于y。
        /// </summary>
        public static bool operator <=(GameFrameX.Utility.Math.FP x, GameFrameX.Utility.Math.FP y)
        {
            return x.rawvalue <= y.rawvalue;
        }

        /// <summary>
        /// 返回指定数字的平方根。
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">
        /// <paramref name="x" />为负数时传递。
        /// </exception>
        public static GameFrameX.Utility.Math.FP Sqrt(GameFrameX.Utility.Math.FP x)
        {
            return new GameFrameX.Utility.Math.FP(float.Sqrt(x.rawvalue));
        }

        /// <summary>
        /// 返回x的正弦值。
        /// 对于小值x，该函数的精度约为9位小数。
        /// 随着x值的增大，可能会失去精度。
        /// 性能：在x64中比Math.Sin()慢约25%，在x86中慢200%。
        /// </summary>
        public static GameFrameX.Utility.Math.FP Sin(GameFrameX.Utility.Math.FP x)
        {
            return new GameFrameX.Utility.Math.FP(float.Sin(x.rawvalue));
        }

        /// <summary>
        /// 返回x的粗略正弦值近似。
        /// 在x86上比Sin()快至少3倍，并且比Math.Sin()稍快，
        /// 但其精度限制在4-5位小数，适用于足够小的x值。
        /// </summary>
        public static GameFrameX.Utility.Math.FP FastSin(GameFrameX.Utility.Math.FP x)
        {
            return Sin(x);
        }

        /// <summary>
        /// 返回x的余弦值。
        /// 有关Sin()的更多详细信息，请参见Sin()。
        /// </summary>
        public static GameFrameX.Utility.Math.FP Cos(GameFrameX.Utility.Math.FP x)
        {
            return new GameFrameX.Utility.Math.FP(float.Cos(x.rawvalue));
        }

        /// <summary>
        /// 返回x的粗略余弦值近似。
        /// 有关FastSin的更多详细信息，请参见FastSin。
        /// </summary>
        public static GameFrameX.Utility.Math.FP FastCos(GameFrameX.Utility.Math.FP x)
        {
            return Cos(x);
        }

        /// <summary>
        /// 返回x的正切值。
        /// </summary>
        /// <remarks>
        /// 此函数未经过充分测试，可能会非常不准确。
        /// </remarks>
        public static GameFrameX.Utility.Math.FP Tan(GameFrameX.Utility.Math.FP x)
        {
            return new GameFrameX.Utility.Math.FP(float.Tan(x.rawvalue));
        }

        /// <summary>
        /// 返回指定数字的反正切值，使用欧拉级数计算。
        /// 此函数的精度至少为7位小数。
        /// </summary>
        public static GameFrameX.Utility.Math.FP Atan(GameFrameX.Utility.Math.FP z)
        {
            return new GameFrameX.Utility.Math.FP(float.Atan(z.rawvalue));
        }

        /// <summary>
        /// 返回y/x的反正切值。
        /// </summary>
        public static GameFrameX.Utility.Math.FP Atan2(GameFrameX.Utility.Math.FP y, GameFrameX.Utility.Math.FP x)
        {
            return new GameFrameX.Utility.Math.FP(float.Atan2(y.rawvalue, x.rawvalue));
        }

        /// <summary>
        /// 返回指定数字的反正弦值。
        /// </summary>
        public static GameFrameX.Utility.Math.FP Asin(GameFrameX.Utility.Math.FP value)
        {
            return FastSub(PiOver2, Acos(value));
        }

        /// <summary>
        /// 返回指定数字的反余弦值，使用Atan和Sqrt计算。
        /// 此函数的精度至少为7位小数。
        /// </summary>
        public static GameFrameX.Utility.Math.FP Acos(GameFrameX.Utility.Math.FP x)
        {
            return new GameFrameX.Utility.Math.FP(float.Acos(x.rawvalue));
        }

        /// <summary>
        /// 隐式转换为GameFrameX.Utility.Math.FP类型。
        /// </summary>
        public static implicit operator GameFrameX.Utility.Math.FP(long value)
        {
            return new GameFrameX.Utility.Math.FP(value);
        }

        /// <summary>
        /// 显式转换为long类型。
        /// </summary>
        public static explicit operator long(GameFrameX.Utility.Math.FP value)
        {
            return (long)value.rawvalue;
        }

        /// <summary>
        /// 显式转换为float类型。
        /// </summary>
        public static explicit operator float(GameFrameX.Utility.Math.FP value)
        {
            return value.rawvalue;
        }

        /// <summary>
        /// 显式转换为GameFrameX.Utility.Math.FP类型，使用decimal值。
        /// </summary>
        public static explicit operator GameFrameX.Utility.Math.FP(decimal value)
        {
            return new GameFrameX.Utility.Math.FP((float)value);
        }

        /// <summary>
        /// 隐式转换为GameFrameX.Utility.Math.FP类型，使用int值。
        /// </summary>
        public static implicit operator GameFrameX.Utility.Math.FP(int value)
        {
            return new GameFrameX.Utility.Math.FP(value);
        }

        /// <summary>
        /// 显式转换为decimal类型。
        /// </summary>
        public static explicit operator decimal(GameFrameX.Utility.Math.FP value)
        {
            return (decimal)value.rawvalue;
        }

        /// <summary>
        /// 将当前值转换为float类型。
        /// </summary>
        public float AsFloat()
        {
            return (float)this;
        }

        /// <summary>
        /// 将当前值转换为int类型。
        /// </summary>
        public int AsInt()
        {
            return (int)this;
        }

        /// <summary>
        /// 将当前值转换为uint类型。
        /// </summary>
        public uint AsUInt()
        {
            return (uint)this;
        }

        /// <summary>
        /// 将当前值转换为long类型。
        /// </summary>
        public long AsLong()
        {
            return (long)this;
        }

        /// <summary>
        /// 将当前值转换为double类型。
        /// </summary>
        public double AsDouble()
        {
            return (double)this;
        }

        /// <summary>
        /// 将当前值转换为decimal类型。
        /// </summary>
        public decimal AsDecimal()
        {
            return (decimal)this;
        }

        /// <summary>
        /// 将指定的FP值转换为float类型。
        /// </summary>
        public static float ToFloat(GameFrameX.Utility.Math.FP value)
        {
            return (float)value;
        }

        /// <summary>
        /// 将指定的FP值转换为int类型。
        /// </summary>
        public static int ToInt(GameFrameX.Utility.Math.FP value)
        {
            return (int)value;
        }

        /// <summary>
        /// 将指定的FP值转换为uint类型。
        /// </summary>
        public static uint ToUInt(GameFrameX.Utility.Math.FP value)
        {
            return (uint)value;
        }

        /// <summary>
        /// 判断指定的FP值是否为无穷大。
        /// </summary>
        public static bool IsInfinity(GameFrameX.Utility.Math.FP value)
        {
            return value == NegativeInfinity || value == PositiveInfinity;
        }

        /// <summary>
        /// 判断指定的FP值是否为非数字（NaN）。
        /// </summary>
        public static bool IsNaN(GameFrameX.Utility.Math.FP value)
        {
            return value == NaN;
        }

        /// <summary>
        /// 判断当前对象是否与指定对象相等。
        /// </summary>
        public override bool Equals(object obj)
        {
            return obj is GameFrameX.Utility.Math.FP _obj && _obj.rawvalue.Equals(rawvalue);
        }

        /// <summary>
        /// 返回当前对象的哈希代码。
        /// </summary>
        public override int GetHashCode()
        {
            return rawvalue.GetHashCode();
        }

        /// <summary>
        /// 判断当前FP值是否与另一个FP值相等。
        /// </summary>
        public bool Equals(GameFrameX.Utility.Math.FP other)
        {
            return rawvalue.Equals(other.rawvalue);
        }

        /// <summary>
        /// 比较当前FP值与另一个FP值的大小。
        /// </summary>
        public int CompareTo(GameFrameX.Utility.Math.FP other)
        {
            return rawvalue.CompareTo(other.rawvalue);
        }

        /// <summary>
        /// 返回当前FP值的字符串表示形式。
        /// </summary>
        public override string ToString()
        {
            return rawvalue.ToString();
        }

        /// <summary>
        /// 返回当前FP值的字符串表示形式，使用指定的格式提供程序。
        /// </summary>
        public string ToString(IFormatProvider provider)
        {
            return ((float)this).ToString(provider);
        }
        
        /// <summary>
        /// 返回当前FP值的字符串表示形式，使用指定的格式。
        /// </summary>
        public string ToString(string format)
        {
            return ((float)this).ToString(format);
        }

        /// <summary>
        /// 从原始值创建FP实例。
        /// </summary>
        public static GameFrameX.Utility.Math.FP FromRaw(long rawValue)
        {
            return new GameFrameX.Utility.Math.FP(rawValue);
        }

        /// <summary>
        /// 使用float值初始化FP实例。
        /// </summary>
        FP(float val)
        {
            this.rawvalue = val;
        }
        
        /// <summary>
        /// 使用long值初始化FP实例。
        /// </summary>
        FP(long val)
        {
            this.rawvalue = val;
        }

        /// <summary>
        /// 使用int值初始化FP实例。
        /// </summary>
        public FP(int rawvalue)
        {
            this.rawvalue = rawvalue;
        }
    }
}
#endif