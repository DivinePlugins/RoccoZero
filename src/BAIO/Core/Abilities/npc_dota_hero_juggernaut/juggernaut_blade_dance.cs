// <copyright file="juggernaut_blade_dance.cs" company="Ensage">
//    Copyright (c) 2017 Ensage.
// </copyright>

namespace Ensage.SDK.Abilities.npc_dota_hero_juggernaut
{
    using Divine.Entity.Entities.Abilities;
    using Divine.Extensions;

    using Ensage.SDK.Abilities.Components;

    public class juggernaut_blade_dance : PassiveAbility, IHasCritChance
    {
        public juggernaut_blade_dance(Ability ability)
            : base(ability)
        {
        }

        public float CritMultiplier
        {
            get
            {
                return this.Ability.GetAbilitySpecialData("blade_dance_crit_mult") / 100.0f;
            }
        }

        public bool IsPseudoChance { get; } = true;

        public float ProcChance
        {
            get
            {
                return this.Ability.GetAbilitySpecialData("blade_dance_crit_chance") / 100.0f;
            }
        }
    }
}