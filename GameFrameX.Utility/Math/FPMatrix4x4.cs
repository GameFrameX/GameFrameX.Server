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
/// 4x4 矩阵结构体，用于表示三维空间中的变换。
/// </summary>
public struct FPMatrix4x4
{
    /// <summary>
    /// 第一行第一列的元素。
    /// </summary>
    public FP M11; // 第一行向量

    /// <summary>
    /// 第一行第二列的元素。
    /// </summary>
    public FP M12;

    /// <summary>
    /// 第一行第三列的元素。
    /// </summary>
    public FP M13;

    /// <summary>
    /// 第一行第四列的元素。
    /// </summary>
    public FP M14;

    /// <summary>
    /// 第二行第一列的元素。
    /// </summary>
    public FP M21; // 第二行向量

    /// <summary>
    /// 第二行第二列的元素。
    /// </summary>
    public FP M22;

    /// <summary>
    /// 第二行第三列的元素。
    /// </summary>
    public FP M23;

    /// <summary>
    /// 第二行第四列的元素。
    /// </summary>
    public FP M24;

    /// <summary>
    /// 第三行第一列的元素。
    /// </summary>
    public FP M31; // 第三行向量

    /// <summary>
    /// 第三行第二列的元素。
    /// </summary>
    public FP M32;

    /// <summary>
    /// 第三行第三列的元素。
    /// </summary>
    public FP M33;

    /// <summary>
    /// 第三行第四列的元素。
    /// </summary>
    public FP M34;

    /// <summary>
    /// 第四行第一列的元素。
    /// </summary>
    public FP M41; // 第四行向量

    /// <summary>
    /// 第四行第二列的元素。
    /// </summary>
    public FP M42;

    /// <summary>
    /// 第四行第三列的元素。
    /// </summary>
    public FP M43;

    /// <summary>
    /// 第四行第四列的元素。
    /// </summary>
    public FP M44;

    internal static FPMatrix4x4 InternalIdentity;

    /// <summary>
    /// 单位矩阵。
    /// </summary>
    public static readonly FPMatrix4x4 Identity;

    public static readonly FPMatrix4x4 Zero;

    static FPMatrix4x4()
    {
        Zero = new FPMatrix4x4();

        Identity = new FPMatrix4x4();
        Identity.M11 = FP.One;
        Identity.M22 = FP.One;
        Identity.M33 = FP.One;
        Identity.M44 = FP.One;

        InternalIdentity = Identity;
    }

    /// <summary>
    /// 初始化矩阵结构的新实例。
    /// </summary>
    /// <param name="m11">第一行第一列的值。</param>
    /// <param name="m12">第一行第二列的值。</param>
    /// <param name="m13">第一行第三列的值。</param>
    /// <param name="m14">第一行第四列的值。</param>
    /// <param name="m21">第二行第一列的值。</param>
    /// <param name="m22">第二行第二列的值。</param>
    /// <param name="m23">第二行第三列的值。</param>
    /// <param name="m24">第二行第四列的值。</param>
    /// <param name="m31">第三行第一列的值。</param>
    /// <param name="m32">第三行第二列的值。</param>
    /// <param name="m33">第三行第三列的值。</param>
    /// <param name="m34">第三行第四列的值。</param>
    /// <param name="m41">第四行第一列的值。</param>
    /// <param name="m42">第四行第二列的值。</param>
    /// <param name="m43">第四行第三列的值。</param>
    /// <param name="m44">第四行第四列的值。</param>
    public FPMatrix4x4(FP m11, FP m12, FP m13, FP m14,
        FP m21, FP m22, FP m23, FP m24,
        FP m31, FP m32, FP m33, FP m34,
        FP m41, FP m42, FP m43, FP m44)
    {
        M11 = m11;
        M12 = m12;
        M13 = m13;
        M14 = m14;
        M21 = m21;
        M22 = m22;
        M23 = m23;
        M24 = m24;
        M31 = m31;
        M32 = m32;
        M33 = m33;
        M34 = m34;
        M41 = m41;
        M42 = m42;
        M43 = m43;
        M44 = m44;
    }

    /// <summary>
    /// 计算两个矩阵的乘积。注意：矩阵乘法不是交换的。
    /// </summary>
    /// <param name="matrix1">第一个矩阵。</param>
    /// <param name="matrix2">第二个矩阵。</param>
    /// <returns>两个矩阵的乘积。</returns>
    public static FPMatrix4x4 Multiply(FPMatrix4x4 matrix1, FPMatrix4x4 matrix2)
    {
        FPMatrix4x4 result;
        Multiply(ref matrix1, ref matrix2, out result);
        return result;
    }

