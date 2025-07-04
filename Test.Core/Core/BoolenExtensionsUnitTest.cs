using FluentAssertions;
using Xunit;
using AnBo.Core;

namespace AnBo.Test
{
    public class BoolenExtensionsUnitTest
    {
        [Fact]
        public void TestCase001_Boolean_Variable_with_value_true_should_return_true_for_IsTrue_and_false_for_IsFalse()
        {
            var trueValue = true;

            trueValue.IsTrue().Should().BeTrue();
            trueValue.IsFalse().Should().BeFalse();
        }

        [Fact]
        public void TestCase002_Boolean_Variable_with_value_false_should_return_true_for_IsFalse_and_false_for_IsTrue()
        {
            var falseValue = false;

            falseValue.IsFalse().Should().BeTrue();
            falseValue.IsTrue().Should().BeFalse();
        }
    }
}
