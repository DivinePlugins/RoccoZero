// <copyright file="sven_storm_bolt.cs" company="Ensage">
//    Copyright (c) 2017 Ensage.
// </copyright>

namespace Ensage.SDK.Abilities.npc_dota_hero_sven
{
    using Divine.Entity.Entities.Abilities;
    using Divine.Entity.Entities.Units.Components;
    using Divine.Extensions;

    using Ensage.SDK.Abilities.Components;

    public class sven_storm_bolt : AreaOfEffectAbility, IHasTargetModifierTexture
    {
        public sven_storm_bolt(Ability ability)
            : base(ability)
        {
        }

        public override UnitState AppliesUnitState { get; } = UnitState.Stunned;

        public override float Radius
        {
            get
            {
                return this.Ability.GetAbilitySpecialData("bolt_aoe");
            }
        }

        public override float Speed
        {
            get
            {
                return this.Ability.GetAbilitySpecialData("bolt_speed");
            }
        }

        public string[] TargetModifierTextureName { get; } = { "sven_storm_bolt" };
    }
}