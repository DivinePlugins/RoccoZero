namespace Ensage.SDK.Abilities
{
    using System.Linq;

    using Divine.Entity.Entities.Abilities;
    using Divine.Entity.Entities.Units;
    using Divine.Extensions;
    using Divine.Prediction;
    using Divine.Prediction.Collision;

    public abstract class PredictionAbility : RangedAbility
    {
        protected PredictionAbility(Ability ability)
            : base(ability)
        {
        }

        public virtual CollisionTypes CollisionTypes { get; } = CollisionTypes.None;

        public virtual bool HasAreaOfEffect
        {
            get
            {
                return this.CollisionTypes == CollisionTypes.None;
            }
        }

        public abstract PredictionSkillshotType PredictionSkillshotType { get; }

        public virtual float Radius
        {
            get
            {
                return this.Ability.GetAbilitySpecialData("radius");
            }
        }

        public virtual float Range
        {
            get
            {
                return this.CastRange;
            }
        }

        public override float Speed
        {
            get
            {
                return this.Ability.GetAbilitySpecialData("speed");
            }
        }

        public virtual PredictionInput GetPredictionInput(params Unit[] targets)
        {
            var input = new PredictionInput
            {
                Owner = this.Owner,
                AreaOfEffect = this.HasAreaOfEffect,
                CollisionTypes = this.CollisionTypes,
                Delay = this.CastPoint + this.ActivationDelay,
                Speed = this.Speed,
                Range = this.CastRange,
                Radius = this.Radius,
                PredictionSkillshotType = this.PredictionSkillshotType
            };

            if (this.HasAreaOfEffect)
            {
                input.AreaOfEffectTargets = targets;
            }

            return input.WithTarget(targets.First());
        }

        public virtual PredictionOutput GetPredictionOutput(PredictionInput input)
        {
            return PredictionManager.GetPrediction(input);
        }

        public bool Cast(Unit target, HitChance minimChance)
        {
            if (!this.CanBeCasted)
            {
                return false;
            }

            var predictionInput = this.GetPredictionInput(target);
            var output = this.GetPredictionOutput(predictionInput);
            if (output.HitChance == HitChance.OutOfRange || output.HitChance == HitChance.Impossible)
            {
                return false;
            }

            if (predictionInput.CollisionTypes != CollisionTypes.None && output.HitChance == HitChance.Collision)
            {
                return false;
            }

            if (output.HitChance < minimChance)
            {
                return false;
            }

            return this.Cast(output.CastPosition);
        }

        public override bool Cast(Unit target)
        {
            return this.Cast(target, HitChance.Medium); // TODO: get prediction config hitchance value
        }

        // TODO: add other UseAbility overload without parameter etc to automatically get best position for clock rockets, magnus ult etc?
    }
}