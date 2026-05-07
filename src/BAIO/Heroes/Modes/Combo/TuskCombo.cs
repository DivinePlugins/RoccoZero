namespace BAIO.Heroes.Modes.Combo
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using BAIO.Core.Extensions;
    using BAIO.Core.Handlers;
    using BAIO.Modes;

    using Base;

    using Divine.Entity;
    using Divine.Entity.Entities.Abilities.Components;
    using Divine.Entity.Entities.Components;
    using Divine.Entity.Entities.Units;
    using Divine.Entity.Entities.Units.Heroes;
    using Divine.Extensions;
    using Divine.Game;
    using Divine.Numerics;
    using Divine.Prediction;
    using Divine.Prediction.Collision;
    using Divine.Zero.Log;

    internal class TuskCombo : ComboMode
    {
        private Vector3 BlinkPosition;
        private readonly Tusk Tusk;
        private List<Hero> teammates;
        public TaskHandler ComboHandler;
        private Vector3 pos;
        private Vector3 shardPredPos;

        public TuskCombo(Tusk hero)
            : base(hero)
        {
            this.Tusk = hero;
            this.ComboHandler = TaskHandler.Run(this.KickCombo);
        }

        public override async Task ExecuteAsync(CancellationToken token)
        {
            if (!await this.ShouldExecute(token))
            {
                return;
            }

            var blink = this.Tusk.BlinkDagger;
            if (blink != null)
            {
                this.MaxTargetRange = Math.Max(this.MaxTargetRange, blink.CastRange * 1.1f);
            }
            var snowball = this.Tusk.Snowball;
            if (snowball != null && snowball.CanBeCasted())
            {
                this.MaxTargetRange = Math.Max(this.MaxTargetRange, snowball.CastRange * 1.1f);
            }

            if ((this.CurrentTarget == null) || !this.CurrentTarget.IsVisible)
            {
                this.Tusk.Context.Orbwalker.OrbwalkTo(null);
                return;
            }

            if (this.CurrentTarget.IsIllusion)
            {
                this.OrbwalkToTarget();
                return;
            }
            var ulti = this.Tusk.WalrusPunch;
            var shards = this.Tusk.IceShards;
            var lSnowball = this.Tusk.LaunchSnowball;
            // var kick = this.Tusk.WalrusKick;
            var tagTeam = this.Tusk.TagTeam;

            var blinkReady = blink != null && blink.CanBeCasted;
            var distance = this.Owner.Distance2D(this.CurrentTarget);

            if (this.Owner.IsInvisible())
            {
                if (ulti.CanBeCasted() && this.Owner.CanAttack())
                {
                    ulti.Cast(this.CurrentTarget);
                    await Task.Delay((int) ulti.GetCastDelay(this.Owner, this.CurrentTarget, true), token);
                }
                else
                {
                    this.Owner.Attack(CurrentTarget);
                    await Task.Delay(200, token);
                }
                return;
            }

            var linkens = this.CurrentTarget.IsLinkensProtected();
            await BreakLinken(token);

            try
            {
                if (blinkReady && this.Owner.Distance2D(this.CurrentTarget) > 600)
                {
                    this.BlinkPosition = this.CurrentTarget.Position.Extend(this.Owner.Position,
                        Math.Max(100, distance - blink.CastRange));

                    blink.Cast(BlinkPosition);


                    var solar = this.Tusk.SolarCrest;
                    if (!this.Tusk.InSnowball && solar != null && solar.CanBeCasted && solar.CanHit(CurrentTarget)
                        && this.Tusk.MedallionCrestHeroes[((Hero)CurrentTarget).HeroId])
                    {
                        solar.Cast(this.CurrentTarget);
                        await Task.Delay(50, token);
                    }
                    var medal = this.Tusk.Medallion;
                    if (!this.Tusk.InSnowball && medal != null && medal.CanBeCasted && medal.CanHit(CurrentTarget)
                        && this.Tusk.MedallionCrestHeroes[((Hero)CurrentTarget).HeroId])
                    {
                        medal.Cast(this.CurrentTarget);
                        await Task.Delay(50, token);
                    }

                    var nullifier = this.Tusk.Nullifier;
                    if (!this.Tusk.InSnowball && nullifier != null && nullifier.CanBeCasted && nullifier.CanHit(CurrentTarget))
                    {
                        nullifier.Cast(this.CurrentTarget);
                        await Task.Delay(50, token);
                    }

                    var bloodthorn2 = this.Tusk.BloodThorn;
                    if (!this.Tusk.InSnowball && bloodthorn2 != null && bloodthorn2.CanBeCasted && bloodthorn2.CanHit(CurrentTarget))
                    {
                        bloodthorn2.Cast(this.CurrentTarget);
                        await Task.Delay(50, token);
                    }

                    if (ulti.CanBeCasted() && this.Owner.CanAttack())
                    {
                        ulti.Cast(this.CurrentTarget);
                        await Task.Delay((int) ulti.GetCastDelay(this.Owner, this.CurrentTarget, true), token);
                    }
                    else
                    {
                        await Task.Delay(blink.GetCastDelay(this.CurrentTarget), token);
                    }
                    await Task.Delay(blink.GetCastDelay(this.CurrentTarget), token);
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

            var solar2 = this.Tusk.SolarCrest;
            if (!this.Tusk.InSnowball && solar2 != null && solar2.CanBeCasted && solar2.CanHit(CurrentTarget)
                && this.Tusk.MedallionCrestHeroes[((Hero)CurrentTarget).HeroId])
            {
                solar2.Cast(this.CurrentTarget);
                await Task.Delay(50, token);
            }
            var medal2 = this.Tusk.Medallion;
            if (!this.Tusk.InSnowball && medal2 != null && medal2.CanBeCasted && medal2.CanHit(CurrentTarget)
                && this.Tusk.MedallionCrestHeroes[((Hero)CurrentTarget).HeroId])
            {
                medal2.Cast(this.CurrentTarget);
                await Task.Delay(50, token);
            }
            var nullifier2 = this.Tusk.Nullifier;
            if (!this.Tusk.InSnowball && nullifier2 != null && nullifier2.CanBeCasted && nullifier2.CanHit(CurrentTarget))
            {
                nullifier2.Cast(this.CurrentTarget);
                await Task.Delay(50, token);
            }

            var bloodthorn3 = this.Tusk.BloodThorn;
            if (!this.Tusk.InSnowball && bloodthorn3 != null && bloodthorn3.CanBeCasted && bloodthorn3.CanHit(CurrentTarget))
            {
                bloodthorn3.Cast(this.CurrentTarget);
                await Task.Delay(50, token);
            }
            if (ulti.CanBeCasted() && this.Owner.CanAttack())
            {
                ulti.Cast(this.CurrentTarget);
                await Task.Delay((int) ulti.GetCastDelay(this.Owner, this.CurrentTarget, true), token);
            }


            if (!blinkReady && snowball.CanBeCasted() && snowball.CanHit(this.CurrentTarget) && distance >= 500)
            {
                if (snowball.CanBeCasted() && snowball.CanHit(this.CurrentTarget) && !linkens)
                {
                    snowball.Cast(this.CurrentTarget);
                    await Task.Delay(500, token);
                }

                if (this.Tusk.InSnowball)
                {
                    teammates =
                        EntityManager.GetEntities<Hero>().Where(
                                x =>
                                    x.IsValid && !x.IsChanneling() && x.Team == this.Owner.Team && x != this.Owner &&
                                    x.Distance2D(this.Owner) <= 400 &&
                                    !x.HasModifier("modifier_tusk_snowball_movement_friendly"))
                            .OrderBy(x => x.Distance2D(this.Owner)).ToList();

                    if (teammates.Any())
                    {
                        foreach (var teammate in teammates)
                        {
                            this.Owner.Attack(teammate);
                        }
                        if (this.Tusk.InSnowball && lSnowball != null && lSnowball.CanBeCasted())
                        {
                            lSnowball.Cast();
                            //await Task.Delay((int)lSnowball.GetCastDelay(this.Owner, this.CurrentTarget, true), token);
                        }
                    }
                    else if (!teammates.Any())
                    {
                        if (this.Tusk.InSnowball && lSnowball != null && lSnowball.CanBeCasted())
                        {
                            lSnowball.Cast();
                            //await Task.Delay((int)lSnowball.GetCastDelay(this.Owner, this.CurrentTarget, true), token);
                        }
                    }
                }
            }
            try
            {
                if (shards.CanBeCasted() && shards.CanHit(this.CurrentTarget))
                {
                    if (this.Tusk.InSnowball && distance > 750)
                    {
                        return;
                    }

                    if (ulti.CanBeCasted() && distance <= 300)
                    {
                        return;
                    }

                    var input = new PredictionInput(this.Owner,
                        this.CurrentTarget,
                        (int) shards.GetCastDelay(this.Owner, this.CurrentTarget) / 1000f,
                        1200,
                        2000,
                        100,
                        PredictionSkillshotType.SkillshotCircle)
                    {
                        CollisionTypes = CollisionTypes.None
                    };

                    var output = PredictionManager.GetPrediction(input);
                    var asdf = this.Owner.InFront(200 + this.Owner.Distance2D(this.CurrentTarget));
                    var arrivalTime = this.Owner.Distance2D(this.CurrentTarget) / 2000;
                    var arrivalTimeMs = arrivalTime * this.CurrentTarget.MovementSpeed;
                    shardPredPos = output.CastPosition.Extend(this.Owner.Position, -arrivalTimeMs - 150);
                    if (this.CurrentTarget.HasModifier("modifier_tusk_walrus_punch_air_time"))
                    {
                        shards.Cast(this.CurrentTarget.Position.Extend(asdf, 200));
                        await Task.Delay((int) shards.GetCastDelay(this.Owner, this.CurrentTarget, true), token);
                    }

                    if (output.HitChance >= HitChance.Medium)
                    {
                        shards.Cast(shardPredPos);
                        await Task.Delay((int) shards.GetCastDelay(this.Owner, this.CurrentTarget, true), token);
                    }
                }
            }
            catch (TaskCanceledException)
            {
                // no
            }
            catch (Exception e)
            {
                LogManager.Error(e);
            }


            if (distance <= 300 && tagTeam.CanBeCasted())
            {
                tagTeam.Cast();
                await Task.Delay((int)tagTeam.GetCastDelay(this.Owner, this.CurrentTarget, true), token);
            }

            var vessel = this.Tusk.Vessel;
            var urn = this.Tusk.Urn;
            if ((urn?.CanBeCasted == true && this.Tusk.UrnHeroes[((Hero)CurrentTarget).HeroId] ||
                vessel?.CanBeCasted == true && this.Tusk.VesselHeroes[((Hero)CurrentTarget).HeroId]) && !linkens
                && this.Owner.Distance2D(this.CurrentTarget) < 400)
            {
                urn?.Cast(this.CurrentTarget);
                vessel?.Cast(this.CurrentTarget);
            }

            var halberd = this.Tusk.HeavensHalberd;
            if (halberd != null && halberd.CanBeCasted && halberd.CanHit(CurrentTarget) && !linkens &&
                this.Tusk.HalberdHeroes[((Hero)CurrentTarget).HeroId])
            {
                halberd.Cast(CurrentTarget);
                await Task.Delay(halberd.GetCastDelay(), token);
            }

            var atos = this.Tusk.RodOfAtos;
            if (atos != null && atos.CanBeCasted && atos.CanHit(CurrentTarget) && !linkens &&
                this.Tusk.AtosHeroes[((Hero)CurrentTarget).HeroId])
            {
                atos.Cast(CurrentTarget);
                await Task.Delay(atos.GetCastDelay(), token);
            }

            var bloodthorn = this.Tusk.BloodThorn;
            if (bloodthorn != null && bloodthorn.CanBeCasted && bloodthorn.CanHit(CurrentTarget) && !linkens &&
                this.Tusk.OrchidBloodthornHeroes[((Hero)CurrentTarget).HeroId])
            {
                bloodthorn.Cast(CurrentTarget);
                await Task.Delay(bloodthorn.GetCastDelay(), token);
            }

            var orchid = this.Tusk.Orchid;
            if (orchid != null && orchid.CanBeCasted && orchid.CanHit(CurrentTarget) && !linkens &&
                this.Tusk.OrchidBloodthornHeroes[((Hero)CurrentTarget).HeroId])
            {
                orchid.Cast(CurrentTarget);
                await Task.Delay(orchid.GetCastDelay(), token);
            }

            this.OrbwalkToTarget();
        }

        protected override void OnDeactivate()
        {
            base.OnDeactivate();
            ComboHandler.Cancel();
        }

        private async Task KickCombo(CancellationToken token)
        {
            var target = this.Context.TargetSelector.GetTargets().FirstOrDefault();
            if (GameManager.IsPaused || !this.Owner.IsAlive || !this.Tusk.WalrusKick.CanBeCasted() || target == null ||
                target.IsLinkensProtected() || target.HasModifier("modifier_item_lotus_orb_active"))
            {
                return;
            }

            if (this.Tusk.KickCombo)
            {
                try
                {
                    var kick = Tusk.WalrusKick;
                    var teammateys =
                        EntityManager.GetEntities<Hero>().Where(
                                x => x.IsValid && x.Team == this.Owner.Team && x != this.Owner && x.Distance2D(this.Owner) <= 1600 + this.Tusk.BlinkDagger.CastRange)
                            .OrderBy(x => x.Distance2D(this.Owner)).FirstOrDefault();

                    var fountain =
                        EntityManager.GetEntities<Unit>()
                            .FirstOrDefault(x => x.IsValid && x.Team == this.Owner.Team && x.ClassId == ClassId.CDOTA_Unit_Fountain);

                    if ((teammateys != null || fountain != null))
                    {

                        if (Tusk.InsecType == InsecType.TeamMate && teammateys != null)
                        {
                            pos = teammateys.Position.Extend(target.Position,
                                      teammateys.Distance2D(target) + kick.CastRange);
                        }
                        else if (Tusk.InsecType == InsecType.Fountain && fountain != null)
                        {
                            pos = fountain.Position.Extend(target.Position,
                                      fountain.Distance2D(target) + kick.CastRange + 100);
                        }

                        if (this.Tusk.BlinkDagger != null && this.Tusk.BlinkDagger.CanBeCasted &&
                            this.Owner.Distance2D(pos) <= this.Tusk.BlinkDagger.CastRange)
                        {
                            this.Tusk.BlinkDagger.Cast(pos);
                            await Task.Delay((int)this.Tusk.BlinkDagger.GetCastDelay(), token);
                        }

                        var ultBlyad = this.Tusk.WalrusPunch;
                        if (ultBlyad.CanBeCasted() && this.Owner.CanAttack())
                        {
                            ultBlyad.Cast(target);
                            await Task.Delay((int)ultBlyad.GetCastDelay(this.Owner, target, true) + 500, token);
                        }

                        if (kick.CanBeCasted() && kick.CanHit(target))
                        {
                            kick.Cast(target);
                            await Task.Delay(500 + (int)GameManager.Ping, token);
                        }

                        await Task.Delay(200, token);
                    }

                    var shard = this.Tusk.IceShards;
                    if (!kick.CanBeCasted() && shard.CanBeCasted())
                    {
                        var inFront = this.Owner.InFront(1700);
                        shard.Cast(inFront);
                        await Task.Delay((int)shard.GetCastDelay(this.Owner, this.CurrentTarget, true) + (int)GameManager.Ping, token);
                    }

                    var lSnowball = this.Tusk.LaunchSnowball;
                    var snowball = this.Tusk.Snowball;
                    if (!kick.CanBeCasted() && snowball.CanBeCasted() && snowball.CanHit(target))
                    {
                        snowball.Cast(target);
                        await Task.Delay((int)GameManager.Ping + 500, token);
                    }

                    if (lSnowball.CanBeCasted() && !snowball.CanBeCasted())
                    {
                        lSnowball.Cast();
                        await Task.Delay((int)GameManager.Ping + 1500, token);
                    }

                }
                catch (Exception e)
                {
                    LogManager.Error(e);
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
                        breakerChanger = this.Tusk.Config.Hero.LinkenBreakerTogglerMenu.Values.ToList();
                    }

                    foreach (var order in breakerChanger)
                    {
                        var euls = this.Tusk.Euls;
                        if (euls != null
                            && euls.Item.Id == order.Key
                            && euls.CanBeCasted && euls.CanHit(this.CurrentTarget))
                        {
                            euls.Cast(this.CurrentTarget);
                            await Task.Delay(euls.GetCastDelay(this.CurrentTarget), token);
                            return;
                        }

                        var force = this.Tusk.ForceStaff;
                        if (force != null
                            && force.Item.Id == order.Key
                            && force.CanBeCasted && force.CanHit(this.CurrentTarget))
                        {
                            force.Cast(this.CurrentTarget);
                            await Task.Delay(force.GetCastDelay(this.CurrentTarget), token);
                            return;
                        }

                        var orchid = this.Tusk.Orchid;
                        if (orchid != null
                            && orchid.Item.Id == order.Key
                            && orchid.CanBeCasted && orchid.CanHit(this.CurrentTarget))
                        {
                            orchid.Cast(this.CurrentTarget);
                            await Task.Delay(orchid.GetCastDelay(this.CurrentTarget), token);
                            return;
                        }

                        var bloodthorn = this.Tusk.BloodThorn;
                        if (bloodthorn != null
                            && bloodthorn.Item.Id == order.Key
                            && bloodthorn.CanBeCasted && bloodthorn.CanHit(this.CurrentTarget))
                        {
                            bloodthorn.Cast(this.CurrentTarget);
                            await Task.Delay(bloodthorn.GetCastDelay(this.CurrentTarget), token);
                            return;
                        }

                        var nullifier = this.Tusk.Nullifier;
                        if (nullifier != null
                            && nullifier.Item.Id == order.Key
                            && nullifier.CanBeCasted && nullifier.CanHit(this.CurrentTarget))
                        {
                            nullifier.Cast(this.CurrentTarget);
                            await Task.Delay(nullifier.GetCastDelay(this.CurrentTarget) + nullifier.GetHitTime(this.CurrentTarget), token);
                            return;
                        }

                        var atos = this.Tusk.RodOfAtos;
                        if (atos != null
                            && atos.Item.Id == order.Key
                            && atos.CanBeCasted && atos.CanHit(this.CurrentTarget))
                        {
                            atos.Cast(this.CurrentTarget);
                            await Task.Delay(atos.GetCastDelay(this.CurrentTarget) + atos.GetHitTime(this.CurrentTarget), token);
                            return;
                        }

                        var hex = this.Tusk.Sheepstick;
                        if (hex != null
                            && hex.Item.Id == order.Key
                            && hex.CanBeCasted && hex.CanHit(this.CurrentTarget))
                        {
                            hex.Cast(this.CurrentTarget);
                            await Task.Delay(hex.GetCastDelay(this.CurrentTarget), token);
                            return;
                        }

                        var diff = this.Tusk.DiffusalBlade;
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