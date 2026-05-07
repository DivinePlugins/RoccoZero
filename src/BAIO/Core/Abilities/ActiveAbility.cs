namespace Ensage.SDK.Abilities
{
    using System.Linq;

    using Divine.Entity.Entities.Abilities;
    using Divine.Entity.Entities.Abilities.Components;
    using Divine.Entity.Entities.Abilities.Items;
    using Divine.Entity.Entities.Runes;
    using Divine.Entity.Entities.Trees;
    using Divine.Entity.Entities.Units;
    using Divine.Extensions;
    using Divine.Game;
    using Divine.Numerics;
    using Divine.Zero.Log;

    using Ensage.SDK.Helpers;

    public abstract class ActiveAbility : BaseAbility
    {
        protected ActiveAbility(Ability ability)
            : base(ability)
        {
        }

        public override bool CanBeCasted
        {
            get
            {
                if (!this.IsReady)
                {
                    return false;
                }

                var owner = this.Owner;
                var isItem = this.Ability is Item;
                if (owner.IsStunned() || isItem && owner.IsMuted() || !isItem && owner.IsSilenced())
                {
                    return false;
                }

                if ((GameManager.RawGameTime - this.LastCastAttempt) < 0.1f)
                {
                    //LogManager.Debug($"blocked {this}");
                    return false;
                }

                return true;
            }
        }

        public virtual float CastPoint
        {
            get
            {
                var level = this.Ability.Level;
                if (level == 0)
                {
                    return 0.0f;
                }

                return this.Ability.AbilityData.GetCastPoint(level - 1);
            }
        }

        public virtual float Speed { get; } = float.MaxValue;

        protected float LastCastAttempt { get; set; }

        public static implicit operator bool(ActiveAbility ability)
        {
            return ability.CanBeCasted;
        }

        public virtual bool CanHit(params Unit[] targets)
        {
            if (!targets.Any())
            {
                return true;
            }

            if (this.Owner.Distance2D(targets.First()) < this.CastRange)
            {
                return true;
            }

            // moar checks
            return false;
        }

        /// <summary>
        ///     Gets the time needed to execute an ability. This assumes that you are already in range and includes turnrate,
        ///     castpoint and ping.
        /// </summary>
        /// <param name="target">The target of the ability.</param>
        /// <returns>Time in ms until the cast.</returns>
        public virtual int GetCastDelay(Unit target)
        {
            if (target == null)
            {
                return GetCastDelay();
            }

            return (int)(((this.CastPoint + this.Owner.TurnTime(target.Position)) * 1000.0f) + GameManager.Ping);
        }

        /// <summary>
        ///     Gets the time needed to execute an ability. This assumes that you are already in range and includes turnrate,
        ///     castpoint and ping.
        /// </summary>
        /// <param name="position">The target position of the ability.</param>
        /// <returns>Time in ms until the cast.</returns>
        public virtual int GetCastDelay(Vector3 position)
        {
            return (int)(((this.CastPoint + this.Owner.TurnTime(position)) * 1000.0f) + GameManager.Ping);
        }

        /// <summary>
        ///     Gets the time needed to execute an ability. This assumes that you are already in range and includes castpoint and
        ///     ping.
        /// </summary>
        /// <returns>Time in ms until the cast.</returns>
        public virtual int GetCastDelay()
        {
            return (int)((this.CastPoint * 1000.0f) + GameManager.Ping);
        }

        public override float GetDamage(params Unit[] targets)
        {
            var damage = this.RawDamage;
            if (damage == 0)
            {
                return 0;
            }

            var amplify = this.Owner.GetSpellAmplification();
            var reduction = 0.0f;
            if (targets.Any())
            {
                reduction = this.Ability.GetDamageReduction(targets.First(), this.DamageType);
            }

            return DamageHelpers.GetSpellDamage(damage, amplify, reduction);
        }

        public override float GetDamage(Unit target, float damageModifier, float targetHealth = float.MinValue)
        {
            var damage = this.RawDamage;
            if (damage == 0)
            {
                return 0;
            }

            var amplify = this.Owner.GetSpellAmplification();
            var reduction = this.Ability.GetDamageReduction(target, this.DamageType);

            return DamageHelpers.GetSpellDamage(damage, amplify, -reduction, damageModifier);
        }

        /// <summary>
        ///     Gets the time until the ability lands on the target. This includes the cast time and assumes that you are in range
        ///     to cast.
        /// </summary>
        /// <param name="target">The target of your ability.</param>
        /// <returns>Time until the spell hits in ms.</returns>
        public virtual int GetHitTime(Unit target)
        {
            return this.GetHitTime(target.Position);
        }

        /// <summary>
        ///     Gets the time until the ability lands on the target position. This includes the cast time and assumes that you are
        ///     in range to cast.
        /// </summary>
        /// <param name="position">The target position of your ability.</param>
        /// <returns>Time until the spell hits in ms.</returns>
        public virtual int GetHitTime(Vector3 position)
        {
            if (this.Speed == float.MaxValue || this.Speed == 0)
            {
                return this.GetCastDelay(position) + (int)(this.ActivationDelay * 1000.0f);
            }

            var time = this.Owner.Distance2D(position) / this.Speed;
            return this.GetCastDelay(position) + (int)((time + this.ActivationDelay) * 1000.0f);
        }

        public virtual bool Cast()
        {
            if (!this.CanBeCasted)
            {
                //LogManager.Debug($"blocked {this}");
                return false;
            }

            var result = this.Ability.Cast();
            if (result)
            {
                this.LastCastAttempt = GameManager.RawGameTime;
            }

            return result;
        }

        public virtual bool Cast(Unit target)
        {
            if (!this.CanBeCasted)
            {
                //LogManager.Debug($"blocked {this}");
                return false;
            }

            bool result;
            if ((this.Ability.AbilityBehavior & AbilityBehavior.UnitTarget) == AbilityBehavior.UnitTarget)
            {
                result = this.Ability.Cast(target);
            }
            else if ((this.Ability.AbilityBehavior & AbilityBehavior.Point) == AbilityBehavior.Point)
            {
                result = this.Ability.Cast(target.Position);
            }
            else
            {
                result = this.Ability.Cast();
            }

            if (result)
            {
                this.LastCastAttempt = GameManager.RawGameTime;
            }

            return result;
        }

        public virtual bool Cast(Tree target)
        {
            if (!this.CanBeCasted)
            {
                //LogManager.Debug($"blocked {this}");
                return false;
            }

            var result = this.Ability.Cast(target);
            if (result)
            {
                this.LastCastAttempt = GameManager.RawGameTime;
            }

            return result;
        }

        public virtual bool Cast(Vector3 position)
        {
            if (!this.CanBeCasted)
            {
                //LogManager.Debug($"blocked {this}");
                return false;
            }

            bool result;
            if ((this.Ability.AbilityBehavior & AbilityBehavior.Point) == AbilityBehavior.Point)
            {
                result = this.Ability.Cast(position);
            }
            else
            {
                result = this.Ability.Cast();
            }

            if (result)
            {
                this.LastCastAttempt = GameManager.RawGameTime;
            }

            return result;
        }

        public virtual bool Cast(Rune target)
        {
            if (!this.CanBeCasted)
            {
                //LogManager.Debug($"blocked {this}");
                return false;
            }

            var result = this.Ability.Cast(target);
            if (result)
            {
                this.LastCastAttempt = GameManager.RawGameTime;
            }

            return result;
        }
    }
}