namespace BAIO.Heroes.Modes.Combo
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using BAIO.Heroes.Base;
    using BAIO.Modes;

    using Divine.Entity;
    using Divine.Entity.Entities;
    using Divine.Entity.Entities.Abilities.Components;
    using Divine.Entity.Entities.EventArgs;
    using Divine.Entity.Entities.Units.Heroes;
    using Divine.Extensions;
    using Divine.Game;
    using Divine.Numerics;
    using Divine.Prediction;
    using Divine.Update;
    using Divine.Zero.Log;

    internal class ClockwerkCombo : ComboMode
    {
        private readonly Clockwerk Clockwerk;

        private Vector3 hookPredictionPos;

        private float hookStartCastTime;

        private bool hookHit;

        private readonly UpdateHandler ultiUpdateHandler;

        public ClockwerkCombo(Clockwerk hero)
            : base(hero)
        {
            this.Clockwerk = hero;
            Entity.NetworkPropertyChanged += this.OnHookCast;
            this.ultiUpdateHandler = UpdateManager.CreateIngameUpdate(0, false, this.HookHitCheck);
        }

        public override async Task ExecuteAsync(CancellationToken token)
        {
            if (!await this.ShouldExecute(token))
            {
                return;
            }

            this.MaxTargetRange = Math.Max(this.MaxTargetRange, Clockwerk.Ulti.CastRange * 1.1f);

            if ((this.CurrentTarget == null) || !this.CurrentTarget.IsVisible)
            {
                this.Clockwerk.Context.Orbwalker.OrbwalkTo(null);
                return;
            }

            if (this.CurrentTarget.IsIllusion)
            {
                this.OrbwalkToTarget();
                return;
            }

            var linkens = this.CurrentTarget.IsLinkensProtected();
            await BreakLinken(token);

            var atos = this.Clockwerk.RodOfAtos;
            if (Clockwerk.Ulti.CanBeCasted && Clockwerk.Ulti.CanHit(this.CurrentTarget) && !this.CurrentTarget.IsInvulnerable())
            {
                if (atos?.CanBeCasted == true && atos.CanHit(this.CurrentTarget) && !this.CurrentTarget.IsStunned() && !linkens &&
                    !this.CurrentTarget.IsRooted())
                {
                    var hookPreAtosInput = Clockwerk.Ulti.GetPredictionInput(this.CurrentTarget);
                    var hookPreAtosOutput = Clockwerk.Ulti.GetPredictionOutput(hookPreAtosInput);

                    if ((hookPreAtosOutput.HitChance != HitChance.OutOfRange) &&
                        (hookPreAtosOutput.HitChance != HitChance.Collision))
                    {
                        atos.Cast(this.CurrentTarget);
                        await Task.Delay(atos.GetHitTime(this.CurrentTarget), token);
                    }
                }

                var hookInput = Clockwerk.Ulti.GetPredictionInput(this.CurrentTarget);
                var hookOutput = Clockwerk.Ulti.GetPredictionOutput(hookInput);
                if (this.CanUltiHit(hookOutput))
                {
                    this.hookPredictionPos = hookOutput.UnitPosition;
                    Clockwerk.Ulti.Cast(this.hookPredictionPos);
                    await Task.Delay(Clockwerk.Ulti.GetHitTime(this.hookPredictionPos), token);
                }
            }

            var insec = this.Clockwerk.Insec.Value;
            if (Clockwerk.Cogs.CanBeCasted && this.Owner.Distance2D(CurrentTarget) <= 170)
            {
                if (insec && (this.CurrentTarget.IsStunned() || this.CurrentTarget.IsRooted()))
                {
                    var teammates =
                        EntityManager.GetEntities<Hero>().Where(
                                x => x.IsValid && x.Team == this.Owner.Team && x.Distance2D(this.Owner) <= 800)
                            .OrderBy(x => x.Distance2D(this.Owner)).FirstOrDefault();

                    if (teammates != null)
                    {
                        var pos = teammates.Position.Extend(CurrentTarget.Position,
                            teammates.Distance2D(CurrentTarget) + 400);
                        this.Owner.Move(pos);
                        await Task.Delay(GetMoveDelay(pos), token);
                        this.Clockwerk.Cogs.Cast();
                        await Task.Delay(Clockwerk.Cogs.GetCastDelay(), token);
                    }
                }
                else
                {
                    this.Clockwerk.Cogs.Cast();
                    await Task.Delay(Clockwerk.Cogs.GetCastDelay(), token);
                }
            }

            if (Clockwerk.BatteryAssault.CanBeCasted &&
                (this.Clockwerk.IsHookModifier || this.Owner.Distance2D(CurrentTarget) <= 200))
            {
                this.Clockwerk.BatteryAssault.Cast();
                await Task.Delay(Clockwerk.BatteryAssault.GetCastDelay(), token);
            }
            if (!insec || !Clockwerk.Cogs.CanBeCasted)
            {
                var bladeMail = this.Clockwerk.BladeMail;
                if (bladeMail != null && bladeMail.CanBeCasted && this.Owner.Distance2D(CurrentTarget) <= 200 && this.Clockwerk.BladeMailUsage)
                {
                    bladeMail.Cast();
                    await Task.Delay(bladeMail.GetCastDelay(), token);
                }

                var lotusOrb = this.Clockwerk.Lotus;
                if (lotusOrb != null && lotusOrb.CanBeCasted && this.Owner.Distance2D(CurrentTarget) <= 200 && this.Clockwerk.LotusUsage)
                {
                    lotusOrb.Cast(this.Owner);
                    await Task.Delay(lotusOrb.GetCastDelay(), token);
                }

                var solar = this.Clockwerk.SolarCrest;
                if (solar != null && solar.CanBeCasted && solar.CanHit(CurrentTarget))
                {
                    solar.Cast(this.CurrentTarget);
                    await Task.Delay(solar.GetCastDelay(), token);
                }

                var vessel = this.Clockwerk.Vessel;
                var urn = this.Clockwerk.Urn;
                if ((urn?.CanBeCasted == true && this.Clockwerk.UrnHeroes[((Hero)CurrentTarget).HeroId]
                    || vessel?.CanBeCasted == true && this.Clockwerk.VesselHeroes[((Hero)CurrentTarget).HeroId])
                    && (this.Clockwerk.IsHookModifier || this.Owner.Distance2D(this.CurrentTarget) < 300) && !linkens)
                {
                    urn?.Cast(this.CurrentTarget);
                    vessel?.Cast(this.CurrentTarget);
                }

                var nullifier = this.Clockwerk.Nullifier;
                if (nullifier != null && nullifier.CanBeCasted && nullifier.CanHit(CurrentTarget) && !linkens &&
                    this.Clockwerk.NullifierHeroes[((Hero)CurrentTarget).HeroId])
                {
                    nullifier.Cast(CurrentTarget);
                    await Task.Delay(nullifier.GetCastDelay(), token);
                }

                var halberd = this.Clockwerk.HeavensHalberd;
                if (halberd != null && halberd.CanBeCasted && halberd.CanHit(CurrentTarget) && !linkens &&
                    this.Clockwerk.HalberdHeroes[((Hero)CurrentTarget).HeroId])
                {
                    halberd.Cast(CurrentTarget);
                    await Task.Delay(halberd.GetCastDelay(), token);
                }

                var orchid = this.Clockwerk.Orchid;
                if (orchid != null && orchid.CanBeCasted && orchid.CanHit(CurrentTarget) && !linkens &&
                    this.Clockwerk.OrchidHeroes[((Hero)CurrentTarget).HeroId])
                {
                    orchid.Cast(CurrentTarget);
                    await Task.Delay(orchid.GetCastDelay(), token);
                }

                var bt = this.Clockwerk.BloodThorn;
                if (bt != null && bt.CanBeCasted && bt.CanHit(CurrentTarget) && !linkens &&
                    this.Clockwerk.OrchidHeroes[((Hero)CurrentTarget).HeroId])
                {
                    bt.Cast(CurrentTarget);
                    await Task.Delay(bt.GetCastDelay(), token);
                }
            }
            this.OrbwalkToTarget();
        }

        private int GetMoveDelay(Vector3 pos)
        {
            return (int) (((this.Owner.Distance2D(pos) / this.Owner.MovementSpeed) * 1000.0) + GameManager.Ping);
        }

        private bool CanUltiHit(PredictionOutput output)
        {
            switch (output.HitChance)
            {
                case HitChance.OutOfRange:
                    return false;
                case HitChance.Impossible:
                    return false;
                case HitChance.Collision:
                    return false;
                case HitChance.Immobile:
                    break;
                case HitChance.Dashing:
                    break;
                case HitChance.VeryHigh:
                    break;
                case HitChance.High:
                    break;
                case HitChance.Medium:
                    break;
                case HitChance.Low:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return output.HitChance >= this.Clockwerk.MinimumHookChance;
        }

        private void OnHookCast(Entity sender, NetworkPropertyChangedEventArgs e)
        {
            if (e.PropertyName != "m_bInAbilityPhase")
            {
                return;
            }

            var newValue = e.NewValue.GetBoolean();
            if (newValue == e.OldValue.GetBoolean())
            {
                return;
            }

            UpdateManager.BeginInvoke(() =>
            {
                if (sender != this.Clockwerk.Ulti)
                {
                    return;
                }

                if (newValue)
                {
                    this.hookHit = true;
                    this.hookStartCastTime = GameManager.RawGameTime;
                    this.ultiUpdateHandler.IsEnabled = true;
                }
                else
                {
                    this.hookHit = false;
                    this.ultiUpdateHandler.IsEnabled = false;
                }
            });
        }

        private void HookHitCheck()
        {
            if (this.CurrentTarget == null || !this.CurrentTarget.IsVisible)
            {
                return;
            }

            var hook = this.Clockwerk.Ulti;
            var input = hook.GetPredictionInput(this.CurrentTarget);
            input.Delay = Math.Max((this.hookStartCastTime - GameManager.RawGameTime) + hook.CastPoint, 0);
            var output = hook.GetPredictionOutput(input);

            if (this.hookPredictionPos.Distance2D(output.UnitPosition) > hook.Radius || !this.CanUltiHit(output))
            {
                this.Owner.Stop();
                this.Cancel();
                this.ultiUpdateHandler.IsEnabled = false;
            }
        }

        protected async Task BreakLinken(CancellationToken token)
        {
            if (this.CurrentTarget != null && this.CurrentTarget.IsValid)
            {
                try
                {
                    List<KeyValuePair<AbilityId, bool>> breakerChanger = new List<KeyValuePair<AbilityId, bool>>();

                    if (this.CurrentTarget.IsLinkensProtected())
                    {
                        breakerChanger = this.Clockwerk.Config.Hero.LinkenBreakerTogglerMenu.Values.ToList();
                    }

                    foreach (var order in breakerChanger)
                    {
                        var euls = this.Clockwerk.Euls;
                        if (euls != null
                            && euls.Item.Id == order.Key
                            && euls.CanBeCasted && euls.CanHit(this.CurrentTarget))
                        {
                            euls.Cast(this.CurrentTarget);
                            await Task.Delay(euls.GetCastDelay(this.CurrentTarget), token);
                            return;
                        }

                        var force = this.Clockwerk.ForceStaff;
                        if (force != null
                            && force.Item.Id == order.Key
                            && force.CanBeCasted && force.CanHit(this.CurrentTarget))
                        {
                            force.Cast(this.CurrentTarget);
                            await Task.Delay(force.GetCastDelay(this.CurrentTarget), token);
                            return;
                        }

                        var orchid = this.Clockwerk.Orchid;
                        if (orchid != null
                            && orchid.Item.Id == order.Key
                            && orchid.CanBeCasted && orchid.CanHit(this.CurrentTarget))
                        {
                            orchid.Cast(this.CurrentTarget);
                            await Task.Delay(orchid.GetCastDelay(this.CurrentTarget), token);
                            return;
                        }

                        var bloodthorn = this.Clockwerk.BloodThorn;
                        if (bloodthorn != null
                            && bloodthorn.Item.Id == order.Key
                            && bloodthorn.CanBeCasted && bloodthorn.CanHit(this.CurrentTarget))
                        {
                            bloodthorn.Cast(this.CurrentTarget);
                            await Task.Delay(bloodthorn.GetCastDelay(this.CurrentTarget), token);
                            return;
                        }

                        var nullifier = this.Clockwerk.Nullifier;
                        if (nullifier != null
                            && nullifier.Item.Id == order.Key
                            && nullifier.CanBeCasted && nullifier.CanHit(this.CurrentTarget))
                        {
                            nullifier.Cast(this.CurrentTarget);
                            await Task.Delay(nullifier.GetCastDelay(this.CurrentTarget) + nullifier.GetHitTime(this.CurrentTarget), token);
                            return;
                        }

                        var atos = this.Clockwerk.RodOfAtos;
                        if (atos != null
                            && atos.Item.Id == order.Key
                            && atos.CanBeCasted && atos.CanHit(this.CurrentTarget))
                        {
                            atos.Cast(this.CurrentTarget);
                            await Task.Delay(atos.GetCastDelay(this.CurrentTarget) + atos.GetHitTime(this.CurrentTarget), token);
                            return;
                        }

                        var hex = this.Clockwerk.Sheepstick;
                        if (hex != null
                            && hex.Item.Id == order.Key
                            && hex.CanBeCasted && hex.CanHit(this.CurrentTarget))
                        {
                            hex.Cast(this.CurrentTarget);
                            await Task.Delay(hex.GetCastDelay(this.CurrentTarget), token);
                            return;
                        }

                        var diff = this.Clockwerk.DiffusalBlade;
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