// <copyright file="void_spirit_aether_remnant.cs" company="Ensage">
//    Copyright (c) 2019 Ensage.
// </copyright>

namespace Ensage.SDK.Abilities.npc_dota_hero_void_spirit
{
    using System;

    using Divine.Entity.Entities.Abilities;
    using Divine.Extensions;
    using Divine.Game;
    using Divine.Numerics;

    public class void_spirit_aether_remnant : LineAbility
    {
        public void_spirit_aether_remnant(Ability ability)
            : base(ability)
        {
        }

        public override float Radius
        {
            get
            {
                return this.Ability.GetAbilitySpecialData("start_radius");
            }
        }

        public override float Range
        {
            get
            {
                return this.Ability.GetAbilitySpecialData("radius");
            }
        }

        public override float Speed
        {
            get
            {
                return this.Ability.GetAbilitySpecialData("projectile_speed");
            }
        }

        protected override float RawDamage
        {
            get
            {
                return this.Ability.GetAbilitySpecialData("impact_damage");
            }
        }

        public bool Cast(Vector3 startPosition, Vector3 direction)
        {
            if (!this.CanBeCasted)
            {
                return false;
            }

            var result = this.Ability.Cast(startPosition, direction);

            if (result)
            {
                this.LastCastAttempt = GameManager.RawGameTime;
            }

            return result;
        }

        public override bool Cast(Vector3 position)
        {
            var distance = Math.Max(this.Owner.AttackRange(), this.Owner.Distance2D(position) - this.CastRange);
            var startPosition = position.Extend(this.Owner.Position, distance);

            return this.Cast(startPosition, position);
        }
    }
}