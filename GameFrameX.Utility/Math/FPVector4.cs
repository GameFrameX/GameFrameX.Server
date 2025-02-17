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
/// 表示一个四维向量结构。
/// </summary>
[Serializable]
public struct FPVector4
{
    private static FP ZeroEpsilonSq = FPMath.Epsilon;
    internal static FPVector4 InternalZero;

    /// <summary>
    /// 向量的 X 组件。
    /// </summary>
    public FP x;

    /// <summary>
    /// 向量的 Y 组件。
    /// </summary>
    public FP y;

    /// <summary>
    /// 向量的 Z 组件。
    /// </summary>
    public FP z;

    /// <summary>
    /// 向量的 W 组件。
    /// </summary>
    public FP w;

    #region 静态只读变量

    /// <summary>
    /// 组件为 (0,0,0,0) 的向量。
    /// </summary>
    public static readonly FPVector4 zero;

    /// <summary>
    /// 组件为 (1,1,1,1) 的向量。
    /// </summary>
    public static readonly FPVector4 one;

    /// <summary>
    /// 组件为 (FP.MinValue,FP.MinValue,FP.MinValue) 的向量。
    /// </summary>
    public static readonly FPVector4 MinValue;

    /// <summary>
    /// 组件为 (FP.MaxValue,FP.MaxValue,FP.MaxValue) 的向量。
    /// </summary>
    public static readonly FPVector4 MaxValue;

    #endregion

    #region 私有静态构造函数

    static FPVector4()
    {
        one = new FPVector4(1, 1, 1, 1);
        zero = new FPVector4(0, 0, 0, 0);
        MinValue = new FPVector4(FP.MinValue);
        MaxValue = new FPVector4(FP.MaxValue);
        InternalZero = zero;
    }

    #endregion

    /// <summary>
    /// 返回向量的绝对值。
    /// </summary>
    /// <param name="other">要计算绝对值的向量。</param>
    /// <returns>返回绝对值向量。</returns>
    public static FPVector4 Abs(FPVector4 other)
    {
        return new FPVector4(FP.Abs(other.x), FP.Abs(other.y), FP.Abs(other.z), FP.Abs(other.w));
    }

    /// <summary>
    /// 获取向量的平方长度。
    /// </summary>
    /// <returns>返回向量的平方长度。</returns>
    public FP sqrMagnitude
    {
        get { return x * x + y * y + z * z + w * w; }
    }

    /// <summary>
    /// 获取向量的长度。
    /// </summary>
    /// <returns>返回向量的长度。</returns>
    public FP magnitude
    {
        get
        {
            var num = sqrMagnitude;
            return FP.Sqrt(num);
        }
    }

    /// <summary>
    /// 限制向量的长度。
    /// </summary>
    /// <param name="vector">要限制的向量。</param>
    /// <param name="maxLength">最大长度。</param>
    /// <returns>返回限制后的向量。</returns>
    public static FPVector4 ClampMagnitude(FPVector4 vector, FP maxLength)
    {
        return Normalize(vector) * maxLength;
    }

    /// <summary>
    /// 获取向量的归一化版本。
    /// </summary>
    /// <returns>返回归一化后的向量。</returns>
    public FPVector4 normalized
    {
        get
        {
            var result = new FPVector4(x, y, z, w);
            result.Normalize();

            return result;
        }
    }

    /// <summary>
    /// 构造函数，初始化结构的新实例。
    /// </summary>
    /// <param name="x">向量的 X 组件。</param>
    /// <param name="y">向量的 Y 组件。</param>
    /// <param name="z">向量的 Z 组件。</param>
    /// <param name="w">向量的 W 组件。</param>
    public FPVector4(int x, int y, int z, int w)
    {
        this.x = x;
        this.y = y;
        this.z = z;
        this.w = w;
    }

    /// <summary>
    /// 构造函数，初始化结构的新实例。
    /// </summary>
    /// <param name="x">向量的 X 组件。</param>
    /// <param name="y">向量的 Y 组件。</param>
    /// <param name="z">向量的 Z 组件。</param>
    /// <param name="w">向量的 W 组件。</param>
    public FPVector4(FP x, FP y, FP z, FP w)
    {
        this.x = x;
        this.y = y;
        this.z = z;
        this.w = w;
    }

