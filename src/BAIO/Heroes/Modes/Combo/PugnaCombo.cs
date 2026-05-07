using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using BAIO.Core.Extensions;
using BAIO.Core.Handlers;
using BAIO.Heroes.Base;
using BAIO.Modes;

using Divine.Entity;
using Divine.Entity.Entities.Abilities.Components;
using Divine.Entity.Entities.Units.Heroes;
using Divine.Extensions;
using Divine.Game;
using Divine.Numerics;
using Divine.Prediction;
using Divine.Prediction.Collision;
using Divine.Zero.Log;

namespace BAIO.Heroes.Modes.Combo
{
    internal class PugnaCombo : ComboMode
    {
        private readonly Pugna Pugna;

        public TaskHandler TurnTheOtherCheekHandler { get; private set; }

        public PugnaCombo(Pugna hero)
            : base(hero)
        {
            this.Pugna = hero;
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
                this.Pugna.Context.Orbwalker.OrbwalkTo(null);
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
                var allTargets = this.Context.TargetSelector.GetTargets().FirstOrDefault();

                var silenced = this.Owner.IsSilenced();

                var sliderValue = this.Pugna.UseBlinkPrediction.Value;

                var myHpThreshold = this.Pugna.SelfHPDrain.Value;

                var postDrainHp = this.Pugna.PostDrainHP.Value;

                var allyPostDrain = this.Pugna.HealAllyTo.Value;

                var healThreshold = this.Pugna.DrainHP.Value;

                var wardTars = this.Pugna.WardTargets.Value;

                if (!silenced)
                {
                    try
                    {
                        this.Pugna.HealTarget =
                            EntityManager.GetEntities<Hero>().FirstOrDefault(
                                x =>
                                    x.IsAlive && x.Team == this.Owner.Team && x != Owner && !x.IsIllusion
                                    && ((float) x.Health / (float) x.MaximumHealth) * 100 < healThreshold
                                    && !x.IsMagicImmune() &&
                                    this.Pugna.HealTargetHeroes[((Hero)CurrentTarget).HeroId]);

                        var myHealth = (float) Owner.Health / (float) Owner.MaximumHealth * 100;

                        if (this.Pugna.HealTarget != null)
                        {
                            this.Pugna.HealTarget = this.Pugna.HealTarget;
                        }

                        if (this.Pugna.HealTarget != null)
                        {
                            if (this.Pugna.HealTarget != null
                                && !Owner.IsChanneling() && myHealth >= myHpThreshold
                                && this.Pugna.HealTarget.Distance2D(this.Owner) <= this.Pugna.Drain.CastRange
                                && this.Pugna.HealTarget.HealthPercent() * 100 < healThreshold)
                            {
                                this.Pugna.Drain.Cast(this.Pugna.HealTarget);
                                this.Pugna.IsHealing = true;
                                await Task.Delay(
                                    (int) this.Pugna.Drain.GetCastDelay(this.Owner, this.Pugna.HealTarget, true),
                                    token);
                            }

                            //Stop Healing; There is no hidden modifier/any way to check if we are healing a target.
                            if ((Owner.IsChanneling() && myHealth <= postDrainHp) && this.Pugna.IsHealing)
                            {
                                Owner.Stop();
                                this.Pugna.IsHealing = false;
                            }

                            if (this.Pugna.HealTarget != null && this.Pugna.IsHealing &&
                                (this.Pugna.HealTarget.HealthPercent() >= ((float) allyPostDrain / 100)))
                            {
                                Owner.Stop();
                                this.Pugna.IsHealing = false;
                            }

                            if (this.Pugna.HealTarget == null && this.Pugna.IsHealing)
                            {
                                Owner.Stop();
                                this.Pugna.IsHealing = false;
                            }
                        }
                    }
                    catch (Exception)
                    {
                        // ignore
                    }
                }

                if (this.Pugna.IsHealing) return;

                if ((this.Pugna.BlinkDagger != null) &&
                    (this.Pugna.BlinkDagger.Item.IsValid) &&
                    this.CurrentTarget != null && Owner.Distance2D(this.CurrentTarget) <= 1200 + sliderValue &&
                    !(Owner.Distance2D(this.CurrentTarget) <= 400) &&
                    this.Pugna.BlinkDagger.Item.CanBeCasted(this.CurrentTarget)
                    && !Owner.IsChanneling())
                {
                    var l = (this.Owner.Distance2D(this.CurrentTarget) - sliderValue) / sliderValue;
                    var posA = this.Owner.Position;
                    var posB = this.CurrentTarget.Position;
                    var x = (posA.X + (l * posB.X)) / (1 + l);
                    var y = (posA.Y + (l * posB.Y)) / (1 + l);
                    var position = new Vector3((int) x, (int) y, posA.Z);

                    this.Pugna.BlinkDagger.Cast(position);
                    await Task.Delay((int) this.Pugna.BlinkDagger.GetCastDelay(position), token);
                }


                if (!silenced && this.CurrentTarget != null)
                {
                    var targets =
                        EntityManager.GetEntities<Hero>().Where(
                                x =>
                                    x.IsValid && x.Team != this.Owner.Team && !x.IsIllusion &&
                                    !x.IsMagicImmune() &&
                                    x.Distance2D(this.Owner) <= this.Pugna.Ward.GetAbilityData("radius"))
                            .ToList();

                    if (targets.Count >= wardTars && this.Pugna.Ward.CanBeCasted() &&
                        !Owner.IsChanneling())
                    {
                        this.Pugna.Ward.Cast(Owner.Position);
                        await Task.Delay((int) this.Pugna.Ward.GetCastDelay(this.Owner, this.Owner, true), token);
                    }

                    try
                    {
                        // var thresholdTars = this.Config.WardTargets.Item.GetValue<Slider>();
                        var manaDecrepify = this.Pugna.Decrepify.AbilityData.GetManaCost(this.Pugna.Decrepify.Level - 1);
                        var manaBlast = this.Pugna.Blast.AbilityData.GetManaCost(this.Pugna.Blast.Level - 1);
                        // var manaDrain = Drain.GetManaCost(Drain.Level - 1);

                        if (this.Pugna.Decrepify.CanBeCasted() && this.CurrentTarget != null &&
                            this.Pugna.Decrepify.CanHit(this.CurrentTarget)
                            && this.Owner.Mana >= manaBlast + manaDecrepify
                            && !Owner.IsChanneling()
                            && this.CurrentTarget.IsAlive)
                        {
                            this.Pugna.Decrepify.Cast(this.CurrentTarget);
                            await Task.Delay(
                                (int) this.Pugna.Decrepify.GetCastDelay(this.Owner, this.CurrentTarget, true), token);
                        }

                        if (this.Pugna.Blast.CanBeCasted()
                            && (!this.Pugna.Decrepify.CanBeCasted() || manaBlast > Owner.Mana - manaDecrepify)
                            && !Owner.IsChanneling()
                            && this.CurrentTarget != null && this.CurrentTarget.IsAlive)
                        {
                            var delay = this.Pugna.Blast.GetAbilityData("delay") + this.Pugna.Blast.GetCastPoint();
                            var blastTargets =
                                EntityManager.GetEntities<Hero>().OrderBy(x => x == allTargets).Where(
                                    x =>
                                        x.IsValid && x.IsVisible && x.Team != Owner.Team && !x.IsIllusion &&
                                        !x.IsMagicImmune()).ToList();
                            var blastCastRange = this.Pugna.Blast.CastRange;

                            if (blastTargets == null) return;
                            var input =
                                new PredictionInput(
                                    Owner,
                                    this.CurrentTarget,
                                    delay,
                                    float.MaxValue,
                                    blastCastRange,
                                    400,
                                    PredictionSkillshotType.SkillshotCircle,
                                    true,
                                    blastTargets)
                                {
                                    CollisionTypes = CollisionTypes.None
                                };

                            var output = PredictionManager.GetPrediction(input);

                            if (output.HitChance >= HitChance.Medium)
                            {
                                this.Pugna.Blast.Cast(output.CastPosition);
                                await Task.Delay(
                                    (int) this.Pugna.Blast.GetCastDelay(this.Owner, this.CurrentTarget, true), token);
                            }
                        }
                    }
                    catch (Exception)
                    {
                        // ignored
                    }
                }

                if (this.Pugna.BloodThorn != null &&
                    this.Pugna.BloodThorn.Item.IsValid &&
                    this.CurrentTarget != null && !Owner.IsChanneling() &&
                    this.Pugna.BloodThorn.Item.CanBeCasted(this.CurrentTarget))
                {
                    this.Pugna.BloodThorn.Cast(this.CurrentTarget);
                    await Task.Delay(this.Pugna.BloodThorn.GetCastDelay(this.CurrentTarget), token);
                }

                if ((this.Pugna.Sheepstick != null) &&
                    (this.Pugna.Sheepstick.Item.IsValid) &&
                    this.CurrentTarget != null && !Owner.IsChanneling() &&
                    this.Pugna.Sheepstick.Item.CanBeCasted(this.CurrentTarget))
                {
                    this.Pugna.Sheepstick.Cast(this.CurrentTarget);
                    await Task.Delay(this.Pugna.Sheepstick.GetCastDelay(this.CurrentTarget), token);
                }

                if (this.Pugna.Dagon != null &&
                    this.Pugna.Dagon.Item.IsValid &&
                    this.CurrentTarget != null && !Owner.IsChanneling() &&
                    this.Pugna.Dagon.Item.CanBeCasted(this.CurrentTarget))
                {
                    this.Pugna.Dagon.Cast(this.CurrentTarget);
                    await Task.Delay(this.Pugna.Dagon.GetCastDelay(this.CurrentTarget), token);
                }

                if (this.Pugna.Orchid != null &&
                    this.Pugna.Orchid.Item.IsValid &&
                    this.CurrentTarget != null && !Owner.IsChanneling() &&
                    this.Pugna.Orchid.Item.CanBeCasted(this.CurrentTarget))
                {
                    this.Pugna.Orchid.Cast(this.CurrentTarget);
                    await Task.Delay(this.Pugna.Orchid.GetCastDelay(this.CurrentTarget), token);
                }

                if (this.Pugna.RodOfAtos != null &&
                    this.Pugna.RodOfAtos.Item.IsValid &&
                    this.CurrentTarget != null && !Owner.IsChanneling() &&
                    this.Pugna.RodOfAtos.Item.CanBeCasted(this.CurrentTarget))
                {
                    this.Pugna.RodOfAtos.Cast(this.CurrentTarget);
                    await Task.Delay(this.Pugna.RodOfAtos.GetCastDelay(this.CurrentTarget), token);
                }

                if (this.Pugna.VeilOfDiscord != null &&
                    this.Pugna.VeilOfDiscord.Item.IsValid &&
                    this.CurrentTarget != null && !Owner.IsChanneling() &&
                    this.Pugna.VeilOfDiscord.Item.CanBeCasted())
                {
                    this.Pugna.VeilOfDiscord.Cast(this.CurrentTarget.Position);
                    await Task.Delay(this.Pugna.VeilOfDiscord.GetCastDelay(this.CurrentTarget), token);
                }

                if (this.Pugna.ShivasGuard != null &&
                    this.Pugna.ShivasGuard.Item.IsValid &&
                    this.CurrentTarget != null && !Owner.IsChanneling() &&
                    this.Pugna.ShivasGuard.Item.CanBeCasted() &&
                    Owner.Distance2D(this.CurrentTarget) <= 900)
                {
                    this.Pugna.ShivasGuard.Cast();
                    await Task.Delay((int) GameManager.Ping, token);
                }

                if (!silenced && this.Pugna.Drain.CanBeCasted() &&
                    (!this.Pugna.Blast.CanBeCasted() || this.Owner.Distance2D(this.CurrentTarget) > this.Pugna.Blast.CastRange)
                    && (!this.Pugna.Decrepify.CanBeCasted() || this.Owner.Distance2D(this.CurrentTarget) > this.Pugna.Decrepify.CastRange)
                    && !Owner.IsChanneling()
                    && this.CurrentTarget != null && this.CurrentTarget.IsAlive)
                {
                    this.Pugna.Drain.Cast(this.CurrentTarget);
                    await Task.Delay((int) this.Pugna.Drain.GetCastDelay(this.Owner, this.CurrentTarget, true) + 50,
                        token);
                }
                else if (!silenced && this.Pugna.Drain.CanBeCasted()
                                   && (this.Owner.Distance2D(this.CurrentTarget) > this.Pugna.Blast.CastRange
                                       || this.Owner.Distance2D(this.CurrentTarget) > this.Pugna.Decrepify.CastRange)
                                   && !Owner.IsChanneling()
                                   && this.CurrentTarget != null && this.CurrentTarget.IsAlive)
                {
                    this.Pugna.Drain.Cast(this.CurrentTarget);
                    await Task.Delay((int) this.Pugna.Drain.GetCastDelay(this.Owner, this.CurrentTarget, true) + 50,
                        token);
                }

                if (this.CurrentTarget != null && !Owner.IsValidOrbwalkingTarget(this.CurrentTarget) &&
                    !Owner.IsChanneling())
                {
                    Orbwalker.Move(GameManager.MousePosition);
                    await Task.Delay(50, token);
                }
                else
                {
                    Orbwalker.OrbwalkTo(this.CurrentTarget);
                }

                await Task.Delay(50, token);
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

        protected async Task BreakLinken(CancellationToken token)
        {
            if (this.CurrentTarget != null && this.CurrentTarget.IsValid)
            {
                try
                {
                    List<KeyValuePair<AbilityId, bool>> breakerChanger = new List<KeyValuePair<AbilityId, bool>>();

                    if (this.CurrentTarget.IsLinkensProtected())
                    {
                        breakerChanger = this.Pugna.Config.Hero.LinkenBreakerTogglerMenu.Values.ToList();
                    }

                    foreach (var order in breakerChanger)
                    {
                        var euls = this.Pugna.Euls;
                        if (euls != null
                            && euls.Item.Id == order.Key
                            && euls.CanBeCasted && euls.CanHit(this.CurrentTarget))
                        {
                            euls.Cast(this.CurrentTarget);
                            await Task.Delay(euls.GetCastDelay(this.CurrentTarget), token);
                            return;
                        }

                        var force = this.Pugna.ForceStaff;
                        if (force != null
                            && force.Item.Id == order.Key
                            && force.CanBeCasted && force.CanHit(this.CurrentTarget))
                        {
                            force.Cast(this.CurrentTarget);
                            await Task.Delay(force.GetCastDelay(this.CurrentTarget), token);
                            return;
                        }

                        var orchid = this.Pugna.Orchid;
                        if (orchid != null
                            && orchid.Item.Id == order.Key
                            && orchid.CanBeCasted && orchid.CanHit(this.CurrentTarget))
                        {
                            orchid.Cast(this.CurrentTarget);
                            await Task.Delay(orchid.GetCastDelay(this.CurrentTarget), token);
                            return;
                        }

                        var bloodthorn = this.Pugna.BloodThorn;
                        if (bloodthorn != null
                            && bloodthorn.Item.Id == order.Key
                            && bloodthorn.CanBeCasted && bloodthorn.CanHit(this.CurrentTarget))
                        {
                            bloodthorn.Cast(this.CurrentTarget);
                            await Task.Delay(bloodthorn.GetCastDelay(this.CurrentTarget), token);
                            return;
                        }

                        var nullifier = this.Pugna.Nullifier;
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

                        var atos = this.Pugna.RodOfAtos;
                        if (atos != null
                            && atos.Item.Id == order.Key
                            && atos.CanBeCasted && atos.CanHit(this.CurrentTarget))
                        {
                            atos.Cast(this.CurrentTarget);
                            await Task.Delay(
                                atos.GetCastDelay(this.CurrentTarget) + atos.GetHitTime(this.CurrentTarget), token);
                            return;
                        }

                        var hex = this.Pugna.Sheepstick;
                        if (hex != null
                            && hex.Item.Id == order.Key
                            && hex.CanBeCasted && hex.CanHit(this.CurrentTarget))
                        {
                            hex.Cast(this.CurrentTarget);
                            await Task.Delay(hex.GetCastDelay(this.CurrentTarget), token);
                            return;
                        }

                        var diff = this.Pugna.DiffusalBlade;
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