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
/// 表示一个3x3矩阵。
/// </summary>
public struct FPMatrix
{
    /// <summary>
    /// 矩阵的第1行第1列元素。
    /// </summary>
    public FP M11; // 1st row vector

    /// <summary>
    /// 矩阵的第1行第2列元素。
    /// </summary>
    public FP M12;

    /// <summary>
    /// 矩阵的第1行第3列元素。
    /// </summary>
    public FP M13;

    /// <summary>
    /// 矩阵的第2行第1列元素。
    /// </summary>
    public FP M21; // 2nd row vector

    /// <summary>
    /// 矩阵的第2行第2列元素。
    /// </summary>
    public FP M22;

    /// <summary>
    /// 矩阵的第2行第3列元素。
    /// </summary>
    public FP M23;

    /// <summary>
    /// 矩阵的第3行第1列元素。
    /// </summary>
    public FP M31; // 3rd row vector

    /// <summary>
    /// 矩阵的第3行第2列元素。
    /// </summary>
    public FP M32;

    /// <summary>
    /// 矩阵的第3行第3列元素。
    /// </summary>
    public FP M33;

    internal static FPMatrix InternalIdentity;

    /// <summary>
    /// 单位矩阵。
    /// </summary>
    public static readonly FPMatrix Identity;

    /// <summary>
    /// 零矩阵。
    /// </summary>
    public static readonly FPMatrix Zero;

    static FPMatrix()
    {
        Zero = new FPMatrix();

        Identity = new FPMatrix();
        Identity.M11 = FP.One;
        Identity.M22 = FP.One;
        Identity.M33 = FP.One;

        InternalIdentity = Identity;
    }

    /// <summary>
    /// 获取矩阵的欧拉角。
    /// </summary>
    public FPVector3 eulerAngles
    {
        get
        {
            var result = new FPVector3();

            result.x = FPMath.Atan2(M32, M33) * FP.Rad2Deg;
            result.y = FPMath.Atan2(-M31, FPMath.Sqrt(M32 * M32 + M33 * M33)) * FP.Rad2Deg;
            result.z = FPMath.Atan2(M21, M11) * FP.Rad2Deg;

            return result * -1;
        }
    }

    /// <summary>
    /// 根据偏航、俯仰和滚转角度创建旋转矩阵。
    /// </summary>
    /// <param name="yaw">偏航角度。</param>
    /// <param name="pitch">俯仰角度。</param>
    /// <param name="roll">滚转角度。</param>
    /// <returns>生成的旋转矩阵。</returns>
    public static FPMatrix CreateFromYawPitchRoll(FP yaw, FP pitch, FP roll)
    {
        FPMatrix matrix;
        FPQuaternion quaternion;
        FPQuaternion.CreateFromYawPitchRoll(yaw, pitch, roll, out quaternion);
        CreateFromQuaternion(ref quaternion, out matrix);
        return matrix;
    }

    /// <summary>
    /// 创建绕X轴旋转的矩阵。
    /// </summary>
    /// <param name="radians">旋转角度（弧度）。</param>
    /// <returns>生成的旋转矩阵。</returns>
    public static FPMatrix CreateRotationX(FP radians)
    {
        FPMatrix matrix;
        var num2 = FP.Cos(radians);
        var num = FP.Sin(radians);
        matrix.M11 = FP.One;
        matrix.M12 = FP.Zero;
        matrix.M13 = FP.Zero;
        matrix.M21 = FP.Zero;
        matrix.M22 = num2;
        matrix.M23 = num;
        matrix.M31 = FP.Zero;
        matrix.M32 = -num;
        matrix.M33 = num2;
        return matrix;
    }

    /// <summary>
    /// 创建绕X轴旋转的矩阵，并将结果输出到指定的矩阵。
    /// </summary>
    /// <param name="radians">旋转角度（弧度）。</param>
    /// <param name="result">输出的旋转矩阵。</param>
    public static void CreateRotationX(FP radians, out FPMatrix result)
    {
        var num2 = FP.Cos(radians);
        var num = FP.Sin(radians);
        result.M11 = FP.One;
        result.M12 = FP.Zero;
        result.M13 = FP.Zero;
        result.M21 = FP.Zero;
        result.M22 = num2;
        result.M23 = num;
        result.M31 = FP.Zero;
        result.M32 = -num;
        result.M33 = num2;
    }

