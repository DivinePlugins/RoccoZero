namespace BeAware.Overlay.SpellModes;

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

using BeAware.MenuManager.Overlay;

using Divine.Entity.Entities.Abilities;
using Divine.Entity.Entities.Abilities.Components;
using Divine.Entity.Entities.Units.Heroes;
using Divine.Entity.Entities.Units.Heroes.Components;
using Divine.Helpers;

using Divine.Numerics;
using Divine.Renderer;

internal sealed class Low : BaseSpellMode
{
    public override void DrawOverlay(SpellsMenu spellsMenu, Hero hero, float mana, Vector2 hpBarPosition, float manaBarSizeY)
    {
        var sizeX = HUDInfo.HpBarSizeX / 3.5f + spellsMenu.ExtraSizeItem.Value;
        var extraSize = new Vector2(sizeX, sizeX * 0.35f);

        var extraPos = new Vector2(spellsMenu.ExtraPosXItem.Value, spellsMenu.ExtraPosYItem.Value);

        var spells = hero.Spellbook.MainSpells;

        var defaultPos = hpBarPosition + new Vector2(HUDInfo.HpBarSizeX / 2 - Math.Max((spells.Count() / 2) * extraSize.X, HUDInfo.HpBarSizeX / 2), HUDInfo.HpBarSizeY + manaBarSizeY + 3);
        var position = defaultPos + extraPos;

        OverlaySpells(spells, position, hero.Id, mana, extraSize);
    }

    protected override void OverlaySpells(IEnumerable<Ability> spells, Vector2 position, HeroId heroId, float mana, Vector2 extraSize)
    {
        foreach (var spell in spells)
        {
            if (spell.IsHidden)
            {
                continue;
            }

            if (spell.IsInAbilityPhase)
            {
                RendererManager.DrawImage("BeAware.Resources.Textures.spell_phase.png", new RectangleF(position.X, position.Y, extraSize.X, extraSize.Y));
            }

            var level = spell.Level;
            var manaCast = spell.ManaCost;
            var isEnoughMana = mana >= manaCast;
            var cooldown = spell.Cooldown;
            var isCooldown = cooldown > 0;
            var isLevel = level > 0;

            var extraRange = new Vector2(level > 4 ? (level - 4) * 5.5f : 0, 0);

            if (isCooldown || !isEnoughMana || !isLevel)
            {
                if (isLevel)
                {
                    extraRange.Y = 13;
                }

                var color = !isLevel ? new Color(100, 10, 10, 190) : (isEnoughMana ? new Color(40, 40, 40, 180) : new Color(25, 25, 130, 190));
                RendererManager.DrawFilledRectangle(new RectangleF(position.X + 1, position.Y, extraSize.X - 1, extraSize.Y), color, color, 0);
            }

            var notinvospell = heroId != HeroId.npc_dota_hero_invoker || (spell.AbilitySlot != AbilitySlot.Slot4 && spell.AbilitySlot != AbilitySlot.Slot5);
            if (notinvospell)
            {
                for (var lvl = 1; lvl <= level; lvl++)
                {
                    var rect = new RectangleF(position.X + (extraSize.X * 0.212f * lvl) - extraSize.X * 0.08f, position.Y + 3, extraSize.X * 0.115f, extraSize.X * 0.115f);
                    RendererManager.DrawFilledRectangle(rect, Color.Zero, new Color(255, 255, 0), 0);
                }
            }

            if (isCooldown)
            {
                var cooldownText = (cooldown > 1 ? Math.Min(Math.Ceiling(cooldown), 99) : Math.Round(cooldown, 1)).ToString();
                var cooldownSize = extraSize.X / 2 + 3;
                var textSize = RendererManager.MeasureText(cooldownText, cooldownSize);

                var pos = position + new Vector2(extraSize.X / 2 - textSize.X / 2, (extraSize.Y / 2) - (textSize.Y / 2) + (extraRange.Y - 2));
                RendererManager.DrawText(cooldownText, pos, Color.WhiteSmoke, cooldownSize);
            }

            if (!isEnoughMana && !isCooldown)
            {
                var manaText = Math.Min(Math.Ceiling(manaCast - mana), 999).ToString(CultureInfo.InvariantCulture);
                var textSize = RendererManager.MeasureText(manaText, extraSize.X / 2 + 1);

                Vector2 pos;
                if (!notinvospell)
                {
                    pos = position + new Vector2(extraSize.X / 2 - textSize.X / 2, (extraSize.Y / 2) - (textSize.Y / 2) + (extraRange.Y - 2));
                }
                else
                {
                    pos = position + new Vector2(extraSize.X / 2 - textSize.X / 2, (extraSize.Y / 2) - (textSize.Y / 2) + (extraRange.Y - 2));
                }

                RendererManager.DrawText(manaText, pos, Color.LightBlue, extraSize.X / 2 + 1);
            }

            RendererManager.DrawRectangle(new RectangleF(position.X, position.Y, extraSize.X + 1 + extraRange.X, extraSize.Y + extraRange.Y), Color.Black, 1);

            position += new Vector2(extraSize.X + extraRange.X, 0);
        }
    }
}