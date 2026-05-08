using Divine.Entity.Entities.Abilities.Items;
using Divine.Game;

namespace Divine.Core.Entities.Abilities.Items.Bases
{
    public abstract class ToggleItem : ActiveItem
    {
        protected ToggleItem(Item item)
            : base(item)
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
                if (!CanBeCasted)
                {
                    Logger.LogDebug($"blocked {this}");
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
                        result = Base.CastToggle();
                    }
                }

                if (result)
                {
                    LastCastAttempt = GameManager.RawGameTime;
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
