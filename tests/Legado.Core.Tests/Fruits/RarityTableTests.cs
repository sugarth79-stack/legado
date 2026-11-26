using Legado.Core.Fruits;
using Legado.Core.World;

namespace Legado.Core.Tests.Fruits;

public class RarityTableTests
{
    [Theory]
    [InlineData(Zone.FactionSea, 0, Rarity.Common)]
    [InlineData(Zone.FactionSea, 99, Rarity.Common)]
    [InlineData(Zone.Gullet, 79, Rarity.Common)]
    [InlineData(Zone.Gullet, 80, Rarity.Uncommon)]
    [InlineData(Zone.WildWaters, 79, Rarity.Uncommon)]
    [InlineData(Zone.WildWaters, 80, Rarity.Rare)]
    [InlineData(Zone.DrownedKingsSea, 79, Rarity.Rare)]
    [InlineData(Zone.DrownedKingsSea, 80, Rarity.Mythic)]
    [InlineData(Zone.DrownedKingsSea, 99, Rarity.Mythic)]
    public void Roll_maps_percent_to_rarity(Zone zone, int roll, Rarity expected)
        => Assert.Equal(expected, RarityTable.Roll(zone, new FixedRandom(roll)));

    [Theory]
    [InlineData(-1)]
    [InlineData(100)]
    public void Roll_rejects_percent_outside_0_99(int roll)
        => Assert.Throws<ArgumentOutOfRangeException>(() => RarityTable.Roll(Zone.Gullet, new FixedRandom(roll)));
}
