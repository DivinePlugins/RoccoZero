using System;

using Divine.Core.ComboFactory.Commons;
using Divine.Core.ComboFactory.Menus;
using Divine.Core.ComboFactory.Menus.Combo;
using Divine.Core.ComboFactory.Menus.Settings;
using Divine.Core.ComboFactory.Menus.Settings.Drawing;
using Divine.Core.Entities;
using Divine.Core.Helpers;
using Divine.Core.Managers.TargetSelector;
using Divine.Core.Managers.Unit;
using Divine.Menu.EventArgs;
using Divine.Menu.Items;
using Divine.Numerics;
using Divine.Renderer;

namespace Divine.Core.ComboFactory.Helpers
{
    public class BaseRenderer
    {
        protected readonly BaseMenuConfig MenuConfig;

        protected readonly BaseComboMenu ComboMenu;

        private readonly BaseWithMuteMenu WithMuteMenu;

        private readonly BaseSettingsMenu SettingsMenu;

        private readonly BaseDamageCalculationMenu DamageCalculationMenu;

        protected readonly BaseTextPanelMenu TextPanelMenu;

        private readonly TargetSelectorManager TargetSelector;

        private readonly PanelMove PanelMove;

        private Vector2 Size = new Vector2(150, 0);

        protected readonly CHero Owner = UnitManager.Owner;

        public BaseRenderer(BaseCommon common)
        {
            MenuConfig = common.MenuConfig;
            ComboMenu = MenuConfig.ComboMenu;
            WithMuteMenu = MenuConfig.ComboMenu.WithMuteMenu;
            SettingsMenu = MenuConfig.SettingsMenu;
            DamageCalculationMenu = SettingsMenu.DrawingMenu.DamageCalculationMenu;
            TextPanelMenu = common.MenuConfig.SettingsMenu.DrawingMenu.TextPanelMenu;

            TargetSelector = common.TargetSelector;

            var panel = new Vector2(HUDInfo.ScreenSize.X - 200, HUDInfo.ScreenSize.Y - 310);
            PanelMove = new PanelMove(panel + new Vector2(TextPanelMenu.X, TextPanelMenu.Y));

            SettingsMenu.EnableDrawingItem.ValueChanged += DisableChanged;
        }

        public virtual void Dispose()
        {
            if (SettingsMenu.EnableDrawingItem)
            {
                TextPanelMenu.ComboPanelItem.ValueChanged -= ComboPanelChanged;

                PanelMove.ValueChanged -= PanelMoveChanged;
                TextPanelMenu.MoveItem.ValueChanged -= MoveChanged;

                RendererManager.Draw -= OnDraw;
            }

            TextPanelMenu.MoveItem.ValueChanged -= MoveChanged;
        }

        private void DisableChanged(MenuSwitcher switcher, SwitcherChangedEventArgs e)
        {
            if (e.Value)
            {
                if (TextPanelMenu.ComboPanelItem)
                {
                    Size.Y += 60;
                }

                PanelMove.Size = Size;

                RendererManager.Draw += OnDraw;

                TextPanelMenu.MoveItem.ValueChanged += MoveChanged;
                PanelMove.ValueChanged += PanelMoveChanged;

                TextPanelMenu.ComboPanelItem.ValueChanged += ComboPanelChanged;
            }
            else
            {
                if (TextPanelMenu.ComboPanelItem)
                {
                    Size.Y -= 60;
                }

                PanelMove.Size = Size;

                TextPanelMenu.ComboPanelItem.ValueChanged -= ComboPanelChanged;

                PanelMove.ValueChanged -= PanelMoveChanged;
                TextPanelMenu.MoveItem.ValueChanged -= MoveChanged;

                RendererManager.Draw -= OnDraw;
            }
        }

        private void MoveChanged(MenuSwitcher switcher, SwitcherChangedEventArgs e)
        {
            if (e.Value)
            {
                PanelMove.ActivateMove = true;
            }
            else
            {
                PanelMove.ActivateMove = false;
            }
        }

        private void PanelMoveChanged(bool isTimeout, Vector2 position)
        {
            if (isTimeout)
            {
                TextPanelMenu.MoveItem.Value = false;
                return;
            }

            var panel = position - new Vector2(HUDInfo.ScreenSize.X - 200, HUDInfo.ScreenSize.Y - 310);

            TextPanelMenu.X.Value = (int)panel.X;
            TextPanelMenu.Y.Value = (int)panel.Y;
        }

        private void ComboPanelChanged(MenuSwitcher switcher, SwitcherChangedEventArgs e)
        {
            if (e.Value)
            {
                Size.Y += 60;
            }
            else
            {
                Size.Y -= 60;
            }

            PanelMove.Size = Size;
        }

        protected void Text(string text, Vector2 pos, Color color)
        {
            RendererManager.DrawText(text, pos, color, "Arial", 21f);
        }

        protected int comboPanelMore;

        protected int ComboPanelMore
        {
            get
            {
                return comboPanelMore;
            }

            set
            {
                Size.Y -= comboPanelMore;
                comboPanelMore = value;
                Size.Y += comboPanelMore;
                PanelMove.Size = Size;
            }
        }

