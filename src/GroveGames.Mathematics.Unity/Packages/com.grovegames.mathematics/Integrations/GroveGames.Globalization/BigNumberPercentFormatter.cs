using System;
using GroveGames.Globalization;

namespace GroveGames.Mathematics.Unity
{
    public static class BigNumberPercentFormatter
    {
        public const int MaxLength = CompactFormatter.MaxLength + 1;

        public static bool TryFormat(BigNumber value, Span<char> destination, out int charsWritten)
        {
            return PercentFormatter.TryFormat(value.Mantissa, value.Exponent, destination, out charsWritten);
        }
    }
}
