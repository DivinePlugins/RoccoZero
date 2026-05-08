using Divine.Core.ComboFactory.Combos;
using Divine.Core.ComboFactory.Menus.Combo;
using Divine.Core.Extensions;
using Divine.Core.Helpers;
using Divine.Entity.Entities.Abilities.Components;
using Divine.Extensions;
using Divine.GameConsole;
using Divine.Update;
using Divine.Zeus.Menus;

namespace Divine.Zeus.Combos
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

                StopCast();

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
                    // LightningBolt
                    var lightningBolt = Abilities.LightningBolt;
                    if (KillStealMenu.AbilitiesSelection[lightningBolt.Id] && lightningBolt.CanBeCasted)
                    {
                        var position = lightningBolt.LightningBoltPrediction(target, false);
                        if (!position.IsDefault)
                        {
                            lightningBolt.UseAbility(position);
                            await Task.Delay(lightningBolt.GetCastDelay(position), token);
                        }
                    }

                    // ArcLightning
                    var arcLightning = Abilities.ArcLightning;
                    if (KillStealMenu.AbilitiesSelection[arcLightning.Id]
                        && arcLightning.CanBeCasted
                        && arcLightning.CanHit(target))
                    {
                        arcLightning.UseAbility(target);
                        await Task.Delay(arcLightning.GetCastDelay(target), token);
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

                    // Nimbus
                    var nimbus = Abilities.Nimbus;
                    if (KillStealMenu.AbilitiesSelection[nimbus.Id] && nimbus.CanBeCasted)
                    {
                        var position = target.Position;
                        MoveCamera(position);

                        nimbus.UseAbility(position);
                        await Task.Delay(nimbus.GetCastDelay(position), token);
                        return;
                    }

                    // ThundergodsWrath
                    var thundergodsWrath = Abilities.ThundergodsWrath;
                    if (KillStealMenu.AbilitiesSelection[thundergodsWrath.Id] && thundergodsWrath.CanBeCasted)
                    {
                        MoveCamera(target.Position);

                        thundergodsWrath.UseAbility();
                        await Task.Delay(thundergodsWrath.GetCastDelay(), token);
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

        private readonly ZSleeper cameraSleeper = new ZSleeper();

        private void MoveCamera(Vector3 position)
        {
            var killStealMenu = (KillStealMenu)KillStealMenu;
            if (killStealMenu.MoveCameraItem)
            {
                if (!position.IsOnScreen() && !cameraSleeper.Sleeping)
                {
                    GameConsoleManager.ExecuteCommand($"dota_camera_set_lookatpos {position.X} {position.Y}");

                    UpdateManager.BeginInvoke((int)killStealMenu.CameraDelayItem, () =>
                    {
                        GameConsoleManager.ExecuteCommand("+dota_camera_center_on_hero");
                    });

                    cameraSleeper.Sleep(3000);
                }
            }
        }
    }
}
