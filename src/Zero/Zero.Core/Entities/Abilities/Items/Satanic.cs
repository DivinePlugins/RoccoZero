using Divine.Core.Entities.Abilities.Components;
using Divine.Core.Entities.Abilities.Items.Bases;
using Divine.Core.Entities.Metadata;
using Divine.Entity.Entities.Abilities.Components;
using Divine.Entity.Entities.Abilities.Items;

namespace Divine.Core.Entities.Abilities.Items
{
    [Item(AbilityId.item_satanic)]
    public sealed class Satanic : ActiveItem, IHasModifier
    {
        public Satanic(Item item)
            : base(item)
        {
        }

        public string ModifierName { get; } = "modifier_item_satanic_unholy";
    }
}