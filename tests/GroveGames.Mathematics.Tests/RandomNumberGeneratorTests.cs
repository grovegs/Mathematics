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
    [InlineData(1u, new[] { 0, 0, 6, 0, 6, 1 }, 632435481u, -1711727438, 1055854304, 4604290726073730454L, true)]
    [InlineData(7u, new[] { 0, 1, 9, 7, 7, 5 }, 258808761u, -1352942761, 1052524348, 4590451353780957136L, false)]
    [InlineData(12345u, new[] { 8, 4, 7, 5, 1, 8 }, 4285686167u, 1533427512, 1057793822, 4602764178701754236L, true)]
    [InlineData(2654435761u, new[] { 3, 8, 9, 6, 10, 7 }, 1120789035u, -1349799832, 1053993488, 4605139209532388508L, true)]
    [InlineData(4294967295u, new[] { 0, 10, 5, 10, 5, 6 }, 1705115567u, -1867677189, 1060291718, 4600805257049269180L, false)]
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
}
