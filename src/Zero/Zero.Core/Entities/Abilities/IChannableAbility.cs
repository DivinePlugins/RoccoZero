namespace Divine.Core.Entities.Abilities
{
    public interface IChannableAbility
    {
        /// <summary>
        ///     Gets the maximum duration of the channeling ability.
        /// </summary>
        float ChannelDuration { get; }

        /// <summary>
        ///     Gets a value indicating whether the ability is currently channeled.
        /// </summary>
        bool IsChanneling { get; }

        /// <summary>
        ///     Gets the remaining duration of the ability, while it's being channeled.
        /// </summary>
        float RemainingDuration { get; }
    }
}