using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using BAIO.Core.Handlers;
using BAIO.Heroes.Base;
using BAIO.Modes;

using Divine.Entity.Entities.Abilities.Components;
using Divine.Entity.Entities.Units.Heroes;
using Divine.Extensions;
using Divine.Zero.Log;

namespace BAIO.Heroes.Modes.Combo
{
    internal class BristlebackCombo : ComboMode
    {
        private readonly Bristleback Bristleback;

        public TaskHandler TurnTheOtherCheekHandler { get; private set; }

        public BristlebackCombo(Bristleback hero)
            : base(hero)
        {
            this.Bristleback = hero;
           // this.TurnTheOtherCheekHandler = TaskHandler.Run(OnUpdate);
        }

        public override async Task ExecuteAsync(CancellationToken token)
        {
            if (!await this.ShouldExecute(token))
            {
                return;
            }

            this.MaxTargetRange = Math.Max(this.MaxTargetRange, 700);

            if ((this.CurrentTarget == null) || !this.CurrentTarget.IsVisible)
            {
                this.Bristleback.Context.Orbwalker.OrbwalkTo(null);
                return;
            }

            if (this.CurrentTarget.IsIllusion)
            {
                this.OrbwalkToTarget();
                return;
            }

            try
            {
                var linkens = this.CurrentTarget.IsLinkensProtected();
                await BreakLinken(token);

                var spray = this.Bristleback.QuillSpray;
                if (spray.CanBeCasted && this.Owner.Distance2D(this.CurrentTarget) <= 700)
                {
                    if (spray.Cast())
                    {
                        await Task.Delay(spray.GetCastDelay() + 55, token);
                    }
                }

                var nasal = this.Bristleback.NasalGoo;
                if (nasal.CanBeCasted && this.Owner.HasAghanimsScepter()
                    ? this.Owner.Distance2D(this.CurrentTarget) <= nasal.Radius
                    : this.Owner.Distance2D(this.CurrentTarget) <= nasal.CastRange)
                {
                    if (this.Owner.HasAghanimsScepter() ? nasal.Cast() : nasal.Cast(CurrentTarget))
                    {
                        await Task.Delay(nasal.GetCastDelay(this.Owner.HasAghanimsScepter() ? null : this.CurrentTarget),
                            token);
                    }
                }

                var abyssal = this.Bristleback.AbyssalBlade;
                if (abyssal != null && abyssal.CanBeCasted && abyssal.CanHit(CurrentTarget) && !linkens &&
                    this.Bristleback.AbyssalBladeHeroes[((Hero)CurrentTarget).HeroId])
                {
                    if (abyssal.Cast(CurrentTarget))
                    {
                        await Task.Delay(abyssal.GetCastDelay(), token);
                    }
                }

                var nullifier = this.Bristleback.Nullifier;
                if (nullifier != null && nullifier.CanBeCasted && nullifier.CanHit(CurrentTarget) && !linkens &&
                    this.Bristleback.NullifierHeroes[((Hero)CurrentTarget).HeroId])
                {
                    if (nullifier.Cast(CurrentTarget))
                    {
                        await Task.Delay(nullifier.GetCastDelay(), token);
                    }
                }
            }
            catch (TaskCanceledException)
            {
                // ignore
            }
            catch (Exception e)
            {
                LogManager.Debug($"{e}");
            }
            this.OrbwalkToTarget();
        }

        /*private async Task OnUpdate(CancellationToken token)
        {
            if (GameManager.IsPaused || !this.Bristleback.TurnTheOtherCheek.Value)
            {
                await Task.Delay(250, token);
                return;
            }

            var enemies =
                EntityManager.GetEntities<Hero>().Where(
                        x =>
                            x.IsValid && x.IsAlive && !x.IsIllusion && x.Team != this.Owner.Team &&
                            x.Distance2D(this.Owner) <= x.GetAttackRange() + 100)
                    .ToList();

            if (enemies.Any())
            {
                foreach (var enemy in enemies.OrderByDescending(x => UnitExtensions.GetAttackDamage(x, this.Owner)))
                {
                    var enemyInFront = UnitExtensions.InFront(enemy, enemy.AttackRange + 50);
                    var pos = enemyInFront.Extend(this.Owner.Position, 10);

                    this.Owner.MoveToDirection(pos);
                    this.Owner.Stop();
                    await Task.Delay(300, token);

                    Context.Particle.AddOrUpdate(
                        Owner,
                        "pos",
                        "particles/ui_mouseactions/drag_selected_ring.vpcf",
                        ParticleAttachment.AbsOrigin,
                        RestartType.None,
                        0,
                        pos,
                        1,
                        Color.Aqua,
                        2,
                        50 * 1.1f);

                    var spray = this.Bristleback.QuillSpray;
                    if (spray.CanBeCasted && this.Owner.Distance2D(enemy) <= 700)
                    {
                        spray.Cast();
                        await Task.Delay(spray.GetCastDelay() + 55, token);
                    }
                }
            }
            await Task.Delay(100, token);
        }*/

