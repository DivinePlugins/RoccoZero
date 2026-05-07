using Divine.Core.Entities.Abilities.Components;
using Divine.Core.Entities.Abilities.Items.Bases;
using Divine.Core.Entities.Metadata;
using Divine.Entity.Entities.Abilities.Components;
using Divine.Entity.Entities.Abilities.Items;

namespace Divine.Core.Entities.Abilities.Items
{
    [Item(AbilityId.item_veil_of_discord)]
    public sealed class VeilOfDiscord : CircleItem, IHasTargetModifier, IHasDamageAmplifier
    {
        public VeilOfDiscord(Item item)
            : base(item)
        {
        }

        public DamageType AmplifierType { get; } = DamageType.Magical;

        public float DamageAmplification
        {
            get
            {
                return GetAbilitySpecialData("spell_amp") / 100.0f; // -20 to all abilities
            }
        }

        public override float Radius
        {
            get
            {
                return GetAbilitySpecialData("debuff_radius");
            }
        }

        public string TargetModifierName { get; } = "modifier_item_veil_of_discord_debuff";
    }
}