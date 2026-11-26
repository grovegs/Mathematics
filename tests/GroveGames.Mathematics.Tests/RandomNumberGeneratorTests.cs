namespace GroveGames.Mathematics.Tests;

public sealed class RandomNumberGeneratorTests
{
    [Fact]
    public void Constructor_ZeroSeed_ThrowsArgumentOutOfRangeException()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new RandomNumberGenerator(0u));
    }

    [Fact]
    public void Constructor_Seed_KeepsSeed()
    {
        var random = new RandomNumberGenerator(12345u);

        Assert.Equal(12345u, random.Seed);
    }

    [Fact]
    public void Constructor_NoSeed_PicksNonZeroSeed()
    {
        var random = new RandomNumberGenerator();

        Assert.NotEqual(0u, random.Seed);
    }

    [Theory]
    [InlineData(1u, new[] { 3, 8, 9, 6, 10, 7 }, 1120789035u, -1349799832, 1053993488, 4605139209532388508L, true)]
    [InlineData(7u, new[] { 9, 4, 6, 6, 3, 0 }, 1721955725u, -1166665369, 1062341180, 4594655660068086424L, true)]
    [InlineData(12345u, new[] { 8, 7, 6, 6, 0, 3 }, 3613875398u, 587986360, 1054105580, 4589002574752975712L, false)]
    [InlineData(2654435761u, new[] { 10, 5, 0, 9, 7, 0 }, 3044154247u, -1234210391, 1062411660, 4596641793462292784L, false)]
    [InlineData(4294967295u, new[] { 3, 2, 2, 4, 1, 1 }, 48115333u, -1495323359, 1059854078, 4587685036453793088L, false)]
    public void Next_Seed_ReturnsPinnedSequence(uint seed, int[] integers, uint unsignedInteger, int integer, int floatBits, long doubleBits, bool boolean)
    {
        var random = new RandomNumberGenerator(seed);
        var drawn = new int[integers.Length];

        for (var i = 0; i < drawn.Length; i++)
        {
            drawn[i] = random.NextInt(10);
        }

        Assert.Equal(integers, drawn);
        Assert.Equal(unsignedInteger, random.NextUInt());
        Assert.Equal(integer, random.NextInt());
        Assert.Equal(floatBits, BitConverter.SingleToInt32Bits(random.NextFloat()));
        Assert.Equal(doubleBits, BitConverter.DoubleToInt64Bits(random.NextDouble()));
        Assert.Equal(boolean, random.NextBool());
    }

    [Fact]
    public void NextInt_Max_IncludesMax()
    {
        var random = new RandomNumberGenerator(7u);
        var seen = new bool[4];

        for (var i = 0; i < 1000; i++)
        {
            seen[random.NextInt(3)] = true;
        }

        Assert.DoesNotContain(false, seen);
    }

    [Fact]
    public void NextInt_MinAndMax_StaysInInclusiveRange()
    {
        var random = new RandomNumberGenerator(7u);
        var lowest = int.MaxValue;
        var highest = int.MinValue;

        for (var i = 0; i < 1000; i++)
        {
            var value = random.NextInt(-2, 2);
            lowest = Math.Min(lowest, value);
            highest = Math.Max(highest, value);
        }

        Assert.Equal(-2, lowest);
        Assert.Equal(2, highest);
    }

    [Fact]
    public void NextUInt_MinAndMax_StaysInInclusiveRange()
    {
        var random = new RandomNumberGenerator(7u);
        var lowest = uint.MaxValue;
        var highest = uint.MinValue;

        for (var i = 0; i < 1000; i++)
        {
            var value = random.NextUInt(10u, 12u);
            lowest = Math.Min(lowest, value);
            highest = Math.Max(highest, value);
        }

        Assert.Equal(10u, lowest);
        Assert.Equal(12u, highest);
    }

    [Fact]
    public void NextFloat_MinAndMax_StaysBelowMax()
    {
        var random = new RandomNumberGenerator(7u);

        for (var i = 0; i < 1000; i++)
        {
            var value = random.NextFloat(-2f, 2f);

            Assert.InRange(value, -2f, 2f);
            Assert.NotEqual(2f, value);
        }
    }

    [Fact]
    public void NextDouble_NoRange_StaysInUnitInterval()
    {
        var random = new RandomNumberGenerator(7u);

        for (var i = 0; i < 1000; i++)
        {
            var value = random.NextDouble();

            Assert.InRange(value, 0d, 1d);
            Assert.NotEqual(1d, value);
        }
    }

    [Fact]
    public void Next_RangesWithSeed_ReturnPinnedSequence()
    {
        var random = new RandomNumberGenerator(7u);
        var integers = new int[6];
        var unsignedIntegers = new uint[6];
        var unsignedMaxima = new uint[6];

        for (var i = 0; i < 6; i++)
        {
            integers[i] = random.NextInt(-5, 5);
        }

        for (var i = 0; i < 6; i++)
        {
            unsignedIntegers[i] = random.NextUInt(3u, 9u);
        }

        for (var i = 0; i < 6; i++)
        {
            unsignedMaxima[i] = random.NextUInt(100u);
        }

        Assert.Equal(new[] { 4, -1, 1, 1, -2, -5 }, integers);
        Assert.Equal(new[] { 5u, 4u, 8u, 4u, 8u, 3u }, unsignedIntegers);
        Assert.Equal(new[] { 32u, 36u, 86u, 79u, 20u, 20u }, unsignedMaxima);
    }

    [Fact]
    public void NextInt_RangeWiderThanIntMaxValue_StaysInRange()
    {
        var random = new RandomNumberGenerator(7u);
        var outside = 0;

        for (var i = 0; i < 1000; i++)
        {
            var value = random.NextInt(-2_000_000_000, 2_000_000_000);

            if (value < -2_000_000_000 || value > 2_000_000_000)
            {
                outside++;
            }
        }

        Assert.Equal(0, outside);
    }

    [Fact]
    public void NextInt_FullRange_ReturnsDifferentValues()
    {
        var random = new RandomNumberGenerator(7u);
        var values = new HashSet<int>();

        for (var i = 0; i < 100; i++)
        {
            values.Add(random.NextInt(int.MinValue, int.MaxValue));
        }

        Assert.True(values.Count > 90);
    }

    [Fact]
    public void NextInt_IntMaxValue_ReturnsNonNegativeValues()
    {
        var random = new RandomNumberGenerator(7u);
        var negative = 0;

        for (var i = 0; i < 1000; i++)
        {
            if (random.NextInt(int.MaxValue) < 0)
            {
                negative++;
            }
        }

        Assert.Equal(0, negative);
    }

    [Fact]
    public void NextUInt_UIntMaxValue_ReturnsDifferentValues()
    {
        var random = new RandomNumberGenerator(7u);
        var values = new HashSet<uint>();

        for (var i = 0; i < 100; i++)
        {
            values.Add(random.NextUInt(uint.MaxValue));
            values.Add(random.NextUInt(0u, uint.MaxValue));
        }

        Assert.True(values.Count > 190);
    }

    [Fact]
    public void NextInt_NegativeMax_ThrowsArgumentOutOfRangeException()
    {
        var random = new RandomNumberGenerator(7u);

        Assert.Throws<ArgumentOutOfRangeException>(() => random.NextInt(-1));
    }

    [Fact]
    public void NextInt_MaxBelowMin_ThrowsArgumentOutOfRangeException()
    {
        var random = new RandomNumberGenerator(7u);

        Assert.Throws<ArgumentOutOfRangeException>(() => random.NextInt(5, 1));
    }

    [Fact]
    public void NextUInt_MaxBelowMin_ThrowsArgumentOutOfRangeException()
    {
        var random = new RandomNumberGenerator(7u);

        Assert.Throws<ArgumentOutOfRangeException>(() => random.NextUInt(5u, 1u));
    }
}
