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
//using log4net;
//using PlaySharp.Toolkit.Logging;
//using Ensage.SDK.Extensions;
//using Ensage.SDK.Helpers;
//using UnitExtensions = Ensage.Common.Extensions.UnitExtensions;
//using Ensage.SDK.Handlers;
//using AbilityExtensions = Ensage.Common.Extensions.AbilityExtensions;

//namespace BAIO.Heroes.Modes.Combo
//{
//    internal class JuggernautCombo : ComboMode
//    {
//        private static readonly ILog Log = AssemblyLogs.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

//        private readonly Juggernaut Juggernaut;

//        public TaskHandler IllusionHandler { get; private set; }

//        public JuggernautCombo(Juggernaut hero)
//            : base(hero)
//        {
//            Juggernaut = hero;
//            this.IllusionHandler = TaskHandler.Run(OnUpdate);
//        }

//        public override async Task ExecuteAsync(CancellationToken token)
//        {
//            if (!await ShouldExecute(token))
//            {
//                return;
//            }

//            if (Owner.HasModifier(Juggernaut.Omnislash.ModifierName) && this.CurrentTarget.IsEthereal())
//            {
//                var nullifier = Juggernaut.Nullifier;
//                if (nullifier != null && nullifier.CanBeCasted && nullifier.CanHit(this.CurrentTarget))
//                {
//                    nullifier.Cast(this.CurrentTarget);
//                    await Task.Delay(nullifier.GetCastDelay(), token);
//                }
//                await Task.Delay(125, token);
//            }

//            var blink = Juggernaut.BlinkDagger;
//            if (blink != null)
//            {
//                MaxTargetRange = blink.CastRange * 1.5f;
//            }

//            if (this.CurrentTarget == null || !this.CurrentTarget.IsVisible)
//            {
//                Juggernaut.Context.Orbwalker.OrbwalkTo(null);
//                return;
//            }

//            if (this.CurrentTarget.IsIllusion)
//            {
//                OrbwalkToTarget();
//                return;
//            }

//            var isInvis = this.Owner.IsInvisible();

//            var sBlade = this.Juggernaut.ShadowBlade;
//            var sEdge = this.Juggernaut.SilverEdge;

//            if (!isInvis && sBlade?.CanBeCasted == true || sEdge?.CanBeCasted == true
//                && this.Owner.Distance2D(this.CurrentTarget) < 2000 &&
//                this.Juggernaut.InvisHeroes[((Hero)CurrentTarget).HeroId])
//            {
//                sBlade?.Cast();
//                sEdge?.Cast();
//            }


//            if (isInvis && this.CurrentTarget != null && this.Owner.CanAttack() &&
//                this.CurrentTarget.Distance2D(this.Owner) <= this.Owner.AttackRange)
//            {
//                this.Owner.Attack(this.CurrentTarget);
//                await Task.Delay((int) (UnitExtensions.AttackBackswing(this.Owner) + GameManager.Ping), token);
//            }

//            var linkens = this.CurrentTarget.IsLinkensProtected();
//            await BreakLinken(token);

//            var omni = Juggernaut.Omnislash;
//            var swift = Juggernaut.SwiftSlash;
//            var healthPercent = Owner.HealthPercent();
//            var targetDistance = Owner.Distance2D(this.CurrentTarget);
//            var attackRange = Owner.AttackRange(this.CurrentTarget);
//            if (omni != null && !isInvis && !this.CurrentTarget.IsIllusion && omni.CanBeCasted &&
//                omni.CanHit(this.CurrentTarget))
//            {
//                var useOmni = healthPercent < 0.15f;
//                if (!useOmni)
//                    if (targetDistance > attackRange * 1.4f &&
//                        Owner.MovementSpeed < this.CurrentTarget.MovementSpeed * 1.2f
//                        || this.CurrentTarget.Health > omni.GetTickDamage(this.CurrentTarget) && this.Juggernaut.OmniHeroes[((Hero)CurrentTarget).HeroId])
//                    {
//                        var unitsClose = EntityManager.GetEntities<Unit>().Where(
//                                x => x.IsVisible
//                                     && x.IsAlive
//                                     && x != this.CurrentTarget
//                                     && Owner.IsEnemy(x)
//                                     && (x.IsIllusion || !(x is Hero))
//                                     && !(x is Building)
//                                     && x.IsRealUnit()
//                                     && x.Distance2D(this.CurrentTarget) <= omni.Radius)
//                            .ToList();

