namespace BeAware.Overlay;

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

using BeAware.MenuManager.Overlay;
using BeAware.Overlay.SpellModes;
using BeAware.Utils;

using Divine.Core.Utils;
using Divine.Entity;
using Divine.Entity.Entities.Abilities.Components;
using Divine.Entity.Entities.Abilities.Items;
using Divine.Entity.Entities.Units.Heroes;
using Divine.Helpers;
using Divine.Menu.EventArgs;
using Divine.Menu.Items;
using Divine.Numerics;
using Divine.Renderer;

using Microsoft.Extensions.Logging.Abstractions;

internal abstract class Overlay
{
    protected VisibleStatusMenu VisibleStatusMenu { get; }

    protected TopPanelMenu TopPanelMenu { get; }

    protected HpBarMenu HpBarMenu { get; }

    protected ManaBarMenu ManaBarMenu { get; }

    protected SpellsMenu SpellsMenu { get; }

    protected ItemsMenu ItemsMenu { get; }

    protected TownPortalScrollMenu TownPortalScrollMenu { get; }

    protected EmemyItemPanelMenu EmemyItemPanelMenu { get; }

    private protected readonly Hero LocalHero = EntityManager.LocalHero;

    private readonly Dictionary<string, BaseSpellMode> SpellModes = new()
    {
        { "Default", new Default() },
        { "Low", new Low() },
        { "Without Texture", new WithoutTexture() },
    };

    private BaseSpellMode SpellMode;

    public Overlay(Common common)
    {
        VisibleStatusMenu = common.MenuConfig.OverlayMenu.TopPanelMenu.VisibleStatusMenu;
        TopPanelMenu = common.MenuConfig.OverlayMenu.TopPanelMenu;
        HpBarMenu = common.MenuConfig.OverlayMenu.HpBarMenu;
        ManaBarMenu = common.MenuConfig.OverlayMenu.ManaBarMenu;
        SpellsMenu = common.MenuConfig.OverlayMenu.SpellsMenu;
        ItemsMenu = common.MenuConfig.OverlayMenu.ItemsMenu;
        TownPortalScrollMenu = common.MenuConfig.OverlayMenu.TownPortalScrollMenu;
        EmemyItemPanelMenu = common.MenuConfig.OverlayMenu.EmemyItemPanelMenu;

        RendererManager.LoadImageFromAssembly("BeAware.Resources.Textures.spell_phase.png");
        RendererManager.LoadImageFromAssembly("BeAware.Resources.Textures.hero_visible_left.png");
        RendererManager.LoadImageFromAssembly("BeAware.Resources.Textures.hero_visible_right.png");
        RendererManager.LoadImageFromAssembly("BeAware.Resources.Textures.topbar_health.png");
        RendererManager.LoadImageFromAssembly("BeAware.Resources.Textures.topbar_mana.png");
        RendererManager.LoadImageFromAssembly("BeAware.Resources.Textures.topbar_ulti.png");
        RendererManager.LoadImage(AbilityId.item_tpscroll, AbilityImageType.Item);

        SpellMode = SpellModes[SpellsMenu.ModeItem.Value];

        SpellsMenu.ModeItem.ValueChanged += SpellModeValueChanging;

        RendererManager.Draw += OnRendererManagerDraw;
    }

    public void Dispose()
    {
        RendererManager.Draw -= OnRendererManagerDraw;
    }

    private void SpellModeValueChanging(MenuSelector selector, SelectorChangedEventArgs e)
    {
        SpellMode = SpellModes[e.Value];
    }

    protected abstract IEnumerable<Hero> Heroes { get; }

    protected abstract bool HeroIsVisible(Hero hero);

    protected abstract bool TopBarUltimateOverlay { get; }

    protected virtual bool ManaBarValue { get; }

    protected abstract bool SpellsOverlay { get; }

    protected abstract bool ItemsOverlay { get; }

    protected abstract bool TownPortalScrollOverlay { get; }

    private float ItemExtraSizeX { get; set; }

    private float ItemExtraSizeY { get; set; }

