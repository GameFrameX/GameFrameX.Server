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
/// 表示一个四元数，用于表示三维空间中的方向和旋转。
/// </summary>
[Serializable]
public struct FPQuaternion
{
    /// <summary>四元数的 X 分量。</summary>
    public FP x;

    /// <summary>四元数的 Y 分量。</summary>
    public FP y;

    /// <summary>四元数的 Z 分量。</summary>
    public FP z;

    /// <summary>四元数的 W 分量。</summary>
    public FP w;

    /// <summary>表示单位四元数的静态只读字段。</summary>
    public static readonly FPQuaternion identity;
    /// <summary>静态构造函数，初始化单位四元数。</summary>
    static FPQuaternion()
    {
        identity = new FPQuaternion(0, 0, 0, 1);
    }

    /// <summary>
    /// 初始化一个新的四元数实例。
    /// </summary>
    /// <param name="x">四元数的 X 分量。</param>
    /// <param name="y">四元数的 Y 分量。</param>
    /// <param name="z">四元数的 Z 分量。</param>
    /// <param name="w">四元数的 W 分量。</param>
    public FPQuaternion(FP x, FP y, FP z, FP w)
    {
        this.x = x;
        this.y = y;
        this.z = z;
        this.w = w;
    }

    /// <summary>
    /// 设置四元数的各个分量。
    /// </summary>
    /// <param name="new_x">新的 X 分量。</param>
    /// <param name="new_y">新的 Y 分量。</param>
    /// <param name="new_z">新的 Z 分量。</param>
    /// <param name="new_w">新的 W 分量。</param>
    public void Set(FP new_x, FP new_y, FP new_z, FP new_w)
    {
        x = new_x;
        y = new_y;
        z = new_z;
        w = new_w;
    }

    /// <summary>
    /// 根据从一个方向到另一个方向的旋转设置四元数。
    /// </summary>
    /// <param name="fromDirection">起始方向。</param>
    /// <param name="toDirection">目标方向。</param>
    public void SetFromToRotation(FPVector3 fromDirection, FPVector3 toDirection)
    {
        var targetRotation = FromToRotation(fromDirection, toDirection);
        Set(targetRotation.x, targetRotation.y, targetRotation.z, targetRotation.w);
    }

    /// <summary>
    /// 获取四元数对应的欧拉角。
    /// </summary>
    /// <returns>表示欧拉角的向量。</returns>
    public FPVector3 eulerAngles
    {
        get
        {
            var result = new FPVector3();

            var ysqr = y * y;
            var t0 = -2 * FP.One * (ysqr + z * z) + FP.One;
            var t1 = +2 * FP.One * (x * y - w * z);
            var t2 = -2 * FP.One * (x * z + w * y);
            var t3 = +2 * FP.One * (y * z - w * x);
            var t4 = -2 * FP.One * (x * x + ysqr) + FP.One;

            t2 = t2 > FP.One ? FP.One : t2;
            t2 = t2 < -FP.One ? -FP.One : t2;

            result.x = FP.Atan2(t3, t4) * FP.Rad2Deg;
            result.y = FP.Asin(t2) * FP.Rad2Deg;
            result.z = FP.Atan2(t1, t0) * FP.Rad2Deg;

            return result * -1;
        }
    }

    /// <summary>
    /// 获取四元数对应的欧拉角（另一种实现）。
    /// </summary>
    /// <returns>表示欧拉角的向量。</returns>
    public FPVector3 eulerAnglesNew
    {
        get
        {
            var r11 = 2 * (x * z + w * y);
            var r12 = w * w - x * x - y * y + z * z;
            var r21 = -2 * (y * z - w * x);
            var r31 = 2 * (x * y + w * z);
            var r32 = w * w - x * x + y * y - z * z;
            return new FPVector3(FP.Atan2(r11, r12), FP.Asin(r21), FP.Atan2(r31, r32));
        }
    }

