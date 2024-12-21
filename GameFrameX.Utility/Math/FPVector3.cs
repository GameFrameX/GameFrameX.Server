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
/// A vector structure.
/// </summary>
[Serializable]
public struct FPVector3
{
    private static FP ZeroEpsilonSq = FPMath.Epsilon;
    internal static FPVector3 InternalZero;
    internal static FPVector3 Arbitrary;

    /// <summary>The X component of the vector.</summary>
    public FP x;

    /// <summary>The Y component of the vector.</summary>
    public FP y;

    /// <summary>The Z component of the vector.</summary>
    public FP z;

    #region Static readonly variables

    /// <summary>
    /// A vector with components (0,0,0);
    /// </summary>
    public static readonly FPVector3 zero;

    /// <summary>
    /// A vector with components (-1,0,0);
    /// </summary>
    public static readonly FPVector3 left;

    /// <summary>
    /// A vector with components (1,0,0);
    /// </summary>
    public static readonly FPVector3 right;

    /// <summary>
    /// A vector with components (0,1,0);
    /// </summary>
    public static readonly FPVector3 up;

    /// <summary>
    /// A vector with components (0,-1,0);
    /// </summary>
    public static readonly FPVector3 down;

    /// <summary>
    /// A vector with components (0,0,-1);
    /// </summary>
    public static readonly FPVector3 back;

    /// <summary>
    /// A vector with components (0,0,1);
    /// </summary>
    public static readonly FPVector3 forward;

    /// <summary>
    /// A vector with components (1,1,1);
    /// </summary>
    public static readonly FPVector3 one;

    /// <summary>
    /// A vector with components
    /// (FP.MinValue,FP.MinValue,FP.MinValue);
    /// </summary>
    public static readonly FPVector3 MinValue;

    /// <summary>
    /// A vector with components
    /// (FP.MaxValue,FP.MaxValue,FP.MaxValue);
    /// </summary>
    public static readonly FPVector3 MaxValue;

    #endregion

    #region Private static constructor

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
    /// Returns the absolute value of a vector.
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    public static FPVector3 Abs(FPVector3 other)
    {
        return new FPVector3(FP.FastAbs(other.x), FP.FastAbs(other.y), FP.FastAbs(other.z));
    }

    /// <summary>
    /// Gets the squared length of the vector.
    /// </summary>
    /// <returns>Returns the squared length of the vector.</returns>
    public FP sqrMagnitude
    {
        get { return x * x + y * y + z * z; }
    }

    /// <summary>
    /// Gets the length of the vector.
    /// </summary>
    /// <returns>Returns the length of the vector.</returns>
    public FP magnitude
    {
        get
        {
            var num = x * x + y * y + z * z;
            return FP.Sqrt(num);
        }
    }

    /// <summary>
    /// Clamps the magnitude of the vector.
    /// </summary>
    /// <param name="vector3"></param>
    /// <param name="maxLength"></param>
    /// <returns></returns>
    public static FPVector3 ClampMagnitude(FPVector3 vector3, FP maxLength)
    {
        return Normalize(vector3) * maxLength;
    }

    /// <summary>
    /// Gets a normalized version of the vector.
    /// </summary>
    /// <returns>Returns a normalized version of the vector.</returns>
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
    /// Constructor initializing a new instance of the structure
    /// </summary>
    /// <param name="x">The X component of the vector.</param>
    /// <param name="y">The Y component of the vector.</param>
    /// <param name="z">The Z component of the vector.</param>
    public FPVector3(int x, int y, int z)
    {
        this.x = x;
        this.y = y;
        this.z = z;
    }

    /// <summary>
    /// Constructor initializing a new instance of the structure
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="z"></param>
    public FPVector3(FP x, FP y, FP z)
    {
        this.x = x;
        this.y = y;
        this.z = z;
    }

    /// <summary>
    /// Multiplies each component of the vector by the same components of the provided vector.
    /// </summary>
    public void Scale(FPVector3 other)
    {
        x = x * other.x;
        y = y * other.y;
        z = z * other.z;
    }