    /// <summary>
    /// 创建绕Y轴旋转的矩阵。
    /// </summary>
    /// <param name="radians">旋转角度（弧度）。</param>
    /// <returns>生成的旋转矩阵。</returns>
    public static FPMatrix CreateRotationY(FP radians)
    {
        FPMatrix matrix;
        var num2 = FP.Cos(radians);
        var num = FP.Sin(radians);
        matrix.M11 = num2;
        matrix.M12 = FP.Zero;
        matrix.M13 = -num;
        matrix.M21 = FP.Zero;
        matrix.M22 = FP.One;
        matrix.M23 = FP.Zero;
        matrix.M31 = num;
        matrix.M32 = FP.Zero;
        matrix.M33 = num2;
        return matrix;
    }

    /// <summary>
    /// 创建绕Y轴旋转的矩阵，并将结果输出到指定的矩阵。
    /// </summary>
    /// <param name="radians">旋转角度（弧度）。</param>
    /// <param name="result">输出的旋转矩阵。</param>
    public static void CreateRotationY(FP radians, out FPMatrix result)
    {
        var num2 = FP.Cos(radians);
        var num = FP.Sin(radians);
        result.M11 = num2;
        result.M12 = FP.Zero;
        result.M13 = -num;
        result.M21 = FP.Zero;
        result.M22 = FP.One;
        result.M23 = FP.Zero;
        result.M31 = num;
        result.M32 = FP.Zero;
        result.M33 = num2;
    }

    /// <summary>
    /// 创建绕Z轴旋转的矩阵。
    /// </summary>
    /// <param name="radians">旋转角度（弧度）。</param>
    /// <returns>生成的旋转矩阵。</returns>
    public static FPMatrix CreateRotationZ(FP radians)
    {
        FPMatrix matrix;
        var num2 = FP.Cos(radians);
        var num = FP.Sin(radians);
        matrix.M11 = num2;
        matrix.M12 = num;
        matrix.M13 = FP.Zero;
        matrix.M21 = -num;
        matrix.M22 = num2;
        matrix.M23 = FP.Zero;
        matrix.M31 = FP.Zero;
        matrix.M32 = FP.Zero;
        matrix.M33 = FP.One;
        return matrix;
    }

    /// <summary>
    /// 创建绕Z轴旋转的矩阵，并将结果输出到指定的矩阵。
    /// </summary>
    /// <param name="radians">旋转角度（弧度）。</param>
    /// <param name="result">输出的旋转矩阵。</param>
    public static void CreateRotationZ(FP radians, out FPMatrix result)
    {
        var num2 = FP.Cos(radians);
        var num = FP.Sin(radians);
        result.M11 = num2;
        result.M12 = num;
        result.M13 = FP.Zero;
        result.M21 = -num;
        result.M22 = num2;
        result.M23 = FP.Zero;
        result.M31 = FP.Zero;
        result.M32 = FP.Zero;
        result.M33 = FP.One;
    }

    /// <summary>
    /// 初始化矩阵结构的新实例。
    /// </summary>
    /// <param name="m11">矩阵的第1行第1列元素。</param>
    /// <param name="m12">矩阵的第1行第2列元素。</param>
    /// <param name="m13">矩阵的第1行第3列元素。</param>
    /// <param name="m21">矩阵的第2行第1列元素。</param>
    /// <param name="m22">矩阵的第2行第2列元素。</param>
    /// <param name="m23">矩阵的第2行第3列元素。</param>
    /// <param name="m31">矩阵的第3行第1列元素。</param>
    /// <param name="m32">矩阵的第3行第2列元素。</param>
    /// <param name="m33">矩阵的第3行第3列元素。</param>

    #region public JMatrix(FP m11, FP m12, FP m13, FP m21, FP m22, FP m23,FP m31, FP m32, FP m33)

    public FPMatrix(FP m11, FP m12, FP m13, FP m21, FP m22, FP m23, FP m31, FP m32, FP m33)
    {
        M11 = m11;
        M12 = m12;
        M13 = m13;
        M21 = m21;
        M22 = m22;
        M23 = m23;
        M31 = m31;
        M32 = m32;
        M33 = m33;
    }

    #endregion

    /// <summary>
    /// 乘以两个矩阵。注意：矩阵乘法不是可交换的。
    /// </summary>
    /// <param name="matrix1">第一个矩阵。</param>
    /// <param name="matrix2">第二个矩阵。</param>
    /// <returns>两个矩阵的乘积。</returns>

