using Divine.Core.Entities.Abilities.Components;
using Divine.Core.Entities.Abilities.Items.Bases;
using Divine.Core.Entities.Metadata;
using Divine.Entity.Entities.Abilities.Components;
using Divine.Entity.Entities.Abilities.Items;
using Divine.Entity.Entities.Units.Components;

namespace Divine.Core.Entities.Abilities.Items
{
    [Item(AbilityId.item_gungir)]
    public sealed class Gleipnir : CircleItem, IHasTargetModifier
    {
        public Gleipnir(Item item)
            : base(item)
        {
        }

        public override UnitState AppliesUnitState { get; } = UnitState.Rooted;

        public override float Speed { get; } = 1900;

        public string TargetModifierName { get; } = "modifier_gungnir_debuff";
    }
}