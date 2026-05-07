using System;

using Divine.Core.ComboFactory.Menus;
using Divine.Core.ComboFactory.Menus.Combo;
using Divine.Core.Entities;
using Divine.Core.Entities.Abilities.Spells.Bases;
using Divine.Core.Extensions;
using Divine.Extensions;
using Divine.Game;
using Divine.Menu.EventArgs;
using Divine.Menu.Items;
using Divine.Numerics;

namespace Divine.Core.ComboFactory.Combos
{
    public abstract class BaseCombo : BaseTaskHandler
    {
        private readonly BaseComboMenu ComboMenu;

        private readonly BaseWithMuteMenu WithMuteMenu;

        public BaseCombo(BaseMenuConfig menuConfig)
        {
            ComboMenu = menuConfig.ComboMenu;
            WithMuteMenu = ComboMenu.WithMuteMenu;

            ComboMenu.ComboHotkeyItem.ValueChanged += ComboHotkeyChanged;
            WithMuteMenu.ComboHotkey.ValueChanged += WithMuteComboAction;
        }

        public override void Dispose()
        {
            ComboMenu.ComboHotkeyItem.ValueChanged -= ComboHotkeyChanged;
            WithMuteMenu.ComboHotkey.ValueChanged -= WithMuteComboAction;
        }

        private void ComboHotkeyChanged(MenuHoldKey switcher, HoldKeyChangedEventArgs e)
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

        private void WithMuteComboAction(MenuHoldKey switcher, HoldKeyChangedEventArgs e)
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

        protected abstract bool WithMute(CUnit target);

        protected void MoveTo(Vector3 position)
        {
            Orbwalker.MoveTo(position);
        }

        protected void MoveToMousePosition()
        {
            Orbwalker.MoveToMousePosition();
        }

        protected void OrbwalkTo(CUnit target, OrbSpell orbSpell)
        {
            switch (ComboMenu.OrbwalkerItem.Value)
            {
                case "Default":
                    {
                        Orbwalker.OrbwalkTo(target, orbSpell, GameManager.MousePosition, false);
                    }
                    break;

                case "Distance":
                    {
                        var mousePosition = GameManager.MousePosition;
                        var ownerDis = Math.Min(Owner.Distance2D(mousePosition), 230);
                        var ownerPos = Owner.Position.Extend(mousePosition, ownerDis);
                        var position = target.Position.Extend(ownerPos, ComboMenu.MinDisInOrbwalkItem.Value);

                        if (ComboMenu.FullDistanceModeItem && Owner.Distance2D(target) <= ComboMenu.MinDisInOrbwalkItem.Value - 100)
                        {
                            MoveTo(position);
                        }
                        else
                        {
                            Orbwalker.OrbwalkTo(target, orbSpell, position, false);
                        }
                    }
                    break;

                case "Free":
                    {
                        var attackRange = Owner.AttackRange(target);
                        if (Owner.Distance2D(target) <= attackRange && !ComboMenu.FullFreeModeItem || target.Distance2D(GameManager.MousePosition) <= attackRange)
                        {
                            Orbwalker.OrbwalkTo(target, orbSpell, GameManager.MousePosition, false);
                        }
                        else
                        {
                            MoveToMousePosition();
                        }
                    }
                    break;

                case "Only Attack":
                    {
                        Orbwalker.AttackTo(target);
                    }
                    break;

                case "No Move":
                    {
                        if (Owner.Distance2D(target) < Owner.AttackRange(target))
                        {
                            Orbwalker.AttackTo(target);
                        }
                    }
                    break;
            }
        }

        protected void OrbwalkTo(CUnit target)
        {
            OrbwalkTo(target, null);
        }
    }
}
