using System;

namespace Legado.Core.Renown
{
    /// <summary>Rango de renombre según la posición en el servidor.</summary>
    public enum RenownRank
    {
        Gray = 1,
        Green,
        Blue,
        Purple,
        Gold
    }

    public static class RenownRules
    {
        /// <summary>Renombre tras morir: resta el 20 % y redondea hacia abajo.</summary>
        public static long AfterDeath(long renown)
        {
            if (renown < 0)
                throw new ArgumentOutOfRangeException(nameof(renown), renown, "El renombre no puede ser negativo.");

            return renown * 4 / 5;
        }

        /// <summary>
        /// Rango según la posición (1 = más renombre del servidor) y la población.
        /// Umbrales inclusivos sobre position / population: Oro 1 %, Púrpura 10 %, Azul 25 %, Verde 50 %.
        /// </summary>
        public static RenownRank Rank(int position, int population)
        {
            if (population < 1)
                throw new ArgumentOutOfRangeException(nameof(population), population, "La población debe ser al menos 1.");
            if (position < 1 || position > population)
                throw new ArgumentOutOfRangeException(nameof(position), position, "La posición debe estar entre 1 y la población.");

            // Aritmética entera para evitar errores de coma flotante en los límites.
            long pos = position;
            long pop = population;
            if (pos * 100 <= pop) return RenownRank.Gold;
            if (pos * 10 <= pop) return RenownRank.Purple;
            if (pos * 4 <= pop) return RenownRank.Blue;
            if (pos * 2 <= pop) return RenownRank.Green;
            return RenownRank.Gray;
        }
    }
}
