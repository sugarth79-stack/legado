using System;

namespace Legado.Core.Dungeons
{
    /// <summary>Calendario de reinicios: el diario y el semanal ocurren a las 04:00 UTC.</summary>
    public static class Resets
    {
        /// <summary>Día de la semana del reinicio semanal.</summary>
        public const DayOfWeek WeeklyDay = DayOfWeek.Wednesday;

        /// <summary>Hora UTC de los reinicios diario y semanal.</summary>
        public static readonly TimeSpan TimeUtc = TimeSpan.FromHours(4);
    }
}
