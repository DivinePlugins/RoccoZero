using Divine.Core.Entities.Abilities.Components;
using Divine.Core.Entities.Abilities.Items.Bases;
using Divine.Core.Entities.Metadata;
using Divine.Entity.Entities.Abilities.Components;
using Divine.Entity.Entities.Abilities.Items;

namespace Divine.Core.Entities.Abilities.Items
{
    [Item(AbilityId.item_soul_ring)]
    public sealed class SoulRing : ActiveItem, IHasModifier, IHasHealthCost, IHasManaRestore
    {
        public SoulRing(Item item)
            : base(item)
        {
        }

        public float HealthCost
        {
            get
            {
                return GetAbilitySpecialData("health_sacrifice");
            }
        }

        public string ModifierName { get; } = "modifier_item_soul_ring_buff";

        public float TotalManaRestore
        {
            get
            {
                return GetAbilitySpecialData("mana_gain");
            }
        }
    }
}