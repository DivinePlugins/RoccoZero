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

        public Attribute ActiveAttribute
        {
            get
            {
                return Base.ActiveAttribute;
            }
        }

        public bool SwitchAttribute(Attribute attribute)
        {
            if (!CanBeCasted)
            {
                return false;
            }

            var result = false;
            var activeAttribute = ActiveAttribute;

            switch (attribute)
            {
                case Attribute.Strength:
                    if (activeAttribute == Attribute.Intelligence)
                    {
                        result = UseAbility() && UseAbility();
                    }
                    else if (activeAttribute == Attribute.Agility)
                    {
                        result = UseAbility();
                    }
                    break;
                case Attribute.Intelligence:
                    if (activeAttribute == Attribute.Agility)
                    {
                        result = UseAbility() && UseAbility();
                    }
                    else if (activeAttribute == Attribute.Strength)
                    {
                        result = UseAbility();
                    }
                    break;
                case Attribute.Agility:
                    if (activeAttribute == Attribute.Strength)
                    {
                        result = UseAbility() && UseAbility();
                    }
                    else if (activeAttribute == Attribute.Intelligence)
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