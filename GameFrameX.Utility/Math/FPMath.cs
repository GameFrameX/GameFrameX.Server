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

/* Copyright (C) <2009-2011> <Thorben Linneweber, Jitter Physics>
 *
 *  This software is provided 'as-is', without any express or implied
 *  warranty.  In no event will the authors be held liable for any damages
 *  arising from the use of this software.
 *
 *  Permission is granted to anyone to use this software for any purpose,
 *  including commercial applications, and to alter it and redistribute it
 *  freely, subject to the following restrictions:
 *
 *  1. The origin of this software must not be misrepresented; you must not
 *      claim that you wrote the original software. If you use this software
 *      in a product, an acknowledgment in the product documentation would be
 *      appreciated but is not required.
 *  2. Altered source versions must be plainly marked as such, and must not be
 *      misrepresented as being the original software.
 *  3. This notice may not be removed or altered from any source distribution.
 */

namespace GameFrameX.Utility.Math;

/// <summary>
/// 包含常见的数学操作。
/// </summary>
public sealed class FPMath
{
    /// <summary>
    /// PI 常量。
    /// </summary>
    public static FP Pi = FP.Pi;

    /// <summary>
    /// PI 除以 2 的常量。
    /// </summary>
    public static FP PiOver2 = FP.PiOver2;

    /// <summary>
    /// 一个小值，通常用于判断数值结果是否为零。
    /// </summary>
    public static FP Epsilon = FP.Epsilon;

    /// <summary>
    /// 角度转弧度的常量。
    /// </summary>
    public static FP Deg2Rad = FP.Deg2Rad;

    /// <summary>
    /// 弧度转角度的常量。
    /// </summary>
    public static FP Rad2Deg = FP.Rad2Deg;

    /// <summary>
    /// FP 无穷大。
    /// </summary>
    public static FP Infinity = FP.MaxValue;

    /// <summary>
    /// 获取平方根。
    /// </summary>
    /// <param name="number">要计算平方根的数字。</param>
    /// <returns>返回平方根的值。</returns>

    #region public static FP Sqrt(FP number)

    public static FP Sqrt(FP number)
    {
        return FP.Sqrt(number);
    }

    #endregion

    /// <summary>
    /// 获取两个值中的最大值。
    /// </summary>
    /// <param name="val1">第一个值。</param>
    /// <param name="val2">第二个值。</param>
    /// <returns>返回较大的值。</returns>

    #region public static FP Max(FP val1, FP val2)

    public static FP Max(FP val1, FP val2)
    {
        return val1 > val2 ? val1 : val2;
    }

    #endregion

    /// <summary>
    /// 获取两个值中的最小值。
    /// </summary>
    /// <param name="val1">第一个值。</param>
    /// <param name="val2">第二个值。</param>
    /// <returns>返回较小的值。</returns>

    #region public static FP Min(FP val1, FP val2)

    public static FP Min(FP val1, FP val2)
    {
        return val1 < val2 ? val1 : val2;
    }

    #endregion

    /// <summary>
    /// 获取三个值中的最大值。
    /// </summary>
    /// <param name="val1">第一个值。</param>
    /// <param name="val2">第二个值。</param>
    /// <param name="val3">第三个值。</param>
    /// <returns>返回较大的值。</returns>

    #region public static FP Max(FP val1, FP val2,FP val3)

    public static FP Max(FP val1, FP val2, FP val3)
    {
        var max12 = val1 > val2 ? val1 : val2;
        return max12 > val3 ? max12 : val3;
    }

    #endregion

    /// <summary>
    /// 返回一个在 [min,max] 范围内的数字。
    /// </summary>
    /// <param name="value">要限制的值。</param>
    /// <param name="min">最小值。</param>
    /// <param name="max">最大值。</param>
    /// <returns>返回限制后的值。</returns>

    #region public static FP Clamp(FP value, FP min, FP max)

    public static FP Clamp(FP value, FP min, FP max)
    {
        if (value < min)
        {
            value = min;
            return value;
        }

        if (value > max)
        {
            value = max;
        }

        return value;
    }

