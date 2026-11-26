using Legado.Core.Dungeons;

namespace Legado.Core.Tests.Dungeons;

public class DungeonRulesTests
{
    [Fact]
    public void CanEnter_Max_WithNpc_IsFalse() =>
        Assert.False(DungeonRules.CanEnter(Difficulty.Max, 5, 1));

    [Fact]
    public void CanEnter_Max_SixPlayersNoNpcs_IsTrue() =>
        Assert.True(DungeonRules.CanEnter(Difficulty.Max, 6, 0));

    [Fact]
    public void CanEnter_Basic_OnePlayerFiveNpcs_IsTrue() =>
        Assert.True(DungeonRules.CanEnter(Difficulty.Basic, 1, 5));

    [Fact]
    public void CanEnter_Basic_SevenMembers_IsFalse() =>
        Assert.False(DungeonRules.CanEnter(Difficulty.Basic, 4, 3));

    [Fact]
    public void CanEnter_EmptyGroup_IsFalse() =>
        Assert.False(DungeonRules.CanEnter(Difficulty.Basic, 0, 0));

    [Fact]
    public void CanEnter_NegativePlayers_Throws() =>
        Assert.Throws<ArgumentOutOfRangeException>(() => DungeonRules.CanEnter(Difficulty.Basic, -1, 2));

    [Fact]
    public void CanEnter_NegativeNpcs_Throws() =>
        Assert.Throws<ArgumentOutOfRangeException>(() => DungeonRules.CanEnter(Difficulty.Normal, 2, -1));

    [Fact]
    public void CanDropFruit_NonStoryNormal_IsFalse() =>
        Assert.False(DungeonRules.CanDropFruit(false, Difficulty.Normal));

    [Fact]
    public void CanDropFruit_StoryBasic_IsTrue() =>
        Assert.True(DungeonRules.CanDropFruit(true, Difficulty.Basic));

    [Fact]
    public void CanDropFruit_NonStoryMax_IsTrue() =>
        Assert.True(DungeonRules.CanDropFruit(false, Difficulty.Max));
}
