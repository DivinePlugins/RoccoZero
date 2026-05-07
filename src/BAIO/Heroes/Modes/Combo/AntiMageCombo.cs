using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using BAIO.Core.Handlers;
using BAIO.Heroes.Base;
using BAIO.Modes;

using Divine.Entity;
using Divine.Entity.Entities.Abilities.Components;
using Divine.Entity.Entities.Units;
using Divine.Entity.Entities.Units.Heroes;
using Divine.Extensions;
using Divine.Game;
using Divine.Prediction;

using Divine.Zero.Log;
namespace BAIO.Heroes.Modes.Combo
{
    internal class AntiMageCombo : ComboMode
    {
        private readonly AntiMage AntiMage;

        public TaskHandler IllusionHandler { get; private set; }

        public AntiMageCombo(AntiMage hero)
            : base(hero)
        {
            this.AntiMage = hero;
            this.IllusionHandler = TaskHandler.Run(OnUpdate);
        }

        public override async Task ExecuteAsync(CancellationToken token)
        {
            if (!await this.ShouldExecute(token))
            {
                return;
            }

            this.MaxTargetRange = Math.Max(this.MaxTargetRange, AntiMage.Blink.CastRange * 1.1f);

            if ((this.CurrentTarget == null) || !this.CurrentTarget.IsVisible)
            {
                this.AntiMage.Context.Orbwalker.OrbwalkTo(null);
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

                var blink = this.AntiMage.Blink;
                if (blink.CanBeCasted && this.Owner.Distance2D(this.CurrentTarget) <= blink.CastRange &&
                    this.Owner.Distance2D(this.CurrentTarget) >= this.AntiMage.MinimumBlinkRange && this.AntiMage.BlinkHeroes[((Hero)CurrentTarget).HeroId])
                {
                    var input = new PredictionInput(this.Owner, CurrentTarget, blink.GetCastDelay() / 1000f,
                        float.MaxValue, blink.CastRange, 100, PredictionSkillshotType.SkillshotCircle, false, null, true);
                    var output = PredictionManager.GetPrediction(input);

                    if (output.HitChance >= HitChance.Medium)
                    {
                        blink.Cast(output.CastPosition);
                        await Task.Delay(blink.GetCastDelay(), token);
                    }
                }

                var abyssal = this.AntiMage.AbyssalBlade;
                if (abyssal != null && abyssal.CanBeCasted && abyssal.CanHit(CurrentTarget) && !linkens &&
                    this.AntiMage.AbyssalBladeHeroes[((Hero)CurrentTarget).HeroId])
                {
                    abyssal.Cast(CurrentTarget);
                    await Task.Delay(abyssal.GetCastDelay(), token);
                }

                var manta = this.AntiMage.Manta;
                if (manta != null && manta.CanBeCasted && this.Owner.Distance2D(CurrentTarget) <= 200 &&
                    this.AntiMage.MantaHeroes[((Hero)CurrentTarget).HeroId])
                {
                    manta.Cast();
                    await Task.Delay(manta.GetCastDelay(), token);
                }

                var nullifier = this.AntiMage.Nullifier;
                if (nullifier != null && nullifier.CanBeCasted && nullifier.CanHit(CurrentTarget) && !linkens &&
                    this.AntiMage.NullifierHeroes[((Hero)CurrentTarget).HeroId])
                {
                    nullifier.Cast(CurrentTarget);
                    await Task.Delay(nullifier.GetCastDelay(), token);
                }
            }
            catch (TaskCanceledException)
            {
                // ignore
            }
            catch (Exception e)
            {
                LogManager.Error(e);
            }

            this.OrbwalkToTarget();
        }

        private async Task OnUpdate(CancellationToken token)
        {
            if (GameManager.IsPaused || !this.AntiMage.Config.General.ComboKey)
            {
                await Task.Delay(250, token);
                return;
            }

            var illusions =
                EntityManager.GetEntities<Unit>().Where(
                        x => x.IsValid && x.IsAlive && x.IsIllusion && x.Team == this.Owner.Team && x.IsControllable)
                    .ToList();

            if (illusions.Any())
            {
                foreach (var illusion in illusions)
                {
                    illusion.Attack(this.CurrentTarget);
                    await Task.Delay(100, token);
                }
            }
            await Task.Delay(100, token);
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
                        breakerChanger = this.AntiMage.Config.Hero.LinkenBreakerTogglerMenu.Values.ToList();
                    }

                    foreach (var order in breakerChanger)
                    {
                        var euls = this.AntiMage.Euls;
                        if (euls != null
                            && euls.Item.Id == order.Key
                            && euls.CanBeCasted && euls.CanHit(this.CurrentTarget))
                        {
                            euls.Cast(this.CurrentTarget);
                            await Task.Delay(euls.GetCastDelay(this.CurrentTarget), token);
                            return;
                        }

                        var force = this.AntiMage.ForceStaff;
                        if (force != null
                            && force.Item.Id == order.Key
                            && force.CanBeCasted && force.CanHit(this.CurrentTarget))
                        {
                            force.Cast(this.CurrentTarget);
                            await Task.Delay(force.GetCastDelay(this.CurrentTarget), token);
                            return;
                        }

                        var orchid = this.AntiMage.Orchid;
                        if (orchid != null
                            && orchid.Item.Id == order.Key
                            && orchid.CanBeCasted && orchid.CanHit(this.CurrentTarget))
                        {
                            orchid.Cast(this.CurrentTarget);
                            await Task.Delay(orchid.GetCastDelay(this.CurrentTarget), token);
                            return;
                        }

                        var bloodthorn = this.AntiMage.BloodThorn;
                        if (bloodthorn != null
                            && bloodthorn.Item.Id == order.Key
                            && bloodthorn.CanBeCasted && bloodthorn.CanHit(this.CurrentTarget))
                        {
                            bloodthorn.Cast(this.CurrentTarget);
                            await Task.Delay(bloodthorn.GetCastDelay(this.CurrentTarget), token);
                            return;
                        }

                        var nullifier = this.AntiMage.Nullifier;
                        if (nullifier != null
                            && nullifier.Item.Id == order.Key
                            && nullifier.CanBeCasted && nullifier.CanHit(this.CurrentTarget))
                        {
                            nullifier.Cast(this.CurrentTarget);
                            await Task.Delay(nullifier.GetCastDelay(this.CurrentTarget) + nullifier.GetHitTime(this.CurrentTarget), token);
                            return;
                        }

                        var atos = this.AntiMage.RodOfAtos;
                        if (atos != null
                            && atos.Item.Id == order.Key
                            && atos.CanBeCasted && atos.CanHit(this.CurrentTarget))
                        {
                            atos.Cast(this.CurrentTarget);
                            await Task.Delay(atos.GetCastDelay(this.CurrentTarget) + atos.GetHitTime(this.CurrentTarget), token);
                            return;
                        }

                        var hex = this.AntiMage.Sheepstick;
                        if (hex != null
                            && hex.Item.Id == order.Key
                            && hex.CanBeCasted && hex.CanHit(this.CurrentTarget))
                        {
                            hex.Cast(this.CurrentTarget);
                            await Task.Delay(hex.GetCastDelay(this.CurrentTarget), token);
                            return;
                        }

                        var diff = this.AntiMage.DiffusalBlade;
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