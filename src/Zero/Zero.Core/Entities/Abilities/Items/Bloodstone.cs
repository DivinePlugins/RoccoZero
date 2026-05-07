using Divine.Core.Entities.Abilities.Components;
using Divine.Core.Entities.Abilities.Items.Bases;
using Divine.Core.Entities.Metadata;
using Divine.Entity.Entities.Abilities.Components;
using Divine.Entity.Entities.Abilities.Items;

namespace Divine.Core.Entities.Abilities.Items
{
    [Item(AbilityId.item_bloodstone)]
    public sealed class Bloodstone : ActiveItem, IHasModifier, IHasHealthRestore
    {
        public Bloodstone(Item item)
            : base(item)
        {
        }

        public string ModifierName { get; } = "modifier_item_bloodstone_active";

        public float TotalHealthRestore
        {
            get
            {
                return Owner.Mana * GetAbilitySpecialData("mana_cost_percentage") / 100f;
            }
        }
    }
}