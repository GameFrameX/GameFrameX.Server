using System;
using Xunit;
using GameFrameX.Utility.Math;

namespace GameFrameX.Tests.Utility.Math
{
    /// <summary>
    /// FPMath 类的单元测试
    /// </summary>
    public class FPMathTests
    {
        #region 常量测试

        [Fact]
        public void Constants_ShouldHaveCorrectValues()
        {
            // 测试数学常量
            Assert.True(FPMath.Abs(FP.Pi - FPMath.Pi) < (FP)0.00001);
            Assert.True(FPMath.Abs(FP.PiOver2 - FPMath.PiOver2) < (FP)0.00001);
            Assert.True(FPMath.Abs(FP.Epsilon - FPMath.Epsilon) < (FP)0.00001);
            Assert.True(FPMath.Abs(FP.Deg2Rad - FPMath.Deg2Rad) < (FP)0.00001);
            Assert.True(FPMath.Abs(FP.Rad2Deg - FPMath.Rad2Deg) < (FP)0.00001);
            Assert.True(FPMath.Abs(FP.MaxValue - FPMath.Infinity) < (FP)0.00001);
        }

        #endregion

        #region 基本数学函数测试

        [Fact]
        public void Sqrt_WithValidInput_ShouldReturnCorrectValue()
        {
            // 测试平方根
            Assert.True(FPMath.Abs(FP.Zero - FPMath.Sqrt(FP.Zero)) < (FP)0.00001);
            Assert.True(FPMath.Abs(FP.One - FPMath.Sqrt(FP.One)) < (FP)0.00001);
            Assert.True(FPMath.Abs((FP)2 - FPMath.Sqrt((FP)4)) < (FP)0.00001);
            Assert.True(FPMath.Abs((FP)3 - FPMath.Sqrt((FP)9)) < (FP)0.00001);
        }

        [Fact]
        public void Max_WithTwoValues_ShouldReturnLargerValue()
        {
            Assert.True(FPMath.Abs((FP)5 - FPMath.Max((FP)3, (FP)5)) < (FP)0.00001);
            Assert.True(FPMath.Abs((FP)5 - FPMath.Max((FP)5, (FP)3)) < (FP)0.00001);
            Assert.True(FPMath.Abs((FP)0 - FPMath.Max((FP)(-1), (FP)0)) < (FP)0.00001);
            Assert.True(FPMath.Abs(FP.One - FPMath.Max(FP.One, FP.One)) < (FP)0.00001);
        }

        [Fact]
        public void Max_WithThreeValues_ShouldReturnLargestValue()
        {
            Assert.True(FPMath.Abs((FP)5 - FPMath.Max((FP)1, (FP)3, (FP)5)) < (FP)0.00001);
            Assert.True(FPMath.Abs((FP)5 - FPMath.Max((FP)5, (FP)3, (FP)1)) < (FP)0.00001);
            Assert.True(FPMath.Abs((FP)5 - FPMath.Max((FP)3, (FP)5, (FP)1)) < (FP)0.00001);
            Assert.True(FPMath.Abs((FP)0 - FPMath.Max((FP)(-3), (FP)(-1), (FP)0)) < (FP)0.00001);
        }

        [Fact]
        public void Min_WithTwoValues_ShouldReturnSmallerValue()
        {
            Assert.True(FPMath.Abs((FP)3 - FPMath.Min((FP)3, (FP)5)) < (FP)0.00001);
            Assert.True(FPMath.Abs((FP)3 - FPMath.Min((FP)5, (FP)3)) < (FP)0.00001);
            Assert.True(FPMath.Abs((FP)(-1) - FPMath.Min((FP)(-1), (FP)0)) < (FP)0.00001);
            Assert.True(FPMath.Abs(FP.One - FPMath.Min(FP.One, FP.One)) < (FP)0.00001);
        }

