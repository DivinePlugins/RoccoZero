using Divine.Entity.Entities.Abilities.Items;

namespace Divine.Core.Entities.Abilities.Items.Bases
{
    public abstract class AuraItem : PassiveItem, IAuraAbility
    {
        protected AuraItem(Item item)
            : base(item)
        {
        }

        public virtual string AuraModifierName { get; } = string.Empty;

        public virtual float AuraRadius
        {
            get
            {
                return GetAbilitySpecialData("aura_radius");
            }
        }
    }
}