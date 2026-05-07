namespace Divine.Core.Entities.Abilities.Components
{
    public interface IHasProcChance
    {
        /// <summary>
        ///     Gets a value indicating whether the <see cref="ProcChance" /> is not the actual proc chance. To get the actual
        ///     chance see <see cref="Utils.GetPseudoChance" />.
        /// </summary>
        bool IsPseudoChance { get; }

        /// <summary>
        ///     Gets the proc chance with from 0 to 1.
        /// </summary>
        float ProcChance { get; }
    }
}