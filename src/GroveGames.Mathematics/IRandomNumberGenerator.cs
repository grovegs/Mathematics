namespace GroveGames.Mathematics;

public interface IRandomNumberGenerator
{
    public uint Seed { get; }
    public bool NextBool();
    public int NextInt();
    public int NextInt(int max);
    public int NextInt(int min, int max);
    public uint NextUInt();
    public uint NextUInt(uint max);
    public uint NextUInt(uint min, uint max);
    public float NextFloat();
    public float NextFloat(float max);
    public float NextFloat(float min, float max);
    public double NextDouble();
    public double NextDouble(double max);
    public double NextDouble(double min, double max);
}
