// <copyright file="storm_spirit_electric_vortex.cs" company="Ensage">
//    Copyright (c) 2017 Ensage.
// </copyright>

namespace Ensage.SDK.Abilities.npc_dota_hero_storm_spirit
{
    using Divine.Entity.Entities.Abilities;
    using Divine.Entity.Entities.Units.Components;
    using Divine.Extensions;

    using Ensage.SDK.Abilities.Components;

    public class storm_spirit_electric_vortex : RangedAbility, IHasTargetModifier, IHasModifier, IAreaOfEffectAbility
    {
        public storm_spirit_electric_vortex(Ability ability)
            : base(ability)
        {
        }

        public override UnitState AppliesUnitState { get; } = UnitState.Stunned;

        public override float CastRange
        {
            get
            {
                if (this.Owner.HasAghanimsScepter())
                {
                    return this.Ability.GetAbilitySpecialData("radius_scepter");
                }

                return base.CastRange;
            }
        }

        public string ModifierName { get; } = "modifier_storm_spirit_electric_vortex_self_slow";

        public float Radius
        {
            get
            {
                if (this.Owner.HasAghanimsScepter())
                {
                    return this.Ability.GetAbilitySpecialData("radius_scepter");
                }

                return 0;
            }
        }

        public string TargetModifierName { get; } = "modifier_storm_spirit_electric_vortex_pull";
    }
}