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
            Assert.True(FPMath.Abs(FP.Pi - FPMath.Pi) < (FP)0.00001m);
            Assert.True(FPMath.Abs(FP.PiOver2 - FPMath.PiOver2) < (FP)0.00001m);
            Assert.True(FPMath.Abs(FP.Epsilon - FPMath.Epsilon) < (FP)0.00001m);
            Assert.True(FPMath.Abs(FP.Deg2Rad - FPMath.Deg2Rad) < (FP)0.00001m);
            Assert.True(FPMath.Abs(FP.Rad2Deg - FPMath.Rad2Deg) < (FP)0.00001m);
            Assert.True(FPMath.Abs(FP.MaxValue - FPMath.Infinity) < (FP)0.00001m);
        }

        #endregion

        #region 基本数学函数测试

        [Fact]
        public void Sqrt_WithValidInput_ShouldReturnCorrectValue()
        {
            // 测试平方根
            Assert.True(FPMath.Abs(FP.Zero - FPMath.Sqrt(FP.Zero)) < (FP)0.00001m);
            Assert.True(FPMath.Abs(FP.One - FPMath.Sqrt(FP.One)) < (FP)0.00001m);
            Assert.True(FPMath.Abs((FP)2 - FPMath.Sqrt((FP)4)) < (FP)0.00001m);
            Assert.True(FPMath.Abs((FP)3 - FPMath.Sqrt((FP)9)) < (FP)0.00001m);
        }

        [Fact]
        public void Max_WithTwoValues_ShouldReturnLargerValue()
        {
            Assert.True(FPMath.Abs((FP)5 - FPMath.Max((FP)3, (FP)5)) < (FP)0.00001m);
            Assert.True(FPMath.Abs((FP)5 - FPMath.Max((FP)5, (FP)3)) < (FP)0.00001m);
            Assert.True(FPMath.Abs((FP)0 - FPMath.Max((FP)(-1), (FP)0)) < (FP)0.00001m);
            Assert.True(FPMath.Abs(FP.One - FPMath.Max(FP.One, FP.One)) < (FP)0.00001m);
        }

        [Fact]
        public void Max_WithThreeValues_ShouldReturnLargestValue()
        {
            Assert.True(FPMath.Abs((FP)5 - FPMath.Max((FP)1, (FP)3, (FP)5)) < (FP)0.00001m);
            Assert.True(FPMath.Abs((FP)5 - FPMath.Max((FP)5, (FP)3, (FP)1)) < (FP)0.00001m);
            Assert.True(FPMath.Abs((FP)5 - FPMath.Max((FP)3, (FP)5, (FP)1)) < (FP)0.00001m);
            Assert.True(FPMath.Abs((FP)0 - FPMath.Max((FP)(-3), (FP)(-1), (FP)0)) < (FP)0.00001m);
        }

        [Fact]
        public void Min_WithTwoValues_ShouldReturnSmallerValue()
        {
            Assert.True(FPMath.Abs((FP)3 - FPMath.Min((FP)3, (FP)5)) < (FP)0.00001m);
            Assert.True(FPMath.Abs((FP)3 - FPMath.Min((FP)5, (FP)3)) < (FP)0.00001m);
            Assert.True(FPMath.Abs((FP)(-1) - FPMath.Min((FP)(-1), (FP)0)) < (FP)0.00001m);
            Assert.True(FPMath.Abs(FP.One - FPMath.Min(FP.One, FP.One)) < (FP)0.00001m);
        }

        [Fact]
        public void Min_WithThreeValues_ShouldReturnSmallestValue()
        {
            Assert.True(FPMath.Abs((FP)1 - FPMath.Min(FPMath.Min((FP)1, (FP)3), (FP)5)) < (FP)0.00001m);
            Assert.True(FPMath.Abs((FP)1 - FPMath.Min(FPMath.Min((FP)5, (FP)3), (FP)1)) < (FP)0.00001m);
            Assert.True(FPMath.Abs((FP)1 - FPMath.Min(FPMath.Min((FP)3, (FP)5), (FP)1)) < (FP)0.00001m);
            Assert.True(FPMath.Abs((FP)(-3) - FPMath.Min(FPMath.Min((FP)(-3), (FP)(-1)), (FP)0)) < (FP)0.00001m);
        }

        [Fact]
        public void Clamp_ShouldLimitValueWithinRange()
        {
            Assert.True(FPMath.Abs((FP)5 - FPMath.Clamp((FP)3, (FP)5, (FP)10)) < (FP)0.00001m);
            Assert.True(FPMath.Abs((FP)10 - FPMath.Clamp((FP)15, (FP)5, (FP)10)) < (FP)0.00001m);
            Assert.True(FPMath.Abs((FP)7 - FPMath.Clamp((FP)7, (FP)5, (FP)10)) < (FP)0.00001m);
            Assert.True(FPMath.Abs(FP.Zero - FPMath.Clamp((FP)(-1), FP.Zero, FP.One)) < (FP)0.00001m);
        }

        [Fact]
        public void Clamp01_ShouldLimitValueBetweenZeroAndOne()
        {
            Assert.True(FPMath.Abs(FP.Zero - FPMath.Clamp01((FP)(-0.5))) < (FP)0.00001m);
            Assert.True(FPMath.Abs(FP.One - FPMath.Clamp01((FP)1.5)) < (FP)0.00001m);
            Assert.True(FPMath.Abs((FP)0.5 - FPMath.Clamp01((FP)0.5)) < (FP)0.00001m);
            Assert.True(FPMath.Abs(FP.Zero - FPMath.Clamp01(FP.Zero)) < (FP)0.00001m);
            Assert.True(FPMath.Abs(FP.One - FPMath.Clamp01(FP.One)) < (FP)0.00001m);
        }

        [Fact]
        public void Abs_ShouldReturnAbsoluteValue()
        {
            Assert.True(FPMath.Abs((FP)5 - FPMath.Abs((FP)5)) < (FP)0.00001m);
            Assert.True(FPMath.Abs((FP)5 - FPMath.Abs((FP)(-5))) < (FP)0.00001m);
            Assert.True(FPMath.Abs(FP.Zero - FPMath.Abs(FP.Zero)) < (FP)0.00001m);
            Assert.True(FPMath.Abs(FP.One - FPMath.Abs(FP.One)) < (FP)0.00001m);
            Assert.True(FPMath.Abs(FP.One - FPMath.Abs(-FP.One)) < (FP)0.00001m);
        }

        [Fact]
        public void Sign_ShouldReturnCorrectSign()
        {
            Assert.True(FPMath.Abs(FP.One - FPMath.Sign((FP)5)) < (FP)0.00001m);
            Assert.True(FPMath.Abs(-FP.One - FPMath.Sign((FP)(-5))) < (FP)0.00001m);
            Assert.True(FPMath.Abs(FP.Zero - FPMath.Sign(FP.Zero)) < (FP)0.00001m);
        }

        [Fact]
        public void Floor_ShouldReturnFloorValue()
        {
            Assert.True(FPMath.Abs((FP)3 - FPMath.Floor((FP)3.7m)) < (FP)0.00001m);
            Assert.True(FPMath.Abs((FP)(-4) - FPMath.Floor((FP)(-3.2m))) < (FP)0.00001m);
            Assert.True(FPMath.Abs(FP.Zero - FPMath.Floor((FP)0.9m)) < (FP)0.00001m);
            Assert.True(FPMath.Abs((FP)5 - FPMath.Floor((FP)5)) < (FP)0.00001m);
        }

        [Fact]
        public void Ceiling_ShouldReturnCeilingValue()
        {
            Assert.True(FPMath.Abs((FP)4 - FPMath.Ceiling((FP)3.2m)) < (FP)0.00001m);
            Assert.True(FPMath.Abs((FP)(-3) - FPMath.Ceiling((FP)(-3.7m))) < (FP)0.00001m);
            Assert.True(FPMath.Abs(FP.One - FPMath.Ceiling((FP)0.1m)) < (FP)0.00001m);
            Assert.True(FPMath.Abs((FP)5 - FPMath.Ceiling((FP)5)) < (FP)0.00001m);
        }

        [Fact]
        public void Round_ShouldReturnRoundedValue()
        {
            Assert.True(FPMath.Abs((FP)4 - FPMath.Round((FP)3.6m)) < (FP)0.00001m);
            Assert.True(FPMath.Abs((FP)3 - FPMath.Round((FP)3.4m)) < (FP)0.00001m);
            Assert.True(FPMath.Abs((FP)(-4) - FPMath.Round((FP)(-3.6m))) < (FP)0.00001m);
            Assert.True(FPMath.Abs((FP)5 - FPMath.Round((FP)5)) < (FP)0.00001m);
        }

        #endregion

        #region 三角函数测试

        [Fact]
        public void Sin_ShouldReturnCorrectValue()
        {
            Assert.True(FPMath.Abs(FP.Zero - FPMath.Sin(FP.Zero)) < (FP)0.00001m);
            Assert.True(FPMath.Abs(FP.One - FPMath.Sin(FPMath.PiOver2)) < (FP)0.00001m);
            Assert.True(FPMath.Abs(FP.Zero - FPMath.Sin(FPMath.Pi)) < (FP)0.00001m);
            Assert.True(FPMath.Abs(-FP.One - FPMath.Sin(FPMath.Pi + FPMath.PiOver2)) < (FP)0.00001m);
        }

        [Fact]
        public void Cos_ShouldReturnCorrectValue()
        {
            Assert.True(FPMath.Abs(FP.One - FPMath.Cos(FP.Zero)) < (FP)0.00001m);
            Assert.True(FPMath.Abs(FP.Zero - FPMath.Cos(FPMath.PiOver2)) < (FP)0.00001m);
            Assert.True(FPMath.Abs(-FP.One - FPMath.Cos(FPMath.Pi)) < (FP)0.00001m);
            Assert.True(FPMath.Abs(FP.Zero - FPMath.Cos(FPMath.Pi + FPMath.PiOver2)) < (FP)0.00001m);
        }

        [Fact]
        public void Tan_ShouldReturnCorrectValue()
        {
            Assert.True(FPMath.Abs(FP.Zero - FPMath.Tan(FP.Zero)) < (FP)0.00001m);
            Assert.True(FPMath.Abs(FP.Zero - FPMath.Tan(FPMath.Pi)) < (FP)0.00001m);
            // Tan(π/4) ≈ 1
            Assert.True(FPMath.Abs(FP.One - FPMath.Tan(FPMath.Pi / (FP)4)) < (FP)0.1m);
        }

        [Fact]
        public void Asin_ShouldReturnCorrectValue()
        {
            Assert.True(FPMath.Abs(FP.Zero - FPMath.Asin(FP.Zero)) < (FP)0.00001m);
            Assert.True(FPMath.Abs(FPMath.PiOver2 - FPMath.Asin(FP.One)) < (FP)0.00001m);
            Assert.True(FPMath.Abs(-FPMath.PiOver2 - FPMath.Asin(-FP.One)) < (FP)0.00001m);
        }

        [Fact]
        public void Acos_ShouldReturnCorrectValue()
        {
            Assert.True(FPMath.Abs(FPMath.PiOver2 - FPMath.Acos(FP.Zero)) < (FP)0.00001m);
            Assert.True(FPMath.Abs(FP.Zero - FPMath.Acos(FP.One)) < (FP)0.00001m);
            Assert.True(FPMath.Abs(FPMath.Pi - FPMath.Acos(-FP.One)) < (FP)0.00001m);
        }

        [Fact]
        public void Atan_ShouldReturnCorrectValue()
        {
            Assert.True(FPMath.Abs(FP.Zero - FPMath.Atan(FP.Zero)) < (FP)0.00001m);
            Assert.True(FPMath.Abs(FPMath.Pi / (FP)4 - FPMath.Atan(FP.One)) < (FP)0.001m);
            Assert.True(FPMath.Abs(-FPMath.Pi / (FP)4 - FPMath.Atan(-FP.One)) < (FP)0.001m);
        }

        [Fact]
        public void Atan2_ShouldReturnCorrectValue()
        {
            Assert.True(FPMath.Abs(FP.Zero - FPMath.Atan2(FP.Zero, FP.One)) < (FP)0.00001m);
            Assert.True(FPMath.Abs(FPMath.PiOver2 - FPMath.Atan2(FP.One, FP.Zero)) < (FP)0.00001m);
            Assert.True(FPMath.Abs(FPMath.Pi - FPMath.Atan2(FP.Zero, -FP.One)) < (FP)0.00001m);
            Assert.True(FPMath.Abs(-FPMath.PiOver2 - FPMath.Atan2(-FP.One, FP.Zero)) < (FP)0.00001m);
        }

        #endregion

        #region 插值函数测试

        [Fact]
        public void Barycentric_ShouldReturnCorrectValue()
        {
            var value1 = (FP)1;
            var value2 = (FP)2;
            var value3 = (FP)3;
            var amount1 = (FP)0.5m;
            var amount2 = (FP)0.3m;

            var result = FPMath.Barycentric(value1, value2, value3, amount1, amount2);
            var expected = value1 + amount1 * (value2 - value1) + amount2 * (value3 - value1);
            Assert.True(FPMath.Abs(expected - result) < (FP)0.00001m);
        }

        [Fact]
        public void CatmullRom_ShouldReturnCorrectValue()
        {
            var value1 = (FP)1;
            var value2 = (FP)2;
            var value3 = (FP)3;
            var value4 = (FP)4;
            var amount = (FP)0.5m;

            var result = FPMath.CatmullRom(value1, value2, value3, value4, amount);
            // 在 amount = 0.5 时，应该接近 value2 和 value3 的中点
            Assert.True(result > value2 && result < value3);
        }

        [Fact]
        public void Distance_ShouldReturnCorrectValue()
        {
            Assert.True(FPMath.Abs((FP)5 - FPMath.Distance((FP)1, (FP)6)) < (FP)0.00001m);
            Assert.True(FPMath.Abs((FP)5 - FPMath.Distance((FP)6, (FP)1)) < (FP)0.00001m);
            Assert.True(FPMath.Abs(FP.Zero - FPMath.Distance((FP)3, (FP)3)) < (FP)0.00001m);
            Assert.True(FPMath.Abs((FP)10 - FPMath.Distance((FP)(-5), (FP)5)) < (FP)0.00001m);
        }

        [Fact]
        public void Hermite_ShouldReturnCorrectValue()
        {
            var value1 = (FP)0;
            var tangent1 = (FP)1;
            var value2 = (FP)1;
            var tangent2 = (FP)1;
            var amount = (FP)0.5m;

            var result = FPMath.Hermite(value1, tangent1, value2, tangent2, amount);
            // 在 amount = 0.5 时，应该在 value1 和 value2 之间
            Assert.True(result >= value1 && result <= value2);
        }

        [Fact]
        public void Lerp_ShouldReturnCorrectValue()
        {
            Assert.True(FPMath.Abs((FP)1 - FPMath.Lerp((FP)1, (FP)3, FP.Zero)) < (FP)0.00001m);
            Assert.True(FPMath.Abs((FP)3 - FPMath.Lerp((FP)1, (FP)3, FP.One)) < (FP)0.00001m);
            Assert.True(FPMath.Abs((FP)2 - FPMath.Lerp((FP)1, (FP)3, (FP)0.5m)) < (FP)0.00001m);
            Assert.True(FPMath.Abs((FP)1.5m - FPMath.Lerp((FP)1, (FP)2, (FP)0.5m)) < (FP)0.00001m);
        }

        [Fact]
        public void Lerp_WithAmountOutOfRange_ShouldClamp()
        {
            // amount > 1 应该被限制为 1
            Assert.True(FPMath.Abs((FP)3 - FPMath.Lerp((FP)1, (FP)3, (FP)1.5m)) < (FP)0.00001m);
            // amount < 0 应该被限制为 0
            Assert.True(FPMath.Abs((FP)1 - FPMath.Lerp((FP)1, (FP)3, (FP)(-0.5m))) < (FP)0.00001m);
        }

        [Fact]
        public void InverseLerp_ShouldReturnCorrectValue()
        {
            Assert.True(FPMath.Abs(FP.Zero - FPMath.InverseLerp((FP)1, (FP)3, (FP)1)) < (FP)0.00001m);
            Assert.True(FPMath.Abs(FP.One - FPMath.InverseLerp((FP)1, (FP)3, (FP)3)) < (FP)0.00001m);
            Assert.True(FPMath.Abs((FP)0.5m - FPMath.InverseLerp((FP)1, (FP)3, (FP)2)) < (FP)0.00001m);
        }

        [Fact]
        public void InverseLerp_WithSameValues_ShouldReturnZero()
        {
            Assert.True(FPMath.Abs(FP.Zero - FPMath.InverseLerp((FP)5, (FP)5, (FP)5)) < (FP)0.00001m);
            Assert.True(FPMath.Abs(FP.Zero - FPMath.InverseLerp((FP)5, (FP)5, (FP)10)) < (FP)0.00001m);
        }

        [Fact]
        public void SmoothStep_ShouldReturnCorrectValue()
        {
            Assert.True(FPMath.Abs((FP)1 - FPMath.SmoothStep((FP)1, (FP)3, FP.Zero)) < (FP)0.00001m);
            Assert.True(FPMath.Abs((FP)3 - FPMath.SmoothStep((FP)1, (FP)3, FP.One)) < (FP)0.00001m);

            var result = FPMath.SmoothStep((FP)1, (FP)3, (FP)0.5m);
            Assert.True(result > (FP)1 && result < (FP)3);
        }

        #endregion

        #region 幂函数测试

        [Fact]
        public void Pow_ShouldReturnCorrectValue()
        {
            Assert.True(FPMath.Abs(FP.One - FPMath.Pow(FP.One, (FP)100)) < (FP)0.00001m); // 1^100 = 1
            Assert.True(FPMath.Abs(FP.One - FPMath.Pow((FP)5, FP.Zero)) < (FP)0.00001m); // 5^0 = 1
            Assert.True(FPMath.Abs((FP)8 - FPMath.Pow((FP)2, (FP)3)) < (FP)0.00001m); // 2^3 = 8
            Assert.True(FPMath.Abs((FP)25 - FPMath.Pow((FP)5, (FP)2)) < (FP)0.00001m); // 5^2 = 25
        }

        [Fact]
        public void Pow_WithZeroBase_ShouldReturnCorrectValue()
        {
            Assert.True(FPMath.Abs(FP.Zero - FPMath.Pow(FP.Zero, (FP)5)) < (FP)0.00001m); // 0^5 = 0
            Assert.True(FPMath.Abs(FP.One - FPMath.Pow(FP.Zero, FP.Zero)) < (FP)0.00001m); // 0^0 = 1
            Assert.True(FPMath.Abs(FP.MaxValue - FPMath.Pow(FP.Zero, (FP)(-1))) < (FP)0.00001m); // 0^(-1) = MaxValue (代替无穷大)
        }

        #endregion

        #region 运动函数测试

        [Fact]
        public void MoveTowards_ShouldReturnCorrectValue()
        {
            Assert.True(FPMath.Abs((FP)3 - FPMath.MoveTowards((FP)1, (FP)5, (FP)2)) < (FP)0.00001m); // 1 向 5 移动 2 = 3
            Assert.True(FPMath.Abs((FP)5 - FPMath.MoveTowards((FP)1, (FP)5, (FP)10)) < (FP)0.00001m); // 移动距离超过目标，返回目标
            Assert.True(FPMath.Abs((FP)3 - FPMath.MoveTowards((FP)5, (FP)1, (FP)2)) < (FP)0.00001m); // 5 向 1 移动 2 = 3
            Assert.True(FPMath.Abs((FP)5 - FPMath.MoveTowards((FP)5, (FP)5, (FP)2)) < (FP)0.00001m); // 已在目标位置
        }

        [Fact]
        public void Repeat_ShouldReturnCorrectValue()
        {
            Assert.True(FPMath.Abs((FP)1 - FPMath.Repeat((FP)6, (FP)5)) < (FP)0.00001m); // 6 % 5 = 1
            Assert.True(FPMath.Abs((FP)0 - FPMath.Repeat((FP)5, (FP)5)) < (FP)0.00001m); // 5 % 5 = 0
            Assert.True(FPMath.Abs((FP)2.5m - FPMath.Repeat((FP)7.5m, (FP)5)) < (FP)0.00001m); // 7.5 % 5 = 2.5
            Assert.True(FPMath.Abs((FP)0 - FPMath.Repeat(FP.Zero, (FP)5)) < (FP)0.00001m); // 0 % 5 = 0
        }

        [Fact]
        public void DeltaAngle_ShouldReturnCorrectValue()
        {
            Assert.True(FPMath.Abs((FP)90m - FPMath.DeltaAngle(FP.Zero, (FP)90m)) < (FP)0.00001m);
            Assert.True(FPMath.Abs((FP)(-90m) - FPMath.DeltaAngle((FP)90m, FP.Zero)) < (FP)0.00001m);
            Assert.True(FPMath.Abs((FP)(-90m) - FPMath.DeltaAngle((FP)10m, (FP)280m)) < (FP)0.00001m); // 跨越 360 度边界
            Assert.True(FPMath.Abs(FP.Zero - FPMath.DeltaAngle((FP)180m, (FP)180m)) < (FP)0.00001m);
        }

        [Fact]
        public void MoveTowardsAngle_ShouldReturnCorrectValue()
        {
            Assert.True(FPMath.Abs((FP)45m - FPMath.MoveTowardsAngle(FP.Zero, (FP)90m, (FP)45m)) < (FP)0.00001m);
            Assert.True(FPMath.Abs((FP)90m - FPMath.MoveTowardsAngle(FP.Zero, (FP)90m, (FP)100m)) < (FP)0.00001m); // 超过目标

            // 跨越边界：从10度到350度，最短路径是向后20度，所以结果应该是350度（等价于-10度）
            var result = FPMath.MoveTowardsAngle((FP)10m, (FP)350m, (FP)30m);
            // 检查结果是否等价于350度（考虑角度的周期性）
            var normalizedResult = FPMath.Repeat(result, (FP)360m);
            Assert.True(FPMath.Abs((FP)350m - normalizedResult) < (FP)0.00001m);
        }

        [Fact]
        public void SmoothDamp_ShouldReturnCorrectValue()
        {
            var velocity = FP.Zero;
            var result = FPMath.SmoothDamp(FP.Zero, (FP)10m, ref velocity, (FP)1m);

            // 结果应该在起始值和目标值之间
            Assert.True(result > FP.Zero && result < (FP)10m);
            // 速度应该被更新
            Assert.NotEqual(FP.Zero, velocity);
        }

        [Fact]
        public void SmoothDamp_WithMaxSpeed_ShouldReturnCorrectValue()
        {
            var velocity = FP.Zero;
            var result = FPMath.SmoothDamp(FP.Zero, (FP)10m, ref velocity, (FP)1m, (FP)5m);

            // 结果应该在起始值和目标值之间
            Assert.True(result > FP.Zero && result < (FP)10m);
        }

        [Fact]
        public void SmoothDamp_WithDeltaTime_ShouldReturnCorrectValue()
        {
            var velocity = FP.Zero;
            var result = FPMath.SmoothDamp(FP.Zero, (FP)10m, ref velocity, (FP)1m, (FP)5m, (FP)0.1m);

            // 结果应该在起始值和目标值之间
            Assert.True(result > FP.Zero && result < (FP)10m);
        }

        #endregion
    }
}