    /// <summary>
    /// 将向量的每个组件与提供的向量的相应组件相乘。
    /// </summary>
    /// <param name="other">要与之相乘的向量。</param>
    public void Scale(FPVector4 other)
    {
        x = x * other.x;
        y = y * other.y;
        z = z * other.z;
        w = w * other.w;
    }

    /// <summary>
    /// 设置向量的所有组件为特定值。
    /// </summary>
    /// <param name="x">向量的 X 组件。</param>
    /// <param name="y">向量的 Y 组件。</param>
    /// <param name="z">向量的 Z 组件。</param>
    /// <param name="w">向量的 W 组件。</param>
    public void Set(FP x, FP y, FP z, FP w)
    {
        this.x = x;
        this.y = y;
        this.z = z;
        this.w = w;
    }

    /// <summary>
    /// 构造函数，初始化结构的新实例。
    /// </summary>
    /// <param name="xyzw">向量的所有组件都设置为 xyzw。</param>
    public FPVector4(FP xyzw)
    {
        x = xyzw;
        y = xyzw;
        z = xyzw;
        w = xyzw;
    }

    /// <summary>
    /// 在两个向量之间进行线性插值。
    /// </summary>
    /// <param name="from">起始向量。</param>
    /// <param name="to">目标向量。</param>
    /// <param name="percent">插值参数。</param>
    /// <returns>返回插值结果。</returns>
    public static FPVector4 Lerp(FPVector4 from, FPVector4 to, FP percent)
    {
        return from + (to - from) * percent;
    }

    /// <summary>
    /// 将向量转换为字符串。
    /// </summary>
    /// <returns>包含所有四个组件的字符串。</returns>
    public override string ToString()
    {
        return $"({x.AsFloat():f5}, {y.AsFloat():f5}, {z.AsFloat():f5}, {w.AsFloat():f5})";
    }

    /// <summary>
    /// 测试一个对象是否等于此向量。
    /// </summary>
    /// <param name="obj">要测试的对象。</param>
    /// <returns>如果相等则返回 true，否则返回 false。</returns>
    public override bool Equals(object obj)
    {
        if (!(obj is FPVector4))
        {
            return false;
        }

        var other = (FPVector4)obj;

        return x == other.x && y == other.y && z == other.z && w == other.w;
    }

    /// <summary>
    /// 将向量的每个组件与提供的向量的相应组件相乘。
    /// </summary>
    /// <param name="vecA">第一个向量。</param>
    /// <param name="vecB">第二个向量。</param>
    /// <returns>返回相乘后的向量。</returns>
    public static FPVector4 Scale(FPVector4 vecA, FPVector4 vecB)
    {
        FPVector4 result;
        result.x = vecA.x * vecB.x;
        result.y = vecA.y * vecB.y;
        result.z = vecA.z * vecB.z;
        result.w = vecA.w * vecB.w;

        return result;
    }

    /// <summary>
    /// 反转向量的方向。
    /// </summary>
    /// <param name="value">要反转的向量。</param>
    /// <returns>返回反转后的向量。</returns>
    public static FPVector4 operator -(FPVector4 value)
    {
        value.x = -value.x;
        value.y = -value.y;
        value.z = -value.z;
        value.w = -value.w;

        return value;
    }

    /// <summary>
    /// 测试两个向量是否相等。
    /// </summary>
    /// <param name="value1">第一个向量。</param>
    /// <param name="value2">第二个向量。</param>
    /// <returns>如果两个向量相等则返回 true，否则返回 false。</returns>
    public static bool operator ==(FPVector4 value1, FPVector4 value2)
    {
        return value1.x == value2.x && value1.y == value2.y && value1.z == value2.z && value1.w == value2.w;
    }

    /// <summary>
    /// 测试两个向量是否不相等。
    /// </summary>
    /// <param name="value1">第一个向量。</param>
    /// <param name="value2">第二个向量。</param>
    /// <returns>如果两个向量不相等则返回 true，否则返回 false。</returns>
    public static bool operator !=(FPVector4 value1, FPVector4 value2)
    {
        if (value1.x == value2.x && value1.y == value2.y && value1.z == value2.z)
        {
            return value1.w != value2.w;
        }

        return true;
    }

