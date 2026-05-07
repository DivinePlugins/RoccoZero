using Divine.Core.Entities.Abilities.Components;
using Divine.Core.Entities.Abilities.Items.Bases;
using Divine.Core.Entities.Metadata;
using Divine.Entity.Entities.Abilities.Components;
using Divine.Entity.Entities.Abilities.Items;

namespace Divine.Core.Entities.Abilities.Items
{
    [Item(AbilityId.item_black_king_bar)]
    public sealed class BladeMail : ActiveItem, IHasModifier
    {
        public BladeMail(Item item)
            : base(item)
        {
        }

        public string ModifierName { get; } = "modifier_item_blade_mail_reflect";
    }
}