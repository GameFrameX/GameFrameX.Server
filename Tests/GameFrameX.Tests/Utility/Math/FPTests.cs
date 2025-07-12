#if FP
using GameFrameX.Utility.Math;
using Xunit;

namespace GameFrameX.Tests.Utility.Math;

/// <summary>
/// FP 类的单元测试
/// </summary>
public class FPTests
{
    /// <summary>
    /// 测试 FP 的常量值
    /// </summary>
    [Fact]
    public void Constants_ShouldHaveCorrectValues()
    {
        // Assert
        Assert.Equal(0, FP.Zero.RawValue);
        Assert.Equal(FP.ONE, FP.One.RawValue);
        Assert.Equal(FP.HALF, FP.Half.RawValue);
        Assert.Equal(FP.TEN, FP.Ten.RawValue);
        Assert.Equal(FP.MAX_VALUE, FP.MaxValue.RawValue);
        Assert.Equal(FP.MIN_VALUE + 2, FP.MinValue.RawValue);
        Assert.Equal(FP.MAX_VALUE, FP.PositiveInfinity.RawValue);
        Assert.Equal(FP.MIN_VALUE + 1, FP.NegativeInfinity.RawValue);
        Assert.Equal(FP.MIN_VALUE, FP.NaN.RawValue);
    }