    #region public static JMatrix Multiply(JMatrix matrix1, JMatrix matrix2)

    public static FPMatrix Multiply(FPMatrix matrix1, FPMatrix matrix2)
    {
        FPMatrix result;
        Multiply(ref matrix1, ref matrix2, out result);
        return result;
    }

    /// <summary>
    /// 乘以两个矩阵。注意：矩阵乘法不是可交换的。
    /// </summary>
    /// <param name="matrix1">第一个矩阵。</param>
    /// <param name="matrix2">第二个矩阵。</param>
    /// <param name="result">两个矩阵的乘积。</param>
    public static void Multiply(ref FPMatrix matrix1, ref FPMatrix matrix2, out FPMatrix result)
    {
        var num0 = matrix1.M11 * matrix2.M11 + matrix1.M12 * matrix2.M21 + matrix1.M13 * matrix2.M31;
        var num1 = matrix1.M11 * matrix2.M12 + matrix1.M12 * matrix2.M22 + matrix1.M13 * matrix2.M32;
        var num2 = matrix1.M11 * matrix2.M13 + matrix1.M12 * matrix2.M23 + matrix1.M13 * matrix2.M33;
        var num3 = matrix1.M21 * matrix2.M11 + matrix1.M22 * matrix2.M21 + matrix1.M23 * matrix2.M31;
        var num4 = matrix1.M21 * matrix2.M12 + matrix1.M22 * matrix2.M22 + matrix1.M23 * matrix2.M32;
        var num5 = matrix1.M21 * matrix2.M13 + matrix1.M22 * matrix2.M23 + matrix1.M23 * matrix2.M33;
        var num6 = matrix1.M31 * matrix2.M11 + matrix1.M32 * matrix2.M21 + matrix1.M33 * matrix2.M31;
        var num7 = matrix1.M31 * matrix2.M12 + matrix1.M32 * matrix2.M22 + matrix1.M33 * matrix2.M32;
        var num8 = matrix1.M31 * matrix2.M13 + matrix1.M32 * matrix2.M23 + matrix1.M33 * matrix2.M33;

        result.M11 = num0;
        result.M12 = num1;
        result.M13 = num2;
        result.M21 = num3;
        result.M22 = num4;
        result.M23 = num5;
        result.M31 = num6;
        result.M32 = num7;
        result.M33 = num8;
    }

    #endregion

    /// <summary>
    /// 矩阵相加。
    /// </summary>
    /// <param name="matrix1">第一个矩阵。</param>
    /// <param name="matrix2">第二个矩阵。</param>
    /// <returns>两个矩阵的和。</returns>

    #region public static JMatrix Add(JMatrix matrix1, JMatrix matrix2)

    public static FPMatrix Add(FPMatrix matrix1, FPMatrix matrix2)
    {
        FPMatrix result;
        Add(ref matrix1, ref matrix2, out result);
        return result;
    }

    /// <summary>
    /// 矩阵相加。
    /// </summary>
    /// <param name="matrix1">第一个矩阵。</param>
    /// <param name="matrix2">第二个矩阵。</param>
    /// <param name="result">两个矩阵的和。</param>
    public static void Add(ref FPMatrix matrix1, ref FPMatrix matrix2, out FPMatrix result)
    {
        result.M11 = matrix1.M11 + matrix2.M11;
        result.M12 = matrix1.M12 + matrix2.M12;
        result.M13 = matrix1.M13 + matrix2.M13;
        result.M21 = matrix1.M21 + matrix2.M21;
        result.M22 = matrix1.M22 + matrix2.M22;
        result.M23 = matrix1.M23 + matrix2.M23;
        result.M31 = matrix1.M31 + matrix2.M31;
        result.M32 = matrix1.M32 + matrix2.M32;
        result.M33 = matrix1.M33 + matrix2.M33;
    }

    #endregion

    /// <summary>
    /// 计算给定矩阵的逆矩阵。
    /// </summary>
    /// <param name="matrix">要计算逆的矩阵。</param>
    /// <returns>逆矩阵。</returns>

    #region public static JMatrix Inverse(JMatrix matrix)

    public static FPMatrix Inverse(FPMatrix matrix)
    {
        FPMatrix result;
        Inverse(ref matrix, out result);
        return result;
    }

