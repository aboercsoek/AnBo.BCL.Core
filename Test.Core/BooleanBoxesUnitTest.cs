using Xunit;

namespace AnBo.Test
{
    public class BooleanBoxesUnitTest
    {
        [Fact]
        public void TestCase001_BoolenBoxes_Should_return_a_boxed_bool_object()
        {
            var trueBox = BooleanBoxes.Box(true);
            var falseBox = BooleanBoxes.Box(false);
            Assert.Equal(BooleanBoxes.TrueBox, trueBox);
            Assert.Equal(BooleanBoxes.FalseBox, falseBox);
        }


    }
}