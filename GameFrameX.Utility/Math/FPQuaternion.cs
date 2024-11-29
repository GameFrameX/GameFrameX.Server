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
/// A Quaternion representing an orientation.
/// </summary>
[Serializable]
public struct FPQuaternion
{
    /// <summary>The X component of the quaternion.</summary>
    public GameFrameX.Utility.Math.FP x;

    /// <summary>The Y component of the quaternion.</summary>
    public GameFrameX.Utility.Math.FP y;

    /// <summary>The Z component of the quaternion.</summary>
    public GameFrameX.Utility.Math.FP z;

    /// <summary>The W component of the quaternion.</summary>
    public GameFrameX.Utility.Math.FP w;

    public static readonly FPQuaternion identity;

    static FPQuaternion()
    {
        identity = new FPQuaternion(0, 0, 0, 1);
    }

    /// <summary>
    /// Initializes a new instance of the JQuaternion structure.
    /// </summary>
    /// <param name="x">The X component of the quaternion.</param>
    /// <param name="y">The Y component of the quaternion.</param>
    /// <param name="z">The Z component of the quaternion.</param>
    /// <param name="w">The W component of the quaternion.</param>
    public FPQuaternion(GameFrameX.Utility.Math.FP x, GameFrameX.Utility.Math.FP y, GameFrameX.Utility.Math.FP z, GameFrameX.Utility.Math.FP w)
    {
        this.x = x;
        this.y = y;
        this.z = z;
        this.w = w;
    }

    public void Set(GameFrameX.Utility.Math.FP new_x, GameFrameX.Utility.Math.FP new_y, GameFrameX.Utility.Math.FP new_z, GameFrameX.Utility.Math.FP new_w)
    {
        x = new_x;
        y = new_y;
        z = new_z;
        w = new_w;
    }

    public void SetFromToRotation(FPVector3 fromDirection, FPVector3 toDirection)
    {
        FPQuaternion targetRotation = FromToRotation(fromDirection, toDirection);
        Set(targetRotation.x, targetRotation.y, targetRotation.z, targetRotation.w);
    }

    public FPVector3 eulerAngles
    {
        get
        {
            FPVector3 result = new FPVector3();

            GameFrameX.Utility.Math.FP ysqr = y * y;
            GameFrameX.Utility.Math.FP t0 = -2 * GameFrameX.Utility.Math.FP.One * (ysqr + z * z) + GameFrameX.Utility.Math.FP.One;
            GameFrameX.Utility.Math.FP t1 = +2 * GameFrameX.Utility.Math.FP.One * (x * y - w * z);
            GameFrameX.Utility.Math.FP t2 = -2 * GameFrameX.Utility.Math.FP.One * (x * z + w * y);
            GameFrameX.Utility.Math.FP t3 = +2 * GameFrameX.Utility.Math.FP.One * (y * z - w * x);
            GameFrameX.Utility.Math.FP t4 = -2 * GameFrameX.Utility.Math.FP.One * (x * x + ysqr) + GameFrameX.Utility.Math.FP.One;

            t2 = t2 > GameFrameX.Utility.Math.FP.One ? GameFrameX.Utility.Math.FP.One : t2;
            t2 = t2 < -GameFrameX.Utility.Math.FP.One ? -GameFrameX.Utility.Math.FP.One : t2;

            result.x = GameFrameX.Utility.Math.FP.Atan2(t3, t4) * GameFrameX.Utility.Math.FP.Rad2Deg;
            result.y = GameFrameX.Utility.Math.FP.Asin(t2) * GameFrameX.Utility.Math.FP.Rad2Deg;
            result.z = GameFrameX.Utility.Math.FP.Atan2(t1, t0) * GameFrameX.Utility.Math.FP.Rad2Deg;

            return result * -1;
        }
    }

