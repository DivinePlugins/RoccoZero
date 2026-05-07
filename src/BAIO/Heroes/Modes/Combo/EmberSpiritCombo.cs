using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

using BAIO.Core.Extensions;
using BAIO.Core.Handlers;
using BAIO.Heroes.Base;
using BAIO.Modes;

using Divine.Entity;
using Divine.Entity.Entities;
using Divine.Entity.Entities.Abilities.Components;
using Divine.Entity.Entities.Units.Heroes;
using Divine.Extensions;
using Divine.Game;
using Divine.Prediction;
using Divine.Zero.Log;

using UnitExtensions = Divine.Extensions.UnitExtensions;

namespace BAIO.Heroes.Modes.Combo
{
    internal class EmberSpiritCombo : ComboMode
    {
        private readonly EmberSpirit EmberSpirit;

        public TaskHandler FindandChainsHandler { get; private set; }

        public TaskHandler AutoChainsHandler { get; private set; }

        public EmberSpiritCombo(EmberSpirit hero)
            : base(hero)
        {
            this.EmberSpirit = hero;
            this.FindandChainsHandler = TaskHandler.Run(FistandChains);
            this.AutoChainsHandler = TaskHandler.Run(AutoChains);
        }

        protected override void OnDeactivate()
        {
            this.FindandChainsHandler.Cancel();

            base.OnDeactivate();
        }

