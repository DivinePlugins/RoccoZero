namespace BAIO.Heroes.Modes.Combo
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using BAIO.Core.Extensions;
    using BAIO.Heroes.Base;
    using BAIO.Modes;

    using Divine.Entity;
    using Divine.Entity.Entities.Abilities.Components;
    using Divine.Entity.Entities.Units.Heroes;
    using Divine.Extensions;
    using Divine.Numerics;
    using Divine.Prediction;
    using Divine.Prediction.Collision;
    using Divine.Zero.Log;

    public class ObsidianDestroyerCombo : ComboMode
    {
        private readonly ObsidianDestroyer ObsidianDestroyer;

        public ObsidianDestroyerCombo(ObsidianDestroyer hero)
            : base(hero)
        {
            this.ObsidianDestroyer = hero;
        }

        public override async Task ExecuteAsync(CancellationToken token)
        {
            if (!await this.ShouldExecute(token))
            {
                return;
            }

            this.MaxTargetRange = 2000f;

            if ((this.CurrentTarget == null) || !this.CurrentTarget.IsVisible)
            {
                this.ObsidianDestroyer.Context.Orbwalker.OrbwalkTo(null);
                return;
            }

            if (this.CurrentTarget.IsIllusion)
            {
                this.OrbwalkToTarget();
                return;
            }

            var sliderValue = this.ObsidianDestroyer.UseBlinkPrediction.Value;


            if ((this.ObsidianDestroyer.BlinkDagger != null) &&
                (this.ObsidianDestroyer.BlinkDagger.Item.IsValid) && Owner.Distance2D(this.CurrentTarget) <= 1200 + sliderValue &&
                !(Owner.Distance2D(this.CurrentTarget) <= 400) &&
                this.ObsidianDestroyer.BlinkDagger.CanBeCasted)
            {
                var l = (this.Owner.Distance2D(this.CurrentTarget) - sliderValue) / sliderValue;
                var posA = this.Owner.Position;
                var posB = this.CurrentTarget.Position;
                var x = (posA.X + (l * posB.X)) / (1 + l);
                var y = (posA.Y + (l * posB.Y)) / (1 + l);
                var position = new Vector3((int)x, (int)y, posA.Z);

                this.ObsidianDestroyer.BlinkDagger.Cast(position);
                await Task.Delay(this.ObsidianDestroyer.BlinkDagger.GetCastDelay(position), token);
            }

            var targets =
                EntityManager.GetEntities<Hero>().Where(
                        x =>
                            x.IsValid && x.Team != this.Owner.Team && !x.IsIllusion &&
                            x.Distance2D(this.Owner) <= 700)
                    .ToList();

            if (!this.Owner.IsSilenced())
            {
                foreach (var ultiTarget in targets)
                {
                    if (this.ObsidianDestroyer.SanityEclipse.Ability.CanBeCasted(ultiTarget))
                    {
                        var ultiDamage = this.ObsidianDestroyer.SanityEclipse.GetDamage(ultiTarget);


                        if (ultiTarget.Health > ultiDamage)
                        {
                            continue;
                        }

                        var delay = this.ObsidianDestroyer.SanityEclipse.GetCastDelay(ultiTarget) / 1000f;
                        var radius = this.ObsidianDestroyer.SanityEclipse.Ability.GetAbilitySpecialData("radius");
                        var input =
                            new PredictionInput(
                                this.Owner,
                                ultiTarget,
                                delay,
                                float.MaxValue,
                                this.ObsidianDestroyer.SanityEclipse.CastRange,
                                radius,
                                PredictionSkillshotType.SkillshotCircle,
                                true)
                            {
                                CollisionTypes = CollisionTypes.None
                            };

                        var output = PredictionManager.GetPrediction(input);
                        var amount = output.AoeTargetsHit.Count(x => x?.Unit is Hero && x.Unit.IsValid && x.Unit.IsAlive && x.Unit.Team != this.Owner.Team) + 1;

                        if (output.HitChance >= HitChance.Medium &&
                            this.ObsidianDestroyer.MinimumTargetToUlti.Value <= amount)
                        {
                            this.ObsidianDestroyer.SanityEclipse.Cast(output.CastPosition);
                            await Task.Delay(this.ObsidianDestroyer.SanityEclipse.GetCastDelay(output.CastPosition), token);
                        }
                    }
                }
            }

            var equ = this.ObsidianDestroyer.Equilibrium;
            if (equ.CanBeCasted && this.Owner.Distance2D(this.CurrentTarget) <= this.Owner.AttackRange)
            {
                equ.Cast();
                await Task.Delay(equ.GetCastDelay(), token);
            }

            var linkens = this.CurrentTarget.IsLinkensProtected();
            await BreakLinken(token);

            var hex = this.ObsidianDestroyer.Sheepstick;
            if (hex != null && hex.CanBeCasted && hex.CanHit(CurrentTarget) && !linkens &&
                this.ObsidianDestroyer.HexHeroes[((Hero)CurrentTarget).HeroId])
            {
                hex.Cast(CurrentTarget);
                await Task.Delay(hex.GetCastDelay(), token);
            }

            var distance = this.CurrentTarget.Distance2D(this.Owner);
            var hurricanePike = this.ObsidianDestroyer.HurricanePike;
            if (hurricanePike != null && !linkens && (((float)this.Owner.Health / this.Owner.MaximumHealth) * 100f <= this.ObsidianDestroyer.HurricanePercentage.Value))
            {
                if (this.Owner.HasModifier(hurricanePike.ModifierName) && this.CurrentTarget.IsVisible)
                {
                    if (!this.ObsidianDestroyer.ArcaneOrb.Ability.IsAutoCastEnabled)
                    {
                        this.ObsidianDestroyer.ArcaneOrb.Ability.CastToggleAutocast();
                    }
                    this.Owner.Attack(this.CurrentTarget);
                    await Task.Delay(125, token);
                    return;
                }
                else if (!this.Owner.HasModifier(hurricanePike.ModifierName) && this.ObsidianDestroyer.ArcaneOrb.Ability.IsAutoCastEnabled)
                {
                    this.ObsidianDestroyer.ArcaneOrb.Ability.CastToggleAutocast();
                    await Task.Delay(50, token);
                }

                if (hurricanePike.CanBeCasted && hurricanePike.CanHit(this.CurrentTarget))
                {
                    hurricanePike.Cast(this.CurrentTarget);
                    await Task.Delay(hurricanePike.GetCastDelay(this.CurrentTarget), token);
                    if (this.ObsidianDestroyer.Equilibrium.CanBeCasted)
                    {
                        this.ObsidianDestroyer.Equilibrium.Cast();
                        await Task.Delay(this.ObsidianDestroyer.Equilibrium.GetCastDelay(), token);
                        return;
                    }
                    return;
                }
            }

            var nullifier = this.ObsidianDestroyer.Nullifier;
            if (nullifier != null && nullifier.CanBeCasted && nullifier.CanHit(CurrentTarget) && !linkens &&
                this.ObsidianDestroyer.NullifierHeroes[((Hero)CurrentTarget).HeroId])
            {
                nullifier.Cast(CurrentTarget);
                await Task.Delay(nullifier.GetCastDelay(), token);
            }

            var atos = this.ObsidianDestroyer.RodOfAtos;
            if (atos != null && atos.CanBeCasted && atos.CanHit(CurrentTarget) && !linkens)
            {
                atos.Cast(CurrentTarget);
                await Task.Delay(atos.GetCastDelay(), token);
            }

            var veil = this.ObsidianDestroyer.VeilOfDiscord;
            if (veil != null && veil.CanBeCasted && veil.CanHit(CurrentTarget))
            {
                veil.Cast(CurrentTarget.Position);
                await Task.Delay(veil.GetCastDelay(), token);
            }

            var satanic = this.ObsidianDestroyer.Satanic;
            if ((satanic != null) && satanic.CanBeCasted && (this.Owner.HealthPercent() < 0.35f))
            {
                satanic.Cast();
                await Task.Delay(satanic.GetCastDelay(), token);
            }

            var useArcaneOrb = this.ObsidianDestroyer.ShouldUseArcaneOrb(this.CurrentTarget);
            if (useArcaneOrb)
            {
                await this.ObsidianDestroyer.UseArcaneOrb(this.CurrentTarget, token);
                //return;
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
                        breakerChanger = this.ObsidianDestroyer.Config.Hero.LinkenBreakerTogglerMenu.Values.ToList();
                    }

                    foreach (var order in breakerChanger)
                    {
                        var euls = this.ObsidianDestroyer.Euls;
                        if (euls != null
                            && euls.Item.Id == order.Key
                            && euls.CanBeCasted && euls.CanHit(this.CurrentTarget))
                        {
                            euls.Cast(this.CurrentTarget);
                            await Task.Delay(euls.GetCastDelay(this.CurrentTarget), token);
                            return;
                        }

                        var force = this.ObsidianDestroyer.ForceStaff;
                        if (force != null
                            && force.Item.Id == order.Key
                            && force.CanBeCasted && force.CanHit(this.CurrentTarget))
                        {
                            force.Cast(this.CurrentTarget);
                            await Task.Delay(force.GetCastDelay(this.CurrentTarget), token);
                            return;
                        }

                        var orchid = this.ObsidianDestroyer.Orchid;
                        if (orchid != null
                            && orchid.Item.Id == order.Key
                            && orchid.CanBeCasted && orchid.CanHit(this.CurrentTarget))
                        {
                            orchid.Cast(this.CurrentTarget);
                            await Task.Delay(orchid.GetCastDelay(this.CurrentTarget), token);
                            return;
                        }

                        var bloodthorn = this.ObsidianDestroyer.BloodThorn;
                        if (bloodthorn != null
                            && bloodthorn.Item.Id == order.Key
                            && bloodthorn.CanBeCasted && bloodthorn.CanHit(this.CurrentTarget))
                        {
                            bloodthorn.Cast(this.CurrentTarget);
                            await Task.Delay(bloodthorn.GetCastDelay(this.CurrentTarget), token);
                            return;
                        }

                        var nullifier = this.ObsidianDestroyer.Nullifier;
                        if (nullifier != null
                            && nullifier.Item.Id == order.Key
                            && nullifier.CanBeCasted && nullifier.CanHit(this.CurrentTarget))
                        {
                            nullifier.Cast(this.CurrentTarget);
                            await Task.Delay(nullifier.GetCastDelay(this.CurrentTarget) + nullifier.GetHitTime(this.CurrentTarget), token);
                            return;
                        }

                        var atos = this.ObsidianDestroyer.RodOfAtos;
                        if (atos != null
                            && atos.Item.Id == order.Key
                            && atos.CanBeCasted && atos.CanHit(this.CurrentTarget))
                        {
                            atos.Cast(this.CurrentTarget);
                            await Task.Delay(atos.GetCastDelay(this.CurrentTarget) + atos.GetHitTime(this.CurrentTarget), token);
                            return;
                        }

                        var hex = this.ObsidianDestroyer.Sheepstick;
                        if (hex != null
                            && hex.Item.Id == order.Key
                            && hex.CanBeCasted && hex.CanHit(this.CurrentTarget))
                        {
                            hex.Cast(this.CurrentTarget);
                            await Task.Delay(hex.GetCastDelay(this.CurrentTarget), token);
                            return;
                        }

                        var diff = this.ObsidianDestroyer.DiffusalBlade;
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
