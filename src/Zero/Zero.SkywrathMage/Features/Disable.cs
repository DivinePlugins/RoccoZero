using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Divine.Core.ComboFactory;
using Divine.Core.ComboFactory.Menus;
using Divine.Core.Entities;
using Divine.Core.Extensions;
using Divine.Core.Managers.Unit;
using Divine.Entity.Entities.Abilities.Components;
using Divine.Menu.EventArgs;
using Divine.Menu.Items;
using Divine.Common.Log;

namespace Divine.SkywrathMage.Features
{
    internal sealed class Disable : BaseTaskHandler
    {
        private BaseDisableMenu DisableMenu { get; }

        private Abilities Abilities { get; }

        public Disable(Common common)
        {
            DisableMenu = common.MenuConfig.DisableMenu;

            Abilities = (Abilities)common.Abilities;

            DisableMenu.EnableItem.ValueChanged += EnableChanged;
        }

        public override void Dispose()
        {
            base.Dispose();

            DisableMenu.EnableItem.ValueChanged -= EnableChanged;
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

        protected override async Task ExecuteAsync(CancellationToken token)
        {
            try
            {
                if (IsStopped || Owner.IsInvisible())
                {
                    return;
                }

                var target = UnitManager<CHero, Enemy, NoIllusion>.Units.FirstOrDefault(x => x.IsVisible && x.IsAlive && IsInAbilityPhase(x));  //TODO Target

                if (IsNullTarget(target))
                {
                    return;
                }

                // Hex
                var hex = Abilities.Hex;
                if (hex != null
                    && DisableMenu.AbilitiesSelection[hex.Id]
                    && hex.CanBeCasted
                    && hex.CanHit(target))
                {
                    hex.UseAbility(target);
                    await Task.Delay(hex.GetCastDelay(target), token);
                    return;
                }

                // Orchid
                var orchid = Abilities.Orchid;
                if (orchid != null
                    && DisableMenu.AbilitiesSelection[orchid.Id]
                    && orchid.CanBeCasted
                    && orchid.CanHit(target))
                {
                    orchid.UseAbility(target);
                    await Task.Delay(orchid.GetCastDelay(target), token);
                    return;
                }

                // Bloodthorn
                var bloodthorn = Abilities.Bloodthorn;
                if (bloodthorn != null
                    && DisableMenu.AbilitiesSelection[bloodthorn.Id]
                    && bloodthorn.CanBeCasted
                    && bloodthorn.CanHit(target))
                {
                    bloodthorn.UseAbility(target);
                    await Task.Delay(bloodthorn.GetCastDelay(target), token);
                    return;
                }

                // AncientSeal
                var ancientSeal = Abilities.AncientSeal;
                if (DisableMenu.AbilitiesSelection[ancientSeal.Id]
                    && ancientSeal.CanBeCasted
                    && ancientSeal.CanHit(target))
                {
                    ancientSeal.UseAbility(target);
                    await Task.Delay(ancientSeal.GetCastDelay(target), token);
                    return;
                }
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

        private bool IsInAbilityPhase(CHero target)
        {
            if (target.Spells.Any(x => DisableAbilities.Contains(x.Id) && x.IsInAbilityPhase))
            {
                return true;
            }

            return false;
        }

        private static AbilityId[] DisableAbilities { get; } =
        {
            AbilityId.queenofpain_blink,
            AbilityId.antimage_blink,
            AbilityId.antimage_mana_void,
            AbilityId.legion_commander_duel,
            AbilityId.doom_bringer_doom,
            AbilityId.faceless_void_time_walk,
            AbilityId.faceless_void_chronosphere,
            AbilityId.witch_doctor_death_ward,
            AbilityId.rattletrap_power_cogs,
            AbilityId.tidehunter_ravage,
            AbilityId.axe_berserkers_call,
            AbilityId.brewmaster_primal_split,
            AbilityId.omniknight_guardian_angel,
            AbilityId.queenofpain_sonic_wave,
            AbilityId.slardar_slithereen_crush,
            AbilityId.lion_finger_of_death,
            AbilityId.lina_laguna_blade
        };
    }
}
