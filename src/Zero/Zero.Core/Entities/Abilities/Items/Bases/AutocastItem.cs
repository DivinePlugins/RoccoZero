using Divine.Entity.Entities.Abilities.Items;

namespace Divine.Core.Entities.Abilities.Items.Bases
{
    public abstract class AutocastItem : RangedItem
    {
        protected AutocastItem(Item item)
            : base(item)
        {
        }

        public virtual bool Enabled
        {
            get
            {
                return IsAutoCastEnabled;
            }

            set
            {
                if (value)
                {
                    if (!Enabled)
                    {
                        ToggleAutocastAbility();
                    }
                }
                else
                {
                    if (Enabled)
                    {
                        ToggleAutocastAbility();
                    }
                }
            }
        }
    }
}