        [Fact]
        public void Min_WithThreeValues_ShouldReturnSmallestValue()
        {
            Assert.True(FPMath.Abs((FP)1 - FPMath.Min(FPMath.Min((FP)1, (FP)3), (FP)5)) < (FP)0.00001);
            Assert.True(FPMath.Abs((FP)1 - FPMath.Min(FPMath.Min((FP)5, (FP)3), (FP)1)) < (FP)0.00001);
            Assert.True(FPMath.Abs((FP)1 - FPMath.Min(FPMath.Min((FP)3, (FP)5), (FP)1)) < (FP)0.00001);
            Assert.True(FPMath.Abs((FP)(-3) - FPMath.Min(FPMath.Min((FP)(-3), (FP)(-1)), (FP)0)) < (FP)0.00001);
        }

        [Fact]
        public void Clamp_ShouldLimitValueWithinRange()
        {
            Assert.True(FPMath.Abs((FP)5 - FPMath.Clamp((FP)3, (FP)5, (FP)10)) < (FP)0.00001);
            Assert.True(FPMath.Abs((FP)10 - FPMath.Clamp((FP)15, (FP)5, (FP)10)) < (FP)0.00001);
            Assert.True(FPMath.Abs((FP)7 - FPMath.Clamp((FP)7, (FP)5, (FP)10)) < (FP)0.00001);
            Assert.True(FPMath.Abs(FP.Zero - FPMath.Clamp((FP)(-1), FP.Zero, FP.One)) < (FP)0.00001);
        }

        [Fact]
        public void Clamp01_ShouldLimitValueBetweenZeroAndOne()
        {
            Assert.True(FPMath.Abs(FP.Zero - FPMath.Clamp01((FP)(-0.5))) < (FP)0.00001);
            Assert.True(FPMath.Abs(FP.One - FPMath.Clamp01((FP)1.5)) < (FP)0.00001);
            Assert.True(FPMath.Abs((FP)0.5 - FPMath.Clamp01((FP)0.5)) < (FP)0.00001);
            Assert.True(FPMath.Abs(FP.Zero - FPMath.Clamp01(FP.Zero)) < (FP)0.00001);
            Assert.True(FPMath.Abs(FP.One - FPMath.Clamp01(FP.One)) < (FP)0.00001);
        }

        [Fact]
        public void Abs_ShouldReturnAbsoluteValue()
        {
            Assert.True(FPMath.Abs((FP)5 - FPMath.Abs((FP)5)) < (FP)0.00001);
            Assert.True(FPMath.Abs((FP)5 - FPMath.Abs((FP)(-5))) < (FP)0.00001);
            Assert.True(FPMath.Abs(FP.Zero - FPMath.Abs(FP.Zero)) < (FP)0.00001);
            Assert.True(FPMath.Abs(FP.One - FPMath.Abs(FP.One)) < (FP)0.00001);
            Assert.True(FPMath.Abs(FP.One - FPMath.Abs(-FP.One)) < (FP)0.00001);
        }

        [Fact]
        public void Sign_ShouldReturnCorrectSign()
        {
            Assert.True(FPMath.Abs(FP.One - FPMath.Sign((FP)5)) < (FP)0.00001);
            Assert.True(FPMath.Abs(-FP.One - FPMath.Sign((FP)(-5))) < (FP)0.00001);
            Assert.True(FPMath.Abs(FP.Zero - FPMath.Sign(FP.Zero)) < (FP)0.00001);
        }

        [Fact]
        public void Floor_ShouldReturnFloorValue()
        {
            Assert.True(FPMath.Abs((FP)3 - FPMath.Floor((FP)3.7)) < (FP)0.00001);
            Assert.True(FPMath.Abs((FP)(-4) - FPMath.Floor((FP)(-3.2))) < (FP)0.00001);
            Assert.True(FPMath.Abs(FP.Zero - FPMath.Floor((FP)0.9)) < (FP)0.00001);
            Assert.True(FPMath.Abs((FP)5 - FPMath.Floor((FP)5)) < (FP)0.00001);
        }