    /// <summary>
    /// Sets all vector component to specific values.
    /// </summary>
    /// <param name="x">The X component of the vector.</param>
    /// <param name="y">The Y component of the vector.</param>
    /// <param name="z">The Z component of the vector.</param>
    public void Set(FP x, FP y, FP z)
    {
        this.x = x;
        this.y = y;
        this.z = z;
    }

    /// <summary>
    /// Constructor initializing a new instance of the structure
    /// </summary>
    /// <param name="xyz">All components of the vector are set to xyz</param>
    public FPVector3(FP xyz)
    {
        x = xyz;
        y = xyz;
        z = xyz;
    }

    /// <summary>
    /// Linearly interpolates between two vectors.
    /// </summary>
    /// <param name="from"></param>
    /// <param name="to"></param>
    /// <param name="percent"></param>
    /// <returns></returns>
    public static FPVector3 Lerp(FPVector3 from, FPVector3 to, FP percent)
    {
        return from + (to - from) * percent;
    }

    /// <summary>
    /// Builds a string from the JVector.
    /// </summary>
    /// <returns>A string containing all three components.</returns>

    #region public override string ToString()

    public override string ToString()
    {
        return string.Format("({0:f5}, {1:f5}, {2:f5})", x.AsFloat(), y.AsFloat(), z.AsFloat());
    }

    #endregion

    /// <summary>
    /// Tests if an object is equal to this vector.
    /// </summary>
    /// <param name="obj">The object to test.</param>
    /// <returns>Returns true if they are euqal, otherwise false.</returns>

    #region public override bool Equals(object obj)

    public override bool Equals(object obj)
    {
        if (!(obj is FPVector3))
        {
            return false;
        }

        var other = (FPVector3)obj;

        return x == other.x && y == other.y && z == other.z;
    }

    #endregion

    /// <summary>
    /// Multiplies each component of the vector by the same components of the provided vector.
    /// </summary>
    public static FPVector3 Scale(FPVector3 vecA, FPVector3 vecB)
    {
        FPVector3 result;
        result.x = vecA.x * vecB.x;
        result.y = vecA.y * vecB.y;
        result.z = vecA.z * vecB.z;

        return result;
    }

    /// <summary>
    /// Inverses the direction of a vector.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static FPVector3 operator -(FPVector3 value)
    {
        value.x = -value.x;
        value.y = -value.y;
        value.z = -value.z;
        return value;
    }

    /// <summary>
    /// Tests if two JVector are equal.
    /// </summary>
    /// <param name="value1">The first value.</param>
    /// <param name="value2">The second value.</param>
    /// <returns>Returns true if both values are equal, otherwise false.</returns>

    #region public static bool operator ==(JVector value1, JVector value2)

    public static bool operator ==(FPVector3 value1, FPVector3 value2)
    {
        return value1.x == value2.x && value1.y == value2.y && value1.z == value2.z;
    }

    #endregion

    /// <summary>
    /// Tests if two JVector are not equal.
    /// </summary>
    /// <param name="value1">The first value.</param>
    /// <param name="value2">The second value.</param>
    /// <returns>Returns false if both values are equal, otherwise true.</returns>

    #region public static bool operator !=(JVector value1, JVector value2)

    public static bool operator !=(FPVector3 value1, FPVector3 value2)
    {
        if (value1.x == value2.x && value1.y == value2.y)
        {
            return value1.z != value2.z;
        }

        return true;
    }

    #endregion

    /// <summary>
    /// Gets a vector with the minimum x,y and z values of both vectors.
    /// </summary>
    /// <param name="value1">The first value.</param>
    /// <param name="value2">The second value.</param>
    /// <returns>A vector with the minimum x,y and z values of both vectors.</returns>

    #region public static JVector Min(JVector value1, JVector value2)

    public static FPVector3 Min(FPVector3 value1, FPVector3 value2)
    {
        Min(ref value1, ref value2, out var result);
        return result;
    }

