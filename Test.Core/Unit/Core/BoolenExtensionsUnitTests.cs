//--------------------------------------------------------------------------
// File:    BoolenExtensionsUnitTests.cs
// Content: Unit tests for BoolExtensions class
// Author:  Andreas Börcsök
// Copyright © 2025 Andreas Börcsök
// License: GNU General Public License v3.0
//--------------------------------------------------------------------------

using FluentAssertions;
using AnBo.Core;

namespace AnBo.Test.Unit;

[Trait("Category", "Unit")]
public class BoolenExtensionsUnitTests
{
    [Fact]
    public void Boolean_Variable_with_value_true_should_return_true_for_IsTrue_and_false_for_IsFalse()
    {
        var trueValue = true;

        trueValue.IsTrue().Should().BeTrue();
        trueValue.IsFalse().Should().BeFalse();
    }

    [Fact]
    public void Boolean_Variable_with_value_false_should_return_true_for_IsFalse_and_false_for_IsTrue()
    {
        var falseValue = false;

        falseValue.IsFalse().Should().BeTrue();
        falseValue.IsTrue().Should().BeFalse();
    }
}
