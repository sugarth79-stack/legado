using Legado.Core.Renown;

namespace Legado.Core.Tests.Renown;

public class RenownRulesTests
{
    [Theory]
    [InlineData(1000, 800)] [InlineData(999, 799)] [InlineData(0, 0)]
    public void AfterDeath_subtracts_twenty_percent_rounding_down(long renown, long expected)
        => Assert.Equal(expected, RenownRules.AfterDeath(renown));

    [Fact]
    public void AfterDeath_rejects_negative()
        => Assert.Throws<ArgumentOutOfRangeException>(() => RenownRules.AfterDeath(-1));

    [Theory]
    [InlineData(1, RenownRank.Gold)] [InlineData(10, RenownRank.Gold)]
    [InlineData(11, RenownRank.Purple)] [InlineData(100, RenownRank.Purple)]
    [InlineData(101, RenownRank.Blue)] [InlineData(250, RenownRank.Blue)]
    [InlineData(251, RenownRank.Green)] [InlineData(500, RenownRank.Green)]
    [InlineData(501, RenownRank.Gray)] [InlineData(1000, RenownRank.Gray)]
    public void Rank_uses_inclusive_top_percentage_thresholds(int position, RenownRank expected)
        => Assert.Equal(expected, RenownRules.Rank(position, 1000));

    // Fija el comportamiento actual con poblaciones pequeñas; pendiente de decisión de diseño.
    [Theory]
    [InlineData(1, 8, RenownRank.Blue)]
    [InlineData(1, 1, RenownRank.Gray)]
    [InlineData(1, 100, RenownRank.Gold)]
    public void Rank_small_population_pins_current_behavior(int position, int population, RenownRank expected)
        => Assert.Equal(expected, RenownRules.Rank(position, population));

    [Theory]
    [InlineData(0, 10)] [InlineData(11, 10)] [InlineData(1, 0)]
    public void Rank_rejects_invalid_arguments(int position, int population)
        => Assert.Throws<ArgumentOutOfRangeException>(() => RenownRules.Rank(position, population));
}
