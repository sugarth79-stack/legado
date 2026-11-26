using Legado.Core.Crews;

namespace Legado.Core.Tests.Crews;

public class CrewTests
{
    [Theory] [InlineData(0)] [InlineData(13)] [InlineData(-1)]
    public void Constructor_rejects_capacity_out_of_range(int capacity)
        => Assert.Throws<ArgumentOutOfRangeException>(() => new Crew(capacity));

    [Theory] [InlineData(1)] [InlineData(12)]
    public void Constructor_accepts_capacity_bounds(int capacity)
        => Assert.Empty(new Crew(capacity).Members);

    [Fact]
    public void Members_is_not_castable_to_list()
        => Assert.False(new Crew(2).Members is List<CrewMember>);

    [Fact]
    public void AddPlayer_with_free_slot_joins()
    {
        var crew = new Crew(3);
        crew.TryAddNpc(new NpcMember("n1"));

        Assert.Equal(JoinResult.Joined, crew.AddPlayer(new PlayerMember("p1")));
        Assert.Equal(2, crew.Members.Count);
    }

    [Fact]
    public void AddPlayer_on_full_crew_of_npcs_replaces_last_added_npc()
    {
        var crew = new Crew(3);
        crew.TryAddNpc(new NpcMember("n1"));
        crew.TryAddNpc(new NpcMember("n2"));
        crew.TryAddNpc(new NpcMember("n3"));

        Assert.Equal(JoinResult.ReplacedNpc, crew.AddPlayer(new PlayerMember("p1")));

        Assert.Equal(3, crew.Members.Count);
        Assert.Single(crew.Members.OfType<PlayerMember>());
        Assert.DoesNotContain(crew.Members, m => m.Id == "n3");
        Assert.Contains(crew.Members, m => m.Id == "n1");
        Assert.Contains(crew.Members, m => m.Id == "n2");
    }

    [Fact]
    public void AddPlayer_replaces_most_recent_npc_even_when_players_were_added_after()
    {
        var crew = new Crew(3);
        crew.TryAddNpc(new NpcMember("n1"));
        crew.TryAddNpc(new NpcMember("n2"));
        crew.AddPlayer(new PlayerMember("p1"));

        Assert.Equal(JoinResult.ReplacedNpc, crew.AddPlayer(new PlayerMember("p2")));

        Assert.DoesNotContain(crew.Members, m => m.Id == "n2");
        Assert.Contains(crew.Members, m => m.Id == "n1");
        Assert.Equal(3, crew.Members.Count);
    }

    [Fact]
    public void AddPlayer_on_full_crew_of_players_returns_full()
    {
        var crew = new Crew(12);
        for (var i = 0; i < 12; i++)
            Assert.Equal(JoinResult.Joined, crew.AddPlayer(new PlayerMember("p" + i)));

        Assert.Equal(JoinResult.Full, crew.AddPlayer(new PlayerMember("p12")));
        Assert.Equal(12, crew.Members.Count);
        Assert.DoesNotContain(crew.Members, m => m.Id == "p12");
    }

    [Fact]
    public void AddPlayer_duplicate_id_throws()
    {
        var crew = new Crew(3);
        crew.AddPlayer(new PlayerMember("p1"));

        Assert.Throws<ArgumentException>(() => crew.AddPlayer(new PlayerMember("p1")));
        Assert.Single(crew.Members);
    }

    [Fact]
    public void TryAddNpc_adds_until_full_then_returns_false()
    {
        var crew = new Crew(2);

        Assert.True(crew.TryAddNpc(new NpcMember("n1")));
        Assert.True(crew.TryAddNpc(new NpcMember("n2")));
        Assert.False(crew.TryAddNpc(new NpcMember("n3")));
        Assert.Equal(2, crew.Members.Count);
    }

    [Fact]
    public void TryAddNpc_duplicate_id_returns_false()
    {
        var crew = new Crew(3);
        crew.TryAddNpc(new NpcMember("n1"));

        Assert.False(crew.TryAddNpc(new NpcMember("n1")));
        Assert.Single(crew.Members);
    }

    [Fact]
    public void Null_arguments_throw()
    {
        var crew = new Crew(2);
        Assert.Throws<ArgumentNullException>(() => crew.TryAddNpc(null!));
        Assert.Throws<ArgumentNullException>(() => crew.AddPlayer(null!));
    }
}