        protected async Task BreakLinken(CancellationToken token)
        {
            if (this.CurrentTarget != null && this.CurrentTarget.IsValid)
            {
                try
                {
                    List<KeyValuePair<AbilityId, bool>> breakerChanger = new List<KeyValuePair<AbilityId, bool>>();

                    if (this.CurrentTarget.IsLinkensProtected())
                    {
                        breakerChanger = this.Bristleback.Config.Hero.LinkenBreakerTogglerMenu.Values.ToList();
                    }

                    foreach (var order in breakerChanger)
                    {
                        var euls = this.Bristleback.Euls;
                        if (euls != null
                            && euls.Item.Id == order.Key
                            && euls.CanBeCasted && euls.CanHit(this.CurrentTarget))
                        {
                            euls.Cast(this.CurrentTarget);
                            await Task.Delay(euls.GetCastDelay(this.CurrentTarget), token);
                            return;
                        }

                        var force = this.Bristleback.ForceStaff;
                        if (force != null
                            && force.Item.Id == order.Key
                            && force.CanBeCasted && force.CanHit(this.CurrentTarget))
                        {
                            force.Cast(this.CurrentTarget);
                            await Task.Delay(force.GetCastDelay(this.CurrentTarget), token);
                            return;
                        }

                        var orchid = this.Bristleback.Orchid;
                        if (orchid != null
                            && orchid.Item.Id == order.Key
                            && orchid.CanBeCasted && orchid.CanHit(this.CurrentTarget))
                        {
                            orchid.Cast(this.CurrentTarget);
                            await Task.Delay(orchid.GetCastDelay(this.CurrentTarget), token);
                            return;
                        }

                        var bloodthorn = this.Bristleback.BloodThorn;
                        if (bloodthorn != null
                            && bloodthorn.Item.Id == order.Key
                            && bloodthorn.CanBeCasted && bloodthorn.CanHit(this.CurrentTarget))
                        {
                            bloodthorn.Cast(this.CurrentTarget);
                            await Task.Delay(bloodthorn.GetCastDelay(this.CurrentTarget), token);
                            return;
                        }

                        var nullifier = this.Bristleback.Nullifier;
                        if (nullifier != null
                            && nullifier.Item.Id == order.Key
                            && nullifier.CanBeCasted && nullifier.CanHit(this.CurrentTarget))
                        {
                            nullifier.Cast(this.CurrentTarget);
                            await Task.Delay(
                                nullifier.GetCastDelay(this.CurrentTarget) + nullifier.GetHitTime(this.CurrentTarget),
                                token);
                            return;
                        }

                        var atos = this.Bristleback.RodOfAtos;
                        if (atos != null
                            && atos.Item.Id == order.Key
                            && atos.CanBeCasted && atos.CanHit(this.CurrentTarget))
                        {
                            atos.Cast(this.CurrentTarget);
                            await Task.Delay(
                                atos.GetCastDelay(this.CurrentTarget) + atos.GetHitTime(this.CurrentTarget), token);
                            return;
                        }

                        var hex = this.Bristleback.Sheepstick;
                        if (hex != null
                            && hex.Item.Id == order.Key
                            && hex.CanBeCasted && hex.CanHit(this.CurrentTarget))
                        {
                            hex.Cast(this.CurrentTarget);
                            await Task.Delay(hex.GetCastDelay(this.CurrentTarget), token);
                            return;
                        }

                        var diff = this.Bristleback.DiffusalBlade;
                        if (diff != null
                            && diff.Item.Id == order.Key
                            && diff.CanBeCasted && diff.CanHit(this.CurrentTarget))
                        {
                            diff.Cast(this.CurrentTarget);
                            await Task.Delay(diff.GetCastDelay(this.CurrentTarget), token);
                            return;
                        }
                    }
                }
                catch (TaskCanceledException)
                {
                    // ignore
                }
                catch (Exception e)
                {
                    LogManager.Error("Linken break error: " + e);
                }
            }
        }
    }
}