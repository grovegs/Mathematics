using System.Collections.Generic;
using GroveGames.Serialization;

namespace GroveGames.Mathematics.Unity.Serialization.Editor.Tests
{
    [Schema]
    public sealed class BigNumberRecord
    {
        public BigNumber Balance;
        public List<BigNumber> History = new();
    }
}
