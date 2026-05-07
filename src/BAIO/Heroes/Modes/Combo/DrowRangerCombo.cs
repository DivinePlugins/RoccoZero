namespace BAIO.Heroes.Modes.Combo
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using BAIO.Core.Extensions;
    using BAIO.Core.Handlers;
    using BAIO.Heroes.Base;
    using BAIO.Modes;

    using Divine.Entity.Entities.Abilities.Components;
    using Divine.Entity.Entities.Units.Heroes;
    using Divine.Extensions;
    using Divine.Game;
    using Divine.Prediction;
    using Divine.Zero.Log;

    public class DrowRangerCombo : ComboMode
    {
        private readonly DrowRanger DrowRanger;
        private TaskHandler ArrowHandler;


        public DrowRangerCombo(DrowRanger hero)
            : base(hero)
        {
            this.DrowRanger = hero;
            this.ArrowHandler = TaskHandler.Run(this.OnUpdate);
        }

        protected override void OnDeactivate()
        {
            base.OnDeactivate();
            this.ArrowHandler?.Cancel();
        }

        public override async Task ExecuteAsync(CancellationToken token)
        {
            if (!await this.ShouldExecute(token))
            {
                return;
            }

            this.MaxTargetRange = Math.Max(this.MaxTargetRange, this.Owner.AttackRange * 1.5f);

            if ((this.CurrentTarget == null) || !this.CurrentTarget.IsVisible)
            {
                this.DrowRanger.Context.Orbwalker.OrbwalkTo(null);
                return;
            }

            if (this.CurrentTarget.IsIllusion)
            {
                this.OrbwalkToTarget();
                return;
            }

            if (this.Owner.IsChanneling())
            {
                return;
            }

            var linkens = this.CurrentTarget.IsLinkensProtected();
            await BreakLinken(token);

            var distance = this.CurrentTarget.Distance2D(this.Owner);
            var hurricanePike = this.DrowRanger.HurricanePike;
            if (hurricanePike != null && !linkens)
            {
                if (this.Owner.HasModifier(hurricanePike.ModifierName))
                {
                    this.Owner.Attack(this.CurrentTarget);
                    await Task.Delay(125, token);
                    return;
                }

                if (hurricanePike.CanBeCasted && hurricanePike.CanHit(this.CurrentTarget) &&
                    (this.DrowRanger.UltiStatus == UltiStatus.Deactivated) &&
                    (distance <= this.DrowRanger.Marksmanship.Radius))
                {
                    hurricanePike.Cast(this.CurrentTarget);
                    await Task.Delay(hurricanePike.GetCastDelay(this.CurrentTarget), token);
                    return;
                }
            }

            if (this.DrowRanger.WaveOfSilence.CanHit(this.CurrentTarget) && (this.DrowRanger.UltiStatus == UltiStatus.Deactivated))
            {
                var manta = this.DrowRanger.Manta;
                var usedManta = false;
                if (this.Owner.IsSilenced() && (manta != null) &&
                    manta.CanBeCasted &&
                    (this.DrowRanger.WaveOfSilence.Ability.Cooldown == 0))
                {
                    manta.Cast();
                    await Task.Delay(manta.GetCastDelay(), token);
                    usedManta = true;
                }

                if (usedManta || this.DrowRanger.WaveOfSilence.CanBeCasted)
                {
                    this.DrowRanger.WaveOfSilence.Cast(this.CurrentTarget);
                    await Task.Delay(this.DrowRanger.WaveOfSilence.GetCastDelay(this.CurrentTarget), token);
                    return;
                }
            }
            else if (this.DrowRanger.WaveOfSilence.CanBeCasted && this.DrowRanger.WaveOfSilence.CanHit(this.CurrentTarget) && !this.CurrentTarget.IsMagicImmune() &&
                     this.DrowRanger.SilenceHeroes[((Hero)CurrentTarget).HeroId])
            {
                if (this.DrowRanger.WaveOfSilence.Cast(this.CurrentTarget))
                {
                    await Task.Delay(this.DrowRanger.WaveOfSilence.GetCastDelay(this.CurrentTarget), token);
                }
            }

            var multiShot = this.DrowRanger.Multishot;
            if (multiShot.CanHit(this.CurrentTarget) && multiShot.CanBeCasted() &&
                this.Owner.Distance2D(this.CurrentTarget) <= this.Owner.AttackRange * 1.5)
            {
                var input = new PredictionInput(this.Owner, this.CurrentTarget, 0.1f, 1250,
                    this.Owner.AttackRange * 1.5f, 100,
                    PredictionSkillshotType.SkillshotLine);
                var output = PredictionManager.GetPrediction(input);

                if (output.HitChance >= HitChance.Medium)
                {
                    if (multiShot.Cast(output.CastPosition))
                    {
                        await Task.Delay((int)(1000f), token);
                    }
                }
            }

            var mom = this.DrowRanger.Mom;
            if ((mom != null) && mom.CanBeCasted && !this.DrowRanger.WaveOfSilence.CanBeCasted)
            {
                mom.Cast();
                await Task.Delay(mom.GetCastDelay(), token);
            }

            var nullifier = this.DrowRanger.Nullifier;
            if (nullifier != null && nullifier.CanBeCasted && nullifier.CanHit(CurrentTarget) && !linkens &&
                this.DrowRanger.NullifierHeroes[((Hero)CurrentTarget).HeroId])
            {
                nullifier.Cast(CurrentTarget);
                await Task.Delay(nullifier.GetCastDelay(), token);
            }

            var hex = this.DrowRanger.Sheepstick;
            if (hex != null && hex.CanBeCasted && hex.CanHit(CurrentTarget) && !linkens &&
                this.DrowRanger.HexHeroes[((Hero)CurrentTarget).HeroId])
            {
                hex.Cast(CurrentTarget);
                await Task.Delay(hex.GetCastDelay(), token);
            }

            var satanic = this.DrowRanger.Satanic;
            if ((satanic != null) && satanic.CanBeCasted && (this.Owner.HealthPercent() < 0.35f))
            {
                satanic.Cast();
                await Task.Delay(satanic.GetCastDelay(), token);
            }

            var mjollnir = this.DrowRanger.Mjollnir;
            if (mjollnir != null && mjollnir.CanBeCasted && this.DrowRanger.UltiStatus >= UltiStatus.Danger)
            {
                mjollnir.Cast(this.Owner);
                await Task.Delay(mjollnir.GetCastDelay(), token);
            }

            var useArrows = this.DrowRanger.ShouldUseFrostArrow(this.CurrentTarget);
            if (useArrows && !this.Owner.HasModifier("modifier_drow_ranger_multishot"))
            {
                await this.DrowRanger.UseFrostArrows(this.CurrentTarget, token);
                this.OrbwalkToTarget();
                return;
            }

            if (!this.Owner.HasModifier("modifier_drow_ranger_multishot"))
            {
                this.OrbwalkToTarget();
            }
        }

        private async Task OnUpdate(CancellationToken token)
        {
            if (!this.Owner.HasModifier("modifier_drow_ranger_multishot"))
            {
                return;
            }

            if (this.Owner.FindModifier("modifier_drow_ranger_multishot").RemainingTime <= 0.45f)
            {
                if (this.Owner.Move(GameManager.MousePosition))
                {
                    await Task.Delay(150, token);
                }
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
                        breakerChanger = this.DrowRanger.Config.Hero.LinkenBreakerTogglerMenu.Values.ToList();
                    }

                    foreach (var order in breakerChanger)
                    {
                        var euls = this.DrowRanger.Euls;
                        if (euls != null
                            && euls.Item.Id == order.Key
                            && euls.CanBeCasted && euls.CanHit(this.CurrentTarget))
                        {
                            euls.Cast(this.CurrentTarget);
                            await Task.Delay(euls.GetCastDelay(this.CurrentTarget), token);
                            return;
                        }

                        var force = this.DrowRanger.ForceStaff;
                        if (force != null
                            && force.Item.Id == order.Key
                            && force.CanBeCasted && force.CanHit(this.CurrentTarget))
                        {
                            force.Cast(this.CurrentTarget);
                            await Task.Delay(force.GetCastDelay(this.CurrentTarget), token);
                            return;
                        }

                        var orchid = this.DrowRanger.Orchid;
                        if (orchid != null
                            && orchid.Item.Id == order.Key
                            && orchid.CanBeCasted && orchid.CanHit(this.CurrentTarget))
                        {
                            orchid.Cast(this.CurrentTarget);
                            await Task.Delay(orchid.GetCastDelay(this.CurrentTarget), token);
                            return;
                        }

                        var bloodthorn = this.DrowRanger.BloodThorn;
                        if (bloodthorn != null
                            && bloodthorn.Item.Id == order.Key
                            && bloodthorn.CanBeCasted && bloodthorn.CanHit(this.CurrentTarget))
                        {
                            bloodthorn.Cast(this.CurrentTarget);
                            await Task.Delay(bloodthorn.GetCastDelay(this.CurrentTarget), token);
                            return;
                        }

                        var nullifier = this.DrowRanger.Nullifier;
                        if (nullifier != null
                            && nullifier.Item.Id == order.Key
                            && nullifier.CanBeCasted && nullifier.CanHit(this.CurrentTarget))
                        {
                            nullifier.Cast(this.CurrentTarget);
                            await Task.Delay(nullifier.GetCastDelay(this.CurrentTarget) + nullifier.GetHitTime(this.CurrentTarget), token);
                            return;
                        }

                        var atos = this.DrowRanger.RodOfAtos;
                        if (atos != null
                            && atos.Item.Id == order.Key
                            && atos.CanBeCasted && atos.CanHit(this.CurrentTarget))
                        {
                            atos.Cast(this.CurrentTarget);
                            await Task.Delay(atos.GetCastDelay(this.CurrentTarget) + atos.GetHitTime(this.CurrentTarget), token);
                            return;
                        }

                        var hex = this.DrowRanger.Sheepstick;
                        if (hex != null
                            && hex.Item.Id == order.Key
                            && hex.CanBeCasted && hex.CanHit(this.CurrentTarget))
                        {
                            hex.Cast(this.CurrentTarget);
                            await Task.Delay(hex.GetCastDelay(this.CurrentTarget), token);
                            return;
                        }

                        var diff = this.DrowRanger.DiffusalBlade;
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
