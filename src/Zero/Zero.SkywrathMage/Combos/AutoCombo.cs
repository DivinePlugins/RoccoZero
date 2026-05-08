namespace Divine.SkywrathMage.Combos;

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Divine.Core.ComboFactory;
using Divine.Core.ComboFactory.Combos;
using Divine.Core.ComboFactory.Menus;
using Divine.Core.ComboFactory.Menus.Combo;
using Divine.Core.Entities;
using Divine.Core.Extensions;
using Divine.Core.Helpers;
using Divine.Core.Managers.Unit;
using Divine.Entity.Entities.Abilities.Components;
using Divine.Menu.EventArgs;
using Divine.Menu.Items;
using Divine.SkywrathMage.Menus;

internal sealed class AutoCombo : BaseTaskHandler
{
    private readonly BaseComboMenu ComboMenu;

    private readonly BaseAeonDiskMenu AeonDiskMenu;

    private readonly AutoComboMenu AutoComboMenu;

    private readonly SmartConcussiveShotMenu SmartConcussiveShotMenu;

    private readonly BaseBladeMailMenu BladeMailMenu;

    private readonly Abilities Abilities;

    private readonly BaseLinkenBreaker LinkenBreaker;

    private readonly BaseKillSteal KillSteal;

    public AutoCombo(Common common)
    {
        ComboMenu = common.MenuConfig.ComboMenu;
        AeonDiskMenu = common.MenuConfig.ComboMenu.AeonDiskMenu;
        AutoComboMenu = ((MoreMenu)common.MenuConfig.MoreMenu).AutoComboMenu;
        SmartConcussiveShotMenu = ((MoreMenu)common.MenuConfig.MoreMenu).SmartConcussiveShotMenu;
        BladeMailMenu = common.MenuConfig.BladeMailMenu;

        Abilities = (Abilities)common.Abilities;
        LinkenBreaker = common.LinkenBreaker;
        KillSteal = common.KillSteal;

        AutoComboMenu.EnableItem.ValueChanged += EnableChanged;
    }

    public override void Dispose()
    {
        base.Dispose();

        AutoComboMenu.EnableItem.ValueChanged -= EnableChanged;
    }

    private void EnableChanged(MenuSwitcher switcher, SwitcherChangedEventArgs e)
    {
        if (e.Value)
        {
            RunAsync();
        }
        else
        {
            Cancel();
        }
    }

    protected override CUnit CurrentTarget
    {
        get
        {
            return UnitManager<CHero, Enemy, NoIllusion>.Units
                .Where(x => x.IsVisible && x.IsAlive && x.Distance2D(Owner) <= 1850 && Utils.AutoCombo(x))
                .OrderBy(x => x.Distance2D(Owner))
                .FirstOrDefault();
        }
    }

