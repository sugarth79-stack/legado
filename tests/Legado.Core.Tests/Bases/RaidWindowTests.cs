using Legado.Core.Bases;

namespace Legado.Core.Tests.Bases;

public class RaidWindowTests
{
    private static readonly TimeZoneInfo Madrid = TimeZoneInfo.FindSystemTimeZoneById("Europe/Madrid");

    private static DateTimeOffset Winter(int day, int hour, int minute) =>
        new DateTimeOffset(2026, 1, day, hour, minute, 0, TimeSpan.FromHours(1));

    [Fact]
    public void Duration_IsFourHours() =>
        Assert.Equal(TimeSpan.FromHours(4), RaidWindow.Duration);

    [Fact]
    public void Winter_Start20_OpenAtStartAndJustBeforeEnd()
    {
        var w = new RaidWindow(TimeSpan.FromHours(20), Madrid);
        Assert.True(w.IsOpen(new DateTimeOffset(2026, 1, 15, 19, 0, 0, TimeSpan.Zero)));
        Assert.True(w.IsOpen(new DateTimeOffset(2026, 1, 15, 22, 59, 0, TimeSpan.Zero)));
    }

    [Fact]
    public void Winter_Start20_ClosedAtLocalMidnight()
    {
        var w = new RaidWindow(TimeSpan.FromHours(20), Madrid);
        Assert.False(w.IsOpen(new DateTimeOffset(2026, 1, 15, 23, 0, 0, TimeSpan.Zero)));
    }

    [Fact]
    public void Summer_Start20_FollowsLocalTime()
    {
        var w = new RaidWindow(TimeSpan.FromHours(20), Madrid);
        Assert.True(w.IsOpen(new DateTimeOffset(2026, 7, 15, 18, 0, 0, TimeSpan.Zero)));
        Assert.False(w.IsOpen(new DateTimeOffset(2026, 7, 15, 17, 59, 0, TimeSpan.Zero)));
    }

    [Fact]
    public void CrossingMidnight_Start22_OpenBeforeAndAfterMidnight()
    {
        var w = new RaidWindow(TimeSpan.FromHours(22), Madrid);
        Assert.True(w.IsOpen(Winter(15, 23, 30)));
        Assert.True(w.IsOpen(Winter(16, 1, 59)));
    }

    [Fact]
    public void CrossingMidnight_Start22_ClosedAtEndAndDuringDay()
    {
        var w = new RaidWindow(TimeSpan.FromHours(22), Madrid);
        Assert.False(w.IsOpen(Winter(16, 2, 0)));
        Assert.False(w.IsOpen(Winter(15, 21, 0)));
    }

    [Fact]
    public void Winter_Start10_NonCrossing_OpensAt10AndClosesAt14Local()
    {
        var w = new RaidWindow(TimeSpan.FromHours(10), Madrid);
        Assert.False(w.IsOpen(new DateTimeOffset(2026, 1, 15, 8, 59, 0, TimeSpan.Zero)));
        Assert.True(w.IsOpen(new DateTimeOffset(2026, 1, 15, 9, 0, 0, TimeSpan.Zero)));
        Assert.True(w.IsOpen(new DateTimeOffset(2026, 1, 15, 12, 59, 0, TimeSpan.Zero)));
        Assert.False(w.IsOpen(new DateTimeOffset(2026, 1, 15, 13, 0, 0, TimeSpan.Zero)));
    }

    // La ventana dura siempre 4 horas reales.
    // 2026-03-29: a las 01:00Z el reloj salta de 02:00 a 03:00. Inicio 01:00 local = 00:00Z.
    [Fact]
    public void SpringForward_Start01_WindowLastsFourRealHours()
    {
        var w = new RaidWindow(TimeSpan.FromHours(1), Madrid);
        Assert.False(w.IsOpen(new DateTimeOffset(2026, 3, 28, 23, 59, 0, TimeSpan.Zero)));
        Assert.True(w.IsOpen(new DateTimeOffset(2026, 3, 29, 0, 0, 0, TimeSpan.Zero)));
        Assert.True(w.IsOpen(new DateTimeOffset(2026, 3, 29, 3, 59, 0, TimeSpan.Zero)));
        Assert.False(w.IsOpen(new DateTimeOffset(2026, 3, 29, 4, 0, 0, TimeSpan.Zero)));
    }

    // 2026-10-25: a las 01:00Z el reloj retrocede de 03:00 a 02:00. Inicio 01:00 local = 23:00Z (día 24).
    [Fact]
    public void FallBack_Start01_WindowLastsFourRealHours()
    {
        var w = new RaidWindow(TimeSpan.FromHours(1), Madrid);
        Assert.False(w.IsOpen(new DateTimeOffset(2026, 10, 24, 22, 59, 0, TimeSpan.Zero)));
        Assert.True(w.IsOpen(new DateTimeOffset(2026, 10, 24, 23, 0, 0, TimeSpan.Zero)));
        Assert.True(w.IsOpen(new DateTimeOffset(2026, 10, 25, 2, 59, 0, TimeSpan.Zero)));
        Assert.False(w.IsOpen(new DateTimeOffset(2026, 10, 25, 3, 0, 0, TimeSpan.Zero)));
    }

