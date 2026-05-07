namespace Ensage.SDK.Abilities
{
    using Divine.Entity.Entities.Abilities;
    using Divine.Extensions;

    using Ensage.SDK.Extensions;

    public abstract class AuraAbility : PassiveAbility, IAuraAbility
    {
        protected AuraAbility(Ability ability)
            : base(ability)
        {
        }

        public virtual string AuraModifierName { get; } = string.Empty;

        public virtual float AuraRadius
        {
            get
            {
                return this.Ability.GetAbilitySpecialData("aura_radius");
            }
        }
    }
}