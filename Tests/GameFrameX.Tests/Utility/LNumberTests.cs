using System;
using Xunit;
using GameFrameX.Utility;

namespace GameFrameX.Tests.Utility
{
    /// <summary>
    /// LNumber 类的单元测试
    /// </summary>
    public class LNumberTests
    {
        #region 常量和静态字段测试

        /// <summary>
        /// 测试常量值
        /// </summary>
        [Fact]
        public void Constants_ShouldHaveCorrectValues()
        {
            // Assert
            Assert.Equal(14, LNumber.FRACTION_BITS);
            Assert.Equal(562949953421311, LNumber.Max);
            Assert.Equal(9999, LNumber.FMax);
        }

        /// <summary>
        /// 测试静态字段
        /// </summary>
        [Fact]
        public void StaticFields_ShouldHaveCorrectValues()
        {
            // Assert
            Assert.Equal(0, (long)LNumber.zero);
            Assert.Equal(1, (long)LNumber.one);
            Assert.Equal(-1, (long)LNumber.minus_one);
            Assert.Equal(0, (long)LNumber.Zero);
        }

        #endregion

        #region Create 方法测试

        /// <summary>
        /// 测试Create方法创建正数
        /// </summary>
        [Fact]
        public void Create_WithPositiveNumbers_ShouldCreateCorrectly()
        {
            // Arrange & Act
            var number = LNumber.Create(123, 4567);

            // Assert
            Assert.Equal(123, number.Floor);
            Assert.True((double)number > 123.0);
            Assert.True((double)number < 124.0);
        }

        /// <summary>
        /// 测试Create方法创建负数
        /// </summary>
        [Fact]
        public void Create_WithNegativeNumbers_ShouldCreateCorrectly()
        {
            // Arrange & Act
            var number = LNumber.Create(-123, -4567);

            // Assert
            Assert.Equal(-123, number.Ceiling);
            Assert.True((double)number < -123.0);
            Assert.True((double)number > -124.0);
        }

        /// <summary>
        /// 测试Create方法创建零
        /// </summary>
        [Fact]
        public void Create_WithZero_ShouldCreateZero()
        {
            // Arrange & Act
            var number = LNumber.Create(0, 0);

            // Assert
            Assert.Equal(0, (long)number);
            Assert.Equal(0.0, (double)number, precision: 4);
        }

        /// <summary>
        /// 测试Create_Row方法
        /// </summary>
        [Fact]
        public void Create_Row_ShouldCreateFromRawValue()
        {
            // Arrange
            long rawValue = 16384; // 1 << 14

            // Act
            var number = LNumber.Create_Row(rawValue);

            // Assert
            Assert.Equal(1, (long)number);
        }

        #endregion

        #region 属性测试

        /// <summary>
        /// 测试Floor属性
        /// </summary>
        [Fact]
        public void Floor_ShouldReturnCorrectValue()
        {
            // Arrange
            var number = LNumber.Create(123, 7890);

            // Act
            var floor = number.Floor;

            // Assert
            Assert.Equal(123, floor);
        }

        /// <summary>
        /// 测试Ceiling属性
        /// </summary>
        [Fact]
        public void Ceiling_ShouldReturnCorrectValue()
        {
            // Arrange
            var number = LNumber.Create(123, 7890);

            // Act
            var ceiling = number.Ceiling;

            // Assert
            Assert.Equal(124, ceiling);
        }

        /// <summary>
        /// 测试负数的Floor和Ceiling
        /// </summary>
        [Fact]
        public void FloorAndCeiling_WithNegativeNumber_ShouldReturnCorrectValues()
        {
            // Arrange
            var number = LNumber.Create(-123, -7890);

            // Act
            var floor = number.Floor;
            var ceiling = number.Ceiling;

            // Assert
            Assert.Equal(-124, floor);
            Assert.Equal(-123, ceiling);
        }

        #endregion

        #region 算术运算符测试

        /// <summary>
        /// 测试加法运算
        /// </summary>
        [Fact]
        public void Addition_ShouldCalculateCorrectly()
        {
            // Arrange
            var a = LNumber.Create(10, 5000);
            var b = LNumber.Create(20, 3000);

            // Act
            var result = a + b;

            // Assert
            Assert.True((double)result > 30.7);
            Assert.True((double)result < 30.9);
        }

        /// <summary>
        /// 测试减法运算
        /// </summary>
        [Fact]
        public void Subtraction_ShouldCalculateCorrectly()
        {
            // Arrange
            var a = LNumber.Create(20, 8000);
            var b = LNumber.Create(10, 3000);

            // Act
            var result = a - b;

            // Assert
            Assert.True((double)result > 10.4);
            Assert.True((double)result < 10.6);
        }

