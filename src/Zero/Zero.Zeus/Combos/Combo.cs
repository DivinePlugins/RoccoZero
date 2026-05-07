namespace Divine.Zeus.Combos;

using System;
using System.Threading;
using System.Threading.Tasks;

using Divine.Core.ComboFactory.Combos;
using Divine.Core.ComboFactory.Menus;
using Divine.Core.ComboFactory.Menus.Combo;
using Divine.Core.Entities;
using Divine.Core.Extensions;
using Divine.Core.Helpers;
using Divine.Core.Managers.TargetSelector;
using Divine.Entity.Entities;
using Divine.Entity.Entities.Abilities;
using Divine.Entity.Entities.Abilities.Components;
using Divine.Entity.Entities.EventArgs;
using Divine.Extensions;
using Divine.Game;
using Divine.Update;
using Divine.Common.Log;
using Divine.Zeus.Menus.Combo;

internal sealed class Combo : BaseCombo
{
    private readonly BaseSpellsMenu SpellsMenu;

    private readonly BaseItemsMenu ItemsMenu;

    private readonly LightningBoltMenu LightningBoltMenu;

    private readonly ThundergodsWrathMenu ThundergodsWrathMenu;

    private readonly BaseBlinkDaggerMenu BlinkDaggerMenu;

    private readonly BaseAeonDiskMenu AeonDiskMenu;

    private readonly BaseWithMuteMenu WithMuteMenu;

    private readonly BaseBladeMailMenu BladeMailMenu;

    private readonly Abilities Abilities;

    private readonly TargetSelectorManager TargetSelector;

    private readonly BaseLinkenBreaker LinkenBreaker;

    public Combo(Common common)
        : base(common.MenuConfig)
    {
        SpellsMenu = common.MenuConfig.ComboMenu.SpellsMenu;
        ItemsMenu = common.MenuConfig.ComboMenu.ItemsMenu;
        LightningBoltMenu = ((ComboMenu)common.MenuConfig.ComboMenu).LightningBoltMenu;
        ThundergodsWrathMenu = ((ComboMenu)common.MenuConfig.ComboMenu).ThundergodsWrathMenu;
        BlinkDaggerMenu = common.MenuConfig.ComboMenu.BlinkDaggerMenu;
        AeonDiskMenu = common.MenuConfig.ComboMenu.AeonDiskMenu;
        WithMuteMenu = common.MenuConfig.ComboMenu.WithMuteMenu;
        BladeMailMenu = common.MenuConfig.BladeMailMenu;

        Abilities = (Abilities)common.Abilities;
        TargetSelector = common.TargetSelector;
        LinkenBreaker = common.LinkenBreaker;

        Entity.NetworkPropertyChanged += OnNetworkPropertyChanged;
    }

    public override void Dispose()
    {
        base.Dispose();

        Entity.NetworkPropertyChanged -= OnNetworkPropertyChanged;
    }

    protected override CUnit CurrentTarget
    {
        get
        {
            return TargetSelector.Target;
        }
    }

    private bool isLightningBoltOnPosition;

    private void OnNetworkPropertyChanged(Entity sender, NetworkPropertyChangedEventArgs e)
    {
        if (e.PropertyName != "m_bInAbilityPhase" || e.NewValue.GetBoolean())
        {
            return;
        }

        UpdateManager.BeginInvoke(() =>
        {
            var ability = sender as Ability;
            if (ability == null || ability.Id != AbilityId.zuus_lightning_bolt)
            {
                return;
            }

            isLightningBoltOnPosition = true;

            UpdateManager.BeginInvoke(600, () =>
            {
                isLightningBoltOnPosition = false;
            });
        });
    }