        protected virtual void OnDraw()
        {
            var comboPanelItem = TextPanelMenu.ComboPanelItem;

            if (comboPanelItem)
            {
                var position = new Vector2(HUDInfo.ScreenSize.X - 200, HUDInfo.ScreenSize.Y - 310) + new Vector2(TextPanelMenu.X, TextPanelMenu.Y);

                if (PanelMove.ActivateMove)
                {
                    var movePosition = position - new Vector2(5);

                    var text = PanelMove.Time.ToString();
                    var textMinSize = Math.Min(Size.X, Size.Y);
                    var sizeText = RendererManager.MeasureText(text, "Arial", textMinSize);

                    RendererManager.DrawText(text, movePosition + ((Size / 2) - (sizeText / 2)), new Color(255, 255, 255, 50), "Arial", textMinSize);
                    RendererManager.DrawRectangle(new(movePosition.X, movePosition.Y, Size.X, Size.Y), Color.WhiteSmoke);
                }

                var isCombo = ComboMenu.ComboHotkeyItem;
                if (comboPanelItem)
                {
                    if (isCombo)
                    {
                        Text($"Combo ON", position, Color.Aqua);
                    }
                    else
                    {
                        Text($"Combo OFF", position, Color.Yellow);
                    }

                    if (WithMuteMenu.ToggleHotkeyItem)
                    {
                        Text($"With Mute ON", position + new Vector2(0, 30), Color.Aqua);
                    }
                    else
                    {
                        Text($"With Mute OFF", position + new Vector2(0, 30), Color.Yellow);
                    }

                    position = position + new Vector2(0, 60 + ComboPanelMore);
                }
            }

            if (SettingsMenu.EnableDamageCalculationItem)
            {
                foreach (var data in BaseDamageCalculation.DamageDate)
                {
                    var hero = data.GetHero;
                    var health = data.GetHealth;
                    var canBeCastedDamage = data.GetCanBeCastedDamage;

                    var hpBarPosition = HUDInfo.GetHpBarPosition(hero);
                    var hpBarSizeX = HUDInfo.HpBarSizeX;
                    var hpBarSizeY = HUDInfo.HpBarSizeY / 2.3f;
                    var hpBarPos = hpBarPosition + new Vector2(0, hpBarSizeY + 1.8f);

                    if (hpBarPosition.IsZero)
                    {
                        continue;
                    }

                    if (DamageCalculationMenu.HpBarItem)
                    {
                        var canHitDamage = data.GetCanHitdamage;
                        var maximumHealth = hero.MaximumHealth;

                        var canBeCastedDamageBar = Math.Max(canBeCastedDamage, 0) / maximumHealth;
                        if (canBeCastedDamageBar > 0)
                        {
                            var canBeCastedDamagePos = Math.Max(health - canBeCastedDamage, 0) / maximumHealth;
                            var canBeCastedDamagePosition = new Vector2(hpBarPos.X + ((hpBarSizeX + canBeCastedDamageBar) * canBeCastedDamagePos), hpBarPos.Y);
                            var canBeCastedDamageSize = new Vector2(hpBarSizeX * (canBeCastedDamageBar + Math.Min(health - canBeCastedDamage, 0) / maximumHealth), hpBarSizeY);
                            var canBeCastedDamageeColor = ((float)health / maximumHealth) - canBeCastedDamageBar > 0 ? new Color(100, 0, 0, 200) : new Color(191, 255, 0, 200);

                            RendererManager.DrawFilledRectangle(new(canBeCastedDamagePosition.X, canBeCastedDamagePosition.Y, canBeCastedDamageSize.X, canBeCastedDamageSize.Y), canBeCastedDamageeColor);
                            RendererManager.DrawRectangle(new(canBeCastedDamagePosition.X, canBeCastedDamagePosition.Y, canBeCastedDamageSize.X, canBeCastedDamageSize.Y), Color.Black);
                        }

                        var canHitDamageBar = Math.Max(canHitDamage, 0) / maximumHealth;
                        if (canHitDamageBar > 0)
                        {
                            var canHitDamagePos = Math.Max(health - canHitDamage, 0) / maximumHealth;
                            var canHitDamagePosition = new Vector2(hpBarPos.X + ((hpBarSizeX + canHitDamageBar) * canHitDamagePos), hpBarPos.Y);
                            var canHitDamageSize = new Vector2(hpBarSizeX * (canHitDamageBar + Math.Min(health - canHitDamage, 0) / maximumHealth), hpBarSizeY);
                            var canHitDamageColor = ((float)health / maximumHealth) - canHitDamageBar > 0 ? new Color(0, 255, 0) : Color.Aqua;

                            RendererManager.DrawFilledRectangle(new(canHitDamagePosition.X, canHitDamagePosition.Y, canHitDamageSize.X, canHitDamageSize.Y),canHitDamageColor);
                            RendererManager.DrawRectangle(new(canHitDamagePosition.X, canHitDamagePosition.Y, canHitDamageSize.X, canHitDamageSize.Y), Color.Black);
                        }
                    }

                    if (DamageCalculationMenu.ValueItem)
                    {
                        var damage = health - (int)canBeCastedDamage;
                        var color = damage > 0 ? Color.WhiteSmoke : Color.Aqua;
                        RendererManager.DrawText($"{damage} ({(int)canBeCastedDamage})", hpBarPosition + new Vector2(hpBarSizeX + (25 * HUDInfo.RatioPercentage), 0), color, "Calibri Bold", 17 * HUDInfo.RatioPercentage);
                    }
                }
            }
        }
    }
}