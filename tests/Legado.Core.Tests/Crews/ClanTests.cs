using Legado.Core.Crews;

namespace Legado.Core.Tests.Crews;

public class ClanTests
{
    [Fact]
    public void Sixty_players_fit_and_the_61st_is_rejected()
    {
        var clan = new Clan();
        for (var i = 0; i < 60; i++)
            Assert.True(clan.TryAdd("p" + i));

        Assert.False(clan.TryAdd("p60"));
        Assert.Equal(60, clan.Count);
    }

    [Fact]
    public void Duplicate_id_is_rejected()
    {
        var clan = new Clan();
        Assert.True(clan.TryAdd("p1"));

        Assert.False(clan.TryAdd("p1"));
        Assert.Equal(1, clan.Count);
    }

    [Fact]
    public void Null_id_throws()
        => Assert.Throws<ArgumentNullException>(() => new Clan().TryAdd(null!));
}
