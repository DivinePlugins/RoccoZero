using Divine.Core.Entities.Abilities.Items.Bases;
using Divine.Core.Entities.Metadata;
using Divine.Core.Extensions;
using Divine.Entity.Entities.Abilities.Components;
using Divine.Entity.Entities.Abilities.Items;

namespace Divine.Core.Entities.Abilities.Items
{
    [Item(AbilityId.item_blink)]
    public sealed class BlinkDagger : RangedItem
    {
        public BlinkDagger(Item item)
            : base(item)
        {
        }

        public override bool CanBeCasted
        {
            get
            {
                return base.CanBeCasted && !Owner.IsRooted();
            }
        }

        protected override float BaseCastRange
        {
            get
            {
                return GetAbilitySpecialData("blink_range");
            }
        }
    }
}