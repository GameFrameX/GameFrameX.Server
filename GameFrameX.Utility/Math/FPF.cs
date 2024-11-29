#if !FP
using System;
using System.IO;

namespace GameFrameX.Utility.Math
{
    public partial struct FP : IEquatable<GameFrameX.Utility.Math.FP>, IComparable<GameFrameX.Utility.Math.FP>
    {
        private float rawvalue;
        public float RawValue => rawvalue;
        public const float MAX_VALUE = float.MaxValue;
        public const float MIN_VALUE = float.MinValue;
        public const float ONE = 1f;
        public const float TEN = 10f;
        public const float HALF = 0.5f;
        public const float PI_TIMES_2 = 6.28318530718f;
        public const float PI = 3.141592653589793f;
        public const float PI_OVER_2 = 1.5707963267948966f;

        public static readonly GameFrameX.Utility.Math.FP MaxValue = new GameFrameX.Utility.Math.FP(MAX_VALUE - 1);
        public static readonly GameFrameX.Utility.Math.FP MinValue = new GameFrameX.Utility.Math.FP(MIN_VALUE + 2);
        public static readonly GameFrameX.Utility.Math.FP One = new GameFrameX.Utility.Math.FP(ONE);
        public static readonly GameFrameX.Utility.Math.FP Ten = new GameFrameX.Utility.Math.FP(TEN);
        public static readonly GameFrameX.Utility.Math.FP Half = new GameFrameX.Utility.Math.FP(HALF);

        public static readonly GameFrameX.Utility.Math.FP Zero = new GameFrameX.Utility.Math.FP();
        public static readonly GameFrameX.Utility.Math.FP PositiveInfinity = new GameFrameX.Utility.Math.FP(MAX_VALUE);
        public static readonly GameFrameX.Utility.Math.FP NegativeInfinity = new GameFrameX.Utility.Math.FP(MIN_VALUE + 1);
        public static readonly GameFrameX.Utility.Math.FP NaN = new GameFrameX.Utility.Math.FP(MIN_VALUE);

        public static readonly GameFrameX.Utility.Math.FP EN1 = One / 10;
        public static readonly GameFrameX.Utility.Math.FP EN2 = One / 100;
        public static readonly GameFrameX.Utility.Math.FP EN3 = One / 1000;
        public static readonly GameFrameX.Utility.Math.FP EN4 = One / 10000;
        public static readonly GameFrameX.Utility.Math.FP EN5 = One / 100000;
        public static readonly GameFrameX.Utility.Math.FP EN6 = One / 1000000;
        public static readonly GameFrameX.Utility.Math.FP EN7 = One / 10000000;
        public static readonly GameFrameX.Utility.Math.FP EN8 = One / 100000000;
        public static readonly GameFrameX.Utility.Math.FP Epsilon = EN3;

        /// <summary>
        /// The value of Pi
        /// </summary>
        public static readonly GameFrameX.Utility.Math.FP Pi = new GameFrameX.Utility.Math.FP(PI);
        public static readonly GameFrameX.Utility.Math.FP PiOver2 = new GameFrameX.Utility.Math.FP(PI_OVER_2);
        public static readonly GameFrameX.Utility.Math.FP PiTimes2 = new GameFrameX.Utility.Math.FP(PI_TIMES_2);
        public static readonly GameFrameX.Utility.Math.FP PiInv = (GameFrameX.Utility.Math.FP)0.3183098861837906715377675267M;
        public static readonly GameFrameX.Utility.Math.FP PiOver2Inv = (GameFrameX.Utility.Math.FP)0.6366197723675813430755350535M;

        public static readonly GameFrameX.Utility.Math.FP Deg2Rad = Pi / new GameFrameX.Utility.Math.FP(180);

        public static readonly GameFrameX.Utility.Math.FP Rad2Deg = new GameFrameX.Utility.Math.FP(180) / Pi;

        /// <summary>
        /// Returns 2 raised to the specified power.
        /// Provides at least 6 decimals of accuracy.
        /// </summary>
        internal static GameFrameX.Utility.Math.FP Pow2(GameFrameX.Utility.Math.FP x)
        {
            return new GameFrameX.Utility.Math.FP((float)System.Math.Pow(2, x.rawvalue));
        }
        
        /// <summary>
        /// Returns the base-2 logarithm of a specified number.
        /// Provides at least 9 decimals of accuracy.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">
        /// The argument was non-positive
        /// </exception>
        public static GameFrameX.Utility.Math.FP Log2(GameFrameX.Utility.Math.FP x)
        {
            return new GameFrameX.Utility.Math.FP(float.Log2(x.rawvalue));
        }

