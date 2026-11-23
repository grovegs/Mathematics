using System.Collections.Generic;

namespace GroveGames.Mathematics;

public sealed class ShuffleBag
{
    private readonly List<int> _items;
    private int _count;
    private int _last;

    public int Count => _count;
    public int Remaining => _items.Count;

    public ShuffleBag(int capacity)
    {
        _items = new List<int>(capacity);
        _last = -1;
    }

    public void Reset(int count)
    {
        _count = count < 0 ? 0 : count;
        _items.Clear();
        _last = -1;
    }

    public bool TryDraw(IRandomNumberGenerator random, out int item)
    {
        if (_count == 0)
        {
            item = -1;
            return false;
        }

        if (_items.Count == 0)
        {
            Refill();
        }

        var lastIndex = _items.Count - 1;
        var index = random.NextInt(lastIndex);

        if (_items[index] == _last && lastIndex > 0)
        {
            index = (index + 1 + random.NextInt(lastIndex - 1)) % _items.Count;
        }

        item = _items[index];
        _items[index] = _items[lastIndex];
        _items.RemoveAt(lastIndex);
        _last = item;
        return true;
    }

    private void Refill()
    {
        for (var i = 0; i < _count; i++)
        {
            _items.Add(i);
        }
    }
}
