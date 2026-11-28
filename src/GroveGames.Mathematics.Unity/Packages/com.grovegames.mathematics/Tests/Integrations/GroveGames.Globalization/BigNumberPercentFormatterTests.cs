using System;
using GroveGames.Globalization;
using NUnit.Framework;

namespace GroveGames.Mathematics.Unity.Globalization.Editor.Tests
{
    public sealed class BigNumberPercentFormatterTests
    {
        [TestCase(0d, 0L, "0%")]
        [TestCase(5d, -2L, "5%")]
        [TestCase(1.576d, -1L, "15.76%")]
        [TestCase(1.5d, 0L, "150%")]
        [TestCase(-5d, -2L, "-5%")]
        [TestCase(9.9994d, 1L, "9999.4%")]
        [TestCase(9.9999996d, 1L, "10K%")]
        [TestCase(1d, 2L, "10K%")]
        [TestCase(1.234d, 3L, "123K%")]
        [TestCase(1.9d, 6L, "190M%")]
        [TestCase(1.5d, 13L, "1.5aa%")]
        public void TryFormat_Fraction_WritesThePercentage(double mantissa, long exponent, string expected)
        {
            var value = new BigNumber(mantissa, exponent);
            Span<char> destination = stackalloc char[BigNumberPercentFormatter.MaxLength];

            var formatted = BigNumberPercentFormatter.TryFormat(value, destination, out var charsWritten);

            Assert.That(formatted, Is.True);
            Assert.That(destination.Slice(0, charsWritten).ToString(), Is.EqualTo(expected));
        }

        [Test]
        public void TryFormat_DestinationTooSmall_ReturnsFalse()
        {
            Span<char> destination = stackalloc char[4];

            var formatted = BigNumberPercentFormatter.TryFormat(new BigNumber(1.9d, 6L), destination, out var charsWritten);

            Assert.That(formatted, Is.False);
            Assert.That(charsWritten, Is.EqualTo(0));
        }

        [Test]
        public void TryParse_Percent_ReturnsTheFraction()
        {
            var parsed = BigNumberPercentFormatter.TryParse("1.5aa%", out var value);

            Assert.That(parsed, Is.True);
            Assert.That(value, Is.EqualTo(new BigNumber(1.5d, 13L)));
        }

        [Test]
        public void TryParse_MissingPercentSign_ReturnsFalse()
        {
            var parsed = BigNumberPercentFormatter.TryParse("15", out _);

            Assert.That(parsed, Is.False);
        }
    }
}
