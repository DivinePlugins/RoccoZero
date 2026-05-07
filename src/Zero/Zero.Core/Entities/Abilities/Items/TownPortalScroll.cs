using Divine.Core.Entities.Abilities.Components;
using Divine.Core.Entities.Abilities.Items.Bases;
using Divine.Core.Entities.Metadata;
using Divine.Entity.Entities.Abilities.Components;
using Divine.Entity.Entities.Abilities.Items;

namespace Divine.Core.Entities.Abilities.Items
{
    [Item(AbilityId.item_tpscroll)]
    public sealed class TownPortalScroll : RangedItem, IChannableAbility, IHasModifier, IHasModifierTexture
    {
        public TownPortalScroll(Item item)
            : base(item)
        {
        }

        internal override void Dispose()
        {
            base.Dispose();
        }

        public float ChannelDuration
        {
            get
            {
                var level = Level;
                if (level == 0)
                {
                    return 0;
                }

                // sometimes can be longer...
                return GetChannelTime(level - 1);
            }
        }

        public string ModifierName { get; } = "modifier_teleporting";

        public string[] ModifierTextureName { get; } = { "item_tpscroll" };

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
    }
}