using Divine.Core.Entities.Abilities.Components;
using Divine.Core.Entities.Abilities.Items.Bases;
using Divine.Core.Entities.Metadata;
using Divine.Entity.Entities.Abilities.Components;
using Divine.Entity.Entities.Abilities.Items;
using Divine.Entity.Entities.Units.Components;

namespace Divine.Core.Entities.Abilities.Items
{
    [Item(AbilityId.item_sheepstick)]
    public sealed class ScytheOfVyse : RangedItem, IHasTargetModifier
    {
        public ScytheOfVyse(Item item)
            : base(item)
        {
        }

        public override UnitState AppliesUnitState { get; } = UnitState.Hexed;

        public string TargetModifierName { get; } = "modifier_sheepstick_debuff";
    }
}