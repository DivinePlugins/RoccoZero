namespace BeAware.Overlay;

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

using BeAware.Helpers;
using BeAware.Utils;

using Divine.Core.Utils;
using Divine.Entity;
using Divine.Entity.Entities.Abilities.Components;
using Divine.Entity.Entities.Units.Heroes;
using Divine.Extensions;
using Divine.Game;
using Divine.Helpers;
using Divine.Menu.EventArgs;
using Divine.Menu.Items;
using Divine.Numerics;
using Divine.Renderer;

internal sealed class EnemyOverlay : Overlay
{
    private readonly PanelMove PanelMove;

    public EnemyOverlay(Common common) : base(common)
    {
        PanelMove = new PanelMove(new Vector2(EmemyItemPanelMenu.PositionXItem, EmemyItemPanelMenu.PositionYItem));
        PanelMove.ValueChanged += PanelMoveChanged;

        EmemyItemPanelMenu.MoveItem.ValueChanged += OnMoveValueChanged;

        RendererManager.LoadImageFromAssembly("BeAware.Resources.Textures.ui_manabar.png");
        RendererManager.LoadImageFromAssembly("BeAware.Resources.Textures.item_panel.png");
    }

    private void OnMoveValueChanged(MenuSwitcher switcher, SwitcherChangedEventArgs e)
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

    protected override IEnumerable<Hero> Heroes
    {
        get
        {
            return EntityManager.GetEntities<Hero>().Where(x => !x.IsAlly(LocalHero) && !x.IsIllusion);
        }
    }

    protected override bool ManaBarValue
    {
        get
        {
            return ManaBarMenu.ManaBarValueItem;
        }
    }

    protected override bool HeroIsVisible(Hero hero)
    {
        if (!VisibleStatusMenu.VisibleStatusEnemyItem)
        {
            return false;
        }

        return hero.IsVisible;
    }

    protected override bool TopBarUltimateOverlay
    {
        get
        {
            return TopPanelMenu.UltimateBarMenu.UltimateBarEnemyItem;
        }
    }

    protected override bool SpellsOverlay
    {
        get
        {
            return SpellsMenu.EnemyOverlayItem;
        }
    }

    protected override bool ItemsOverlay
    {
        get
        {
            return ItemsMenu.EnemyOverlayItem;
        }
    }

    protected override bool TownPortalScrollOverlay
    {
        get
        {
            return TownPortalScrollMenu.EnemyItem;
        }
    }

    protected override void EnemyNotVisibleTimer(Hero hero, TopPanel topPanel)
    {
        if (!VisibleStatusMenu.EnemyNotVisibleTimeItem)
        {
            return;
        }

        var notVisibleTime = (int)Math.Min(GameManager.Time - hero.LastVisibleTime, 99);
        if (notVisibleTime <= 0)
        {
            return;
        }

        var text = notVisibleTime.ToString();
        var size = VisibleStatusMenu.SizeItem.Value * HUDInfo.RatioPercentage;
        var extraPos = new Vector2((31 * HUDInfo.RatioPercentage) - RendererManager.MeasureText(text, size).X / 2, (10 * HUDInfo.RatioPercentage) - (size * 0.4f));
        var color = new Color((int)VisibleStatusMenu.RedItem.Value, (int)VisibleStatusMenu.GreenItem.Value, (int)VisibleStatusMenu.BlueItem.Value);
        RendererManager.DrawText(text, topPanel.IconPosition + extraPos, color, size);
    }

    protected override void HpBar(int health, int maximumHealth, float healthPerc, Vector2 hpBarPosition)
    {
        if (!HpBarMenu.HpBarValueItem)
        {
            return;
        }

        var hpText = $"{health}/{maximumHealth}";
        var sizeText = 12.5f * HUDInfo.RatioPercentage;
        var measureText = RendererManager.MeasureText(hpText, sizeText);

        RendererManager.DrawText(hpText, HpBarUtils.HpBarValuePosition(hpBarPosition, measureText.X), Color.WhiteSmoke, sizeText);
    }