        [Fact]
        public void Ceiling_ShouldReturnCeilingValue()
        {
            Assert.True(FPMath.Abs((FP)4 - FPMath.Ceiling((FP)3.2)) < (FP)0.00001);
            Assert.True(FPMath.Abs((FP)(-3) - FPMath.Ceiling((FP)(-3.7))) < (FP)0.00001);
            Assert.True(FPMath.Abs(FP.One - FPMath.Ceiling((FP)0.1)) < (FP)0.00001);
            Assert.True(FPMath.Abs((FP)5 - FPMath.Ceiling((FP)5)) < (FP)0.00001);
        }

        [Fact]
        public void Round_ShouldReturnRoundedValue()
        {
            Assert.True(FPMath.Abs((FP)4 - FPMath.Round((FP)3.6)) < (FP)0.00001);
            Assert.True(FPMath.Abs((FP)3 - FPMath.Round((FP)3.4)) < (FP)0.00001);
            Assert.True(FPMath.Abs((FP)(-4) - FPMath.Round((FP)(-3.6))) < (FP)0.00001);
            Assert.True(FPMath.Abs((FP)5 - FPMath.Round((FP)5)) < (FP)0.00001);
        }

        #endregion

        #region 三角函数测试

        [Fact]
        public void Sin_ShouldReturnCorrectValue()
        {
            Assert.True(FPMath.Abs(FP.Zero - FPMath.Sin(FP.Zero)) < (FP)0.00001);
            Assert.True(FPMath.Abs(FP.One - FPMath.Sin(FPMath.PiOver2)) < (FP)0.00001);
            Assert.True(FPMath.Abs(FP.Zero - FPMath.Sin(FPMath.Pi)) < (FP)0.00001);
            Assert.True(FPMath.Abs(-FP.One - FPMath.Sin(FPMath.Pi + FPMath.PiOver2)) < (FP)0.00001);
        }

        [Fact]
        public void Cos_ShouldReturnCorrectValue()
        {
            Assert.True(FPMath.Abs(FP.One - FPMath.Cos(FP.Zero)) < (FP)0.00001);
            Assert.True(FPMath.Abs(FP.Zero - FPMath.Cos(FPMath.PiOver2)) < (FP)0.00001);
            Assert.True(FPMath.Abs(-FP.One - FPMath.Cos(FPMath.Pi)) < (FP)0.00001);
            Assert.True(FPMath.Abs(FP.Zero - FPMath.Cos(FPMath.Pi + FPMath.PiOver2)) < (FP)0.00001);
        }

        [Fact]
        public void Tan_ShouldReturnCorrectValue()
        {
            Assert.True(FPMath.Abs(FP.Zero - FPMath.Tan(FP.Zero)) < (FP)0.00001);
            Assert.True(FPMath.Abs(FP.Zero - FPMath.Tan(FPMath.Pi)) < (FP)0.00001);
            // Tan(π/4) ≈ 1
            Assert.True(FPMath.Abs(FP.One - FPMath.Tan(FPMath.Pi / (FP)4)) < (FP)0.1);
        }

        [Fact]
        public void Asin_ShouldReturnCorrectValue()
        {
            Assert.True(FPMath.Abs(FP.Zero - FPMath.Asin(FP.Zero)) < (FP)0.00001);
            Assert.True(FPMath.Abs(FPMath.PiOver2 - FPMath.Asin(FP.One)) < (FP)0.00001);
            Assert.True(FPMath.Abs(-FPMath.PiOver2 - FPMath.Asin(-FP.One)) < (FP)0.00001);
        }

        [Fact]
        public void Acos_ShouldReturnCorrectValue()
        {
            Assert.True(FPMath.Abs(FPMath.PiOver2 - FPMath.Acos(FP.Zero)) < (FP)0.00001);
            Assert.True(FPMath.Abs(FP.Zero - FPMath.Acos(FP.One)) < (FP)0.00001);
            Assert.True(FPMath.Abs(FPMath.Pi - FPMath.Acos(-FP.One)) < (FP)0.00001);
        }