    /// <summary>
    /// 计算两个矩阵的乘积。注意：矩阵乘法不是交换的。
    /// </summary>
    /// <param name="matrix1">第一个矩阵。</param>
    /// <param name="matrix2">第二个矩阵。</param>
    /// <param name="result">两个矩阵的乘积。</param>
    public static void Multiply(ref FPMatrix4x4 matrix1, ref FPMatrix4x4 matrix2, out FPMatrix4x4 result)
    {
        // 第一行
        result.M11 = matrix1.M11 * matrix2.M11 + matrix1.M12 * matrix2.M21 + matrix1.M13 * matrix2.M31 + matrix1.M14 * matrix2.M41;
        result.M12 = matrix1.M11 * matrix2.M12 + matrix1.M12 * matrix2.M22 + matrix1.M13 * matrix2.M32 + matrix1.M14 * matrix2.M42;
        result.M13 = matrix1.M11 * matrix2.M13 + matrix1.M12 * matrix2.M23 + matrix1.M13 * matrix2.M33 + matrix1.M14 * matrix2.M43;
        result.M14 = matrix1.M11 * matrix2.M14 + matrix1.M12 * matrix2.M24 + matrix1.M13 * matrix2.M34 + matrix1.M14 * matrix2.M44;

        // 第二行
        result.M21 = matrix1.M21 * matrix2.M11 + matrix1.M22 * matrix2.M21 + matrix1.M23 * matrix2.M31 + matrix1.M24 * matrix2.M41;
        result.M22 = matrix1.M21 * matrix2.M12 + matrix1.M22 * matrix2.M22 + matrix1.M23 * matrix2.M32 + matrix1.M24 * matrix2.M42;
        result.M23 = matrix1.M21 * matrix2.M13 + matrix1.M22 * matrix2.M23 + matrix1.M23 * matrix2.M33 + matrix1.M24 * matrix2.M43;
        result.M24 = matrix1.M21 * matrix2.M14 + matrix1.M22 * matrix2.M24 + matrix1.M23 * matrix2.M34 + matrix1.M24 * matrix2.M44;

        // 第三行
        result.M31 = matrix1.M31 * matrix2.M11 + matrix1.M32 * matrix2.M21 + matrix1.M33 * matrix2.M31 + matrix1.M34 * matrix2.M41;
        result.M32 = matrix1.M31 * matrix2.M12 + matrix1.M32 * matrix2.M22 + matrix1.M33 * matrix2.M32 + matrix1.M34 * matrix2.M42;
        result.M33 = matrix1.M31 * matrix2.M13 + matrix1.M32 * matrix2.M23 + matrix1.M33 * matrix2.M33 + matrix1.M34 * matrix2.M43;
        result.M34 = matrix1.M31 * matrix2.M14 + matrix1.M32 * matrix2.M24 + matrix1.M33 * matrix2.M34 + matrix1.M34 * matrix2.M44;

        // 第四行
        result.M41 = matrix1.M41 * matrix2.M11 + matrix1.M42 * matrix2.M21 + matrix1.M43 * matrix2.M31 + matrix1.M44 * matrix2.M41;
        result.M42 = matrix1.M41 * matrix2.M12 + matrix1.M42 * matrix2.M22 + matrix1.M43 * matrix2.M32 + matrix1.M44 * matrix2.M42;
        result.M43 = matrix1.M41 * matrix2.M13 + matrix1.M42 * matrix2.M23 + matrix1.M43 * matrix2.M33 + matrix1.M44 * matrix2.M43;
        result.M44 = matrix1.M41 * matrix2.M14 + matrix1.M42 * matrix2.M24 + matrix1.M43 * matrix2.M34 + matrix1.M44 * matrix2.M44;
    }

    /// <summary>
    /// 将两个矩阵相加。
    /// </summary>
    /// <param name="matrix1">第一个矩阵。</param>
    /// <param name="matrix2">第二个矩阵。</param>
    /// <returns>两个矩阵的和。</returns>
    public static FPMatrix4x4 Add(FPMatrix4x4 matrix1, FPMatrix4x4 matrix2)
    {
        FPMatrix4x4 result;
        Add(ref matrix1, ref matrix2, out result);
        return result;
    }

    /// <summary>
    /// 将两个矩阵相加。
    /// </summary>
    /// <param name="matrix1">第一个矩阵。</param>
    /// <param name="matrix2">第二个矩阵。</param>
    /// <param name="result">两个矩阵的和。</param>
    public static void Add(ref FPMatrix4x4 matrix1, ref FPMatrix4x4 matrix2, out FPMatrix4x4 result)
    {
        result.M11 = matrix1.M11 + matrix2.M11;
        result.M12 = matrix1.M12 + matrix2.M12;
        result.M13 = matrix1.M13 + matrix2.M13;
        result.M14 = matrix1.M14 + matrix2.M14;

        result.M21 = matrix1.M21 + matrix2.M21;
        result.M22 = matrix1.M22 + matrix2.M22;
        result.M23 = matrix1.M23 + matrix2.M23;
        result.M24 = matrix1.M24 + matrix2.M24;

        result.M31 = matrix1.M31 + matrix2.M31;
        result.M32 = matrix1.M32 + matrix2.M32;
        result.M33 = matrix1.M33 + matrix2.M33;
        result.M34 = matrix1.M34 + matrix2.M34;

        result.M41 = matrix1.M41 + matrix2.M41;
        result.M42 = matrix1.M42 + matrix2.M42;
        result.M43 = matrix1.M43 + matrix2.M43;
        result.M44 = matrix1.M44 + matrix2.M44;
    }

    /// <summary>
    /// 计算给定矩阵的逆矩阵。
    /// </summary>
    /// <param name="matrix">要计算逆的矩阵。</param>
    /// <returns>逆矩阵。</returns>
    public static FPMatrix4x4 Inverse(FPMatrix4x4 matrix)
    {
        FPMatrix4x4 result;
        Inverse(ref matrix, out result);
        return result;
    }

    public FP determinant
    {
        get
        {
            // | a b c d |     | f g h |     | e g h |     | e f h |     | e f g |
            // | e f g h | = a | j k l | - b | i k l | + c | i j l | - d | i j k |
            // | i j k l |     | n o p |     | m o p |     | m n p |     | m n o |
            // | m n o p |
            //
            //   | f g h |
            // a | j k l | = a ( f ( kp - lo ) - g ( jp - ln ) + h ( jo - kn ) )
            //   | n o p |
            //
            //   | e g h |     
            // b | i k l | = b ( e ( kp - lo ) - g ( ip - lm ) + h ( io - km ) )
            //   | m o p |     
            //
            //   | e f h |
            // c | i j l | = c ( e ( jp - ln ) - f ( ip - lm ) + h ( in - jm ) )
            //   | m n p |
            //
            //   | e f g |
            // d | i j k | = d ( e ( jo - kn ) - f ( io - km ) + g ( in - jm ) )
            //   | m n o |
            //
            // Cost of operation
            // 17 adds and 28 muls.
            //
            // add: 6 + 8 + 3 = 17
            // mul: 12 + 16 = 28

            FP a = M11, b = M12, c = M13, d = M14;
            FP e = M21, f = M22, g = M23, h = M24;
            FP i = M31, j = M32, k = M33, l = M34;
            FP m = M41, n = M42, o = M43, p = M44;

            var kp_lo = k * p - l * o;
            var jp_ln = j * p - l * n;
            var jo_kn = j * o - k * n;
            var ip_lm = i * p - l * m;
            var io_km = i * o - k * m;
            var in_jm = i * n - j * m;

            return a * (f * kp_lo - g * jp_ln + h * jo_kn) -
                   b * (e * kp_lo - g * ip_lm + h * io_km) +
                   c * (e * jp_ln - f * ip_lm + h * in_jm) -
                   d * (e * jo_kn - f * io_km + g * in_jm);
        }
    }

