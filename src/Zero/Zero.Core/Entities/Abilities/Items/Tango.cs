using Divine.Core.Entities.Abilities.Components;
using Divine.Core.Entities.Abilities.Items.Bases;
using Divine.Core.Entities.Metadata;
using Divine.Entity.Entities.Abilities.Components;
using Divine.Entity.Entities.Abilities.Items;

namespace Divine.Core.Entities.Abilities.Items
{
    [Item(AbilityId.item_tango)]
    [Item(AbilityId.item_tango_single)]
    public sealed class Tango : RangedItem, IHasModifier
    {
        public Tango(Item item)
            : base(item)
        {
        }

        public string ModifierName { get; } = "modifier_tango_heal";
    }
}