    /// <summary>
    /// 计算两个四元数之间的夹角。
    /// </summary>
    /// <param name="a">第一个四元数。</param>
    /// <param name="b">第二个四元数。</param>
    /// <returns>两个四元数之间的夹角（以度为单位）。</returns>
    public static FP Angle(FPQuaternion a, FPQuaternion b)
    {
        var aInv = Inverse(a);
        var f = b * aInv;

        var angle = FP.Acos(f.w) * 2 * FP.Rad2Deg;

        if (angle > 180)
        {
            angle = 360 - angle;
        }

        return angle;
    }

    #region public static JQuaternion Add(JQuaternion quaternion1, JQuaternion quaternion2)

    /// <summary>
    /// 计算两个四元数的和。
    /// </summary>
    /// <param name="quaternion1">第一个四元数。</param>
    /// <param name="quaternion2">第二个四元数。</param>
    /// <returns>两个四元数的和。</returns>
    public static FPQuaternion Add(FPQuaternion quaternion1, FPQuaternion quaternion2)
    {
        Add(ref quaternion1, ref quaternion2, out var result);
        return result;
    }

    /// <summary>
    /// 根据指定的前方向量创建一个四元数，使用默认的向上向量。
    /// </summary>
    /// <param name="forward">前方向量。</param>
    /// <returns>表示旋转的四元数。</returns>
    public static FPQuaternion LookRotation(FPVector3 forward)
    {
        return CreateFromMatrix(FPMatrix.LookAt(forward, FPVector3.up));
    }

    /// <summary>
    /// 根据指定的前方向量和向上向量创建一个四元数。
    /// </summary>
    /// <param name="forward">前方向量。</param>
    /// <param name="upwards">向上向量。</param>
    /// <returns>表示旋转的四元数。</returns>
    public static FPQuaternion LookRotation(FPVector3 forward, FPVector3 upwards)
    {
        return CreateFromMatrix(FPMatrix.LookAt(forward, upwards));
    }

    /// <summary>
    /// 在两个四元数之间进行球面线性插值。
    /// </summary>
    /// <param name="from">起始四元数。</param>
    /// <param name="to">目标四元数。</param>
    /// <param name="t">插值参数，范围在 0 到 1 之间。</param>
    /// <returns>插值结果四元数。</returns>
    public static FPQuaternion Slerp(FPQuaternion from, FPQuaternion to, FP t)
    {
        t = FPMath.Clamp(t, 0, 1);

        var dot = Dot(from, to);

        if (dot < FP.Zero)
        {
            to = Multiply(to, -1);
            dot = -dot;
        }

        var halfTheta = FP.Acos(dot);

        return Multiply(Multiply(from, FP.Sin((1 - t) * halfTheta)) + Multiply(to, FP.Sin(t * halfTheta)), 1 / FP.Sin(halfTheta));
    }

    /// <summary>
    /// 将一个四元数朝向另一个四元数旋转，但不超过指定的最大角度。
    /// </summary>
    /// <param name="from">起始四元数。</param>
    /// <param name="to">目标四元数。</param>
    /// <param name="maxDegreesDelta">最大旋转角度（以度为单位）。</param>
    /// <returns>旋转后的四元数。</returns>
    public static FPQuaternion RotateTowards(FPQuaternion from, FPQuaternion to, FP maxDegreesDelta)
    {
        var dot = Dot(from, to);

        if (dot < FP.Zero)
        {
            to = Multiply(to, -1);
            dot = -dot;
        }

        var halfTheta = FP.Acos(dot);
        var theta = halfTheta * 2;

        maxDegreesDelta *= FP.Deg2Rad;

        if (maxDegreesDelta >= theta)
        {
            return to;
        }

        maxDegreesDelta /= theta;

        return Multiply(Multiply(from, FP.Sin((1 - maxDegreesDelta) * halfTheta)) + Multiply(to, FP.Sin(maxDegreesDelta * halfTheta)), 1 / FP.Sin(halfTheta));
    }

