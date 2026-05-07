using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Divine.Core.ComboFactory;
using Divine.Core.Entities;
using Divine.Core.Extensions;
using Divine.Core.Managers.Unit;
using Divine.Entity.Entities.Units.Heroes;
using Divine.Extensions;
using Divine.Menu.EventArgs;
using Divine.Menu.Items;
using Divine.Modifier;
using Divine.Modifier.EventArgs;
using Divine.SkywrathMage.Menus;

namespace Divine.SkywrathMage.Features
{
    internal sealed class EulControl : BaseTaskHandler
    {
        private readonly BladeMailMenu BladeMailMenu;

        private readonly Abilities Abilities;

        public EulControl(Common common)
        {
            BladeMailMenu = (BladeMailMenu)common.MenuConfig.BladeMailMenu;

            Abilities = (Abilities)common.Abilities;

            BladeMailMenu.EulControlItem.ValueChanged += EulControlChanged;
        }

        public override void Dispose()
        {
            base.Dispose();

            BladeMailMenu.EulControlItem.ValueChanged -= EulControlChanged;

            if (BladeMailMenu.EulControlItem)
            {
                ModifierManager.ModifierAdded -= OnModifierAdded;
                ModifierManager.ModifierRemoved -= OnModifierRemoved;
            }
        }

        private void EulControlChanged(MenuSwitcher switcher, SwitcherChangedEventArgs e)
        {
            if (e.Value)
            {
                ModifierManager.ModifierAdded += OnModifierAdded;
                ModifierManager.ModifierRemoved += OnModifierRemoved;
            }
            else
            {
                Cancel();

                ModifierManager.ModifierAdded -= OnModifierAdded;
                ModifierManager.ModifierRemoved -= OnModifierRemoved;
            }
        }

        private void OnModifierAdded(ModifierAddedEventArgs e)
        {
            var modifier = e.Modifier;
            if (modifier.Owner is Hero hero && hero.IsEnemy(Owner.Base) && Abilities.MysticFlare != null && modifier.Name == Abilities.MysticFlare.TargetModifierName)
            {
                RunAsync();
            }
        }

        private void OnModifierRemoved(ModifierRemovedEventArgs e)
        {
            if (Abilities.MysticFlare != null && e.Modifier.Name == Abilities.MysticFlare.TargetModifierName)
            {
                Cancel();
            }
        }

        protected override async Task ExecuteAsync(CancellationToken token)
        {
            if (IsStopped)
            {
                return;
            }

            var eul = Abilities.Eul;
            if (eul != null && eul.CanBeCasted)
            {
                if (UnitManager<CHero, Enemy, NoIllusion>.Units.Any(x => x.IsVisible && x.IsAlive && x.HasModifier("modifier_item_blade_mail_reflect")))
                {
                    eul.UseAbility(Owner);
                    await Task.Delay(eul.GetCastDelay(), token);
                }
            }
        }
    }
}