    public FPVector3 eulerAnglesNew
    {
        get
        {
            GameFrameX.Utility.Math.FP r11 = 2 * (x * z + w * y);
            GameFrameX.Utility.Math.FP r12 = w * w - x * x - y * y + z * z;
            GameFrameX.Utility.Math.FP r21 = -2 * (y * z - w * x);
            GameFrameX.Utility.Math.FP r31 = 2 * (x * y + w * z);
            GameFrameX.Utility.Math.FP r32 = w * w - x * x + y * y - z * z;
            return new FPVector3(GameFrameX.Utility.Math.FP.Atan2(r11, r12), GameFrameX.Utility.Math.FP.Asin(r21), GameFrameX.Utility.Math.FP.Atan2(r31, r32));
        }
    }

    public static GameFrameX.Utility.Math.FP Angle(FPQuaternion a, FPQuaternion b)
    {
        FPQuaternion aInv = Inverse(a);
        FPQuaternion f = b * aInv;

        GameFrameX.Utility.Math.FP angle = GameFrameX.Utility.Math.FP.Acos(f.w) * 2 * GameFrameX.Utility.Math.FP.Rad2Deg;

        if (angle > 180)
        {
            angle = 360 - angle;
        }

        return angle;
    }

    /// <summary>
    /// Quaternions are added.
    /// </summary>
    /// <param name="quaternion1">The first quaternion.</param>
    /// <param name="quaternion2">The second quaternion.</param>
    /// <returns>The sum of both quaternions.</returns>

    #region public static JQuaternion Add(JQuaternion quaternion1, JQuaternion quaternion2)

    public static FPQuaternion Add(FPQuaternion quaternion1, FPQuaternion quaternion2)
    {
        FPQuaternion result;
        Add(ref quaternion1, ref quaternion2, out result);
        return result;
    }

    public static FPQuaternion LookRotation(FPVector3 forward)
    {
        return CreateFromMatrix(FPMatrix.LookAt(forward, FPVector3.up));
    }

    public static FPQuaternion LookRotation(FPVector3 forward, FPVector3 upwards)
    {
        return CreateFromMatrix(FPMatrix.LookAt(forward, upwards));
    }

    public static FPQuaternion Slerp(FPQuaternion from, FPQuaternion to, GameFrameX.Utility.Math.FP t)
    {
        t = FPMath.Clamp(t, 0, 1);

        GameFrameX.Utility.Math.FP dot = Dot(from, to);

        if (dot < GameFrameX.Utility.Math.FP.Zero)
        {
            to = Multiply(to, -1);
            dot = -dot;
        }

        GameFrameX.Utility.Math.FP halfTheta = GameFrameX.Utility.Math.FP.Acos(dot);

        return Multiply(Multiply(from, GameFrameX.Utility.Math.FP.Sin((1 - t) * halfTheta)) + Multiply(to, GameFrameX.Utility.Math.FP.Sin(t * halfTheta)), 1 / GameFrameX.Utility.Math.FP.Sin(halfTheta));
    }

    public static FPQuaternion RotateTowards(FPQuaternion from, FPQuaternion to, GameFrameX.Utility.Math.FP maxDegreesDelta)
    {
        GameFrameX.Utility.Math.FP dot = Dot(from, to);

        if (dot < GameFrameX.Utility.Math.FP.Zero)
        {
            to = Multiply(to, -1);
            dot = -dot;
        }

        GameFrameX.Utility.Math.FP halfTheta = GameFrameX.Utility.Math.FP.Acos(dot);
        GameFrameX.Utility.Math.FP theta = halfTheta * 2;

        maxDegreesDelta *= GameFrameX.Utility.Math.FP.Deg2Rad;

        if (maxDegreesDelta >= theta)
        {
            return to;
        }

        maxDegreesDelta /= theta;

        return Multiply(Multiply(from, GameFrameX.Utility.Math.FP.Sin((1 - maxDegreesDelta) * halfTheta)) + Multiply(to, GameFrameX.Utility.Math.FP.Sin(maxDegreesDelta * halfTheta)), 1 / GameFrameX.Utility.Math.FP.Sin(halfTheta));
    }