    #endregion

    /// <summary>
    /// 返回一个在 [FP.Zero, FP.One] 范围内的数字。
    /// </summary>
    /// <param name="value">要限制的值。</param>
    /// <returns>返回限制后的值。</returns>
    public static FP Clamp01(FP value)
    {
        if (value < FP.Zero)
        {
            return FP.Zero;
        }

        if (value > FP.One)
        {
            return FP.One;
        }

        return value;
    }

    /// <summary>
    /// 将矩阵的每个元素的符号更改为 '+'。
    /// </summary>
    /// <param name="matrix">输入矩阵。</param>
    /// <param name="result">输出绝对值矩阵。</param>

    #region public static void Absolute(ref JMatrix matrix,out JMatrix result)

    public static void Absolute(ref FPMatrix matrix, out FPMatrix result)
    {
        result.M11 = FP.Abs(matrix.M11);
        result.M12 = FP.Abs(matrix.M12);
        result.M13 = FP.Abs(matrix.M13);
        result.M21 = FP.Abs(matrix.M21);
        result.M22 = FP.Abs(matrix.M22);
        result.M23 = FP.Abs(matrix.M23);
        result.M31 = FP.Abs(matrix.M31);
        result.M32 = FP.Abs(matrix.M32);
        result.M33 = FP.Abs(matrix.M33);
    }

    #endregion

    /// <summary>
    /// 返回值的正弦。
    /// </summary>
    /// <param name="value">要计算正弦的值。</param>
    /// <returns>返回正弦值。</returns>
    public static FP Sin(FP value)
    {
        return FP.Sin(value);
    }

    /// <summary>
    /// 返回值的余弦。
    /// </summary>
    /// <param name="value">要计算余弦的值。</param>
    /// <returns>返回余弦值。</returns>
    public static FP Cos(FP value)
    {
        return FP.Cos(value);
    }

    /// <summary>
    /// 返回值的正切。
    /// </summary>
    /// <param name="value">要计算正切的值。</param>
    /// <returns>返回正切值。</returns>
    public static FP Tan(FP value)
    {
        return FP.Tan(value);
    }

    /// <summary>
    /// 返回值的反正弦。
    /// </summary>
    /// <param name="value">要计算反正弦的值。</param>
    /// <returns>返回反正弦值。</returns>
    public static FP Asin(FP value)
    {
        return FP.Asin(value);
    }

    /// <summary>
    /// 返回值的反余弦。
    /// </summary>
    /// <param name="value">要计算反余弦的值。</param>
    /// <returns>返回反余弦值。</returns>
    public static FP Acos(FP value)
    {
        return FP.Acos(value);
    }

    /// <summary>
    /// 返回值的反正切。
    /// </summary>
    /// <param name="value">要计算反正切的值。</param>
    /// <returns>返回反正切值。</returns>
    public static FP Atan(FP value)
    {
        return FP.Atan(value);
    }

    /// <summary>
    /// 返回坐标 x-y 的反正切。
    /// </summary>
    /// <param name="y">y 坐标。</param>
    /// <param name="x">x 坐标。</param>
    /// <returns>返回反正切值。</returns>
    public static FP Atan2(FP y, FP x)
    {
        return FP.Atan2(y, x);
    }

    /// <summary>
    /// 返回小于或等于指定数字的最大整数。
    /// </summary>
    /// <param name="value">要计算的值。</param>
    /// <returns>返回小于或等于指定数字的最大整数。</returns>
    public static FP Floor(FP value)
    {
        return FP.Floor(value);
    }

    /// <summary>
    /// 返回大于或等于指定数字的最小整数。
    /// </summary>
    /// <param name="value">要计算的值。</param>
    /// <returns>返回大于或等于指定数字的最小整数。</returns>
    public static FP Ceiling(FP value)
    {
        return FP.Ceiling(value);
    }