    private float ItemExtraPosX { get; set; }

    private float ItemExtraPosY { get; set; }

    protected virtual void EnemyNotVisibleTimer(Hero hero, TopPanel topPanel)
    {
    }

    private void OnRendererManagerDraw()
    {
        try
        {
            ItemExtraSizeX = HUDInfo.HpBarSizeX / 3.5f + ItemsMenu.ExtraSizeItem.Value;
            ItemExtraSizeY = ItemExtraSizeX / 1.24f;
            ItemExtraPosX = ItemsMenu.ExtraPosXItem.Value * 0.5f;
            ItemExtraPosY = ItemsMenu.ExtraPosYItem.Value * 0.5f;

            var manaBarSizeY = ManaBarUtils.ManaBarSizeY + (ManaBarValue ? 5 : 0);

            foreach (var hero in Heroes)
            {
                var mana = hero.Mana;
                var maximumMana = hero.MaximumMana;
                var manaPerc = mana / maximumMana;

                var isVisible = hero.IsVisible;
                var health = hero.Health;
                var maximumHealth = hero.MaximumHealth;
                var healthPerc = (float)health / maximumHealth;
                var topPanel = new TopPanel(hero);
                if (topPanel != null)
                {
                    if (TopPanelMenu.HealthManaBarItem && hero.IsAlive)
                    {
                        var helthManaBackPosition = topPanel.HelthManaBackPosition;
                        var helthManaBackSize = topPanel.HelthManaBackSize;
                        RendererManager.DrawFilledRectangle(new Rect(helthManaBackPosition.X, helthManaBackPosition.Y, helthManaBackSize.X, helthManaBackSize.Y), Color.Zero, new Color(31, 42, 17), 0);

                        var healthPosition = topPanel.HealthPosition;
                        RendererManager.DrawImage("BeAware.Resources.Textures.topbar_health.png", new Rect(healthPosition.X, healthPosition.Y, topPanel.HealthSizeX(healthPerc), topPanel.HealthSizeY));

                        var manaPosition = topPanel.ManaPosition;
                        RendererManager.DrawImage("BeAware.Resources.Textures.topbar_mana.png", new Rect(manaPosition.X, manaPosition.Y, topPanel.ManaSizeX(manaPerc), topPanel.ManaSizeY));
                    }

                    if (TopBarUltimateOverlay)
                    {
                        var ultimate = hero.Spellbook.MainSpells.FirstOrDefault(x => x.AbilityType == AbilityType.Ultimate && !x.IsHidden);
                        if (ultimate != null)
                        {
                            var cooldown = ultimate.CooldownInFog;
                            if (cooldown > 0)
                            {
                                var ultimateSpellPosition = topPanel.UltimateSpellPosition;
                                var ultimateSpellSize = topPanel.UltimateSpellSize;
                                RendererManager.DrawImage(ultimate.Name, new Rect(ultimateSpellPosition.X, ultimateSpellPosition.Y, ultimateSpellSize.X, ultimateSpellSize.Y), ImageType.RoundAbility, true);

                                var ultimateBackPosition = topPanel.UltimateBackPosition;
                                var ultimateBackSize = topPanel.UltimateBackSize;
                                RendererManager.DrawImage("BeAware.Resources.Textures.topbar_ulti.png", new Rect(ultimateBackPosition.X, ultimateBackPosition.Y, ultimateBackSize.X, ultimateBackSize.Y));

                                var cooldownText = Math.Min((int)cooldown, 99).ToString(CultureInfo.InvariantCulture);
                                var ultimateCooldownPosition = topPanel.UltimateCooldownPosition(cooldownText);
                                var ultimateCooldownSize = topPanel.UltimateCooldownSize;
                                RendererManager.DrawText(cooldownText, ultimateCooldownPosition + new Vector2(2), Color.Black, ultimateCooldownSize);
                                RendererManager.DrawText(cooldownText, ultimateCooldownPosition, Color.WhiteSmoke, ultimateCooldownSize);
                            }
                        }
                    }
                }

                if (!isVisible || !hero.IsAlive)
                {
                    continue;
                }

                var hpBarPosition = HUDInfo.GetHpBarPosition(hero);
                if (hpBarPosition.IsDefault)
                {
                    continue;
                }

                HpBar(health, maximumHealth, healthPerc, hpBarPosition);
                ManaBar(mana, maximumMana, manaPerc, hpBarPosition, manaBarSizeY);

                if (hero.Handle != LocalHero.Handle)
                {
                    if (SpellsOverlay)
                    {
                        SpellMode.DrawOverlay(SpellsMenu, hero, mana, hpBarPosition, manaBarSizeY);
                    }

                    if (ItemsOverlay)
                    {
                        var items = hero.Inventory.MainItems;

                        var defaultPos = hpBarPosition - new Vector2(-HUDInfo.HpBarSizeX / 2 + Math.Max(items.Count() / 2 * ItemExtraSizeX, HUDInfo.HpBarSizeX / 2), ItemExtraSizeY);
                        var position = defaultPos + new Vector2(ItemExtraPosX, ItemExtraPosY);

                        foreach (var item in items)
                        {
                            DrawItems(item, position, mana);
                            position += new Vector2(ItemExtraSizeX, 0);
                        }
                    }
                }

                if (TownPortalScrollOverlay)
                {
                    DrawTownPortalScroll(hero, hpBarPosition, mana);
                }
            }

            EmemyItemPanel();

            foreach (var hero in Heroes.ToArray())
            {
                var topPanel = new TopPanel(hero);
                if (!hero.IsAlive || topPanel == null)
                {
                    continue;
                }

                EnemyNotVisibleTimer(hero, topPanel);

                if (!HeroIsVisible(hero))
                {
                    continue;
                }

                RendererManager.DrawImage($"BeAware.Resources.Textures.{topPanel.VisibleIconName}.png", topPanel.VisibleIconRectangle);
            }
        }
        catch (Exception e)
        {
            Logger.LogError(e);
        }
    }