    /// <summary>
    /// 根据欧拉角创建一个四元数。
    /// </summary>
    /// <param name="x">绕 X 轴的旋转角度（以度为单位）。</param>
    /// <param name="y">绕 Y 轴的旋转角度（以度为单位）。</param>
    /// <param name="z">绕 Z 轴的旋转角度（以度为单位）。</param>
    /// <returns>表示旋转的四元数。</returns>
    public static FPQuaternion Euler(FP x, FP y, FP z)
    {
        x *= FP.Deg2Rad;
        y *= FP.Deg2Rad;
        z *= FP.Deg2Rad;

        CreateFromYawPitchRoll(y, x, z, out var rotation);

        return rotation;
    }

    /// <summary>
    /// 根据欧拉角向量创建一个四元数。
    /// </summary>
    /// <param name="eulerAngles">欧拉角向量。</param>
    /// <returns>表示旋转的四元数。</returns>
    public static FPQuaternion Euler(FPVector3 eulerAngles)
    {
        return Euler(eulerAngles.x, eulerAngles.y, eulerAngles.z);
    }

    /// <summary>
    /// 根据指定的角度和轴创建一个四元数。
    /// </summary>
    /// <param name="angle">旋转角度（以度为单位）。</param>
    /// <param name="axis">旋转轴。</param>
    /// <returns>表示旋转的四元数。</returns>
    public static FPQuaternion AngleAxis(FP angle, FPVector3 axis)
    {
        axis = axis * FP.Deg2Rad;
        axis.Normalize();

        var halfAngle = angle * FP.Deg2Rad * FP.Half;

        FPQuaternion rotation;
        var sin = FP.Sin(halfAngle);

        rotation.x = axis.x * sin;
        rotation.y = axis.y * sin;
        rotation.z = axis.z * sin;
        rotation.w = FP.Cos(halfAngle);

        return rotation;
    }

    /// <summary>
    /// 根据指定的偏航角、俯仰角和翻滚角创建一个四元数。
    /// </summary>
    /// <param name="yaw">偏航角（绕 Y 轴的旋转角度）。</param>
    /// <param name="pitch">俯仰角（绕 X 轴的旋转角度）。</param>
    /// <param name="roll">翻滚角（绕 Z 轴的旋转角度）。</param>
    /// <param name="result">表示旋转的四元数。</param>
    public static void CreateFromYawPitchRoll(FP yaw, FP pitch, FP roll, out FPQuaternion result)
    {
        var num9 = roll * FP.Half;
        var num6 = FP.Sin(num9);
        var num5 = FP.Cos(num9);
        var num8 = pitch * FP.Half;
        var num4 = FP.Sin(num8);
        var num3 = FP.Cos(num8);
        var num7 = yaw * FP.Half;
        var num2 = FP.Sin(num7);
        var num = FP.Cos(num7);
        result.x = num * num4 * num5 + num2 * num3 * num6;
        result.y = num2 * num3 * num5 - num * num4 * num6;
        result.z = num * num3 * num6 - num2 * num4 * num5;
        result.w = num * num3 * num5 + num2 * num4 * num6;
    }

    /// <summary>
    /// 计算两个四元数的和。
    /// </summary>
    /// <param name="quaternion1">第一个四元数。</param>
    /// <param name="quaternion2">第二个四元数。</param>
    /// <param name="result">两个四元数的和。</param>
    public static void Add(ref FPQuaternion quaternion1, ref FPQuaternion quaternion2, out FPQuaternion result)
    {
        result.x = quaternion1.x + quaternion2.x;
        result.y = quaternion1.y + quaternion2.y;
        result.z = quaternion1.z + quaternion2.z;
        result.w = quaternion1.w + quaternion2.w;
    }

    #endregion

    /// <summary>
    /// 计算四元数的共轭。
    /// </summary>
    /// <param name="value">要计算共轭的四元数。</param>
    /// <returns>四元数的共轭。</returns>
    public static FPQuaternion Conjugate(FPQuaternion value)
    {
        FPQuaternion quaternion;
        quaternion.x = -value.x;
        quaternion.y = -value.y;
        quaternion.z = -value.z;
        quaternion.w = value.w;
        return quaternion;
    }