    /// <summary>
    /// 获取两个向量的最小 x、y、z 和 w 值。
    /// </summary>
    /// <param name="value1">第一个向量。</param>
    /// <param name="value2">第二个向量。</param>
    /// <returns>返回最小值向量。</returns>
    public static FPVector4 Min(FPVector4 value1, FPVector4 value2)
    {
        Min(ref value1, ref value2, out var result);
        return result;
    }

    /// <summary>
    /// 获取两个向量的最小 x、y、z 和 w 值。
    /// </summary>
    /// <param name="value1">第一个向量。</param>
    /// <param name="value2">第二个向量。</param>
    /// <param name="result">返回最小值向量。</param>
    public static void Min(ref FPVector4 value1, ref FPVector4 value2, out FPVector4 result)
    {
        result.x = value1.x < value2.x ? value1.x : value2.x;
        result.y = value1.y < value2.y ? value1.y : value2.y;
        result.z = value1.z < value2.z ? value1.z : value2.z;
        result.w = value1.w < value2.w ? value1.w : value2.w;
    }

    /// <summary>
    /// 获取两个向量的最大 x、y、z 和 w 值。
    /// </summary>
    /// <param name="value1">第一个向量。</param>
    /// <param name="value2">第二个向量。</param>
    /// <returns>返回最大值向量。</returns>
    public static FPVector4 Max(FPVector4 value1, FPVector4 value2)
    {
        Max(ref value1, ref value2, out var result);
        return result;
    }

    /// <summary>
    /// 获取两个向量的最大 x、y、z 和 w 值。
    /// </summary>
    /// <param name="value1">第一个向量。</param>
    /// <param name="value2">第二个向量。</param>
    /// <param name="result">返回最大值向量。</param>
    public static void Max(ref FPVector4 value1, ref FPVector4 value2, out FPVector4 result)
    {
        result.x = value1.x > value2.x ? value1.x : value2.x;
        result.y = value1.y > value2.y ? value1.y : value2.y;
        result.z = value1.z > value2.z ? value1.z : value2.z;
        result.w = value1.w > value2.w ? value1.w : value2.w;
    }

    /// <summary>
    /// 计算两个向量之间的距离。
    /// </summary>
    /// <param name="v1">第一个向量。</param>
    /// <param name="v2">第二个向量。</param>
    /// <returns>返回两个向量之间的距离。</returns>
    public static FP Distance(FPVector4 v1, FPVector4 v2)
    {
        return FP.Sqrt((v1.x - v2.x) * (v1.x - v2.x) + (v1.y - v2.y) * (v1.y - v2.y) + (v1.z - v2.z) * (v1.z - v2.z) + (v1.w - v2.w) * (v1.w - v2.w));
    }

    /// <summary>
    /// 将向量的长度设置为零。
    /// </summary>
    public void MakeZero()
    {
        x = FP.Zero;
        y = FP.Zero;
        z = FP.Zero;
        w = FP.Zero;
    }

    /// <summary>
    /// 检查向量的长度是否为零。
    /// </summary>
    /// <returns>如果向量为零则返回 true，否则返回 false。</returns>
    public bool IsZero()
    {
        return sqrMagnitude == FP.Zero;
    }

    /// <summary>
    /// 检查向量的长度是否接近零。
    /// </summary>
    /// <returns>如果向量接近零则返回 true，否则返回 false。</returns>
    public bool IsNearlyZero()
    {
        return sqrMagnitude < ZeroEpsilonSq;
    }

    /// <summary>
    /// 通过给定的矩阵变换向量。
    /// </summary>
    /// <param name="position">要变换的向量。</param>
    /// <param name="matrix">变换矩阵。</param>
    /// <returns>变换后的向量。</returns>
    public static FPVector4 Transform(FPVector4 position, FPMatrix4x4 matrix)
    {
        Transform(ref position, ref matrix, out var result);
        return result;
    }