    public static FPQuaternion Euler(GameFrameX.Utility.Math.FP x, GameFrameX.Utility.Math.FP y, GameFrameX.Utility.Math.FP z)
    {
        x *= GameFrameX.Utility.Math.FP.Deg2Rad;
        y *= GameFrameX.Utility.Math.FP.Deg2Rad;
        z *= GameFrameX.Utility.Math.FP.Deg2Rad;

        FPQuaternion rotation;
        CreateFromYawPitchRoll(y, x, z, out rotation);

        return rotation;
    }

    public static FPQuaternion Euler(FPVector3 eulerAngles)
    {
        return Euler(eulerAngles.x, eulerAngles.y, eulerAngles.z);
    }

    public static FPQuaternion AngleAxis(GameFrameX.Utility.Math.FP angle, FPVector3 axis)
    {
        axis = axis * GameFrameX.Utility.Math.FP.Deg2Rad;
        axis.Normalize();

        GameFrameX.Utility.Math.FP halfAngle = angle * GameFrameX.Utility.Math.FP.Deg2Rad * GameFrameX.Utility.Math.FP.Half;

        FPQuaternion rotation;
        GameFrameX.Utility.Math.FP sin = GameFrameX.Utility.Math.FP.Sin(halfAngle);

        rotation.x = axis.x * sin;
        rotation.y = axis.y * sin;
        rotation.z = axis.z * sin;
        rotation.w = GameFrameX.Utility.Math.FP.Cos(halfAngle);

        return rotation;
    }

    public static void CreateFromYawPitchRoll(GameFrameX.Utility.Math.FP yaw, GameFrameX.Utility.Math.FP pitch, GameFrameX.Utility.Math.FP roll, out FPQuaternion result)
    {
        GameFrameX.Utility.Math.FP num9 = roll * GameFrameX.Utility.Math.FP.Half;
        GameFrameX.Utility.Math.FP num6 = GameFrameX.Utility.Math.FP.Sin(num9);
        GameFrameX.Utility.Math.FP num5 = GameFrameX.Utility.Math.FP.Cos(num9);
        GameFrameX.Utility.Math.FP num8 = pitch * GameFrameX.Utility.Math.FP.Half;
        GameFrameX.Utility.Math.FP num4 = GameFrameX.Utility.Math.FP.Sin(num8);
        GameFrameX.Utility.Math.FP num3 = GameFrameX.Utility.Math.FP.Cos(num8);
        GameFrameX.Utility.Math.FP num7 = yaw * GameFrameX.Utility.Math.FP.Half;
        GameFrameX.Utility.Math.FP num2 = GameFrameX.Utility.Math.FP.Sin(num7);
        GameFrameX.Utility.Math.FP num = GameFrameX.Utility.Math.FP.Cos(num7);
        result.x = ((num * num4) * num5) + ((num2 * num3) * num6);
        result.y = ((num2 * num3) * num5) - ((num * num4) * num6);
        result.z = ((num * num3) * num6) - ((num2 * num4) * num5);
        result.w = ((num * num3) * num5) + ((num2 * num4) * num6);
    }

    /// <summary>
    /// Quaternions are added.
    /// </summary>
    /// <param name="quaternion1">The first quaternion.</param>
    /// <param name="quaternion2">The second quaternion.</param>
    /// <param name="result">The sum of both quaternions.</param>
    public static void Add(ref FPQuaternion quaternion1, ref FPQuaternion quaternion2, out FPQuaternion result)
    {
        result.x = quaternion1.x + quaternion2.x;
        result.y = quaternion1.y + quaternion2.y;
        result.z = quaternion1.z + quaternion2.z;
        result.w = quaternion1.w + quaternion2.w;
    }

    #endregion

    public static FPQuaternion Conjugate(FPQuaternion value)
    {
        FPQuaternion quaternion;
        quaternion.x = -value.x;
        quaternion.y = -value.y;
        quaternion.z = -value.z;
        quaternion.w = value.w;
        return quaternion;
    }

