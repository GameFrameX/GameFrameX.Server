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

using GameFrameX.Foundation.Logger;

namespace GameFrameX.Utility;

/// <summary>
/// </summary>
public struct LNumber : IComparable<LNumber>, IEquatable<LNumber>
{
    /// <summary>
    /// </summary>
    public const int FRACTION_BITS = 14; // 小数位位数 14

    private const int INTEGER_BITS = sizeof(long) * 8 - FRACTION_BITS; // 整数位位数 50

    private const int FRACTION_MASK = (int)(uint.MaxValue >> INTEGER_BITS); // 2^14-1 = 16384-1 =16383 == 01111111111111
    private const int INTEGER_MASK = -1 & ~FRACTION_MASK; // -16384
    private const int FRACTION_RANGE = FRACTION_MASK + 1; // 16384 == 10000000000000

    /// <summary>
    /// </summary>
    public const long Max = 562949953421311; // 2^50 = 1125899906842624 - 1

    /// <summary>
    /// </summary>
    public const long FMax = 9999; //  2^14-1 = 16384-1 =16383 == 01111111111111

    /// <summary>
    /// </summary>
    public static readonly LNumber MaxValue = Create_Row(Max);

    /// <summary>
    /// 最小值
    /// </summary>
    public static readonly LNumber MinValue = Create_Row(-Max);

    /// <summary>
    /// 0
    /// </summary>
    public static readonly LNumber zero = Create_Row(0);

    /// <summary>
    /// 1
    /// </summary>
    public static readonly LNumber one = 1;

    /// <summary>
    /// -1
    /// </summary>
    public static readonly LNumber minus_one = -one;

    /// <summary>
    /// 1
    /// </summary>
    public static readonly LNumber epsilon = Create_Row(1);

    /// <summary>
    /// 0
    /// </summary>
    public static readonly LNumber Zero = new();

    private const int Muti_FACTOR = 16384;

    /// <summary>
    /// </summary>
    public long Raw;

    /// <summary>
    /// 天花板数
    /// </summary>
    public long Ceiling
    {
        get
        {
            // 如果没有小数部分，直接返回整数部分
            if ((Raw & FRACTION_MASK) == 0)
            {
                return Raw >> FRACTION_BITS;
            }
            
            // 有小数部分时，正数向上取整，负数向零取整（即向上取整）
            if (Raw > 0)
            {
                return (Raw >> FRACTION_BITS) + 1;
            }
            else
            {
                // 对于负数，向零取整意味着要加1（因为右移对负数是向下取整）
                return (Raw >> FRACTION_BITS) + 1;
            }
        }
    }

    /// <summary>
    /// 地板数
    /// </summary>
    public long Floor
    {
        get
        {
            // 直接右移，这对正数和负数都能正确实现向下取整
            return Raw >> FRACTION_BITS;
        }
    }

    /// <summary>
    /// </summary>
    /// <param name="i"></param>
    /// <param name="f"></param>
    /// <returns></returns>
    public static LNumber Create(long i, long f)
    {
#if XNUMBER_CHECK
        if (i > Max || i < -Max || f > FMax || f < -FMax)
            Debug.LogError("Xnumber 创建失败！ " + i + "." + f);
#endif

        // 确定符号：如果整数部分为负，则结果为负；如果整数部分为0且小数部分为负，则结果为负
        var sign = (i < 0 || (i == 0 && f < 0)) ? -1 : 1;

        if (i < 0)
        {
            i = -i;
        }

        if (f < 0)
        {
            f = -f;
        }

        i = i << FRACTION_BITS;
        f = (f << FRACTION_BITS) / 10000;

        LNumber ret;
        ret.Raw = sign * (i + f);
        return ret;
    }

    /// <summary>
    /// </summary>
    /// <param name="i"></param>
    /// <returns></returns>
    public static LNumber Create_Row(long i)
    {
        LNumber ret;
        ret.Raw = i;
        return ret;
    }

    /// <summary>Compares the current instance with another object of the same type and returns an integer that indicates whether the current instance precedes, follows, or occurs in the same position in the sort order as the other object.</summary>
    /// <param name="other">An object to compare with this instance.</param>
    /// <returns>
    /// A value that indicates the relative order of the objects being compared. The return value has these meanings:
    /// <list type="table">
    ///     <listheader>
    ///         <term> Value</term><description> Meaning</description>
    ///     </listheader>
    ///     <item>
    ///         <term> Less than zero</term><description> This instance precedes <paramref name="other" /> in the sort order.</description>
    ///     </item>
    ///     <item>
    ///         <term> Zero</term><description> This instance occurs in the same position in the sort order as <paramref name="other" />.</description>
    ///     </item>
    ///     <item>
    ///         <term> Greater than zero</term><description> This instance follows <paramref name="other" /> in the sort order.</description>
    ///     </item>
    /// </list>
    /// </returns>
    public int CompareTo(LNumber other)
    {
        return CompareTo(other.Raw);
    }

