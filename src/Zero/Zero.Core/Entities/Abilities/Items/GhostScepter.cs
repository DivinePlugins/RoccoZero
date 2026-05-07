using Divine.Core.Entities.Abilities.Components;
using Divine.Core.Entities.Abilities.Items.Bases;
using Divine.Core.Entities.Metadata;
using Divine.Entity.Entities.Abilities.Components;
using Divine.Entity.Entities.Abilities.Items;

namespace Divine.Core.Entities.Abilities.Items
{
    [Item(AbilityId.item_ghost)]
    public sealed class GhostScepter : ActiveItem, IHasModifier
    {
        public GhostScepter(Item item)
            : base(item)
        {
        }

        public string ModifierName { get; } = "modifier_ghost_state";
    }
}