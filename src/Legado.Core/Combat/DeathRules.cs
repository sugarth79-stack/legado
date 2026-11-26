using System;

namespace Legado.Core.Combat
{
    /// <summary>Reglas de muerte, saqueo y cadáver.</summary>
    public static class DeathRules
    {
        /// <summary>Tiempo que persiste un cadáver recuperable.</summary>
        public static readonly TimeSpan PveCorpseLifetime = TimeSpan.FromMinutes(30);

        /// <summary>Determina las consecuencias de una muerte según su contexto.</summary>
        public static DeathOutcome Resolve(DeathContext ctx)
        {
            if (ctx == null) throw new ArgumentNullException(nameof(ctx));

            bool shipwreck = ctx.Cause == DeathCause.Shipwreck;
            bool losesFruits = shipwreck || (ctx.Cause == DeathCause.Player && ctx.VictimOnline);

            return new DeathOutcome(
                LosesCarriedItems: true,
                LosesFruits: losesFruits,
                LootableByAnyone: ctx.InPvpZone,
                KillerLoots: ctx.Cause == DeathCause.Player,
                // En un naufragio no hay cadáver recuperable.
                CorpseLifetime: shipwreck ? (TimeSpan?)null : PveCorpseLifetime);
        }
    }
}
