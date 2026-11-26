using System;

namespace GroveGames.Mathematics.Tests;

public sealed class BigNumberTests
{
    private const double Tolerance = 1e-9;

    [Fact]
    public void Constructor_ValueAboveTen_NormalizesMantissaAndExponent()
    {
        var number = new BigNumber(1234d, 0L);

        Assert.Equal(1.234d, number.Mantissa, Tolerance);
        Assert.Equal(3L, number.Exponent);
    }

    [Fact]
    public void Constructor_ValueBelowOne_NormalizesToNegativeExponent()
    {
        var number = new BigNumber(0.05d, 0L);

        Assert.Equal(5d, number.Mantissa, Tolerance);
        Assert.Equal(-2L, number.Exponent);
    }

    [Fact]
    public void Constructor_NotFinite_ReturnsZero()
    {
        var notNumber = new BigNumber(double.NaN, 5L);
        var infinite = new BigNumber(double.PositiveInfinity, 5L);

        Assert.Equal(BigNumber.Zero, notNumber);
        Assert.Equal(BigNumber.Zero, infinite);
    }

    [Fact]
    public void Add_CloseExponents_SumsValues()
    {
        BigNumber left = 1500;
        BigNumber right = 250;

        var sum = left + right;

        Assert.Equal(1750d, sum.ToDouble(), Tolerance);
    }

    [Fact]
    public void Constructor_Integer_KeepsExactMantissa()
    {
        BigNumber number = 73;

        Assert.Equal(7.3d, number.Mantissa);
        Assert.Equal(1L, number.Exponent);
    }

    [Fact]
    public void Add_IntegersSummingToHundred_EqualsHundred()
    {
        BigNumber sum = 75;

        sum = sum + 7 + 1 + 15 + 2;

        Assert.Equal((BigNumber)100, sum);
        Assert.True(sum >= 100);
    }

    [Fact]
    public void Add_RepeatedEarnAndSpend_DoesNotDrift()
    {
        var sum = BigNumber.Zero;
        var expected = 0L;

        for (var i = 0; i < 10000; i++)
        {
            var amount = i % 3 == 2 ? -100 : (i * 37 % 97) + 1;

            if (expected + amount < 0)
            {
                continue;
            }

            sum += amount;
            expected += amount;
        }

        Assert.Equal((BigNumber)expected, sum);
    }

    [Fact]
    public void Add_TenthsTenTimes_EqualsOne()
    {
        var sum = BigNumber.Zero;

        for (var i = 0; i < 10; i++)
        {
            sum += 0.1d;
        }

        Assert.Equal(BigNumber.One, sum);
    }

    [Theory]
    [InlineData(435)]
    [InlineData(115)]
    [InlineData(99999)]
    [InlineData(123456789012345L)]
    public void ToDouble_Integer_ReturnsExactValue(long value)
    {
        BigNumber number = value;

        var result = number.ToDouble();

        Assert.Equal((double)value, result);
    }

    [Fact]
    public void Floor_ValueWithInexactProduct_KeepsValue()
    {
        BigNumber value = 435;

        var floored = value.Floor();

        Assert.Equal(value, floored);
    }

    [Fact]
    public void Round_HalfWithInexactProduct_RoundsAwayFromZero()
    {
        BigNumber value = 2.675d;

        var rounded = (value * 100).Round();

        Assert.Equal((BigNumber)268, rounded);
    }

    [Fact]
    public void TryParse_DriftedMantissa_SnapsToValue()
    {
        var parsed = BigNumber.TryParse("9.999999999999998e1", out var value);

        Assert.True(parsed);
        Assert.Equal((BigNumber)100, value);
    }

    [Fact]
    public void Add_FarApartExponents_KeepsLargerValue()
    {
        var large = new BigNumber(1d, 100L);
        BigNumber small = 1;

        var sum = large + small;

        Assert.Equal(large, sum);
    }

    [Fact]
    public void Subtract_EqualValues_ReturnsZero()
    {
        var value = new BigNumber(4.2d, 50L);

        var difference = value - value;

        Assert.True(difference.IsZero);
    }

    [Fact]
    public void Multiply_BeyondDoubleRange_AddsExponents()
    {
        var value = new BigNumber(2d, 200L);

        var product = value * value;

        Assert.Equal(4d, product.Mantissa, Tolerance);
        Assert.Equal(400L, product.Exponent);
    }

    [Fact]
    public void Divide_ByZero_ReturnsZero()
    {
        BigNumber value = 10;

        var quotient = value / BigNumber.Zero;

        Assert.Equal(BigNumber.Zero, quotient);
    }

    [Fact]
    public void Divide_LargeValues_SubtractsExponents()
    {
        var left = new BigNumber(6d, 500L);
        var right = new BigNumber(2d, 100L);

        var quotient = left / right;

        Assert.Equal(3d, quotient.Mantissa, Tolerance);
        Assert.Equal(400L, quotient.Exponent);
    }

    [Fact]
    public void CompareTo_MixedSignsAndExponents_OrdersByValue()
    {
        var values = new[] { new BigNumber(5d, 10L), -1000, BigNumber.Zero, new BigNumber(-2d, 10L), 3, new BigNumber(1d, 300L) };

        Array.Sort(values);

        Assert.Equal(new BigNumber(-2d, 10L), values[0]);
        Assert.Equal((BigNumber)(-1000), values[1]);
        Assert.Equal(BigNumber.Zero, values[2]);
        Assert.Equal((BigNumber)3, values[3]);
        Assert.Equal(new BigNumber(5d, 10L), values[4]);
        Assert.Equal(new BigNumber(1d, 300L), values[5]);
    }

