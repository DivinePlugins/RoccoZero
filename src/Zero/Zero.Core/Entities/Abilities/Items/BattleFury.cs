using Divine.Core.Entities.Abilities.Items.Bases;
using Divine.Core.Entities.Metadata;
using Divine.Entity.Entities.Abilities.Components;
using Divine.Entity.Entities.Abilities.Items;

namespace Divine.Core.Entities.Abilities.Items
{
    [Item(AbilityId.item_bfury)]
    public sealed class BattleFury : RangedItem //TODO DamageIncrease
    {
        public BattleFury(Item item)
            : base(item)
        {
        }

        public float CastRangeOnWard
        {
            get
            {
                return GetAbilitySpecialData("cast_range_ward");
            }
        }
    }
}