namespace Divine.Core.Entities.Abilities.Components
{
    public interface IHasCritChance : IHasProcChance
    {
        /// <summary>
        ///     Gets the damage multiplier for the critical attack.
        /// </summary>
        float CritMultiplier { get; }
    }
}