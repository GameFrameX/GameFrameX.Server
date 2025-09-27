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
/// 表示一个三维向量结构。
/// </summary>
[Serializable]
public struct FPVector3
{
    private static FP ZeroEpsilonSq = FPMath.Epsilon;
    internal static FPVector3 InternalZero;
    internal static FPVector3 Arbitrary;

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

    #region 静态只读变量

    /// <summary>
    /// 组件为 (0,0,0) 的向量。
    /// </summary>
    public static readonly FPVector3 zero;

    /// <summary>
    /// 组件为 (-1,0,0) 的向量。
    /// </summary>
    public static readonly FPVector3 left;

    /// <summary>
    /// 组件为 (1,0,0) 的向量。
    /// </summary>
    public static readonly FPVector3 right;

    /// <summary>
    /// 组件为 (0,1,0) 的向量。
    /// </summary>
    public static readonly FPVector3 up;

    /// <summary>
    /// 组件为 (0,-1,0) 的向量。
    /// </summary>
    public static readonly FPVector3 down;

    /// <summary>
    /// 组件为 (0,0,-1) 的向量。
    /// </summary>
    public static readonly FPVector3 back;

    /// <summary>
    /// 组件为 (0,0,1) 的向量。
    /// </summary>
    public static readonly FPVector3 forward;

    /// <summary>
    /// 组件为 (1,1,1) 的向量。
    /// </summary>
    public static readonly FPVector3 one;

    /// <summary>
    /// 组件为 (FP.MinValue,FP.MinValue,FP.MinValue) 的向量。
    /// </summary>
    public static readonly FPVector3 MinValue;

    /// <summary>
    /// 组件为 (FP.MaxValue,FP.MaxValue,FP.MaxValue) 的向量。
    /// </summary>
    public static readonly FPVector3 MaxValue;

    #endregion

    #region 私有静态构造函数

    static FPVector3()
    {
        one = new FPVector3(1, 1, 1);
        zero = new FPVector3(0, 0, 0);
        left = new FPVector3(-1, 0, 0);
        right = new FPVector3(1, 0, 0);
        up = new FPVector3(0, 1, 0);
        down = new FPVector3(0, -1, 0);
        back = new FPVector3(0, 0, -1);
        forward = new FPVector3(0, 0, 1);
        MinValue = new FPVector3(FP.MinValue);
        MaxValue = new FPVector3(FP.MaxValue);
        Arbitrary = new FPVector3(1, 1, 1);
        InternalZero = zero;
    }

    #endregion

    /// <summary>
    /// 返回向量的绝对值。
    /// </summary>
    /// <param name="other">要计算绝对值的向量。</param>
    /// <returns>返回绝对值向量。</returns>
    public static FPVector3 Abs(FPVector3 other)
    {
        return new FPVector3(FP.FastAbs(other.x), FP.FastAbs(other.y), FP.FastAbs(other.z));
    }

    /// <summary>
    /// 获取向量的平方长度。
    /// </summary>
    /// <returns>返回向量的平方长度。</returns>
    public FP sqrMagnitude
    {
        get { return x * x + y * y + z * z; }
    }

    /// <summary>
    /// 获取向量的长度。
    /// </summary>
    /// <returns>返回向量的长度。</returns>
    public FP magnitude
    {
        get
        {
            var num = x * x + y * y + z * z;
            return FP.Sqrt(num);
        }
    }

    /// <summary>
    /// 限制向量的长度。
    /// </summary>
    /// <param name="vector3">要限制的向量。</param>
    /// <param name="maxLength">最大长度。</param>
    /// <returns>返回限制后的向量。</returns>
    public static FPVector3 ClampMagnitude(FPVector3 vector3, FP maxLength)
    {
        return Normalize(vector3) * maxLength;
    }