    private int CompareTo(long other)
    {
        return Raw.CompareTo(other);
    }

    /// <summary>
    /// 判断是否相等
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    public bool Equals(LNumber other)
    {
        return Raw == other.Raw;
    }

    /// <summary>Indicates whether this instance and a specified object are equal.</summary>
    /// <param name="obj">The object to compare with the current instance.</param>
    /// <returns>
    /// <see langword="true" /> if <paramref name="obj" /> and this instance are the same type and represent the same value; otherwise, <see langword="false" />.
    /// </returns>
    public override bool Equals(object obj)
    {
        return obj is LNumber && (LNumber)obj == this;
    }

    /// <summary>
    /// 获取哈希
    /// </summary>
    /// <returns></returns>
    public override int GetHashCode()
    {
        return Raw.GetHashCode();
    }

    /// <summary>
    /// 转为字符串
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
        return ((double)this).ToString("f4");
    }

    /// <summary>
    /// 格式化
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public string ToString(string str)
    {
        return ((double)this).ToString(str);
    }


    /**********************操作符号重载*****************************/

    /// <summary>
    /// 二元操作符 +
    /// </summary>
    /// <param name="lhs"></param>
    /// <param name="rhs"></param>
    /// <returns></returns>
    public static LNumber operator +(LNumber lhs, LNumber rhs)
    {
        LNumber r;
        r.Raw = lhs.Raw + rhs.Raw;
        return r;
    }

    /// <summary>
    /// 二元操作符 -
    /// </summary>
    /// <param name="lhs"></param>
    /// <param name="rhs"></param>
    /// <returns></returns>
    public static LNumber operator -(LNumber lhs, LNumber rhs)
    {
        LNumber r;
        r.Raw = lhs.Raw - rhs.Raw;
        return r;
    }

    /// <summary>
    /// 二元操作符 *
    /// </summary>
    /// <param name="lhs"></param>
    /// <param name="rhs"></param>
    /// <returns></returns>
    public static LNumber operator *(LNumber lhs, LNumber rhs)
    {
#if XNUMBER_CHECK
        var tmp = (int)lhs * (int)rhs;
        if (tmp > max || tmp < -max)
            Debug.LogError("Number数据超上限了 " + lhs + " * " + rhs);
#endif
        LNumber r;
        if (lhs.Raw > int.MaxValue || rhs.Raw > int.MaxValue || lhs.Raw < int.MinValue || rhs.Raw < int.MinValue)
        {
            //可能越界
            BigInteger a = lhs.Raw;
            BigInteger b = rhs.Raw;
            var c = (a * b + (FRACTION_RANGE >> 1)) >> FRACTION_BITS;

            if (c > long.MinValue && c < long.MaxValue)
            {
                r.Raw = long.Parse(c.ToString()); //未越界
            }
            else if ((lhs > 0 && rhs > 0) || (lhs < 0 && rhs < 0))
            {
                LogHelper.Error("LNumber*已越界>" + c);
                r.Raw = long.MaxValue;
            }
            else
            {
                LogHelper.Error("LNumber*已越界>" + c);
                r.Raw = long.MinValue;
            }
        }
        else
        {
            r.Raw = (lhs.Raw * rhs.Raw + (FRACTION_RANGE >> 1)) >> FRACTION_BITS;
        }

        return r;
    }

    /// <summary>
    /// 二元操作符 /
    /// </summary>
    /// <param name="lhs"></param>
    /// <param name="rhs"></param>
    /// <returns></returns>
    public static LNumber operator /(LNumber lhs, LNumber rhs)
    {
        if (lhs.Raw == 0)
        {
            return 0;
        }

        var factor = 1;
        if (rhs.Raw < 0)
        {
            factor = -1;
        }

        if ((rhs.Raw + factor) >> 1 == 0)
        {
            //Debug.LogError("除0了");
            return 0;
        }

        LNumber r;
        if (lhs.Raw > 1L << (62 - FRACTION_BITS))
        {
            //可能越界了
            BigInteger a = lhs.Raw;
            BigInteger b = rhs.Raw;
            var c = ((a << (FRACTION_BITS + 1)) / b + factor) >> 1;

            if (c > long.MinValue && c < long.MaxValue)
            {
                r.Raw = long.Parse(c.ToString()); //未越界
            }
            else if ((lhs > 0 && rhs > 0) || (lhs < 0 && rhs < 0))
            {
                LogHelper.Error("LNumber/已越界>" + c);
                r.Raw = long.MaxValue;
            }
            else
            {
                LogHelper.Error("LNumber/已越界>" + c);
                r.Raw = long.MinValue;
            }
        }
        else
        {
            r.Raw = ((lhs.Raw << (FRACTION_BITS + 1)) / rhs.Raw + factor) >> 1;
        }

        return r;
    }

    /// <summary>
    /// 一元操作符 - (负数操作)
    /// </summary>
    /// <param name="x"></param>
    /// <returns></returns>
    public static LNumber operator -(LNumber x)
    {
        LNumber r;
        r.Raw = -x.Raw;
        return r;
    }

    /// <summary>
    /// 二元操作符 %
    /// </summary>
    /// <param name="lhs"></param>
    /// <param name="rhs"></param>
    /// <returns></returns>
    public static LNumber operator %(LNumber lhs, LNumber rhs)
    {
        LNumber r;
        r.Raw = lhs.Raw % rhs.Raw;
        return r;
    }

    /// <summary>
    /// 比较运算符 小于
    /// </summary>
    /// <param name="lhs"></param>
    /// <param name="rhs"></param>
    /// <returns></returns>
    public static bool operator <(LNumber lhs, LNumber rhs)
    {
        return lhs.Raw < rhs.Raw;
    }

    /// <summary>
    /// 比较运算符 小于等于
    /// </summary>
    /// <param name="lhs"></param>
    /// <param name="rhs"></param>
    /// <returns></returns>
    public static bool operator <=(LNumber lhs, LNumber rhs)
    {
        return lhs.Raw <= rhs.Raw;
    }

    /// <summary>
    /// 比较运算符 >
    /// </summary>
    /// <param name="lhs"></param>
    /// <param name="rhs"></param>
    /// <returns></returns>
    public static bool operator >(LNumber lhs, LNumber rhs)
    {
        return lhs.Raw > rhs.Raw;
    }

    /// <summary>
    /// 比较运算符 >=
    /// </summary>
    /// <param name="lhs"></param>
    /// <param name="rhs"></param>
    /// <returns></returns>
    public static bool operator >=(LNumber lhs, LNumber rhs)
    {
        return lhs.Raw >= rhs.Raw;
    }

    /// <summary>
    /// 比较运算符 ==
    /// </summary>
    /// <param name="lhs"></param>
    /// <param name="rhs"></param>
    /// <returns></returns>
    public static bool operator ==(LNumber lhs, LNumber rhs)
    {
        return lhs.Raw == rhs.Raw;
    }

    /// <summary>
    /// 比较运算符 !=
    /// </summary>
    /// <param name="lhs"></param>
    /// <param name="rhs"></param>
    /// <returns></returns>
    public static bool operator !=(LNumber lhs, LNumber rhs)
    {
        return lhs.Raw != rhs.Raw;
    }

    /**********************数据类型转换*****************************/

    /// <summary>
    /// long类型转换
    /// </summary>
    /// <param name="number"></param>
    /// <returns></returns>
    public static explicit operator long(LNumber number)
    {
        if (number.Raw > 0)
        {
            return number.Raw >> FRACTION_BITS;
        }

        return (number.Raw + FRACTION_MASK) >> FRACTION_BITS;
    }

    /// <summary>
    /// double类型转换
    /// </summary>
    /// <param name="number"></param>
    /// <returns></returns>
    public static explicit operator double(LNumber number)
    {
        return (number.Raw >> FRACTION_BITS) + (number.Raw & FRACTION_MASK) / (double)FRACTION_RANGE;
    }

    /// <summary>
    /// float 类型转换
    /// </summary>
    /// <param name="number"></param>
    /// <returns></returns>
    public static implicit operator float(LNumber number)
    {
        return (float)(double)number;
    }

    /**********************赋值运算*****************************/
    /// <summary>
    /// 赋值运算
    /// </summary>
    /// <param name="value"></param>
    public static implicit operator LNumber(long value)
    {
#if XNUMBER_CHECK
        var tmp = value;
        if (tmp > Max || tmp < -Max)
            Debug.LogError("Number数据超上限了 " + value);
#endif
        //LNumber r;
        //r.raw = value << FRACTION_BITS;
        //return r;
        return Create(value, 0);
    }

    /// <summary>
    /// 赋值运算
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static implicit operator LNumber(int value)
    {
#if XNUMBER_CHECK
        var tmp = value;
        if (tmp > Max || tmp < -Max)
            Debug.LogError("Number数据超上限了 " + value);
#endif
        //LNumber r;
        //r.raw = value << FRACTION_BITS;
        return Create(value, 0);
    }

    /*public static implicit operator LNumber(float number)
{
#if XNUMBER_CHECK
    var tmp = (long)number;
    if (tmp > Max || tmp < -Max)
        Debug.LogError("Number数据超上限了 " + number);
#endif
    return Convert(number);
}

public static LNumber Convert(float f)
{
    LNumber r;
    r.raw = (long)(f * Muti_FACTOR);
    return r;
}*/
}