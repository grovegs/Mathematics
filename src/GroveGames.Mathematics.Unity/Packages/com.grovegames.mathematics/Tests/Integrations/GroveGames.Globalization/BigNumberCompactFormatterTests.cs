using System;
using GroveGames.Globalization;
using NUnit.Framework;

namespace GroveGames.Mathematics.Unity.Globalization.Editor.Tests
{
    public sealed class BigNumberCompactFormatterTests
    {
        [TestCase(0d, 0L, "0")]
        [TestCase(2.5d, 2L, "250")]
        [TestCase(1.2345d, 4L, "12.3K")]
        [TestCase(1.25d, 6L, "1.25M")]
        [TestCase(4.2d, 12L, "4.2T")]
        [TestCase(1.5d, 15L, "1.5aa")]
        [TestCase(7d, 18L, "7ab")]
        [TestCase(1.5d, 400L, "15ey")]
        [TestCase(-1.5d, 6L, "-1.5M")]
        [TestCase(9050000d, 0L, "9.05M")]
        [TestCase(10050d, 0L, "10.1K")]
        public void TryFormat_Value_MatchesTheNumberFormatter(double mantissa, long exponent, string expected)
        {
            var value = new BigNumber(mantissa, exponent);
            Span<char> destination = stackalloc char[BigNumberCompactFormatter.MaxLength];

            var formatted = BigNumberCompactFormatter.TryFormat(value, NumberRounding.Nearest, destination, out var charsWritten);

            Assert.That(formatted, Is.True);
            Assert.That(destination.Slice(0, charsWritten).ToString(), Is.EqualTo(expected));
        }

        [TestCase(1.2496d, 6L, NumberRounding.Floor, "1.24M")]
        [TestCase(1.2496d, 6L, NumberRounding.Ceiling, "1.25M")]
        [TestCase(-1.2496d, 6L, NumberRounding.Floor, "-1.25M")]
        [TestCase(9.991d, 14L, NumberRounding.Ceiling, "1aa")]
        public void TryFormat_Rounding_RoundsTheValue(double mantissa, long exponent, NumberRounding rounding, string expected)
        {
            var value = new BigNumber(mantissa, exponent);
            Span<char> destination = stackalloc char[BigNumberCompactFormatter.MaxLength];

            var formatted = BigNumberCompactFormatter.TryFormat(value, rounding, destination, out var charsWritten);

            Assert.That(formatted, Is.True);
            Assert.That(destination.Slice(0, charsWritten).ToString(), Is.EqualTo(expected));
        }

        [TestCase(NumberRounding.Floor)]
        [TestCase(NumberRounding.Ceiling)]
        public void TryFormat_RoundingAfterArithmeticNoise_ShowsTheIntendedValue(NumberRounding rounding)
        {
            var value = (new BigNumber(1d, 0L) - new BigNumber(0.9d, 0L)) * new BigNumber(1d, 7L);
            Span<char> destination = stackalloc char[BigNumberCompactFormatter.MaxLength];

            var formatted = BigNumberCompactFormatter.TryFormat(value, rounding, destination, out var charsWritten);

            Assert.That(formatted, Is.True);
            Assert.That(destination.Slice(0, charsWritten).ToString(), Is.EqualTo("1M"));
        }

        [Test]
        public void TryFormat_WholeValueAfterArithmetic_StaysExact()
        {
            var value = new BigNumber(1d, 3L) - new BigNumber(1d, 0L);
            Span<char> destination = stackalloc char[BigNumberCompactFormatter.MaxLength];

            var formatted = BigNumberCompactFormatter.TryFormat(value, NumberRounding.Nearest, destination, out var charsWritten);

            Assert.That(formatted, Is.True);
            Assert.That(destination.Slice(0, charsWritten).ToString(), Is.EqualTo("999"));
        }

        [TestCase(0d, 0L)]
        [TestCase(2.5d, 2L)]
        [TestCase(1.25d, 6L)]
        [TestCase(1.5d, 15L)]
        [TestCase(1.5d, 400L)]
        [TestCase(-1.5d, 6L)]
        public void TryParse_FormattedValue_RoundTrips(double mantissa, long exponent)
        {
            var value = new BigNumber(mantissa, exponent);
            Span<char> destination = stackalloc char[BigNumberCompactFormatter.MaxLength];
            BigNumberCompactFormatter.TryFormat(value, NumberRounding.Nearest, destination, out var charsWritten);
            var text = destination.Slice(0, charsWritten).ToString();

            var parsed = BigNumberCompactFormatter.TryParse(text, out var result);
            BigNumberCompactFormatter.TryFormat(result, NumberRounding.Nearest, destination, out var rewritten);

            Assert.That(parsed, Is.True);
            Assert.That(destination.Slice(0, rewritten).ToString(), Is.EqualTo(text));
        }

        [Test]
        public void TryParse_LetterUnit_ReturnsTheValue()
        {
            var parsed = BigNumberCompactFormatter.TryParse("10aa", out var value);

            Assert.That(parsed, Is.True);
            Assert.That(value, Is.EqualTo(new BigNumber(1d, 16L)));
        }

        [Test]
        public void TryParse_InvalidText_ReturnsFalse()
        {
            var parsed = BigNumberCompactFormatter.TryParse("1e3", out _);

            Assert.That(parsed, Is.False);
        }
    }
}
