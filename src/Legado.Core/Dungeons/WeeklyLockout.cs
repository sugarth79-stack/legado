using System;
using System.Collections.Generic;

namespace Legado.Core.Dungeons
{
    /// <summary>
    /// Bloqueo semanal por mazmorra (no por dificultad): completar una mazmorra, en cualquier
    /// dificultad, la bloquea hasta el siguiente reinicio semanal.
    /// </summary>
    public sealed class WeeklyLockout
    {
        private readonly Dictionary<string, DateTimeOffset> _lastCompletion = new Dictionary<string, DateTimeOffset>();

        /// <summary>Registra una finalización; se conserva únicamente la más reciente.</summary>
        public void RecordCompletion(string dungeonId, DateTimeOffset at)
        {
            ValidateId(dungeonId);
            if (!_lastCompletion.TryGetValue(dungeonId, out var previous) || at > previous)
                _lastCompletion[dungeonId] = at;
        }

        /// <summary>Indica si la mazmorra sigue bloqueada en <paramref name="now"/>.</summary>
        public bool IsLocked(string dungeonId, DateTimeOffset now)
        {
            ValidateId(dungeonId);
            if (!_lastCompletion.TryGetValue(dungeonId, out var completedAt)) return false;
            return now < NextReset(completedAt);
        }

        /// <summary>Primer reinicio semanal estrictamente posterior al instante dado.</summary>
        private static DateTimeOffset NextReset(DateTimeOffset after)
        {
            DateTime utc = after.UtcDateTime;
            DateTime candidate = utc.Date + Resets.TimeUtc;
            candidate = candidate.AddDays(((int)Resets.WeeklyDay - (int)candidate.DayOfWeek + 7) % 7);
            if (candidate <= utc) candidate = candidate.AddDays(7);
            return new DateTimeOffset(DateTime.SpecifyKind(candidate, DateTimeKind.Utc));
        }

        private static void ValidateId(string dungeonId)
        {
            if (string.IsNullOrEmpty(dungeonId))
                throw new ArgumentException("El identificador de la mazmorra no puede ser nulo ni vacío.", nameof(dungeonId));
        }
    }
}