    /// <summary>
    /// 将值四舍五入到最接近的整数值。
    /// 如果值恰好在偶数和奇数之间，则返回偶数值。
    /// </summary>
    /// <param name="value">要四舍五入的值。</param>
    /// <returns>返回四舍五入后的值。</returns>
    public static FP Round(FP value)
    {
        return FP.Round(value);
    }

    /// <summary>
    /// 返回 Fix64 数字的符号。
    /// 如果值为正，则返回 1；如果为 0，则返回 0；如果为负，则返回 -1。
    /// </summary>
    /// <param name="value">要检查符号的值。</param>
    /// <returns>返回值的符号。</returns>
    public static int Sign(FP value)
    {
        return FP.Sign(value);
    }

    /// <summary>
    /// 返回 Fix64 数字的绝对值。
    /// 注意：Abs(Fix64.MinValue) == Fix64.MaxValue。
    /// </summary>
    /// <param name="value">要计算绝对值的值。</param>
    /// <returns>返回绝对值。</returns>
    public static FP Abs(FP value)
    {
        return FP.Abs(value);
    }

    /// <summary>
    /// 返回三个值的重心插值。
    /// </summary>
    /// <param name="value1">第一个值。</param>
    /// <param name="value2">第二个值。</param>
    /// <param name="value3">第三个值。</param>
    /// <param name="amount1">第一个权重。</param>
    /// <param name="amount2">第二个权重。</param>
    /// <returns>返回重心插值的结果。</returns>
    public static FP Barycentric(FP value1, FP value2, FP value3, FP amount1, FP amount2)
    {
        return value1 + (value2 - value1) * amount1 + (value3 - value1) * amount2;
    }

    /// <summary>
    /// 返回四个值的 CatmullRom 插值。
    /// </summary>
    /// <param name="value1">第一个值。</param>
    /// <param name="value2">第二个值。</param>
    /// <param name="value3">第三个值。</param>
    /// <param name="value4">第四个值。</param>
    /// <param name="amount">插值量。</param>
    /// <returns>返回 CatmullRom 插值的结果。</returns>
    public static FP CatmullRom(FP value1, FP value2, FP value3, FP value4, FP amount)
    {
        // 使用来自 http://www.mvps.org/directx/articles/catmull/ 的公式
        // 内部使用 FPs 以避免精度损失
        var amountSquared = amount * amount;
        var amountCubed = amountSquared * amount;
        return FP.Half * (2 * FP.One * value2 +
                          (value3 - value1) * amount +
                          (2 * FP.One * value1 - 5 * FP.One * value2 + 4 * FP.One * value3 - value4) * amountSquared +
                          (2 * FP.One * value2 - value1 - 3 * FP.One * value3 + value4) * amountCubed);
    }

    /// <summary>
    /// 返回两个值之间的距离。
    /// </summary>
    /// <param name="value1">第一个值。</param>
    /// <param name="value2">第二个值。</param>
    /// <returns>返回两个值之间的距离。</returns>
    public static FP Distance(FP value1, FP value2)
    {
        return FP.Abs(value1 - value2);
    }

    /// <summary>
    /// 返回两个值的 Hermite 插值。
    /// </summary>
    /// <param name="value1">第一个值。</param>
    /// <param name="tangent1">第一个切线。</param>
    /// <param name="value2">第二个值。</param>
    /// <param name="tangent2">第二个切线。</param>
    /// <param name="amount">插值量。</param>
    /// <returns>返回 Hermite 插值的结果。</returns>
    public static FP Hermite(FP value1, FP tangent1, FP value2, FP tangent2, FP amount)
    {
        // 所有值转换为 FP 以避免精度损失
        // 否则，对于高数量的参数：amount，结果将是 NaN 而不是无穷大
        FP v1 = value1, v2 = value2, t1 = tangent1, t2 = tangent2, s = amount, result;
        var sCubed = s * s * s;
        var sSquared = s * s;

        if (amount == FP.Zero)
        {
            result = value1;
        }
        else if (amount == FP.One)
        {
            result = value2;
        }
        else
        {
            result = (2 * v1 - 2 * v2 + t2 + t1) * sCubed +
                     (3 * v2 - 3 * v1 - 2 * t1 - t2) * sSquared +
                     t1 * s +
                     v1;
        }

        return result;
    }