    protected virtual void HpBar(int health, int maximumHealth, float healthPerc, Vector2 hpBarPosition)
    {
    }

    protected virtual void ManaBar(float mana, float maximumMana, float manaPerc, Vector2 hpBarPosition, float manaBarSizeY)
    {
    }

    protected virtual void EmemyItemPanel()
    {
    }

    private void DrawItems(Item item, Vector2 position, float mana)
    {
        try
        {
            RendererManager.DrawImage(item.Name, new Rect(position.X + 1, position.Y, (float)(ItemExtraSizeX), ItemExtraSizeY), ImageType.Item, true);

            var ratio = HUDInfo.RatioPercentage;
            var manaCost = item.ManaCost;
            var enoughMana = mana >= manaCost;
            var cooldown = Math.Ceiling(item.Cooldown);
            if (cooldown > 0 || !enoughMana)
            {

                RendererManager.DrawFilledRectangle(new Rect(position.X + 1, position.Y, ItemExtraSizeX - 1, ItemExtraSizeY), Color.Zero, enoughMana ? new Color(40, 40, 40, 180) : new Color(25, 25, 130, 190), 0);
            }
            else
            {
                RendererManager.DrawFilledRectangle(new Rect(position.X + 1, position.Y, ItemExtraSizeX - 1, ItemExtraSizeY), Color.Zero, new Color(0, 0, 0, 100), 0);
            }

            if (cooldown > 0)
            {
                var cooldownText = Math.Min(cooldown, 99).ToString(CultureInfo.InvariantCulture);
                var cooldownSize = ItemExtraSizeY / 2 + 3;
                var textSize = RendererManager.MeasureText(cooldownText, cooldownSize);
                var pos = position + new Vector2(ItemExtraSizeX / 2 - textSize.X / 2, (ItemExtraSizeY / 2) - (textSize.Y / 2));

                RendererManager.DrawText(cooldownText, pos, Color.WhiteSmoke, cooldownSize);
            }

            if (!enoughMana && cooldown <= 0)
            {
                var manaCostText = Math.Min(Math.Ceiling(manaCost - mana), 999).ToString(CultureInfo.InvariantCulture);
                var manaCostSize = ItemExtraSizeY / 2 + 3;
                var textSize = RendererManager.MeasureText(manaCostText, manaCostSize);
                var pos = position + new Vector2(ItemExtraSizeX / 2 - textSize.X / 2, (ItemExtraSizeY / 2) - (textSize.Y / 2));

                RendererManager.DrawText(manaCostText, pos, Color.LightBlue, manaCostSize);
            }

            var itemId = item.Id;
            if ((item.IsRequiringCharges || itemId == AbilityId.item_bottle || itemId == AbilityId.item_ward_dispenser || itemId == AbilityId.item_ward_observer || itemId == AbilityId.item_ward_sentry) && cooldown <= 0)
            {
                var currentCharges = item.CurrentCharges.ToString();
                var tSize = Math.Min(ItemExtraSizeY - 2, 13 * ratio);
                var textSize = RendererManager.MeasureText(currentCharges, tSize);
                var tPos = position + new Vector2(ItemExtraSizeX - textSize.X - 2, ItemExtraSizeY - textSize.Y - 1);

                RendererManager.DrawFilledRectangle(new Rect(tPos.X - 1, tPos.Y, textSize.X + 1, textSize.Y + 1), Color.Zero, new Color(0, 0, 0, 220), 0);
                RendererManager.DrawText(currentCharges, tPos, new Color(168, 168, 168), tSize);

                var secondcharges = item.SecondaryCharges;
                if (secondcharges > 0)
                {
                    tPos = position + new Vector2(2, ItemExtraSizeY - textSize.Y - 1);
                    currentCharges = secondcharges.ToString();
                    tSize = Math.Min(ItemExtraSizeY - 2, 13 * ratio);
                    var textSize1 = RendererManager.MeasureText(currentCharges, tSize);

                    RendererManager.DrawFilledRectangle(new Rect(tPos.X - 1, tPos.Y, textSize.X + 1, textSize.Y + 1), Color.Zero, new Color(0, 0, 0, 220), 0);
                    RendererManager.DrawText(currentCharges, tPos, new Color(168, 168, 168), tSize);
                }
            }

            RendererManager.DrawRectangle(new Rect(position.X, position.Y, ItemExtraSizeX + 1, ItemExtraSizeY), Color.Black, 1);
        }
        catch (Exception e)
        {
            Logger.LogError(e);
        }
    }