    public static GameFrameX.Utility.Math.FP Dot(FPQuaternion a, FPQuaternion b)
    {
        return a.w * b.w + a.x * b.x + a.y * b.y + a.z * b.z;
    }

    public static FPQuaternion Inverse(FPQuaternion rotation)
    {
        GameFrameX.Utility.Math.FP invNorm = GameFrameX.Utility.Math.FP.One / ((rotation.x * rotation.x) + (rotation.y * rotation.y) + (rotation.z * rotation.z) + (rotation.w * rotation.w));
        return Multiply(Conjugate(rotation), invNorm);
    }

    public static FPQuaternion FromToRotation(FPVector3 fromVector3, FPVector3 toVector3)
    {
        FPVector3 w = FPVector3.Cross(fromVector3, toVector3);
        FPQuaternion q = new FPQuaternion(w.x, w.y, w.z, FPVector3.Dot(fromVector3, toVector3));
        q.w += GameFrameX.Utility.Math.FP.Sqrt(fromVector3.sqrMagnitude * toVector3.sqrMagnitude);
        q.Normalize();

        return q;
    }

    public static FPQuaternion Lerp(FPQuaternion a, FPQuaternion b, GameFrameX.Utility.Math.FP t)
    {
        t = FPMath.Clamp(t, GameFrameX.Utility.Math.FP.Zero, GameFrameX.Utility.Math.FP.One);

        return LerpUnclamped(a, b, t);
    }

    public static FPQuaternion LerpUnclamped(FPQuaternion a, FPQuaternion b, GameFrameX.Utility.Math.FP t)
    {
        FPQuaternion result = Multiply(a, (1 - t)) + Multiply(b, t);
        result.Normalize();

        return result;
    }

    /// <summary>
    /// Quaternions are subtracted.
    /// </summary>
    /// <param name="quaternion1">The first quaternion.</param>
    /// <param name="quaternion2">The second quaternion.</param>
    /// <returns>The difference of both quaternions.</returns>

    #region public static JQuaternion Subtract(JQuaternion quaternion1, JQuaternion quaternion2)

    public static FPQuaternion Subtract(FPQuaternion quaternion1, FPQuaternion quaternion2)
    {
        FPQuaternion result;
        Subtract(ref quaternion1, ref quaternion2, out result);
        return result;
    }

    /// <summary>
    /// Quaternions are subtracted.
    /// </summary>
    /// <param name="quaternion1">The first quaternion.</param>
    /// <param name="quaternion2">The second quaternion.</param>
    /// <param name="result">The difference of both quaternions.</param>
    public static void Subtract(ref FPQuaternion quaternion1, ref FPQuaternion quaternion2, out FPQuaternion result)
    {
        result.x = quaternion1.x - quaternion2.x;
        result.y = quaternion1.y - quaternion2.y;
        result.z = quaternion1.z - quaternion2.z;
        result.w = quaternion1.w - quaternion2.w;
    }

    #endregion

    /// <summary>
    /// Multiply two quaternions.
    /// </summary>
    /// <param name="quaternion1">The first quaternion.</param>
    /// <param name="quaternion2">The second quaternion.</param>
    /// <returns>The product of both quaternions.</returns>

    #region public static JQuaternion Multiply(JQuaternion quaternion1, JQuaternion quaternion2)

    public static FPQuaternion Multiply(FPQuaternion quaternion1, FPQuaternion quaternion2)
    {
        FPQuaternion result;
        Multiply(ref quaternion1, ref quaternion2, out result);
        return result;
    }

