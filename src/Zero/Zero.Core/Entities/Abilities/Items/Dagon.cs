using Divine.Core.Entities.Abilities.Items.Bases;
using Divine.Core.Entities.Metadata;
using Divine.Entity.Entities.Abilities.Components;
using Divine.Entity.Entities.Abilities.Items;

namespace Divine.Core.Entities.Abilities.Items
{
    [Item(AbilityId.item_dagon)]
    [Item(AbilityId.item_dagon_2)]
    [Item(AbilityId.item_dagon_3)]
    [Item(AbilityId.item_dagon_4)]
    [Item(AbilityId.item_dagon_5)]
    public sealed class Dagon : RangedItem
    {
        public Dagon(Item item)
            : base(item)
        {
        }

        public override DamageType DamageType
        {
            get
            {
                return DamageType.Magical;
            }
        }

        protected override float RawDamage
        {
            get
            {
                return GetAbilitySpecialData("damage");
            }
        }
    }
}