    [Fact]
    public void Pow_GrowthCurve_MatchesDoublePow()
    {
        BigNumber growth = 1.15d;

        var result = BigNumber.Pow(growth, 50d);

        Assert.Equal(Math.Pow(1.15d, 50d), result.ToDouble(), 1e-6d);
    }

    [Fact]
    public void Pow_BeyondDoubleRange_ReturnsLargeExponent()
    {
        BigNumber ten = 10;

        var result = BigNumber.Pow(ten, 1000d);

        Assert.Equal(1d, result.Mantissa, 1e-6d);
        Assert.Equal(1000L, result.Exponent);
    }

    [Fact]
    public void Floor_FractionalValue_DropsFraction()
    {
        BigNumber value = 123.7d;

        var floored = value.Floor();

        Assert.Equal((BigNumber)123, floored);
    }

    [Fact]
    public void Floor_IntegerValue_KeepsValue()
    {
        BigNumber value = 123;

        var floored = value.Floor();

        Assert.Equal(value, floored);
    }

    [Theory]
    [InlineData(123.4d, 123d)]
    [InlineData(123.5d, 124d)]
    [InlineData(-2.5d, -3d)]
    [InlineData(0.4d, 0d)]
    [InlineData(0.6d, 1d)]
    public void Round_FractionalValue_RoundsToNearestAwayFromZero(double value, double expected)
    {
        BigNumber number = value;

        var rounded = number.Round();

        Assert.Equal((BigNumber)expected, rounded);
    }

    [Fact]
    public void Round_HugeValue_KeepsValue()
    {
        var value = new BigNumber(1.5d, 30L);

        var rounded = value.Round();

        Assert.Equal(value, rounded);
    }

    [Fact]
    public void ToDouble_BeyondDoubleRange_ReturnsInfinity()
    {
        var value = new BigNumber(1d, 400L);

        var result = value.ToDouble();

        Assert.True(double.IsPositiveInfinity(result));
    }

    [Theory]
    [InlineData(0d, 0L, "0")]
    [InlineData(5d, 0L, "5")]
    [InlineData(1.25d, 0L, "1.25")]
    [InlineData(1d, 3L, "1e3")]
    [InlineData(1.234567d, 6L, "1.234567e6")]
    [InlineData(-1.5d, 400L, "-1.5e400")]
    [InlineData(2d, -5L, "2e-5")]
    public void ToString_Value_WritesMantissaAndExponent(double mantissa, long exponent, string expected)
    {
        var value = new BigNumber(mantissa, exponent);

        var text = value.ToString();

        Assert.Equal(expected, text);
    }

    [Theory]
    [InlineData("0", 0d, 0L)]
    [InlineData("42", 4.2d, 1L)]
    [InlineData("1e3", 1d, 3L)]
    [InlineData("2.5E6", 2.5d, 6L)]
    [InlineData(" 1.5e400 ", 1.5d, 400L)]
    [InlineData("-4e3", -4d, 3L)]
    [InlineData("3e-2", 3d, -2L)]
    public void TryParse_Text_ReturnsNumber(string text, double mantissa, long exponent)
    {
        var isParsed = BigNumber.TryParse(text, out var value);

        Assert.True(isParsed);
        Assert.Equal(mantissa, value.Mantissa, Tolerance);
        Assert.Equal(exponent, value.Exponent);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("e3")]
    [InlineData("1e")]
    [InlineData("1.2.3")]
    [InlineData("1a")]
    [InlineData("1e3.5")]
    [InlineData(null)]
    public void TryParse_InvalidText_ReturnsFalseAndZero(string? text)
    {
        var isParsed = BigNumber.TryParse(text, out var value);

        Assert.False(isParsed);
        Assert.Equal(BigNumber.Zero, value);
    }

    [Theory]
    [InlineData(1.23d, 3L)]
    [InlineData(2.5d, 6L)]
    [InlineData(9.99d, 80L)]
    [InlineData(1.2345678901234d, 4000L)]
    public void TryParse_ToStringValue_RoundTrips(double mantissa, long exponent)
    {
        var number = new BigNumber(mantissa, exponent);

        BigNumber.TryParse(number.ToString(), out var parsed);

        Assert.Equal(number, parsed);
    }

    [Theory]
    [InlineData(1.5d, 10L, 7.3d, 3L, 4611809404718548750L, 6L)]
    [InlineData(1d, 0L, 3d, 0L, 4614688418178968227L, -1L)]
    [InlineData(2.5d, 100L, 9.99d, 50L, 4612817553468899404L, 49L)]
    [InlineData(-4.2d, 7L, 6.1d, 2L, -4603933920708143601L, 4L)]
    public void Divide_MantissaQuotientBelowOne_ReturnsPinnedValue(double leftMantissa, long leftExponent, double rightMantissa, long rightExponent, long mantissaBits, long exponent)
    {
        var left = new BigNumber(leftMantissa, leftExponent);
        var right = new BigNumber(rightMantissa, rightExponent);

        var quotient = left / right;

        Assert.Equal(mantissaBits, BitConverter.DoubleToInt64Bits(quotient.Mantissa));
        Assert.Equal(exponent, quotient.Exponent);
    }
}
