using Divine.Core.Entities.Abilities.Components;
using Divine.Core.Entities.Abilities.Items.Bases;
using Divine.Core.Entities.Metadata;
using Divine.Entity.Entities.Abilities.Components;
using Divine.Entity.Entities.Abilities.Items;

namespace Divine.Core.Entities.Abilities.Items
{
    [Item(AbilityId.item_magic_stick)]
    public sealed class MagicStick : ActiveItem, IHasHealthRestore, IHasManaRestore
    {
        public MagicStick(Item item)
            : base(item)
        {
        }

        public override bool CanBeCasted
        {
            get
            {
                return CurrentCharges > 0 && base.CanBeCasted;
            }
        }

        public float TotalHealthRestore
        {
            get
            {
                var chargeRestore = GetAbilitySpecialData("restore_per_charge");
                return CurrentCharges * chargeRestore;
            }
        }

        public float TotalManaRestore
        {
            get
            {
                return TotalHealthRestore;
            }
        }
    }
}