        /// <summary>
        /// 测试乘法运算
        /// </summary>
        [Fact]
        public void Multiplication_ShouldCalculateCorrectly()
        {
            // Arrange
            var a = LNumber.Create(5, 0);
            var b = LNumber.Create(3, 0);

            // Act
            var result = a * b;

            // Assert
            Assert.Equal(15, (long)result);
        }

        /// <summary>
        /// 测试除法运算
        /// </summary>
        [Fact]
        public void Division_ShouldCalculateCorrectly()
        {
            // Arrange
            var a = LNumber.Create(15, 0);
            var b = LNumber.Create(3, 0);

            // Act
            var result = a / b;

            // Assert
            Assert.Equal(5, (long)result);
        }

        /// <summary>
        /// 测试除零
        /// </summary>
        [Fact]
        public void Division_ByZero_ShouldReturnZero()
        {
            // Arrange
            var a = LNumber.Create(10, 0);
            var b = LNumber.zero;

            // Act
            var result = a / b;

            // Assert
            Assert.Equal(0, (long)result);
        }

        /// <summary>
        /// 测试取模运算
        /// </summary>
        [Fact]
        public void Modulo_ShouldCalculateCorrectly()
        {
            // Arrange
            var a = LNumber.Create(17, 0);
            var b = LNumber.Create(5, 0);

            // Act
            var result = a % b;

            // Assert
            Assert.Equal(2, (long)result);
        }

        /// <summary>
        /// 测试负号运算
        /// </summary>
        [Fact]
        public void Negation_ShouldReturnNegativeValue()
        {
            // Arrange
            var a = LNumber.Create(10, 5000);

            // Act
            var result = -a;

            // Assert
            Assert.True((double)result < -10.4);
            Assert.True((double)result > -10.6);
        }

        #endregion

        #region 比较运算符测试

        /// <summary>
        /// 测试小于运算符
        /// </summary>
        [Fact]
        public void LessThan_ShouldCompareCorrectly()
        {
            // Arrange
            var a = LNumber.Create(5, 0);
            var b = LNumber.Create(10, 0);

            // Act & Assert
            Assert.True(a < b);
            Assert.False(b < a);
            Assert.False(a < a);
        }

        /// <summary>
        /// 测试小于等于运算符
        /// </summary>
        [Fact]
        public void LessThanOrEqual_ShouldCompareCorrectly()
        {
            // Arrange
            var a = LNumber.Create(5, 0);
            var b = LNumber.Create(10, 0);

            // Act & Assert
            Assert.True(a <= b);
            Assert.False(b <= a);
            Assert.True(a <= a);
        }

        /// <summary>
        /// 测试大于运算符
        /// </summary>
        [Fact]
        public void GreaterThan_ShouldCompareCorrectly()
        {
            // Arrange
            var a = LNumber.Create(10, 0);
            var b = LNumber.Create(5, 0);

            // Act & Assert
            Assert.True(a > b);
            Assert.False(b > a);
            Assert.False(a > a);
        }

        /// <summary>
        /// 测试大于等于运算符
        /// </summary>
        [Fact]
        public void GreaterThanOrEqual_ShouldCompareCorrectly()
        {
            // Arrange
            var a = LNumber.Create(10, 0);
            var b = LNumber.Create(5, 0);

            // Act & Assert
            Assert.True(a >= b);
            Assert.False(b >= a);
            Assert.True(a >= a);
        }

        /// <summary>
        /// 测试等于运算符
        /// </summary>
        [Fact]
        public void Equality_ShouldCompareCorrectly()
        {
            // Arrange
            var a = LNumber.Create(10, 5000);
            var b = LNumber.Create(10, 5000);
            var c = LNumber.Create(10, 6000);

            // Act & Assert
            Assert.True(a == b);
            Assert.False(a == c);
        }

        /// <summary>
        /// 测试不等于运算符
        /// </summary>
        [Fact]
        public void Inequality_ShouldCompareCorrectly()
        {
            // Arrange
            var a = LNumber.Create(10, 5000);
            var b = LNumber.Create(10, 5000);
            var c = LNumber.Create(10, 6000);

            // Act & Assert
            Assert.False(a != b);
            Assert.True(a != c);
        }

        #endregion

        #region 类型转换测试

        /// <summary>
        /// 测试转换为long
        /// </summary>
        [Fact]
        public void ExplicitCastToLong_ShouldConvertCorrectly()
        {
            // Arrange
            var number = LNumber.Create(123, 7890);

            // Act
            var longValue = (long)number;

            // Assert
            Assert.Equal(123, longValue);
        }

        /// <summary>
        /// 测试转换为double
        /// </summary>
        [Fact]
        public void ExplicitCastToDouble_ShouldConvertCorrectly()
        {
            // Arrange
            var number = LNumber.Create(123, 5000);

            // Act
            var doubleValue = (double)number;

            // Assert
            Assert.True(doubleValue > 123.4);
            Assert.True(doubleValue < 123.6);
        }

