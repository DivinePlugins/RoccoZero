// <copyright file="drow_ranger_frost_arrows.cs" company="Ensage">
//    Copyright (c) 2017 Ensage.
// </copyright>

namespace Ensage.SDK.Abilities.npc_dota_hero_drow_ranger
{
    using Divine.Entity.Entities.Abilities;
    using Divine.Entity.Entities.Units;
    using Divine.Entity.Entities.Units.Heroes;
    using Divine.Extensions;

    using Ensage.SDK.Abilities.Components;

    public class drow_ranger_frost_arrows : OrbAbility, IHasTargetModifier
    {
        public drow_ranger_frost_arrows(Ability ability)
            : base(ability)
        {
        }

        public string TargetModifierName { get; } = "modifier_drow_ranger_frost_arrows_slow";

        public float GetModifierDuration(Unit target)
        {
            if (target is Hero)
            {
                return this.Ability.GetAbilitySpecialData("frost_arrows_creep_duration") / 4.0f; // 1.5 seconds. They removed the hero tooltip.
            }

            return this.Ability.GetAbilitySpecialData("frost_arrows_creep_duration");
        }
    }
}