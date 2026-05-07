using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Divine.Core.ComboFactory;
using Divine.Core.ComboFactory.Menus;
using Divine.Core.Entities;
using Divine.Core.Extensions;
using Divine.Core.Managers.Unit;
using Divine.Entity.Entities.Components;
using Divine.Menu.EventArgs;
using Divine.Menu.Items;

using Ensage.SDK.TargetSelector.Modes;

namespace Divine.SkywrathMage.Features
{
    internal sealed class Farm : BaseTaskHandler
    {
        private readonly BaseFarmMenu FarmMenu;

        private readonly FarmSelector Selector;

        private readonly Abilities Abilities;

        public Farm(Common common)
        {
            FarmMenu = common.MenuConfig.FarmMenu;

            Abilities = (Abilities)common.Abilities;

            Selector = new FarmSelector(Owner.Base, common.TargetSelector);

            FarmMenu.FarmHotkeyItem.ValueChanged += FarmHotkeyChanged;
            FarmMenu.HeroHarrasItem.ValueChanged += HeroHarrasChanged;
        }

        public override void Dispose()
        {
            base.Dispose();

            FarmMenu.FarmHotkeyItem.ValueChanged -= FarmHotkeyChanged;
            FarmMenu.HeroHarrasItem.ValueChanged -= HeroHarrasChanged;

            Selector.Dispose();
        }

        private void FarmHotkeyChanged(MenuHoldKey holdKey, HoldKeyChangedEventArgs e)
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

        private void HeroHarrasChanged(MenuSelector selector, SelectorChangedEventArgs e)
        {
            Selector.Hero = e.Value == "Disable";
        }

        protected async override Task ExecuteAsync(CancellationToken token)
        {
            if (IsStopped)
            {
                return;
            }

            var arcaneBolt = Abilities.ArcaneBolt;
            if (FarmMenu.FarmItem.Value == "Arcane Bolt & Attack")
            {
                var unit = UnitManager<CUnit, Enemy, NoIllusion>.Units.FirstOrDefault(x =>
                                                                       (x.ClassId == ClassId.CDOTA_BaseNPC_Creep_Neutral ||
                                                                       x.ClassId == ClassId.CDOTA_BaseNPC_Invoker_Forged_Spirit ||
                                                                       x.ClassId == ClassId.CDOTA_BaseNPC_Warlock_Golem ||
                                                                       x.ClassId == ClassId.CDOTA_BaseNPC_Creep ||
                                                                       x.ClassId == ClassId.CDOTA_BaseNPC_Creep_Lane ||
                                                                       x.ClassId == ClassId.CDOTA_BaseNPC_Creep_Siege ||
                                                                       x.ClassId == ClassId.CDOTA_Unit_Hero_Beastmaster_Boar ||
                                                                       x.ClassId == ClassId.CDOTA_Unit_SpiritBear ||
                                                                       x.ClassId == ClassId.CDOTA_Unit_Broodmother_Spiderling) &&
                                                                       x.IsVisible &&
                                                                       x.IsAlive &&
                                                                       x.IsSpawned &&
                                                                       x.Distance2D(Owner) < arcaneBolt.CastRange &&
                                                                       x.Health < arcaneBolt.GetDamage(x));

                // ArcaneBolt
                if (unit != null && arcaneBolt.CanBeCasted && arcaneBolt.CanHit(unit))
                {
                    arcaneBolt.UseAbility(unit);
                    await Task.Delay(arcaneBolt.GetCastDelay(unit), token);
                    return;
                }
            }

            var selectorTarget = Selector.GetTarget();
            if (selectorTarget != null)
            {
                var target = UnitManager.GetUnitByHandle(selectorTarget.Handle);
                if (target != null && FarmMenu.HeroHarrasItem.Value == "Arcane Bolt & Attack" && target is CHero)
                {
                    if (arcaneBolt.CanBeCasted && arcaneBolt.CanHit(target))
                    {
                        arcaneBolt.UseAbility(target);
                        await Task.Delay(arcaneBolt.GetCastDelay(target), token);
                        return;
                    }
                }

                Orbwalker.OrbwalkTo(target);
                return;
            }

            Orbwalker.MoveToMousePosition();
        }
    }
}