    /// <summary>
    /// 计算给定矩阵的逆矩阵。
    /// </summary>
    /// <param name="matrix">要计算逆的矩阵。</param>
    /// <param name="result">逆矩阵。</param>
    public static void Inverse(ref FPMatrix4x4 matrix, out FPMatrix4x4 result)
    {
        //                                       -1
        // If you have matrix M, inverse Matrix M   can compute
        //
        //     -1       1      
        //    M   = --------- A
        //            det(M)
        //
        // A is adjugate (adjoint) of M, where,
        //
        //      T
        // A = C
        //
        // C is Cofactor matrix of M, where,
        //           i + j
        // C   = (-1)      * det(M  )
        //  ij                    ij
        //
        //     [ a b c d ]
        // M = [ e f g h ]
        //     [ i j k l ]
        //     [ m n o p ]
        //
        // First Row
        //           2 | f g h |
        // C   = (-1)  | j k l | = + ( f ( kp - lo ) - g ( jp - ln ) + h ( jo - kn ) )
        //  11         | n o p |
        //
        //           3 | e g h |
        // C   = (-1)  | i k l | = - ( e ( kp - lo ) - g ( ip - lm ) + h ( io - km ) )
        //  12         | m o p |
        //
        //           4 | e f h |
        // C   = (-1)  | i j l | = + ( e ( jp - ln ) - f ( ip - lm ) + h ( in - jm ) )
        //  13         | m n p |
        //
        //           5 | e f g |
        // C   = (-1)  | i j k | = - ( e ( jo - kn ) - f ( io - km ) + g ( in - jm ) )
        //  14         | m n o |
        //
        // Second Row
        //           3 | b c d |
        // C   = (-1)  | j k l | = - ( b ( kp - lo ) - c ( jp - ln ) + d ( jo - kn ) )
        //  21         | n o p |
        //
        //           4 | a c d |
        // C   = (-1)  | i k l | = + ( a ( kp - lo ) - c ( ip - lm ) + d ( io - km ) )
        //  22         | m o p |
        //
        //           5 | a b d |
        // C   = (-1)  | i j l | = - ( a ( jp - ln ) - b ( ip - lm ) + d ( in - jm ) )
        //  23         | m n p |
        //
        //           6 | a b c |
        // C   = (-1)  | i j k | = + ( a ( jo - kn ) - b ( io - km ) + c ( in - jm ) )
        //  24         | m n o |
        //
        // Third Row
        //           4 | b c d |
        // C   = (-1)  | f g h | = + ( b ( gp - ho ) - c ( fp - hn ) + d ( fo - gn ) )
        //  31         | n o p |
        //
        //           5 | a c d |
        // C   = (-1)  | e g h | = - ( a ( gp - ho ) - c ( ep - hm ) + d ( eo - gm ) )
        //  32         | m o p |
        //
        //           6 | a b d |
        // C   = (-1)  | e f h | = + ( a ( fp - hn ) - b ( ep - hm ) + d ( en - fm ) )
        //  33         | m n p |
        //
        //           7 | a b c |
        // C   = (-1)  | e f g | = - ( a ( fo - gn ) - b ( eo - gm ) + c ( en - fm ) )
        //  34         | m n o |
        //
        // Fourth Row
        //           5 | b c d |
        // C   = (-1)  | f g h | = - ( b ( gl - hk ) - c ( fl - hj ) + d ( fk - gj ) )
        //  41         | j k l |
        //
        //           6 | a c d |
        // C   = (-1)  | e g h | = + ( a ( gl - hk ) - c ( el - hi ) + d ( ek - gi ) )
        //  42         | i k l |
        //
        //           7 | a b d |
        // C   = (-1)  | e f h | = - ( a ( fl - hj ) - b ( el - hi ) + d ( ej - fi ) )
        //  43         | i j l |
        //
        //           8 | a b c |
        // C   = (-1)  | e f g | = + ( a ( fk - gj ) - b ( ek - gi ) + c ( ej - fi ) )
        //  44         | i j k |
        //
        // Cost of operation
        // 53 adds, 104 muls, and 1 div.
        FP a = matrix.M11, b = matrix.M12, c = matrix.M13, d = matrix.M14;
        FP e = matrix.M21, f = matrix.M22, g = matrix.M23, h = matrix.M24;
        FP i = matrix.M31, j = matrix.M32, k = matrix.M33, l = matrix.M34;
        FP m = matrix.M41, n = matrix.M42, o = matrix.M43, p = matrix.M44;

        var kp_lo = k * p - l * o;
        var jp_ln = j * p - l * n;
        var jo_kn = j * o - k * n;
        var ip_lm = i * p - l * m;
        var io_km = i * o - k * m;
        var in_jm = i * n - j * m;

        var a11 = f * kp_lo - g * jp_ln + h * jo_kn;
        var a12 = -(e * kp_lo - g * ip_lm + h * io_km);
        var a13 = e * jp_ln - f * ip_lm + h * in_jm;
        var a14 = -(e * jo_kn - f * io_km + g * in_jm);

        var det = a * a11 + b * a12 + c * a13 + d * a14;

        if (det == FP.Zero)
        {
            result.M11 = FP.PositiveInfinity;
            result.M12 = FP.PositiveInfinity;
            result.M13 = FP.PositiveInfinity;
            result.M14 = FP.PositiveInfinity;
            result.M21 = FP.PositiveInfinity;
            result.M22 = FP.PositiveInfinity;
            result.M23 = FP.PositiveInfinity;
            result.M24 = FP.PositiveInfinity;
            result.M31 = FP.PositiveInfinity;
            result.M32 = FP.PositiveInfinity;
            result.M33 = FP.PositiveInfinity;
            result.M34 = FP.PositiveInfinity;
            result.M41 = FP.PositiveInfinity;
            result.M42 = FP.PositiveInfinity;
            result.M43 = FP.PositiveInfinity;
            result.M44 = FP.PositiveInfinity;
        }
        else
        {
            var invDet = FP.One / det;

            result.M11 = a11 * invDet;
            result.M21 = a12 * invDet;
            result.M31 = a13 * invDet;
            result.M41 = a14 * invDet;

            result.M12 = -(b * kp_lo - c * jp_ln + d * jo_kn) * invDet;
            result.M22 = (a * kp_lo - c * ip_lm + d * io_km) * invDet;
            result.M32 = -(a * jp_ln - b * ip_lm + d * in_jm) * invDet;
            result.M42 = (a * jo_kn - b * io_km + c * in_jm) * invDet;

            var gp_ho = g * p - h * o;
            var fp_hn = f * p - h * n;
            var fo_gn = f * o - g * n;
            var ep_hm = e * p - h * m;
            var eo_gm = e * o - g * m;
            var en_fm = e * n - f * m;

            result.M13 = (b * gp_ho - c * fp_hn + d * fo_gn) * invDet;
            result.M23 = -(a * gp_ho - c * ep_hm + d * eo_gm) * invDet;
            result.M33 = (a * fp_hn - b * ep_hm + d * en_fm) * invDet;
            result.M43 = -(a * fo_gn - b * eo_gm + c * en_fm) * invDet;

            var gl_hk = g * l - h * k;
            var fl_hj = f * l - h * j;
            var fk_gj = f * k - g * j;
            var el_hi = e * l - h * i;
            var ek_gi = e * k - g * i;
            var ej_fi = e * j - f * i;

            result.M14 = -(b * gl_hk - c * fl_hj + d * fk_gj) * invDet;
            result.M24 = (a * gl_hk - c * el_hi + d * ek_gi) * invDet;
            result.M34 = -(a * fl_hj - b * el_hi + d * ej_fi) * invDet;
            result.M44 = (a * fk_gj - b * ek_gi + c * ej_fi) * invDet;
        }
    }