    /// <summary>
    /// 获取向量的归一化版本。
    /// </summary>
    /// <returns>返回归一化后的向量。</returns>
    public FPVector3 normalized
    {
        get
        {
            var result = new FPVector3(x, y, z);
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
    public FPVector3(int x, int y, int z)
    {
        this.x = x;
        this.y = y;
        this.z = z;
    }

    /// <summary>
    /// 构造函数，初始化结构的新实例。
    /// </summary>
    /// <param name="x">向量的 X 组件。</param>
    /// <param name="y">向量的 Y 组件。</param>
    /// <param name="z">向量的 Z 组件。</param>
    public FPVector3(FP x, FP y, FP z)
    {
        this.x = x;
        this.y = y;
        this.z = z;
    }

    /// <summary>
    /// 将向量的每个组件与提供的向量的相应组件相乘。
    /// </summary>
    /// <param name="other">要与之相乘的向量。</param>
    public void Scale(FPVector3 other)
    {
        x = x * other.x;
        y = y * other.y;
        z = z * other.z;
    }

    /// <summary>
    /// 设置向量的所有组件为特定值。
    /// </summary>
    /// <param name="x">向量的 X 组件。</param>
    /// <param name="y">向量的 Y 组件。</param>
    /// <param name="z">向量的 Z 组件。</param>
    public void Set(FP x, FP y, FP z)
    {
        this.x = x;
        this.y = y;
        this.z = z;
    }

    /// <summary>
    /// 构造函数，初始化结构的新实例。
    /// </summary>
    /// <param name="xyz">向量的所有组件都设置为 xyz。</param>
    public FPVector3(FP xyz)
    {
        x = xyz;
        y = xyz;
        z = xyz;
    }

    /// <summary>
    /// 在两个向量之间进行线性插值。
    /// </summary>
    /// <param name="from">起始向量。</param>
    /// <param name="to">目标向量。</param>
    /// <param name="percent">插值比例。</param>
    /// <returns>返回插值结果。</returns>
    public static FPVector3 Lerp(FPVector3 from, FPVector3 to, FP percent)
    {
        return from + (to - from) * percent;
    }

    /// <summary>
    /// 将向量转换为字符串。
    /// </summary>
    /// <returns>包含所有三个组件的字符串。</returns>
    public override string ToString()
    {
        return $"({x.AsFloat():f5}, {y.AsFloat():f5}, {z.AsFloat():f5})";
    }

    /// <summary>
    /// 测试一个对象是否等于此向量。
    /// </summary>
    /// <param name="obj">要测试的对象。</param>
    /// <returns>如果相等则返回 true，否则返回 false。</returns>
    public override bool Equals(object obj)
    {
        if (!(obj is FPVector3))
        {
            return false;
        }

        var other = (FPVector3)obj;

        return x == other.x && y == other.y && z == other.z;
    }

    /// <summary>
    /// 将向量的每个组件与提供的向量的相应组件相乘。
    /// </summary>
    /// <param name="vecA">第一个向量。</param>
    /// <param name="vecB">第二个向量。</param>
    /// <returns>返回乘积向量。</returns>
    public static FPVector3 Scale(FPVector3 vecA, FPVector3 vecB)
    {
        FPVector3 result;
        result.x = vecA.x * vecB.x;
        result.y = vecA.y * vecB.y;
        result.z = vecA.z * vecB.z;

        return result;
    }

    /// <summary>
    /// 反转向量的方向。
    /// </summary>
    /// <param name="value">要反转的向量。</param>
    /// <returns>返回反转后的向量。</returns>
    public static FPVector3 operator -(FPVector3 value)
    {
        value.x = -value.x;
        value.y = -value.y;
        value.z = -value.z;
        return value;
    }

    /// <summary>
    /// 测试两个向量是否相等。
    /// </summary>
    /// <param name="value1">第一个向量。</param>
    /// <param name="value2">第二个向量。</param>
    /// <returns>如果两个向量相等则返回 true，否则返回 false。</returns>
    public static bool operator ==(FPVector3 value1, FPVector3 value2)
    {
        return value1.x == value2.x && value1.y == value2.y && value1.z == value2.z;
    }

    /// <summary>
    /// 测试两个向量是否不相等。
    /// </summary>
    /// <param name="value1">第一个向量。</param>
    /// <param name="value2">第二个向量。</param>
    /// <returns>如果两个向量不相等则返回 true，否则返回 false。</returns>
    public static bool operator !=(FPVector3 value1, FPVector3 value2)
    {
        if (value1.x == value2.x && value1.y == value2.y)
        {
            return value1.z != value2.z;
        }

        return true;
    }

    /// <summary>
    /// 获取两个向量的最小 x、y 和 z 值。
    /// </summary>
    /// <param name="value1">第一个向量。</param>
    /// <param name="value2">第二个向量。</param>
    /// <returns>返回最小值向量。</returns>
    public static FPVector3 Min(FPVector3 value1, FPVector3 value2)
    {
        Min(ref value1, ref value2, out var result);
        return result;
    }

    /// <summary>
    /// 获取两个向量的最小 x、y 和 z 值。
    /// </summary>
    /// <param name="value1">第一个向量。</param>
    /// <param name="value2">第二个向量。</param>
    /// <param name="result">返回最小值向量。</param>
    public static void Min(ref FPVector3 value1, ref FPVector3 value2, out FPVector3 result)
    {
        result.x = value1.x < value2.x ? value1.x : value2.x;
        result.y = value1.y < value2.y ? value1.y : value2.y;
        result.z = value1.z < value2.z ? value1.z : value2.z;
    }

    /// <summary>
    /// 获取两个向量的最大 x、y 和 z 值。
    /// </summary>
    /// <param name="value1">第一个向量。</param>
    /// <param name="value2">第二个向量。</param>
    /// <returns>返回最大值向量。</returns>
    public static FPVector3 Max(FPVector3 value1, FPVector3 value2)
    {
        Max(ref value1, ref value2, out var result);
        return result;
    }

    /// <summary>
    /// 计算两个三维向量之间的欧几里得距离。
    /// </summary>
    /// <param name="v1">第一个向量。</param>
    /// <param name="v2">第二个向量。</param>
    /// <returns>返回两个向量之间的距离。</returns>
    public static FP Distance(FPVector3 v1, FPVector3 v2)
    {
        return FP.Sqrt((v1.x - v2.x) * (v1.x - v2.x) + (v1.y - v2.y) * (v1.y - v2.y) + (v1.z - v2.z) * (v1.z - v2.z));
    }

    /// <summary>
    /// 获取两个向量的最大 x、y 和 z 值。
    /// </summary>
    /// <param name="value1">第一个向量。</param>
    /// <param name="value2">第二个向量。</param>
    /// <param name="result">返回最大值向量。</param>
    public static void Max(ref FPVector3 value1, ref FPVector3 value2, out FPVector3 result)
    {
        result.x = value1.x > value2.x ? value1.x : value2.x;
        result.y = value1.y > value2.y ? value1.y : value2.y;
        result.z = value1.z > value2.z ? value1.z : value2.z;
    }

    /// <summary>
    /// 设置向量的长度为零。
    /// </summary>
    public void MakeZero()
    {
        x = FP.Zero;
        y = FP.Zero;
        z = FP.Zero;
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
    /// <returns>返回变换后的向量。</returns>
    public static FPVector3 Transform(FPVector3 position, FPMatrix matrix)
    {
        Transform(ref position, ref matrix, out var result);
        return result;
    }

    /// <summary>
    /// 通过给定的矩阵变换向量。
    /// </summary>
    /// <param name="position">要变换的向量。</param>
    /// <param name="matrix">变换矩阵。</param>
    /// <param name="result">返回变换后的向量。</param>
    public static void Transform(ref FPVector3 position, ref FPMatrix matrix, out FPVector3 result)
    {
        var num0 = position.x * matrix.M11 + position.y * matrix.M21 + position.z * matrix.M31;
        var num1 = position.x * matrix.M12 + position.y * matrix.M22 + position.z * matrix.M32;
        var num2 = position.x * matrix.M13 + position.y * matrix.M23 + position.z * matrix.M33;

        result.x = num0;
        result.y = num1;
        result.z = num2;
    }

    /// <summary>
    /// 通过给定矩阵的转置变换向量。
    /// </summary>
    /// <param name="position">要变换的向量。</param>
    /// <param name="matrix">变换矩阵。</param>
    /// <param name="result">返回变换后的向量。</param>
    public static void TransposedTransform(ref FPVector3 position, ref FPMatrix matrix, out FPVector3 result)
    {
        var num0 = position.x * matrix.M11 + position.y * matrix.M12 + position.z * matrix.M13;
        var num1 = position.x * matrix.M21 + position.y * matrix.M22 + position.z * matrix.M23;
        var num2 = position.x * matrix.M31 + position.y * matrix.M32 + position.z * matrix.M33;

        result.x = num0;
        result.y = num1;
        result.z = num2;
    }

    /// <summary>
    /// 计算两个向量的点积。
    /// </summary>
    /// <param name="vector1">第一个向量。</param>
    /// <param name="vector2">第二个向量。</param>
    /// <returns>返回两个向量的点积。</returns>
    public static FP Dot(FPVector3 vector1, FPVector3 vector2)
    {
        return Dot(ref vector1, ref vector2);
    }

    /// <summary>
    /// 计算两个向量的点积。
    /// </summary>
    /// <param name="vector1">第一个向量。</param>
    /// <param name="vector2">第二个向量。</param>
    /// <returns>返回两个向量的点积。</returns>
    public static FP Dot(ref FPVector3 vector1, ref FPVector3 vector2)
    {
        return vector1.x * vector2.x + vector1.y * vector2.y + vector1.z * vector2.z;
    }

    /// <summary>
    /// 将向量投影到另一个向量上。
    /// </summary>
    /// <param name="vector3">要投影的向量。</param>
    /// <param name="onNormal">投影的基准向量。</param>
    /// <returns>返回投影后的向量。</returns>
    public static FPVector3 Project(FPVector3 vector3, FPVector3 onNormal)
    {
        var sqrtMag = Dot(onNormal, onNormal);
        if (sqrtMag < FPMath.Epsilon)
        {
            return zero;
        }

        return onNormal * Dot(vector3, onNormal) / sqrtMag;
    }

    /// <summary>
    /// 将向量投影到由法线定义的平面上。
    /// </summary>
    /// <param name="vector3">要投影的向量。</param>
    /// <param name="planeNormal">平面的法线。</param>
    /// <returns>返回投影后的向量。</returns>
    public static FPVector3 ProjectOnPlane(FPVector3 vector3, FPVector3 planeNormal)
    {
        return vector3 - Project(vector3, planeNormal);
    }

    /// <summary>
    /// 返回两个向量之间的角度（以度为单位）。
    /// </summary>
    /// <param name="from">起始向量。</param>
    /// <param name="to">目标向量。</param>
    /// <returns>返回两个向量之间的角度。</returns>
    public static FP Angle(FPVector3 from, FPVector3 to)
    {
        return FPMath.Acos(FPMath.Clamp(Dot(from.normalized, to.normalized), -FP.One, FP.One)) * FPMath.Rad2Deg;
    }

    /// <summary>
    /// 返回两个向量之间的较小角度（以度为单位）。
    /// </summary>
    /// <param name="from">起始向量。</param>
    /// <param name="to">目标向量。</param>
    /// <param name="axis">旋转轴。</param>
    /// <returns>返回两个向量之间的带符号角度。</returns>
    public static FP SignedAngle(FPVector3 from, FPVector3 to, FPVector3 axis)
    {
        FPVector3 fromNorm = from.normalized, toNorm = to.normalized;
        var unsignedAngle = FPMath.Acos(FPMath.Clamp(Dot(fromNorm, toNorm), -FP.One, FP.One)) * FPMath.Rad2Deg;
        FP sign = FPMath.Sign(Dot(axis, Cross(fromNorm, toNorm)));
        return unsignedAngle * sign;
    }

    /// <summary>
    /// 将两个向量相加。
    /// </summary>
    /// <param name="value1">第一个向量。</param>
    /// <param name="value2">第二个向量。</param>
    /// <returns>返回两个向量的和。</returns>
    public static FPVector3 Add(FPVector3 value1, FPVector3 value2)
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
    public static void Add(ref FPVector3 value1, ref FPVector3 value2, out FPVector3 result)
    {
        var num0 = value1.x + value2.x;
        var num1 = value1.y + value2.y;
        var num2 = value1.z + value2.z;

        result.x = num0;
        result.y = num1;
        result.z = num2;
    }

    /// <summary>
    /// 将向量除以一个因子。
    /// </summary>
    /// <param name="value1">要除的向量。</param>
    /// <param name="scaleFactor">缩放因子。</param>
    /// <returns>返回缩放后的向量。</returns>
    public static FPVector3 Divide(FPVector3 value1, FP scaleFactor)
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
    public static void Divide(ref FPVector3 value1, FP scaleFactor, out FPVector3 result)
    {
        result.x = value1.x / scaleFactor;
        result.y = value1.y / scaleFactor;
        result.z = value1.z / scaleFactor;
    }

    /// <summary>
    /// 将两个向量相减。
    /// </summary>
    /// <param name="value1">第一个向量。</param>
    /// <param name="value2">第二个向量。</param>
    /// <returns>返回两个向量的差。</returns>
    public static FPVector3 Subtract(FPVector3 value1, FPVector3 value2)
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
    public static void Subtract(ref FPVector3 value1, ref FPVector3 value2, out FPVector3 result)
    {
        var num0 = value1.x - value2.x;
        var num1 = value1.y - value2.y;
        var num2 = value1.z - value2.z;

        result.x = num0;
        result.y = num1;
        result.z = num2;
    }

    /// <summary>
    /// 计算两个向量的叉积。
    /// </summary>
    /// <param name="vector1">第一个向量。</param>
    /// <param name="vector2">第二个向量。</param>
    /// <returns>返回两个向量的叉积。</returns>
    public static FPVector3 Cross(FPVector3 vector1, FPVector3 vector2)
    {
        Cross(ref vector1, ref vector2, out var result);
        return result;
    }

    /// <summary>
    /// 计算两个向量的叉积。
    /// </summary>
    /// <param name="vector1">第一个向量。</param>
    /// <param name="vector2">第二个向量。</param>
    /// <param name="result">返回两个向量的叉积。</param>
    public static void Cross(ref FPVector3 vector1, ref FPVector3 vector2, out FPVector3 result)
    {
        var num3 = vector1.y * vector2.z - vector1.z * vector2.y;
        var num2 = vector1.z * vector2.x - vector1.x * vector2.z;
        var num = vector1.x * vector2.y - vector1.y * vector2.x;
        result.x = num3;
        result.y = num2;
        result.z = num;
    }

    /// <summary>
    /// 获取向量的哈希码。
    /// </summary>
    /// <returns>返回向量的哈希码。</returns>
    public override int GetHashCode()
    {
        return x.GetHashCode() ^ y.GetHashCode() ^ z.GetHashCode();
    }

    /// <summary>
    /// 反转向量的方向。
    /// </summary>
    public void Negate()
    {
        x = -x;
        y = -y;
        z = -z;
    }

    /// <summary>
    /// 反转向量的方向。
    /// </summary>
    /// <param name="value">要反转的向量。</param>
    /// <returns>返回反转后的向量。</returns>
    public static FPVector3 Negate(FPVector3 value)
    {
        Negate(ref value, out var result);
        return result;
    }

    /// <summary>
    /// 反转向量的方向。
    /// </summary>
    /// <param name="value">要反转的向量。</param>
    /// <param name="result">返回反转后的向量。</param>
    public static void Negate(ref FPVector3 value, out FPVector3 result)
    {
        var num0 = -value.x;
        var num1 = -value.y;
        var num2 = -value.z;

        result.x = num0;
        result.y = num1;
        result.z = num2;
    }

    /// <summary>
    /// 归一化给定的向量。
    /// </summary>
    /// <param name="value">要归一化的向量。</param>
    /// <returns>返回归一化后的向量。</returns>
    public static FPVector3 Normalize(FPVector3 value)
    {
        Normalize(ref value, out var result);
        return result;
    }

    /// <summary>
    /// 归一化此向量。
    /// </summary>
    public void Normalize()
    {
        var num2 = x * x + y * y + z * z;
        var num = FP.One / FP.Sqrt(num2);
        x *= num;
        y *= num;
        z *= num;
    }

    /// <summary>
    /// 归一化给定的向量。
    /// </summary>
    /// <param name="value">要归一化的向量。</param>
    /// <param name="result">返回归一化后的向量。</param>
    public static void Normalize(ref FPVector3 value, out FPVector3 result)
    {
        var num2 = value.x * value.x + value.y * value.y + value.z * value.z;
        var num = FP.One / FP.Sqrt(num2);
        result.x = value.x * num;
        result.y = value.y * num;
        result.z = value.z * num;
    }

    /// <summary>
    /// 交换两个向量的组件。
    /// </summary>
    /// <param name="vector1">第一个向量。</param>
    /// <param name="vector2">第二个向量。</param>
    public static void Swap(ref FPVector3 vector1, ref FPVector3 vector2)
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
    }

    /// <summary>
    /// 将向量与因子相乘。
    /// </summary>
    /// <param name="value1">要乘的向量。</param>
    /// <param name="scaleFactor">缩放因子。</param>
    /// <returns>返回乘积向量。</returns>
    public static FPVector3 Multiply(FPVector3 value1, FP scaleFactor)
    {
        Multiply(ref value1, scaleFactor, out var result);
        return result;
    }

    /// <summary>
    /// 将向量与因子相乘。
    /// </summary>
    /// <param name="value1">要乘的向量。</param>
    /// <param name="scaleFactor">缩放因子。</param>
    /// <param name="result">返回乘积向量。</param>
    public static void Multiply(ref FPVector3 value1, FP scaleFactor, out FPVector3 result)
    {
        result.x = value1.x * scaleFactor;
        result.y = value1.y * scaleFactor;
        result.z = value1.z * scaleFactor;
    }

    /// <summary>
    /// 计算两个向量的叉积。
    /// </summary>
    /// <param name="value1">第一个向量。</param>
    /// <param name="value2">第二个向量。</param>
    /// <returns>返回两个向量的叉积。</returns>
    public static FPVector3 operator %(FPVector3 value1, FPVector3 value2)
    {
        Cross(ref value1, ref value2, out var result);
        return result;
    }

    /// <summary>
    /// 计算两个向量的点积。
    /// </summary>
    /// <param name="value1">第一个向量。</param>
    /// <param name="value2">第二个向量。</param>
    /// <returns>返回两个向量的点积。</returns>
    public static FP operator *(FPVector3 value1, FPVector3 value2)
    {
        return Dot(ref value1, ref value2);
    }

    /// <summary>
    /// 将向量与缩放因子相乘。
    /// </summary>
    /// <param name="value1">要缩放的向量。</param>
    /// <param name="value2">缩放因子。</param>
    /// <returns>返回缩放后的向量。</returns>
    public static FPVector3 operator *(FPVector3 value1, FP value2)
    {
        Multiply(ref value1, value2, out var result);
        return result;
    }

    /// <summary>
    /// 将向量与缩放因子相乘。
    /// </summary>
    /// <param name="value2">要缩放的向量。</param>
    /// <param name="value1">缩放因子。</param>
    /// <returns>返回缩放后的向量。</returns>
    public static FPVector3 operator *(FP value1, FPVector3 value2)
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
    public static FPVector3 operator -(FPVector3 value1, FPVector3 value2)
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
    public static FPVector3 operator +(FPVector3 value1, FPVector3 value2)
    {
        Add(ref value1, ref value2, out var result);
        return result;
    }

    /// <summary>
    /// 将向量除以一个因子。
    /// </summary>
    /// <param name="value1">要除的向量。</param>
    /// <param name="scaleFactor">缩放因子。</param>
    /// <returns>返回缩放后的向量。</returns>
    public static FPVector3 operator /(FPVector3 value1, FP scaleFactor)
    {
        Divide(ref value1, scaleFactor, out var result);
        return result;
    }

    /// <summary>
    /// 计算两个向量的叉积。
    /// </summary>
    /// <param name="value1">第一个向量。</param>
    /// <param name="value2">第二个向量。</param>
    /// <returns>返回两个向量的叉积。</returns>
    public static FPVector3 operator ^(FPVector3 value1, FPVector3 value2)
    {
        var v = new FPVector3
        {
            x = value1.y * value2.z - value1.z * value2.y,
            y = value1.z * value2.x - value1.x * value2.z,
            z = value1.x * value2.y - value1.y * value2.x,
        };
        return v;
    }

    /// <summary>
    /// 将向量转换为二维向量。
    /// </summary>
    /// <returns>返回转换后的二维向量。</returns>
    public FPVector2 ToFPVector2()
    {
        return new FPVector2(x, y);
    }

    /// <summary>
    /// 将向量转换为四维向量。
    /// </summary>
    /// <returns>返回转换后的四维向量。</returns>
    public FPVector4 ToFPVector4()
    {
        return new FPVector4(x, y, z, FP.One);
    }
}