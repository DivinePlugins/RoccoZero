using Divine.Core.Entities.Abilities.Items.Bases;
using Divine.Core.Entities.Metadata;
using Divine.Entity.Entities.Abilities.Components;
using Divine.Entity.Entities.Abilities.Items;
using Divine.Entity.Entities.Units.Heroes.Components;
using Divine.Game;

namespace Divine.Core.Entities.Abilities.Items
{
    [Item(AbilityId.item_power_treads)]
    public sealed class PowerTreads : ActiveItem
    {
        public PowerTreads(Item item)
            : base(item)
        {
            Base = item as Entity.Entities.Abilities.Items.PowerTreads;
        }

        public new Entity.Entities.Abilities.Items.PowerTreads Base { get; }

        public HeroAttribute ActiveAttribute
        {
            get
            {
                return Base.ActiveAttribute;
            }
        }

        public bool SwitchAttribute(HeroAttribute attribute)
        {
            if (!CanBeCasted)
            {
                return false;
            }

            var result = false;
            var activeAttribute = ActiveAttribute;

            switch (attribute)
            {
                case HeroAttribute.Strength:
                    if (activeAttribute == HeroAttribute.Intelligence)
                    {
                        result = UseAbility() && UseAbility();
                    }
                    else if (activeAttribute == HeroAttribute.Agility)
                    {
                        result = UseAbility();
                    }
                    break;
                case HeroAttribute.Intelligence:
                    if (activeAttribute == HeroAttribute.Agility)
                    {
                        result = UseAbility() && UseAbility();
                    }
                    else if (activeAttribute == HeroAttribute.Strength)
                    {
                        result = UseAbility();
                    }
                    break;
                case HeroAttribute.Agility:
                    if (activeAttribute == HeroAttribute.Strength)
                    {
                        result = UseAbility() && UseAbility();
                    }
                    else if (activeAttribute == HeroAttribute.Intelligence)
                    {
                        result = UseAbility();
                    }
                    break;
            }

            if (result)
            {
                LastCastAttempt = GameManager.RawGameTime;
            }

            return result;
        }
    }
}