    /// <summary>
    /// 将矩阵乘以一个缩放因子。
    /// </summary>
    /// <param name="matrix1">要缩放的矩阵。</param>
    /// <param name="scaleFactor">缩放因子。</param>
    /// <returns>缩放后的矩阵。</returns>
    public static FPMatrix4x4 Multiply(FPMatrix4x4 matrix1, FP scaleFactor)
    {
        FPMatrix4x4 result;
        Multiply(ref matrix1, scaleFactor, out result);
        return result;
    }

    /// <summary>
    /// 将矩阵乘以一个缩放因子。
    /// </summary>
    /// <param name="matrix1">要缩放的矩阵。</param>
    /// <param name="scaleFactor">缩放因子。</param>
    /// <param name="result">缩放后的矩阵。</param>
    public static void Multiply(ref FPMatrix4x4 matrix1, FP scaleFactor, out FPMatrix4x4 result)
    {
        var num = scaleFactor;
        result.M11 = matrix1.M11 * num;
        result.M12 = matrix1.M12 * num;
        result.M13 = matrix1.M13 * num;
        result.M14 = matrix1.M14 * num;

        result.M21 = matrix1.M21 * num;
        result.M22 = matrix1.M22 * num;
        result.M23 = matrix1.M23 * num;
        result.M24 = matrix1.M24 * num;

        result.M31 = matrix1.M31 * num;
        result.M32 = matrix1.M32 * num;
        result.M33 = matrix1.M33 * num;
        result.M34 = matrix1.M34 * num;

        result.M41 = matrix1.M41 * num;
        result.M42 = matrix1.M42 * num;
        result.M43 = matrix1.M43 * num;
        result.M44 = matrix1.M44 * num;
    }

    /// <summary>
    /// 根据四元数创建旋转矩阵。
    /// </summary>
    /// <param name="quaternion">用于创建矩阵的四元数。</param>
    /// <returns>表示方向的旋转矩阵。</returns>
    public static FPMatrix4x4 Rotate(FPQuaternion quaternion)
    {
        FPMatrix4x4 result;
        Rotate(ref quaternion, out result);
        return result;
    }

    /// <summary>
    /// 根据四元数创建旋转矩阵。
    /// </summary>
    /// <param name="quaternion">用于创建矩阵的四元数。</param>
    /// <param name="result">表示方向的旋转矩阵。</param>
    public static void Rotate(ref FPQuaternion quaternion, out FPMatrix4x4 result)
    {
        // Precalculate coordinate products
        var x = quaternion.x * 2;
        var y = quaternion.y * 2;
        var z = quaternion.z * 2;
        var xx = quaternion.x * x;
        var yy = quaternion.y * y;
        var zz = quaternion.z * z;
        var xy = quaternion.x * y;
        var xz = quaternion.x * z;
        var yz = quaternion.y * z;
        var wx = quaternion.w * x;
        var wy = quaternion.w * y;
        var wz = quaternion.w * z;

        // Calculate 3x3 matrix from orthonormal basis
        result.M11 = FP.One - (yy + zz);
        result.M21 = xy + wz;
        result.M31 = xz - wy;
        result.M41 = FP.Zero;
        result.M12 = xy - wz;
        result.M22 = FP.One - (xx + zz);
        result.M32 = yz + wx;
        result.M42 = FP.Zero;
        result.M13 = xz + wy;
        result.M23 = yz - wx;
        result.M33 = FP.One - (xx + yy);
        result.M43 = FP.Zero;
        result.M14 = FP.Zero;
        result.M24 = FP.Zero;
        result.M34 = FP.Zero;
        result.M44 = FP.One;
    }

    /// <summary>
    /// 创建转置矩阵。
    /// </summary>
    /// <param name="matrix">要转置的矩阵。</param>
    /// <returns>转置后的矩阵。</returns>
    public static FPMatrix4x4 Transpose(FPMatrix4x4 matrix)
    {
        FPMatrix4x4 result;
        Transpose(ref matrix, out result);
        return result;
    }