    /// <summary>
    /// Gets a vector with the minimum x,y and z values of both vectors.
    /// </summary>
    /// <param name="value1">The first value.</param>
    /// <param name="value2">The second value.</param>
    /// <param name="result">A vector with the minimum x,y and z values of both vectors.</param>
    public static void Min(ref FPVector3 value1, ref FPVector3 value2, out FPVector3 result)
    {
        result.x = value1.x < value2.x ? value1.x : value2.x;
        result.y = value1.y < value2.y ? value1.y : value2.y;
        result.z = value1.z < value2.z ? value1.z : value2.z;
    }

    #endregion

    /// <summary>
    /// Gets a vector with the maximum x,y and z values of both vectors.
    /// </summary>
    /// <param name="value1">The first value.</param>
    /// <param name="value2">The second value.</param>
    /// <returns>A vector with the maximum x,y and z values of both vectors.</returns>

    #region public static JVector Max(JVector value1, JVector value2)

    public static FPVector3 Max(FPVector3 value1, FPVector3 value2)
    {
        FPVector3 result;
        Max(ref value1, ref value2, out result);
        return result;
    }

    /// <summary>
    /// Gets a vector with the maximum x,y and z values of both vectors.
    /// </summary>
    /// <param name="v1"></param>
    /// <param name="v2"></param>
    /// <returns></returns>
    public static FP Distance(FPVector3 v1, FPVector3 v2)
    {
        return FP.Sqrt((v1.x - v2.x) * (v1.x - v2.x) + (v1.y - v2.y) * (v1.y - v2.y) + (v1.z - v2.z) * (v1.z - v2.z));
    }

    /// <summary>
    /// Gets a vector with the maximum x,y and z values of both vectors.
    /// </summary>
    /// <param name="value1">The first value.</param>
    /// <param name="value2">The second value.</param>
    /// <param name="result">A vector with the maximum x,y and z values of both vectors.</param>
    public static void Max(ref FPVector3 value1, ref FPVector3 value2, out FPVector3 result)
    {
        result.x = value1.x > value2.x ? value1.x : value2.x;
        result.y = value1.y > value2.y ? value1.y : value2.y;
        result.z = value1.z > value2.z ? value1.z : value2.z;
    }

    #endregion

    /// <summary>
    /// Sets the length of the vector to zero.
    /// </summary>

    #region public void MakeZero()

    public void MakeZero()
    {
        x = FP.Zero;
        y = FP.Zero;
        z = FP.Zero;
    }

    #endregion

    /// <summary>
    /// Checks if the length of the vector is zero.
    /// </summary>
    /// <returns>Returns true if the vector is zero, otherwise false.</returns>

    #region public bool IsZero()

    public bool IsZero()
    {
        return sqrMagnitude == FP.Zero;
    }

    /// <summary>
    /// Checks if the length of the vector is nearly zero.
    /// </summary>
    /// <returns>Returns true if the vector is nearly zero, otherwise false.</returns>
    public bool IsNearlyZero()
    {
        return sqrMagnitude < ZeroEpsilonSq;
    }

    #endregion

    /// <summary>
    /// Transforms a vector by the given matrix.
    /// </summary>
    /// <param name="position">The vector to transform.</param>
    /// <param name="matrix">The transform matrix.</param>
    /// <returns>The transformed vector.</returns>

    #region public static JVector Transform(JVector position, JMatrix matrix)

    public static FPVector3 Transform(FPVector3 position, FPMatrix matrix)
    {
        FPVector3 result;
        Transform(ref position, ref matrix, out result);
        return result;
    }

    /// <summary>
    /// Transforms a vector by the given matrix.
    /// </summary>
    /// <param name="position">The vector to transform.</param>
    /// <param name="matrix">The transform matrix.</param>
    /// <param name="result">The transformed vector.</param>
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
    /// Transforms a vector by the transposed of the given Matrix.
    /// </summary>
    /// <param name="position">The vector to transform.</param>
    /// <param name="matrix">The transform matrix.</param>
    /// <param name="result">The transformed vector.</param>
    public static void TransposedTransform(ref FPVector3 position, ref FPMatrix matrix, out FPVector3 result)
    {
        var num0 = position.x * matrix.M11 + position.y * matrix.M12 + position.z * matrix.M13;
        var num1 = position.x * matrix.M21 + position.y * matrix.M22 + position.z * matrix.M23;
        var num2 = position.x * matrix.M31 + position.y * matrix.M32 + position.z * matrix.M33;

        result.x = num0;
        result.y = num1;
        result.z = num2;
    }

