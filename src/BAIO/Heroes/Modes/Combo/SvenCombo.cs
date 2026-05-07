namespace BAIO.Heroes.Modes.Combo
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using BAIO.Modes;

    using Base;

    using Divine.Entity.Entities.Abilities.Components;
    using Divine.Entity.Entities.Units.Heroes;
    using Divine.Extensions;
    using Divine.Zero.Log;

    using Attribute = Divine.Entity.Entities.Units.Heroes.Components.Attribute;

    internal class SvenCombo : ComboMode
    {
        private readonly Sven Sven;

        private static Attribute DefaultAttribute => Attribute.Strength;

        public SvenCombo(Sven hero)
            : base(hero)
        {
            this.Sven = hero;
        }

        public override async Task ExecuteAsync(CancellationToken token)
        {
            if (!await this.ShouldExecute(token))
            {
                return;
            }

            var blink = this.Sven.BlinkDagger;
            if (blink != null)
            {
                this.MaxTargetRange = Math.Max(this.MaxTargetRange, blink.CastRange * 1.3f);
            }

            if ((this.CurrentTarget == null) || !this.CurrentTarget.IsVisible)
            {
                this.Sven.Context.Orbwalker.OrbwalkTo(null);
                return;
            }

            if (this.CurrentTarget.IsIllusion)
            {
                this.OrbwalkToTarget();
                return;
            }

            var ulti = this.Sven.Ulti;
            var stun = this.Sven.Stun;
            var warcry = this.Sven.Warcry;

            var forceStaff = this.Sven.ForceStaff;
            var forceStaffReady = (forceStaff != null) && forceStaff.CanBeCasted;
            var blinkReady = blink != null && blink.CanBeCasted;
            var distance = this.Owner.Distance2D(this.CurrentTarget);

            var linkens = this.CurrentTarget.IsLinkensProtected();
            await BreakLinken(token);

            if (blinkReady && this.Owner.Distance2D(this.CurrentTarget) > 600)
            {
                if (this.Sven.UseUltiBefore)
                {
                    if (ulti.CanBeCasted)
                    {
                        ulti.Cast();
                        await Task.Delay(ulti.GetCastDelay(), token);
                    }
                    if (warcry.CanBeCasted)
                    {
                        warcry.Cast();
                    }
                }

                var blinkPosition = this.CurrentTarget.Position.Extend(this.Owner.Position, Math.Max(100, distance - blink.CastRange));
                blink.Cast(blinkPosition);

                if (stun.CanBeCasted && stun.CanHit(this.CurrentTarget) && blinkPosition.Distance2D(this.CurrentTarget.Position) <= stun.CastRange)
                {
                    if (!linkens && this.Sven.Treads != null && this.Sven.Treads.CanBeCasted)
                    {
                        this.Sven.Treads.SwitchAttribute(Attribute.Intelligence);
                        await Task.Delay(80, token);
                        stun.Cast(this.CurrentTarget);
                        await Task.Delay(stun.GetCastDelay(this.CurrentTarget), token);
                        this.Sven.Treads.SwitchAttribute(Attribute.Strength);
                        await Task.Delay(80, token);
                    }
                    else if (!linkens)
                    {
                        stun.Cast(this.CurrentTarget);
                        await Task.Delay(stun.GetCastDelay(this.CurrentTarget), token);
                    }
                }
                else
                {
                    await Task.Delay(blink.GetCastDelay(this.CurrentTarget), token);
                }
            }

            if (stun.CanBeCasted && stun.CanHit(this.CurrentTarget))
            {
                if (!linkens && this.Sven.Treads != null && this.Sven.Treads.CanBeCasted)
                {
                    this.Sven.Treads.SwitchAttribute(Attribute.Intelligence);
                    await Task.Delay(100, token);
                    stun.Cast(this.CurrentTarget);
                    await Task.Delay(stun.GetCastDelay(this.CurrentTarget), token);
                    this.Sven.Treads.SwitchAttribute(Attribute.Strength);
                    await Task.Delay(300, token);
                }
                else if (!linkens)
                {
                    stun.Cast(this.CurrentTarget);
                    await Task.Delay(stun.GetCastDelay(this.CurrentTarget), token);
                }
            }

            if (distance <= 400)
            {
                if (ulti.CanBeCasted)
                {
                    ulti.Cast();
                    await Task.Delay(ulti.GetCastDelay(), token);
                }
                if (warcry.CanBeCasted)
                {
                    warcry.Cast();
                }
            }

            var mom = this.Sven.Mom;
            var cantCast = !ulti.CanBeCasted && !stun.CanBeCasted && !warcry.CanBeCasted;

            if (mom != null && mom.CanBeCasted && cantCast)
            {
                mom.Cast();
                await Task.Delay(mom.GetCastDelay(), token);
            }

            var halberd = this.Sven.HeavensHalberd;
            if (halberd != null && halberd.CanBeCasted && halberd.CanHit(CurrentTarget) && !linkens &&
                this.Sven.HalberdHeroes[((Hero)CurrentTarget).HeroId])
            {
                halberd.Cast(CurrentTarget);
                await Task.Delay(halberd.GetCastDelay(), token);
            }

            var bloodthorn = this.Sven.BloodThorn;
            if (bloodthorn != null && bloodthorn.CanBeCasted && bloodthorn.CanHit(CurrentTarget) && !linkens &&
                this.Sven.BtOrchidHeroes[((Hero)CurrentTarget).HeroId])
            {
                bloodthorn.Cast(CurrentTarget);
                await Task.Delay(bloodthorn.GetCastDelay(), token);
            }

            var orchid = this.Sven.Orchid;
            if (orchid != null && orchid.CanBeCasted && orchid.CanHit(CurrentTarget) && !linkens &&
                this.Sven.BtOrchidHeroes[((Hero)CurrentTarget).HeroId])
            {
                orchid.Cast(CurrentTarget);
                await Task.Delay(orchid.GetCastDelay(), token);
            }

            var bkb = this.Sven.BlackKingBar;
            if (bkb != null && bkb.CanBeCasted && this.Sven.BkbHeroes[((Hero)CurrentTarget).HeroId])
            {
                bkb.Cast();
                await Task.Delay(bkb.GetCastDelay(), token);
            }

            var mjollnir = this.Sven.Mjollnir;
            if (mjollnir != null && mjollnir.CanBeCasted)
            {
                mjollnir.Cast(this.Owner);
                await Task.Delay(mjollnir.GetCastDelay(), token);
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
                        breakerChanger = this.Sven.Config.Hero.LinkenBreakerTogglerMenu.Values.ToList();
                    }

                    foreach (var order in breakerChanger)
                    {
                        var euls = this.Sven.Euls;
                        if (euls != null
                            && euls.Item.Id == order.Key
                            && euls.CanBeCasted && euls.CanHit(this.CurrentTarget))
                        {
                            euls.Cast(this.CurrentTarget);
                            await Task.Delay(euls.GetCastDelay(this.CurrentTarget), token);
                            return;
                        }

                        var force = this.Sven.ForceStaff;
                        if (force != null
                            && force.Item.Id == order.Key
                            && force.CanBeCasted && force.CanHit(this.CurrentTarget))
                        {
                            force.Cast(this.CurrentTarget);
                            await Task.Delay(force.GetCastDelay(this.CurrentTarget), token);
                            return;
                        }

                        var orchid = this.Sven.Orchid;
                        if (orchid != null
                            && orchid.Item.Id == order.Key
                            && orchid.CanBeCasted && orchid.CanHit(this.CurrentTarget))
                        {
                            orchid.Cast(this.CurrentTarget);
                            await Task.Delay(orchid.GetCastDelay(this.CurrentTarget), token);
                            return;
                        }

                        var bloodthorn = this.Sven.BloodThorn;
                        if (bloodthorn != null
                            && bloodthorn.Item.Id == order.Key
                            && bloodthorn.CanBeCasted && bloodthorn.CanHit(this.CurrentTarget))
                        {
                            bloodthorn.Cast(this.CurrentTarget);
                            await Task.Delay(bloodthorn.GetCastDelay(this.CurrentTarget), token);
                            return;
                        }

                        var nullifier = this.Sven.Nullifier;
                        if (nullifier != null
                            && nullifier.Item.Id == order.Key
                            && nullifier.CanBeCasted && nullifier.CanHit(this.CurrentTarget))
                        {
                            nullifier.Cast(this.CurrentTarget);
                            await Task.Delay(nullifier.GetCastDelay(this.CurrentTarget) + nullifier.GetHitTime(this.CurrentTarget), token);
                            return;
                        }

                        var atos = this.Sven.RodOfAtos;
                        if (atos != null
                            && atos.Item.Id == order.Key
                            && atos.CanBeCasted && atos.CanHit(this.CurrentTarget))
                        {
                            atos.Cast(this.CurrentTarget);
                            await Task.Delay(atos.GetCastDelay(this.CurrentTarget) + atos.GetHitTime(this.CurrentTarget), token);
                            return;
                        }

                        var hex = this.Sven.Sheepstick;
                        if (hex != null
                            && hex.Item.Id == order.Key
                            && hex.CanBeCasted && hex.CanHit(this.CurrentTarget))
                        {
                            hex.Cast(this.CurrentTarget);
                            await Task.Delay(hex.GetCastDelay(this.CurrentTarget), token);
                            return;
                        }

                        var diff = this.Sven.DiffusalBlade;
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
