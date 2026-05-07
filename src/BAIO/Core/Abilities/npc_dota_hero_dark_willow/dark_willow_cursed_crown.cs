// <copyright file="dark_willow_cursed_crown.cs" company="Ensage">
//    Copyright (c) 2017 Ensage.
// </copyright>

namespace Ensage.SDK.Abilities.npc_dota_hero_dark_willow
{
    using Divine.Entity.Entities.Abilities;
    using Divine.Entity.Entities.Units.Components;
    using Divine.Extensions;

    using Ensage.SDK.Abilities.Components;

    public class dark_willow_cursed_crown : AreaOfEffectAbility, IHasTargetModifier, IHasTargetModifierTexture
    {
        public dark_willow_cursed_crown(Ability ability)
            : base(ability)
        {
        }

        public override float ActivationDelay
        {
            get
            {
                return this.Ability.GetAbilitySpecialData("delay");
            }
        }

        public override UnitState AppliesUnitState { get; } = UnitState.Stunned;

        public override float Radius
        {
            get
            {
                return this.Ability.GetAbilitySpecialData("stun_radius");
            }
        }

        // debuff
        public string TargetModifierName { get; } = "modifier_dark_willow_cursed_crown";

        // stun
        public string[] TargetModifierTextureName { get; } = { "dark_willow_cursed_crown" };
    }
}