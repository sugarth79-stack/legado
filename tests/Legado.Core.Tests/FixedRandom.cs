using Legado.Core.Fruits;

namespace Legado.Core.Tests;

public sealed class FixedRandom : IRandomSource
{
    private readonly int _value;

    public FixedRandom(int value) => _value = value;

    public int NextPercent() => _value;
}
