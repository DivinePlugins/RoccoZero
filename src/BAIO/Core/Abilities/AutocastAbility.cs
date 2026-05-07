namespace Ensage.SDK.Abilities
{
    using Divine.Entity.Entities.Abilities;

    public abstract class AutocastAbility : RangedAbility
    {
        protected AutocastAbility(Ability ability)
            : base(ability)
        {
        }

        public virtual bool Enabled
        {
            get
            {
                return this.Ability.IsAutoCastEnabled;
            }

            set
            {
                if (value)
                {
                    if (!this.Enabled)
                    {
                        this.Ability.CastToggleAutocast();
                    }
                }
                else
                {
                    if (this.Enabled)
                    {
                        this.Ability.CastToggleAutocast();
                    }
                }
            }
        }
    }
}