using System.Collections.Generic;
using GroveGames.Mathematics;

namespace GroveGames.Mathematics.Testing
{
    public sealed class FakeRandomNumberGenerator : IRandomNumberGenerator
    {
        private readonly Queue<int> _integers;
        private readonly Queue<bool> _booleans;

        public uint Seed => 0;

        public FakeRandomNumberGenerator()
        {
            _integers = new Queue<int>();
            _booleans = new Queue<bool>();
        }

        public void EnqueueInt(int value)
        {
            _integers.Enqueue(value);
        }

        public void EnqueueBool(bool value)
        {
            _booleans.Enqueue(value);
        }

        public bool NextBool()
        {
            return _booleans.Count > 0 && _booleans.Dequeue();
        }

        public int NextInt()
        {
            return _integers.Count > 0 ? _integers.Dequeue() : 0;
        }

        public int NextInt(int max)
        {
            return System.Math.Min(NextInt(), max);
        }

        public int NextInt(int min, int max)
        {
            return min + NextInt(max - min);
        }

        public uint NextUInt()
        {
            return (uint)NextInt();
        }

        public uint NextUInt(uint max)
        {
            return (uint)NextInt((int)max);
        }

        public uint NextUInt(uint min, uint max)
        {
            return (uint)NextInt((int)min, (int)max);
        }

        public float NextFloat()
        {
            return 0f;
        }

        public float NextFloat(float max)
        {
            return 0f;
        }

        public float NextFloat(float min, float max)
        {
            return min;
        }

        public double NextDouble()
        {
            return 0d;
        }

        public double NextDouble(double max)
        {
            return 0d;
        }

        public double NextDouble(double min, double max)
        {
            return min;
        }
    }
}
