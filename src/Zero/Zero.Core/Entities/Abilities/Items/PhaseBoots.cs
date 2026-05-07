using Divine.Core.Entities.Abilities.Components;
using Divine.Core.Entities.Abilities.Items.Bases;
using Divine.Core.Entities.Metadata;
using Divine.Entity.Entities.Abilities.Components;
using Divine.Entity.Entities.Abilities.Items;

namespace Divine.Core.Entities.Abilities.Items
{
    [Item(AbilityId.item_phase_boots)]
    public sealed class PhaseBoots : ActiveItem, IHasModifier
    {
        public PhaseBoots(Item item)
            : base(item)
        {
        }

        public string ModifierName { get; } = "modifier_item_phase_boots_active";
    }
}