    /// <summary>
    /// 创建转置矩阵。
    /// </summary>
    /// <param name="matrix">要转置的矩阵。</param>
    /// <param name="result">转置后的矩阵。</param>
    public static void Transpose(ref FPMatrix4x4 matrix, out FPMatrix4x4 result)
    {
        result.M11 = matrix.M11;
        result.M12 = matrix.M21;
        result.M13 = matrix.M31;
        result.M14 = matrix.M41;
        result.M21 = matrix.M12;
        result.M22 = matrix.M22;
        result.M23 = matrix.M32;
        result.M24 = matrix.M42;
        result.M31 = matrix.M13;
        result.M32 = matrix.M23;
        result.M33 = matrix.M33;
        result.M34 = matrix.M43;
        result.M41 = matrix.M14;
        result.M42 = matrix.M24;
        result.M43 = matrix.M34;
        result.M44 = matrix.M44;
    }

    /// <summary>
    /// 重载乘法运算符，计算两个矩阵的乘积。
    /// </summary>
    /// <param name="value1">第一个矩阵。</param>
    /// <param name="value2">第二个矩阵。</param>
    /// <returns>两个矩阵的乘积。</returns>
    public static FPMatrix4x4 operator *(FPMatrix4x4 value1, FPMatrix4x4 value2)
    {
        FPMatrix4x4 result;
        Multiply(ref value1, ref value2, out result);
        return result;
    }

    /// <summary>
    /// 计算矩阵的迹。
    /// </summary>
    /// <returns>矩阵的迹。</returns>
    public FP Trace()
    {
        return M11 + M22 + M33 + M44;
    }

    /// <summary>
    /// 重载加法运算符，计算两个矩阵的和。
    /// </summary>
    /// <param name="value1">第一个矩阵。</param>
    /// <param name="value2">第二个矩阵。</param>
    /// <returns>两个矩阵的和。</returns>
    public static FPMatrix4x4 operator +(FPMatrix4x4 value1, FPMatrix4x4 value2)
    {
        FPMatrix4x4 result;
        Add(ref value1, ref value2, out result);
        return result;
    }

    /// <summary>
    /// 返回给定矩阵的元素取反后的新矩阵。
    /// </summary>
    /// <param name="value">源矩阵。</param>
    /// <returns>取反后的矩阵。</returns>
    public static FPMatrix4x4 operator -(FPMatrix4x4 value)
    {
        FPMatrix4x4 result;

        result.M11 = -value.M11;
        result.M12 = -value.M12;
        result.M13 = -value.M13;
        result.M14 = -value.M14;
        result.M21 = -value.M21;
        result.M22 = -value.M22;
        result.M23 = -value.M23;
        result.M24 = -value.M24;
        result.M31 = -value.M31;
        result.M32 = -value.M32;
        result.M33 = -value.M33;
        result.M34 = -value.M34;
        result.M41 = -value.M41;
        result.M42 = -value.M42;
        result.M43 = -value.M43;
        result.M44 = -value.M44;

        return result;
    }

    /// <summary>
    /// 重载减法运算符，计算两个矩阵的差。
    /// </summary>
    /// <param name="value1">第一个矩阵。</param>
    /// <param name="value2">第二个矩阵。</param>
    /// <returns>两个矩阵的差。</returns>
    public static FPMatrix4x4 operator -(FPMatrix4x4 value1, FPMatrix4x4 value2)
    {
        FPMatrix4x4 result;
        Multiply(ref value2, -FP.One, out value2);
        Add(ref value1, ref value2, out result);
        return result;
    }

    /// <summary>
    /// 重载相等运算符，判断两个矩阵是否相等。
    /// </summary>
    /// <param name="value1">第一个矩阵。</param>
    /// <param name="value2">第二个矩阵。</param>
    /// <returns>如果两个矩阵相等，则返回 true；否则返回 false。</returns>
    public static bool operator ==(FPMatrix4x4 value1, FPMatrix4x4 value2)
    {
        return value1.M11 == value2.M11 &&
               value1.M12 == value2.M12 &&
               value1.M13 == value2.M13 &&
               value1.M14 == value2.M14 &&
               value1.M21 == value2.M21 &&
               value1.M22 == value2.M22 &&
               value1.M23 == value2.M23 &&
               value1.M24 == value2.M24 &&
               value1.M31 == value2.M31 &&
               value1.M32 == value2.M32 &&
               value1.M33 == value2.M33 &&
               value1.M34 == value2.M34 &&
               value1.M41 == value2.M41 &&
               value1.M42 == value2.M42 &&
               value1.M43 == value2.M43 &&
               value1.M44 == value2.M44;
    }

    /// <summary>
    /// 重载不相等运算符，判断两个矩阵是否不相等。
    /// </summary>
    /// <param name="value1">第一个矩阵。</param>
    /// <param name="value2">第二个矩阵。</param>
    /// <returns>如果两个矩阵不相等，则返回 true；否则返回 false。</returns>
    public static bool operator !=(FPMatrix4x4 value1, FPMatrix4x4 value2)
    {
        return value1.M11 != value2.M11 ||
               value1.M12 != value2.M12 ||
               value1.M13 != value2.M13 ||
               value1.M14 != value2.M14 ||
               value1.M21 != value2.M21 ||
               value1.M22 != value2.M22 ||
               value1.M23 != value2.M23 ||
               value1.M24 != value2.M24 ||
               value1.M31 != value2.M31 ||
               value1.M32 != value2.M32 ||
               value1.M33 != value2.M33 ||
               value1.M34 != value2.M34 ||
               value1.M41 != value2.M41 ||
               value1.M42 != value2.M42 ||
               value1.M43 != value2.M43 ||
               value1.M44 != value2.M44;
    }

    /// <summary>
    /// 重写 Equals 方法，判断当前矩阵是否与指定对象相等。
    /// </summary>
    /// <param name="obj">要比较的对象。</param>
    /// <returns>如果当前矩阵与指定对象相等，则返回 true；否则返回 false。</returns>
    public override bool Equals(object obj)
    {
        if (!(obj is FPMatrix4x4))
        {
            return false;
        }

        var other = (FPMatrix4x4)obj;

        return M11 == other.M11 &&
               M12 == other.M12 &&
               M13 == other.M13 &&
               M14 == other.M14 &&
               M21 == other.M21 &&
               M22 == other.M22 &&
               M23 == other.M23 &&
               M24 == other.M24 &&
               M31 == other.M31 &&
               M32 == other.M32 &&
               M33 == other.M33 &&
               M34 == other.M34 &&
               M41 == other.M41 &&
               M42 == other.M42 &&
               M43 == other.M43 &&
               M44 == other.M44;
    }

