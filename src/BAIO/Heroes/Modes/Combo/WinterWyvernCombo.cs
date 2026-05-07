//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Reflection;
//using System.Text;
//using System.Threading;
//using System.Threading.Tasks;
//using BAIO.Heroes.Base;
//using BAIO.Modes;
//using Ensage;
//using Ensage.Common.Extensions;
//using Ensage.SDK.Handlers;
//using Ensage.SDK.Helpers;
//using Ensage.SDK.Prediction;
//using log4net;
//using PlaySharp.Toolkit.Logging;
//using SharpDX;
//using AbilityExtensions = Ensage.SDK.Extensions.AbilityExtensions;

//namespace BAIO.Heroes.Modes.Combo
//{
//    internal class WinterWyvernCombo : ComboMode
//    {
//        private static readonly ILog Log = AssemblyLogs.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

//        private readonly WinterWyvern WinterWyvern;

//        private readonly string CurseModifier = "modifier_winter_wyvern_winters_curse";

//        public TaskHandler HealHandler { get; private set; }

//        public WinterWyvernCombo(WinterWyvern hero)
//            : base(hero)
//        {
//            this.WinterWyvern = hero;
//            this.HealHandler = TaskHandler.Run(HealOnUpdate);
//        }

//        public override async Task ExecuteAsync(CancellationToken token)
//        {
//            if (!await this.ShouldExecute(token))
//            {
//                return;
//            }

//            this.MaxTargetRange = Math.Max(this.MaxTargetRange, 1200);

//            if ((this.CurrentTarget == null) || !this.CurrentTarget.IsVisible || this.CurrentTarget.HasModifier(CurseModifier))
//            {
//                this.WinterWyvern.Context.Orbwalker.OrbwalkTo(null);
//                return;
//            }

//            if (this.CurrentTarget.IsIllusion)
//            {
//                this.OrbwalkToTarget();
//                return;
//            }

//            try
//            {
//                if ((this.WinterWyvern.BlinkDagger != null) &&
//                    (this.WinterWyvern.BlinkDagger.Item.IsValid) && Owner.Distance2D(this.CurrentTarget) <= 1200 + 350 &&
//                    !(Owner.Distance2D(this.CurrentTarget) <= 400) &&
//                    this.WinterWyvern.BlinkDagger.CanBeCasted && this.WinterWyvern.UseBlink)
//                {
//                    var l = (this.Owner.Distance2D(this.CurrentTarget) - 350) / 350;
//                    var posA = this.Owner.Position;
//                    var posB = this.CurrentTarget.Position;
//                    var x = (posA.X + (l * posB.X)) / (1 + l);
//                    var y = (posA.Y + (l * posB.Y)) / (1 + l);
//                    var position = new Vector3((int)x, (int)y, posA.Z);

//                    this.WinterWyvern.BlinkDagger.Cast(position);
//                    await Task.Delay(this.WinterWyvern.BlinkDagger.GetCastDelay(position), token);
//                }

//                var linkens = this.CurrentTarget.IsLinkensProtected();
//                await BreakLinken(token);

//                var ulti = this.WinterWyvern.WintersCurse;
//                if (ulti.CanBeCasted() &&
//                    this.WinterWyvern.HeroestoUlti[((Hero)CurrentTarget).HeroId] &&
//                    this.WinterWyvern.NearHeroesCount(this.CurrentTarget) >= this.WinterWyvern.HeroCountToUlti.Value &&
//                    this.Owner.Distance2D(this.CurrentTarget) <= this.WinterWyvern.WintersCurse.CastRange &&
//                    !this.CurrentTarget.IsStunned())
//                {
//                    ulti.Cast(this.CurrentTarget);
//                    await Task.Delay((int)ulti.GetCastDelay(this.Owner, this.CurrentTarget) + 200, token);
//                }

//                var q = this.WinterWyvern.ArcticBurn;
//                if (this.Owner.AghanimState())
//                {
//                    if (!q.IsToggled && q.CanBeCasted() &&
//                        this.Owner.Distance2D(this.CurrentTarget) <=
//                        AbilityExtensions.GetAbilitySpecialData(q, "attack_range_bonus") + this.Owner.AttackRange)
//                    {
//                        q.ToggleAbility();
//                        await Task.Delay(50, token);
//                    }
//                    await Task.Delay(125, token);
//                }
//                else if (!this.Owner.AghanimState() && q.CanBeCasted() && this.Owner.Distance2D(this.CurrentTarget) <=
//                    AbilityExtensions.GetAbilitySpecialData(q, "attack_range_bonus") + this.Owner.AttackRange)
//                {
//                    q.Cast();
//                    await Task.Delay(50, token);
//                }

