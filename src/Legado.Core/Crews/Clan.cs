using System;
using System.Collections.Generic;

namespace Legado.Core.Crews
{
    /// <summary>Clan con un máximo de jugadores, sin ids repetidos.</summary>
    public sealed class Clan
    {
        public const int MaxMembers = 60;

        private readonly HashSet<string> _members = new HashSet<string>();

        public int Count => _members.Count;

        /// <summary>Añade un jugador; false si el clan está lleno o el id ya está.</summary>
        public bool TryAdd(string playerId)
        {
            if (playerId is null) throw new ArgumentNullException(nameof(playerId));
            if (_members.Count >= MaxMembers) return false;
            return _members.Add(playerId);
        }
    }
}
