namespace Ensage.SDK.TargetSelector.Modes;

using System;
using System.Linq;

using Divine.Entity;
using Divine.Entity.Entities.Units;
using Divine.Entity.Entities.Units.Creeps;
using Divine.Extensions;
using Divine.Game;

using Ensage.SDK.Prediction;

public class AutoAttackModeSelector : IDisposable
{
    private readonly HealthPrediction HealthPrediction;

    public AutoAttackModeSelector(Unit owner)
    {
        Owner = owner;
        HealthPrediction = new HealthPrediction();
    }

    public float BonusRange
    {
        get
        {
            return Owner.IsMelee ? BonusMeleeRange : BonusRangedRange;
        }
    }

    private static bool LaneClearRateLimitResult { get; set; }

    private static double LaneClearRateLimitTime { get; set; }

    public bool Deny { get; set; } = true;

    public bool Lasthit { get; set; } = true;

    public float BonusMeleeRange { get; set; }

    public float BonusRangedRange { get; set; }

    private Unit Owner { get; }

    public Unit GetTarget()
    {
        if (Lasthit)
        {
            var killableCreep = EntityManager.GetEntities<Creep>().FirstOrDefault(unit => unit.IsVisible && IsValid(unit) && CanKill(unit));

            if (killableCreep != null)
            {
                return killableCreep;
            }
        }

        if (Lasthit && HealthPrediction != null)
        {
            if ((GameManager.RawGameTime - LaneClearRateLimitTime) > 0.25f)
            {
                LaneClearRateLimitResult = HealthPrediction.ShouldWait();
                LaneClearRateLimitTime = GameManager.RawGameTime;
            }

            if (LaneClearRateLimitResult)
            {
                return null;
            }
        }

        if (Deny)
        {
            var denyCreep = EntityManager.GetEntities<Creep>().FirstOrDefault(
                unit => unit.IsVisible && IsValid(unit, true) && unit.HealthPercent() < 0.5f && CanKill(unit));

            if (denyCreep != null)
            {
                return denyCreep;
            }
        }

        return null;
    }

    private bool CanKill(Unit target)
    {
        var extraMoveTime = 0.0f;
        if (BonusRange > 0.0f && !Owner.IsInAttackRange(target))
        {
            var speed = Owner.MovementSpeed;
            if (speed > 0)
            {
                var distance = Math.Max(0.0f, Owner.Distance2D(target) - Owner.AttackRange());
                extraMoveTime = distance / speed;
            }
        }

        return Owner.GetAttackDamage(target, true)
               > HealthPrediction.GetPrediction(target, (Owner.GetAutoAttackArrivalTime(target) + (GameManager.Ping / 2000f) + extraMoveTime) - 0.05f);
    }

    private bool IsValid(Unit target, bool myTeam = false)
    {
        if (myTeam)
        {
            return target.Team == Owner.Team && Owner.IsValidOrbwalkingTarget(target, BonusRange);
        }

        return target.Team != Owner.Team && Owner.IsValidOrbwalkingTarget(target, BonusRange);
    }

    public void Dispose()
    {
        HealthPrediction.Dispose();
        GC.SuppressFinalize(this);
    }
}