    /// <summary>
    /// 计算两个四元数的点积。
    /// </summary>
    /// <param name="a">第一个四元数。</param>
    /// <param name="b">第二个四元数。</param>
    /// <returns>两个四元数的点积。</returns>
    public static FP Dot(FPQuaternion a, FPQuaternion b)
    {
        return a.w * b.w + a.x * b.x + a.y * b.y + a.z * b.z;
    }

    /// <summary>
    /// 计算四元数的逆。
    /// </summary>
    /// <param name="rotation">要计算逆的四元数。</param>
    /// <returns>四元数的逆。</returns>
    public static FPQuaternion Inverse(FPQuaternion rotation)
    {
        var invNorm = FP.One / (rotation.x * rotation.x + rotation.y * rotation.y + rotation.z * rotation.z + rotation.w * rotation.w);
        return Multiply(Conjugate(rotation), invNorm);
    }

    /// <summary>
    /// 计算从一个向量到另一个向量的旋转四元数。
    /// </summary>
    /// <param name="fromVector3">起始向量。</param>
    /// <param name="toVector3">目标向量。</param>
    /// <returns>表示从一个向量到另一个向量的旋转的四元数。</returns>
    public static FPQuaternion FromToRotation(FPVector3 fromVector3, FPVector3 toVector3)
    {
        var w = FPVector3.Cross(fromVector3, toVector3);
        var q = new FPQuaternion(w.x, w.y, w.z, FPVector3.Dot(fromVector3, toVector3));
        q.w += FP.Sqrt(fromVector3.sqrMagnitude * toVector3.sqrMagnitude);
        q.Normalize();

        return q;
    }

    /// <summary>
    /// 在两个四元数之间进行线性插值。
    /// </summary>
    /// <param name="a">起始四元数。</param>
    /// <param name="b">目标四元数。</param>
    /// <param name="t">插值参数，范围在 0 到 1 之间。</param>
    /// <returns>插值结果四元数。</returns>
    public static FPQuaternion Lerp(FPQuaternion a, FPQuaternion b, FP t)
    {
        t = FPMath.Clamp(t, FP.Zero, FP.One);

        return LerpUnclamped(a, b, t);
    }

    /// <summary>
    /// 在两个四元数之间进行线性插值，不进行参数限制。
    /// </summary>
    /// <param name="a">起始四元数。</param>
    /// <param name="b">目标四元数。</param>
    /// <param name="t">插值参数。</param>
    /// <returns>插值结果四元数。</returns>
    public static FPQuaternion LerpUnclamped(FPQuaternion a, FPQuaternion b, FP t)
    {
        var result = Multiply(a, 1 - t) + Multiply(b, t);
        result.Normalize();

        return result;
    }

    #region public static JQuaternion Subtract(JQuaternion quaternion1, JQuaternion quaternion2)

    /// <summary>
    /// 计算两个四元数的差。
    /// </summary>
    /// <param name="quaternion1">第一个四元数。</param>
    /// <param name="quaternion2">第二个四元数。</param>
    /// <returns>两个四元数的差。</returns>
    public static FPQuaternion Subtract(FPQuaternion quaternion1, FPQuaternion quaternion2)
    {
        Subtract(ref quaternion1, ref quaternion2, out var result);
        return result;
    }

    /// <summary>
    /// 计算两个四元数的差。
    /// </summary>
    /// <param name="quaternion1">第一个四元数。</param>
    /// <param name="quaternion2">第二个四元数。</param>
    /// <param name="result">两个四元数的差。</param>
    public static void Subtract(ref FPQuaternion quaternion1, ref FPQuaternion quaternion2, out FPQuaternion result)
    {
        result.x = quaternion1.x - quaternion2.x;
        result.y = quaternion1.y - quaternion2.y;
        result.z = quaternion1.z - quaternion2.z;
        result.w = quaternion1.w - quaternion2.w;
    }