    protected override void ManaBar(float mana, float maximumMana, float manaPerc, Vector2 hpBarPosition, float manaBarSizeY)
    {
        if (!ManaBarMenu.ManaBarItem)
        {
            return;
        }

        var manaBarPosition = ManaBarUtils.ManaBarPosition(hpBarPosition);
        var pos = ManaBarUtils.ManaBarBackPosition(manaBarPosition);
        var size = new Vector2(HUDInfo.HpBarSizeX - 1, manaBarSizeY + 3);
        var size2 = new Vector2(ManaBarUtils.ManaBarSizeX(manaPerc), manaBarSizeY);

        RendererManager.DrawFilledRectangle(new Rect(pos.X, pos.Y, size.X, size.Y), Color.Zero, Color.Black, 0);
        RendererManager.DrawImage("BeAware.Resources.Textures.ui_manabar.png", new Rect(manaBarPosition.X, manaBarPosition.Y, size2.X, size2.Y));

        if (ManaBarMenu.ManaBarValueItem)
        {
            var manaText = $"{ (int)mana }/{ (int)maximumMana }";
            var sizeText = 12.5f * HUDInfo.RatioPercentage;
            var measureText = RendererManager.MeasureText(manaText, sizeText);

            RendererManager.DrawText(manaText, ManaBarUtils.ManaBarValuePosition(manaBarPosition, measureText.X), Color.WhiteSmoke, sizeText);
        }
    }

    private void PanelMoveChanged(bool isTimeout, Vector2 position)
    {
        if (isTimeout)
        {
            EmemyItemPanelMenu.MoveItem.Value = false;
            return;
        }

        EmemyItemPanelMenu.PositionXItem.Value = (int)position.X;
        EmemyItemPanelMenu.PositionYItem.Value = (int)position.Y;
    }

    protected override void EmemyItemPanel()
    {
        if (EmemyItemPanelMenu.EmemyItemPanelItem)
        {
            var size = EmemyItemPanelMenu.SizeItem.Value;
            var sizeX = 20 + size * 0.4f;
            var sizeY = sizeX / 1.30f;

            var panelPosition = new Vector2(EmemyItemPanelMenu.PositionXItem, EmemyItemPanelMenu.PositionYItem);
            var panelSize = new Vector2(sizeX * 7.31f, 5 * (sizeY + 2) + 2);
            PanelMove.Size = new Vector2(panelSize.X + 9, panelSize.Y + 9);

            RendererManager.DrawImage("BeAware.Resources.Textures.item_panel.png", new Rect(panelPosition.X - 2, panelPosition.Y, panelSize.X, panelSize.Y));

            var itemPosition = panelPosition;

            foreach (var hero in Heroes)
            {
                itemPosition += new Vector2(0, 2);

                RendererManager.DrawImage(hero.Name, new Rect(itemPosition.X, itemPosition.Y, sizeX * 1.36f, sizeY), ImageType.Unit, true);

                var mana = hero.Mana;
                var maximumMana = hero.MaximumMana;

                DrawTownPortalScroll(hero, itemPosition + new Vector2(sizeX * 0.85f, sizeY * 0.35f), new Vector2(sizeY * 0.65f), mana);

                itemPosition += new Vector2(sizeX * 1.4f, 0);

                foreach (var item in hero.Inventory.MainItems)
                {
                    var itemSize = new Vector2(sizeX, sizeY);
                    RendererManager.DrawImage(item.Name, new Rect(itemPosition.X, itemPosition.Y, itemSize.X, itemSize.Y), ImageType.Item, true);

                    var manaCost = item.ManaCost;
                    var enoughMana = mana >= manaCost;
                    var cooldown = Math.Ceiling(item.CooldownInFog);
                    if (cooldown > 0 || !enoughMana)
                    {
                        RendererManager.DrawFilledRectangle(new Rect(itemPosition.X, itemPosition.Y, itemSize.X, itemSize.Y), Color.Zero, enoughMana ? new Color(40, 40, 40, 180) : new Color(25, 25, 130, 190), 0);
                    }
                    else
                    {
                        RendererManager.DrawFilledRectangle(new Rect(itemPosition.X, itemPosition.Y, itemSize.X, itemSize.Y), Color.Zero, new Color(0, 0, 0, 100), 0);
                    }


                    if (cooldown > 0)
                    {
                        var cooldownText = Math.Min(cooldown, 99).ToString(CultureInfo.InvariantCulture);
                        var cooldownSize = itemSize.Y - 1;
                        var textSize = RendererManager.MeasureText(cooldownText, cooldownSize);
                        var pos = itemPosition + new Vector2(itemSize.X / 2 - textSize.X / 2, (itemSize.Y / 2) - (textSize.Y / 2));

                        RendererManager.DrawText(cooldownText, pos, Color.WhiteSmoke, cooldownSize);
                    }

                    if (!enoughMana && cooldown <= 0)
                    {
                        var manaCostText = Math.Min(Math.Ceiling(manaCost - mana), 999).ToString(CultureInfo.InvariantCulture);
                        var textSize = RendererManager.MeasureText(manaCostText, itemSize.Y - 1);
                        var pos = itemPosition + new Vector2(itemSize.X / 2 - textSize.X / 2, (itemSize.Y / 2) - (textSize.Y / 2));

                        RendererManager.DrawText(manaCostText, pos, Color.LightBlue, itemSize.Y - 1);
                    }

                    var itemId = item.Id;
                    if ((item.IsRequiringCharges || itemId == AbilityId.item_bottle || itemId == AbilityId.item_ward_dispenser || itemId == AbilityId.item_ward_observer || itemId == AbilityId.item_ward_sentry) && cooldown <= 0)
                    {
                        var currentCharges = item.CurrentCharges.ToString();
                        var tSize = itemSize.Y / 2;
                        var textSize = RendererManager.MeasureText(currentCharges, tSize);
                        var tPos = itemPosition + new Vector2(itemSize.X - textSize.X - 1, itemSize.Y - textSize.Y);

                        RendererManager.DrawFilledRectangle(new Rect(tPos.X - 1, tPos.Y, textSize.X + 1, textSize.Y), Color.Zero, new Color(0, 0, 0, 220), 0);
                        RendererManager.DrawText(currentCharges, tPos, new Color(168, 168, 168), tSize);

                        var secondcharges = item.SecondaryCharges;
                        if (secondcharges > 0)
                        {
                            tPos = itemPosition + new Vector2(2, itemSize.Y - textSize.Y);
                            currentCharges = secondcharges.ToString();
                            tSize = itemSize.Y - 1;
                            var textSize1 = RendererManager.MeasureText(currentCharges, tSize);

                            RendererManager.DrawFilledRectangle(new Rect(tPos.X - 1, tPos.Y, textSize.X + 1, textSize.Y + 1), Color.Zero, new Color(0, 0, 0, 220), 0);
                            RendererManager.DrawText(currentCharges, tPos, new Color(168, 168, 168), tSize);
                        }
                    }

                    itemPosition += new Vector2(sizeX - 1, 0);
                }

                itemPosition = new Vector2(panelPosition.X, sizeY + itemPosition.Y);
            }

            if (PanelMove.ActivateMove)
            {
                var text = PanelMove.Time.ToString();
                var sizeText = RendererManager.MeasureText(text, panelSize.Y);

                RendererManager.DrawText(text, panelPosition - new Vector2((sizeText.X * 0.5f) - (panelSize.X / 2), 0), new Color(255, 255, 255, 50), panelSize.X / 2);
                RendererManager.DrawRectangle(new Rect(panelPosition.X - 5, panelPosition.Y - 5, PanelMove.Size.X, PanelMove.Size.Y), Color.WhiteSmoke, 1);
            }
        }
    }

