using Legado.Core.Progression;

namespace Legado.Core.Tests.Progression;

public class FruitSlotsTests
{
    [Theory]
    [InlineData(1, 0)] [InlineData(9, 0)] [InlineData(10, 1)] [InlineData(29, 1)]
    [InlineData(30, 2)] [InlineData(50, 3)] [InlineData(69, 3)] [InlineData(70, 4)]
    [InlineData(79, 4)] [InlineData(80, 5)]
    public void ForLevel_returns_slots_unlocked(int level, int expected)
        => Assert.Equal(expected, FruitSlots.ForLevel(level));

    [Theory] [InlineData(0)] [InlineData(-1)] [InlineData(81)]
    public void ForLevel_rejects_out_of_range(int level)
        => Assert.Throws<ArgumentOutOfRangeException>(() => FruitSlots.ForLevel(level));
}
