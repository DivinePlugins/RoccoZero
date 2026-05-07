namespace Divine.Core.Entities.Abilities.Spells.Bases;

using System.Linq;

using Divine.Core.Extensions;
using Divine.Core.Helpers;
using Divine.Entity.Entities.Abilities;
using Divine.Entity.Entities.Abilities.Components;
using Divine.Entity.Entities.Runes;
using Divine.Entity.Entities.Trees;
using Divine.Extensions;
using Divine.Game;
using Divine.Numerics;

public abstract class ActiveSpell : CAbility, IActiveAbility
{
    protected ActiveSpell(Ability ability)
        : base(ability)
    {
    }

    public override bool CanBeCasted
    {
        get
        {
            if (!IsReady)
            {
                return false;
            }

            if (Owner.IsStunned() || Owner.IsMuted() || Owner.IsSilenced())
            {
                return false;
            }

            return true;
        }
    }

    public virtual float CastPoint
    {
        get
        {
            var level = Level;
            if (level == 0)
            {
                return 0f;
            }

            return GetCastPoint(level - 1);
        }
    }

    public override float Speed { get; } = float.MaxValue;

    //protected float LastCastAttempt { get; set; }

    public virtual bool CanHit(CUnit target)
    {
        return Owner.Distance2D(target) < CastRange;
    }

    public virtual int GetCastDelay(CUnit target)
    {
        return (int)(((CastPoint + Owner.GetTurnTime(target.Position)) * 1000.0f) + GameManager.Ping);
    }

    public virtual int GetCastDelay(Vector3 position)
    {
        return (int)(((CastPoint + Owner.GetTurnTime(position)) * 1000.0f) + GameManager.Ping);
    }

    public virtual int GetCastDelay()
    {
        return (int)((CastPoint * 1000.0f) + GameManager.Ping);
    }

    public override float GetDamage(params CUnit[] targets)
    {
        var damage = RawDamage;
        if (damage == 0)
        {
            return 0;
        }

        var amplify = Owner.GetSpellAmplification();
        var reduction = 0.0f;
        if (targets.Any())
        {
            reduction = this.GetDamageReduction(targets.First(), DamageType);
        }

        return DamageHelpers.GetSpellDamage(damage, amplify, reduction);
    }

    public override float GetDamage(CUnit target, float damageModifier, float targetHealth = float.MinValue)
    {
        var damage = RawDamage;
        if (damage == 0)
        {
            return 0;
        }

        var amplify = Owner.GetSpellAmplification();
        var reduction = this.GetDamageReduction(target, DamageType);

        return DamageHelpers.GetSpellDamage(damage, amplify, -reduction, damageModifier);
    }

    public virtual int GetHitTime(CUnit target)
    {
        return GetHitTime(target.Position);
    }

    public virtual int GetHitTime(Vector3 position)
    {
        if (Speed == float.MaxValue || Speed == 0)
        {
            return GetCastDelay(position) + (int)(ActivationDelay * 1000.0f);
        }

        var time = Owner.Distance2D(position) / Speed;
        return GetCastDelay(position) + (int)((time + ActivationDelay) * 1000.0f);
    }

    private protected static readonly Sleeper CastSleeper = new Sleeper();

    public override bool UseAbility()
    {
        if (CastSleeper.Sleeping || IsInAbilityPhase)
        {
            return false;
        }

        var result = base.UseAbility();
        if (result)
        {
            CastSleeper.Sleep(100);
        }

        return result;
    }

    public override bool UseAbility(CUnit target)
    {
        if (CastSleeper.Sleeping || IsInAbilityPhase)
        {
            return false;
        }

        bool result;
        if ((AbilityBehavior & AbilityBehavior.UnitTarget) == AbilityBehavior.UnitTarget)
        {
            result = base.UseAbility(target);
        }
        else if ((AbilityBehavior & AbilityBehavior.Point) == AbilityBehavior.Point)
        {
            result = base.UseAbility(target.Position);
        }
        else
        {
            result = base.UseAbility();
        }

        if (result)
        {
            CastSleeper.Sleep(100);
        }

        return result;
    }

    public override bool UseAbility(Tree target)
    {
        if (CastSleeper.Sleeping || IsInAbilityPhase)
        {
            return false;
        }

        var result = base.UseAbility(target);
        if (result)
        {
            CastSleeper.Sleep(100);
        }

        return result;
    }

    public override bool UseAbility(Vector3 position)
    {
        if (CastSleeper.Sleeping || IsInAbilityPhase)
        {
            return false;
        }

        bool result;
        if ((AbilityBehavior & AbilityBehavior.Point) == AbilityBehavior.Point)
        {
            result = base.UseAbility(position);
        }
        else
        {
            result = base.UseAbility();
        }

        if (result)
        {
            CastSleeper.Sleep(100);
        }

        return result;
    }

    public override bool UseAbility(Rune target)
    {
        if (CastSleeper.Sleeping || IsInAbilityPhase)
        {
            return false;
        }

        var result = base.UseAbility(target);
        if (result)
        {
            CastSleeper.Sleep(100);
        }

        return result;
    }
}