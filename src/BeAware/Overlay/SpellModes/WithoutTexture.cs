namespace BeAware.Overlay.SpellModes;

using System;
using System.Collections.Generic;
using System.Globalization;

using Divine.Entity.Entities.Abilities;
using Divine.Entity.Entities.Abilities.Components;
using Divine.Entity.Entities.Units.Heroes.Components;
using Divine.Numerics;
using Divine.Renderer;

internal sealed class WithoutTexture : BaseSpellMode
{
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
            var enoughMana = mana >= manaCast;
            var cooldown = spell.Cooldown;

            if (cooldown > 0 || !enoughMana || level <= 0)
            {
                var color = level <= 0 ? new Color(10, 10, 10, 210) : (enoughMana ? new Color(40, 40, 40, 180) : new Color(25, 25, 130, 190));
                RendererManager.DrawFilledRectangle(new RectangleF(position.X + 1, position.Y, extraSize.X - 1, extraSize.Y), color, color, 0);
            }

            var notinvospell = heroId != HeroId.npc_dota_hero_invoker || (spell.AbilitySlot != AbilitySlot.Slot4 && spell.AbilitySlot != AbilitySlot.Slot5);
            if (notinvospell)
            {
                var levelText = level.ToString();
                var textSize = RendererManager.MeasureText(levelText, extraSize.X / 2);

                RendererManager.DrawFilledRectangle(new RectangleF(position.X + 1, position.Y, textSize.X + 2, textSize.Y + 1), Color.Zero, new Color(0, 0, 0, 220), 0);
                RendererManager.DrawText(levelText, new Vector2(position.X + 1, position.Y), new Color(168, 168, 168), extraSize.X / 2);
            }

            if (cooldown > 0)
            {
                var cooldownText = (cooldown > 1 ? Math.Min(Math.Ceiling(cooldown), 99) : Math.Round(cooldown, 1)).ToString();
                var cooldownSize = extraSize.X / 2 + 3;
                var textSize = RendererManager.MeasureText(cooldownText, cooldownSize);

                RendererManager.DrawText(cooldownText, position + new Vector2(extraSize.X / 2 - textSize.X / 2, (extraSize.Y / 2) - (textSize.Y / 2)), Color.WhiteSmoke, cooldownSize);
            }

            if (!enoughMana && cooldown <= 0)
            {
                var manaText = Math.Min(Math.Ceiling(manaCast - mana), 999).ToString(CultureInfo.InvariantCulture);
                var textSize = RendererManager.MeasureText(manaText, extraSize.X / 2 + 1);

                Vector2 pos;
                if (!notinvospell)
                {
                    pos = position + new Vector2(extraSize.X / 2 - textSize.X / 2, (float)((extraSize.Y / 1.5) - (textSize.Y / 2)));
                }
                else
                {
                    pos = position + new Vector2((float)(extraSize.X / 1.5 - textSize.X / 2), (float)((extraSize.Y / 1.5) - (textSize.Y / 2)));
                }

                RendererManager.DrawText(manaText, pos, Color.LightBlue, extraSize.X / 2 + 1);
            }

            RendererManager.DrawRectangle(new RectangleF(position.X, position.Y, extraSize.X + 1, extraSize.Y), Color.Black, 1);

            position += new Vector2(extraSize.X, 0);
        }
    }
}