    protected override async Task ExecuteAsync(CancellationToken token)
    {
        try
        {
            if (IsStopped)
            {
                return;
            }

            var target = CurrentTarget;
            if (IsNullTarget(target))
            {
                MoveToMousePosition();
                return;
            }

            var targetModifiers = new TargetModifiers(target);
            if (BladeMailMenu.BladeMailItem && targetModifiers.ModifierReflect != null)
            {
                MoveToMousePosition();
                return;
            }

            // Blink
            var blink = Abilities.Blink;
            if (blink != null
                && ItemsMenu.ItemsSelection[blink.Id]
                && Owner.Distance2D(GameManager.MousePosition) > BlinkDaggerMenu.BlinkDistanceEnemyItem.Value
                && Owner.Distance2D(target) > 600
                && blink.CanBeCasted)
            {
                var blinkPos = target.Position.Extend(GameManager.MousePosition, BlinkDaggerMenu.BlinkDistanceEnemyItem.Value);
                if (Owner.Distance2D(blinkPos) < blink.CastRange)
                {
                    blink.UseAbility(blinkPos);
                    await Task.Delay(blink.GetCastDelay(blinkPos), token);
                }
            }

            if (!target.IsBlockMagicDamage(targetModifiers) && WithMute(target))
            {
                if (!target.IsShieldAbilities(targetModifiers) || targetModifiers.IsSilverDebuff)
                {
                    var comboBreaker = AeonDiskMenu.EnableItem && target.ComboBreaker();

                    // Hex
                    var modifierStun = targetModifiers.ModifierStun;
                    var modifierHex = targetModifiers.ModifierHex;
                    var hex = Abilities.Hex;
                    if (hex != null
                        && ItemsMenu.ItemsSelection[hex.Id]
                        && !comboBreaker
                        && hex.CanBeCasted
                        && hex.CanHit(target)
                        && modifierStun.IsModifier(0.3f)
                        && modifierHex.IsModifier(0.3f))
                    {
                        hex.UseAbility(target);
                        await Task.Delay(hex.GetCastDelay(target), token);
                    }

                    // Orchid
                    var orchid = Abilities.Orchid;
                    if (orchid != null
                        && ItemsMenu.ItemsSelection[orchid.Id]
                        && !comboBreaker
                        && orchid.CanBeCasted
                        && orchid.CanHit(target))
                    {
                        orchid.UseAbility(target);
                        await Task.Delay(orchid.GetCastDelay(target), token);
                    }

                    // Bloodthorn
                    var bloodthorn = Abilities.Bloodthorn;
                    if (bloodthorn != null
                        && ItemsMenu.ItemsSelection[bloodthorn.Id]
                        && !comboBreaker
                        && bloodthorn.CanBeCasted
                        && bloodthorn.CanHit(target))
                    {
                        bloodthorn.UseAbility(target);
                        await Task.Delay(bloodthorn.GetCastDelay(target), token);
                    }

                    // Nullifier
                    var nullifier = Abilities.Nullifier;
                    if (nullifier != null
                        && ItemsMenu.ItemsSelection[nullifier.Id]
                        && !comboBreaker
                        && nullifier.CanBeCasted
                        && nullifier.CanHit(target)
                        && modifierStun.IsModifier(0.5f)
                        && modifierHex.IsModifier(0.5f))
                    {
                        nullifier.UseAbility(target);
                        await Task.Delay(nullifier.GetCastDelay(target), token);
                    }

                    var modifierAtos = targetModifiers.ModifierAtos;
                    var modifierGleipnir = targetModifiers.ModifierGleipnir;

                    // Atos
                    var atos = Abilities.Atos;
                    if (atos != null
                        && ItemsMenu.ItemsSelection[atos.Id]
                        && !comboBreaker
                        && atos.CanBeCasted
                        && atos.CanHit(target)
                        && modifierAtos.IsModifier(0.5f)
                        && modifierGleipnir.IsModifier(0.5f)
                        && modifierStun.IsModifier(0.5f))
                    {
                        atos.UseAbility(target);
                        await Task.Delay(atos.GetCastDelay(target), token);
                    }

                    // Gleipnir
                    var gleipnir = Abilities.Gleipnir;
                    if (gleipnir != null
                        && ItemsMenu.ItemsSelection[gleipnir.Id]
                        && !comboBreaker
                        && gleipnir.CanBeCasted
                        && gleipnir.CanHit(target)
                        && modifierAtos.IsModifier(0.5f)
                        && modifierGleipnir.IsModifier(0.5f)
                        && modifierStun.IsModifier(0.5f))
                    {
                        gleipnir.UseAbility(target);
                        await Task.Delay(gleipnir.GetCastDelay(target), token);
                    }

                    // Veil
                    var veil = Abilities.Veil;
                    if (veil != null
                        && ItemsMenu.ItemsSelection[veil.Id]
                        && veil.CanBeCasted
                        && veil.CanHit(target))
                    {
                        veil.UseAbility(target.Position);
                        await Task.Delay(veil.GetCastDelay(target.Position), token);
                    }

                    // Ethereal
                    var ethereal = Abilities.Ethereal;
                    if (ethereal != null
                        && ItemsMenu.ItemsSelection[ethereal.Id]
                        && !comboBreaker
                        && ethereal.CanBeCasted
                        && ethereal.CanHit(target))
                    {
                        ethereal.UseAbility(target);
                        MultiSleeper<string>.Sleep($"IsHitTime_{target.Name}_{ethereal.Name}", ethereal.GetHitTime(target));
                        await Task.Delay(ethereal.GetCastDelay(target), token);
                        return;
                    }

                    // Shivas
                    var shivas = Abilities.Shivas;
                    if (shivas != null
                        && ItemsMenu.ItemsSelection[shivas.Id]
                        && shivas.CanBeCasted
                        && shivas.CanHit(target))
                    {
                        shivas.UseAbility();
                        await Task.Delay(shivas.GetCastDelay(), token);
                    }

                    if (!MultiSleeper<string>.Sleeping($"IsHitTime_{target.Name}_item_ethereal_blade") || targetModifiers.IsEthereal)
                    {
                        // LightningBolt
                        var lightningBolt = Abilities.LightningBolt;
                        if (SpellsMenu.SpellsSelection[lightningBolt.Id] && lightningBolt.CanBeCasted)
                        {
                            var methodsItem = LightningBoltMenu.MethodsItem.Value;
                            if (!isLightningBoltOnPosition && lightningBolt.CanHit(target) && (methodsItem == "On Target" || methodsItem == "On Target & Prediction On Position"))
                            {
                                lightningBolt.UseAbility(target);
                                await Task.Delay(lightningBolt.GetCastDelay(target), token);
                            }
                            else if (methodsItem == "Prediction On Position" || methodsItem == "On Target & Prediction On Position")
                            {
                                var position = lightningBolt.LightningBoltPrediction(target, isLightningBoltOnPosition);
                                if (!position.IsZero)
                                {
                                    lightningBolt.UseAbility(position);
                                    await Task.Delay(lightningBolt.GetCastDelay(position), token);
                                }
                            }
                        }

                        // ArcLightning
                        var arcLightning = Abilities.ArcLightning;
                        if (SpellsMenu.SpellsSelection[arcLightning.Id]
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
                            && ItemsMenu.ItemsSelection[AbilityId.item_dagon_5]
                            && !comboBreaker
                            && dagon.CanBeCasted
                            && dagon.CanHit(target))
                        {
                            dagon.UseAbility(target);
                            await Task.Delay(dagon.GetCastDelay(target), token);
                            return;
                        }

                        // Nimbus
                        var nimbus = Abilities.Nimbus;
                        if (SpellsMenu.SpellsSelection[nimbus.Id] && !comboBreaker && nimbus.CanBeCasted)
                        {
                            nimbus.UseAbility(target.Position);
                            await Task.Delay(nimbus.GetCastDelay(target.Position), token);
                            return;
                        }

                        // Thundergods Wrath
                        var thundergodsWrath = Abilities.ThundergodsWrath;
                        if (SpellsMenu.SpellsSelection[thundergodsWrath.Id]
                            && !comboBreaker
                            && (float)target.Health / target.MaximumHealth * 100 < ThundergodsWrathMenu.MinHealthToUltItem.Value
                            && Owner.Distance2D(target) < ThundergodsWrathMenu.MinRangeToUltItem.Value
                            && thundergodsWrath.CanBeCasted)
                        {
                            thundergodsWrath.UseAbility();
                            await Task.Delay(thundergodsWrath.GetCastDelay(), token);
                        }
                    }

                    // Urn
                    var urn = Abilities.Urn;
                    if (urn != null
                        && ItemsMenu.ItemsSelection[urn.Id]
                        && !comboBreaker
                        && urn.CanBeCasted
                        && urn.CanHit(target))
                    {
                        urn.UseAbility(target);
                        await Task.Delay(urn.GetCastDelay(target), token);
                    }

                    // Vessel
                    var vessel = Abilities.Vessel;
                    if (vessel != null
                        && ItemsMenu.ItemsSelection[vessel.Id]
                        && !comboBreaker
                        && vessel.CanBeCasted
                        && vessel.CanHit(target))
                    {
                        vessel.UseAbility(target);
                        await Task.Delay(vessel.GetCastDelay(target), token);
                    }
                }
                else
                {
                    LinkenBreaker.RunAsync();
                }
            }

            OrbwalkTo(target);
        }
        catch (TaskCanceledException)
        {
            // canceled
        }
        catch (Exception e)
        {
            LogManager.Error(e);
        }
    }

    protected override bool WithMute(CUnit target)
    {
        if (!WithMuteMenu.ToggleHotkeyItem)
        {
            return true;
        }

        var hex = Abilities.Hex;
        if (hex != null && ItemsMenu.ItemsSelection[hex.Id] && hex.CanBeCasted && hex.CanHit(target))
        {
            return true;
        }

        return true;
    }
}