    #endregion

    /// <summary>
    /// Calculates the dot product of two vectors.
    /// </summary>
    /// <param name="vector1">The first vector.</param>
    /// <param name="vector2">The second vector.</param>
    /// <returns>Returns the dot product of both vectors.</returns>

    #region public static FP Dot(JVector vector1, JVector vector2)

    public static FP Dot(FPVector3 vector1, FPVector3 vector2)
    {
        return Dot(ref vector1, ref vector2);
    }


    /// <summary>
    /// Calculates the dot product of both vectors.
    /// </summary>
    /// <param name="vector1">The first vector.</param>
    /// <param name="vector2">The second vector.</param>
    /// <returns>Returns the dot product of both vectors.</returns>
    public static FP Dot(ref FPVector3 vector1, ref FPVector3 vector2)
    {
        return vector1.x * vector2.x + vector1.y * vector2.y + vector1.z * vector2.z;
    }

    #endregion

    // Projects a vector onto another vector.
    /// <summary>
    /// Projects a vector onto another vector.
    /// </summary>
    /// <param name="vector3"></param>
    /// <param name="onNormal"></param>
    /// <returns></returns>
    public static FPVector3 Project(FPVector3 vector3, FPVector3 onNormal)
    {
        var sqrtMag = Dot(onNormal, onNormal);
        if (sqrtMag < FPMath.Epsilon)
        {
            return zero;
        }

        return onNormal * Dot(vector3, onNormal) / sqrtMag;
    }

    // Projects a vector onto a plane defined by a normal orthogonal to the plane.
    /// <summary>
    /// Projects a vector onto a plane defined by a normal orthogonal to the plane.
    /// </summary>
    /// <param name="vector3"></param>
    /// <param name="planeNormal"></param>
    /// <returns></returns>
    public static FPVector3 ProjectOnPlane(FPVector3 vector3, FPVector3 planeNormal)
    {
        return vector3 - Project(vector3, planeNormal);
    }


    // Returns the angle in degrees between /from/ and /to/. This is always the smallest
    /// <summary>
    /// Returns the angle in degrees between /from/ and /to/. This is always the smallest
    /// </summary>
    /// <param name="from"></param>
    /// <param name="to"></param>
    /// <returns></returns>
    public static FP Angle(FPVector3 from, FPVector3 to)
    {
        return FPMath.Acos(FPMath.Clamp(Dot(from.normalized, to.normalized), -FP.One, FP.One)) * FPMath.Rad2Deg;
    }

    // The smaller of the two possible angles between the two vectors is returned, therefore the result will never be greater than 180 degrees or smaller than -180 degrees.
    // If you imagine the from and to vectors as lines on a piece of paper, both originating from the same point, then the /axis/ vector would point up out of the paper.
    // The measured angle between the two vectors would be positive in a clockwise direction and negative in an anti-clockwise direction.
    /// <summary>
    /// The smaller of the two possible angles between the two vectors is returned, therefore the result will never be greater than 180 degrees or smaller than -180 degrees.
    /// </summary>
    /// <param name="from"></param>
    /// <param name="to"></param>
    /// <param name="axis"></param>
    /// <returns></returns>
    public static FP SignedAngle(FPVector3 from, FPVector3 to, FPVector3 axis)
    {
        FPVector3 fromNorm = from.normalized, toNorm = to.normalized;
        var unsignedAngle = FPMath.Acos(FPMath.Clamp(Dot(fromNorm, toNorm), -FP.One, FP.One)) * FPMath.Rad2Deg;
        FP sign = FPMath.Sign(Dot(axis, Cross(fromNorm, toNorm)));
        return unsignedAngle * sign;
    }