    /// <summary>
    /// 计算矩阵的行列式。
    /// </summary>
    /// <returns>矩阵的行列式。</returns>
    public FP Determinant()
    {
        return M11 * M22 * M33 + M12 * M23 * M31 + M13 * M21 * M32 -
               M31 * M22 * M13 - M32 * M23 * M11 - M33 * M21 * M12;
    }

    /// <summary>
    /// 计算给定矩阵的逆矩阵。
    /// </summary>
    /// <param name="matrix">要计算逆的矩阵。</param>
    /// <param name="result">逆矩阵。</param>
    public static void Invert(ref FPMatrix matrix, out FPMatrix result)
    {
        var determinantInverse = 1 / matrix.Determinant();
        var m11 = (matrix.M22 * matrix.M33 - matrix.M23 * matrix.M32) * determinantInverse;
        var m12 = (matrix.M13 * matrix.M32 - matrix.M33 * matrix.M12) * determinantInverse;
        var m13 = (matrix.M12 * matrix.M23 - matrix.M22 * matrix.M13) * determinantInverse;

        var m21 = (matrix.M23 * matrix.M31 - matrix.M21 * matrix.M33) * determinantInverse;
        var m22 = (matrix.M11 * matrix.M33 - matrix.M13 * matrix.M31) * determinantInverse;
        var m23 = (matrix.M13 * matrix.M21 - matrix.M11 * matrix.M23) * determinantInverse;

        var m31 = (matrix.M21 * matrix.M32 - matrix.M22 * matrix.M31) * determinantInverse;
        var m32 = (matrix.M12 * matrix.M31 - matrix.M11 * matrix.M32) * determinantInverse;
        var m33 = (matrix.M11 * matrix.M22 - matrix.M12 * matrix.M21) * determinantInverse;

        result.M11 = m11;
        result.M12 = m12;
        result.M13 = m13;

        result.M21 = m21;
        result.M22 = m22;
        result.M23 = m23;

        result.M31 = m31;
        result.M32 = m32;
        result.M33 = m33;
    }

    /// <summary>
    /// Calculates the inverse of a give matrix.
    /// </summary>
    /// <param name="matrix">The matrix to invert.</param>
    /// <param name="result">The inverted JMatrix.</param>
    public static void Inverse(ref FPMatrix matrix, out FPMatrix result)
    {
        var det = 1024 * matrix.M11 * matrix.M22 * matrix.M33 -
                  1024 * matrix.M11 * matrix.M23 * matrix.M32 -
                  1024 * matrix.M12 * matrix.M21 * matrix.M33 +
                  1024 * matrix.M12 * matrix.M23 * matrix.M31 +
                  1024 * matrix.M13 * matrix.M21 * matrix.M32 -
                  1024 * matrix.M13 * matrix.M22 * matrix.M31;

        var num11 = 1024 * matrix.M22 * matrix.M33 - 1024 * matrix.M23 * matrix.M32;
        var num12 = 1024 * matrix.M13 * matrix.M32 - 1024 * matrix.M12 * matrix.M33;
        var num13 = 1024 * matrix.M12 * matrix.M23 - 1024 * matrix.M22 * matrix.M13;

        var num21 = 1024 * matrix.M23 * matrix.M31 - 1024 * matrix.M33 * matrix.M21;
        var num22 = 1024 * matrix.M11 * matrix.M33 - 1024 * matrix.M31 * matrix.M13;
        var num23 = 1024 * matrix.M13 * matrix.M21 - 1024 * matrix.M23 * matrix.M11;

        var num31 = 1024 * matrix.M21 * matrix.M32 - 1024 * matrix.M31 * matrix.M22;
        var num32 = 1024 * matrix.M12 * matrix.M31 - 1024 * matrix.M32 * matrix.M11;
        var num33 = 1024 * matrix.M11 * matrix.M22 - 1024 * matrix.M21 * matrix.M12;

        if (det == 0)
        {
            result.M11 = FP.PositiveInfinity;
            result.M12 = FP.PositiveInfinity;
            result.M13 = FP.PositiveInfinity;
            result.M21 = FP.PositiveInfinity;
            result.M22 = FP.PositiveInfinity;
            result.M23 = FP.PositiveInfinity;
            result.M31 = FP.PositiveInfinity;
            result.M32 = FP.PositiveInfinity;
            result.M33 = FP.PositiveInfinity;
        }
        else
        {
            result.M11 = num11 / det;
            result.M12 = num12 / det;
            result.M13 = num13 / det;
            result.M21 = num21 / det;
            result.M22 = num22 / det;
            result.M23 = num23 / det;
            result.M31 = num31 / det;
            result.M32 = num32 / det;
            result.M33 = num33 / det;
        }
    }