    // Hora local inexistente (02:30 el 2026-03-29): empieza en la primera hora válida, 03:00 CEST = 01:00Z.
    [Fact]
    public void SpringForward_StartInGap_StartsAtFirstValidTime()
    {
        var w = new RaidWindow(new TimeSpan(2, 30, 0), Madrid);
        Assert.False(w.IsOpen(new DateTimeOffset(2026, 3, 29, 0, 59, 0, TimeSpan.Zero)));
        Assert.True(w.IsOpen(new DateTimeOffset(2026, 3, 29, 1, 0, 0, TimeSpan.Zero)));
        Assert.True(w.IsOpen(new DateTimeOffset(2026, 3, 29, 4, 59, 0, TimeSpan.Zero)));
        Assert.False(w.IsOpen(new DateTimeOffset(2026, 3, 29, 5, 0, 0, TimeSpan.Zero)));
    }

    // Hora local ambigua (02:30 el 2026-10-25): primera aparición = 02:30 CEST = 00:30Z.
    [Fact]
    public void FallBack_StartInOverlap_UsesFirstOccurrence()
    {
        var w = new RaidWindow(new TimeSpan(2, 30, 0), Madrid);
        Assert.False(w.IsOpen(new DateTimeOffset(2026, 10, 25, 0, 29, 0, TimeSpan.Zero)));
        Assert.True(w.IsOpen(new DateTimeOffset(2026, 10, 25, 0, 30, 0, TimeSpan.Zero)));
        Assert.True(w.IsOpen(new DateTimeOffset(2026, 10, 25, 4, 29, 0, TimeSpan.Zero)));
        Assert.False(w.IsOpen(new DateTimeOffset(2026, 10, 25, 4, 30, 0, TimeSpan.Zero)));
    }

    // Inicio 00:30 local el 2026-03-29 = 23:30Z del día 28; el intervalo cruza el adelanto de reloj.
    [Fact]
    public void SpringForward_Start0030_SpanCrossingTransition_LastsFourRealHours()
    {
        var w = new RaidWindow(new TimeSpan(0, 30, 0), Madrid);
        Assert.False(w.IsOpen(new DateTimeOffset(2026, 3, 28, 23, 29, 0, TimeSpan.Zero)));
        Assert.True(w.IsOpen(new DateTimeOffset(2026, 3, 28, 23, 30, 0, TimeSpan.Zero)));
        Assert.True(w.IsOpen(new DateTimeOffset(2026, 3, 29, 3, 29, 0, TimeSpan.Zero)));
        Assert.False(w.IsOpen(new DateTimeOffset(2026, 3, 29, 3, 30, 0, TimeSpan.Zero)));
    }

    // Inicio 02:00 local el 2026-10-25: primer instante del solape, primera aparición = 00:00Z.
    [Fact]
    public void FallBack_Start02_FirstInstantOfOverlap_UsesFirstOccurrence()
    {
        var w = new RaidWindow(TimeSpan.FromHours(2), Madrid);
        Assert.False(w.IsOpen(new DateTimeOffset(2026, 10, 24, 23, 59, 0, TimeSpan.Zero)));
        Assert.True(w.IsOpen(new DateTimeOffset(2026, 10, 25, 0, 0, 0, TimeSpan.Zero)));
        Assert.True(w.IsOpen(new DateTimeOffset(2026, 10, 25, 3, 59, 0, TimeSpan.Zero)));
        Assert.False(w.IsOpen(new DateTimeOffset(2026, 10, 25, 4, 0, 0, TimeSpan.Zero)));
    }

    // Zona sin cambio de hora y ventana que cruza la medianoche.
    [Fact]
    public void Utc_Start21_CrossingMidnight_LastsFourHours()
    {
        var w = new RaidWindow(TimeSpan.FromHours(21), TimeZoneInfo.Utc);
        Assert.True(w.IsOpen(new DateTimeOffset(2026, 6, 10, 21, 0, 0, TimeSpan.Zero)));
        Assert.True(w.IsOpen(new DateTimeOffset(2026, 6, 11, 0, 59, 0, TimeSpan.Zero)));
        Assert.False(w.IsOpen(new DateTimeOffset(2026, 6, 11, 1, 0, 0, TimeSpan.Zero)));
        Assert.False(w.IsOpen(new DateTimeOffset(2026, 6, 10, 20, 59, 0, TimeSpan.Zero)));
    }

    [Fact]
    public void Now_IsEvaluatedByInstantRegardlessOfOffset()
    {
        var w = new RaidWindow(TimeSpan.FromHours(20), Madrid);
        var utc = new DateTimeOffset(2026, 1, 15, 19, 0, 0, TimeSpan.Zero);
        var plus5 = utc.ToOffset(TimeSpan.FromHours(5));
        Assert.Equal(w.IsOpen(utc), w.IsOpen(plus5));
        Assert.True(w.IsOpen(plus5));
        var closedUtc = new DateTimeOffset(2026, 1, 15, 23, 0, 0, TimeSpan.Zero);
        Assert.False(w.IsOpen(closedUtc.ToOffset(TimeSpan.FromHours(5))));
    }

    [Fact]
    public void Constructor_Start24Hours_Throws() =>
        Assert.Throws<ArgumentOutOfRangeException>(() => new RaidWindow(TimeSpan.FromHours(24), Madrid));

    [Fact]
    public void Constructor_NegativeStart_Throws() =>
        Assert.Throws<ArgumentOutOfRangeException>(() => new RaidWindow(TimeSpan.FromMinutes(-1), Madrid));

    [Fact]
    public void Constructor_NullTimeZone_Throws() =>
        Assert.Throws<ArgumentNullException>(() => new RaidWindow(TimeSpan.FromHours(20), null!));
}