    /// <summary>
    /// Adds two vectors.
    /// </summary>
    /// <param name="value1">The first vector.</param>
    /// <param name="value2">The second vector.</param>
    /// <returns>The sum of both vectors.</returns>

    #region public static void Add(JVector value1, JVector value2)

    public static FPVector3 Add(FPVector3 value1, FPVector3 value2)
    {
        FPVector3 result;
        Add(ref value1, ref value2, out result);
        return result;
    }

    /// <summary>
    /// Adds to vectors.
    /// </summary>
    /// <param name="value1">The first vector.</param>
    /// <param name="value2">The second vector.</param>
    /// <param name="result">The sum of both vectors.</param>
    public static void Add(ref FPVector3 value1, ref FPVector3 value2, out FPVector3 result)
    {
        var num0 = value1.x + value2.x;
        var num1 = value1.y + value2.y;
        var num2 = value1.z + value2.z;

        result.x = num0;
        result.y = num1;
        result.z = num2;
    }

    #endregion

    /// <summary>
    /// Divides a vector by a factor.
    /// </summary>
    /// <param name="value1">The vector to divide.</param>
    /// <param name="scaleFactor">The scale factor.</param>
    /// <returns>Returns the scaled vector.</returns>
    public static FPVector3 Divide(FPVector3 value1, FP scaleFactor)
    {
        FPVector3 result;
        Divide(ref value1, scaleFactor, out result);
        return result;
    }

    /// <summary>
    /// Divides a vector by a factor.
    /// </summary>
    /// <param name="value1">The vector to divide.</param>
    /// <param name="scaleFactor">The scale factor.</param>
    /// <param name="result">Returns the scaled vector.</param>
    public static void Divide(ref FPVector3 value1, FP scaleFactor, out FPVector3 result)
    {
        result.x = value1.x / scaleFactor;
        result.y = value1.y / scaleFactor;
        result.z = value1.z / scaleFactor;
    }

    /// <summary>
    /// Subtracts two vectors.
    /// </summary>
    /// <param name="value1">The first vector.</param>
    /// <param name="value2">The second vector.</param>
    /// <returns>The difference of both vectors.</returns>

    #region public static JVector Subtract(JVector value1, JVector value2)

    public static FPVector3 Subtract(FPVector3 value1, FPVector3 value2)
    {
        FPVector3 result;
        Subtract(ref value1, ref value2, out result);
        return result;
    }

    /// <summary>
    /// Subtracts to vectors.
    /// </summary>
    /// <param name="value1">The first vector.</param>
    /// <param name="value2">The second vector.</param>
    /// <param name="result">The difference of both vectors.</param>
    public static void Subtract(ref FPVector3 value1, ref FPVector3 value2, out FPVector3 result)
    {
        var num0 = value1.x - value2.x;
        var num1 = value1.y - value2.y;
        var num2 = value1.z - value2.z;

        result.x = num0;
        result.y = num1;
        result.z = num2;
    }

    #endregion

    /// <summary>
    /// The cross product of two vectors.
    /// </summary>
    /// <param name="vector1">The first vector.</param>
    /// <param name="vector2">The second vector.</param>
    /// <returns>The cross product of both vectors.</returns>

    #region public static JVector Cross(JVector vector1, JVector vector2)

    public static FPVector3 Cross(FPVector3 vector1, FPVector3 vector2)
    {
        FPVector3 result;
        Cross(ref vector1, ref vector2, out result);
        return result;
    }

    /// <summary>
    /// The cross product of two vectors.
    /// </summary>
    /// <param name="vector1">The first vector.</param>
    /// <param name="vector2">The second vector.</param>
    /// <param name="result">The cross product of both vectors.</param>
    public static void Cross(ref FPVector3 vector1, ref FPVector3 vector2, out FPVector3 result)
    {
        var num3 = vector1.y * vector2.z - vector1.z * vector2.y;
        var num2 = vector1.z * vector2.x - vector1.x * vector2.z;
        var num = vector1.x * vector2.y - vector1.y * vector2.x;
        result.x = num3;
        result.y = num2;
        result.z = num;
    }