    /// <summary>
    /// 通过给定的矩阵变换向量。
    /// </summary>
    /// <param name="position">要变换的三维向量。</param>
    /// <param name="matrix">变换矩阵。</param>
    /// <returns>变换后的四维向量。</returns>
    public static FPVector4 Transform(FPVector3 position, FPMatrix4x4 matrix)
    {
        Transform(ref position, ref matrix, out var result);
        return result;
    }

    /// <summary>
    /// 通过给定的矩阵变换三维向量。
    /// </summary>
    /// <param name="vector3">要变换的三维向量。</param>
    /// <param name="matrix">变换矩阵。</param>
    /// <param name="result">变换后的四维向量。</param>
    public static void Transform(ref FPVector3 vector3, ref FPMatrix4x4 matrix, out FPVector4 result)
    {
        result.x = vector3.x * matrix.M11 + vector3.y * matrix.M12 + vector3.z * matrix.M13 + matrix.M14;
        result.y = vector3.x * matrix.M21 + vector3.y * matrix.M22 + vector3.z * matrix.M23 + matrix.M24;
        result.z = vector3.x * matrix.M31 + vector3.y * matrix.M32 + vector3.z * matrix.M33 + matrix.M34;
        result.w = vector3.x * matrix.M41 + vector3.y * matrix.M42 + vector3.z * matrix.M43 + matrix.M44;
    }

    /// <summary>
    /// 通过给定的矩阵变换四维向量。
    /// </summary>
    /// <param name="vector">要变换的四维向量。</param>
    /// <param name="matrix">变换矩阵。</param>
    /// <param name="result">变换后的四维向量。</param>
    public static void Transform(ref FPVector4 vector, ref FPMatrix4x4 matrix, out FPVector4 result)
    {
        result.x = vector.x * matrix.M11 + vector.y * matrix.M12 + vector.z * matrix.M13 + vector.w * matrix.M14;
        result.y = vector.x * matrix.M21 + vector.y * matrix.M22 + vector.z * matrix.M23 + vector.w * matrix.M24;
        result.z = vector.x * matrix.M31 + vector.y * matrix.M32 + vector.z * matrix.M33 + vector.w * matrix.M34;
        result.w = vector.x * matrix.M41 + vector.y * matrix.M42 + vector.z * matrix.M43 + vector.w * matrix.M44;
    }

    /// <summary>
    /// 计算两个向量的点积。
    /// </summary>
    /// <param name="vector1">第一个向量。</param>
    /// <param name="vector2">第二个向量。</param>
    /// <returns>返回两个向量的点积。</returns>
    public static FP Dot(FPVector4 vector1, FPVector4 vector2)
    {
        return Dot(ref vector1, ref vector2);
    }

    /// <summary>
    /// 计算两个向量的点积。
    /// </summary>
    /// <param name="vector1">第一个向量。</param>
    /// <param name="vector2">第二个向量。</param>
    /// <returns>返回两个向量的点积。</returns>
    public static FP Dot(ref FPVector4 vector1, ref FPVector4 vector2)
    {
        return vector1.x * vector2.x + vector1.y * vector2.y + vector1.z * vector2.z + vector1.w * vector2.w;
    }

    /// <summary>
    /// 将两个向量相加。
    /// </summary>
    /// <param name="value1">第一个向量。</param>
    /// <param name="value2">第二个向量。</param>
    /// <returns>返回两个向量的和。</returns>
    public static FPVector4 Add(FPVector4 value1, FPVector4 value2)
    {
        Add(ref value1, ref value2, out var result);
        return result;
    }

    /// <summary>
    /// 将两个向量相加。
    /// </summary>
    /// <param name="value1">第一个向量。</param>
    /// <param name="value2">第二个向量。</param>
    /// <param name="result">返回两个向量的和。</param>
    public static void Add(ref FPVector4 value1, ref FPVector4 value2, out FPVector4 result)
    {
        result.x = value1.x + value2.x;
        result.y = value1.y + value2.y;
        result.z = value1.z + value2.z;
        result.w = value1.w + value2.w;
    }

    /// <summary>
    /// 将向量除以一个因子。
    /// </summary>
    /// <param name="value1">要除的向量。</param>
    /// <param name="scaleFactor">缩放因子。</param>
    /// <returns>返回缩放后的向量。</returns>
    public static FPVector4 Divide(FPVector4 value1, FP scaleFactor)
    {
        Divide(ref value1, scaleFactor, out var result);
        return result;
    }