    /// <summary>
    /// 测试从整数构造 FP
    /// </summary>
    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(-1)]
    [InlineData(100)]
    [InlineData(-100)]
    [InlineData(int.MaxValue)]
    [InlineData(int.MinValue)]
    public void Constructor_FromInt_ShouldCreateCorrectValue(int value)
    {
        // Act
        var fp = new FP(value);
        
        // Assert
        Assert.Equal(value * FP.ONE, fp.RawValue);
        Assert.Equal(value, fp.AsInt());
    }

    /// <summary>
    /// 测试从长整数隐式转换
    /// </summary>
    [Theory]
    [InlineData(0L)]
    [InlineData(1L)]
    [InlineData(-1L)]
    [InlineData(1000L)]
    [InlineData(-1000L)]
    public void ImplicitConversion_FromLong_ShouldWork(long value)
    {
        // Act
        FP fp = value;
        
        // Assert
        Assert.Equal(value * FP.ONE, fp.RawValue);
        Assert.Equal(value, fp.AsLong());
    }

    /// <summary>
    /// 测试从十进制显式转换
    /// </summary>
    [Theory]
    [InlineData(0.0)]
    [InlineData(1.0)]
    [InlineData(-1.0)]
    [InlineData(0.5)]
    [InlineData(-0.5)]
    [InlineData(3.14159)]
    [InlineData(-3.14159)]
    public void ExplicitConversion_FromDecimal_ShouldWork(double doubleValue)
    {
        // Arrange
        var decimalValue = (decimal)doubleValue;
        
        // Act
        var fp = (FP)decimalValue;
        var backToDecimal = (decimal)fp;
        
        // Assert
        // 允许一定的精度误差
        Assert.True(System.Math.Abs((double)(backToDecimal - decimalValue)) < 0.0001);
    }

    /// <summary>
    /// 测试加法运算
    /// </summary>
    [Theory]
    [InlineData(0, 0, 0)]
    [InlineData(1, 1, 2)]
    [InlineData(1, -1, 0)]
    [InlineData(-1, -1, -2)]
    [InlineData(100, 200, 300)]
    public void Addition_ShouldWorkCorrectly(int a, int b, int expected)
    {
        // Arrange
        FP fpA = a;
        FP fpB = b;
        FP fpExpected = expected;
        
        // Act
        var result = fpA + fpB;
        
        // Assert
        Assert.Equal(fpExpected.RawValue, result.RawValue);
    }

    /// <summary>
    /// 测试减法运算
    /// </summary>
    [Theory]
    [InlineData(0, 0, 0)]
    [InlineData(1, 1, 0)]
    [InlineData(1, -1, 2)]
    [InlineData(-1, -1, 0)]
    [InlineData(300, 100, 200)]
    public void Subtraction_ShouldWorkCorrectly(int a, int b, int expected)
    {
        // Arrange
        FP fpA = a;
        FP fpB = b;
        FP fpExpected = expected;
        
        // Act
        var result = fpA - fpB;
        
        // Assert
        Assert.Equal(fpExpected.RawValue, result.RawValue);
    }

    /// <summary>
    /// 测试乘法运算
    /// </summary>
    [Theory]
    [InlineData(0, 5, 0)]
    [InlineData(1, 1, 1)]
    [InlineData(2, 3, 6)]
    [InlineData(-2, 3, -6)]
    [InlineData(-2, -3, 6)]
    [InlineData(10, 10, 100)]
    public void Multiplication_ShouldWorkCorrectly(int a, int b, int expected)
    {
        // Arrange
        FP fpA = a;
        FP fpB = b;
        FP fpExpected = expected;
        
        // Act
        var result = fpA * fpB;
        
        // Assert
        Assert.Equal(fpExpected.RawValue, result.RawValue);
    }

    /// <summary>
    /// 测试除法运算
    /// </summary>
    [Theory]
    [InlineData(6, 2, 3)]
    [InlineData(6, 3, 2)]
    [InlineData(-6, 2, -3)]
    [InlineData(-6, -2, 3)]
    [InlineData(100, 10, 10)]
    public void Division_ShouldWorkCorrectly(int a, int b, int expected)
    {
        // Arrange
        FP fpA = a;
        FP fpB = b;
        FP fpExpected = expected;
        
        // Act
        var result = fpA / fpB;
        
        // Assert
        Assert.Equal(fpExpected.RawValue, result.RawValue);
    }

    /// <summary>
    /// 测试除零操作
    /// </summary>
    [Fact]
    public void Division_ByZero_ShouldReturnMaxValue()
    {
        // Arrange
        FP fpA = 1;
        FP fpZero = 0;
        
        // Act
        var result = fpA / fpZero;
        
        // Assert
        Assert.Equal(FP.MAX_VALUE, result.RawValue);
    }

    /// <summary>
    /// 测试取模运算
    /// </summary>
    [Theory]
    [InlineData(7, 3, 1)]
    [InlineData(10, 5, 0)]
    [InlineData(-7, 3, -1)]
    [InlineData(7, -3, 1)]
    public void Modulo_ShouldWorkCorrectly(int a, int b, int expected)
    {
        // Arrange
        FP fpA = a;
        FP fpB = b;
        FP fpExpected = expected;
        
        // Act
        var result = fpA % fpB;
        
        // Assert
        Assert.Equal(fpExpected.RawValue, result.RawValue);
    }

    /// <summary>
    /// 测试比较运算符
    /// </summary>
    [Fact]
    public void ComparisonOperators_ShouldWorkCorrectly()
    {
        // Arrange
        FP fp1 = 1;
        FP fp2 = 2;
        FP fp1Copy = 1;
        
        // Assert
        Assert.True(fp1 < fp2);
        Assert.True(fp2 > fp1);
        Assert.True(fp1 <= fp2);
        Assert.True(fp1 <= fp1Copy);
        Assert.True(fp2 >= fp1);
        Assert.True(fp1 >= fp1Copy);
        Assert.True(fp1 == fp1Copy);
        Assert.True(fp1 != fp2);
        Assert.False(fp1 > fp2);
        Assert.False(fp2 < fp1);
    }

    /// <summary>
    /// 测试 Abs 方法
    /// </summary>
    [Theory]
    [InlineData(5, 5)]
    [InlineData(-5, 5)]
    [InlineData(0, 0)]
    public void Abs_ShouldReturnAbsoluteValue(int input, int expected)
    {
        // Arrange
        FP fp = input;
        FP expectedFp = expected;
        
        // Act
        var result = FP.Abs(fp);
        
        // Assert
        Assert.Equal(expectedFp.RawValue, result.RawValue);
    }

    /// <summary>
    /// 测试 Abs 方法处理 MinValue
    /// </summary>
    [Fact]
    public void Abs_WithMinValue_ShouldReturnMaxValue()
    {
        // Act
        var result = FP.Abs(FP.MinValue);
        
        // Assert
        Assert.Equal(FP.MaxValue.RawValue, result.RawValue);
    }

    /// <summary>
    /// 测试 Sign 方法
    /// </summary>
    [Theory]
    [InlineData(5, 1)]
    [InlineData(-5, -1)]
    [InlineData(0, 0)]
    public void Sign_ShouldReturnCorrectSign(int input, int expected)
    {
        // Arrange
        FP fp = input;
        
        // Act
        var result = FP.Sign(fp);
        
        // Assert
        Assert.Equal(expected, result);
    }

    /// <summary>
    /// 测试 Floor 方法
    /// </summary>
    [Fact]
    public void Floor_ShouldReturnFloorValue()
    {
        // Arrange
        var fp1 = (FP)3.7m;
        var fp2 = (FP)(-3.7m);
        var fp3 = (FP)3.0m;
        
        // Act
        var result1 = FP.Floor(fp1);
        var result2 = FP.Floor(fp2);
        var result3 = FP.Floor(fp3);
        
        // Assert
        Assert.Equal(3, result1.AsInt());
        Assert.Equal(-4, result2.AsInt());
        Assert.Equal(3, result3.AsInt());
    }

    /// <summary>
    /// 测试 Ceiling 方法
    /// </summary>
    [Fact]
    public void Ceiling_ShouldReturnCeilingValue()
    {
        // Arrange
        var fp1 = (FP)3.2m;
        var fp2 = (FP)(-3.2m);
        var fp3 = (FP)3.0m;
        
        // Act
        var result1 = FP.Ceiling(fp1);
        var result2 = FP.Ceiling(fp2);
        var result3 = FP.Ceiling(fp3);
        
        // Assert
        Assert.Equal(4, result1.AsInt());
        Assert.Equal(-3, result2.AsInt());
        Assert.Equal(3, result3.AsInt());
    }

    /// <summary>
    /// 测试 Round 方法
    /// </summary>
    [Fact]
    public void Round_ShouldRoundToNearestEven()
    {
        // Arrange
        var fp1 = (FP)3.5m;
        var fp2 = (FP)4.5m;
        var fp3 = (FP)3.2m;
        var fp4 = (FP)3.7m;
        
        // Act
        var result1 = FP.Round(fp1);
        var result2 = FP.Round(fp2);
        var result3 = FP.Round(fp3);
        var result4 = FP.Round(fp4);
        
        // Assert
        Assert.Equal(4, result1.AsInt()); // 3.5 rounds to 4 (even)
        Assert.Equal(4, result2.AsInt()); // 4.5 rounds to 4 (even)
        Assert.Equal(3, result3.AsInt()); // 3.2 rounds to 3
        Assert.Equal(4, result4.AsInt()); // 3.7 rounds to 4
    }

    /// <summary>
    /// 测试 Max 方法
    /// </summary>
    [Theory]
    [InlineData(5, 3, 5)]
    [InlineData(3, 5, 5)]
    [InlineData(-3, -5, -3)]
    [InlineData(0, 0, 0)]
    public void Max_ShouldReturnMaximumValue(int a, int b, int expected)
    {
        // Arrange
        FP fpA = a;
        FP fpB = b;
        FP fpExpected = expected;
        
        // Act
        var result = FP.Max(fpA, fpB);
        
        // Assert
        Assert.Equal(fpExpected.RawValue, result.RawValue);
    }

    /// <summary>
    /// 测试 Min 方法
    /// </summary>
    [Theory]
    [InlineData(5, 3, 3)]
    [InlineData(3, 5, 3)]
    [InlineData(-3, -5, -5)]
    [InlineData(0, 0, 0)]
    public void Min_ShouldReturnMinimumValue(int a, int b, int expected)
    {
        // Arrange
        FP fpA = a;
        FP fpB = b;
        FP fpExpected = expected;
        
        // Act
        var result = FP.Min(fpA, fpB);
        
        // Assert
        Assert.Equal(fpExpected.RawValue, result.RawValue);
    }

    /// <summary>
    /// 测试 Clamp 方法
    /// </summary>
    [Theory]
    [InlineData(5, 0, 10, 5)]
    [InlineData(-5, 0, 10, 0)]
    [InlineData(15, 0, 10, 10)]
    [InlineData(5, 5, 5, 5)]
    public void Clamp_ShouldClampValueBetweenMinAndMax(int value, int min, int max, int expected)
    {
        // Arrange
        FP fpValue = value;
        FP fpMin = min;
        FP fpMax = max;
        FP fpExpected = expected;
        
        // Act
        var result = FP.Clamp(fpValue, fpMin, fpMax);
        
        // Assert
        Assert.Equal(fpExpected.RawValue, result.RawValue);
    }

    /// <summary>
    /// 测试 IsInfinity 方法
    /// </summary>
    [Fact]
    public void IsInfinity_ShouldDetectInfinityValues()
    {
        // Assert
        Assert.True(FP.IsInfinity(FP.PositiveInfinity));
        Assert.True(FP.IsInfinity(FP.NegativeInfinity));
        Assert.False(FP.IsInfinity(FP.Zero));
        Assert.False(FP.IsInfinity(FP.One));
        Assert.False(FP.IsInfinity(FP.NaN));
    }

    /// <summary>
    /// 测试 IsNaN 方法
    /// </summary>
    [Fact]
    public void IsNaN_ShouldDetectNaNValue()
    {
        // Assert
        Assert.True(FP.IsNaN(FP.NaN));
        Assert.False(FP.IsNaN(FP.Zero));
        Assert.False(FP.IsNaN(FP.One));
        Assert.False(FP.IsNaN(FP.PositiveInfinity));
        Assert.False(FP.IsNaN(FP.NegativeInfinity));
    }

    /// <summary>
    /// 测试 Equals 方法
    /// </summary>
    [Fact]
    public void Equals_ShouldWorkCorrectly()
    {
        // Arrange
        FP fp1 = 5;
        FP fp2 = 5;
        FP fp3 = 3;
        
        // Assert
        Assert.True(fp1.Equals(fp2));
        Assert.False(fp1.Equals(fp3));
        Assert.True(fp1.Equals((object)fp2));
        Assert.False(fp1.Equals((object)fp3));
        Assert.False(fp1.Equals("not an FP"));
    }

    /// <summary>
    /// 测试 CompareTo 方法
    /// </summary>
    [Fact]
    public void CompareTo_ShouldWorkCorrectly()
    {
        // Arrange
        FP fp1 = 3;
        FP fp2 = 5;
        FP fp3 = 3;
        
        // Assert
        Assert.True(fp1.CompareTo(fp2) < 0);
        Assert.True(fp2.CompareTo(fp1) > 0);
        Assert.Equal(0, fp1.CompareTo(fp3));
    }

    /// <summary>
    /// 测试 GetHashCode 方法
    /// </summary>
    [Fact]
    public void GetHashCode_ShouldBeConsistent()
    {
        // Arrange
        FP fp1 = 5;
        FP fp2 = 5;
        FP fp3 = 3;
        
        // Assert
        Assert.Equal(fp1.GetHashCode(), fp2.GetHashCode());
        Assert.NotEqual(fp1.GetHashCode(), fp3.GetHashCode());
    }

    /// <summary>
    /// 测试 ToString 方法
    /// </summary>
    [Fact]
    public void ToString_ShouldReturnStringRepresentation()
    {
        // Arrange
        FP fp = 5;
        
        // Act
        var result = fp.ToString();
        
        // Assert
        Assert.NotNull(result);
        Assert.NotEmpty(result);
        Assert.Contains("5", result);
    }

    /// <summary>
    /// 测试 FromRaw 方法
    /// </summary>
    [Fact]
    public void FromRaw_ShouldCreateFPFromRawValue()
    {
        // Arrange
        long rawValue = FP.ONE * 5; // 5.0 in fixed point
        
        // Act
        var fp = FP.FromRaw(rawValue);
        
        // Assert
        Assert.Equal(rawValue, fp.RawValue);
        Assert.Equal(5, fp.AsInt());
    }

    /// <summary>
    /// 测试 RawValue 属性
    /// </summary>
    [Fact]
    public void RawValue_ShouldReturnUnderlyingValue()
    {
        // Arrange
        FP fp = 5;
        
        // Act
        var rawValue = fp.RawValue;
        
        // Assert
        Assert.Equal(5 * FP.ONE, rawValue);
    }

    /// <summary>
    /// 测试类型转换方法
    /// </summary>
    [Fact]
    public void TypeConversions_ShouldWorkCorrectly()
    {
        // Arrange
        FP fp = 5;
        
        // Act & Assert
        Assert.Equal(5, fp.AsInt());
        Assert.Equal(5u, fp.AsUInt());
        Assert.Equal(5L, fp.AsLong());
        Assert.Equal(5.0f, fp.AsFloat(), precision: 5);
        Assert.Equal(5.0, fp.AsDouble(), precision: 5);
        Assert.Equal(5.0m, fp.AsDecimal());
    }

    /// <summary>
    /// 测试静态转换方法
    /// </summary>
    [Fact]
    public void StaticConversions_ShouldWorkCorrectly()
    {
        // Arrange
        FP fp = 5;
        
        // Act & Assert
        Assert.Equal(5, FP.ToInt(fp));
        Assert.Equal(5u, FP.ToUInt(fp));
        Assert.Equal(5.0f, FP.ToFloat(fp), precision: 5);
    }

    /// <summary>
    /// 测试精度常量
    /// </summary>
    [Fact]
    public void Precision_ShouldHaveCorrectValue()
    {
        // Assert
        Assert.True(FP.Precision > 0);
        Assert.True(FP.Precision < 1);
    }

    /// <summary>
    /// 测试小数常量
    /// </summary>
    [Fact]
    public void DecimalConstants_ShouldHaveCorrectValues()
    {
        // Assert
        Assert.Equal(0.1m, (decimal)FP.EN1, precision: 10);
        Assert.Equal(0.01m, (decimal)FP.EN2, precision: 10);
        Assert.Equal(0.001m, (decimal)FP.EN3, precision: 10);
        Assert.Equal(0.0001m, (decimal)FP.EN4, precision: 10);
        Assert.Equal(0.00001m, (decimal)FP.EN5, precision: 10);
        Assert.Equal(0.000001m, (decimal)FP.EN6, precision: 10);
        Assert.Equal(0.0000001m, (decimal)FP.EN7, precision: 10);
        Assert.Equal(0.00000001m, (decimal)FP.EN8, precision: 10);
    }

    /// <summary>
    /// 测试数学常量
    /// </summary>
    [Fact]
    public void MathConstants_ShouldHaveCorrectValues()
    {
        // Assert
        Assert.True(System.Math.Abs((double)FP.Pi - System.Math.PI) < 0.001);
        Assert.True(System.Math.Abs((double)FP.PiOver2 - System.Math.PI / 2) < 0.001);
        Assert.True(System.Math.Abs((double)FP.PiTimes2 - System.Math.PI * 2) < 0.001);
    }

    /// <summary>
    /// 测试角度弧度转换常量
    /// </summary>
    [Fact]
    public void AngleConversionConstants_ShouldHaveCorrectValues()
    {
        // Arrange
        var degrees180 = (FP)180;
        var radiansPi = FP.Pi;
        
        // Act
        var degreesToRadians = degrees180 * FP.Deg2Rad;
        var radiansTodegrees = radiansPi * FP.Rad2Deg;
        
        // Assert
        Assert.True(System.Math.Abs((double)(degreesToRadians - FP.Pi)) < 0.001);
        Assert.True(System.Math.Abs((double)(radiansTodegrees - degrees180)) < 0.001);
    }

    /// <summary>
    /// 测试 Sqrt 方法
    /// </summary>
    [Theory]
    [InlineData(0, 0)]
    [InlineData(1, 1)]
    [InlineData(4, 2)]
    [InlineData(9, 3)]
    [InlineData(16, 4)]
    [InlineData(25, 5)]
    public void Sqrt_ShouldReturnCorrectSquareRoot(int input, int expected)
    {
        // Arrange
        FP fp = input;
        FP expectedFp = expected;
        
        // Act
        var result = FP.Sqrt(fp);
        
        // Assert
        Assert.Equal(expectedFp.RawValue, result.RawValue);
    }

    /// <summary>
    /// 测试 Sqrt 方法处理负数
    /// </summary>
    [Fact]
    public void Sqrt_WithNegativeValue_ShouldThrowArgumentOutOfRangeException()
    {
        // Arrange
        FP negativeFp = -1;
        
        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() => FP.Sqrt(negativeFp));
    }

    /// <summary>
    /// 测试 Sin 方法
    /// </summary>
    [Fact]
    public void Sin_ShouldReturnCorrectValues()
    {
        // Arrange & Act & Assert
        Assert.True(System.Math.Abs((double)FP.Sin(FP.Zero)) < 0.001);
        Assert.True(System.Math.Abs((double)FP.Sin(FP.PiOver2) - 1.0) < 0.01);
        Assert.True(System.Math.Abs((double)FP.Sin(FP.Pi)) < 0.01);
        Assert.True(System.Math.Abs((double)FP.Sin(FP.PiTimes2)) < 0.01);
        Assert.True(System.Math.Abs((double)FP.Sin(-FP.PiOver2) + 1.0) < 0.01);
    }

    /// <summary>
    /// 测试 FastSin 方法
    /// </summary>
    [Fact]
    public void FastSin_ShouldReturnApproximateValues()
    {
        // Arrange & Act & Assert
        Assert.True(System.Math.Abs((double)FP.FastSin(FP.Zero)) < 0.01);
        Assert.True(System.Math.Abs((double)FP.FastSin(FP.PiOver2) - 1.0) < 0.1);
        Assert.True(System.Math.Abs((double)FP.FastSin(FP.Pi)) < 0.1);
    }

    /// <summary>
    /// 测试 Cos 方法
    /// </summary>
    [Fact]
    public void Cos_ShouldReturnCorrectValues()
    {
        // Arrange & Act & Assert
        Assert.True(System.Math.Abs((double)FP.Cos(FP.Zero) - 1.0) < 0.001);
        Assert.True(System.Math.Abs((double)FP.Cos(FP.PiOver2)) < 0.01);
        Assert.True(System.Math.Abs((double)FP.Cos(FP.Pi) + 1.0) < 0.01);
        Assert.True(System.Math.Abs((double)FP.Cos(FP.PiTimes2) - 1.0) < 0.01);
    }

    /// <summary>
    /// 测试 FastCos 方法
    /// </summary>
    [Fact]
    public void FastCos_ShouldReturnApproximateValues()
    {
        // Arrange & Act & Assert
        Assert.True(System.Math.Abs((double)FP.FastCos(FP.Zero) - 1.0) < 0.01);
        Assert.True(System.Math.Abs((double)FP.FastCos(FP.PiOver2)) < 0.1);
        Assert.True(System.Math.Abs((double)FP.FastCos(FP.Pi) + 1.0) < 0.1);
    }

    /// <summary>
    /// 测试 Tan 方法
    /// </summary>
    [Fact]
    public void Tan_ShouldReturnCorrectValues()
    {
        // Arrange & Act & Assert
        Assert.True(System.Math.Abs((double)FP.Tan(FP.Zero)) < 0.001);
        Assert.True(System.Math.Abs((double)FP.Tan(FP.Pi)) < 0.01);
        // Tan(π/4) ≈ 1
        var piOver4 = FP.Pi / (FP)4;
        Assert.True(System.Math.Abs((double)FP.Tan(piOver4) - 1.0) < 0.1);
    }

    /// <summary>
    /// 测试 Atan 方法
    /// </summary>
    [Fact]
    public void Atan_ShouldReturnCorrectValues()
    {
        // Arrange & Act & Assert
        Assert.True(System.Math.Abs((double)FP.Atan(FP.Zero)) < 0.001);
        Assert.True(System.Math.Abs((double)FP.Atan(FP.One) - System.Math.PI / 4) < 0.01);
        Assert.True(System.Math.Abs((double)FP.Atan(-FP.One) + System.Math.PI / 4) < 0.01);
    }

    /// <summary>
    /// 测试 Atan2 方法
    /// </summary>
    [Fact]
    public void Atan2_ShouldReturnCorrectValues()
    {
        // Arrange & Act & Assert
        Assert.True(System.Math.Abs((double)FP.Atan2(FP.Zero, FP.One)) < 0.001);
        Assert.True(System.Math.Abs((double)FP.Atan2(FP.One, FP.Zero) - System.Math.PI / 2) < 0.01);
        Assert.True(System.Math.Abs((double)FP.Atan2(-FP.One, FP.Zero) + System.Math.PI / 2) < 0.01);
        Assert.True(System.Math.Abs((double)FP.Atan2(FP.One, FP.One) - System.Math.PI / 4) < 0.01);
    }

    /// <summary>
    /// 测试 Asin 方法
    /// </summary>
    [Fact]
    public void Asin_ShouldReturnCorrectValues()
    {
        // Arrange & Act & Assert
        Assert.True(System.Math.Abs((double)FP.Asin(FP.Zero)) < 0.001);
        Assert.True(System.Math.Abs((double)FP.Asin(FP.One) - System.Math.PI / 2) < 0.01);
        Assert.True(System.Math.Abs((double)FP.Asin(-FP.One) + System.Math.PI / 2) < 0.01);
    }

    /// <summary>
    /// 测试 Acos 方法
    /// </summary>
    [Fact]
    public void Acos_ShouldReturnCorrectValues()
    {
        // Arrange & Act & Assert
        Assert.True(System.Math.Abs((double)FP.Acos(FP.One)) < 0.001);
        Assert.True(System.Math.Abs((double)FP.Acos(FP.Zero) - System.Math.PI / 2) < 0.01);
        Assert.True(System.Math.Abs((double)FP.Acos(-FP.One) - System.Math.PI) < 0.01);
    }

    /// <summary>
    /// 测试 Acos 方法处理超出范围的值
    /// </summary>
    [Theory]
    [InlineData(2)]
    [InlineData(-2)]
    public void Acos_WithOutOfRangeValue_ShouldThrowArgumentOutOfRangeException(int value)
    {
        // Arrange
        FP fp = value;
        
        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() => FP.Acos(fp));
    }

    /// <summary>
    /// 测试 FastMod 方法
    /// </summary>
    [Theory]
    [InlineData(7, 3, 1)]
    [InlineData(10, 5, 0)]
    [InlineData(-7, 3, -1)]
    public void FastMod_ShouldWorkCorrectly(int a, int b, int expected)
    {
        // Arrange
        FP fpA = a;
        FP fpB = b;
        FP fpExpected = expected;
        
        // Act
        var result = FP.FastMod(fpA, fpB);
        
        // Assert
        Assert.Equal(fpExpected.RawValue, result.RawValue);
    }

    /// <summary>
    /// 测试一元负号运算符
    /// </summary>
    [Theory]
    [InlineData(5, -5)]
    [InlineData(-5, 5)]
    [InlineData(0, 0)]
    public void UnaryMinus_ShouldReturnNegatedValue(int input, int expected)
    {
        // Arrange
        FP fp = input;
        FP expectedFp = expected;
        
        // Act
        var result = -fp;
        
        // Assert
        Assert.Equal(expectedFp.RawValue, result.RawValue);
    }

    /// <summary>
    /// 测试一元负号运算符处理 MinValue
    /// </summary>
    [Fact]
    public void UnaryMinus_WithMinValue_ShouldReturnMaxValue()
    {
        // Act
        var result = -FP.MinValue;
        
        // Assert
        Assert.Equal(FP.MaxValue.RawValue, result.RawValue);
    }

    /// <summary>
    /// 测试 FastAdd 方法
    /// </summary>
    [Theory]
    [InlineData(1, 2, 3)]
    [InlineData(-1, 1, 0)]
    [InlineData(0, 0, 0)]
    public void FastAdd_ShouldWorkCorrectly(int a, int b, int expected)
    {
        // Arrange
        FP fpA = a;
        FP fpB = b;
        FP fpExpected = expected;
        
        // Act
        var result = FP.FastAdd(fpA, fpB);
        
        // Assert
        Assert.Equal(fpExpected.RawValue, result.RawValue);
    }

    /// <summary>
    /// 测试 FastSub 方法
    /// </summary>
    [Theory]
    [InlineData(3, 1, 2)]
    [InlineData(1, 3, -2)]
    [InlineData(0, 0, 0)]
    public void FastSub_ShouldWorkCorrectly(int a, int b, int expected)
    {
        // Arrange
        FP fpA = a;
        FP fpB = b;
        FP fpExpected = expected;
        
        // Act
        var result = FP.FastSub(fpA, fpB);
        
        // Assert
        Assert.Equal(fpExpected.RawValue, result.RawValue);
    }

    /// <summary>
    /// 测试 FastMul 方法
    /// </summary>
    [Theory]
    [InlineData(2, 3, 6)]
    [InlineData(-2, 3, -6)]
    [InlineData(0, 5, 0)]
    public void FastMul_ShouldWorkCorrectly(int a, int b, int expected)
    {
        // Arrange
        FP fpA = a;
        FP fpB = b;
        FP fpExpected = expected;
        
        // Act
        var result = FP.FastMul(fpA, fpB);
        
        // Assert
        Assert.Equal(fpExpected.RawValue, result.RawValue);
    }

    /// <summary>
    /// 测试 FastAbs 方法
    /// </summary>
    [Theory]
    [InlineData(5, 5)]
    [InlineData(-5, 5)]
    [InlineData(0, 0)]
    public void FastAbs_ShouldReturnAbsoluteValue(int input, int expected)
    {
        // Arrange
        FP fp = input;
        FP expectedFp = expected;
        
        // Act
        var result = FP.FastAbs(fp);
        
        // Assert
        Assert.Equal(expectedFp.RawValue, result.RawValue);
    }

    /// <summary>
    /// 测试数学函数的精度
    /// </summary>
    [Fact]
    public void MathFunctions_ShouldHaveReasonablePrecision()
    {
        // 测试一些基本的三角恒等式
        var angle = FP.Pi / (FP)6; // 30度
        
        // sin²(x) + cos²(x) = 1
        var sin = FP.Sin(angle);
        var cos = FP.Cos(angle);
        var identity = sin * sin + cos * cos;
        Assert.True(System.Math.Abs((double)(identity - FP.One)) < 0.1);
        
        // tan(x) = sin(x) / cos(x)
        var tan = FP.Tan(angle);
        var tanFromSinCos = sin / cos;
        Assert.True(System.Math.Abs((double)(tan - tanFromSinCos)) < 0.1);
    }

    /// <summary>
    /// 测试边界值处理
    /// </summary>
    [Fact]
    public void BoundaryValues_ShouldBeHandledCorrectly()
    {
        // 测试最大值和最小值的运算
        Assert.Equal(FP.MaxValue.RawValue, (FP.MaxValue + FP.Zero).RawValue);
        Assert.Equal(FP.MinValue.RawValue, (FP.MinValue + FP.Zero).RawValue);
        
        // 测试接近零的值
        var verySmall = FP.FromRaw(1);
        Assert.True(verySmall > FP.Zero);
        Assert.True(verySmall < FP.Precision);
    }
}
#endif