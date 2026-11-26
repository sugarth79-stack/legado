using System;
using Legado.Core.World;

namespace Legado.Core.Fruits
{
    public static class RarityTable
    {
        public static Rarity Roll(Zone zone, IRandomSource rng)
        {
            if (rng is null) throw new ArgumentNullException(nameof(rng));

            var percent = rng.NextPercent();
            if (percent < 0 || percent > 99)
                throw new ArgumentOutOfRangeException(nameof(rng), percent, "NextPercent must return a value between 0 and 99.");

            var (low, high, highFromPercent) = Entry(zone);
            return percent >= highFromPercent ? high : low;
        }

        private static (Rarity low, Rarity high, int highFromPercent) Entry(Zone zone)
        {
            switch (zone)
            {
                case Zone.FactionSea: return (Rarity.Common, Rarity.Common, 100);
                case Zone.Gullet: return (Rarity.Common, Rarity.Uncommon, 80);
                case Zone.WildWaters: return (Rarity.Uncommon, Rarity.Rare, 80);
                case Zone.DrownedKingsSea: return (Rarity.Rare, Rarity.Mythic, 80);
                default: throw new ArgumentOutOfRangeException(nameof(zone), zone, null);
            }
        }
    }
}
