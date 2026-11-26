namespace Legado.Core.Fruits
{
    public interface IRandomSource
    {
        /// <summary>Returns an integer percentage in the range 0-99 inclusive.</summary>
        int NextPercent();
    }
}
