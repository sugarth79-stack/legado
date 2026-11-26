using System;
using System.Collections.Generic;

namespace Legado.Core.Professions
{
    public static class ProfessionRules
    {
        /// <summary>Máximo de oficios por personaje.</summary>
        public const int MaxPerCharacter = 3;

        /// <summary>Fracción de la ganancia activa que se obtiene estando desconectado.</summary>
        public const double OfflineGainFactor = 0.001;

        /// <summary>
        /// Una combinación es válida si tiene como mucho <see cref="MaxPerCharacter"/> oficios y ninguno repetido.
        /// Una colección vacía es válida. Un oficio congelado sigue ocupando su hueco.
        /// </summary>
        public static bool IsValidCombination(IReadOnlyCollection<Profession> professions)
        {
            if (professions == null) throw new ArgumentNullException(nameof(professions));
            if (professions.Count > MaxPerCharacter) return false;

            var seen = new HashSet<Profession>();
            foreach (var p in professions)
            {
                if (!seen.Add(p)) return false;
            }
            return true;
        }

        /// <summary>
        /// Buceador está congelado si el personaje ha comido alguna fruta; el resto siempre está activo.
        /// El nivel del oficio congelado no se modifica.
        /// </summary>
        public static bool IsActive(Profession profession, int fruitsEaten)
        {
            if (fruitsEaten < 0)
                throw new ArgumentOutOfRangeException(nameof(fruitsEaten), fruitsEaten, "Las frutas comidas no pueden ser negativas.");
            return !(profession == Profession.Diver && fruitsEaten > 0);
        }

        /// <summary>Un oficio solo gana habilidad mientras está activo.</summary>
        public static bool CanGainSkill(Profession profession, int fruitsEaten)
            => IsActive(profession, fruitsEaten);

        /// <summary>Ganancia obtenida estando desconectado a partir de la ganancia activa.</summary>
        public static double OfflineGain(double activeGain)
        {
            if (double.IsNaN(activeGain) || double.IsInfinity(activeGain) || activeGain < 0)
                throw new ArgumentOutOfRangeException(nameof(activeGain), activeGain, "La ganancia activa debe ser un número finito no negativo.");
            return activeGain * OfflineGainFactor;
        }
    }
}
