using Divine.Core.Entities.Abilities.Components;
using Divine.Core.Entities.Abilities.Items.Bases;
using Divine.Core.Entities.Metadata;
using Divine.Core.Extensions;
using Divine.Entity.Entities.Abilities.Components;
using Divine.Entity.Entities.Abilities.Items;

namespace Divine.Core.Entities.Abilities.Items
{
    [Item(AbilityId.item_mjollnir)]
    public sealed class Mjollnir : RangedItem, IHasTargetModifier, IAreaOfEffectAbility
    {
        public Mjollnir(Item item)
            : base(item)
        {
        }

        public float Radius
        {
            get
            {
                return GetAbilitySpecialData("static_radius");
            }
        }

        public string TargetModifierName { get; } = "modifier_item_mjollnir_static";

        public override bool CanHit(CUnit target)
        {
            return Owner.Distance2D(target) < Radius;
        }
    }
}