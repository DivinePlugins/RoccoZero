// <copyright file="drow_ranger_marksmanship.cs" company="Ensage">
//    Copyright (c) 2017 Ensage.
// </copyright>

namespace Ensage.SDK.Abilities.npc_dota_hero_drow_ranger
{
    using Divine.Entity.Entities.Abilities;
    using Divine.Extensions;

    using Ensage.SDK.Abilities.Components;

    public class drow_ranger_marksmanship : PassiveAbility, IHasModifier, IAreaOfEffectAbility, IHasProcChance
    {
        public drow_ranger_marksmanship(Ability ability)
            : base(ability)
        {
        }

        public string ModifierName { get; } = "modifier_drow_ranger_marksmanship_aura_bonus";

        public float Radius
        {
            get
            {
                return this.Ability.GetAbilitySpecialData("disable_range");
            }
        }

        public bool IsPseudoChance { get; } = true;

        public float ProcChance
        {
            get
            {
                return this.Ability.GetAbilitySpecialData("chance");
            }
        }
    }
}
