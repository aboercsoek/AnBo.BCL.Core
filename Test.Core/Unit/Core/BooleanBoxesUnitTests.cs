//--------------------------------------------------------------------------
// File:    BooleanBoxesUnitTests.cs
// Content: Unit tests for BooleanBoxes class
// Author:  Andreas Börcsök
// Copyright © 2025 Andreas Börcsök
// License: GNU General Public License v3.0
//--------------------------------------------------------------------------

using FluentAssertions;
using AnBo.Core;

namespace AnBo.Test.Unit;

[Trait("Category", "Unit")]
public class BooleanBoxesUnitTests
{
    [Fact]
    public void BoolenBoxes_Should_return_a_boxed_bool_object()
    {
        BooleanBoxes.Box(true).Should().Be(BooleanBoxes.TrueBox);
        BooleanBoxes.Box(false).Should().Be(BooleanBoxes.FalseBox);
    }
}