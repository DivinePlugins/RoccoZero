// <copyright file="obsidian_destroyer_astral_imprisonment.cs" company="Ensage">
//    Copyright (c) 2017 Ensage.
// </copyright>

namespace Ensage.SDK.Abilities.npc_dota_hero_obsidian_destroyer
{
    using Divine.Entity.Entities.Abilities;
    using Divine.Entity.Entities.Units.Components;
    using Divine.Extensions;

    using Ensage.SDK.Abilities.Components;

    public class obsidian_destroyer_astral_imprisonment : AreaOfEffectAbility, IHasTargetModifier
    {
        public obsidian_destroyer_astral_imprisonment(Ability ability)
            : base(ability)
        {
        }

        public override UnitState AppliesUnitState { get; } = UnitState.Invulnerable;

        public override float Radius
        {
            get
            {
                if (Owner.HasAghanimsScepter())
                {
                    return this.Ability.GetAbilitySpecialData("scepter_damage_radius");
                }

                return 0f;
            }
        }

        public string TargetModifierName { get; } = "modifier_obsidian_destroyer_astral_imprisonment_prison";

        protected override float RawDamage
        {
            get
            {
                var bonus = 0f;

                if (Owner.HasAghanimsScepter())
                {
                    bonus += this.Ability.GetAbilitySpecialData("scepter_damage_radius");
                }

                return bonus + this.Ability.GetAbilitySpecialData("damage");
            }
        }
    }
}