    private void DrawTownPortalScroll(Hero hero, Vector2 hpBarPosition, float mana)
    {
        var tp = hero.Inventory.TownPortalScroll;
        if (tp != null && tp.IsValid)
        {
            var ratio = HUDInfo.RatioPercentage;
            var size = new Vector2(17, 15) * ratio;
            var position = hpBarPosition - new Vector2(LocalHero.Handle == hero.Handle ? 20 : 17, 0) * ratio;

            RendererManager.DrawImage(@"items\item_tpscroll.png", new Rect(position.X, position.Y, size.X, size.Y));

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
                var cooldownSize = cooldown > 9 ? 12 * ratio : 14 * ratio;
                var textSize = RendererManager.MeasureText(cooldownText, cooldownSize);
                var pos = position + new Vector2(size.X / 2 - textSize.X / 2, size.Y / 2 - textSize.Y / 2);

                RendererManager.DrawText(cooldownText, pos, Color.WhiteSmoke, cooldownSize);
            }

            if (!enoughMana && cooldown <= 0)
            {
                var manaCostText = Math.Min(Math.Ceiling(manaCost - mana), 99).ToString(CultureInfo.InvariantCulture);
                var manaCostSize = 12 * ratio;
                var textSize = RendererManager.MeasureText(manaCostText, manaCostSize);
                var pos = position + new Vector2(size.X / 2 - textSize.X / 2, size.Y / 2 - textSize.Y / 2);

                RendererManager.DrawText(manaCostText, pos, Color.LightBlue, manaCostSize);
            }
        }
    }
}
