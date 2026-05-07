using Divine.Core.Entities.Abilities.Components;
using Divine.Core.Entities.Abilities.Items.Bases;
using Divine.Core.Entities.Metadata;
using Divine.Entity.Entities.Abilities.Components;
using Divine.Entity.Entities.Abilities.Items;

namespace Divine.Core.Entities.Abilities.Items
{
    [Item(AbilityId.item_force_staff)]
    public sealed class ForceStaff : RangedItem, IHasTargetModifier
    {
        public ForceStaff(Item item)
            : base(item)
        {
        }

        public float PushLength
        {
            get
            {
                return GetAbilitySpecialData("push_length");
            }
        }

        public float PushSpeed { get; } = 1500f;

        public string TargetModifierName { get; } = "modifier_item_forcestaff_active";
    }
}