        /// <summary>
        /// Returns a number indicating the sign of a Fix64 number.
        /// Returns 1 if the value is positive, 0 if is 0, and -1 if it is negative.
        /// </summary>
        public static int Sign(GameFrameX.Utility.Math.FP value)
        {
            return value.rawvalue < 0 ? -1 : value.rawvalue > 0 ? 1 : 0;
        }


        /// <summary>
        /// Returns the absolute value of a Fix64 number.
        /// Note: Abs(Fix64.MinValue) == Fix64.MaxValue.
        /// </summary>
        public static GameFrameX.Utility.Math.FP Abs(GameFrameX.Utility.Math.FP value)
        {
            return new GameFrameX.Utility.Math.FP(System.Math.Abs(value.rawvalue));
        }

        /// <summary>
        /// Returns the absolute value of a Fix64 number.
        /// FastAbs(Fix64.MinValue) is undefined.
        /// </summary>
        public static GameFrameX.Utility.Math.FP FastAbs(GameFrameX.Utility.Math.FP value)
        {
            return Abs(value);
        }


        /// <summary>
        /// Returns the largest integer less than or equal to the specified number.
        /// </summary>
        public static GameFrameX.Utility.Math.FP Floor(GameFrameX.Utility.Math.FP value)
        {
            return new GameFrameX.Utility.Math.FP(float.Floor(value.rawvalue));
        }

        /// <summary>
        /// Returns the smallest integral value that is greater than or equal to the specified number.
        /// </summary>
        public static GameFrameX.Utility.Math.FP Ceiling(GameFrameX.Utility.Math.FP value)
        {
            return new GameFrameX.Utility.Math.FP(float.Ceiling(value.rawvalue));
        }

        /// <summary>
        /// Rounds a value to the nearest integral value.
        /// If the value is halfway between an even and an uneven value, returns the even value.
        /// </summary>
        public static GameFrameX.Utility.Math.FP Round(GameFrameX.Utility.Math.FP value)
        {
            return new GameFrameX.Utility.Math.FP(float.Round(value.rawvalue));
        }

        public static GameFrameX.Utility.Math.FP Max(GameFrameX.Utility.Math.FP left, GameFrameX.Utility.Math.FP right)
        {
            return new GameFrameX.Utility.Math.FP(float.Max(left.rawvalue, right.rawvalue));
        }

        public static GameFrameX.Utility.Math.FP Min(GameFrameX.Utility.Math.FP left, GameFrameX.Utility.Math.FP right)
        {
            return new GameFrameX.Utility.Math.FP(float.Min(left.rawvalue, right.rawvalue));
        }

        public static GameFrameX.Utility.Math.FP Clamp(GameFrameX.Utility.Math.FP origin, GameFrameX.Utility.Math.FP min, GameFrameX.Utility.Math.FP max)
        {
            return Min(Max(origin, min), max);
        }

        /// <summary>
        /// Adds x and y. Performs saturating addition, i.e. in case of overflow, 
        /// rounds to MinValue or MaxValue depending on sign of operands.
        /// </summary>
        public static GameFrameX.Utility.Math.FP operator +(GameFrameX.Utility.Math.FP x, GameFrameX.Utility.Math.FP y)
        {
            return new GameFrameX.Utility.Math.FP(x.rawvalue + y.rawvalue);
        }

        /// <summary>
        /// Subtracts y from x. Performs saturating substraction, i.e. in case of overflow, 
        /// rounds to MinValue or MaxValue depending on sign of operands.
        /// </summary>
        public static GameFrameX.Utility.Math.FP operator -(GameFrameX.Utility.Math.FP x, GameFrameX.Utility.Math.FP y)
        {
            return new GameFrameX.Utility.Math.FP(x.rawvalue - y.rawvalue);
        }

        /// <summary>
        /// Subtracts y from x witout performing overflow checking. Should be inlined by the CLR.
        /// </summary>
        public static GameFrameX.Utility.Math.FP FastSub(GameFrameX.Utility.Math.FP x, GameFrameX.Utility.Math.FP y)
        {
            return x - y;
        }

