using System;
using System.Collections.Generic;

namespace Legado.Core.Crews
{
    /// <summary>Resultado de intentar unir a un jugador a una tripulación.</summary>
    public enum JoinResult
    {
        /// <summary>Había hueco libre y el jugador entró.</summary>
        Joined,
        /// <summary>La tripulación estaba llena y el jugador ocupó el lugar del último NPC añadido.</summary>
        ReplacedNpc,
        /// <summary>La tripulación estaba llena de jugadores; no entró.</summary>
        Full
    }

    /// <summary>Tripulación con un número limitado de plazas.</summary>
    public sealed class Crew
    {
        public const int MaxCapacity = 12;

        private readonly int _capacity;
        private readonly List<CrewMember> _members = new List<CrewMember>();

        public Crew(int capacity)
        {
            if (capacity < 1 || capacity > MaxCapacity)
                throw new ArgumentOutOfRangeException(nameof(capacity), capacity,
                    "La capacidad debe estar entre 1 y " + MaxCapacity + ".");
            _capacity = capacity;
        }

        public IReadOnlyList<CrewMember> Members => _members;

        /// <summary>Añade un NPC si hay hueco y su id no está ya en la tripulación.</summary>
        public bool TryAddNpc(NpcMember npc)
        {
            if (npc is null) throw new ArgumentNullException(nameof(npc));
            if (_members.Count >= _capacity || Contains(npc.Id)) return false;
            _members.Add(npc);
            return true;
        }

        /// <summary>
        /// Añade un jugador. Si la tripulación está llena, sustituye al último NPC añadido;
        /// si no hay NPC que sustituir, devuelve <see cref="JoinResult.Full"/>.
        /// </summary>
        public JoinResult AddPlayer(PlayerMember player)
        {
            if (player is null) throw new ArgumentNullException(nameof(player));
            if (Contains(player.Id))
                throw new ArgumentException("El id ya pertenece a la tripulación.", nameof(player));

            if (_members.Count < _capacity)
            {
                _members.Add(player);
                return JoinResult.Joined;
            }

            for (var i = _members.Count - 1; i >= 0; i--)
            {
                if (_members[i] is NpcMember)
                {
                    _members[i] = player;
                    return JoinResult.ReplacedNpc;
                }
            }
            return JoinResult.Full;
        }

        private bool Contains(string id) => _members.Exists(m => m.Id == id);
    }
}