    /// <summary>
    /// 将向量除以一个因子。
    /// </summary>
    /// <param name="value1">要除的向量。</param>
    /// <param name="scaleFactor">缩放因子。</param>
    /// <param name="result">返回缩放后的向量。</param>
    public static void Divide(ref FPVector4 value1, FP scaleFactor, out FPVector4 result)
    {
        result.x = value1.x / scaleFactor;
        result.y = value1.y / scaleFactor;
        result.z = value1.z / scaleFactor;
        result.w = value1.w / scaleFactor;
    }

    /// <summary>
    /// 将两个向量相减。
    /// </summary>
    /// <param name="value1">第一个向量。</param>
    /// <param name="value2">第二个向量。</param>
    /// <returns>返回两个向量的差。</returns>
    public static FPVector4 Subtract(FPVector4 value1, FPVector4 value2)
    {
        Subtract(ref value1, ref value2, out var result);
        return result;
    }

    /// <summary>
    /// 将两个向量相减。
    /// </summary>
    /// <param name="value1">第一个向量。</param>
    /// <param name="value2">第二个向量。</param>
    /// <param name="result">返回两个向量的差。</param>
    public static void Subtract(ref FPVector4 value1, ref FPVector4 value2, out FPVector4 result)
    {
        result.x = value1.x - value2.x;
        result.y = value1.y - value2.y;
        result.z = value1.z - value2.z;
        result.w = value1.w - value2.w;
    }

    /// <summary>
    /// 获取向量的哈希码。
    /// </summary>
    /// <returns>返回向量的哈希码。</returns>
    public override int GetHashCode()
    {
        return x.GetHashCode() ^ y.GetHashCode() ^ z.GetHashCode() ^ w.GetHashCode();
    }

    /// <summary>
    /// 反转向量的方向。
    /// </summary>
    public void Negate()
    {
        x = -x;
        y = -y;
        z = -z;
        w = -w;
    }

    /// <summary>
    /// 反转向量的方向。
    /// </summary>
    /// <param name="value">要反转的向量。</param>
    /// <returns>返回反转后的向量。</returns>
    public static FPVector4 Negate(FPVector4 value)
    {
        Negate(ref value, out var result);
        return result;
    }

    /// <summary>
    /// 反转向量的方向。
    /// </summary>
    /// <param name="value">要反转的向量。</param>
    /// <param name="result">返回反转后的向量。</param>
    public static void Negate(ref FPVector4 value, out FPVector4 result)
    {
        result.x = -value.x;
        result.y = -value.y;
        result.z = -value.z;
        result.w = -value.w;
    }

    /// <summary>
    /// 归一化给定的向量。
    /// </summary>
    /// <param name="value">要归一化的向量。</param>
    /// <returns>返回归一化后的向量。</returns>
    public static FPVector4 Normalize(FPVector4 value)
    {
        Normalize(ref value, out var result);
        return result;
    }

    /// <summary>
    /// 归一化此向量。
    /// </summary>
    public void Normalize()
    {
        var num2 = x * x + y * y + z * z + w * w;
        var num = FP.One / FP.Sqrt(num2);
        x *= num;
        y *= num;
        z *= num;
        w *= num;
    }

    /// <summary>
    /// 归一化给定的向量。
    /// </summary>
    /// <param name="value">要归一化的向量。</param>
    /// <param name="result">返回归一化后的向量。</param>
    public static void Normalize(ref FPVector4 value, out FPVector4 result)
    {
        var num2 = value.x * value.x + value.y * value.y + value.z * value.z + value.w * value.w;
        var num = FP.One / FP.Sqrt(num2);
        result.x = value.x * num;
        result.y = value.y * num;
        result.z = value.z * num;
        result.w = value.w * num;
    }