        public static GameFrameX.Utility.Math.FP operator *(GameFrameX.Utility.Math.FP x, GameFrameX.Utility.Math.FP y)
        {
            return new GameFrameX.Utility.Math.FP(x.rawvalue * y.rawvalue);
        }
        /// <summary>
        /// Performs multiplication without checking for overflow.
        /// Useful for performance-critical code where the values are guaranteed not to cause overflow
        /// </summary>
        public static GameFrameX.Utility.Math.FP FastMul(GameFrameX.Utility.Math.FP x, GameFrameX.Utility.Math.FP y)
        {
            return x * y;
        }

        //[MethodImplAttribute(MethodImplOptions.AggressiveInlining)] 
        public static int CountLeadingZeroes(ulong x)
        {
            int result = 0;
            while ((x & 0xF000000000000000) == 0)
            {
                result += 4;
                x <<= 4;
            }
            while ((x & 0x8000000000000000) == 0)
            {
                result += 1;
                x <<= 1;
            }
            return result;
        }

        public static GameFrameX.Utility.Math.FP operator /(GameFrameX.Utility.Math.FP x, GameFrameX.Utility.Math.FP y)
        {
            return new GameFrameX.Utility.Math.FP(x.rawvalue / y.rawvalue);
        }

        public static GameFrameX.Utility.Math.FP operator %(GameFrameX.Utility.Math.FP x, GameFrameX.Utility.Math.FP y)
        {
            return new GameFrameX.Utility.Math.FP(x.rawvalue%y.rawvalue);
        }

        public static GameFrameX.Utility.Math.FP operator -(GameFrameX.Utility.Math.FP x)
        {
            return new GameFrameX.Utility.Math.FP(-x.rawvalue);
        }

        public static bool operator ==(GameFrameX.Utility.Math.FP x, GameFrameX.Utility.Math.FP y)
        {
            return x.rawvalue.Equals(y.rawvalue);
        }

        public static bool operator !=(GameFrameX.Utility.Math.FP x, GameFrameX.Utility.Math.FP y)
        {
            return false == x.rawvalue.Equals(y.rawvalue);
        }

        public static bool operator >(GameFrameX.Utility.Math.FP x, GameFrameX.Utility.Math.FP y)
        {
            return x.rawvalue > y.rawvalue;
        }

        public static bool operator <(GameFrameX.Utility.Math.FP x, GameFrameX.Utility.Math.FP y)
        {
            return x.rawvalue < y.rawvalue;
        }

        public static bool operator >=(GameFrameX.Utility.Math.FP x, GameFrameX.Utility.Math.FP y)
        {
            return x.rawvalue >= y.rawvalue;
        }

        public static bool operator <=(GameFrameX.Utility.Math.FP x, GameFrameX.Utility.Math.FP y)
        {
            return x.rawvalue <= y.rawvalue;
        }


        /// <summary>
        /// Returns the square root of a specified number.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">
        /// The argument was negative.
        /// </exception>
        public static GameFrameX.Utility.Math.FP Sqrt(GameFrameX.Utility.Math.FP x)
        {
            return new GameFrameX.Utility.Math.FP(float.Sqrt(x.rawvalue));
        }

        /// <summary>
        /// Returns the Sine of x.
        /// This function has about 9 decimals of accuracy for small values of x.
        /// It may lose accuracy as the value of x grows.
        /// Performance: about 25% slower than Math.Sin() in x64, and 200% slower in x86.
        /// </summary>
        public static GameFrameX.Utility.Math.FP Sin(GameFrameX.Utility.Math.FP x)
        {
            return new GameFrameX.Utility.Math.FP(float.Sin(x.rawvalue));
        }

        /// <summary>
        /// Returns a rough approximation of the Sine of x.
        /// This is at least 3 times faster than Sin() on x86 and slightly faster than Math.Sin(),
        /// however its accuracy is limited to 4-5 decimals, for small enough values of x.
        /// </summary>
        public static GameFrameX.Utility.Math.FP FastSin(GameFrameX.Utility.Math.FP x)
        {
            return Sin(x);
        }

        /// <summary>
        /// Returns the cosine of x.
        /// See Sin() for more details.
        /// </summary>
        public static GameFrameX.Utility.Math.FP Cos(GameFrameX.Utility.Math.FP x)
        {
            return new GameFrameX.Utility.Math.FP(float.Cos(x.rawvalue));
        }

        /// <summary>
        /// Returns a rough approximation of the cosine of x.
        /// See FastSin for more details.
        /// </summary>
        public static GameFrameX.Utility.Math.FP FastCos(GameFrameX.Utility.Math.FP x)
        {
            return Cos(x);
        }

