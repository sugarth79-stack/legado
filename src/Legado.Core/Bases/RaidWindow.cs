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
        private readonly TimeSpan _end;
        private readonly TimeZoneInfo _clanTimeZone;

        /// <param name="localStart">Hora de inicio local, entre 00:00 inclusive y 24:00 exclusive.</param>
        /// <param name="clanTimeZone">Zona horaria del clan.</param>
        public RaidWindow(TimeSpan localStart, TimeZoneInfo clanTimeZone)
        {
            if (clanTimeZone == null) throw new ArgumentNullException(nameof(clanTimeZone));
            if (localStart < TimeSpan.Zero || localStart >= TimeSpan.FromHours(24))
                throw new ArgumentOutOfRangeException(nameof(localStart), "La hora de inicio debe estar entre 00:00 y 24:00 (exclusive).");

            _start = localStart;
            _end = TimeSpan.FromTicks((localStart + Duration).Ticks % TimeSpan.FromHours(24).Ticks);
            _clanTimeZone = clanTimeZone;
        }

        /// <summary>Indica si la ventana está abierta en el instante dado, evaluado en la hora local del clan.</summary>
        public bool IsOpen(DateTimeOffset now)
        {
            var t = TimeZoneInfo.ConvertTime(now, _clanTimeZone).TimeOfDay;
            return _start < _end
                ? t >= _start && t < _end
                : t >= _start || t < _end;
        }
    }
}
