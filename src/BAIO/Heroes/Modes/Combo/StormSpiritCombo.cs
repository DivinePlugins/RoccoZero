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

    using Divine.Entity.Entities.Abilities.Components;
    using Divine.Entity.Entities.Units.Heroes;
    using Divine.Extensions;
    using Divine.Game;
    using Divine.Prediction;
    using Divine.Prediction.Collision;
    using Divine.Zero.Log;

    internal class StormSpiritCombo : ComboMode
    {
        private readonly StormSpirit StormSpirit;

        public StormSpiritCombo(StormSpirit hero)
            : base(hero)
        {
            this.StormSpirit = hero;
        }

        public override async Task ExecuteAsync(CancellationToken token)
        {
            if (!await this.ShouldExecute(token))
            {
                return;
            }

            if ((this.CurrentTarget == null) || !this.CurrentTarget.IsVisible)
            {
                this.StormSpirit.Context.Orbwalker.OrbwalkTo(null);
                return;
            }

            this.MaxTargetRange = Math.Max(this.MaxTargetRange, this.StormSpirit.DistanceForUlt.Value * 1.1f);

            if (this.CurrentTarget.IsIllusion)
            {
                this.OrbwalkToTarget();
                return;
            }

            // Modifier info
            var inUltimate = this.StormSpirit.InUlti ||
                             this.StormSpirit.Ulti.Ability.IsInAbilityPhase;

            // Ulti info
            var speed = this.StormSpirit.Ulti.Speed;
            var distance = this.StormSpirit.Owner.Distance2D(this.CurrentTarget);
            var approxTravelTime = distance / speed; // in seconds

            var remnant = this.StormSpirit.Remnant;
            var vortex = this.StormSpirit.Vortex;
            var overload = this.StormSpirit.Overload;
            var ulti = this.StormSpirit.Ulti;

            if (ulti.CanBeCasted && ulti.CanHit(this.CurrentTarget) &&
                this.Owner.Distance2D(CurrentTarget) >= this.Owner.AttackRange - 100
                && this.Owner.Distance2D(CurrentTarget) <= this.StormSpirit.DistanceForUlt.Value && !inUltimate)
            {
                var input = new PredictionInput(this.Owner,
                    CurrentTarget,
                    ulti.GetCastDelay() / 1000f,
                    speed,
                    this.MaxTargetRange,
                    200,
                    PredictionSkillshotType.SkillshotCircle,
                    true, null, true)
                {
                    CollisionTypes = CollisionTypes.None
                };

                var output = PredictionManager.GetPrediction(input);

                // LogManager.Debug($"Owner: {input.Owner.Name}");
                // LogManager.Debug($"TargetName: {input.Target.Name}");
                // LogManager.Debug($"Delay: {input.Delay}");
                // LogManager.Debug($"Range: {input.Range}");
                // LogManager.Debug($"Speed: {input.Speed}");
                // LogManager.Debug($"Radius: {input.Radius}");
                // LogManager.Debug($"Type: {input.PredictionSkillshotType}");
                // LogManager.Debug($"Arrival Time: {output.ArrivalTime}");
                // LogManager.Debug($"GetCastDelay");

                if (output.HitChance >= HitChance.Low)
                {
                    var soul = this.StormSpirit.SoulRing;
                    if (soul?.CanBeCasted == true && this.Owner.CanCast() &&
                        this.Owner.Health >= this.Owner.MaximumHealth * 0.4f)
                    {
                        soul.Cast();
                        await Task.Delay(soul.GetCastDelay(), token);
                    }

                    var manaCost = ulti.GetManaCost(output.CastPosition);
                    if (manaCost <= this.Owner.Mana)
                    {
                        ulti.Cast(output.CastPosition);
                        await Task.Delay(ulti.GetCastDelay() + (int) (approxTravelTime * 1000f), token);
                    }
                }
            }

            var linkens = this.CurrentTarget.IsLinkensProtected();
            await BreakLinken(token);

            var vortexCost = vortex?.Ability.AbilityData.GetManaCost(vortex.Ability.Level - 1);
            var remnantCost = remnant?.Ability.AbilityData.GetManaCost(remnant.Ability.Level - 1);

            if ((this.StormSpirit.InOverload || this.StormSpirit.InUlti) && this.Owner.CanAttack())
            {
                Owner.Attack(CurrentTarget);
                await Task.Delay(((int) this.Owner.AttackBackswing() * 1000) + (int)GameManager.Ping, token);
            }

            var ignore = this.StormSpirit.IgnoreOverload.Value;
            if ((ignore || !this.StormSpirit.InOverload) && vortex?.CanBeCasted == true &&
                (vortexCost + remnantCost) <= this.Owner.Mana)
            {
                if (this.Owner.HasAghanimsScepter() && this.CurrentTarget.Distance2D(this.Owner) <= vortex.CastRange)
                {
                    vortex.Cast();
                    await Task.Delay(vortex.GetCastDelay() + 50, token);
                }
                else if (!this.Owner.HasAghanimsScepter() && !linkens && this.CurrentTarget.Distance2D(this.Owner) <= vortex.CastRange)
                {
                    vortex.Cast(CurrentTarget);
                    await Task.Delay(vortex.GetCastDelay() + 50, token);
                }
            }

            if (!this.StormSpirit.InOverload && this.CurrentTarget.IsAlive && remnant?.CanBeCasted == true &&
                vortex?.CanBeCasted == false && this.Owner.Distance2D(this.CurrentTarget) <= remnant.Radius)
            {
                remnant?.Cast();
                await Task.Delay(remnant.GetCastDelay() + 50, token);
            }

            var sheepStick = this.StormSpirit.Sheepstick;
            if (sheepStick != null && sheepStick.CanBeCasted && sheepStick.CanHit(CurrentTarget) && !linkens &&
                this.StormSpirit.SheepHeroes[((Hero)CurrentTarget).HeroId])
            {
                sheepStick.Cast(this.CurrentTarget);
                await Task.Delay(sheepStick.GetCastDelay(), token);
            }

            var shivasGuard = this.StormSpirit.ShivasGuard;
            if (shivasGuard != null && shivasGuard.CanBeCasted && shivasGuard.CanHit(CurrentTarget))
            {
                shivasGuard.Cast(CurrentTarget);
                await Task.Delay(shivasGuard.GetCastDelay(), token);
            }

            var bloodthorn = this.StormSpirit.BloodThorn;
            if (bloodthorn != null && bloodthorn.CanBeCasted && bloodthorn.CanHit(CurrentTarget) && !linkens &&
                this.StormSpirit.BtAndOrchidHeroes[((Hero)CurrentTarget).HeroId])
            {
                bloodthorn.Cast(this.CurrentTarget);
                await Task.Delay(bloodthorn.GetCastDelay(), token);
            }

            var nullifier = this.StormSpirit.Nullifier;
            if (nullifier != null && nullifier.CanBeCasted && nullifier.CanHit(this.CurrentTarget) && !linkens &&
                this.StormSpirit.NullifierHeroes[((Hero)CurrentTarget).HeroId])
            {
                nullifier.Cast(this.CurrentTarget);
                await Task.Delay(nullifier.GetCastDelay(), token);
            }

            var orchid = this.StormSpirit.Orchid;
            if (orchid != null && orchid.CanBeCasted && orchid.CanHit(CurrentTarget) && !linkens &&
                this.StormSpirit.BtAndOrchidHeroes[((Hero)CurrentTarget).HeroId])
            {
                orchid.Cast(this.CurrentTarget);
                await Task.Delay(orchid.GetCastDelay(), token);
            }

            var mjollnir = this.StormSpirit.Mjollnir;
            if (mjollnir != null && mjollnir.CanBeCasted)
            {
                mjollnir.Cast(this.Owner);
                await Task.Delay(mjollnir.GetCastDelay(), token);
            }

            try
            {
                var cantCastAbilities = !remnant.CanBeCasted && !vortex.CanBeCasted;
                var ultiPos = this.StormSpirit.UltiPos(this.CurrentTarget);

                if (cantCastAbilities && !this.StormSpirit.InOverload && !inUltimate &&
                    this.Owner.Distance2D(CurrentTarget) <= this.Owner.AttackRange - 100 &&
                    this.StormSpirit.Ulti.CanBeCasted)
                {
                    this.StormSpirit.Ulti.Cast(ultiPos);
                    await Task.Delay(this.StormSpirit.Ulti.GetCastDelay(), token);
                }
                if ((this.StormSpirit.InOverload || this.StormSpirit.InUlti) && this.Owner.CanAttack())
                {
                    Owner.Attack(CurrentTarget);
                    await Task.Delay(((int)this.Owner.AttackBackswing() * 1000) + (int)GameManager.Ping + 50, token);
                }

                else if (!this.Owner.IsAttacking() && !this.Owner.IsChanneling() && !this.StormSpirit.InOverload && !inUltimate
                    && !this.StormSpirit.Ulti.CanBeCasted)
                {
                    this.OrbwalkToTarget();
                }
            }
            catch (TaskCanceledException)
            {
                //
            }
            catch (Exception e)
            {
                LogManager.Error(e);
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
                        breakerChanger = this.StormSpirit.Config.Hero.LinkenBreakerTogglerMenu.Values.ToList();
                    }

                    foreach (var order in breakerChanger)
                    {
                        var euls = this.StormSpirit.Euls;
                        if (euls != null
                            && euls.Item.Id == order.Key
                            && euls.CanBeCasted && euls.CanHit(this.CurrentTarget))
                        {
                            euls.Cast(this.CurrentTarget);
                            await Task.Delay(euls.GetCastDelay(this.CurrentTarget), token);
                            return;
                        }

                        var force = this.StormSpirit.ForceStaff;
                        if (force != null
                            && force.Item.Id == order.Key
                            && force.CanBeCasted && force.CanHit(this.CurrentTarget))
                        {
                            force.Cast(this.CurrentTarget);
                            await Task.Delay(force.GetCastDelay(this.CurrentTarget), token);
                            return;
                        }

                        var orchid = this.StormSpirit.Orchid;
                        if (orchid != null
                            && orchid.Item.Id == order.Key
                            && orchid.CanBeCasted && orchid.CanHit(this.CurrentTarget))
                        {
                            orchid.Cast(this.CurrentTarget);
                            await Task.Delay(orchid.GetCastDelay(this.CurrentTarget), token);
                            return;
                        }

                        var bloodthorn = this.StormSpirit.BloodThorn;
                        if (bloodthorn != null
                            && bloodthorn.Item.Id == order.Key
                            && bloodthorn.CanBeCasted && bloodthorn.CanHit(this.CurrentTarget))
                        {
                            bloodthorn.Cast(this.CurrentTarget);
                            await Task.Delay(bloodthorn.GetCastDelay(this.CurrentTarget), token);
                            return;
                        }

                        var nullifier = this.StormSpirit.Nullifier;
                        if (nullifier != null
                            && nullifier.Item.Id == order.Key
                            && nullifier.CanBeCasted && nullifier.CanHit(this.CurrentTarget))
                        {
                            nullifier.Cast(this.CurrentTarget);
                            await Task.Delay(nullifier.GetCastDelay(this.CurrentTarget) + nullifier.GetHitTime(this.CurrentTarget), token);
                            return;
                        }

                        var atos = this.StormSpirit.RodOfAtos;
                        if (atos != null
                            && atos.Item.Id == order.Key
                            && atos.CanBeCasted && atos.CanHit(this.CurrentTarget))
                        {
                            atos.Cast(this.CurrentTarget);
                            await Task.Delay(atos.GetCastDelay(this.CurrentTarget) + atos.GetHitTime(this.CurrentTarget), token);
                            return;
                        }

                        var hex = this.StormSpirit.Sheepstick;
                        if (hex != null
                            && hex.Item.Id == order.Key
                            && hex.CanBeCasted && hex.CanHit(this.CurrentTarget))
                        {
                            hex.Cast(this.CurrentTarget);
                            await Task.Delay(hex.GetCastDelay(this.CurrentTarget), token);
                            return;
                        }

                        var diff = this.StormSpirit.DiffusalBlade;
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