using Legado.Core.Professions;

namespace Legado.Core.Tests.Professions;

public class ProfessionRulesTests
{
    [Theory]
    [InlineData(double.NaN)]
    [InlineData(double.PositiveInfinity)]
    [InlineData(double.NegativeInfinity)]
    public void OfflineGain_rejects_non_finite(double gain)
        => Assert.Throws<ArgumentOutOfRangeException>(() => ProfessionRules.OfflineGain(gain));

    [Fact]
    public void Profession_has_21_values()
        => Assert.Equal(21, Enum.GetValues<Profession>().Length);

    [Fact]
    public void MaxPerCharacter_is_3()
        => Assert.Equal(3, ProfessionRules.MaxPerCharacter);

    [Fact]
    public void Three_distinct_professions_are_valid()
        => Assert.True(ProfessionRules.IsValidCombination(new[] { Profession.Miner, Profession.Cook, Profession.Diver }));

    [Fact]
    public void Empty_combination_is_valid()
        => Assert.True(ProfessionRules.IsValidCombination(Array.Empty<Profession>()));

    [Fact]
    public void Four_professions_are_invalid()
        => Assert.False(ProfessionRules.IsValidCombination(new[] { Profession.Miner, Profession.Cook, Profession.Diver, Profession.Tamer }));

    [Fact]
    public void Repeated_profession_is_invalid()
        => Assert.False(ProfessionRules.IsValidCombination(new[] { Profession.Miner, Profession.Miner }));

    [Fact]
    public void Null_combination_throws()
        => Assert.Throws<ArgumentNullException>(() => ProfessionRules.IsValidCombination(null!));

    [Fact]
    public void Frozen_diver_still_occupies_slot()
        => Assert.True(ProfessionRules.IsValidCombination(new[] { Profession.Diver, Profession.Fisher, Profession.Cook }));

    [Fact]
    public void Diver_is_active_only_without_fruits()
    {
        Assert.True(ProfessionRules.IsActive(Profession.Diver, 0));
        Assert.False(ProfessionRules.IsActive(Profession.Diver, 1));
    }

    [Fact]
    public void Other_professions_are_always_active()
        => Assert.True(ProfessionRules.IsActive(Profession.Fisher, 3));

    [Fact]
    public void Diver_reactivates_after_losing_fruits()
    {
        // Congelado con 2 frutas; al volver a 0 se descongela (el nivel nunca se tocó).
        Assert.False(ProfessionRules.IsActive(Profession.Diver, 2));
        Assert.True(ProfessionRules.IsActive(Profession.Diver, 0));
    }

    [Fact]
    public void CanGainSkill_follows_IsActive()
    {
        Assert.False(ProfessionRules.CanGainSkill(Profession.Diver, 1));
        Assert.True(ProfessionRules.CanGainSkill(Profession.Diver, 0));
        Assert.True(ProfessionRules.CanGainSkill(Profession.Fisher, 3));
    }

    [Fact]
    public void Negative_fruits_throw()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => ProfessionRules.IsActive(Profession.Diver, -1));
        Assert.Throws<ArgumentOutOfRangeException>(() => ProfessionRules.CanGainSkill(Profession.Diver, -1));
    }

    [Fact]
    public void OfflineGain_is_one_thousandth()
        => Assert.Equal(1.0, ProfessionRules.OfflineGain(1000), 1e-9);

    [Fact]
    public void OfflineGain_negative_throws()
        => Assert.Throws<ArgumentOutOfRangeException>(() => ProfessionRules.OfflineGain(-1));
}