    /// <summary>
    /// 交换两个向量的组件。
    /// </summary>
    /// <param name="vector1">第一个向量。</param>
    /// <param name="vector2">第二个向量。</param>
    public static void Swap(ref FPVector4 vector1, ref FPVector4 vector2)
    {
        var temp = vector1.x;
        vector1.x = vector2.x;
        vector2.x = temp;

        temp = vector1.y;
        vector1.y = vector2.y;
        vector2.y = temp;

        temp = vector1.z;
        vector1.z = vector2.z;
        vector2.z = temp;

        temp = vector1.w;
        vector1.w = vector2.w;
        vector2.w = temp;
    }

    /// <summary>
    /// 将向量与因子相乘。
    /// </summary>
    /// <param name="value1">要乘的向量。</param>
    /// <param name="scaleFactor">缩放因子。</param>
    /// <returns>返回乘法后的向量。</returns>
    public static FPVector4 Multiply(FPVector4 value1, FP scaleFactor)
    {
        Multiply(ref value1, scaleFactor, out var result);
        return result;
    }

    /// <summary>
    /// 将向量与因子相乘。
    /// </summary>
    /// <param name="value1">要乘的向量。</param>
    /// <param name="scaleFactor">缩放因子。</param>
    /// <param name="result">返回乘法后的向量。</param>
    public static void Multiply(ref FPVector4 value1, FP scaleFactor, out FPVector4 result)
    {
        result.x = value1.x * scaleFactor;
        result.y = value1.y * scaleFactor;
        result.z = value1.z * scaleFactor;
        result.w = value1.w * scaleFactor;
    }

    /// <summary>
    /// 计算两个向量的点积。
    /// </summary>
    /// <param name="value1">第一个向量。</param>
    /// <param name="value2">第二个向量。</param>
    /// <returns>返回两个向量的点积。</returns>
    public static FP operator *(FPVector4 value1, FPVector4 value2)
    {
        return Dot(ref value1, ref value2);
    }

    /// <summary>
    /// 将向量与缩放因子相乘。
    /// </summary>
    /// <param name="value1">要缩放的向量。</param>
    /// <param name="value2">缩放因子。</param>
    /// <returns>返回缩放后的向量。</returns>
    public static FPVector4 operator *(FPVector4 value1, FP value2)
    {
        Multiply(ref value1, value2, out var result);
        return result;
    }

    /// <summary>
    /// 将缩放因子与向量相乘。
    /// </summary>
    /// <param name="value1">缩放因子。</param>
    /// <param name="value2">要缩放的向量。</param>
    /// <returns>返回缩放后的向量。</returns>
    public static FPVector4 operator *(FP value1, FPVector4 value2)
    {
        Multiply(ref value2, value1, out var result);
        return result;
    }

    /// <summary>
    /// 将两个向量相减。
    /// </summary>
    /// <param name="value1">第一个向量。</param>
    /// <param name="value2">第二个向量。</param>
    /// <returns>返回两个向量的差。</returns>
    public static FPVector4 operator -(FPVector4 value1, FPVector4 value2)
    {
        Subtract(ref value1, ref value2, out var result);
        return result;
    }

    /// <summary>
    /// 将两个向量相加。
    /// </summary>
    /// <param name="value1">第一个向量。</param>
    /// <param name="value2">第二个向量。</param>
    /// <returns>返回两个向量的和。</returns>
    public static FPVector4 operator +(FPVector4 value1, FPVector4 value2)
    {
        Add(ref value1, ref value2, out var result);
        return result;
    }

    /// <summary>
    /// 将向量除以因子。
    /// </summary>
    /// <param name="value1">要除的向量。</param>
    /// <param name="scaleFactor">缩放因子。</param>
    /// <returns>返回缩放后的向量。</returns>
    public static FPVector4 operator /(FPVector4 value1, FP scaleFactor)
    {
        Divide(ref value1, scaleFactor, out var result);
        return result;
    }

    /// <summary>
    /// 将向量转换为 FPVector2。
    /// </summary>
    /// <returns>返回转换后的 FPVector2。</returns>
    public FPVector2 ToFPVector2()
    {
        return new FPVector2(x, y);
    }

    /// <summary>
    /// 将向量转换为 FPVector3。
    /// </summary>
    /// <returns>返回转换后的 FPVector3。</returns>
    public FPVector3 ToFPVector()
    {
        return new FPVector3(x, y, z);
    }
}