using FluentAssertions;
using Xunit;
using AnBo.Core;

namespace AnBo.Test;

public class BooleanBoxesUnitTest
{
    [Fact]
    public void BoolenBoxes_Should_return_a_boxed_bool_object()
    {
        BooleanBoxes.Box(true).Should().Be(BooleanBoxes.TrueBox);
        BooleanBoxes.Box(false).Should().Be(BooleanBoxes.FalseBox);
    }
}