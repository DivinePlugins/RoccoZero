namespace Ensage.SDK.Prediction;

using System.Linq;

using Divine.Entity;
using Divine.Entity.Entities.Components;
using Divine.Entity.Entities.Units;
using Divine.Entity.Entities.Units.Buildings;
using Divine.Entity.Entities.Units.Creeps;
using Divine.Extensions;

internal sealed class CreepStatus
{
    private float _attackPoint;

    private bool _isMelee;

    private bool _isMeleeCached;

    private bool _isSiege;

    private bool _isSiegeCached;

    private bool _isTower;

    private bool _isTowerCached;

    private bool _isValid = true;

    private Unit _lastPossibleTarget;

    private float _missileSpeed;

    private Unit _target;

    private Team _team = Team.Undefined;

    private float _timeBetweenAttacks;

    public CreepStatus(Unit source)
    {
        Source = source;
    }

    public float AttackPoint
    {
        get
        {
            if (_attackPoint == 0f)
            {
                _attackPoint = (float)Source.AttackPoint();
            }

            return _attackPoint;
        }
    }

    public bool IsMelee
    {
        get
        {
            if (!_isMeleeCached)
            {
                _isMelee = Source.IsMelee;
                _isMeleeCached = true;
            }

            return _isMelee;
        }
    }

    public bool IsSiege
    {
        get
        {
            if (!_isSiegeCached)
            {
                _isSiege = Source.Name.Contains("siege");
                _isSiegeCached = true;
            }

            return _isSiege;
        }
    }

    public bool IsTower
    {
        get
        {
            if (!_isTowerCached)
            {
                _isTower = Source as Tower != null;
                _isTowerCached = true;
            }

            return _isTower;
        }
    }

    public bool IsValid
    {
        get
        {
            if (!_isValid)
            {
                return false;
            }

            if (!Source.IsValid || !Source.IsAlive)
            {
                return false;
            }

            return _isValid;
        }
    }

    public float LastAttackAnimationTime { get; set; }

    public float MissileSpeed
    {
        get
        {
            if (_missileSpeed == 0f)
            {
                if (IsMelee)
                {
                    _missileSpeed = float.MaxValue;
                }
                else
                {
                    _missileSpeed = (float)Source.ProjectileSpeed();
                }
            }

            return _missileSpeed;
        }
    }

    public Unit Source { get; set; }

    public Unit Target
    {
        get
        {
            if (_target != null && _target.IsValid && Source.IsValidOrbwalkingTarget(_target))
            {
                return _target;
            }

            if (_lastPossibleTarget != null &&
                _lastPossibleTarget.IsValid &&
                Source.IsDirectlyFacing(_lastPossibleTarget) &&
                Source.IsValidOrbwalkingTarget(_lastPossibleTarget))
            {
                return _lastPossibleTarget;
            }

            var possibleTarget = EntityManager.GetEntities<Creep>().FirstOrDefault(
                unit => unit.IsValid &&
                        unit.Team != Team &&
                        Source.IsDirectlyFacing(unit) &&
                        Source.IsValidOrbwalkingTarget(unit));

            if (possibleTarget != null)
            {
                _lastPossibleTarget = possibleTarget;
                return possibleTarget;
            }

            return null;
        }

        set
        {
            _target = value;
        }
    }

    public Team Team
    {
        get
        {
            if (_team == Team.Undefined)
            {
                _team = Source.Team;
            }

            return _team;
        }
    }

    public float TimeBetweenAttacks
    {
        get
        {
            if (_timeBetweenAttacks == 0f)
            {
                _timeBetweenAttacks = 1 / Source.AttacksPerSecond;
            }

            return _timeBetweenAttacks;
        }
    }

    public float GetAutoAttackArrivalTime(Unit target)
    {
        var result = Source.GetProjectileArrivalTime(target, AttackPoint, MissileSpeed, !IsTower);

        if (IsTower)
        {
            result += 0.15f;
        }
        else
        {
            result -= 0.10f;
        }

        return result;
    }
}