//                var blast = this.WinterWyvern.SplinterBlast;
//                var target = this.WinterWyvern.SplinterBlastUnit(this.CurrentTarget);
//                var modifier = this.CurrentTarget.HasModifier("modifier_winter_wyvern_winters_curse_aura") ||
//                               this.CurrentTarget.HasModifier("modifier_winter_wyvern_winters_curse")
//                    ? this.CurrentTarget.Modifiers.FirstOrDefault(x =>
//                        x.Name.Contains("modifier_winter_wyvern_winters_curse")) : null;

//                if (modifier != null && target != null)
//                {
//                    if (blast.CanBeCasted() && !this.CurrentTarget.IsMagicImmune() && !target.IsMagicImmune() &&
//                        this.Owner.Distance2D(target) <= blast.CastRange &&
//                        modifier.RemainingTime <= this.WinterWyvern.GetBlastHitTime(this.CurrentTarget) + (target.Distance2D(this.CurrentTarget) / 650))
//                    {
//                        blast.Cast(target);
//                        await Task.Delay((int)blast.GetCastDelay(this.Owner, target), token);
//                    }
//                }

//                else if (modifier == null && target != null &&
//                         (this.WinterWyvern.NearHeroesCount(this.CurrentTarget) < this.WinterWyvern.HeroCountToUlti.Value || !this.WinterWyvern.WintersCurse.CanBeCasted()))
//                {
//                    if (blast.CanBeCasted() && !this.CurrentTarget.IsMagicImmune() && !target.IsMagicImmune() &&
//                        this.Owner.Distance2D(target) <= blast.CastRange)
//                    {
//                        blast.Cast(target);
//                        await Task.Delay((int)blast.GetCastDelay(this.Owner, target), token);
//                    }
//                }

//                var abyssal = this.WinterWyvern.AbyssalBlade;
//                if (abyssal != null && abyssal.CanBeCasted && abyssal.CanHit(CurrentTarget) && !linkens &&
//                    this.WinterWyvern.AbyssalBladeHeroes[((Hero)CurrentTarget).HeroId])
//                {
//                    abyssal.Cast(CurrentTarget);
//                    await Task.Delay(abyssal.GetCastDelay(), token);
//                }

//                var manta = this.WinterWyvern.Manta;
//                if (manta != null && manta.CanBeCasted && this.Owner.Distance2D(CurrentTarget) <= 200 &&
//                    this.WinterWyvern.MantaHeroes[((Hero)CurrentTarget).HeroId])
//                {
//                    manta.Cast();
//                    await Task.Delay(manta.GetCastDelay(), token);
//                }

//                var nullifier = this.WinterWyvern.Nullifier;
//                if (nullifier != null && nullifier.CanBeCasted && nullifier.CanHit(CurrentTarget) && !linkens &&
//                    this.WinterWyvern.NullifierHeroes[((Hero)CurrentTarget).HeroId])
//                {
//                    nullifier.Cast(CurrentTarget);
//                    await Task.Delay(nullifier.GetCastDelay(), token);
//                }
//            }
//            catch (TaskCanceledException)
//            {
//                // ignore
//            }
//            catch (Exception e)
//            {
//                LogManager.Debug($"{e}");
//            }
//            this.OrbwalkToTarget();
//        }

//        private async Task HealOnUpdate(CancellationToken token)
//        {
//            if (GameManager.IsPaused)
//            {
//                await Task.Delay(250, token);
//                return;
//            }

//            if ((this.WinterWyvern.HealOnlyInCombo && !this.WinterWyvern.Config.General.ComboKey.Value))
//            {
//                await Task.Delay(250, token);
//                return;
//            }

//            var allies =
//                EntityManager.GetEntities<Hero>().Where(
//                        x =>
//                            x.IsValid && x.IsAlive && !x.IsIllusion && x.Team == this.Owner.Team &&
//                            (((float)x.Health / x.MaximumHealth) * 100f) <= this.WinterWyvern.HealThreshold.Value.Value &&
//                            x.Distance2D(this.Owner) <= 1100 &&
//                            this.WinterWyvern.HeroestoHeal.Value.IsEnabled(x.Name))
//                    .ToList();

//            if (allies.Any() && this.WinterWyvern.ColdEmbrace.CanBeCasted())
//            {
//                foreach (var ally in allies)
//                {
//                    this.WinterWyvern.ColdEmbrace.Cast(ally);
//                    await Task.Delay((int)this.WinterWyvern.ColdEmbrace.GetCastDelay(this.Owner, ally), token);
//                }
//            }
//            await Task.Delay(100, token);
//        }

