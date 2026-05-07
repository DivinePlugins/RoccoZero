using Divine.Core.Entities.Abilities.Components;
using Divine.Core.Entities.Abilities.Items.Bases;
using Divine.Core.Entities.Metadata;
using Divine.Entity.Entities.Abilities.Components;
using Divine.Entity.Entities.Abilities.Items;

namespace Divine.Core.Entities.Abilities.Items
{
    [Item(AbilityId.item_travel_boots)]
    [Item(AbilityId.item_travel_boots_2)]
    public sealed class BootsOfTravel : RangedItem, IChannableAbility, IHasModifier, IHasModifierTexture
    {
        public BootsOfTravel(Item item)
            : base(item)
        {
        }

        public float ChannelDuration
        {
            get
            {
                var level = Base.Level;
                if (level == 0)
                {
                    return 0;
                }

                return GetChannelTime(level - 1);
            }
        }

        public float RemainingDuration
        {
            get
            {
                if (!IsChanneling)
                {
                    return 0;
                }

                return ChannelDuration - ChannelTime;
            }
        }

        public string ModifierName { get; } = "modifier_teleporting";

        public string[] ModifierTextureName { get; } = { "item_travel_boots", "item_travel_boots_2", "item_travel_boots_tinker" };
    }
}