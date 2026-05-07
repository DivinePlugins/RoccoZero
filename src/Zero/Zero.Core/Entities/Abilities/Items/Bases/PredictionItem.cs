namespace Divine.Core.Entities.Abilities.Items.Bases;

using System.Linq;

using Divine.Entity.Entities.Abilities.Items;
using Divine.Entity.Entities.Units;
using Divine.Entity.Entities.Units.Heroes;
using Divine.Prediction;
using Divine.Prediction.Collision;

public abstract class PredictionItem : RangedItem
{
    protected PredictionItem(Item item)
        : base(item)
    {
        Prediction = new Prediction();
    }

    public virtual CollisionTypes CollisionTypes { get; } = CollisionTypes.None;

    public virtual bool HasAreaOfEffect
    {
        get
        {
            return CollisionTypes == CollisionTypes.None;
        }
    }

    public abstract PredictionSkillshotType PredictionSkillshotType { get; }

    public virtual float Radius
    {
        get
        {
            return GetAbilitySpecialData("radius");
        }
    }

    public virtual float Range
    {
        get
        {
            return CastRange;
        }
    }

    public override float Speed
    {
        get
        {
            return GetAbilitySpecialData("speed");
        }
    }

    protected IPrediction Prediction { get; }

    public virtual PredictionInput GetPredictionInput(params Unit[] targets)
    {
        var input = new PredictionInput
        {
            Owner = Owner.Base as Hero,   // TODO
            AreaOfEffect = HasAreaOfEffect,
            CollisionTypes = CollisionTypes,
            Delay = CastPoint + ActivationDelay,
            Speed = Speed,
            Range = CastRange,
            Radius = Radius,
            PredictionSkillshotType = PredictionSkillshotType
        };

        if (HasAreaOfEffect)
        {
            input.AreaOfEffectTargets = targets;
        }

        return input.WithTarget(targets.First());
    }

    public virtual PredictionOutput GetPredictionOutput(PredictionInput input)
    {
        return Prediction.GetPrediction(input);
    }

    public bool UseAbility(Unit target, HitChance minimChance)
    {
        if (!CanBeCasted)
        {
            return false;
        }

        var predictionInput = GetPredictionInput(target);
        var output = GetPredictionOutput(predictionInput);
        if (output.HitChance is HitChance.OutOfRange or HitChance.Impossible)
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

        return UseAbility(output.CastPosition);
    }
}