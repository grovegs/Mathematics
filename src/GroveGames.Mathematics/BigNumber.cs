using System;
using System.Globalization;

namespace GroveGames.Mathematics;

public readonly struct BigNumber : IEquatable<BigNumber>, IComparable<BigNumber>
{
    private const int MaxSignificantDigits = 17;
    private const int MantissaDecimals = 14;
    private const long MaxDoubleExponent = 308;
    private const long MinDoubleExponent = -324;

    private static readonly double[] s_powersOfTen =
    {
        1e0, 1e1, 1e2, 1e3, 1e4, 1e5, 1e6, 1e7, 1e8, 1e9, 1e10, 1e11, 1e12, 1e13, 1e14, 1e15, 1e16, 1e17
    };

    public static readonly BigNumber Zero = new(0d, 0L);
    public static readonly BigNumber One = new(1d, 0L);

    private readonly double _mantissa;
    private readonly long _exponent;

    public double Mantissa => _mantissa;
    public long Exponent => _exponent;
    public bool IsZero => _mantissa == 0d;
    public int Sign => Math.Sign(_mantissa);

    public BigNumber(double mantissa, long exponent)
    {
        Normalize(mantissa, exponent, out _mantissa, out _exponent);
    }

    public static implicit operator BigNumber(int value)
    {
        return new BigNumber(value, 0L);
    }

    public static implicit operator BigNumber(long value)
    {
        return new BigNumber(value, 0L);
    }

    public static implicit operator BigNumber(double value)
    {
        return new BigNumber(value, 0L);
    }

    public static explicit operator double(BigNumber value)
    {
        return value.ToDouble();
    }

    public static BigNumber operator -(BigNumber value)
    {
        return new BigNumber(-value._mantissa, value._exponent);
    }

    public static BigNumber operator +(BigNumber left, BigNumber right)
    {
        if (left.IsZero)
        {
            return right;
        }

        if (right.IsZero)
        {
            return left;
        }

        var difference = left._exponent - right._exponent;

        if (difference > MaxSignificantDigits)
        {
            return left;
        }

        if (difference < -MaxSignificantDigits)
        {
            return right;
        }

        if (difference >= 0)
        {
            return new BigNumber(left._mantissa + right._mantissa / s_powersOfTen[difference], left._exponent);
        }

        return new BigNumber(right._mantissa + left._mantissa / s_powersOfTen[-difference], right._exponent);
    }

    public static BigNumber operator -(BigNumber left, BigNumber right)
    {
        return left + -right;
    }

    public static BigNumber operator *(BigNumber left, BigNumber right)
    {
        return new BigNumber(left._mantissa * right._mantissa, left._exponent + right._exponent);
    }

    public static BigNumber operator /(BigNumber left, BigNumber right)
    {
        if (right.IsZero)
        {
            return Zero;
        }

        return new BigNumber(left._mantissa / right._mantissa, left._exponent - right._exponent);
    }

    public static bool operator ==(BigNumber left, BigNumber right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(BigNumber left, BigNumber right)
    {
        return !left.Equals(right);
    }

    public static bool operator <(BigNumber left, BigNumber right)
    {
        return left.CompareTo(right) < 0;
    }

    public static bool operator >(BigNumber left, BigNumber right)
    {
        return left.CompareTo(right) > 0;
    }

    public static bool operator <=(BigNumber left, BigNumber right)
    {
        return left.CompareTo(right) <= 0;
    }

    public static bool operator >=(BigNumber left, BigNumber right)
    {
        return left.CompareTo(right) >= 0;
    }

    public static BigNumber Max(BigNumber left, BigNumber right)
    {
        return left >= right ? left : right;
    }

    public static BigNumber Min(BigNumber left, BigNumber right)
    {
        return left <= right ? left : right;
    }

    public static BigNumber Pow(BigNumber value, double power)
    {
        if (power == 0d)
        {
            return One;
        }

        if (value.Sign <= 0)
        {
            return Zero;
        }

        var logarithm = value.Log10() * power;
        var exponent = Math.Floor(logarithm);
        return new BigNumber(Math.Pow(10d, logarithm - exponent), (long)exponent);
    }

    public double Log10()
    {
        return Math.Log10(Math.Abs(_mantissa)) + _exponent;
    }

    public BigNumber Floor()
    {
        if (_exponent >= MantissaDecimals)
        {
            return this;
        }

        if (_exponent < 0)
        {
            return _mantissa < 0d ? -One : Zero;
        }

        return new BigNumber(Math.Floor(ToDouble()), 0L);
    }

    public BigNumber Round()
    {
        if (_exponent >= MantissaDecimals)
        {
            return this;
        }

        return new BigNumber(Math.Round(ToDouble(), MidpointRounding.AwayFromZero), 0L);
    }

    public double ToDouble()
    {
        if (_exponent > MaxDoubleExponent)
        {
            return _mantissa < 0d ? double.NegativeInfinity : double.PositiveInfinity;
        }

        if (_exponent < MinDoubleExponent)
        {
            return 0d;
        }

        if (_exponent >= 0L && _exponent <= MantissaDecimals)
        {
            return Math.Round(_mantissa * s_powersOfTen[_exponent], MantissaDecimals - (int)_exponent);
        }

        return ScaleByPowerOfTen(_mantissa, _exponent);
    }

    public bool Equals(BigNumber other)
    {
        return _mantissa == other._mantissa && _exponent == other._exponent;
    }

    public override bool Equals(object? obj)
    {
        return obj is BigNumber other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(_mantissa, _exponent);
    }

    public int CompareTo(BigNumber other)
    {
        var sign = Sign;
        var otherSign = other.Sign;

        if (sign != otherSign)
        {
            return sign.CompareTo(otherSign);
        }

        if (sign == 0)
        {
            return 0;
        }

        if (_exponent != other._exponent)
        {
            return _exponent.CompareTo(other._exponent) * sign;
        }

        return _mantissa.CompareTo(other._mantissa);
    }

    public static bool TryParse(string? text, out BigNumber value)
    {
        return TryParse(text.AsSpan(), out value);
    }

    public static bool TryParse(ReadOnlySpan<char> text, out BigNumber value)
    {
        value = Zero;
        text = text.Trim();
        var separator = text.IndexOfAny('e', 'E');
        var number = separator < 0 ? text : text.Slice(0, separator);
        var exponent = 0L;

        if (number.Length == 0 || !double.TryParse(number, NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out var mantissa))
        {
            return false;
        }

        if (separator >= 0 && !long.TryParse(text.Slice(separator + 1), NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture, out exponent))
        {
            return false;
        }

        value = new BigNumber(mantissa, exponent);
        return true;
    }

    public override string ToString()
    {
        var mantissa = _mantissa.ToString("R", CultureInfo.InvariantCulture);
        return _exponent == 0L ? mantissa : $"{mantissa}e{_exponent.ToString(CultureInfo.InvariantCulture)}";
    }

    private static void Normalize(double mantissa, long exponent, out double normalizedMantissa, out long normalizedExponent)
    {
        if (mantissa == 0d || double.IsNaN(mantissa) || double.IsInfinity(mantissa))
        {
            normalizedMantissa = 0d;
            normalizedExponent = 0L;
            return;
        }

        var shift = (int)Math.Floor(Math.Log10(Math.Abs(mantissa)));
        var shifted = ScaleByPowerOfTen(mantissa, -shift);

        if (Math.Abs(shifted) >= 10d)
        {
            shifted /= 10d;
            shift++;
        }
        else if (Math.Abs(shifted) < 1d)
        {
            shifted *= 10d;
            shift--;
        }

        shifted = Math.Round(shifted, MantissaDecimals);

        if (Math.Abs(shifted) >= 10d)
        {
            shifted /= 10d;
            shift++;
        }

        if (double.IsNaN(shifted) || double.IsInfinity(shifted))
        {
            normalizedMantissa = 0d;
            normalizedExponent = 0L;
            return;
        }

        normalizedMantissa = shifted;
        normalizedExponent = exponent + shift;
    }

    private static double ScaleByPowerOfTen(double value, long power)
    {
        if (power >= 0L && power < s_powersOfTen.Length)
        {
            return value * s_powersOfTen[power];
        }

        if (power < 0L && -power < s_powersOfTen.Length)
        {
            return value / s_powersOfTen[-power];
        }

        return value * Math.Pow(10d, power);
    }
}
