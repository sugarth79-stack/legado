using Legado.Core.Dungeons;

namespace Legado.Core.Tests.Dungeons;

public class WeeklyLockoutTests
{
    private static DateTimeOffset Utc(int month, int day, int hour, int minute) =>
        new DateTimeOffset(2026, month, day, hour, minute, 0, TimeSpan.Zero);

    [Fact]
    public void Resets_AreWednesdayAt0400Utc()
    {
        Assert.Equal(DayOfWeek.Wednesday, Resets.WeeklyDay);
        Assert.Equal(TimeSpan.FromHours(4), Resets.TimeUtc);
    }

    [Fact]
    public void NeverCompleted_IsNotLocked() =>
        Assert.False(new WeeklyLockout().IsLocked("d1", Utc(12, 2, 3, 0)));

    [Fact]
    public void CompletedMonday_LockedUntilWednesday0400()
    {
        var l = new WeeklyLockout();
        l.RecordCompletion("d1", Utc(11, 30, 20, 0));
        Assert.True(l.IsLocked("d1", Utc(12, 2, 3, 59)));
        Assert.False(l.IsLocked("d1", Utc(12, 2, 4, 0)));
    }

    [Fact]
    public void CompletedExactlyAtReset_LockedUntilNextWednesday()
    {
        var l = new WeeklyLockout();
        l.RecordCompletion("d1", Utc(12, 2, 4, 0));
        Assert.True(l.IsLocked("d1", Utc(12, 2, 4, 0)));
        Assert.True(l.IsLocked("d1", Utc(12, 9, 3, 59)));
        Assert.False(l.IsLocked("d1", Utc(12, 9, 4, 0)));
    }

    [Fact]
    public void CompletedWednesdayBeforeReset_LockedOnlyUntilSameDay0400()
    {
        var l = new WeeklyLockout();
        l.RecordCompletion("d1", Utc(12, 2, 3, 0));
        Assert.False(l.IsLocked("d1", Utc(12, 2, 4, 0)));
    }

    [Fact]
    public void Lockout_IsPerDungeon_NotPerDifficulty()
    {
        // RecordCompletion e IsLocked no reciben dificultad: completar en Basic bloquea también Max.
        var l = new WeeklyLockout();
        l.RecordCompletion("d1", Utc(11, 30, 20, 0));
        Assert.True(l.IsLocked("d1", Utc(12, 1, 12, 0)));
        Assert.False(l.IsLocked("d2", Utc(12, 1, 12, 0)));
    }

    [Fact]
    public void LaterCompletion_ReplacesEarlier_EarlierDoesNot()
    {
        var l = new WeeklyLockout();
        l.RecordCompletion("d1", Utc(11, 30, 20, 0));
        l.RecordCompletion("d1", Utc(12, 2, 5, 0));
        Assert.True(l.IsLocked("d1", Utc(12, 5, 0, 0)));
        l.RecordCompletion("d1", Utc(11, 20, 0, 0));
        Assert.True(l.IsLocked("d1", Utc(12, 5, 0, 0)));
    }

    [Fact]
    public void OffsetInstants_AreNormalizedToUtc()
    {
        var l = new WeeklyLockout();
        // 2026-12-02T05:30+01:00 == 04:30Z, posterior al reinicio.
        l.RecordCompletion("d1", new DateTimeOffset(2026, 12, 2, 5, 30, 0, TimeSpan.FromHours(1)));
        Assert.True(l.IsLocked("d1", Utc(12, 9, 3, 59)));
        Assert.False(l.IsLocked("d1", Utc(12, 9, 4, 0)));
    }

    [Fact]
    public void NullOrEmptyDungeonId_Throws()
    {
        var l = new WeeklyLockout();
        Assert.Throws<ArgumentException>(() => l.RecordCompletion(null!, Utc(12, 2, 4, 0)));
        Assert.Throws<ArgumentException>(() => l.RecordCompletion("", Utc(12, 2, 4, 0)));
        Assert.Throws<ArgumentException>(() => l.IsLocked(null!, Utc(12, 2, 4, 0)));
        Assert.Throws<ArgumentException>(() => l.IsLocked("", Utc(12, 2, 4, 0)));
    }
}
