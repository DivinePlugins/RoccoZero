namespace Ensage.SDK.Prediction;

using System;
using System.Collections.Generic;
using System.Linq;

using Divine.Entity;
using Divine.Entity.Entities;
using Divine.Entity.Entities.EventArgs;
using Divine.Entity.Entities.Units;
using Divine.Entity.Entities.Units.Buildings;
using Divine.Entity.Entities.Units.Creeps;
using Divine.Extensions;
using Divine.Game;
using Divine.Projectile;
using Divine.Projectile.EventArgs;
using Divine.Update;

public sealed class HealthPrediction
{
    private readonly Dictionary<uint, CreepStatus> CreepStatuses = new();

    private readonly Unit Owner = EntityManager.LocalHero;

    public HealthPrediction()
    {
        UpdateManager.CreateIngameUpdate(1000, OnUpdate);
        ProjectileManager.TrackingProjectileAdded += OnTrackingProjectile;
        Entity.AnimationChanged += OnAnimationChanged;
        Entity.NetworkPropertyChanged += OnNetworkPropertyChanged;
    }

    public void Dispose()
    {
        UpdateManager.DestroyIngameUpdate(OnUpdate);
        ProjectileManager.TrackingProjectileAdded -= OnTrackingProjectile;
        Entity.AnimationChanged -= OnAnimationChanged;
        Entity.NetworkPropertyChanged -= OnNetworkPropertyChanged;
    }

    public float GetPrediction(Unit unit, float untilTime)
    {
        var now = GameManager.RawGameTime;
        var health = (float)unit.Health;
        untilTime = Math.Max(0f, untilTime);
        untilTime = now + untilTime;

        var handle = unit.Handle;
        var team = unit.Team;

        foreach (var pair in CreepStatuses.Where(e => e.Value.IsValid && e.Value.Team != team && e.Value.Target?.Handle == handle))
        {
            var entry = pair.Value;
            var damage = entry.Source.GetAttackDamage(unit, true);

            float attackHitTime;

            if (entry.LastAttackAnimationTime == 0f || (now - entry.LastAttackAnimationTime) > (entry.TimeBetweenAttacks + 0.2))
            {
                continue;
            }

            if (entry.IsMelee)
            {
                // melee creeps
                attackHitTime = entry.LastAttackAnimationTime + entry.AttackPoint;
            }
            else
            {
                // ranged creeps
                attackHitTime = (entry.LastAttackAnimationTime - entry.TimeBetweenAttacks) + entry.GetAutoAttackArrivalTime(unit);
            }

            // delete next line and improve prediction if Stop() will be used to cancel auto attack :broscience:
            attackHitTime -= 0.05f;

            while (attackHitTime <= untilTime)
            {
                if (attackHitTime > now)
                {
                    health -= damage;
                }

                attackHitTime += entry.TimeBetweenAttacks;
            }
        }

        if (health > 0f)
        {
            // towers
            var closestTower = EntityManager.GetEntities<Tower>().OrderBy(tower => tower.Distance2D(Owner)).FirstOrDefault();
            if (closestTower != null)
            {
                var towerTarget = closestTower.AttackTarget;
                if (towerTarget != null && towerTarget == unit)
                {
                    var creepStatus = GetCreepStatusEntry(closestTower);
                    var damage = closestTower.GetAttackDamage(unit);
                    var attackHitTime = (creepStatus.LastAttackAnimationTime - creepStatus.TimeBetweenAttacks) + creepStatus.GetAutoAttackArrivalTime(unit);

                    while (attackHitTime <= untilTime)
                    {
                        if (attackHitTime > now)
                        {
                            health -= damage;
                        }

                        attackHitTime += creepStatus.TimeBetweenAttacks;
                    }
                }
            }
        }

        return health;
    }

    public bool ShouldWait(float t = 2f)
    {
        return EntityManager.GetEntities<Creep>()
            .Any(x => x.Team != Owner.Team && Owner.IsValidOrbwalkingTarget(x) && GetPrediction(x, t / Owner.AttacksPerSecond) < Owner.GetAttackDamage(x, true));
    }

    private CreepStatus GetCreepStatusEntry(Unit source)
    {
        var handle = source.Handle;

        if (!CreepStatuses.ContainsKey(handle))
        {
            CreepStatuses.Add(handle, new CreepStatus(source));
        }

        return CreepStatuses[handle];
    }

    private void OnAnimationChanged(Entity sender, AnimationChangedEventArgs e)
    {
        var creep = sender as Creep;

        if (creep == null)
        {
            return;
        }

        if (Owner.Distance2D(creep) > 3000)
        {
            return;
        }

        if (creep.IsNeutral)
        {
            return;
        }

        if (!e.Name.Contains("attack", StringComparison.InvariantCultureIgnoreCase))
        {
            return;
        }

        var creepStatus = GetCreepStatusEntry(creep);
        creepStatus.LastAttackAnimationTime = GameManager.RawGameTime - (GameManager.Ping / 2000f);
    }

    private void OnNetworkPropertyChanged(Entity sender, NetworkPropertyChangedEventArgs e)
    {
        if (!e.PropertyName.Equals("m_htowerattacktarget", StringComparison.InvariantCultureIgnoreCase))
        {
            return;
        }

        UpdateManager.BeginInvoke(() =>
        {
            var tower = sender as Tower;
            if (tower == null)
            {
                return;
            }

            if (Owner.Distance2D(tower) > 3000)
            {
                return;
            }

            var creepStatus = GetCreepStatusEntry(tower);
            creepStatus.LastAttackAnimationTime = GameManager.RawGameTime - (GameManager.Ping / 2000f);
        });
    }

    private void OnTrackingProjectile(TrackingProjectileAddedEventArgs e)
    {
        var sourceCreep = e.Projectile.Source as Creep;
        var sourceTower = e.Projectile.Source as Tower;

        if (sourceCreep != null)
        {
            if (Owner.Distance2D(sourceCreep) > 3000)
            {
                return;
            }

            if (sourceCreep.IsNeutral)
            {
                return;
            }

            var creepStatus = GetCreepStatusEntry(sourceCreep);
            creepStatus.Target = e.Projectile.Target as Creep;
        }
        else if (sourceTower != null)
        {
            if (Owner.Distance2D(sourceTower) > 3000)
            {
                return;
            }

            var creepStatus = GetCreepStatusEntry(sourceTower);
            creepStatus.LastAttackAnimationTime = GameManager.RawGameTime - creepStatus.AttackPoint - (GameManager.Ping / 2000f);
        }
    }

    private void OnUpdate()
    {
        var toRemove = CreepStatuses.Where(pair => !pair.Value.IsValid || Owner.Distance2D(pair.Value.Source) > 4000).ToList();

        foreach (var remove in toRemove)
        {
            CreepStatuses.Remove(remove.Key);
        }
    }
}