        /// <summary>
        /// 测试转换为float
        /// </summary>
        [Fact]
        public void ImplicitCastToFloat_ShouldConvertCorrectly()
        {
            // Arrange
            var number = LNumber.Create(123, 5000);

            // Act
            float floatValue = number;

            // Assert
            Assert.True(floatValue > 123.4f);
            Assert.True(floatValue < 123.6f);
        }

        /// <summary>
        /// 测试从long隐式转换
        /// </summary>
        [Fact]
        public void ImplicitCastFromLong_ShouldConvertCorrectly()
        {
            // Arrange
            long longValue = 123;

            // Act
            LNumber number = longValue;

            // Assert
            Assert.Equal(123, (long)number);
        }

        /// <summary>
        /// 测试从int隐式转换
        /// </summary>
        [Fact]
        public void ImplicitCastFromInt_ShouldConvertCorrectly()
        {
            // Arrange
            int intValue = 123;

            // Act
            LNumber number = intValue;

            // Assert
            Assert.Equal(123, (long)number);
        }

        #endregion

        #region IComparable和IEquatable接口测试

        /// <summary>
        /// 测试CompareTo方法
        /// </summary>
        [Fact]
        public void CompareTo_ShouldReturnCorrectValues()
        {
            // Arrange
            var a = LNumber.Create(5, 0);
            var b = LNumber.Create(10, 0);
            var c = LNumber.Create(5, 0);

            // Act & Assert
            Assert.True(a.CompareTo(b) < 0);
            Assert.True(b.CompareTo(a) > 0);
            Assert.Equal(0, a.CompareTo(c));
        }

        /// <summary>
        /// 测试Equals方法
        /// </summary>
        [Fact]
        public void Equals_ShouldReturnCorrectValues()
        {
            // Arrange
            var a = LNumber.Create(10, 5000);
            var b = LNumber.Create(10, 5000);
            var c = LNumber.Create(10, 6000);
            var d = "not a number";

            // Act & Assert
            Assert.True(a.Equals(b));
            Assert.False(a.Equals(c));
            Assert.False(a.Equals(d));
        }

        #endregion

        #region ToString方法测试

        /// <summary>
        /// 测试ToString方法
        /// </summary>
        [Fact]
        public void ToString_ShouldReturnFormattedString()
        {
            // Arrange
            var number = LNumber.Create(123, 5000);

            // Act
            var result = number.ToString();

            // Assert
            Assert.NotNull(result);
            Assert.Contains("123", result);
        }

        /// <summary>
        /// 测试带格式的ToString方法
        /// </summary>
        [Fact]
        public void ToString_WithFormat_ShouldReturnFormattedString()
        {
            // Arrange
            var number = LNumber.Create(123, 5000);

            // Act
            var result = number.ToString("f2");

            // Assert
            Assert.NotNull(result);
            Assert.Contains("123", result);
        }

        #endregion

        #region GetHashCode测试

        /// <summary>
        /// 测试GetHashCode方法
        /// </summary>
        [Fact]
        public void GetHashCode_ShouldReturnConsistentValue()
        {
            // Arrange
            var a = LNumber.Create(123, 5000);
            var b = LNumber.Create(123, 5000);

            // Act
            var hashA = a.GetHashCode();
            var hashB = b.GetHashCode();

            // Assert
            Assert.Equal(hashA, hashB);
        }

        #endregion

        #region 边界和特殊情况测试

        /// <summary>
        /// 测试最大值和最小值
        /// </summary>
        [Fact]
        public void MaxAndMinValues_ShouldBeHandledCorrectly()
        {
            // Act & Assert
            Assert.True(LNumber.MaxValue > LNumber.zero);
            Assert.True(LNumber.MinValue < LNumber.zero);
            Assert.True(LNumber.MaxValue > LNumber.MinValue);
        }

        /// <summary>
        /// 测试epsilon值
        /// </summary>
        [Fact]
        public void Epsilon_ShouldBeVerySmall()
        {
            // Act & Assert
            Assert.True(LNumber.epsilon > LNumber.zero);
            Assert.True(LNumber.epsilon < LNumber.one);
        }

        /// <summary>
        /// 测试精度
        /// </summary>
        [Fact]
        public void Precision_ShouldBeAccurate()
        {
            // Arrange
            var a = LNumber.Create(1, 0);
            var b = LNumber.epsilon;

            // Act
            var sum = a + b;

            // Assert
            Assert.True(sum > a);
            Assert.True(sum != a);
        }

        /// <summary>
        /// 测试连续运算的精度
        /// </summary>
        [Fact]
        public void ContinuousOperations_ShouldMaintainPrecision()
        {
            // Arrange
            var a = LNumber.Create(10, 0);
            var b = LNumber.Create(3, 0);

            // Act
            var result = (a / b) * b;

            // Assert
            // 由于精度限制，结果可能不完全等于原值，但应该很接近
            var difference = a - result;
            Assert.True((double)difference < 0.1);
        }

        #endregion
    }
}