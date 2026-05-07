// <copyright file="drow_ranger_wave_of_silence.cs" company="Ensage">
//    Copyright (c) 2017 Ensage.
// </copyright>

namespace Ensage.SDK.Abilities.npc_dota_hero_drow_ranger
{
    using Divine.Entity.Entities.Abilities;
    using Divine.Entity.Entities.Units.Components;
    using Divine.Extensions;

    using Ensage.SDK.Abilities.Components;

    public class drow_ranger_wave_of_silence : LineAbility, IHasTargetModifier
    {
        public drow_ranger_wave_of_silence(Ability ability)
            : base(ability)
        {
        }

        public override UnitState AppliesUnitState { get; } = UnitState.Silenced;

        public override float Radius
        {
            get
            {
                return this.Ability.GetAbilitySpecialData("wave_width");
            }
        }

        public override float Speed
        {
            get
            {
                return this.Ability.GetAbilitySpecialData("wave_speed");
            }
        }

        public string TargetModifierName { get; } = "modifier_drowranger_wave_of_silence_knockback";
    }
}