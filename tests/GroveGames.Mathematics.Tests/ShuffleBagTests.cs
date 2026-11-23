using System.Collections.Generic;

namespace GroveGames.Mathematics.Tests;

public sealed class ShuffleBagTests
{
    private const uint Seed = 12345u;
    private const int Count = 5;
    private const int Rounds = 200;

    [Fact]
    public void TryDraw_OneRound_ReturnsEveryItemOnce()
    {
        var bag = new ShuffleBag(Count);
        var random = new RandomNumberGenerator(Seed);
        var drawn = new HashSet<int>();
        bag.Reset(Count);

        for (var i = 0; i < Count; i++)
        {
            bag.TryDraw(random, out var item);
            drawn.Add(item);
        }

        Assert.True(drawn.SetEquals(new[] { 0, 1, 2, 3, 4 }));
        Assert.Equal(0, bag.Remaining);
    }

    [Fact]
    public void TryDraw_ManyRounds_NeverReturnsSameItemTwiceInARow()
    {
        var bag = new ShuffleBag(Count);
        var random = new RandomNumberGenerator(Seed);
        bag.Reset(Count);
        bag.TryDraw(random, out var previous);
        var repeats = 0;

        for (var i = 1; i < Count * Rounds; i++)
        {
            bag.TryDraw(random, out var item);

            if (item == previous)
            {
                repeats++;
            }

            previous = item;
        }

        Assert.Equal(0, repeats);
    }

    [Fact]
    public void TryDraw_SameSeed_ReturnsSameSequence()
    {
        var first = Draw(new RandomNumberGenerator(Seed));

        var second = Draw(new RandomNumberGenerator(Seed));

        Assert.Equal(first, second);
    }

    [Fact]
    public void TryDraw_SingleItem_ReturnsIt()
    {
        var bag = new ShuffleBag(1);
        var random = new RandomNumberGenerator(Seed);
        bag.Reset(1);

        var first = bag.TryDraw(random, out var firstItem);
        var second = bag.TryDraw(random, out var secondItem);

        Assert.True(first && second);
        Assert.Equal(0, firstItem);
        Assert.Equal(0, secondItem);
    }

    [Fact]
    public void TryDraw_Empty_ReturnsFalse()
    {
        var bag = new ShuffleBag(0);
        bag.Reset(0);

        var drawn = bag.TryDraw(new RandomNumberGenerator(Seed), out var item);

        Assert.False(drawn);
        Assert.Equal(-1, item);
    }

    [Fact]
    public void Reset_MidRound_StartsAFullRound()
    {
        var bag = new ShuffleBag(Count);
        var random = new RandomNumberGenerator(Seed);
        bag.Reset(Count);
        bag.TryDraw(random, out _);

        bag.Reset(Count);
        bag.TryDraw(random, out _);

        Assert.Equal(Count - 1, bag.Remaining);
    }

    private static List<int> Draw(IRandomNumberGenerator random)
    {
        var bag = new ShuffleBag(Count);
        var items = new List<int>(Count * 3);
        bag.Reset(Count);

        for (var i = 0; i < Count * 3; i++)
        {
            bag.TryDraw(random, out var item);
            items.Add(item);
        }

        return items;
    }
}