    /// <summary>
    /// 返回两个值的线性插值。
    /// </summary>
    /// <param name="value1">第一个值。</param>
    /// <param name="value2">第二个值。</param>
    /// <param name="amount">插值量。</param>
    /// <returns>返回线性插值的结果。</returns>
    public static FP Lerp(FP value1, FP value2, FP amount)
    {
        return value1 + (value2 - value1) * Clamp01(amount);
    }

    /// <summary>
    /// 返回两个值的反向线性插值。
    /// </summary>
    /// <param name="value1">第一个值。</param>
    /// <param name="value2">第二个值。</param>
    /// <param name="amount">插值量。</param>
    /// <returns>返回反向线性插值的结果。</returns>
    public static FP InverseLerp(FP value1, FP value2, FP amount)
    {
        if (value1 != value2)
        {
            return Clamp01((amount - value1) / (value2 - value1));
        }

        return FP.Zero;
    }

    /// <summary>
    /// 返回两个值的平滑 Hermite 插值。
    /// </summary>
    /// <param name="value1">第一个值。</param>
    /// <param name="value2">第二个值。</param>
    /// <param name="amount">插值量。</param>
    /// <returns>返回平滑 Hermite 插值的结果。</returns>
    public static FP SmoothStep(FP value1, FP value2, FP amount)
    {
        // 预期 0 < amount < 1
        // 如果 amount < 0，返回 value1
        // 如果 amount > 1，返回 value2
        var result = Clamp(amount, FP.Zero, FP.One);
        result = Hermite(value1, FP.Zero, value2, FP.Zero, result);
        return result;
    }

    /// <summary>
    /// 返回 2 的指定幂。
    /// 提供至少 6 位小数的精度。
    /// </summary>
    /// <param name="x">要计算的指数。</param>
    /// <returns>返回 2 的 x 次幂。</returns>
    internal static FP Pow2(FP x)
    {
        return FP.Pow2(x);
    }

    /// <summary>
    /// 返回指定数字的以 2 为底的对数。
    /// 提供至少 9 位小数的精度。
    /// </summary>
    /// <param name="x">要计算对数的值。</param>
    /// <exception cref="ArgumentOutOfRangeException">
    /// 参数为非正数时抛出。
    /// </exception>
    internal static FP Log2(FP x)
    {
        return FP.Log2(x);
    }

    /// <summary>
    /// 返回指定数字的指定幂。
    /// 提供约 5 位数字的结果精度。
    /// </summary>
    /// <param name="b">底数。</param>
    /// <param name="exp">指数。</param>
    /// <exception cref="DivideByZeroException">
    /// 底数为零且指数为负时抛出。
    /// </exception>
    /// <exception cref="ArgumentOutOfRangeException">
    /// 底数为负且指数非零时抛出。
    /// </exception>
    public static FP Pow(FP b, FP exp)
    {
        if (b == FP.One)
        {
            return FP.One;
        }

        if (exp.RawValue == 0)
        {
            return FP.One;
        }

        if (b.RawValue == 0)
        {
            if (exp.RawValue < 0)
            {
                //throw new DivideByZeroException();
                return FP.MaxValue;
            }

            return FP.Zero;
        }

        var log2 = Log2(b);
        return Pow2(exp * log2);
    }

    /// <summary>
    /// 返回一个在最小值和最大值之间限制的值。
    /// </summary>
    /// <param name="current">当前值。</param>
    /// <param name="target">目标值。</param>
    /// <param name="maxDelta">最大变化量。</param>
    /// <returns>返回限制后的值。</returns>
    public static FP MoveTowards(FP current, FP target, FP maxDelta)
    {
        if (Abs(target - current) <= maxDelta)
        {
            return target;
        }

        return current + Sign(target - current) * maxDelta;
    }

