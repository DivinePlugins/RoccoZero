using Divine.Core.Entities.Abilities.Components;
using Divine.Core.Entities.Abilities.Items.Bases;
using Divine.Core.Entities.Metadata;
using Divine.Entity.Entities.Abilities.Components;
using Divine.Entity.Entities.Abilities.Items;
using Divine.Entity.Entities.Units.Components;

namespace Divine.Core.Entities.Abilities.Items
{
    [Item(AbilityId.item_dust)]
    public sealed class DustOfAppearance : ActiveItem, IAreaOfEffectAbility, IHasTargetModifier
    {
        public DustOfAppearance(Item item)
            : base(item)
        {
        }

        public override UnitState AppliesUnitState { get; } = UnitState.ProvidesVision;

        public float Radius
        {
            get
            {
                return GetAbilitySpecialData("radius");
            }
        }

        public string TargetModifierName { get; } = "modifier_item_dustofappearance";
    }
}