namespace Legado.Core.Combat
{
    /// <summary>Causa de la muerte de un personaje.</summary>
    public enum DeathCause { Monster, Player, Shipwreck }

    /// <summary>Circunstancias en las que muere un personaje.</summary>
    public record DeathContext(DeathCause Cause, bool VictimOnline, bool InPvpZone);
}