//                        var abilityLevel = omni.Ability.Level - 1;

//                        if (unitsClose.Count > 0 && unitsClose.Count <= abilityLevel)
//                        {
//                            var close = unitsClose;
//                            foreach (var unit in close)
//                            {
//                                var close1 = unitsClose;
//                                var unitsInRadius = EntityManager.GetEntities<Unit>().Where(
//                                    x => !close1.Contains(x)
//                                         && x.IsVisible
//                                         && x.IsAlive
//                                         && x != this.CurrentTarget
//                                         && Owner.IsEnemy(x)
//                                         && (x.IsIllusion || !(x is Hero))
//                                         && !(x is Building)
//                                         && x.IsRealUnit()
//                                         && x.Distance2D(unit) < omni.Radius);
//                                unitsClose = unitsClose.Concat(unitsInRadius).ToList();

//                                if (unitsClose.Count > abilityLevel)
//                                    break;
//                            }
//                        }

//                        useOmni = unitsClose.Count <= abilityLevel;
//                    }

//                if (useOmni)
//                {
//                    Unit omniTarget;
//                    if (this.CurrentTarget.IsReflectingAbilities())
//                        omniTarget = EntityManager<Unit>
//                            .Entities.Where(
//                                x => x.IsVisible
//                                     && x.IsAlive
//                                     && Owner.IsEnemy(x)
//                                     && !(x is Building)
//                                     && x.IsRealUnit()
//                                     && !x.IsReflectingAbilities()
//                                     && x.Distance2D(Owner) < omni.CastRange)
//                            .OrderBy(x => x.Distance2D(this.CurrentTarget))
//                            .FirstOrDefault();
//                    else
//                        omniTarget = this.CurrentTarget;

//                    if (omniTarget != null)
//                    {
//                        omni.Cast(omniTarget);
//                        await Task.Delay(omni.GetCastDelay(omniTarget), token);
//                    }
//                }
//            }

//            if (swift != null && !isInvis && !this.CurrentTarget.IsIllusion && AbilityExtensions.CanBeCasted(swift) &&
//                swift.CanHit(this.CurrentTarget) && !this.CurrentTarget.IsReflectingAbilities())
//            {
//                if (swift.Cast(this.CurrentTarget))
//                {
//                    await Task.Delay(omni.GetCastDelay(this.CurrentTarget), token);
//                }
//            }

//            var bladeFury = Juggernaut.BladeFury;
//            if (blink != null && !this.CurrentTarget.IsIllusion && blink.CanBeCasted &&
//                blink.CanHit(this.CurrentTarget) && this.Juggernaut.BladeFuryOnCombo)
//            {
//                var useBlink = omni != null && omni.CanBeCasted && !omni.CanHit(this.CurrentTarget);
//                if (!useBlink)
//                    if (targetDistance > 600)
//                    {
//                        var enemyCount = EntityManager.GetEntities<Hero>().Count(
//                            x => x.IsAlive
//                                 && x.IsVisible
//                                 && x != this.CurrentTarget
//                                 && Owner.IsEnemy(x)
//                                 && !x.IsIllusion
//                                 && x.Distance2D(this.CurrentTarget) < 800);
//                        useBlink = enemyCount <= 1 || bladeFury != null && bladeFury.CanBeCasted;
//                    }

//                if (useBlink)
//                {
//                    var blinkPos = this.CurrentTarget.IsMoving
//                        ? this.CurrentTarget.InFront(75)
//                        : this.CurrentTarget.Position;
//                    blink.Cast(blinkPos);
//                    await Task.Delay(blink.GetCastDelay(blinkPos), token);
//                }
//            }

//            var mom = this.Juggernaut.Mom;
//            var cantCast = this.Owner.HasModifier(this.Juggernaut.Omnislash.ModifierName);

//            if (mom != null && mom.CanBeCasted && cantCast)
//            {
//                mom.Cast();
//                await Task.Delay(mom.GetCastDelay(), token);
//            }

//            var solarCrest = this.Juggernaut.SolarCrest;
//            if (solarCrest != null && solarCrest.CanBeCasted && solarCrest.CanHit(this.CurrentTarget))
//            {
//                solarCrest.Cast(this.CurrentTarget);
//                await Task.Delay(solarCrest.GetCastDelay(), token);
//            }

