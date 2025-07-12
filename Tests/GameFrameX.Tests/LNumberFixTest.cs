using GameFrameX.Utility;
using Xunit;

namespace GameFrameX.Tests
{
    public class LNumberFixTest
    {
        [Fact]
        public void TestCreateNegativeNumbers()
        {
            // 测试负数的 Create 方法
            var negativeNumber = LNumber.Create(-123, -4567);
            
            // 验证结果应该是负数
            Assert.True(negativeNumber.Raw < 0, "Raw value should be negative");
            Assert.True((double)negativeNumber < 0, "Double value should be negative");
            Assert.Equal(-124, negativeNumber.Floor);
            Assert.Equal(-123, negativeNumber.Ceiling);
            Assert.Equal(-123, (long)negativeNumber);
        }
        
        [Fact]
        public void TestCreatePositiveNumbers()
        {
            // 测试正数的 Create 方法
            var positiveNumber = LNumber.Create(123, 4567);
            
            // 验证结果应该是正数
            Assert.True(positiveNumber.Raw > 0, "Raw value should be positive");
            Assert.True((double)positiveNumber > 0, "Double value should be positive");
            Assert.Equal(123, positiveNumber.Floor);
            Assert.Equal(124, positiveNumber.Ceiling);
            Assert.Equal(123, (long)positiveNumber);
        }
        
        [Fact]
        public void TestCreateMixedSigns()
        {
            // 测试混合符号的情况
            var number1 = LNumber.Create(-123, 4567); // 整数部分负，小数部分正
            var number2 = LNumber.Create(123, -4567); // 整数部分正，小数部分负
            var number3 = LNumber.Create(0, -4567);   // 整数部分为0，小数部分负
            
            // number1 应该是负数（因为整数部分为负）
            Assert.True((double)number1 < 0, "number1 should be negative");
            
            // number2 应该是正数（因为整数部分为正）
            Assert.True((double)number2 > 0, "number2 should be positive");
            
            // number3 应该是负数（因为整数部分为0且小数部分为负）
            Assert.True((double)number3 < 0, "number3 should be negative");
        }
    }
}