        [Fact]
        public void Atan_ShouldReturnCorrectValue()
        {
            Assert.True(FPMath.Abs(FP.Zero - FPMath.Atan(FP.Zero)) < (FP)0.00001);
            Assert.True(FPMath.Abs(FPMath.Pi / (FP)4 - FPMath.Atan(FP.One)) < (FP)0.001);
            Assert.True(FPMath.Abs(-FPMath.Pi / (FP)4 - FPMath.Atan(-FP.One)) < (FP)0.001);
        }

        [Fact]
        public void Atan2_ShouldReturnCorrectValue()
        {
            Assert.True(FPMath.Abs(FP.Zero - FPMath.Atan2(FP.Zero, FP.One)) < (FP)0.00001);
            Assert.True(FPMath.Abs(FPMath.PiOver2 - FPMath.Atan2(FP.One, FP.Zero)) < (FP)0.00001);
            Assert.True(FPMath.Abs(FPMath.Pi - FPMath.Atan2(FP.Zero, -FP.One)) < (FP)0.00001);
            Assert.True(FPMath.Abs(-FPMath.PiOver2 - FPMath.Atan2(-FP.One, FP.Zero)) < (FP)0.00001);
        }

        #endregion

        #region 插值函数测试

        [Fact]
        public void Barycentric_ShouldReturnCorrectValue()
        {
            var value1 = (FP)1;
            var value2 = (FP)2;
            var value3 = (FP)3;
            var amount1 = (FP)0.5;
            var amount2 = (FP)0.3;

            var result = FPMath.Barycentric(value1, value2, value3, amount1, amount2);
            var expected = value1 + amount1 * (value2 - value1) + amount2 * (value3 - value1);
            Assert.True(FPMath.Abs(expected - result) < (FP)0.00001);
        }

        [Fact]
        public void CatmullRom_ShouldReturnCorrectValue()
        {
            var value1 = (FP)1;
            var value2 = (FP)2;
            var value3 = (FP)3;
            var value4 = (FP)4;
            var amount = (FP)0.5;

            var result = FPMath.CatmullRom(value1, value2, value3, value4, amount);
            // 在 amount = 0.5 时，应该接近 value2 和 value3 的中点
            Assert.True(result > value2 && result < value3);
        }

        [Fact]
        public void Distance_ShouldReturnCorrectValue()
        {
            Assert.True(FPMath.Abs((FP)5 - FPMath.Distance((FP)1, (FP)6)) < (FP)0.00001);
            Assert.True(FPMath.Abs((FP)5 - FPMath.Distance((FP)6, (FP)1)) < (FP)0.00001);
            Assert.True(FPMath.Abs(FP.Zero - FPMath.Distance((FP)3, (FP)3)) < (FP)0.00001);
            Assert.True(FPMath.Abs((FP)10 - FPMath.Distance((FP)(-5), (FP)5)) < (FP)0.00001);
        }

        [Fact]
        public void Hermite_ShouldReturnCorrectValue()
        {
            var value1 = (FP)0;
            var tangent1 = (FP)1;
            var value2 = (FP)1;
            var tangent2 = (FP)1;
            var amount = (FP)0.5;

            var result = FPMath.Hermite(value1, tangent1, value2, tangent2, amount);
            // 在 amount = 0.5 时，应该在 value1 和 value2 之间
            Assert.True(result >= value1 && result <= value2);
        }

        [Fact]
        public void Lerp_ShouldReturnCorrectValue()
        {
            Assert.True(FPMath.Abs((FP)1 - FPMath.Lerp((FP)1, (FP)3, FP.Zero)) < (FP)0.00001);
            Assert.True(FPMath.Abs((FP)3 - FPMath.Lerp((FP)1, (FP)3, FP.One)) < (FP)0.00001);
            Assert.True(FPMath.Abs((FP)2 - FPMath.Lerp((FP)1, (FP)3, (FP)0.5)) < (FP)0.00001);
            Assert.True(FPMath.Abs((FP)1.5 - FPMath.Lerp((FP)1, (FP)2, (FP)0.5)) < (FP)0.00001);
        }