//            var bloodthorn = this.Juggernaut.BloodThorn;
//            if (bloodthorn != null && bloodthorn.CanBeCasted && bloodthorn.CanHit(CurrentTarget) && !linkens)
//            {
//                bloodthorn.Cast(this.CurrentTarget);
//                await Task.Delay(bloodthorn.GetCastDelay(), token);
//            }

//            var orchid = this.Juggernaut.Orchid;
//            if (orchid != null && orchid.CanBeCasted && orchid.CanHit(CurrentTarget) && !linkens)
//            {
//                orchid.Cast(this.CurrentTarget);
//                await Task.Delay(orchid.GetCastDelay(), token);
//            }

//            var healingWard = Juggernaut.HealingWardAbility;
//            if (healingWard != null && !isInvis && healingWard.CanBeCasted)
//            {
//                var recentDmgPercent = (float) Juggernaut.Owner.RecentDamage / Juggernaut.Owner.MaximumHealth;

//                if (healthPercent < 0.35f || recentDmgPercent > 0.2)
//                {
//                    healingWard.Cast(Owner.Position);
//                    await Task.Delay(healingWard.GetCastDelay(Owner.Position), token);
//                }
//            }

//            if (!this.CurrentTarget.IsStunned() && !linkens && !this.CurrentTarget.IsIllusion &&
//                Juggernaut.AbyssalBladeHeroes[((Hero)CurrentTarget).HeroId])
//            {
//                var abysal = Juggernaut.AbyssalBlade;
//                if (abysal != null && abysal.CanBeCasted && abysal.CanHit(this.CurrentTarget))
//                {
//                    abysal.Cast(this.CurrentTarget);
//                    await Task.Delay(abysal.GetCastDelay(this.CurrentTarget), token);
//                }
//            }

//            var manta = Juggernaut.Manta;
//            if (manta != null && manta.CanBeCasted && !isInvis &&
//                (targetDistance < attackRange * 1.3f && Juggernaut.MantaHeroes[((Hero)CurrentTarget).HeroId]
//                 || Owner.IsSilenced() || Owner.IsRooted()))
//            {
//                var isSilenced = Owner.IsSilenced();
//                manta.Cast();
//                await Task.Delay(manta.GetCastDelay(), token);
//                if (isSilenced)
//                    return;
//            }

//            var mjollnir = Juggernaut.Mjollnir;
//            if (mjollnir != null && mjollnir.CanBeCasted && !isInvis && mjollnir.CanHit(this.CurrentTarget))
//            {
//                mjollnir.Cast(Owner);
//                await Task.Delay(mjollnir.GetCastDelay(), token);
//            }

//            var diffusal = Juggernaut.DiffusalBlade;
//            if (diffusal != null && !linkens && !this.CurrentTarget.IsIllusion && diffusal.CanBeCasted && !isInvis &&
//                diffusal.CanHit(this.CurrentTarget) &&
//                Juggernaut.DiffusalHeroes[((Hero)CurrentTarget).HeroId])
//            {
//                diffusal.Cast(CurrentTarget);
//                await Task.Delay(diffusal.GetCastDelay(), token);
//            }

//            var nullfier = Juggernaut.Nullifier;
//            if (nullfier != null && !linkens && !this.CurrentTarget.IsIllusion && nullfier.CanBeCasted && !isInvis &&
//                nullfier.CanHit(this.CurrentTarget))
//            {
//                nullfier.Cast(this.CurrentTarget);
//                await Task.Delay(nullfier.GetCastDelay(this.CurrentTarget), token);
//            }

//            if (bladeFury != null && bladeFury.CanBeCasted && !isInvis && bladeFury.CanHit(CurrentTarget) && this.Juggernaut.BladeFuryOnCombo)
//            {
//                var enemyCount = EntityManager.GetEntities<Hero>().Count(
//                    x => x.IsAlive
//                         && x.IsVisible
//                         && x != this.CurrentTarget
//                         && this.Owner.IsEnemy(x)
//                         && !x.IsIllusion
//                         && x.Distance2D(this.CurrentTarget) < 800);

//                if (enemyCount > 1
//                    || !this.CurrentTarget.IsIllusion
//                    && bladeFury.GetTickDamage(this.CurrentTarget) >
//                    Owner.GetAttackDamage(this.CurrentTarget) * bladeFury.TickRate
//                    && bladeFury.GetTotalDamage(this.CurrentTarget) >= 0.5f * this.CurrentTarget.Health)
//                {
//                    bladeFury.Cast();
//                    await Task.Delay(bladeFury.GetCastDelay(), token);
//                }
//            }