        /// <summary>
        /// Returns the tangent of x.
        /// </summary>
        /// <remarks>
        /// This function is not well-tested. It may be wildly inaccurate.
        /// </remarks>
        public static GameFrameX.Utility.Math.FP Tan(GameFrameX.Utility.Math.FP x)
        {
            return new GameFrameX.Utility.Math.FP(float.Tan(x.rawvalue));
        }

        /// <summary>
        /// Returns the arctan of of the specified number, calculated using Euler series
        /// This function has at least 7 decimals of accuracy.
        /// </summary>
        public static GameFrameX.Utility.Math.FP Atan(GameFrameX.Utility.Math.FP z)
        {
            return new GameFrameX.Utility.Math.FP(float.Atan(z.rawvalue));
        }

        public static GameFrameX.Utility.Math.FP Atan2(GameFrameX.Utility.Math.FP y, GameFrameX.Utility.Math.FP x)
        {
            return new GameFrameX.Utility.Math.FP(float.Atan2(y.rawvalue, x.rawvalue));
        }

        public static GameFrameX.Utility.Math.FP Asin(GameFrameX.Utility.Math.FP value)
        {
            return FastSub(PiOver2, Acos(value));
        }

        /// <summary>
        /// Returns the arccos of of the specified number, calculated using Atan and Sqrt
        /// This function has at least 7 decimals of accuracy.
        /// </summary>
        public static GameFrameX.Utility.Math.FP Acos(GameFrameX.Utility.Math.FP x)
        {
            return new GameFrameX.Utility.Math.FP(float.Acos(x.rawvalue));
        }

        public static implicit operator GameFrameX.Utility.Math.FP(long value)
        {
            return new GameFrameX.Utility.Math.FP(value);
        }

        public static explicit operator long(GameFrameX.Utility.Math.FP value)
        {
            return (long)value.rawvalue;
        }

        public static explicit operator float(GameFrameX.Utility.Math.FP value)
        {
            return value.rawvalue;
        }

        public static explicit operator GameFrameX.Utility.Math.FP(decimal value)
        {
            return new GameFrameX.Utility.Math.FP((float)value);
        }

        public static implicit operator GameFrameX.Utility.Math.FP(int value)
        {
            return new GameFrameX.Utility.Math.FP(value);
        }

        public static explicit operator decimal(GameFrameX.Utility.Math.FP value)
        {
            return (decimal)value.rawvalue;
        }

        public float AsFloat()
        {
            return (float)this;
        }

        public int AsInt()
        {
            return (int)this;
        }

        public uint AsUInt()
        {
            return (uint)this;
        }

        public long AsLong()
        {
            return (long)this;
        }

        public double AsDouble()
        {
            return (double)this;
        }

        public decimal AsDecimal()
        {
            return (decimal)this;
        }

        public static float ToFloat(GameFrameX.Utility.Math.FP value)
        {
            return (float)value;
        }

        public static int ToInt(GameFrameX.Utility.Math.FP value)
        {
            return (int)value;
        }

        public static uint ToUInt(GameFrameX.Utility.Math.FP value)
        {
            return (uint)value;
        }

        public static bool IsInfinity(GameFrameX.Utility.Math.FP value)
        {
            return value == NegativeInfinity || value == PositiveInfinity;
        }

        public static bool IsNaN(GameFrameX.Utility.Math.FP value)
        {
            return value == NaN;
        }

        public override bool Equals(object obj)
        {
            return obj is GameFrameX.Utility.Math.FP _obj && _obj.rawvalue.Equals(rawvalue);
        }

        public override int GetHashCode()
        {
            return rawvalue.GetHashCode();
        }

        public bool Equals(GameFrameX.Utility.Math.FP other)
        {
            return rawvalue.Equals(other.rawvalue);
        }

        public int CompareTo(GameFrameX.Utility.Math.FP other)
        {
            return rawvalue.CompareTo(other.rawvalue);
        }

        public override string ToString()
        {
            return rawvalue.ToString();
        }

        public string ToString(IFormatProvider provider)
        {
            return ((float)this).ToString(provider);
        }
        
        public string ToString(string format)
        {
            return ((float)this).ToString(format);
        }

        public static GameFrameX.Utility.Math.FP FromRaw(long rawValue)
        {
            return new GameFrameX.Utility.Math.FP(rawValue);
        }

        FP(float val)
        {
            this.rawvalue = val;
        }
        
        FP(long val)
        {
            this.rawvalue = val;
        }

        public FP(int rawvalue)
        {
            this.rawvalue = rawvalue;
        }
    }
}
#endif