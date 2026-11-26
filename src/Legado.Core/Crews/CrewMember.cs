namespace Legado.Core.Crews
{
    /// <summary>Miembro de una tripulación.</summary>
    public abstract record CrewMember(string Id);

    /// <summary>Miembro controlado por un jugador.</summary>
    public record PlayerMember(string Id) : CrewMember(Id);

    /// <summary>Miembro controlado por el juego (NPC).</summary>
    public record NpcMember(string Id) : CrewMember(Id);
}