    #endregion

    #region public static JQuaternion Multiply(JQuaternion quaternion1, JQuaternion quaternion2)

    /// <summary>
    /// 计算两个四元数的乘积。
    /// </summary>
    /// <param name="quaternion1">第一个四元数。</param>
    /// <param name="quaternion2">第二个四元数。</param>
    /// <returns>两个四元数的乘积。</returns>
    public static FPQuaternion Multiply(FPQuaternion quaternion1, FPQuaternion quaternion2)
    {
        Multiply(ref quaternion1, ref quaternion2, out var result);
        return result;
    }

    /// <summary>
    /// 计算两个四元数的乘积。
    /// </summary>
    /// <param name="quaternion1">第一个四元数。</param>
    /// <param name="quaternion2">第二个四元数。</param>
    /// <param name="result">两个四元数的乘积。</param>
    public static void Multiply(ref FPQuaternion quaternion1, ref FPQuaternion quaternion2, out FPQuaternion result)
    {
        var x = quaternion1.x;
        var y = quaternion1.y;
        var z = quaternion1.z;
        var w = quaternion1.w;
        var num4 = quaternion2.x;
        var num3 = quaternion2.y;
        var num2 = quaternion2.z;
        var num = quaternion2.w;
        var num12 = y * num2 - z * num3;
        var num11 = z * num4 - x * num2;
        var num10 = x * num3 - y * num4;
        var num9 = x * num4 + y * num3 + z * num2;
        result.x = x * num + num4 * w + num12;
        result.y = y * num + num3 * w + num11;
        result.z = z * num + num2 * w + num10;
        result.w = w * num - num9;
    }

    #endregion

    #region public static JQuaternion Multiply(JQuaternion quaternion1, FP scaleFactor)

    /// <summary>
    /// 计算四元数与缩放因子的乘积。
    /// </summary>
    /// <param name="quaternion1">要缩放的四元数。</param>
    /// <param name="scaleFactor">缩放因子。</param>
    /// <returns>缩放后的四元数。</returns>
    public static FPQuaternion Multiply(FPQuaternion quaternion1, FP scaleFactor)
    {
        Multiply(ref quaternion1, scaleFactor, out var result);
        return result;
    }

    /// <summary>
    /// 缩放一个四元数。
    /// </summary>
    /// <param name="quaternion1">要缩放的四元数。</param>
    /// <param name="scaleFactor">缩放因子。</param>
    /// <param name="result">缩放后的四元数。</param>
    public static void Multiply(ref FPQuaternion quaternion1, FP scaleFactor, out FPQuaternion result)
    {
        result.x = quaternion1.x * scaleFactor;
        result.y = quaternion1.y * scaleFactor;
        result.z = quaternion1.z * scaleFactor;
        result.w = quaternion1.w * scaleFactor;
    }

    #endregion

    #region public void Normalize()

    /// <summary>
    /// 对当前四元数进行归一化。
    /// </summary>
    /// <remarks>
    /// 归一化会将四元数的模长变为1，确保其表示一个有效的旋转。
    /// </remarks>
    public void Normalize()
    {
        var num2 = x * x + y * y + z * z + w * w;
        var num = 1 / FP.Sqrt(num2);
        x *= num;
        y *= num;
        z *= num;
        w *= num;
    }

    #endregion

    /// <summary>
    /// 从轴和角度创建一个四元数。
    /// </summary>
    /// <param name="axis">旋转轴，必须是单位向量。</param>
    /// <param name="angle">旋转角度，以弧度为单位。</param>
    /// <returns>表示旋转的 FPQuaternion。</returns>
    public static FPQuaternion CreateFromAxisAngle(FPVector3 axis, FP angle)
    {
        // 确保旋转轴是单位向量
        axis = axis.normalized;

        // 将角度减半并计算正弦和余弦值
        var halfAngle = angle * FP.Half;
        var sinHalfAngle = FPMath.Sin(halfAngle);
        var cosHalfAngle = FPMath.Cos(halfAngle);

        // 使用公式生成四元数
        return new FPQuaternion(
            axis.x * sinHalfAngle,
            axis.y * sinHalfAngle,
            axis.z * sinHalfAngle,
            cosHalfAngle
        );
    }