//            if (this.Juggernaut.BladeFuryOnlyChase && this.Owner.HasModifier(bladeFury.ModifierName))
//            {
//                this.Owner.Move(this.CurrentTarget.InFront(20));
//            }
//            else
//            {
//                this.OrbwalkToTarget();
//            }
//        }

//        private async Task OnUpdate(CancellationToken token)
//        {
//            if (GameManager.IsPaused || !this.Juggernaut.Config.General.ComboKey.Value)
//            {
//                await Task.Delay(250, token);
//                return;
//            }

//            var illusions =
//                EntityManager.GetEntities<Unit>().Where(
//                        x => x.IsValid && x.IsAlive && x.IsIllusion && x.Team == this.Owner.Team && x.IsControllable)
//                    .ToList();

//            if (illusions.Any())
//            {
//                foreach (var illusion in illusions)
//                {
//                    illusion.Attack(this.CurrentTarget);
//                    await Task.Delay(100, token);
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
//                        breakerChanger = this.Juggernaut.Config.Hero.LinkenBreakerPriorityMenu.Value.Dictionary.Where(
//                                x => this.Juggernaut.Config.Hero.LinkenBreakerTogglerMenu.Value.IsEnabled(x.Key))
//                            .OrderByDescending(x => x.Value)
//                            .ToList();
//                    }

//                    foreach (var order in breakerChanger)
//                    {
//                        var euls = this.Juggernaut.Euls;
//                        if (euls != null
//                            && euls.Item.Id == order.Key
//                            && euls.CanBeCasted && euls.CanHit(this.CurrentTarget))
//                        {
//                            euls.Cast(this.CurrentTarget);
//                            await Task.Delay(euls.GetCastDelay(this.CurrentTarget), token);
//                            return;
//                        }

//                        var force = this.Juggernaut.ForceStaff;
//                        if (force != null
//                            && force.Item.Id == order.Key
//                            && force.CanBeCasted && force.CanHit(this.CurrentTarget))
//                        {
//                            force.Cast(this.CurrentTarget);
//                            await Task.Delay(force.GetCastDelay(this.CurrentTarget), token);
//                            return;
//                        }

//                        var orchid = this.Juggernaut.Orchid;
//                        if (orchid != null
//                            && orchid.Item.Id == order.Key
//                            && orchid.CanBeCasted && orchid.CanHit(this.CurrentTarget))
//                        {
//                            orchid.Cast(this.CurrentTarget);
//                            await Task.Delay(orchid.GetCastDelay(this.CurrentTarget), token);
//                            return;
//                        }

//                        var bloodthorn = this.Juggernaut.BloodThorn;
//                        if (bloodthorn != null
//                            && bloodthorn.Item.Id == order.Key
//                            && bloodthorn.CanBeCasted && bloodthorn.CanHit(this.CurrentTarget))
//                        {
//                            bloodthorn.Cast(this.CurrentTarget);
//                            await Task.Delay(bloodthorn.GetCastDelay(this.CurrentTarget), token);
//                            return;
//                        }

//                        var nullifier = this.Juggernaut.Nullifier;
//                        if (nullifier != null
//                            && nullifier.Item.Id == order.Key
//                            && nullifier.CanBeCasted && nullifier.CanHit(this.CurrentTarget))
//                        {
//                            nullifier.Cast(this.CurrentTarget);
//                            await Task.Delay(
//                                nullifier.GetCastDelay(this.CurrentTarget) + nullifier.GetHitTime(this.CurrentTarget),
//                                token);
//                            return;
//                        }

//                        var atos = this.Juggernaut.RodOfAtos;
//                        if (atos != null
//                            && atos.Item.Id == order.Key
//                            && atos.CanBeCasted && atos.CanHit(this.CurrentTarget))
//                        {
//                            atos.Cast(this.CurrentTarget);
//                            await Task.Delay(
//                                atos.GetCastDelay(this.CurrentTarget) + atos.GetHitTime(this.CurrentTarget), token);
//                            return;
//                        }

//                        var hex = this.Juggernaut.Sheepstick;
//                        if (hex != null
//                            && hex.Item.Id == order.Key
//                            && hex.CanBeCasted && hex.CanHit(this.CurrentTarget))
//                        {
//                            hex.Cast(this.CurrentTarget);
//                            await Task.Delay(hex.GetCastDelay(this.CurrentTarget), token);
//                            return;
//                        }

//                        var diff = this.Juggernaut.DiffusalBlade;
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