    /// <summary>
    /// 返回大于或等于指定数字的最小整数。
    /// </summary>
    /// <param name="t">要计算的值。</param>
    /// <param name="length">长度。</param>
    /// <returns>返回结果。</returns>
    public static FP Repeat(FP t, FP length)
    {
        return t - Floor(t / length) * length;
    }

    /// <summary>
    /// 返回两个向量之间的角度。
    /// </summary>
    /// <param name="current">当前角度。</param>
    /// <param name="target">目标角度。</param>
    /// <returns>返回两个角度之间的差值。</returns>
    public static FP DeltaAngle(FP current, FP target)
    {
        var num = Repeat(target - current, 360 * FP.One);
        if (num > 180 * FP.One)
        {
            num -= 360 * FP.One;
        }

        return num;
    }

    /// <summary>
    /// 返回两个向量之间的角度。
    /// </summary>
    /// <param name="current">当前角度。</param>
    /// <param name="target">目标角度。</param>
    /// <param name="maxDelta">最大变化量。</param>
    /// <returns>返回更新后的角度。</returns>
    public static FP MoveTowardsAngle(FP current, FP target, FP maxDelta)
    {
        target = current + DeltaAngle(current, target);
        return MoveTowards(current, target, maxDelta);
    }

    /// <summary>
    /// 返回平滑阻尼函数的值。
    /// </summary>
    /// <param name="current">当前值。</param>
    /// <param name="target">目标值。</param>
    /// <param name="currentVelocity">当前速度。</param>
    /// <param name="smoothTime">平滑时间。</param>
    /// <param name="maxSpeed">最大速度。</param>
    /// <returns>返回平滑阻尼后的值。</returns>
    public static FP SmoothDamp(FP current, FP target, ref FP currentVelocity, FP smoothTime, FP maxSpeed)
    {
        var deltaTime = FP.EN2;
        return SmoothDamp(current, target, ref currentVelocity, smoothTime, maxSpeed, deltaTime);
    }

    /// <summary>
    /// 返回平滑阻尼函数的值。
    /// </summary>
    /// <param name="current">当前值。</param>
    /// <param name="target">目标值。</param>
    /// <param name="currentVelocity">当前速度。</param>
    /// <param name="smoothTime">平滑时间。</param>
    /// <returns>返回平滑阻尼后的值。</returns>
    public static FP SmoothDamp(FP current, FP target, ref FP currentVelocity, FP smoothTime)
    {
        var deltaTime = FP.EN2;
        var positiveInfinity = FP.MaxValue;
        return SmoothDamp(current, target, ref currentVelocity, smoothTime, positiveInfinity, deltaTime);
    }

    /// <summary>
    /// 返回平滑阻尼函数的值。
    /// </summary>
    /// <param name="current">当前值。</param>
    /// <param name="target">目标值。</param>
    /// <param name="currentVelocity">当前速度。</param>
    /// <param name="smoothTime">平滑时间。</param>
    /// <param name="maxSpeed">最大速度。</param>
    /// <param name="deltaTime">时间增量。</param>
    /// <returns>返回平滑阻尼后的值。</returns>
    public static FP SmoothDamp(FP current, FP target, ref FP currentVelocity, FP smoothTime, FP maxSpeed, FP deltaTime)
    {
        smoothTime = Max(FP.EN4, smoothTime);
        var num = 2 * FP.One / smoothTime;
        var num2 = num * deltaTime;
        var num3 = FP.One / (FP.One + num2 + 48 * FP.EN2 * num2 * num2 + 235 * FP.EN3 * num2 * num2 * num2);
        var num4 = current - target;
        var num5 = target;
        var max = maxSpeed * smoothTime;
        num4 = Clamp(num4, -max, max);
        target = current - num4;
        var num7 = (currentVelocity + num * num4) * deltaTime;
        currentVelocity = (currentVelocity - num * num7) * num3;
        var num8 = target + (num4 + num7) * num3;
        if (num5 - current > FP.Zero == num8 > num5)
        {
            num8 = num5;
            currentVelocity = (num8 - num5) / deltaTime;
        }

        return num8;
    }
}