    /// <summary>
    /// 重写 GetHashCode 方法，返回当前矩阵的哈希代码。
    /// </summary>
    /// <returns>当前矩阵的哈希代码。</returns>
    public override int GetHashCode()
    {
        return M11.GetHashCode() ^
               M12.GetHashCode() ^
               M13.GetHashCode() ^
               M14.GetHashCode() ^
               M21.GetHashCode() ^
               M22.GetHashCode() ^
               M23.GetHashCode() ^
               M24.GetHashCode() ^
               M31.GetHashCode() ^
               M32.GetHashCode() ^
               M33.GetHashCode() ^
               M34.GetHashCode() ^
               M41.GetHashCode() ^
               M42.GetHashCode() ^
               M43.GetHashCode() ^
               M44.GetHashCode();
    }

    /// <summary>
    /// 创建平移矩阵。
    /// </summary>
    /// <param name="xPosition">在 X 轴上的平移量。</param>
    /// <param name="yPosition">在 Y 轴上的平移量。</param>
    /// <param name="zPosition">在 Z 轴上的平移量。</param>
    /// <returns>平移矩阵。</returns>
    public static FPMatrix4x4 Translate(FP xPosition, FP yPosition, FP zPosition)
    {
        FPMatrix4x4 result;

        result.M11 = FP.One;
        result.M12 = FP.Zero;
        result.M13 = FP.Zero;
        result.M14 = xPosition;
        result.M21 = FP.Zero;
        result.M22 = FP.One;
        result.M23 = FP.Zero;
        result.M24 = yPosition;
        result.M31 = FP.Zero;
        result.M32 = FP.Zero;
        result.M33 = FP.One;
        result.M34 = zPosition;
        result.M41 = FP.Zero;
        result.M42 = FP.Zero;
        result.M43 = FP.Zero;
        result.M44 = FP.One;

        return result;
    }

    /// <summary>
    /// 根据三维向量创建平移矩阵。
    /// </summary>
    /// <param name="translation">平移向量。</param>
    /// <returns>平移矩阵。</returns>
    public static FPMatrix4x4 Translate(FPVector3 translation)
    {
        return Translate(translation.x, translation.y, translation.z);
    }

    /// <summary>
    /// 创建缩放矩阵。
    /// </summary>
    /// <param name="xScale">在 X 轴上的缩放值。</param>
    /// <param name="yScale">在 Y 轴上的缩放值。</param>
    /// <param name="zScale">在 Z 轴上的缩放值。</param>
    /// <returns>缩放矩阵。</returns>
    public static FPMatrix4x4 Scale(FP xScale, FP yScale, FP zScale)
    {
        FPMatrix4x4 result;

        result.M11 = xScale;
        result.M12 = FP.Zero;
        result.M13 = FP.Zero;
        result.M14 = FP.Zero;
        result.M21 = FP.Zero;
        result.M22 = yScale;
        result.M23 = FP.Zero;
        result.M24 = FP.Zero;
        result.M31 = FP.Zero;
        result.M32 = FP.Zero;
        result.M33 = zScale;
        result.M34 = FP.Zero;
        result.M41 = FP.Zero;
        result.M42 = FP.Zero;
        result.M43 = FP.Zero;
        result.M44 = FP.One;

        return result;
    }

    /// <summary>
    /// 创建带有中心点的缩放矩阵。
    /// </summary>
    /// <param name="xScale">在 X 轴上的缩放值。</param>
    /// <param name="yScale">在 Y 轴上的缩放值。</param>
    /// <param name="zScale">在 Z 轴上的缩放值。</param>
    /// <param name="centerPoint">缩放的中心点。</param>
    /// <returns>缩放矩阵。</returns>
    public static FPMatrix4x4 Scale(FP xScale, FP yScale, FP zScale, FPVector3 centerPoint)
    {
        FPMatrix4x4 result;

        var tx = centerPoint.x * (FP.One - xScale);
        var ty = centerPoint.y * (FP.One - yScale);
        var tz = centerPoint.z * (FP.One - zScale);

        result.M11 = xScale;
        result.M12 = FP.Zero;
        result.M13 = FP.Zero;
        result.M14 = FP.Zero;
        result.M21 = FP.Zero;
        result.M22 = yScale;
        result.M23 = FP.Zero;
        result.M24 = FP.Zero;
        result.M31 = FP.Zero;
        result.M32 = FP.Zero;
        result.M33 = zScale;
        result.M34 = FP.Zero;
        result.M41 = tx;
        result.M42 = ty;
        result.M43 = tz;
        result.M44 = FP.One;

        return result;
    }

    /// <summary>
    /// 创建缩放矩阵。
    /// </summary>
    /// <param name="scales">包含每个轴的缩放值的向量。</param>
    /// <returns>缩放矩阵。</returns>
    public static FPMatrix4x4 Scale(FPVector3 scales)
    {
        return Scale(scales.x, scales.y, scales.z);
    }

    /// <summary>
    /// 创建带有中心点的缩放矩阵。
    /// </summary>
    /// <param name="scales">包含每个轴的缩放值的向量。</param>
    /// <param name="centerPoint">缩放的中心点。</param>
    /// <returns>缩放矩阵。</returns>
    public static FPMatrix4x4 Scale(FPVector3 scales, FPVector3 centerPoint)
    {
        return Scale(scales.x, scales.y, scales.z, centerPoint);
    }

    /// <summary>
    /// 创建均匀缩放矩阵，使每个轴的缩放相等。
    /// </summary>
    /// <param name="scale">均匀缩放因子。</param>
    /// <returns>缩放矩阵。</returns>
    public static FPMatrix4x4 Scale(FP scale)
    {
        return Scale(scale, scale, scale);
    }

    /// <summary>
    /// 创建均匀缩放矩阵，使每个轴的缩放相等，并指定中心点。
    /// </summary>
    /// <param name="scale">均匀缩放因子。</param>
    /// <param name="centerPoint">缩放的中心点。</param>
    /// <returns>缩放矩阵。</returns>
    public static FPMatrix4x4 Scale(FP scale, FPVector3 centerPoint)
    {
        return Scale(scale, scale, scale, centerPoint);
    }