    #region public static JQuaternion CreateFromMatrix(JMatrix matrix)

    /// <summary>
    /// 从矩阵创建一个四元数。
    /// </summary>
    /// <param name="matrix">表示方向的矩阵。</param>
    /// <returns>表示方向的 FPQuaternion。</returns>
    public static FPQuaternion CreateFromMatrix(FPMatrix matrix)
    {
        CreateFromMatrix(ref matrix, out var result);
        return result;
    }

    /// <summary>
    /// 从矩阵创建一个四元数。
    /// </summary>
    /// <param name="matrix">表示方向的矩阵。</param>
    /// <param name="result">表示方向的 FPQuaternion。</param>
    public static void CreateFromMatrix(ref FPMatrix matrix, out FPQuaternion result)
    {
        var num8 = matrix.M11 + matrix.M22 + matrix.M33;
        if (num8 > FP.Zero)
        {
            var num = FP.Sqrt(num8 + FP.One);
            result.w = num * FP.Half;
            num = FP.Half / num;
            result.x = (matrix.M23 - matrix.M32) * num;
            result.y = (matrix.M31 - matrix.M13) * num;
            result.z = (matrix.M12 - matrix.M21) * num;
        }
        else if (matrix.M11 >= matrix.M22 && matrix.M11 >= matrix.M33)
        {
            var num7 = FP.Sqrt(FP.One + matrix.M11 - matrix.M22 - matrix.M33);
            var num4 = FP.Half / num7;
            result.x = FP.Half * num7;
            result.y = (matrix.M12 + matrix.M21) * num4;
            result.z = (matrix.M13 + matrix.M31) * num4;
            result.w = (matrix.M23 - matrix.M32) * num4;
        }
        else if (matrix.M22 > matrix.M33)
        {
            var num6 = FP.Sqrt(FP.One + matrix.M22 - matrix.M11 - matrix.M33);
            var num3 = FP.Half / num6;
            result.x = (matrix.M21 + matrix.M12) * num3;
            result.y = FP.Half * num6;
            result.z = (matrix.M32 + matrix.M23) * num3;
            result.w = (matrix.M31 - matrix.M13) * num3;
        }
        else
        {
            var num5 = FP.Sqrt(FP.One + matrix.M33 - matrix.M11 - matrix.M22);
            var num2 = FP.Half / num5;
            result.x = (matrix.M31 + matrix.M13) * num2;
            result.y = (matrix.M32 + matrix.M23) * num2;
            result.z = FP.Half * num5;
            result.w = (matrix.M12 - matrix.M21) * num2;
        }
    }

    #endregion

    #region public static FP operator *(JQuaternion value1, JQuaternion value2)

    /// <summary>
    /// 乘以两个四元数。
    /// </summary>
    /// <param name="value1">第一个四元数。</param>
    /// <param name="value2">第二个四元数。</param>
    /// <returns>两个四元数的乘积。</returns>
    public static FPQuaternion operator *(FPQuaternion value1, FPQuaternion value2)
    {
        Multiply(ref value1, ref value2, out var result);
        return result;
    }

    #endregion

    #region public static FP operator +(JQuaternion value1, JQuaternion value2)

    /// <summary>
    /// 加上两个四元数。
    /// </summary>
    /// <param name="value1">第一个四元数。</param>
    /// <param name="value2">第二个四元数。</param>
    /// <returns>两个四元数的和。</returns>
    public static FPQuaternion operator +(FPQuaternion value1, FPQuaternion value2)
    {
        Add(ref value1, ref value2, out var result);
        return result;
    }

