namespace Ensage.SDK.Abilities
{
    using Divine.Entity.Entities.Abilities;
    using Divine.Game;
    using Divine.Zero.Log;

    public abstract class ToggleAbility : ActiveAbility
    {
        protected ToggleAbility(Ability ability)
            : base(ability)
        {
        }

        public virtual bool Enabled
        {
            get
            {
                return this.Ability.IsToggled;
            }

            set
            {
                if (!this.CanBeCasted)
                {
                    //LogManager.Debug($"blocked {this}");
                    return;
                }

                var result = false;
                if (value)
                {
                    if (!this.Enabled)
                    {
                        result = this.Ability.CastToggle();
                    }
                }
                else
                {
                    if (this.Enabled)
                    {
                        result = this.Ability.CastToggle();
                    }
                }

                if (result)
                {
                    this.LastCastAttempt = GameManager.RawGameTime;
                }
            }
        }

        /// <summary>
        ///     Toggles the ability to the opposite of its current state.
        /// </summary>
        /// <returns>Will always return true.</returns>
        public override bool Cast()
        {
            this.Enabled = !this.Enabled;
            return true; // TODO: return if setter was successful?
        }
    }
}