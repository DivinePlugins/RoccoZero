using Divine.Core.Entities.Abilities.Components;
using Divine.Core.Entities.Abilities.Items.Bases;
using Divine.Core.Entities.Metadata;
using Divine.Entity.Entities.Abilities.Components;
using Divine.Entity.Entities.Abilities.Items;

namespace Divine.Core.Entities.Abilities.Items
{
    [Item(AbilityId.item_mask_of_madness)]
    public sealed class MaskOfMadness : ActiveItem, IHasModifier
    {
        public MaskOfMadness(Item item)
            : base(item)
        {
        }

        public string ModifierName { get; } = "modifier_item_mask_of_madness_berserk";
    }
}