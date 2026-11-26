using System;

namespace Legado.Core.Progression
{
    public static class FruitSlots
    {
        private static readonly int[] Thresholds = { 10, 30, 50, 70, 80 };

        public static int ForLevel(int level)
        {
            if (level < 1 || level > GameLimits.MaxLevel)
                throw new ArgumentOutOfRangeException(nameof(level), level, $"El nivel debe estar entre 1 y {GameLimits.MaxLevel}.");

            var slots = 0;
            foreach (var threshold in Thresholds)
            {
                if (level >= threshold) slots++;
            }
            return slots;
        }
    }
}