    #endregion

    /// <summary>
    /// Gets the hashcode of the vector.
    /// </summary>
    /// <returns>Returns the hashcode of the vector.</returns>

    #region public override int GetHashCode()

    public override int GetHashCode()
    {
        return x.GetHashCode() ^ y.GetHashCode() ^ z.GetHashCode();
    }

    #endregion

    /// <summary>
    /// Inverses the direction of the vector.
    /// </summary>

    #region public static JVector Negate(JVector value)

    public void Negate()
    {
        x = -x;
        y = -y;
        z = -z;
    }

    /// <summary>
    /// Inverses the direction of a vector.
    /// </summary>
    /// <param name="value">The vector to inverse.</param>
    /// <returns>The negated vector.</returns>
    public static FPVector3 Negate(FPVector3 value)
    {
        FPVector3 result;
        Negate(ref value, out result);
        return result;
    }

    /// <summary>
    /// Inverses the direction of a vector.
    /// </summary>
    /// <param name="value">The vector to inverse.</param>
    /// <param name="result">The negated vector.</param>
    public static void Negate(ref FPVector3 value, out FPVector3 result)
    {
        var num0 = -value.x;
        var num1 = -value.y;
        var num2 = -value.z;

        result.x = num0;
        result.y = num1;
        result.z = num2;
    }

    #endregion

    /// <summary>
    /// Normalizes the given vector.
    /// </summary>
    /// <param name="value">The vector which should be normalized.</param>
    /// <returns>A normalized vector.</returns>

    #region public static JVector Normalize(JVector value)

    public static FPVector3 Normalize(FPVector3 value)
    {
        FPVector3 result;
        Normalize(ref value, out result);
        return result;
    }

    /// <summary>
    /// Normalizes this vector.
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
    /// Normalizes the given vector.
    /// </summary>
    /// <param name="value">The vector which should be normalized.</param>
    /// <param name="result">A normalized vector.</param>
    public static void Normalize(ref FPVector3 value, out FPVector3 result)
    {
        var num2 = value.x * value.x + value.y * value.y + value.z * value.z;
        var num = FP.One / FP.Sqrt(num2);
        result.x = value.x * num;
        result.y = value.y * num;
        result.z = value.z * num;
    }

    #endregion

    #region public static void Swap(ref JVector vector1, ref JVector vector2)

    /// <summary>
    /// Swaps the components of both vectors.
    /// </summary>
    /// <param name="vector1">The first vector to swap with the second.</param>
    /// <param name="vector2">The second vector to swap with the first.</param>
    public static void Swap(ref FPVector3 vector1, ref FPVector3 vector2)
    {
        FP temp;

        temp = vector1.x;
        vector1.x = vector2.x;
        vector2.x = temp;

        temp = vector1.y;
        vector1.y = vector2.y;
        vector2.y = temp;

        temp = vector1.z;
        vector1.z = vector2.z;
        vector2.z = temp;
    }

    #endregion

    /// <summary>
    /// Multiply a vector with a factor.
    /// </summary>
    /// <param name="value1">The vector to multiply.</param>
    /// <param name="scaleFactor">The scale factor.</param>
    /// <returns>Returns the multiplied vector.</returns>

    #region public static JVector Multiply(JVector value1, FP scaleFactor)

    public static FPVector3 Multiply(FPVector3 value1, FP scaleFactor)
    {
        FPVector3 result;
        Multiply(ref value1, scaleFactor, out result);
        return result;
    }

    /// <summary>
    /// Multiply a vector with a factor.
    /// </summary>
    /// <param name="value1">The vector to multiply.</param>
    /// <param name="scaleFactor">The scale factor.</param>
    /// <param name="result">Returns the multiplied vector.</param>
    public static void Multiply(ref FPVector3 value1, FP scaleFactor, out FPVector3 result)
    {
        result.x = value1.x * scaleFactor;
        result.y = value1.y * scaleFactor;
        result.z = value1.z * scaleFactor;
    }

