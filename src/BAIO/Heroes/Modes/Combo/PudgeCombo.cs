namespace BAIO.Heroes.Modes.Combo
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using BAIO.Core.Handlers;
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

    internal class PudgeCombo : ComboMode
    {
        private readonly Pudge Pudge;

        private readonly UpdateHandler hookUpdateHandler;

        private Vector3 hookCastPosition;

        private float hookStartCastTime;

        private TaskHandler ComboHandler;

        public PudgeCombo(Pudge hero)
            : base(hero)
        {
            this.Pudge = hero;
            Entity.NetworkPropertyChanged += this.OnHookCast;
            this.hookUpdateHandler = UpdateManager.CreateIngameUpdate(0, false, this.HookHitCheck);
            this.ComboHandler = TaskHandler.Run(this.HookAllies);
        }

        protected override void OnDeactivate()
        {
            Entity.NetworkPropertyChanged -= this.OnHookCast;
            this.ComboHandler.Cancel();
            UpdateManager.DestroyIngameUpdate(this.hookUpdateHandler);

            base.OnDeactivate();
        }

        public override async Task ExecuteAsync(CancellationToken token)
        {
            if (!await this.ShouldExecute(token))
            {
                return;
            }

            var hook = this.Pudge.Hook;
            var blink = this.Pudge.BlinkDagger;
            var atos = this.Pudge.RodOfAtos;
            var force = this.Pudge.ForceStaff;

            this.MaxTargetRange = Math.Max(this.MaxTargetRange, hook.CastRange * 1.1f);
            if (blink != null && this.Pudge.BlinkCombo)
            {
                this.MaxTargetRange = Math.Max(this.MaxTargetRange, blink.CastRange * 1.3f);

                if (force != null)
                {
                    this.MaxTargetRange = Math.Max(this.MaxTargetRange, (blink.CastRange * 1.3f) + (500));
                }
            }

            if (atos != null)
            {
                this.MaxTargetRange = Math.Max(this.MaxTargetRange, atos.CastRange * 1.1f);
            }

            if (this.CurrentTarget == null || !this.CurrentTarget.IsVisible && !this.Pudge.KeepPosition.Value)
            {
                this.Pudge.Context.Orbwalker.OrbwalkTo(null);
                return;
            }

            if (this.CurrentTarget.IsIllusion && !this.Pudge.KeepPosition.Value)
            {
                this.OrbwalkToTarget();
                return;
            }

            var rot = this.Pudge.Rot;
            var ult = this.Pudge.Dismember;

            var forceStaff = this.Pudge.ForceStaff;
            var forceStaffReady = (forceStaff != null) && forceStaff.CanBeCasted && this.Pudge.BlinkCombo;
            var blinkReady = (blink != null) && blink.CanBeCasted && this.Pudge.BlinkCombo;

            var linkens = this.CurrentTarget.IsLinkensProtected();
            await BreakLinken(token);

            if (blinkReady && this.Owner.Distance2D(this.CurrentTarget) > 600 && !this.Pudge.HookModifierDetected)
            {
                var distance = this.Owner.Distance2D(this.CurrentTarget);
                var blinkPosition = this.CurrentTarget.Position.Extend(this.Owner.Position,
                    Math.Max(100, distance - blink.CastRange));
                blink.Cast(blinkPosition);

                if (ult.CanBeCasted && blinkPosition.Distance2D(this.CurrentTarget.Position) <= ult.CastRange)
                {
                    rot.Enabled = true;

                    if (!linkens)
                    {
                        ult.Cast(this.CurrentTarget);
                        await Task.Delay(ult.GetCastDelay(this.CurrentTarget) + 500, token);
                    }
                    else
                    {
                        await BreakLinken(token);
                        return;
                    }
                }
                else
                {
                    await Task.Delay(blink.GetCastDelay(this.CurrentTarget), token);
                }
            }

            if (forceStaffReady && this.Owner.Distance2D(this.CurrentTarget) > 500 &&
                !this.CurrentTarget.IsLinkensProtected())
            {
                if (this.Owner.FindRotationAngle(this.CurrentTarget.Position) > 0.3f)
                {
                    var turnPosition = this.Owner.Position.Extend(this.CurrentTarget.Position, 100);
                    this.Owner.Move(turnPosition);
                    await Task.Delay((int)(this.Owner.TurnTime(turnPosition) * 1000) + 200, token);
                }

                forceStaff.Cast(this.Owner);
                await Task.Delay((int)((forceStaff.PushLength / forceStaff.PushSpeed) * 1000), token);
            }

            var vessel = this.Pudge.Vessel;
            var urn = this.Pudge.Urn;
            if ((urn?.CanBeCasted == true && this.Pudge.UrnHeroes[((Hero)CurrentTarget).HeroId]
                 || vessel?.CanBeCasted == true && this.Pudge.VesselHeroes[((Hero)CurrentTarget).HeroId])
                && (this.Pudge.HookModifierDetected || this.Owner.Distance2D(this.CurrentTarget) < 300) && !linkens &&
                !ult.IsChanneling)
            {
                urn?.Cast(this.CurrentTarget);
                vessel?.Cast(this.CurrentTarget);
            }

            if (rot.CanBeCasted && !rot.Enabled && (this.Pudge.HookModifierDetected || rot.CanHit(this.CurrentTarget)))
            {
                rot.Enabled = true;
                await Task.Delay(rot.GetCastDelay(), token);
            }

            if (linkens)
            {
                await BreakLinken(token);
            }

            if (this.Pudge.EtherealBlade != null && !linkens && !ult.IsChanneling && this.Pudge.EtherealBlade.CanBeCasted &&
                this.Pudge.EbHeroes[((Hero)CurrentTarget).HeroId] &&
                (this.Pudge.EtherealBlade.CanHit(this.CurrentTarget) || this.Pudge.HookModifierDetected))
            {
                this.Pudge.EtherealBlade.Cast(this.CurrentTarget);
                await Task.Delay(this.Pudge.EtherealBlade.GetCastDelay(this.CurrentTarget), token);
            }

            if (this.Pudge.Dagon != null && Pudge.EtherealBlade != null && !linkens && !ult.IsChanneling && this.Pudge.Dagon.CanBeCasted &&
                (this.Pudge.Dagon.CanHit(this.CurrentTarget) || this.Pudge.HookModifierDetected))
            {
                this.Pudge.Dagon.Cast(this.CurrentTarget);
                await Task.Delay(this.Pudge.Dagon.GetCastDelay(this.CurrentTarget), token);
            }

            if (ult.CanBeCasted && (ult.CanHit(this.CurrentTarget) || this.Pudge.HookModifierDetected) && !linkens && this.Owner.Distance2D(this.CurrentTarget) <= 200)
            {
                ult.Cast(this.CurrentTarget);
                await Task.Delay(ult.GetCastDelay(this.CurrentTarget) + 500, token);
            }

            if (hook.CanBeCasted && !ult.IsChanneling)
            {
                if (this.CurrentTarget.HasModifier("modifier_eul_cyclone"))
                {
                    var remainingTime = this.CurrentTarget.GetModifierByName("modifier_eul_cyclone").RemainingTime;
                    if (remainingTime <= (float)(this.Owner.Distance2D(this.CurrentTarget) / hook.Speed) + (float)(hook.GetCastDelay(this.CurrentTarget.Position) / 1000f))
                    {
                        this.hookCastPosition = this.CurrentTarget.Position;
                        hook.Cast(this.hookCastPosition);
                        await Task.Delay(hook.GetHitTime(this.hookCastPosition), token);
                    }
                }
            }

            if (hook.CanBeCasted && hook.CanHit(this.CurrentTarget) && !this.CurrentTarget.IsInvulnerable() && !ult.IsChanneling)
            {
                if (atos?.CanBeCasted == true
                    && atos.CanHit(this.CurrentTarget)
                    && !this.CurrentTarget.IsStunned()
                    && !this.CurrentTarget.IsRooted())
                {
                    var hookPreAtosInput = hook.GetPredictionInput(this.CurrentTarget);
                    var hookPreAtosOutput = hook.GetPredictionOutput(hookPreAtosInput);

                    if ((hookPreAtosOutput.HitChance != HitChance.OutOfRange) &&
                        (hookPreAtosOutput.HitChance != HitChance.Collision))
                    {
                        atos.Cast(this.CurrentTarget);
                        await Task.Delay(atos.GetHitTime(this.CurrentTarget), token);
                    }
                }

                var hookInput = hook.GetPredictionInput(this.CurrentTarget);
                var hookOutput = hook.GetPredictionOutput(hookInput);

                if (this.ShouldCastHook(hookOutput))
                {
                    this.hookCastPosition = hookOutput.UnitPosition;
                    hook.Cast(this.hookCastPosition);
                    await Task.Delay(hook.GetHitTime(this.hookCastPosition), token);
                }
            }

            var channel = ult.IsChanneling;

            if (!channel)
            {
                var bladeMail = this.Pudge.BladeMail;
                if (bladeMail != null && bladeMail.CanBeCasted && this.Owner.Distance2D(CurrentTarget) <= 200 &&
                    this.Pudge.BladeMailUsage)
                {
                    bladeMail.Cast();
                    await Task.Delay(bladeMail.GetCastDelay(), token);
                }

                var lotusOrb = this.Pudge.Lotus;
                if (lotusOrb != null && lotusOrb.CanBeCasted && this.Owner.Distance2D(CurrentTarget) <= 200)
                {
                    lotusOrb.Cast(this.Owner);
                    await Task.Delay(lotusOrb.GetCastDelay(), token);
                }

                var solar = this.Pudge.SolarCrest;
                if (solar != null && solar.CanBeCasted && solar.CanHit(CurrentTarget))
                {
                    solar.Cast(this.CurrentTarget);
                    await Task.Delay(solar.GetCastDelay(), token);
                }

                var nullifier = this.Pudge.Nullifier;
                if (nullifier != null && nullifier.CanBeCasted && nullifier.CanHit(CurrentTarget) && !linkens &&
                    this.Pudge.NullifierHeroes[((Hero)CurrentTarget).HeroId])
                {
                    nullifier.Cast(CurrentTarget);
                    await Task.Delay(nullifier.GetCastDelay(), token);
                }

                var halberd = this.Pudge.HeavensHalberd;
                if (halberd != null && halberd.CanBeCasted && halberd.CanHit(CurrentTarget) && !linkens &&
                    this.Pudge.HalberdHeroes[((Hero)CurrentTarget).HeroId])
                {
                    halberd.Cast(CurrentTarget);
                    await Task.Delay(halberd.GetCastDelay(), token);
                }
            }

            if (!this.Pudge.KeepPosition.Value)
            {
                this.OrbwalkToTarget();
            }
        }

        private async Task HookAllies(CancellationToken token)
        {
            if (GameManager.IsPaused || !this.Owner.IsAlive || !this.Pudge.Hook.CanBeCasted)
            {
                await Task.Delay(250, token);
                return;
            }

            var hook = this.Pudge.Hook;
            var ult = this.Pudge.Dismember;


            var ally = EntityManager.GetEntities<Hero>()
                .Where(x => x != null && x.IsValid && x.IsAlive && x.IsAlly(this.Owner) && x.Handle != this.Owner.Handle &&
                            x.Distance2D(this.Owner) <= hook.CastRange)
                .OrderBy(y => y.Distance2D(GameManager.MousePosition)).FirstOrDefault();

            if (hook.CanBeCasted && ally != null && !ult.IsChanneling && this.Pudge.AllyHook)
            {
                var hookInput = hook.GetPredictionInput(ally);
                var hookOutput = hook.GetPredictionOutput(hookInput);

                if (this.ShouldCastHook(hookOutput))
                {
                    this.hookCastPosition = hookOutput.UnitPosition;
                    hook.Cast(this.hookCastPosition);
                    await Task.Delay(hook.GetHitTime(this.hookCastPosition), token);
                }
            }

            await Task.Delay(125, token);
        }

        private void HookHitCheck()
        {
            if (this.CurrentTarget == null || !this.CurrentTarget.IsVisible)
            {
                return;
            }

            var hook = this.Pudge.Hook;
            var input = hook.GetPredictionInput(this.CurrentTarget);
            input.Delay = Math.Max((this.hookStartCastTime - GameManager.RawGameTime) + hook.CastPoint, 0);
            var output = hook.GetPredictionOutput(input);

            if (this.hookCastPosition.Distance2D(output.UnitPosition) > hook.Radius || !this.ShouldCastHook(output))
            {
                this.Owner.Stop();
                this.Cancel();
                this.hookUpdateHandler.IsEnabled = false;
            }
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
                if (sender != this.Pudge.Hook)
                {
                    return;
                }

                if (newValue)
                {
                    this.hookStartCastTime = GameManager.RawGameTime;
                    this.hookUpdateHandler.IsEnabled = true;
                }
                else
                {
                    this.hookUpdateHandler.IsEnabled = false;
                }
            });
        }

        private bool ShouldCastHook(PredictionOutput output)
        {
            if (output.HitChance == HitChance.OutOfRange || output.HitChance == HitChance.Impossible)
            {
                return false;
            }

            if (output.HitChance == HitChance.Collision)
            {
                return false;
            }

            if (output.HitChance < this.Pudge.MinimumHookChance)
            {
                return false;
            }

            return true;
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
                        breakerChanger = this.Pudge.Config.Hero.LinkenBreakerTogglerMenu.Values.ToList();
                    }

                    foreach (var order in breakerChanger)
                    {
                        var euls = this.Pudge.Euls;
                        if (euls != null
                            && euls.Item.Id == order.Key
                            && euls.CanBeCasted && euls.CanHit(this.CurrentTarget))
                        {
                            euls.Cast(this.CurrentTarget);
                            await Task.Delay(euls.GetCastDelay(this.CurrentTarget), token);
                            return;
                        }

                        var force = this.Pudge.ForceStaff;
                        if (force != null
                            && force.Item.Id == order.Key
                            && force.CanBeCasted && force.CanHit(this.CurrentTarget))
                        {
                            force.Cast(this.CurrentTarget);
                            await Task.Delay(force.GetCastDelay(this.CurrentTarget), token);
                            return;
                        }

                        var orchid = this.Pudge.Orchid;
                        if (orchid != null
                            && orchid.Item.Id == order.Key
                            && orchid.CanBeCasted && orchid.CanHit(this.CurrentTarget))
                        {
                            orchid.Cast(this.CurrentTarget);
                            await Task.Delay(orchid.GetCastDelay(this.CurrentTarget), token);
                            return;
                        }

                        var bloodthorn = this.Pudge.BloodThorn;
                        if (bloodthorn != null
                            && bloodthorn.Item.Id == order.Key
                            && bloodthorn.CanBeCasted && bloodthorn.CanHit(this.CurrentTarget))
                        {
                            bloodthorn.Cast(this.CurrentTarget);
                            await Task.Delay(bloodthorn.GetCastDelay(this.CurrentTarget), token);
                            return;
                        }

                        var nullifier = this.Pudge.Nullifier;
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

                        var atos = this.Pudge.RodOfAtos;
                        if (atos != null
                            && atos.Item.Id == order.Key
                            && atos.CanBeCasted && atos.CanHit(this.CurrentTarget))
                        {
                            atos.Cast(this.CurrentTarget);
                            await Task.Delay(
                                atos.GetCastDelay(this.CurrentTarget) + atos.GetHitTime(this.CurrentTarget), token);
                            return;
                        }

                        var hex = this.Pudge.Sheepstick;
                        if (hex != null
                            && hex.Item.Id == order.Key
                            && hex.CanBeCasted && hex.CanHit(this.CurrentTarget))
                        {
                            hex.Cast(this.CurrentTarget);
                            await Task.Delay(hex.GetCastDelay(this.CurrentTarget), token);
                            return;
                        }

                        var diff = this.Pudge.DiffusalBlade;
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