    #endregion

    /// <summary>
    /// 将矩阵乘以一个缩放因子。
    /// </summary>
    /// <param name="matrix1">要缩放的矩阵。</param>
    /// <param name="scaleFactor">缩放因子。</param>
    /// <returns>缩放后的矩阵。</returns>

    #region public static JMatrix Multiply(JMatrix matrix1, FP scaleFactor)

    public static FPMatrix Multiply(FPMatrix matrix1, FP scaleFactor)
    {
        FPMatrix result;
        Multiply(ref matrix1, scaleFactor, out result);
        return result;
    }

    /// <summary>
    /// 将矩阵乘以一个缩放因子，并将结果输出到指定的矩阵。
    /// </summary>
    /// <param name="matrix1">要缩放的矩阵。</param>
    /// <param name="scaleFactor">缩放因子。</param>
    /// <param name="result">缩放后的矩阵。</param>
    public static void Multiply(ref FPMatrix matrix1, FP scaleFactor, out FPMatrix result)
    {
        var num = scaleFactor;
        result.M11 = matrix1.M11 * num;
        result.M12 = matrix1.M12 * num;
        result.M13 = matrix1.M13 * num;
        result.M21 = matrix1.M21 * num;
        result.M22 = matrix1.M22 * num;
        result.M23 = matrix1.M23 * num;
        result.M31 = matrix1.M31 * num;
        result.M32 = matrix1.M32 * num;
        result.M33 = matrix1.M33 * num;
    }

    #endregion

    /// <summary>
    /// 根据四元数创建表示方向的矩阵。
    /// </summary>
    /// <param name="quaternion">用于创建矩阵的四元数。</param>
    /// <returns>表示方向的矩阵。</returns>

    #region public static JMatrix CreateFromQuaternion(JQuaternion quaternion)

    public static FPMatrix CreateFromLookAt(FPVector3 position, FPVector3 target)
    {
        LookAt(target - position, FPVector3.up, out var result);
        return result;
    }

    /// <summary>
    /// 创建一个观察矩阵。
    /// </summary>
    /// <param name="forward">前向向量。</param>
    /// <param name="upwards">向上向量。</param>
    /// <returns>观察矩阵。</returns>
    public static FPMatrix LookAt(FPVector3 forward, FPVector3 upwards)
    {
        LookAt(forward, upwards, out var result);

        return result;
    }

    /// <summary>
    /// 创建一个观察矩阵。
    /// </summary>
    /// <param name="forward">前向向量。</param>
    /// <param name="upwards">向上向量。</param>
    /// <param name="result">输出的观察矩阵。</param>
    public static void LookAt(FPVector3 forward, FPVector3 upwards, out FPMatrix result)
    {
        var zaxis = forward;
        zaxis.Normalize();
        var xaxis = FPVector3.Cross(upwards, zaxis);
        xaxis.Normalize();
        var yaxis = FPVector3.Cross(zaxis, xaxis);

        result.M11 = xaxis.x;
        result.M21 = yaxis.x;
        result.M31 = zaxis.x;
        result.M12 = xaxis.y;
        result.M22 = yaxis.y;
        result.M32 = zaxis.y;
        result.M13 = xaxis.z;
        result.M23 = yaxis.z;
        result.M33 = zaxis.z;
    }

    /// <summary>
    /// 根据四元数创建表示方向的矩阵。
    /// </summary>
    /// <param name="quaternion">用于创建矩阵的四元数。</param>
    /// <returns>表示方向的矩阵。</returns>
    public static FPMatrix CreateFromQuaternion(FPQuaternion quaternion)
    {
        FPMatrix result;
        CreateFromQuaternion(ref quaternion, out result);
        return result;
    }

