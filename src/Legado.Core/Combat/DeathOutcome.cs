using System;

namespace Legado.Core.Combat
{
    /// <summary>Consecuencias de una muerte: pérdidas, saqueo y duración del cadáver.</summary>
    public record DeathOutcome(
        bool LosesCarriedItems,
        bool LosesFruits,
        bool LootableByAnyone,
        bool KillerLoots,
        TimeSpan? CorpseLifetime);
}
