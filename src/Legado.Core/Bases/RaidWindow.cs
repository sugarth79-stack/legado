using System;

namespace Legado.Core.Bases
{
    /// <summary>
    /// Ventana diaria de asalto de 4 horas, definida por su hora de inicio en la zona horaria del clan.
    /// </summary>
    public sealed class RaidWindow
    {
        /// <summary>Duración de la ventana.</summary>
        public static readonly TimeSpan Duration = TimeSpan.FromHours(4);

        private readonly TimeSpan _start;
        private readonly TimeZoneInfo _clanTimeZone;

        /// <param name="localStart">Hora de inicio local, entre 00:00 inclusive y 24:00 exclusive.</param>
        /// <param name="clanTimeZone">Zona horaria del clan.</param>
        public RaidWindow(TimeSpan localStart, TimeZoneInfo clanTimeZone)
        {
            if (clanTimeZone == null) throw new ArgumentNullException(nameof(clanTimeZone));
            if (localStart < TimeSpan.Zero || localStart >= TimeSpan.FromHours(24))
                throw new ArgumentOutOfRangeException(nameof(localStart), "La hora de inicio debe estar entre 00:00 y 24:00 (exclusive).");

            _start = localStart;
            _clanTimeZone = clanTimeZone;
        }

        /// <summary>
        /// Indica si la ventana está abierta en el instante dado. La ventana dura siempre exactamente
        /// 4 horas reales y empieza a la hora local del clan de cada día natural local.
        /// Si esa hora local no existe (adelanto de reloj), empieza en la primera hora válida posterior;
        /// si es ambigua (retraso de reloj), se usa la primera aparición.
        /// </summary>
        public bool IsOpen(DateTimeOffset now)
        {
            var localDate = TimeZoneInfo.ConvertTime(now, _clanTimeZone).Date;
            return IsInWindowStartingOn(localDate, now) || IsInWindowStartingOn(localDate.AddDays(-1), now);
        }

        private bool IsInWindowStartingOn(DateTime localDate, DateTimeOffset now)
        {
            var start = StartInstant(localDate);
            return start <= now && now < start + Duration;
        }

        private DateTimeOffset StartInstant(DateTime localDate)
        {
            var local = DateTime.SpecifyKind(localDate.Date + _start, DateTimeKind.Unspecified);

            if (_clanTimeZone.IsInvalidTime(local))
            {
                // Hueco de adelanto de reloj: avanzar de minuto en minuto hasta la primera hora válida.
                local = new DateTime(local.Year, local.Month, local.Day, local.Hour, local.Minute, 0, DateTimeKind.Unspecified);
                do
                {
                    local = local.AddMinutes(1);
                } while (_clanTimeZone.IsInvalidTime(local));
            }

            TimeSpan offset;
            if (_clanTimeZone.IsAmbiguousTime(local))
            {
                // Solape de retraso de reloj: primera aparición = mayor desfase UTC.
                offset = TimeSpan.MinValue;
                foreach (var o in _clanTimeZone.GetAmbiguousTimeOffsets(local))
                    if (o > offset) offset = o;
            }
            else
            {
                offset = _clanTimeZone.GetUtcOffset(local);
            }

            return new DateTimeOffset(local, offset);
        }
    }
}
