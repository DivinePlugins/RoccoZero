using Divine.Core.Entities.Abilities.Components;
using Divine.Core.Entities.Abilities.Items.Bases;
using Divine.Core.Entities.Metadata;
using Divine.Entity.Entities.Abilities.Components;
using Divine.Entity.Entities.Abilities.Items;

namespace Divine.Core.Entities.Abilities.Items
{
    [Item(AbilityId.item_arcane_boots)]
    public sealed class ArcaneBoots : ActiveItem, IAreaOfEffectAbility, IHasManaRestore
    {
        public ArcaneBoots(Item item)
            : base(item)
        {
        }

        public float Radius
        {
            get
            {
                return GetAbilitySpecialData("replenish_radius");
            }
        }

        public float TotalManaRestore
        {
            get
            {
                return GetAbilitySpecialData("replenish_amount");
            }
        }
    }
}