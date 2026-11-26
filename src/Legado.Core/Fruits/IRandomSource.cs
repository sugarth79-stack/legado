namespace Legado.Core.Fruits
{
    public interface IRandomSource
    {
        /// <summary>Devuelve un porcentaje entero en el rango 0-99, ambos inclusive.</summary>
        int NextPercent();
    }
}