    /// <summary>
    /// 根据四元数创建表示方向的矩阵。
    /// </summary>
    /// <param name="quaternion">用于创建矩阵的四元数。</param>
    /// <param name="result">表示方向的矩阵。</param>
    public static void CreateFromQuaternion(ref FPQuaternion quaternion, out FPMatrix result)
    {
        var num9 = quaternion.x * quaternion.x;
        var num8 = quaternion.y * quaternion.y;
        var num7 = quaternion.z * quaternion.z;
        var num6 = quaternion.x * quaternion.y;
        var num5 = quaternion.z * quaternion.w;
        var num4 = quaternion.z * quaternion.x;
        var num3 = quaternion.y * quaternion.w;
        var num2 = quaternion.y * quaternion.z;
        var num = quaternion.x * quaternion.w;
        result.M11 = FP.One - 2 * (num8 + num7);
        result.M12 = 2 * (num6 + num5);
        result.M13 = 2 * (num4 - num3);
        result.M21 = 2 * (num6 - num5);
        result.M22 = FP.One - 2 * (num7 + num9);
        result.M23 = 2 * (num2 + num);
        result.M31 = 2 * (num4 + num3);
        result.M32 = 2 * (num2 - num);
        result.M33 = FP.One - 2 * (num8 + num9);
    }

    #endregion

    /// <summary>
    /// 创建转置矩阵。
    /// </summary>
    /// <param name="matrix">要转置的矩阵。</param>
    /// <returns>转置后的矩阵。</returns>

    #region public static JMatrix Transpose(JMatrix matrix)

    public static FPMatrix Transpose(FPMatrix matrix)
    {
        FPMatrix result;
        Transpose(ref matrix, out result);
        return result;
    }

    /// <summary>
    /// 创建转置矩阵。
    /// </summary>
    /// <param name="matrix">要转置的矩阵。</param>
    /// <param name="result">转置后的矩阵。</param>
    public static void Transpose(ref FPMatrix matrix, out FPMatrix result)
    {
        result.M11 = matrix.M11;
        result.M12 = matrix.M21;
        result.M13 = matrix.M31;
        result.M21 = matrix.M12;
        result.M22 = matrix.M22;
        result.M23 = matrix.M32;
        result.M31 = matrix.M13;
        result.M32 = matrix.M23;
        result.M33 = matrix.M33;
    }

    #endregion

    /// <summary>
    /// 乘以两个矩阵。
    /// </summary>
    /// <param name="value1">第一个矩阵。</param>
    /// <param name="value2">第二个矩阵。</param>
    /// <returns>两个矩阵的乘积。</returns>

    #region public static JMatrix operator *(JMatrix value1,JMatrix value2)

    public static FPMatrix operator *(FPMatrix value1, FPMatrix value2)
    {
        FPMatrix result;
        Multiply(ref value1, ref value2, out result);
        return result;
    }

    #endregion

    /// <summary>
    /// 计算矩阵的迹。
    /// </summary>
    /// <returns>矩阵的迹。</returns>
    public FP Trace()
    {
        return M11 + M22 + M33;
    }

    /// <summary>
    /// 将两个矩阵相加。
    /// </summary>
    /// <param name="value1">第一个矩阵。</param>
    /// <param name="value2">第二个矩阵。</param>
    /// <returns>两个矩阵的和。</returns>

    #region public static JMatrix operator +(JMatrix value1, JMatrix value2)

    public static FPMatrix operator +(FPMatrix value1, FPMatrix value2)
    {
        FPMatrix result;
        Add(ref value1, ref value2, out result);
        return result;
    }

    #endregion

    /// <summary>
    /// 将两个矩阵相减。
    /// </summary>
    /// <param name="value1">第一个矩阵。</param>
    /// <param name="value2">第二个矩阵。</param>
    /// <returns>两个矩阵的差。</returns>

    #region public static JMatrix operator -(JMatrix value1, JMatrix value2)

    public static FPMatrix operator -(FPMatrix value1, FPMatrix value2)
    {
        FPMatrix result;
        Multiply(ref value2, -FP.One, out value2);
        Add(ref value1, ref value2, out result);
        return result;
    }

    #endregion

    /// <summary>
    /// 判断两个矩阵是否相等。
    /// </summary>
    /// <param name="value1">第一个矩阵。</param>
    /// <param name="value2">第二个矩阵。</param>
    /// <returns>如果相等则返回true，否则返回false。</returns>
    public static bool operator ==(FPMatrix value1, FPMatrix value2)
    {
        return value1.M11 == value2.M11 &&
               value1.M12 == value2.M12 &&
               value1.M13 == value2.M13 &&
               value1.M21 == value2.M21 &&
               value1.M22 == value2.M22 &&
               value1.M23 == value2.M23 &&
               value1.M31 == value2.M31 &&
               value1.M32 == value2.M32 &&
               value1.M33 == value2.M33;
    }

