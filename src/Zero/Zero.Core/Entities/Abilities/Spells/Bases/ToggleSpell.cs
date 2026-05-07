using Divine.Entity.Entities.Abilities;

namespace Divine.Core.Entities.Abilities.Spells.Bases
{
    public abstract class ToggleSpell : ActiveSpell
    {
        protected ToggleSpell(Ability ability)
            : base(ability)
        {
        }

        public virtual bool Enabled
        {
            get
            {
                return IsToggled;
            }

            set
            {
                if (CastSleeper.Sleeping)
                {
                    return;
                }

                if (!CanBeCasted)
                {
                    return;
                }

                var result = false;
                if (value)
                {
                    if (!Enabled)
                    {
                        result = ToggleAbility();
                    }
                }
                else
                {
                    if (Enabled)
                    {
                        result = ToggleAbility();
                    }
                }

                if (result)
                {
                    CastSleeper.Sleep(100); //TODO Re
                }
            }
        }

        public override bool UseAbility()
        {
            Enabled = !Enabled;
            return true; // TODO: return if setter was successful?
        }
    }
}