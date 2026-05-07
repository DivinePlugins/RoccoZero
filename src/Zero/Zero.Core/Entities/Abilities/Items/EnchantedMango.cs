using Divine.Core.Entities.Abilities.Components;
using Divine.Core.Entities.Abilities.Items.Bases;
using Divine.Core.Entities.Metadata;
using Divine.Entity.Entities.Abilities.Components;
using Divine.Entity.Entities.Abilities.Items;

namespace Divine.Core.Entities.Abilities.Items
{
    [Item(AbilityId.item_enchanted_mango)]
    public sealed class EnchantedMango : RangedItem, IHasManaRestore
    {
        public EnchantedMango(Item item)
            : base(item)
        {
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