    /// <summary>
    /// 创建围绕 X 轴旋转的矩阵。
    /// </summary>
    /// <param name="radians">围绕 X 轴旋转的弧度。</param>
    /// <returns>旋转矩阵。</returns>
    public static FPMatrix4x4 RotateX(FP radians)
    {
        FPMatrix4x4 result;

        var c = FPMath.Cos(radians);
        var s = FPMath.Sin(radians);

        // [  1  0  0  0 ]
        // [  0  c  s  0 ]
        // [  0 -s  c  0 ]
        // [  0  0  0  1 ]
        result.M11 = FP.One;
        result.M12 = FP.Zero;
        result.M13 = FP.Zero;
        result.M14 = FP.Zero;
        result.M21 = FP.Zero;
        result.M22 = c;
        result.M23 = s;
        result.M24 = FP.Zero;
        result.M31 = FP.Zero;
        result.M32 = -s;
        result.M33 = c;
        result.M34 = FP.Zero;
        result.M41 = FP.Zero;
        result.M42 = FP.Zero;
        result.M43 = FP.Zero;
        result.M44 = FP.One;

        return result;
    }

    /// <summary>
    /// 创建围绕 X 轴旋转的矩阵，并指定中心点。
    /// </summary>
    /// <param name="radians">围绕 X 轴旋转的弧度。</param>
    /// <param name="centerPoint">旋转的中心点。</param>
    /// <returns>旋转矩阵。</returns>
    public static FPMatrix4x4 RotateX(FP radians, FPVector3 centerPoint)
    {
        FPMatrix4x4 result;

        var c = FPMath.Cos(radians);
        var s = FPMath.Sin(radians);

        var y = centerPoint.y * (FP.One - c) + centerPoint.z * s;
        var z = centerPoint.z * (FP.One - c) - centerPoint.y * s;

        // [  1  0  0  0 ]
        // [  0  c  s  0 ]
        // [  0 -s  c  0 ]
        // [  0  y  z  1 ]
        result.M11 = FP.One;
        result.M12 = FP.Zero;
        result.M13 = FP.Zero;
        result.M14 = FP.Zero;
        result.M21 = FP.Zero;
        result.M22 = c;
        result.M23 = s;
        result.M24 = FP.Zero;
        result.M31 = FP.Zero;
        result.M32 = -s;
        result.M33 = c;
        result.M34 = FP.Zero;
        result.M41 = FP.Zero;
        result.M42 = y;
        result.M43 = z;
        result.M44 = FP.One;

        return result;
    }

    /// <summary>
    /// 创建围绕 Y 轴旋转的矩阵。
    /// </summary>
    /// <param name="radians">围绕 Y 轴旋转的弧度。</param>
    /// <returns>旋转矩阵。</returns>
    public static FPMatrix4x4 RotateY(FP radians)
    {
        FPMatrix4x4 result;

        var c = FPMath.Cos(radians);
        var s = FPMath.Sin(radians);

        // [  c  0 -s  0 ]
        // [  0  1  0  0 ]
        // [  s  0  c  0 ]
        // [  0  0  0  1 ]
        result.M11 = c;
        result.M12 = FP.Zero;
        result.M13 = -s;
        result.M14 = FP.Zero;
        result.M21 = FP.Zero;
        result.M22 = FP.One;
        result.M23 = FP.Zero;
        result.M24 = FP.Zero;
        result.M31 = s;
        result.M32 = FP.Zero;
        result.M33 = c;
        result.M34 = FP.Zero;
        result.M41 = FP.Zero;
        result.M42 = FP.Zero;
        result.M43 = FP.Zero;
        result.M44 = FP.One;

        return result;
    }

    /// <summary>
    /// 创建围绕 Y 轴旋转的矩阵，并指定中心点。
    /// </summary>
    /// <param name="radians">围绕 Y 轴旋转的弧度。</param>
    /// <param name="centerPoint">旋转的中心点。</param>
    /// <returns>旋转矩阵。</returns>
    public static FPMatrix4x4 RotateY(FP radians, FPVector3 centerPoint)
    {
        FPMatrix4x4 result;

        var c = FPMath.Cos(radians);
        var s = FPMath.Sin(radians);

        var x = centerPoint.x * (FP.One - c) - centerPoint.z * s;
        var z = centerPoint.x * (FP.One - c) + centerPoint.x * s;

        // [  c  0 -s  0 ]
        // [  0  1  0  0 ]
        // [  s  0  c  0 ]
        // [  x  0  z  1 ]
        result.M11 = c;
        result.M12 = FP.Zero;
        result.M13 = -s;
        result.M14 = FP.Zero;
        result.M21 = FP.Zero;
        result.M22 = FP.One;
        result.M23 = FP.Zero;
        result.M24 = FP.Zero;
        result.M31 = s;
        result.M32 = FP.Zero;
        result.M33 = c;
        result.M34 = FP.Zero;
        result.M41 = x;
        result.M42 = FP.Zero;
        result.M43 = z;
        result.M44 = FP.One;

        return result;
    }

    /// <summary>
    /// 创建围绕 Z 轴旋转的矩阵。
    /// </summary>
    /// <param name="radians">围绕 Z 轴旋转的弧度。</param>
    /// <returns>旋转矩阵。</returns>
    public static FPMatrix4x4 RotateZ(FP radians)
    {
        FPMatrix4x4 result;

        var c = FPMath.Cos(radians);
        var s = FPMath.Sin(radians);

        // [  c  s  0  0 ]
        // [ -s  c  0  0 ]
        // [  0  0  1  0 ]
        // [  0  0  0  1 ]
        result.M11 = c;
        result.M12 = s;
        result.M13 = FP.Zero;
        result.M14 = FP.Zero;
        result.M21 = -s;
        result.M22 = c;
        result.M23 = FP.Zero;
        result.M24 = FP.Zero;
        result.M31 = FP.Zero;
        result.M32 = FP.Zero;
        result.M33 = FP.One;
        result.M34 = FP.Zero;
        result.M41 = FP.Zero;
        result.M42 = FP.Zero;
        result.M43 = FP.Zero;
        result.M44 = FP.One;

        return result;
    }