        public override async Task ExecuteAsync(CancellationToken token)
        {
            if (!await this.ShouldExecute(token))
            {
                return;
            }

            var blink = this.EmberSpirit.BlinkDagger;
            if (blink != null)
            {
                this.MaxTargetRange = Math.Max(this.MaxTargetRange, blink.CastRange + 1200 * 1.3f);
            }
            else if (blink == null)
            {
                this.MaxTargetRange = Math.Max(this.MaxTargetRange, 2000);
            }

            if ((this.CurrentTarget == null) || !this.CurrentTarget.IsVisible)
            {
                this.EmberSpirit.Context.Orbwalker.OrbwalkTo(null);
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

                var ulti = this.EmberSpirit.FireRemnant;
                var activator = this.EmberSpirit.ActiveFireRemnant;
                var stacks = ulti.Ability.CurrentCharges;
                var dagger = this.EmberSpirit.BlinkDagger;

                var w = this.EmberSpirit.Fist;
                var q = this.EmberSpirit.SearingChains;
                var prediction =
                    PredictionManager.GetPrediction(
                        this.EmberSpirit.FistPredictionInput(this.CurrentTarget));

                if (dagger != null && dagger.CanBeCasted && this.Owner.Distance2D(this.CurrentTarget) <= dagger.CastRange + w.CastRange
                    && this.Owner.Distance2D(this.CurrentTarget) >= 350)
                {
                    dagger.Cast(this.CurrentTarget.Position.Extend(this.CurrentTarget.Position, 1200));
                    await Task.Delay(dagger.GetCastDelay(this.CurrentTarget.Position), token);
                }

                var veil = this.EmberSpirit.VeilOfDiscord;
                if (veil != null && veil.CanBeCasted && veil.CanHit(this.CurrentTarget))
                {
                    veil.Cast(this.CurrentTarget.Position);
                    await Task.Delay(veil.GetCastDelay(this.CurrentTarget.Position), token);
                }

                if (
                    this.Owner.HasModifiers(new[]
                    {
                        "modifier_ember_spirit_sleight_of_fist_caster",
                        "modifier_ember_spirit_fire_remnant"
                    }, false))
                {
                    if (!this.Owner.IsSilenced() && q.CanBeCasted && q.CanHit(this.CurrentTarget) &&
                        !this.CurrentTarget.IsMagicImmune() && this.Owner.Distance2D(this.CurrentTarget) <= 350)
                    {
                        q.Cast();
                        await Task.Delay(125, token);
                    }
                    return;
                }

                if (w.CanBeCasted && this.Owner.Distance2D(prediction.CastPosition) <= 1400 && !this.CurrentTarget.IsEthereal())
                {
                    w.Cast(prediction.CastPosition);
                    await Task.Delay(1, token);
                }

                if (stacks - this.EmberSpirit.LeaveSpirits.Value > 0 && this.CurrentTarget.IsRooted())
                {
                    for (int i = 0; i < stacks - this.EmberSpirit.LeaveSpirits.Value; i++)
                    {
                        ulti.Cast(this.CurrentTarget.Position);
                        await Task.Delay(ulti.GetCastDelay(), token);
                    }
                }
                else
                {
                    var anySpirits = EntityManager.GetEntities<Entity>().Any(
                        x => x.Name == "npc_dota_ember_spirit_remnant" && x.Distance2D(this.CurrentTarget) <= 450);
                    if (anySpirits)
                    {
                        await Task.Delay(150, token);
                        activator.Cast(this.CurrentTarget.Position);
                        await Task.Delay(activator.GetCastDelay(), token);
                    }
                }

                if (q.CanBeCasted && this.Owner.Distance2D(this.CurrentTarget) <= 350)
                {
                    q.Cast();
                    await Task.Delay(125, token);
                }

                var e = this.EmberSpirit.FlameGuard;
                if (e.CanBeCasted && this.Owner.Distance2D(this.CurrentTarget) <= 300)
                {
                    e.Cast();
                    await Task.Delay(e.GetCastDelay(), token);
                }

                var abyssal = this.EmberSpirit.AbyssalBlade;
                if (abyssal != null && abyssal.CanBeCasted && abyssal.CanHit(CurrentTarget) && !linkens &&
                    this.EmberSpirit.AbyssalBladeHeroes[((Hero)CurrentTarget).HeroId])
                {
                    abyssal.Cast(CurrentTarget);
                    await Task.Delay(abyssal.GetCastDelay(), token);
                }

                var ethereal = this.EmberSpirit.EtherealBlade;
                if (ethereal != null && ethereal.CanBeCasted && ethereal.CanHit(CurrentTarget) && !linkens)
                {
                    ethereal.Cast(CurrentTarget);
                    await Task.Delay(ethereal.GetCastDelay(), token);
                }

                var hex = this.EmberSpirit.Sheepstick;
                if (hex != null && hex.CanBeCasted && hex.CanHit(CurrentTarget) && !linkens &&
                    this.EmberSpirit.HexHeroes[((Hero)CurrentTarget).HeroId])
                {
                    hex.Cast(CurrentTarget);
                    await Task.Delay(hex.GetCastDelay(), token);
                }

                var shivas = this.EmberSpirit.ShivasGuard;
                if (shivas != null && shivas.CanBeCasted && this.Owner.Distance2D(this.CurrentTarget) <= 400)
                {
                    shivas.Cast();
                    await Task.Delay(shivas.GetCastDelay(), token);
                }

                var manta = this.EmberSpirit.Manta;
                if (manta != null && manta.CanBeCasted && this.Owner.Distance2D(CurrentTarget) <= 200 &&
                    this.EmberSpirit.MantaHeroes[((Hero)CurrentTarget).HeroId])
                {
                    manta.Cast();
                    await Task.Delay(manta.GetCastDelay(), token);
                }

                var nullifier = this.EmberSpirit.Nullifier;
                if (nullifier != null && nullifier.CanBeCasted && nullifier.CanHit(CurrentTarget) && !linkens &&
                    this.EmberSpirit.NullifierHeroes[((Hero)CurrentTarget).HeroId])
                {
                    nullifier.Cast(CurrentTarget);
                    await Task.Delay(nullifier.GetCastDelay(), token);
                }

                var mjollnir = this.EmberSpirit.Mjollnir;
                if (mjollnir != null && mjollnir.CanBeCasted && this.Owner.Distance2D(this.CurrentTarget) <= 400)
                {
                    mjollnir.Cast(this.Owner);
                    await Task.Delay(mjollnir.GetCastDelay(), token);
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

        private async Task FistandChains(CancellationToken token)
        {
            var target = this.Context.TargetSelector.GetTargets().FirstOrDefault();
            if (GameManager.IsPaused || !this.Owner.IsAlive || target == null)
            {
                if (this.EmberSpirit.FistandChains.Value)
                {
                    this.Context.Orbwalker.OrbwalkTo(null);
                    return;
                }
                return;
            }

            if (this.EmberSpirit.FistandChains.Value)
            {
                try
                {
                    if (this.Owner.Distance2D(target) >= this.EmberSpirit.Fist.CastRange && !this.Owner.IsChanneling())
                    {
                        this.Context.Orbwalker.OrbwalkTo(null);
                    }

                    var w = this.EmberSpirit.Fist;
                    var q = this.EmberSpirit.SearingChains;
                    var prediction = PredictionManager.GetPrediction(this.EmberSpirit.FistPredictionInput(target));
                    if (
                        this.Owner.HasModifiers(new[]
                        {
                            "modifier_ember_spirit_sleight_of_fist_caster",
                            "modifier_ember_spirit_fire_remnant"
                        }, false))
                    {
                        LogManager.Debug($"in sleight mode");
                        if (!this.Owner.IsSilenced() && q.CanBeCasted && q.CanHit(target) &&
                            !target.IsMagicImmune())
                        {
                            q.Cast();
                            await Task.Delay(125, token);
                        }
                        return;
                    }

                    if (w.CanBeCasted && this.Owner.Distance2D(prediction.CastPosition) <= 1400)
                    {
                        w.Cast(prediction.CastPosition);
                        await Task.Delay(1, token);
                    }

                    var veil = this.EmberSpirit.VeilOfDiscord;
                    if (veil != null && veil.CanBeCasted && veil.CanHit(target))
                    {
                        veil.Cast(target.Position);
                        await Task.Delay(veil.GetCastDelay(target), token);
                    }
                }
                catch (TaskCanceledException)
                {
                    //нет
                }
                catch (Exception e)
                {
                    LogManager.Debug($"{e}");
                }
            }
            await Task.Delay(100, token);
        }

        private async Task AutoChains(CancellationToken token)
        {
            while (this.EmberSpirit.AutoChain.Value)
            {
                if (!this.EmberSpirit.Config.General.ComboKey.Value && !this.EmberSpirit.FistandChains.Value)
                {
                    var target = this.Context.TargetSelector.GetTargets().FirstOrDefault();
                    if (target != null)
                    {
                        var mod = this.Owner.FindModifier("modifier_ember_spirit_sleight_of_fist_caster");
                        if (mod != null)
                        {
                            if (this.EmberSpirit.SearingChains.CanBeCasted)
                            {
                                if (this.Owner.Distance2D(target) <= 400)
                                {
                                    this.EmberSpirit.SearingChains.Cast();
                                    await Task.Delay(100, token);
                                }
                            }
                        }
                    }
                }
                await Task.Delay(1, token);
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
                        breakerChanger = this.EmberSpirit.Config.Hero.LinkenBreakerTogglerMenu.Values.ToList();
                    }

                    foreach (var order in breakerChanger)
                    {
                        var euls = this.EmberSpirit.Euls;
                        if (euls != null
                            && euls.Item.Id == order.Key
                            && euls.CanBeCasted && euls.CanHit(this.CurrentTarget))
                        {
                            euls.Cast(this.CurrentTarget);
                            await Task.Delay(euls.GetCastDelay(this.CurrentTarget), token);
                            return;
                        }

                        var force = this.EmberSpirit.ForceStaff;
                        if (force != null
                            && force.Item.Id == order.Key
                            && force.CanBeCasted && force.CanHit(this.CurrentTarget))
                        {
                            force.Cast(this.CurrentTarget);
                            await Task.Delay(force.GetCastDelay(this.CurrentTarget), token);
                            return;
                        }

                        var orchid = this.EmberSpirit.Orchid;
                        if (orchid != null
                            && orchid.Item.Id == order.Key
                            && orchid.CanBeCasted && orchid.CanHit(this.CurrentTarget))
                        {
                            orchid.Cast(this.CurrentTarget);
                            await Task.Delay(orchid.GetCastDelay(this.CurrentTarget), token);
                            return;
                        }

                        var bloodthorn = this.EmberSpirit.BloodThorn;
                        if (bloodthorn != null
                            && bloodthorn.Item.Id == order.Key
                            && bloodthorn.CanBeCasted && bloodthorn.CanHit(this.CurrentTarget))
                        {
                            bloodthorn.Cast(this.CurrentTarget);
                            await Task.Delay(bloodthorn.GetCastDelay(this.CurrentTarget), token);
                            return;
                        }

                        var nullifier = this.EmberSpirit.Nullifier;
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

                        var atos = this.EmberSpirit.RodOfAtos;
                        if (atos != null
                            && atos.Item.Id == order.Key
                            && atos.CanBeCasted && atos.CanHit(this.CurrentTarget))
                        {
                            atos.Cast(this.CurrentTarget);
                            await Task.Delay(
                                atos.GetCastDelay(this.CurrentTarget) + atos.GetHitTime(this.CurrentTarget), token);
                            return;
                        }

                        var hex = this.EmberSpirit.Sheepstick;
                        if (hex != null
                            && hex.Item.Id == order.Key
                            && hex.CanBeCasted && hex.CanHit(this.CurrentTarget))
                        {
                            hex.Cast(this.CurrentTarget);
                            await Task.Delay(hex.GetCastDelay(this.CurrentTarget), token);
                            return;
                        }

                        var diff = this.EmberSpirit.DiffusalBlade;
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