    private void DrawTownPortalScroll(Hero hero, Vector2 position, Vector2 size, float mana)
    {
        var tp = hero.Inventory.TownPortalScroll;
        if (tp != null)
        {
            RendererManager.DrawImage(tp.Name, new Rect(position.X, position.Y, size.X, size.Y), ImageType.RoundAbility, true);

            var manaCost = tp.ManaCost;
            var enoughMana = mana >= manaCost;

            var cooldown = Math.Ceiling(tp.Cooldown);
            if (cooldown > 0 || !enoughMana)
            {
                RendererManager.DrawFilledRectangle(new Rect(position.X, position.Y, size.X, size.Y), Color.Zero, enoughMana ? new Color(40, 40, 40, 180) : new Color(25, 25, 130, 190), 0);
            }
            else
            {
                RendererManager.DrawFilledRectangle(new Rect(position.X, position.Y, size.X, size.Y), Color.Zero, new Color(0, 0, 0, 100), 0);
            }

            if (cooldown > 0)
            {
                var cooldownText = Math.Min(cooldown, 99).ToString(CultureInfo.InvariantCulture);
                var cooldownSize = cooldown > 9 ? size.Y * 0.65f : size.Y * 0.7f;
                var textSize = RendererManager.MeasureText(cooldownText, cooldownSize);
                var pos = position + new Vector2(size.X / 2 - textSize.X / 2, size.Y / 2 - textSize.Y / 2);

                RendererManager.DrawText(cooldownText, pos, Color.WhiteSmoke, cooldownSize);
            }

            if (!enoughMana && cooldown <= 0)
            {
                var manaCostText = Math.Min(Math.Ceiling(manaCost - mana), 99).ToString(CultureInfo.InvariantCulture);
                var manaCostSize = size.Y * 0.65f;
                var textSize = RendererManager.MeasureText(manaCostText, manaCostSize);
                var pos = position + new Vector2(size.X / 2 - textSize.X / 2, size.Y / 2 - textSize.Y / 2);

                RendererManager.DrawText(manaCostText, pos, Color.LightBlue, manaCostSize);
            }
        }
    }
}