//        protected async Task BreakLinken(CancellationToken token)
//        {
//            if (this.CurrentTarget != null && this.CurrentTarget.IsValid)
//            {
//                try
//                {
//                    List<KeyValuePair<AbilityId, bool>> breakerChanger = new List<KeyValuePair<AbilityId, bool>>();

//                    if (this.CurrentTarget.IsLinkensProtected())
//                    {
//                        breakerChanger = this.WinterWyvern.Config.Hero.LinkenBreakerPriorityMenu.Value.Dictionary.Where(
//                                x => this.WinterWyvern.Config.Hero.LinkenBreakerTogglerMenu.Value.IsEnabled(x.Key))
//                            .OrderByDescending(x => x.Value)
//                            .ToList();
//                    }

//                    foreach (var order in breakerChanger)
//                    {
//                        var euls = this.WinterWyvern.Euls;
//                        if (euls != null
//                            && euls.Item.Id == order.Key
//                            && euls.CanBeCasted && euls.CanHit(this.CurrentTarget))
//                        {
//                            euls.Cast(this.CurrentTarget);
//                            await Task.Delay(euls.GetCastDelay(this.CurrentTarget), token);
//                            return;
//                        }

//                        var force = this.WinterWyvern.ForceStaff;
//                        if (force != null
//                            && force.Item.Id == order.Key
//                            && force.CanBeCasted && force.CanHit(this.CurrentTarget))
//                        {
//                            force.Cast(this.CurrentTarget);
//                            await Task.Delay(force.GetCastDelay(this.CurrentTarget), token);
//                            return;
//                        }

//                        var orchid = this.WinterWyvern.Orchid;
//                        if (orchid != null
//                            && orchid.Item.Id == order.Key
//                            && orchid.CanBeCasted && orchid.CanHit(this.CurrentTarget))
//                        {
//                            orchid.Cast(this.CurrentTarget);
//                            await Task.Delay(orchid.GetCastDelay(this.CurrentTarget), token);
//                            return;
//                        }

//                        var bloodthorn = this.WinterWyvern.BloodThorn;
//                        if (bloodthorn != null
//                            && bloodthorn.Item.Id == order.Key
//                            && bloodthorn.CanBeCasted && bloodthorn.CanHit(this.CurrentTarget))
//                        {
//                            bloodthorn.Cast(this.CurrentTarget);
//                            await Task.Delay(bloodthorn.GetCastDelay(this.CurrentTarget), token);
//                            return;
//                        }

//                        var nullifier = this.WinterWyvern.Nullifier;
//                        if (nullifier != null
//                            && nullifier.Item.Id == order.Key
//                            && nullifier.CanBeCasted && nullifier.CanHit(this.CurrentTarget))
//                        {
//                            nullifier.Cast(this.CurrentTarget);
//                            await Task.Delay(nullifier.GetCastDelay(this.CurrentTarget) + nullifier.GetHitTime(this.CurrentTarget), token);
//                            return;
//                        }

//                        var atos = this.WinterWyvern.RodOfAtos;
//                        if (atos != null
//                            && atos.Item.Id == order.Key
//                            && atos.CanBeCasted && atos.CanHit(this.CurrentTarget))
//                        {
//                            atos.Cast(this.CurrentTarget);
//                            await Task.Delay(atos.GetCastDelay(this.CurrentTarget) + atos.GetHitTime(this.CurrentTarget), token);
//                            return;
//                        }

//                        var hex = this.WinterWyvern.Sheepstick;
//                        if (hex != null
//                            && hex.Item.Id == order.Key
//                            && hex.CanBeCasted && hex.CanHit(this.CurrentTarget))
//                        {
//                            hex.Cast(this.CurrentTarget);
//                            await Task.Delay(hex.GetCastDelay(this.CurrentTarget), token);
//                            return;
//                        }

//                        var diff = this.WinterWyvern.DiffusalBlade;
//                        if (diff != null
//                            && diff.Item.Id == order.Key
//                            && diff.CanBeCasted && diff.CanHit(this.CurrentTarget))
//                        {
//                            diff.Cast(this.CurrentTarget);
//                            await Task.Delay(diff.GetCastDelay(this.CurrentTarget), token);
//                            return;
//                        }
//                    }
//                }
//                catch (TaskCanceledException)
//                {
//                    // ignore
//                }
//                catch (Exception e)
//                {
//                    LogManager.Error("Linken break error: " + e);
//                }
//            }
//        }
//    }
//}