using Divine.Core.Entities.Abilities.Components;
using Divine.Core.Entities.Abilities.Items.Bases;
using Divine.Core.Entities.Metadata;
using Divine.Entity.Entities.Abilities.Components;
using Divine.Entity.Entities.Abilities.Items;
using Divine.Entity.Entities.Units.Components;

namespace Divine.Core.Entities.Abilities.Items
{
    [Item(AbilityId.item_abyssal_blade)]
    public sealed class AbyssalBlade : RangedItem, IHasTargetModifierTexture
    {
        public AbyssalBlade(Item item)
            : base(item)
        {
        }

        public override UnitState AppliesUnitState { get; } = UnitState.Stunned;

        public string[] TargetModifierTextureName { get; } = { "item_abyssal_blade" };
    }
}