    protected override async Task ExecuteAsync(CancellationToken token)
    {
        try
        {
            if (AutoComboMenu.DisableWhenComboItem && ComboMenu.ComboHotkeyItem)
            {
                return;
            }

            if (KillSteal.IsKillSteal || IsStopped || Owner.IsInvisible())
            {
                return;
            }

            if (AutoComboMenu.OwnerMinHealthItem.Value > ((float)Owner.Health / Owner.MaximumHealth) * 100)
            {
                return;
            }

            var target = CurrentTarget;
            if (IsNullTarget(target))
            {
                return;
            }

            if (AeonDiskMenu.EnableItem && target.ComboBreaker())
            {
                return;
            }

            var targetModifiers = new TargetModifiers(target);
            if (BladeMailMenu.BladeMailItem && targetModifiers.ModifierReflect != null)
            {
                return;
            }

            if (target.IsBlockMagicDamage(targetModifiers))
            {
                return;
            }

            if (target.IsShieldAbilities(targetModifiers) && !targetModifiers.IsSilverDebuff)
            {
                LinkenBreaker.RunAsync();
                return;
            }

            // Hex
            var modifierStun = targetModifiers.ModifierStun;
            var modifierHex = targetModifiers.ModifierHex;
            var hex = Abilities.Hex;
            if (hex != null
                && AutoComboMenu.ItemsSelection[hex.Id]
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
                && AutoComboMenu.ItemsSelection[orchid.Id]
                && orchid.CanBeCasted
                && orchid.CanHit(target))
            {
                orchid.UseAbility(target);
                await Task.Delay(orchid.GetCastDelay(target), token);
            }

            // Bloodthorn
            var bloodthorn = Abilities.Bloodthorn;
            if (bloodthorn != null
                && AutoComboMenu.ItemsSelection[bloodthorn.Id]
                && bloodthorn.CanBeCasted
                && bloodthorn.CanHit(target))
            {
                bloodthorn.UseAbility(target);
                await Task.Delay(bloodthorn.GetCastDelay(target), token);
            }

            // MysticFlare
            var mysticFlare = Abilities.MysticFlare;
            if (AutoComboMenu.SpellsSelection[mysticFlare.Id]
                && AutoComboMenu.MinHealthToUltItem.Value <= ((float)target.Health / target.MaximumHealth) * 100
                && mysticFlare.CanBeCasted
                && mysticFlare.CanHit(target))
            {
                var position = mysticFlare.MysticFlarePrediction(target);
                mysticFlare.UseAbility(position);
                await Task.Delay(mysticFlare.GetCastDelay(position), token);
            }

            // Nullifier
            var nullifier = Abilities.Nullifier;
            if (nullifier != null
                && AutoComboMenu.ItemsSelection[nullifier.Id]
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
                && AutoComboMenu.ItemsSelection[atos.Id]
                && atos.CanBeCasted
                && atos.CanHit(target)
                && modifierAtos.IsModifier(0.5f)
                && modifierStun.IsModifier(0.5f))
            {
                atos.UseAbility(target);
                await Task.Delay(atos.GetCastDelay(target), token);
            }

            // Gleipnir
            var gleipnir = Abilities.Gleipnir;
            if (gleipnir != null
                && AutoComboMenu.ItemsSelection[gleipnir.Id]
                && gleipnir.CanBeCasted
                && gleipnir.CanHit(target)
                && modifierAtos.IsModifier(0.5f)
                && modifierGleipnir.IsModifier(0.5f)
                && modifierStun.IsModifier(0.5f))
            {
                gleipnir.UseAbility(target);
                await Task.Delay(gleipnir.GetCastDelay(target), token);
            }

            // AncientSeal
            var ancientSeal = Abilities.AncientSeal;
            if (AutoComboMenu.SpellsSelection[ancientSeal.Id]
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
                && AutoComboMenu.ItemsSelection[veil.Id]
                && veil.CanBeCasted
                && veil.CanHit(target))
            {
                veil.UseAbility(target.Position);
                await Task.Delay(veil.GetCastDelay(target.Position), token);
            }

            // Ethereal
            var ethereal = Abilities.Ethereal;
            if (ethereal != null
                && AutoComboMenu.ItemsSelection[ethereal.Id]
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
                && AutoComboMenu.ItemsSelection[shivas.Id]
                && shivas.CanBeCasted
                && shivas.CanHit(target))
            {
                shivas.UseAbility();
                await Task.Delay(shivas.GetCastDelay(), token);
            }

            if (!ZMultiSleeper<string>.Sleeping($"IsHitTime_{target.Name}_item_ethereal_blade") || targetModifiers.IsEthereal)
            {
                // ConcussiveShot
                var concussiveShot = Abilities.ConcussiveShot;
                if (AutoComboMenu.SpellsSelection[concussiveShot.Id]
                    && concussiveShot.CanBeCasted
                    && concussiveShot.CanHit(target)
                    && Utils.ConcussiveShotTarget(SmartConcussiveShotMenu, target, concussiveShot.TargetHit))
                {
                    concussiveShot.UseAbility();
                    await Task.Delay(concussiveShot.GetCastDelay(), token);
                }

                // ArcaneBolt
                var arcaneBolt = Abilities.ArcaneBolt;
                var arcaneBoltId = arcaneBolt.Id;
                if (AutoComboMenu.SpellsSelection[arcaneBoltId]
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
                    && AutoComboMenu.ItemsSelection[AbilityId.item_dagon_5]
                    && dagon.CanBeCasted
                    && dagon.CanHit(target))
                {
                    dagon.UseAbility(target);
                    await Task.Delay(dagon.GetCastDelay(target), token);
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