    /// <summary>
    /// Multiply two quaternions.
    /// </summary>
    /// <param name="quaternion1">The first quaternion.</param>
    /// <param name="quaternion2">The second quaternion.</param>
    /// <param name="result">The product of both quaternions.</param>
    public static void Multiply(ref FPQuaternion quaternion1, ref FPQuaternion quaternion2, out FPQuaternion result)
    {
        GameFrameX.Utility.Math.FP x = quaternion1.x;
        GameFrameX.Utility.Math.FP y = quaternion1.y;
        GameFrameX.Utility.Math.FP z = quaternion1.z;
        GameFrameX.Utility.Math.FP w = quaternion1.w;
        GameFrameX.Utility.Math.FP num4 = quaternion2.x;
        GameFrameX.Utility.Math.FP num3 = quaternion2.y;
        GameFrameX.Utility.Math.FP num2 = quaternion2.z;
        GameFrameX.Utility.Math.FP num = quaternion2.w;
        GameFrameX.Utility.Math.FP num12 = (y * num2) - (z * num3);
        GameFrameX.Utility.Math.FP num11 = (z * num4) - (x * num2);
        GameFrameX.Utility.Math.FP num10 = (x * num3) - (y * num4);
        GameFrameX.Utility.Math.FP num9 = ((x * num4) + (y * num3)) + (z * num2);
        result.x = ((x * num) + (num4 * w)) + num12;
        result.y = ((y * num) + (num3 * w)) + num11;
        result.z = ((z * num) + (num2 * w)) + num10;
        result.w = (w * num) - num9;
    }

    #endregion

    /// <summary>
    /// Scale a quaternion
    /// </summary>
    /// <param name="quaternion1">The quaternion to scale.</param>
    /// <param name="scaleFactor">Scale factor.</param>
    /// <returns>The scaled quaternion.</returns>

    #region public static JQuaternion Multiply(JQuaternion quaternion1, FP scaleFactor)

    public static FPQuaternion Multiply(FPQuaternion quaternion1, GameFrameX.Utility.Math.FP scaleFactor)
    {
        FPQuaternion result;
        Multiply(ref quaternion1, scaleFactor, out result);
        return result;
    }

    /// <summary>
    /// Scale a quaternion
    /// </summary>
    /// <param name="quaternion1">The quaternion to scale.</param>
    /// <param name="scaleFactor">Scale factor.</param>
    /// <param name="result">The scaled quaternion.</param>
    public static void Multiply(ref FPQuaternion quaternion1, GameFrameX.Utility.Math.FP scaleFactor, out FPQuaternion result)
    {
        result.x = quaternion1.x * scaleFactor;
        result.y = quaternion1.y * scaleFactor;
        result.z = quaternion1.z * scaleFactor;
        result.w = quaternion1.w * scaleFactor;
    }

    #endregion

    /// <summary>
    /// Sets the length of the quaternion to one.
    /// </summary>

    #region public void Normalize()

    public void Normalize()
    {
        GameFrameX.Utility.Math.FP num2 = (((x * x) + (y * y)) + (z * z)) + (w * w);
        GameFrameX.Utility.Math.FP num = 1 / (GameFrameX.Utility.Math.FP.Sqrt(num2));
        x *= num;
        y *= num;
        z *= num;
        w *= num;
    }

    #endregion

    public static FPQuaternion CreateFromAxisAngle(FPVector3 axis, GameFrameX.Utility.Math.FP angle)
    {
        // 确保旋转轴是单位向量
        axis = axis.normalized;

        // 将角度减半并计算正弦和余弦值
        GameFrameX.Utility.Math.FP halfAngle = angle * GameFrameX.Utility.Math.FP.Half;
        GameFrameX.Utility.Math.FP sinHalfAngle = FPMath.Sin(halfAngle);
        GameFrameX.Utility.Math.FP cosHalfAngle = FPMath.Cos(halfAngle);

        // 使用公式生成四元数
        return new FPQuaternion(
            axis.x * sinHalfAngle,
            axis.y * sinHalfAngle,
            axis.z * sinHalfAngle,
            cosHalfAngle
        );
    }

    /// <summary>
    /// Creates a quaternion from a matrix.
    /// </summary>
    /// <param name="matrix">A matrix representing an orientation.</param>
    /// <returns>JQuaternion representing an orientation.</returns>

