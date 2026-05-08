using Divine.Core.ComboFactory.Combos;
using Divine.Core.ComboFactory.Menus.Combo;
using Divine.Core.Extensions;
using Divine.Core.Helpers;
using Divine.Entity.Entities.Abilities.Components;

namespace Divine.SkywrathMage.Combos
{
    internal sealed class KillSteal : BaseKillSteal
    {
        private readonly BaseComboMenu ComboMenu;

        private readonly Abilities Abilities;

        private readonly BaseLinkenBreaker LinkenBreaker;

        public KillSteal(Common common)
            : base(common.MenuConfig)
        {
            ComboMenu = common.MenuConfig.ComboMenu;

            Abilities = (Abilities)common.Abilities;

            LinkenBreaker = common.LinkenBreaker;
        }

        protected override async Task ExecuteAsync(CancellationToken token)
        {
            try
            {
                if (KillStealMenu.DisableWhenComboItem && ComboMenu.ComboHotkeyItem || IsStopped)
                {
                    return;
                }

                var target = CurrentTarget;
                if (IsNullTarget(target))
                {
                    return;
                }

                var targetModifiers = new TargetModifiers(target, "modifier_dazzle_shallow_grave", "modifier_necrolyte_reapers_scythe", "modifier_templar_assassin_refraction_absorb");
                if (Owner.IsInvisible() || target.IsBlockMagicDamage(targetModifiers) || target.ComboBreaker() || Reincarnation(target) || targetModifiers.IsModifiers)
                {
                    return;
                }

                if (target.IsShieldAbilities(targetModifiers) && !targetModifiers.IsSilverDebuff)
                {
                    LinkenBreaker.RunAsync();
                    return;
                }

                // Ancient Seal
                var ancientSeal = Abilities.AncientSeal;
                if (KillStealMenu.AbilitiesSelection[ancientSeal.Id]
                    && ancientSeal.CanBeCasted
                    && ancientSeal.CanHit(target))
                {
                    ancientSeal.UseAbility(target);
                    await Task.Delay(ancientSeal.GetCastDelay(target), token);
                    return;
                }

                // Veil
                var veil = Abilities.Veil;
                if (veil != null
                    && KillStealMenu.AbilitiesSelection[veil.Id]
                    && veil.CanBeCasted
                    && veil.CanHit(target))
                {
                    veil.UseAbility(target.Position);
                    await Task.Delay(veil.GetCastDelay(target.Position), token);
                }

                // Ethereal
                var ethereal = Abilities.Ethereal;
                if (ethereal != null
                    && KillStealMenu.AbilitiesSelection[ethereal.Id]
                    && ethereal.CanBeCasted
                    && ethereal.CanHit(target))
                {
                    ethereal.UseAbility(target);
                    ZMultiSleeper<string>.Sleep($"IsHitTime_{target.Name}_{ethereal.Name}", ethereal.GetHitTime(target));
                    await Task.Delay(ethereal.GetCastDelay(target), token);
                    return;
                }

                // Shivas
                var shivas = Abilities.Shivas;
                if (shivas != null
                    && KillStealMenu.AbilitiesSelection[shivas.Id]
                    && shivas.CanBeCasted
                    && shivas.CanHit(target))
                {
                    shivas.UseAbility();
                    await Task.Delay(shivas.GetCastDelay(), token);
                }

                if (!ZMultiSleeper<string>.Sleeping($"IsHitTime_{target.Name}_item_ethereal_blade") || targetModifiers.IsEthereal)
                {
                    // Concussive Shot
                    var concussiveShot = Abilities.ConcussiveShot;
                    if (KillStealMenu.AbilitiesSelection[concussiveShot.Id]
                        && concussiveShot.CanBeCasted
                        && concussiveShot.CanHit(target))
                    {
                        var targetHit = concussiveShot.TargetHit;
                        if (target == targetHit)
                        {
                            concussiveShot.UseAbility();
                            await Task.Delay(concussiveShot.GetCastDelay(), token);
                        }
                    }

                    // Arcane Bolt
                    var arcaneBolt = Abilities.ArcaneBolt;
                    var arcaneBoltId = arcaneBolt.Id;
                    if (KillStealMenu.AbilitiesSelection[arcaneBoltId]
                        && arcaneBolt.CanBeCasted
                        && arcaneBolt.CanHit(target))
                    {
                        arcaneBolt.UseAbility(target);
                        var castDelay = arcaneBolt.GetCastDelay(target);
                        var hitTime = arcaneBolt.GetHitTime(target) - (castDelay + 340);
                        ZMultiSleeper<string>.DelaySleep($"IsHitTime_{target.Name}_{arcaneBoltId}", castDelay + 40, hitTime);
                        await Task.Delay(castDelay, token);
                        return;
                    }

                    // Dagon
                    var dagon = Abilities.Dagon;
                    if (dagon != null
                        && KillStealMenu.AbilitiesSelection[AbilityId.item_dagon_5]
                        && dagon.CanBeCasted
                        && dagon.CanHit(target))
                    {
                        dagon.UseAbility(target);
                        await Task.Delay(dagon.GetCastDelay(target), token);
                        return;
                    }
                }
            }
            catch (TaskCanceledException)
            {
                // canceled
            }
            catch (Exception e)
            {
                Logger.LogError(e);
            }
        }
    }
}