        [Fact]
        public void Lerp_WithAmountOutOfRange_ShouldClamp()
        {
            // amount > 1 应该被限制为 1
            Assert.True(FPMath.Abs((FP)3 - FPMath.Lerp((FP)1, (FP)3, (FP)1.5)) < (FP)0.00001);
            // amount < 0 应该被限制为 0
            Assert.True(FPMath.Abs((FP)1 - FPMath.Lerp((FP)1, (FP)3, (FP)(-0.5))) < (FP)0.00001);
        }

        [Fact]
        public void InverseLerp_ShouldReturnCorrectValue()
        {
            Assert.True(FPMath.Abs(FP.Zero - FPMath.InverseLerp((FP)1, (FP)3, (FP)1)) < (FP)0.00001);
            Assert.True(FPMath.Abs(FP.One - FPMath.InverseLerp((FP)1, (FP)3, (FP)3)) < (FP)0.00001);
            Assert.True(FPMath.Abs((FP)0.5 - FPMath.InverseLerp((FP)1, (FP)3, (FP)2)) < (FP)0.00001);
        }

        [Fact]
        public void InverseLerp_WithSameValues_ShouldReturnZero()
        {
            Assert.True(FPMath.Abs(FP.Zero - FPMath.InverseLerp((FP)5, (FP)5, (FP)5)) < (FP)0.00001);
            Assert.True(FPMath.Abs(FP.Zero - FPMath.InverseLerp((FP)5, (FP)5, (FP)10)) < (FP)0.00001);
        }

        [Fact]
        public void SmoothStep_ShouldReturnCorrectValue()
        {
            Assert.True(FPMath.Abs((FP)1 - FPMath.SmoothStep((FP)1, (FP)3, FP.Zero)) < (FP)0.00001);
            Assert.True(FPMath.Abs((FP)3 - FPMath.SmoothStep((FP)1, (FP)3, FP.One)) < (FP)0.00001);
            
            var result = FPMath.SmoothStep((FP)1, (FP)3, (FP)0.5);
            Assert.True(result > (FP)1 && result < (FP)3);
        }

        #endregion

        #region 幂函数测试

        [Fact]
        public void Pow_ShouldReturnCorrectValue()
        {
            Assert.True(FPMath.Abs(FP.One - FPMath.Pow(FP.One, (FP)100)) < (FP)0.00001); // 1^100 = 1
            Assert.True(FPMath.Abs(FP.One - FPMath.Pow((FP)5, FP.Zero)) < (FP)0.00001); // 5^0 = 1
            Assert.True(FPMath.Abs((FP)8 - FPMath.Pow((FP)2, (FP)3)) < (FP)0.00001); // 2^3 = 8
            Assert.True(FPMath.Abs((FP)25 - FPMath.Pow((FP)5, (FP)2)) < (FP)0.00001); // 5^2 = 25
        }

        [Fact]
        public void Pow_WithZeroBase_ShouldReturnCorrectValue()
        {
            Assert.True(FPMath.Abs(FP.Zero - FPMath.Pow(FP.Zero, (FP)5)) < (FP)0.00001); // 0^5 = 0
            Assert.True(FPMath.Abs(FP.One - FPMath.Pow(FP.Zero, FP.Zero)) < (FP)0.00001); // 0^0 = 1
            Assert.True(FPMath.Abs(FP.MaxValue - FPMath.Pow(FP.Zero, (FP)(-1))) < (FP)0.00001); // 0^(-1) = MaxValue (代替无穷大)
        }

        #endregion

        #region 运动函数测试