    #region public static JQuaternion CreateFromMatrix(JMatrix matrix)

    public static FPQuaternion CreateFromMatrix(FPMatrix matrix)
    {
        FPQuaternion result;
        CreateFromMatrix(ref matrix, out result);
        return result;
    }

    /// <summary>
    /// Creates a quaternion from a matrix.
    /// </summary>
    /// <param name="matrix">A matrix representing an orientation.</param>
    /// <param name="result">JQuaternion representing an orientation.</param>
    public static void CreateFromMatrix(ref FPMatrix matrix, out FPQuaternion result)
    {
        GameFrameX.Utility.Math.FP num8 = (matrix.M11 + matrix.M22) + matrix.M33;
        if (num8 > GameFrameX.Utility.Math.FP.Zero)
        {
            GameFrameX.Utility.Math.FP num = GameFrameX.Utility.Math.FP.Sqrt((num8 + GameFrameX.Utility.Math.FP.One));
            result.w = num * GameFrameX.Utility.Math.FP.Half;
            num = GameFrameX.Utility.Math.FP.Half / num;
            result.x = (matrix.M23 - matrix.M32) * num;
            result.y = (matrix.M31 - matrix.M13) * num;
            result.z = (matrix.M12 - matrix.M21) * num;
        }
        else if ((matrix.M11 >= matrix.M22) && (matrix.M11 >= matrix.M33))
        {
            GameFrameX.Utility.Math.FP num7 = GameFrameX.Utility.Math.FP.Sqrt((((GameFrameX.Utility.Math.FP.One + matrix.M11) - matrix.M22) - matrix.M33));
            GameFrameX.Utility.Math.FP num4 = GameFrameX.Utility.Math.FP.Half / num7;
            result.x = GameFrameX.Utility.Math.FP.Half * num7;
            result.y = (matrix.M12 + matrix.M21) * num4;
            result.z = (matrix.M13 + matrix.M31) * num4;
            result.w = (matrix.M23 - matrix.M32) * num4;
        }
        else if (matrix.M22 > matrix.M33)
        {
            GameFrameX.Utility.Math.FP num6 = GameFrameX.Utility.Math.FP.Sqrt((((GameFrameX.Utility.Math.FP.One + matrix.M22) - matrix.M11) - matrix.M33));
            GameFrameX.Utility.Math.FP num3 = GameFrameX.Utility.Math.FP.Half / num6;
            result.x = (matrix.M21 + matrix.M12) * num3;
            result.y = GameFrameX.Utility.Math.FP.Half * num6;
            result.z = (matrix.M32 + matrix.M23) * num3;
            result.w = (matrix.M31 - matrix.M13) * num3;
        }
        else
        {
            GameFrameX.Utility.Math.FP num5 = GameFrameX.Utility.Math.FP.Sqrt((((GameFrameX.Utility.Math.FP.One + matrix.M33) - matrix.M11) - matrix.M22));
            GameFrameX.Utility.Math.FP num2 = GameFrameX.Utility.Math.FP.Half / num5;
            result.x = (matrix.M31 + matrix.M13) * num2;
            result.y = (matrix.M32 + matrix.M23) * num2;
            result.z = GameFrameX.Utility.Math.FP.Half * num5;
            result.w = (matrix.M12 - matrix.M21) * num2;
        }
    }

    #endregion

    /// <summary>
    /// Multiply two quaternions.
    /// </summary>
    /// <param name="value1">The first quaternion.</param>
    /// <param name="value2">The second quaternion.</param>
    /// <returns>The product of both quaternions.</returns>

    #region public static FP operator *(JQuaternion value1, JQuaternion value2)

    public static FPQuaternion operator *(FPQuaternion value1, FPQuaternion value2)
    {
        FPQuaternion result;
        Multiply(ref value1, ref value2, out result);
        return result;
    }

    #endregion