    /// <summary>
    /// 创建围绕 Z 轴旋转的矩阵，并指定中心点。
    /// </summary>
    /// <param name="radians">围绕 Z 轴旋转的弧度。</param>
    /// <param name="centerPoint">旋转的中心点。</param>
    /// <returns>旋转矩阵。</returns>
    public static FPMatrix4x4 RotateZ(FP radians, FPVector3 centerPoint)
    {
        FPMatrix4x4 result;

        var c = FPMath.Cos(radians);
        var s = FPMath.Sin(radians);

        var x = centerPoint.x * (1 - c) + centerPoint.y * s;
        var y = centerPoint.y * (1 - c) - centerPoint.x * s;

        // [  c  s  0  0 ]
        // [ -s  c  0  0 ]
        // [  0  0  1  0 ]
        // [  x  y  0  1 ]
        result.M11 = c;
        result.M12 = s;
        result.M13 = FP.Zero;
        result.M14 = FP.Zero;
        result.M21 = -s;
        result.M22 = c;
        result.M23 = FP.Zero;
        result.M24 = FP.Zero;
        result.M31 = FP.Zero;
        result.M32 = FP.Zero;
        result.M33 = FP.One;
        result.M34 = FP.Zero;
        result.M41 = FP.Zero;
        result.M42 = FP.Zero;
        result.M43 = FP.Zero;
        result.M44 = FP.One;

        return result;
    }

    /// <summary>
    /// 创建围绕给定轴旋转的矩阵。
    /// </summary>
    /// <param name="axis">旋转轴。</param>
    /// <param name="angle">旋转角度。</param>
    /// <param name="result">结果旋转矩阵。</param>
    public static void AxisAngle(ref FPVector3 axis, FP angle, out FPMatrix4x4 result)
    {
        // a: 角度
        // x, y, z: 轴的单位向量。
        //
        // 旋转矩阵 M 可以通过以下公式计算。
        //
        //        T               T
        //  M = uu + (cos a)( I-uu ) + (sin a)S
        //
        // 其中：
        //
        //  u = ( x, y, z )
        //
        //      [  0 -z  y ]
        //  S = [  z  0 -x ]
        //      [ -y  x  0 ]
        //
        //      [ 1 0 0 ]
        //  I = [ 0 1 0 ]
        //      [ 0 0 1 ]
        //
        //
        //     [  xx+cosa*(1-xx)   yx-cosa*yx-sina*z zx-cosa*xz+sina*y ]
        // M = [ xy-cosa*yx+sina*z    yy+cosa(1-yy)  yz-cosa*yz-sina*x ]
        //     [ zx-cosa*zx-sina*y zy-cosa*zy+sina*x   zz+cosa*(1-zz)  ]
        //
        FP x = axis.x, y = axis.y, z = axis.z;
        FP sa = FPMath.Sin(angle), ca = FPMath.Cos(angle);
        FP xx = x * x, yy = y * y, zz = z * z;
        FP xy = x * y, xz = x * z, yz = y * z;

        result.M11 = xx + ca * (FP.One - xx);
        result.M12 = xy - ca * xy + sa * z;
        result.M13 = xz - ca * xz - sa * y;
        result.M14 = FP.Zero;
        result.M21 = xy - ca * xy - sa * z;
        result.M22 = yy + ca * (FP.One - yy);
        result.M23 = yz - ca * yz + sa * x;
        result.M24 = FP.Zero;
        result.M31 = xz - ca * xz + sa * y;
        result.M32 = yz - ca * yz - sa * x;
        result.M33 = zz + ca * (FP.One - zz);
        result.M34 = FP.Zero;
        result.M41 = FP.Zero;
        result.M42 = FP.Zero;
        result.M43 = FP.Zero;
        result.M44 = FP.One;
    }

    /// <summary>
    /// 创建围绕给定轴旋转的矩阵。
    /// </summary>
    /// <param name="axis">旋转轴。</param>
    /// <param name="angle">旋转角度。</param>
    /// <returns>结果旋转矩阵。</returns>
    public static FPMatrix4x4 AngleAxis(FP angle, FPVector3 axis)
    {
        FPMatrix4x4 result;
        AxisAngle(ref axis, angle, out result);
        return result;
    }

    /// <summary>
    /// 返回矩阵的字符串表示形式。
    /// </summary>
    /// <returns>矩阵的字符串表示形式。</returns>
    public override string ToString()
    {
        return string.Format("{0}|{1}|{2}|{3}|{4}|{5}|{6}|{7}|{8}|{9}|{10}|{11}|{12}|{13}|{14}|{15}",
                             M11.RawValue, M12.RawValue, M13.RawValue, M14.RawValue,
                             M21.RawValue, M22.RawValue, M23.RawValue, M24.RawValue,
                             M31.RawValue, M32.RawValue, M33.RawValue, M34.RawValue,
                             M41.RawValue, M42.RawValue, M43.RawValue, M44.RawValue);
    }

    /// <summary>
    /// 创建平移、旋转和缩放的组合矩阵。
    /// </summary>
    /// <param name="translation">平移向量。</param>
    /// <param name="rotation">旋转四元数。</param>
    /// <param name="scale">缩放向量。</param>
    /// <param name="matrix">组合后的矩阵。</param>
    public static void TRS(FPVector3 translation, FPQuaternion rotation, FPVector3 scale, out FPMatrix4x4 matrix)
    {
        matrix = Translate(translation) * Rotate(rotation) * Scale(scale);
    }

    /// <summary>
    /// 创建平移、旋转和缩放的组合矩阵。
    /// </summary>
    /// <param name="translation">平移向量。</param>
    /// <param name="rotation">旋转四元数。</param>
    /// <param name="scale">缩放向量。</param>
    /// <returns>组合后的矩阵。</returns>
    public static FPMatrix4x4 TRS(FPVector3 translation, FPQuaternion rotation, FPVector3 scale)
    {
        FPMatrix4x4 result;
        TRS(translation, rotation, scale, out result);
        return result;
    }
}