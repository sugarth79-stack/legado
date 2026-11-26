using System;

namespace Legado.Core.Dungeons
{
    /// <summary>Reglas de acceso y botín de las mazmorras.</summary>
    public static class DungeonRules
    {
        private const int MaxGroupSize = 6;

        /// <summary>
        /// Indica si un grupo puede entrar: entre 1 y 6 miembros (jugadores más NPC);
        /// en dificultad Max no se admiten NPC.
        /// </summary>
        public static bool CanEnter(Difficulty d, int players, int npcs)
        {
            if (players < 0) throw new ArgumentOutOfRangeException(nameof(players), "El número de jugadores no puede ser negativo.");
            if (npcs < 0) throw new ArgumentOutOfRangeException(nameof(npcs), "El número de NPC no puede ser negativo.");

            int total = players + npcs;
            if (total < 1 || total > MaxGroupSize) return false;
            return d != Difficulty.Max || npcs == 0;
        }

        /// <summary>Indica si la mazmorra puede soltar fruta: si es de historia o si la dificultad es Max.</summary>
        public static bool CanDropFruit(bool storyDungeon, Difficulty d) =>
            storyDungeon || d == Difficulty.Max;
    }
}
