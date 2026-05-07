using Divine.Entity.Entities.Abilities;

namespace Divine.Core.Entities.Abilities.Spells.Bases
{
    public abstract class AuraSpell : PassiveSpell, IAuraAbility
    {
        protected AuraSpell(Ability ability)
            : base(ability)
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