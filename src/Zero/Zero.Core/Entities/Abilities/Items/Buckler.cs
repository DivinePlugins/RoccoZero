using Divine.Core.Entities.Abilities.Components;
using Divine.Core.Entities.Abilities.Items.Bases;
using Divine.Core.Entities.Metadata;
using Divine.Entity.Entities.Abilities.Components;
using Divine.Entity.Entities.Abilities.Items;

namespace Divine.Core.Entities.Abilities.Items
{
    [Item(AbilityId.item_buckler)]
    public sealed class Buckler : ActiveItem, IAreaOfEffectAbility, IHasModifier
    {
        public Buckler(Item item)
            : base(item)
        {
        }

        public string ModifierName { get; } = "modifier_item_buckler_effect";

        public float Radius
        {
            get
            {
                return GetAbilitySpecialData("bonus_aoe_radius");
            }
        }
    }
}