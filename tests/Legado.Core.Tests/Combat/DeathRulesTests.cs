using Legado.Core.Combat;

namespace Legado.Core.Tests.Combat;

public class DeathRulesTests
{
    [Fact]
    public void Pvp_online_loses_items_and_fruits_killer_loots()
    {
        var o = DeathRules.Resolve(new(DeathCause.Player, VictimOnline: true, InPvpZone: true));
        Assert.True(o.LosesCarriedItems);
        Assert.True(o.LosesFruits);
        Assert.True(o.KillerLoots);
    }

    [Fact]
    public void Pvp_offline_guard_keeps_fruits()
    {
        var o = DeathRules.Resolve(new(DeathCause.Player, VictimOnline: false, InPvpZone: true));
        Assert.True(o.LosesCarriedItems);
        Assert.False(o.LosesFruits);
    }

    [Fact]
    public void Pve_keeps_fruits_corpse_lasts_30_minutes()
    {
        var o = DeathRules.Resolve(new(DeathCause.Monster, true, InPvpZone: false));
        Assert.True(o.LosesCarriedItems);
        Assert.False(o.LosesFruits);
        Assert.False(o.LootableByAnyone);
        Assert.Equal(TimeSpan.FromMinutes(30), o.CorpseLifetime);
    }

    [Fact]
    public void Pve_in_pvp_zone_corpse_lootable_by_anyone()
        => Assert.True(DeathRules.Resolve(new(DeathCause.Monster, true, InPvpZone: true)).LootableByAnyone);

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void Shipwreck_loses_everything_including_fruits(bool online)
    {
        var o = DeathRules.Resolve(new(DeathCause.Shipwreck, online, InPvpZone: true));
        Assert.True(o.LosesCarriedItems);
        Assert.True(o.LosesFruits);
    }

    [Fact]
    public void Shipwreck_has_no_recoverable_corpse_and_killer_does_not_loot()
    {
        var o = DeathRules.Resolve(new(DeathCause.Shipwreck, true, InPvpZone: false));
        Assert.Null(o.CorpseLifetime);
        Assert.False(o.KillerLoots);
    }

    [Fact]
    public void Non_shipwreck_deaths_use_pve_corpse_lifetime()
        => Assert.Equal(DeathRules.PveCorpseLifetime,
            DeathRules.Resolve(new(DeathCause.Player, true, InPvpZone: true)).CorpseLifetime);
}