    /// <summary>
    /// Add two quaternions.
    /// </summary>
    /// <param name="value1">The first quaternion.</param>
    /// <param name="value2">The second quaternion.</param>
    /// <returns>The sum of both quaternions.</returns>

    #region public static FP operator +(JQuaternion value1, JQuaternion value2)

    public static FPQuaternion operator +(FPQuaternion value1, FPQuaternion value2)
    {
        FPQuaternion result;
        Add(ref value1, ref value2, out result);
        return result;
    }

    #endregion

    /// <summary>
    /// Subtract two quaternions.
    /// </summary>
    /// <param name="value1">The first quaternion.</param>
    /// <param name="value2">The second quaternion.</param>
    /// <returns>The difference of both quaternions.</returns>

    #region public static FP operator -(JQuaternion value1, JQuaternion value2)

    public static FPQuaternion operator -(FPQuaternion value1, FPQuaternion value2)
    {
        FPQuaternion result;
        Subtract(ref value1, ref value2, out result);
        return result;
    }

    #endregion

    /**
     *  @brief Rotates a {@link TSVector} by the {@link TSQuanternion}.
     **/
    public static FPVector3 operator *(FPQuaternion quat, FPVector3 vec)
    {
        GameFrameX.Utility.Math.FP num = quat.x * 2 * GameFrameX.Utility.Math.FP.One;
        GameFrameX.Utility.Math.FP num2 = quat.y * 2 * GameFrameX.Utility.Math.FP.One;
        GameFrameX.Utility.Math.FP num3 = quat.z * 2 * GameFrameX.Utility.Math.FP.One;
        GameFrameX.Utility.Math.FP num4 = quat.x * num;
        GameFrameX.Utility.Math.FP num5 = quat.y * num2;
        GameFrameX.Utility.Math.FP num6 = quat.z * num3;
        GameFrameX.Utility.Math.FP num7 = quat.x * num2;
        GameFrameX.Utility.Math.FP num8 = quat.x * num3;
        GameFrameX.Utility.Math.FP num9 = quat.y * num3;
        GameFrameX.Utility.Math.FP num10 = quat.w * num;
        GameFrameX.Utility.Math.FP num11 = quat.w * num2;
        GameFrameX.Utility.Math.FP num12 = quat.w * num3;

        FPVector3 result;
        result.x = (GameFrameX.Utility.Math.FP.One - (num5 + num6)) * vec.x + (num7 - num12) * vec.y + (num8 + num11) * vec.z;
        result.y = (num7 + num12) * vec.x + (GameFrameX.Utility.Math.FP.One - (num4 + num6)) * vec.y + (num9 - num10) * vec.z;
        result.z = (num8 - num11) * vec.x + (num9 + num10) * vec.y + (GameFrameX.Utility.Math.FP.One - (num4 + num5)) * vec.z;

        return result;
    }

    public override string ToString()
    {
        return string.Format("({0:f5}, {1:f5}, {2:f5}, {3:f5})", x.AsFloat(), y.AsFloat(), z.AsFloat(), w.AsFloat());
    }

    public static bool operator ==(FPQuaternion value1, FPQuaternion value2)
    {
        return (((value1.x == value2.x) && (value1.y == value2.y)) && (value1.z == value2.z) && (value1.w == value2.w));
    }

    public static bool operator !=(FPQuaternion value1, FPQuaternion value2)
    {
        if ((value1.x == value2.x) && (value1.y == value2.y))
        {
            return (value1.z != value2.z) && (value1.w != value2.w);
        }

        return true;
    }

    public override bool Equals(object obj)
    {
        if (!(obj is FPQuaternion)) return false;
        FPQuaternion other = (FPQuaternion)obj;

        return (((x == other.x) && (y == other.y)) && (z == other.z) && (w == other.w));
    }

    public override int GetHashCode()
    {
        return x.GetHashCode() ^ y.GetHashCode() ^ z.GetHashCode() ^ w.GetHashCode();
    }
}