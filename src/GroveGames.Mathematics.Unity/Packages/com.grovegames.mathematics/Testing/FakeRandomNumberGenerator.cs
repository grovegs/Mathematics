using System.Collections.Generic;
using GroveGames.Mathematics;

namespace GroveGames.Mathematics.Testing
{
    public sealed class FakeRandomNumberGenerator : IRandomNumberGenerator
    {
        private readonly Queue<int> _integers;
        private readonly Queue<bool> _booleans;
        private readonly Queue<float> _floats;
        private readonly Queue<double> _doubles;

        public uint Seed => 0;

        public FakeRandomNumberGenerator()
        {
            _integers = new Queue<int>();
            _booleans = new Queue<bool>();
            _floats = new Queue<float>();
            _doubles = new Queue<double>();
        }

        public void EnqueueInt(int value)
        {
            _integers.Enqueue(value);
        }

        public void EnqueueBool(bool value)
        {
            _booleans.Enqueue(value);
        }

        public void EnqueueFloat(float value)
        {
            _floats.Enqueue(value);
        }

        public void EnqueueDouble(double value)
        {
            _doubles.Enqueue(value);
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
            if (max < 0)
            {
                throw new System.ArgumentOutOfRangeException(nameof(max), max, "The maximum cannot be negative.");
            }

            return System.Math.Min(NextInt(), max);
        }

        public int NextInt(int min, int max)
        {
            if (max < min)
            {
                throw new System.ArgumentOutOfRangeException(nameof(max), max, "The maximum cannot be below the minimum.");
            }

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
            if (max < min)
            {
                throw new System.ArgumentOutOfRangeException(nameof(max), max, "The maximum cannot be below the minimum.");
            }

            return (uint)NextInt((int)min, (int)max);
        }

        public float NextFloat()
        {
            return _floats.Count > 0 ? _floats.Dequeue() : 0f;
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
            return _doubles.Count > 0 ? _doubles.Dequeue() : 0d;
        }

        public double NextDouble(double max)
        {
            return NextDouble() * max;
        }

        public double NextDouble(double min, double max)
        {
            return NextDouble() * (max - min) + min;
        }
    }
}