    /// <summary>
    /// 判断两个矩阵是否不相等。
    /// </summary>
    /// <param name="value1">第一个矩阵。</param>
    /// <param name="value2">第二个矩阵。</param>
    /// <returns>如果不相等则返回true，否则返回false。</returns>
    public static bool operator !=(FPMatrix value1, FPMatrix value2)
    {
        return value1.M11 != value2.M11 ||
               value1.M12 != value2.M12 ||
               value1.M13 != value2.M13 ||
               value1.M21 != value2.M21 ||
               value1.M22 != value2.M22 ||
               value1.M23 != value2.M23 ||
               value1.M31 != value2.M31 ||
               value1.M32 != value2.M32 ||
               value1.M33 != value2.M33;
    }

    /// <summary>
    /// 判断当前矩阵是否与指定对象相等。
    /// </summary>
    /// <param name="obj">要比较的对象。</param>
    /// <returns>如果相等则返回true，否则返回false。</returns>
    public override bool Equals(object obj)
    {
        if (!(obj is FPMatrix))
        {
            return false;
        }

        var other = (FPMatrix)obj;

        return M11 == other.M11 &&
               M12 == other.M12 &&
               M13 == other.M13 &&
               M21 == other.M21 &&
               M22 == other.M22 &&
               M23 == other.M23 &&
               M31 == other.M31 &&
               M32 == other.M32 &&
               M33 == other.M33;
    }

    /// <summary>
    /// 获取当前矩阵的哈希代码。
    /// </summary>
    /// <returns>当前矩阵的哈希代码。</returns>
    public override int GetHashCode()
    {
        return M11.GetHashCode() ^
               M12.GetHashCode() ^
               M13.GetHashCode() ^
               M21.GetHashCode() ^
               M22.GetHashCode() ^
               M23.GetHashCode() ^
               M31.GetHashCode() ^
               M32.GetHashCode() ^
               M33.GetHashCode();
    }

    /// <summary>
    /// 根据给定轴和角度创建旋转矩阵。
    /// </summary>
    /// <param name="axis">旋转轴。</param>
    /// <param name="angle">旋转角度。</param>
    /// <param name="result">输出的旋转矩阵。</param>

    #region public static void CreateFromAxisAngle(ref JVector axis, FP angle, out JMatrix result)

    public static void CreateFromAxisAngle(ref FPVector3 axis, FP angle, out FPMatrix result)
    {
        var x = axis.x;
        var y = axis.y;
        var z = axis.z;
        var num2 = FP.Sin(angle);
        var num = FP.Cos(angle);
        var num11 = x * x;
        var num10 = y * y;
        var num9 = z * z;
        var num8 = x * y;
        var num7 = x * z;
        var num6 = y * z;
        result.M11 = num11 + num * (FP.One - num11);
        result.M12 = num8 - num * num8 + num2 * z;
        result.M13 = num7 - num * num7 - num2 * y;
        result.M21 = num8 - num * num8 - num2 * z;
        result.M22 = num10 + num * (FP.One - num10);
        result.M23 = num6 - num * num6 + num2 * x;
        result.M31 = num7 - num * num7 + num2 * y;
        result.M32 = num6 - num * num6 - num2 * x;
        result.M33 = num9 + num * (FP.One - num9);
    }

    /// <summary>
    /// 根据给定轴和角度创建旋转矩阵。
    /// </summary>
    /// <param name="axis">旋转轴。</param>
    /// <param name="angle">旋转角度。</param>
    /// <returns>生成的旋转矩阵。</returns>
    public static FPMatrix AngleAxis(FP angle, FPVector3 axis)
    {
        FPMatrix result;
        CreateFromAxisAngle(ref axis, angle, out result);
        return result;
    }

    #endregion

    /// <summary>
    /// 将矩阵转换为字符串表示形式。
    /// </summary>
    /// <returns>矩阵的字符串表示形式。</returns>
    public override string ToString()
    {
        return string.Format("{0}|{1}|{2}|{3}|{4}|{5}|{6}|{7}|{8}", M11.RawValue, M12.RawValue, M13.RawValue, M21.RawValue, M22.RawValue, M23.RawValue, M31.RawValue, M32.RawValue, M33.RawValue);
    }
}