    #endregion

    /// <summary>
    /// Calculates the cross product of two vectors.
    /// </summary>
    /// <param name="value1">The first vector.</param>
    /// <param name="value2">The second vector.</param>
    /// <returns>Returns the cross product of both.</returns>

    #region public static JVector operator %(JVector value1, JVector value2)

    public static FPVector3 operator %(FPVector3 value1, FPVector3 value2)
    {
        FPVector3 result;
        Cross(ref value1, ref value2, out result);
        return result;
    }

    #endregion

    /// <summary>
    /// Calculates the dot product of two vectors.
    /// </summary>
    /// <param name="value1">The first vector.</param>
    /// <param name="value2">The second vector.</param>
    /// <returns>Returns the dot product of both.</returns>

    #region public static FP operator *(JVector value1, JVector value2)

    public static FP operator *(FPVector3 value1, FPVector3 value2)
    {
        return Dot(ref value1, ref value2);
    }

    #endregion

    /// <summary>
    /// Multiplies a vector by a scale factor.
    /// </summary>
    /// <param name="value1">The vector to scale.</param>
    /// <param name="value2">The scale factor.</param>
    /// <returns>Returns the scaled vector.</returns>

    #region public static JVector operator *(JVector value1, FP value2)

    public static FPVector3 operator *(FPVector3 value1, FP value2)
    {
        FPVector3 result;
        Multiply(ref value1, value2, out result);
        return result;
    }

    #endregion

    /// <summary>
    /// Multiplies a vector by a scale factor.
    /// </summary>
    /// <param name="value2">The vector to scale.</param>
    /// <param name="value1">The scale factor.</param>
    /// <returns>Returns the scaled vector.</returns>

    #region public static JVector operator *(FP value1, JVector value2)

    public static FPVector3 operator *(FP value1, FPVector3 value2)
    {
        FPVector3 result;
        Multiply(ref value2, value1, out result);
        return result;
    }

    #endregion

    /// <summary>
    /// Subtracts two vectors.
    /// </summary>
    /// <param name="value1">The first vector.</param>
    /// <param name="value2">The second vector.</param>
    /// <returns>The difference of both vectors.</returns>

    #region public static JVector operator -(JVector value1, JVector value2)

    public static FPVector3 operator -(FPVector3 value1, FPVector3 value2)
    {
        FPVector3 result;
        Subtract(ref value1, ref value2, out result);
        return result;
    }

    #endregion

    /// <summary>
    /// Adds two vectors.
    /// </summary>
    /// <param name="value1">The first vector.</param>
    /// <param name="value2">The second vector.</param>
    /// <returns>The sum of both vectors.</returns>

    #region public static JVector operator +(JVector value1, JVector value2)

    public static FPVector3 operator +(FPVector3 value1, FPVector3 value2)
    {
        FPVector3 result;
        Add(ref value1, ref value2, out result);
        return result;
    }

    #endregion

    /// <summary>
    /// Divides a vector by a factor.
    /// </summary>
    /// <param name="value1">The vector to divide.</param>
    /// <param name="scaleFactor">The scale factor.</param>
    /// <returns>Returns the scaled vector.</returns>
    public static FPVector3 operator /(FPVector3 value1, FP scaleFactor)
    {
        Divide(ref value1, scaleFactor, out var result);
        return result;
    }

    /// <summary>
    /// Calculates the cross product of two vectors.
    /// </summary>
    /// <param name="value1"></param>
    /// <param name="value2"></param>
    /// <returns></returns>
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
    /// Converts the vector to a 2D vector
    /// </summary>
    /// <returns></returns>
    public FPVector2 ToFPVector2()
    {
        return new FPVector2(x, y);
    }

    /// <summary>
    /// Converts the vector to a 4D vector
    /// </summary>
    /// <returns></returns>
    public FPVector4 ToFPVector4()
    {
        return new FPVector4(x, y, z, FP.One);
    }
}