    #endregion

    #region public static FP operator -(JQuaternion value1, JQuaternion value2)

    /// <summary>
    /// 减去两个四元数。
    /// </summary>
    /// <param name="value1">第一个四元数。</param>
    /// <param name="value2">第二个四元数。</param>
    /// <returns>两个四元数的差。</returns>
    public static FPQuaternion operator -(FPQuaternion value1, FPQuaternion value2)
    {
        Subtract(ref value1, ref value2, out var result);
        return result;
    }

    #endregion

    /// <summary>
    /// 使用四元数旋转一个三维向量。
    /// </summary>
    /// <param name="quat">要应用的四元数。</param>
    /// <param name="vec">要旋转的三维向量。</param>
    /// <returns>旋转后的三维向量。</returns>
    public static FPVector3 operator *(FPQuaternion quat, FPVector3 vec)
    {
        var num = quat.x * 2 * FP.One;
        var num2 = quat.y * 2 * FP.One;
        var num3 = quat.z * 2 * FP.One;
        var num4 = quat.x * num;
        var num5 = quat.y * num2;
        var num6 = quat.z * num3;
        var num7 = quat.x * num2;
        var num8 = quat.x * num3;
        var num9 = quat.y * num3;
        var num10 = quat.w * num;
        var num11 = quat.w * num2;
        var num12 = quat.w * num3;

        FPVector3 result;
        result.x = (FP.One - (num5 + num6)) * vec.x + (num7 - num12) * vec.y + (num8 + num11) * vec.z;
        result.y = (num7 + num12) * vec.x + (FP.One - (num4 + num6)) * vec.y + (num9 - num10) * vec.z;
        result.z = (num8 - num11) * vec.x + (num9 + num10) * vec.y + (FP.One - (num4 + num5)) * vec.z;

        return result;
    }

    /// <summary>
    /// 返回四元数的字符串表示形式。
    /// </summary>
    /// <returns>四元数的字符串表示形式。</returns>
    public override string ToString()
    {
        return $"({x.AsFloat():f5}, {y.AsFloat():f5}, {z.AsFloat():f5}, {w.AsFloat():f5})";
    }

    /// <summary>
    /// 判断两个四元数是否相等。
    /// </summary>
    /// <param name="value1">第一个四元数。</param>
    /// <param name="value2">第二个四元数。</param>
    /// <returns>如果两个四元数相等，则返回 true；否则返回 false。</returns>
    public static bool operator ==(FPQuaternion value1, FPQuaternion value2)
    {
        return value1.x == value2.x && value1.y == value2.y && value1.z == value2.z && value1.w == value2.w;
    }

    /// <summary>
    /// 判断两个四元数是否不相等。
    /// </summary>
    /// <param name="value1">第一个四元数。</param>
    /// <param name="value2">第二个四元数。</param>
    /// <returns>如果两个四元数不相等，则返回 true；否则返回 false。</returns>
    public static bool operator !=(FPQuaternion value1, FPQuaternion value2)
    {
        if (value1.x == value2.x && value1.y == value2.y)
        {
            return value1.z != value2.z && value1.w != value2.w;
        }

        return true;
    }

    /// <summary>
    /// 判断当前四元数是否与指定对象相等。
    /// </summary>
    /// <param name="obj">要比较的对象。</param>
    /// <returns>如果当前四元数与指定对象相等，则返回 true；否则返回 false。</returns>
    public override bool Equals(object obj)
    {
        if (!(obj is FPQuaternion))
        {
            return false;
        }

        var other = (FPQuaternion)obj;

        return x == other.x && y == other.y && z == other.z && w == other.w;
    }

    /// <summary>
    /// 返回当前四元数的哈希代码。
    /// </summary>
    /// <returns>当前四元数的哈希代码。</returns>
    public override int GetHashCode()
    {
        return x.GetHashCode() ^ y.GetHashCode() ^ z.GetHashCode() ^ w.GetHashCode();
    }
}