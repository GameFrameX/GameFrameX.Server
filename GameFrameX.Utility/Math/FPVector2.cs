#region License

/*
MIT License
Copyright © 2006 The Mono.Xna Team

All rights reserved.

Authors
 * Alan McGovern

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/

#endregion License

namespace GameFrameX.Utility.Math;

/// <summary>
/// 表示一个二维向量结构。
/// </summary>
[Serializable]
public struct FPVector2 : IEquatable<FPVector2>
{
    #region Private Fields

    /// <summary>
    /// 零向量 (0, 0) 的私有静态实例。
    /// </summary>
    private static FPVector2 zeroVector = new(0, 0);

    /// <summary>
    /// 单位向量 (1, 1) 的私有静态实例。
    /// </summary>
    private static FPVector2 oneVector = new(1, 1);

    /// <summary>
    /// 右方向向量 (1, 0) 的私有静态实例。
    /// </summary>
    private static FPVector2 rightVector = new(1, 0);

    /// <summary>
    /// 左方向向量 (-1, 0) 的私有静态实例。
    /// </summary>
    private static FPVector2 leftVector = new(-1, 0);

    /// <summary>
    /// 上方向向量 (0, 1) 的私有静态实例。
    /// </summary>
    private static FPVector2 upVector = new(0, 1);

    /// <summary>
    /// 下方向向量 (0, -1) 的私有静态实例。
    /// </summary>
    private static FPVector2 downVector = new(0, -1);

    #endregion Private Fields

    #region Public Fields

    /// <summary>
    /// 向量的 X 坐标。
    /// </summary>
    public FP x;

    /// <summary>
    /// 向量的 Y 坐标。
    /// </summary>
    public FP y;

    #endregion Public Fields

    #region Properties

    /// <summary>
    /// 获取一个表示零向量 (0, 0) 的静态属性。
    /// </summary>
    public static FPVector2 zero
    {
        get { return zeroVector; }
    }

    /// <summary>
    /// 获取一个表示单位向量 (1, 1) 的静态属性。
    /// </summary>
    public static FPVector2 one
    {
        get { return oneVector; }
    }

    /// <summary>
    /// 获取一个表示右方向向量 (1, 0) 的静态属性。
    /// </summary>
    public static FPVector2 right
    {
        get { return rightVector; }
    }

    /// <summary>
    /// 获取一个表示左方向向量 (-1, 0) 的静态属性。
    /// </summary>
    public static FPVector2 left
    {
        get { return leftVector; }
    }

    /// <summary>
    /// 获取一个表示上方向向量 (0, 1) 的静态属性。
    /// </summary>
    public static FPVector2 up
    {
        get { return upVector; }
    }

    /// <summary>
    /// 获取一个表示下方向向量 (0, -1) 的静态属性。
    /// </summary>
    public static FPVector2 down
    {
        get { return downVector; }
    }

    #endregion Properties

    #region Constructors

    /// <summary>
    /// 标准 2D 向量的构造函数。
    /// </summary>
    /// <param name="x">向量的 X 坐标。</param>
    /// <param name="y">向量的 Y 坐标。</param>
    public FPVector2(FP x, FP y)
    {
        this.x = x;
        this.y = y;
    }

    /// <summary>
    /// 创建一个“方形”向量的构造函数，即 X 和 Y 坐标相同。
    /// </summary>
    /// <param name="value">向量的 X 和 Y 坐标值。</param>
    public FPVector2(FP value)
    {
        x = value;
        y = value;
    }

    /// <summary>
    /// 设置向量的 X 和 Y 坐标。
    /// </summary>
    /// <param name="x">向量的 X 坐标。</param>
    /// <param name="y">向量的 Y 坐标。</param>
    public void Set(FP x, FP y)
    {
        this.x = x;
        this.y = y;
    }

    #endregion Constructors

    #region Public Methods

    /// <summary>
    /// 计算向量在给定法线上的反射向量。
    /// </summary>
    /// <param name="vector">原始向量。</param>
    /// <param name="normal">法线向量。</param>
    /// <param name="result">反射向量。</param>
    public static void Reflect(ref FPVector2 vector, ref FPVector2 normal, out FPVector2 result)
    {
        var dot = Dot(vector, normal);
        result.x = vector.x - 2 * FP.One * dot * normal.x;
        result.y = vector.y - 2 * FP.One * dot * normal.y;
    }

    /// <summary>
    /// 计算向量在给定法线上的反射向量。
    /// </summary>
    /// <param name="vector">原始向量。</param>
    /// <param name="normal">法线向量。</param>
    /// <returns>反射向量。</returns>
    public static FPVector2 Reflect(FPVector2 vector, FPVector2 normal)
    {
        Reflect(ref vector, ref normal, out var result);
        return result;
    }

    /// <summary>
    /// 计算两个 FPVector2 的和。
    /// </summary>
    /// <param name="value1">第一个 FPVector2。</param>
    /// <param name="value2">第二个 FPVector2。</param>
    /// <returns>两个 FPVector2 的和。</returns>
    public static FPVector2 Add(FPVector2 value1, FPVector2 value2)
    {
        value1.x += value2.x;
        value1.y += value2.y;
        return value1;
    }

    /// <summary>
    /// 计算两个 FPVector2 的和。
    /// </summary>
    /// <param name="value1">第一个 FPVector2。</param>
    /// <param name="value2">第二个 FPVector2。</param>
    /// <param name="result">两个 FPVector2 的和。</param>
    public static void Add(ref FPVector2 value1, ref FPVector2 value2, out FPVector2 result)
    {
        result.x = value1.x + value2.x;
        result.y = value1.y + value2.y;
    }

    /// <summary>
    /// 计算三个点的重心插值。
    /// </summary>
    /// <param name="value1">第一个点。</param>
    /// <param name="value2">第二个点。</param>
    /// <param name="value3">第三个点。</param>
    /// <param name="amount1">权重1。</param>
    /// <param name="amount2">权重2。</param>
    /// <returns>重心插值结果。</returns>
    public static FPVector2 Barycentric(FPVector2 value1, FPVector2 value2, FPVector2 value3, FP amount1, FP amount2)
    {
        return new FPVector2(
            FPMath.Barycentric(value1.x, value2.x, value3.x, amount1, amount2),
            FPMath.Barycentric(value1.y, value2.y, value3.y, amount1, amount2));
    }

    /// <summary>
    /// 计算三个点的重心插值。
    /// </summary>
    /// <param name="value1">第一个点。</param>
    /// <param name="value2">第二个点。</param>
    /// <param name="value3">第三个点。</param>
    /// <param name="amount1">权重1。</param>
    /// <param name="amount2">权重2。</param>
    /// <param name="result">重心插值结果。</param>
    public static void Barycentric(ref FPVector2 value1, ref FPVector2 value2, ref FPVector2 value3, FP amount1,
        FP amount2, out FPVector2 result)
    {
        result = new FPVector2(
            FPMath.Barycentric(value1.x, value2.x, value3.x, amount1, amount2),
            FPMath.Barycentric(value1.y, value2.y, value3.y, amount1, amount2));
    }

    /// <summary>
    /// 计算四个点的 Catmull-Rom 插值。
    /// </summary>
    /// <param name="value1">第一个点。</param>
    /// <param name="value2">第二个点。</param>
    /// <param name="value3">第三个点。</param>
    /// <param name="value4">第四个点。</param>
    /// <param name="amount">插值参数。</param>
    /// <returns>Catmull-Rom 插值结果。</returns>
    public static FPVector2 CatmullRom(FPVector2 value1, FPVector2 value2, FPVector2 value3, FPVector2 value4, FP amount)
    {
        return new FPVector2(
            FPMath.CatmullRom(value1.x, value2.x, value3.x, value4.x, amount),
            FPMath.CatmullRom(value1.y, value2.y, value3.y, value4.y, amount));
    }

    /// <summary>
    /// 计算四个点的 Catmull-Rom 插值。
    /// </summary>
    /// <param name="value1">第一个点。</param>
    /// <param name="value2">第二个点。</param>
    /// <param name="value3">第三个点。</param>
    /// <param name="value4">第四个点。</param>
    /// <param name="amount">插值参数。</param>
    /// <param name="result">Catmull-Rom 插值结果。</param>
    public static void CatmullRom(ref FPVector2 value1, ref FPVector2 value2, ref FPVector2 value3, ref FPVector2 value4,
        FP amount, out FPVector2 result)
    {
        result = new FPVector2(
            FPMath.CatmullRom(value1.x, value2.x, value3.x, value4.x, amount),
            FPMath.CatmullRom(value1.y, value2.y, value3.y, value4.y, amount));
    }

    /// <summary>
    /// 将向量限制在指定的最小值和最大值之间。
    /// </summary>
    /// <param name="value1">要限制的向量。</param>
    /// <param name="min">最小值。</param>
    /// <param name="max">最大值。</param>
    /// <returns>限制后的向量。</returns>
    public static FPVector2 Clamp(FPVector2 value1, FPVector2 min, FPVector2 max)
    {
        return new FPVector2(
            FPMath.Clamp(value1.x, min.x, max.x),
            FPMath.Clamp(value1.y, min.y, max.y));
    }

    /// <summary>
    /// 将向量限制在指定的最小值和最大值之间。
    /// </summary>
    /// <param name="value1">要限制的向量。</param>
    /// <param name="min">最小值。</param>
    /// <param name="max">最大值。</param>
    /// <param name="result">限制后的向量。</param>
    public static void Clamp(ref FPVector2 value1, ref FPVector2 min, ref FPVector2 max, out FPVector2 result)
    {
        result = new FPVector2(
            FPMath.Clamp(value1.x, min.x, max.x),
            FPMath.Clamp(value1.y, min.y, max.y));
    }

    /// <summary>
    /// 计算两个向量之间的距离。
    /// </summary>
    /// <param name="value1">第一个向量。</param>
    /// <param name="value2">第二个向量。</param>
    /// <returns>两个向量之间的距离。</returns>
    public static FP Distance(FPVector2 value1, FPVector2 value2)
    {
        FP result;
        DistanceSquared(ref value1, ref value2, out result);
        return FP.Sqrt(result);
    }

    /// <summary>
    /// 计算两个向量之间的距离。
    /// </summary>
    /// <param name="value1">第一个向量。</param>
    /// <param name="value2">第二个向量。</param>
    /// <param name="result">两个向量之间的距离。</param>
    public static void Distance(ref FPVector2 value1, ref FPVector2 value2, out FP result)
    {
        DistanceSquared(ref value1, ref value2, out result);
        result = FP.Sqrt(result);
    }

    /// <summary>
    /// 计算两个向量之间的平方距离。
    /// </summary>
    /// <param name="value1">第一个向量。</param>
    /// <param name="value2">第二个向量。</param>
    /// <returns>两个向量之间的平方距离。</returns>
    public static FP DistanceSquared(FPVector2 value1, FPVector2 value2)
    {
        DistanceSquared(ref value1, ref value2, out var result);
        return result;
    }

    /// <summary>
    /// 计算两个向量之间的平方距离。
    /// </summary>
    /// <param name="value1">第一个向量。</param>
    /// <param name="value2">第二个向量。</param>
    /// <param name="result">两个向量之间的平方距离。</param>
    public static void DistanceSquared(ref FPVector2 value1, ref FPVector2 value2, out FP result)
    {
        result = (value1.x - value2.x) * (value1.x - value2.x) + (value1.y - value2.y) * (value1.y - value2.y);
    }

    /// <summary>
    /// 计算两个向量的除法。
    /// </summary>
    /// <param name="value1">被除数向量。</param>
    /// <param name="value2">除数向量。</param>
    /// <returns>两个向量的除法结果。</returns>
    public static FPVector2 Divide(FPVector2 value1, FPVector2 value2)
    {
        value1.x /= value2.x;
        value1.y /= value2.y;
        return value1;
    }

    /// <summary>
    /// 计算两个向量的除法。
    /// </summary>
    /// <param name="value1">被除数向量。</param>
    /// <param name="value2">除数向量。</param>
    /// <param name="result">两个向量的除法结果。</param>
    public static void Divide(ref FPVector2 value1, ref FPVector2 value2, out FPVector2 result)
    {
        result.x = value1.x / value2.x;
        result.y = value1.y / value2.y;
    }

    /// <summary>
    /// 计算向量与标量的除法。
    /// </summary>
    /// <param name="value1">被除数向量。</param>
    /// <param name="divider">除数标量。</param>
    /// <returns>向量与标量的除法结果。</returns>
    public static FPVector2 Divide(FPVector2 value1, FP divider)
    {
        var factor = 1 / divider;
        value1.x *= factor;
        value1.y *= factor;
        return value1;
    }

    /// <summary>
    /// 计算向量与标量的除法。
    /// </summary>
    /// <param name="value1">被除数向量。</param>
    /// <param name="divider">除数标量。</param>
    /// <param name="result">向量与标量的除法结果。</param>
    public static void Divide(ref FPVector2 value1, FP divider, out FPVector2 result)
    {
        var factor = 1 / divider;
        result.x = value1.x * factor;
        result.y = value1.y * factor;
    }

    /// <summary>
    /// 计算两个向量的点积。
    /// </summary>
    /// <param name="value1">第一个向量。</param>
    /// <param name="value2">第二个向量。</param>
    /// <returns>两个向量的点积。</returns>
    public static FP Dot(FPVector2 value1, FPVector2 value2)
    {
        return value1.x * value2.x + value1.y * value2.y;
    }

    /// <summary>
    /// 计算两个向量的点积。
    /// </summary>
    /// <param name="value1">第一个向量。</param>
    /// <param name="value2">第二个向量。</param>
    /// <param name="result">两个向量的点积。</param>
    public static void Dot(ref FPVector2 value1, ref FPVector2 value2, out FP result)
    {
        result = value1.x * value2.x + value1.y * value2.y;
    }

    /// <summary>
    /// 检查对象是否等于当前 FPVector2 实例。
    /// </summary>
    /// <param name="obj">要比较的对象。</param>
    /// <returns>如果对象等于当前实例，则返回 true；否则返回 false。</returns>
    public override bool Equals(object obj)
    {
        return obj is FPVector2 ? this == (FPVector2)obj : false;
    }

    /// <summary>
    /// 检查另一个 FPVector2 是否等于当前实例。
    /// </summary>
    /// <param name="other">要比较的 FPVector2。</param>
    /// <returns>如果另一个 FPVector2 等于当前实例，则返回 true；否则返回 false。</returns>
    public bool Equals(FPVector2 other)
    {
        return this == other;
    }

    /// <summary>
    /// 获取当前 FPVector2 实例的哈希代码。
    /// </summary>
    /// <returns>当前 FPVector2 实例的哈希代码。</returns>
    public override int GetHashCode()
    {
        return (int)(x + y);
    }

    /// <summary>
    /// 计算四个点的 Hermite 插值。
    /// </summary>
    /// <param name="value1">第一个点。</param>
    /// <param name="tangent1">第一个点的切线。</param>
    /// <param name="value2">第二个点。</param>
    /// <param name="tangent2">第二个点的切线。</param>
    /// <param name="amount">插值参数。</param>
    /// <returns>Hermite 插值结果。</returns>
    public static FPVector2 Hermite(FPVector2 value1, FPVector2 tangent1, FPVector2 value2, FPVector2 tangent2, FP amount)
    {
        Hermite(ref value1, ref tangent1, ref value2, ref tangent2, amount, out var result);
        return result;
    }

    /// <summary>
    /// 计算四个点的 Hermite 插值。
    /// </summary>
    /// <param name="value1">第一个点。</param>
    /// <param name="tangent1">第一个点的切线。</param>
    /// <param name="value2">第二个点。</param>
    /// <param name="tangent2">第二个点的切线。</param>
    /// <param name="amount">插值参数。</param>
    /// <param name="result">Hermite 插值结果。</param>
    public static void Hermite(ref FPVector2 value1, ref FPVector2 tangent1, ref FPVector2 value2, ref FPVector2 tangent2,
        FP amount, out FPVector2 result)
    {
        result.x = FPMath.Hermite(value1.x, tangent1.x, value2.x, tangent2.x, amount);
        result.y = FPMath.Hermite(value1.y, tangent1.y, value2.y, tangent2.y, amount);
    }

    /// <summary>
    /// 获取向量的长度（模）。
    /// </summary>
    public FP Magnitude
    {
        get
        {
            DistanceSquared(ref this, ref zeroVector, out var result);
            return FP.Sqrt(result);
        }
    }

    /// <summary>
    /// 将向量的长度限制在指定的最大值内。
    /// </summary>
    /// <param name="vector">要限制的向量。</param>
    /// <param name="maxLength">最大长度。</param>
    /// <returns>限制后的向量。</returns>
    public static FPVector2 ClampMagnitude(FPVector2 vector, FP maxLength)
    {
        return Normalize(vector) * maxLength;
    }

    /// <summary>
    /// 获取向量的平方长度。
    /// </summary>
    /// <returns>向量的平方长度。</returns>
    public FP LengthSquared()
    {
        DistanceSquared(ref this, ref zeroVector, out var result);
        return result;
    }

    /// <summary>
    /// 在两个向量之间进行线性插值，插值参数在 0 到 1 之间。
    /// </summary>
    /// <param name="value1">起始向量。</param>
    /// <param name="value2">结束向量。</param>
    /// <param name="amount">插值参数。</param>
    /// <returns>插值结果。</returns>
    public static FPVector2 Lerp(FPVector2 value1, FPVector2 value2, FP amount)
    {
        amount = FPMath.Clamp(amount, 0, 1);

        return new FPVector2(
            FPMath.Lerp(value1.x, value2.x, amount),
            FPMath.Lerp(value1.y, value2.y, amount));
    }

    /// <summary>
    /// 在两个向量之间进行线性插值，插值参数不受限制。
    /// </summary>
    /// <param name="value1">起始向量。</param>
    /// <param name="value2">结束向量。</param>
    /// <param name="amount">插值参数。</param>
    /// <returns>插值结果。</returns>
    public static FPVector2 LerpUnclamped(FPVector2 value1, FPVector2 value2, FP amount)
    {
        return new FPVector2(
            FPMath.Lerp(value1.x, value2.x, amount),
            FPMath.Lerp(value1.y, value2.y, amount));
    }

    /// <summary>
    /// 在两个向量之间进行线性插值，插值参数不受限制。
    /// </summary>
    /// <param name="value1">起始向量。</param>
    /// <param name="value2">结束向量。</param>
    /// <param name="amount">插值参数。</param>
    /// <param name="result">插值结果。</param>
    public static void LerpUnclamped(ref FPVector2 value1, ref FPVector2 value2, FP amount, out FPVector2 result)
    {
        result = new FPVector2(
            FPMath.Lerp(value1.x, value2.x, amount),
            FPMath.Lerp(value1.y, value2.y, amount));
    }

    /// <summary>
    /// 返回两个向量中的较大值。
    /// </summary>
    /// <param name="value1">第一个向量。</param>
    /// <param name="value2">第二个向量。</param>
    /// <returns>较大的向量。</returns>
    public static FPVector2 Max(FPVector2 value1, FPVector2 value2)
    {
        return new FPVector2(
            FPMath.Max(value1.x, value2.x),
            FPMath.Max(value1.y, value2.y));
    }

    /// <summary>
    /// 返回两个向量中的较大值。
    /// </summary>
    /// <param name="value1">第一个向量。</param>
    /// <param name="value2">第二个向量。</param>
    /// <param name="result">较大的向量。</param>
    public static void Max(ref FPVector2 value1, ref FPVector2 value2, out FPVector2 result)
    {
        result.x = FPMath.Max(value1.x, value2.x);
        result.y = FPMath.Max(value1.y, value2.y);
    }

    /// <summary>
    /// 返回两个向量中的较小值。
    /// </summary>
    /// <param name="value1">第一个向量。</param>
    /// <param name="value2">第二个向量。</param>
    /// <returns>较小的向量。</returns>
    public static FPVector2 Min(FPVector2 value1, FPVector2 value2)
    {
        return new FPVector2(
            FPMath.Min(value1.x, value2.x),
            FPMath.Min(value1.y, value2.y));
    }

    /// <summary>
    /// 返回两个向量中的较小值。
    /// </summary>
    /// <param name="value1">第一个向量。</param>
    /// <param name="value2">第二个向量。</param>
    /// <param name="result">较小的向量。</param>
    public static void Min(ref FPVector2 value1, ref FPVector2 value2, out FPVector2 result)
    {
        result.x = FPMath.Min(value1.x, value2.x);
        result.y = FPMath.Min(value1.y, value2.y);
    }

    /// <summary>
    /// 按元素缩放向量。
    /// </summary>
    /// <param name="other">缩放因子向量。</param>
    public void Scale(FPVector2 other)
    {
        x = x * other.x;
        y = y * other.y;
    }

    /// <summary>
    /// 按元素缩放两个向量。
    /// </summary>
    /// <param name="value1">第一个向量。</param>
    /// <param name="value2">第二个向量。</param>
    /// <returns>缩放后的向量。</returns>
    public static FPVector2 Scale(FPVector2 value1, FPVector2 value2)
    {
        FPVector2 result;
        result.x = value1.x * value2.x;
        result.y = value1.y * value2.y;

        return result;
    }

    /// <summary>
    /// 按元素乘法操作两个向量。
    /// </summary>
    /// <param name="value1">第一个向量。</param>
    /// <param name="value2">第二个向量。</param>
    /// <returns>乘法结果。</returns>
    public static FPVector2 Multiply(FPVector2 value1, FPVector2 value2)
    {
        value1.x *= value2.x;
        value1.y *= value2.y;
        return value1;
    }

    /// <summary>
    /// 将向量与标量相乘。
    /// </summary>
    /// <param name="value1">向量。</param>
    /// <param name="scaleFactor">标量。</param>
    /// <returns>乘法结果。</returns>
    public static FPVector2 Multiply(FPVector2 value1, FP scaleFactor)
    {
        value1.x *= scaleFactor;
        value1.y *= scaleFactor;
        return value1;
    }

    /// <summary>
    /// 将向量与标量相乘，并将结果存储在输出参数中。
    /// </summary>
    /// <param name="value1">向量。</param>
    /// <param name="scaleFactor">标量。</param>
    /// <param name="result">乘法结果。</param>
    public static void Multiply(ref FPVector2 value1, FP scaleFactor, out FPVector2 result)
    {
        result.x = value1.x * scaleFactor;
        result.y = value1.y * scaleFactor;
    }

    /// <summary>
    /// 按元素乘法操作两个向量，并将结果存储在输出参数中。
    /// </summary>
    /// <param name="value1">第一个向量。</param>
    /// <param name="value2">第二个向量。</param>
    /// <param name="result">乘法结果。</param>
    public static void Multiply(ref FPVector2 value1, ref FPVector2 value2, out FPVector2 result)
    {
        result.x = value1.x * value2.x;
        result.y = value1.y * value2.y;
    }

    /// <summary>
    /// 取向量的负值。
    /// </summary>
    /// <param name="value">输入向量。</param>
    /// <returns>负值向量。</returns>
    public static FPVector2 Negate(FPVector2 value)
    {
        value.x = -value.x;
        value.y = -value.y;
        return value;
    }

    /// <summary>
    /// 取向量的负值，并将结果存储在输出参数中。
    /// </summary>
    /// <param name="value">输入向量。</param>
    /// <param name="result">负值向量。</param>
    public static void Negate(ref FPVector2 value, out FPVector2 result)
    {
        result.x = -value.x;
        result.y = -value.y;
    }

    /// <summary>
    /// 归一化当前向量。
    /// </summary>
    public void Normalize()
    {
        Normalize(ref this, out this);
    }

    /// <summary>
    /// 归一化指定的向量。
    /// </summary>
    /// <param name="value">输入向量。</param>
    /// <returns>归一化后的向量。</returns>
    public static FPVector2 Normalize(FPVector2 value)
    {
        Normalize(ref value, out value);
        return value;
    }

    /// <summary>
    /// 获取归一化后的向量。
    /// </summary>
    public FPVector2 Normalized
    {
        get
        {
            Normalize(ref this, out var result);

            return result;
        }
    }

    /// <summary>
    /// 归一化指定的向量，并将结果存储在输出参数中。
    /// </summary>
    /// <param name="value">输入向量。</param>
    /// <param name="result">归一化后的向量。</param>
    public static void Normalize(ref FPVector2 value, out FPVector2 result)
    {
        DistanceSquared(ref value, ref zeroVector, out var factor);
        factor = FP.One / FP.Sqrt(factor);
        result.x = value.x * factor;
        result.y = value.y * factor;
    }

    /// <summary>
    /// 计算两个向量之间的平滑插值。
    /// </summary>
    /// <param name="value1">起始向量。</param>
    /// <param name="value2">结束向量。</param>
    /// <param name="amount">插值因子，范围在 [0, 1] 之间。</param>
    /// <returns>平滑插值后的向量。</returns>
    public static FPVector2 SmoothStep(FPVector2 value1, FPVector2 value2, FP amount)
    {
        return new FPVector2(
            FPMath.SmoothStep(value1.x, value2.x, amount),
            FPMath.SmoothStep(value1.y, value2.y, amount));
    }

    /// <summary>
    /// 计算两个向量之间的平滑插值，并将结果存储在输出参数中。
    /// </summary>
    /// <param name="value1">起始向量。</param>
    /// <param name="value2">结束向量。</param>
    /// <param name="amount">插值因子，范围在 [0, 1] 之间。</param>
    /// <param name="result">平滑插值后的向量。</param>
    public static void SmoothStep(ref FPVector2 value1, ref FPVector2 value2, FP amount, out FPVector2 result)
    {
        result = new FPVector2(
            FPMath.SmoothStep(value1.x, value2.x, amount),
            FPMath.SmoothStep(value1.y, value2.y, amount));
    }

    /// <summary>
    /// 计算两个向量的差。
    /// </summary>
    /// <param name="value1">第一个向量。</param>
    /// <param name="value2">第二个向量。</param>
    /// <returns>两个向量的差。</returns>
    public static FPVector2 Subtract(FPVector2 value1, FPVector2 value2)
    {
        value1.x -= value2.x;
        value1.y -= value2.y;
        return value1;
    }

    /// <summary>
    /// 计算两个向量的差，并将结果存储在输出参数中。
    /// </summary>
    /// <param name="value1">第一个向量。</param>
    /// <param name="value2">第二个向量。</param>
    /// <param name="result">两个向量的差。</param>
    public static void Subtract(ref FPVector2 value1, ref FPVector2 value2, out FPVector2 result)
    {
        result.x = value1.x - value2.x;
        result.y = value1.y - value2.y;
    }

    /// <summary>
    /// 计算两个向量之间的夹角（以度为单位）。
    /// </summary>
    /// <param name="a">第一个向量。</param>
    /// <param name="b">第二个向量。</param>
    /// <returns>两个向量之间的夹角（以度为单位）。</returns>
    public static FP Angle(FPVector2 a, FPVector2 b)
    {
        return FP.Acos(a.Normalized * b.Normalized) * FP.Rad2Deg;
    }

    /// <summary>
    /// 旋转向量。
    /// </summary>
    /// <param name="a">输入向量。</param>
    /// <param name="deg">旋转角度（以度为单位）。</param>
    /// <returns>旋转后的向量。</returns>
    public static FPVector2 Rotate(FPVector2 a, FP deg)
    {
        var rad = FPMath.Deg2Rad * deg;
        var cosa = FPMath.Cos(rad);
        var sina = FPMath.Sin(rad);

        return new FPVector2(
            a.x * cosa + a.y * sina,
            a.y * cosa - a.x * sina
        );
    }

    /// <summary>
    /// 将二维向量转换为三维向量，Z 坐标默认为 0。
    /// </summary>
    /// <returns>转换后的三维向量。</returns>
    public FPVector3 ToFPVector3()
    {
        return new FPVector3(x, y, 0);
    }

    /// <summary>
    /// 返回向量的字符串表示形式。
    /// </summary>
    /// <returns>向量的字符串表示形式。</returns>
    public override string ToString()
    {
        return $"({x.AsFloat():f5}, {y.AsFloat():f5})";
    }

    #endregion Public Methods

    #region Operators

    /// <summary>
    /// 取反运算符，返回一个新的 FPVector2，其 x 和 y 分量为原向量的相反数。
    /// </summary>
    /// <param name="value">要取反的 FPVector2。</param>
    /// <returns>取反后的 FPVector2。</returns>
    public static FPVector2 operator -(FPVector2 value)
    {
        value.x = -value.x;
        value.y = -value.y;
        return value;
    }

    /// <summary>
    /// 判断两个 FPVector2 是否相等。
    /// </summary>
    /// <param name="value1">第一个 FPVector2。</param>
    /// <param name="value2">第二个 FPVector2。</param>
    /// <returns>如果两个 FPVector2 相等，则返回 true；否则返回 false。</returns>
    public static bool operator ==(FPVector2 value1, FPVector2 value2)
    {
        return value1.x == value2.x && value1.y == value2.y;
    }

    /// <summary>
    /// 判断两个 FPVector2 是否不相等。
    /// </summary>
    /// <param name="value1">第一个 FPVector2。</param>
    /// <param name="value2">第二个 FPVector2。</param>
    /// <returns>如果两个 FPVector2 不相等，则返回 true；否则返回 false。</returns>
    public static bool operator !=(FPVector2 value1, FPVector2 value2)
    {
        return value1.x != value2.x || value1.y != value2.y;
    }

    /// <summary>
    /// 向量加法运算符，返回两个 FPVector2 的和。
    /// </summary>
    /// <param name="value1">第一个 FPVector2。</param>
    /// <param name="value2">第二个 FPVector2。</param>
    /// <returns>两个 FPVector2 的和。</returns>
    public static FPVector2 operator +(FPVector2 value1, FPVector2 value2)
    {
        value1.x += value2.x;
        value1.y += value2.y;
        return value1;
    }

    /// <summary>
    /// 向量减法运算符，返回两个 FPVector2 的差。
    /// </summary>
    /// <param name="value1">被减数 FPVector2。</param>
    /// <param name="value2">减数 FPVector2。</param>
    /// <returns>两个 FPVector2 的差。</returns>
    public static FPVector2 operator -(FPVector2 value1, FPVector2 value2)
    {
        value1.x -= value2.x;
        value1.y -= value2.y;
        return value1;
    }

    /// <summary>
    /// 计算两个 FPVector2 的点积。
    /// </summary>
    /// <param name="value1">第一个 FPVector2。</param>
    /// <param name="value2">第二个 FPVector2。</param>
    /// <returns>两个 FPVector2 的点积。</returns>
    public static FP operator *(FPVector2 value1, FPVector2 value2)
    {
        return Dot(value1, value2);
    }

    /// <summary>
    /// 向量与标量的乘法运算符，返回一个新的 FPVector2，其 x 和 y 分量分别为原向量与标量的乘积。
    /// </summary>
    /// <param name="value">FPVector2。</param>
    /// <param name="scaleFactor">标量。</param>
    /// <returns>新的 FPVector2。</returns>
    public static FPVector2 operator *(FPVector2 value, FP scaleFactor)
    {
        value.x *= scaleFactor;
        value.y *= scaleFactor;
        return value;
    }

    /// <summary>
    /// 标量与向量的乘法运算符，返回一个新的 FPVector2，其 x 和 y 分量分别为标量与原向量的乘积。
    /// </summary>
    /// <param name="scaleFactor">标量。</param>
    /// <param name="value">FPVector2。</param>
    /// <returns>新的 FPVector2。</returns>
    public static FPVector2 operator *(FP scaleFactor, FPVector2 value)
    {
        value.x *= scaleFactor;
        value.y *= scaleFactor;
        return value;
    }

    /// <summary>
    /// 向量除法运算符，返回一个新的 FPVector2，其 x 和 y 分量分别为原向量与另一个向量对应分量的商。
    /// </summary>
    /// <param name="value1">被除数 FPVector2。</param>
    /// <param name="value2">除数 FPVector2。</param>
    /// <returns>新的 FPVector2。</returns>
    public static FPVector2 operator /(FPVector2 value1, FPVector2 value2)
    {
        value1.x /= value2.x;
        value1.y /= value2.y;
        return value1;
    }

    /// <summary>
    /// 向量与标量的除法运算符，返回一个新的 FPVector2，其 x 和 y 分量分别为原向量与标量的商。
    /// </summary>
    /// <param name="value1">被除数 FPVector2。</param>
    /// <param name="divider">标量。</param>
    /// <returns>新的 FPVector2。</returns>
    public static FPVector2 operator /(FPVector2 value1, FP divider)
    {
        var factor = 1 / divider;
        value1.x *= factor;
        value1.y *= factor;
        return value1;
    }

    #endregion Operators
}