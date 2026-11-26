using System;

namespace Legado.Core.Combat
{
    /// <summary>Reglas de muerte, saqueo y cadáver.</summary>
    public static class DeathRules
    {
        /// <summary>Tiempo que persiste un cadáver recuperable.</summary>
        public static readonly TimeSpan PveCorpseLifetime = TimeSpan.FromMinutes(30);

        /// <summary>
        /// Determina las consecuencias de una muerte según su contexto.
        /// El cadáver es saqueable por cualquiera solo en zona PvP y si existe (un naufragio no deja cadáver).
        /// </summary>
        public static DeathOutcome Resolve(DeathContext ctx)
        {
            if (ctx == null) throw new ArgumentNullException(nameof(ctx));

            bool shipwreck = ctx.Cause == DeathCause.Shipwreck;
            bool losesFruits = shipwreck || (ctx.Cause == DeathCause.Player && ctx.VictimOnline);

            return new DeathOutcome(
                LosesCarriedItems: true,
                LosesFruits: losesFruits,
                // Sin cadáver (naufragio) no hay nada que saquear.
                LootableByAnyone: ctx.InPvpZone && !shipwreck,
                KillerLoots: ctx.Cause == DeathCause.Player,
                // En un naufragio no hay cadáver recuperable.
                CorpseLifetime: shipwreck ? (TimeSpan?)null : PveCorpseLifetime);
        }
    }
}