        [Fact]
        public void MoveTowards_ShouldReturnCorrectValue()
        {
            Assert.True(FPMath.Abs((FP)3 - FPMath.MoveTowards((FP)1, (FP)5, (FP)2)) < (FP)0.00001); // 1 向 5 移动 2 = 3
            Assert.True(FPMath.Abs((FP)5 - FPMath.MoveTowards((FP)1, (FP)5, (FP)10)) < (FP)0.00001); // 移动距离超过目标，返回目标
            Assert.True(FPMath.Abs((FP)3 - FPMath.MoveTowards((FP)5, (FP)1, (FP)2)) < (FP)0.00001); // 5 向 1 移动 2 = 3
            Assert.True(FPMath.Abs((FP)5 - FPMath.MoveTowards((FP)5, (FP)5, (FP)2)) < (FP)0.00001); // 已在目标位置
        }

        [Fact]
        public void Repeat_ShouldReturnCorrectValue()
        {
            Assert.True(FPMath.Abs((FP)1 - FPMath.Repeat((FP)6, (FP)5)) < (FP)0.00001); // 6 % 5 = 1
            Assert.True(FPMath.Abs((FP)0 - FPMath.Repeat((FP)5, (FP)5)) < (FP)0.00001); // 5 % 5 = 0
            Assert.True(FPMath.Abs((FP)2.5 - FPMath.Repeat((FP)7.5, (FP)5)) < (FP)0.00001); // 7.5 % 5 = 2.5
            Assert.True(FPMath.Abs((FP)0 - FPMath.Repeat(FP.Zero, (FP)5)) < (FP)0.00001); // 0 % 5 = 0
        }

        [Fact]
        public void DeltaAngle_ShouldReturnCorrectValue()
        {
            Assert.True(FPMath.Abs((FP)90 - FPMath.DeltaAngle(FP.Zero, (FP)90)) < (FP)0.00001);
            Assert.True(FPMath.Abs((FP)(-90) - FPMath.DeltaAngle((FP)90, FP.Zero)) < (FP)0.00001);
            Assert.True(FPMath.Abs((FP)(-90) - FPMath.DeltaAngle((FP)10, (FP)280)) < (FP)0.00001); // 跨越 360 度边界
            Assert.True(FPMath.Abs(FP.Zero - FPMath.DeltaAngle((FP)180, (FP)180)) < (FP)0.00001);
        }

        [Fact]
        public void MoveTowardsAngle_ShouldReturnCorrectValue()
        {
            Assert.True(FPMath.Abs((FP)45 - FPMath.MoveTowardsAngle(FP.Zero, (FP)90, (FP)45)) < (FP)0.00001);
            Assert.True(FPMath.Abs((FP)90 - FPMath.MoveTowardsAngle(FP.Zero, (FP)90, (FP)100)) < (FP)0.00001); // 超过目标
            Assert.True(FPMath.Abs((FP)350 - FPMath.MoveTowardsAngle((FP)10, (FP)350, (FP)30)) < (FP)0.00001); // 跨越边界
        }

        [Fact]
        public void SmoothDamp_ShouldReturnCorrectValue()
        {
            var velocity = FP.Zero;
            var result = FPMath.SmoothDamp(FP.Zero, (FP)10, ref velocity, (FP)1);
            
            // 结果应该在起始值和目标值之间
            Assert.True(result > FP.Zero && result < (FP)10);
            // 速度应该被更新
            Assert.NotEqual(FP.Zero, velocity);
        }

        [Fact]
        public void SmoothDamp_WithMaxSpeed_ShouldReturnCorrectValue()
        {
            var velocity = FP.Zero;
            var result = FPMath.SmoothDamp(FP.Zero, (FP)10, ref velocity, (FP)1, (FP)5);
            
            // 结果应该在起始值和目标值之间
            Assert.True(result > FP.Zero && result < (FP)10);
        }

        [Fact]
        public void SmoothDamp_WithDeltaTime_ShouldReturnCorrectValue()
        {
            var velocity = FP.Zero;
            var result = FPMath.SmoothDamp(FP.Zero, (FP)10, ref velocity, (FP)1, (FP)5, (FP)0.1);
            
            // 结果应该在起始值和目标值之间
            Assert.True(result > FP.Zero && result < (FP)10);
        }

        #endregion
    }
}