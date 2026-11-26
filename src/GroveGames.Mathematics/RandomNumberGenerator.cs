namespace GroveGames.Mathematics;

public sealed class RandomNumberGenerator : IRandomNumberGenerator
{
    private const uint SignBit = 0x80000000u;
    private const uint FloatOneBits = 0x3f800000u;
    private const ulong DoubleOneBits = 0x3ff0000000000000ul;
    private const uint SeedMultiplier = 2654435761u;

    private readonly uint _seed;
    private uint _state;

    public uint Seed => _seed;

    public RandomNumberGenerator(uint seed)
    {
        ArgumentOutOfRangeException.ThrowIfZero(seed);

        _seed = seed;
        _state = unchecked(seed * SeedMultiplier);
        NextState();
    }

    public RandomNumberGenerator() : this(CreateSeed())
    {
    }

    public bool NextBool()
    {
        return (NextState() & 1u) == 1u;
    }

    public int NextInt()
    {
        return unchecked((int)(NextState() ^ SignBit));
    }

    public int NextInt(int max)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(max);

        return unchecked((int)Scale((uint)max + 1u));
    }

    public int NextInt(int min, int max)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(max, min);

        return unchecked((int)(Scale((uint)(max - min) + 1u) + (uint)min));
    }

    public uint NextUInt()
    {
        return unchecked(NextState() - 1u);
    }

    public uint NextUInt(uint max)
    {
        return Scale(unchecked(max + 1u));
    }

    public uint NextUInt(uint min, uint max)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(max, min);

        return unchecked(Scale(max - min + 1u) + min);
    }

    public float NextFloat()
    {
        return BitConverter.Int32BitsToSingle(unchecked((int)(FloatOneBits | (NextState() >> 9)))) - 1f;
    }

    public float NextFloat(float max)
    {
        return NextFloat() * max;
    }

    public float NextFloat(float min, float max)
    {
        return NextFloat() * (max - min) + min;
    }

    public double NextDouble()
    {
        var high = (ulong)NextState() << 20;
        var bits = high ^ NextState();
        return BitConverter.Int64BitsToDouble(unchecked((long)(DoubleOneBits | bits))) - 1d;
    }

    public double NextDouble(double max)
    {
        return NextDouble() * max;
    }

    public double NextDouble(double min, double max)
    {
        return NextDouble() * (max - min) + min;
    }

    private static uint CreateSeed()
    {
        var seed = unchecked((uint)DateTime.UtcNow.Ticks);
        return seed == 0u ? 1u : seed;
    }

    private uint Scale(uint range)
    {
        return range == 0u ? NextUInt() : (uint)(NextState() * (ulong)range >> 32);
    }

    private uint NextState()
    {
        var current = _state;
        _state ^= _state << 13;
        _state ^= _state >> 17;
        _state ^= _state << 5;
        return current;
    }
}
