namespace BeAware.Overlay.SpellModes;

using System;
using System.Collections.Generic;
using System.Linq;

using BeAware.MenuManager.Overlay;

using Divine.Entity.Entities.Abilities;
using Divine.Entity.Entities.Units.Heroes;
using Divine.Entity.Entities.Units.Heroes.Components;
using Divine.Helpers;

using Divine.Numerics;

internal abstract class BaseSpellMode
{
    public virtual void DrawOverlay(SpellsMenu spellsMenu, Hero hero, float mana, Vector2 hpBarPosition, float manaBarSizeY)
    {
        var sizeX = HUDInfo.HpBarSizeX / 4.12f + spellsMenu.ExtraSizeItem.Value;
        var extraSize = new Vector2(sizeX, sizeX + 1);

        var extraPos = new Vector2(spellsMenu.ExtraPosXItem.Value, spellsMenu.ExtraPosYItem.Value);

        var spells = hero.Spellbook.MainSpells;

        var defaultPos = hpBarPosition + new Vector2(HUDInfo.HpBarSizeX / 2 - Math.Max((spells.Count() / 2) * extraSize.X, HUDInfo.HpBarSizeX / 2), HUDInfo.HpBarSizeY + manaBarSizeY + 3);
        var position = defaultPos + extraPos;

        OverlaySpells(spells, position, hero.Id, mana, extraSize);
    }

    protected abstract void OverlaySpells(IEnumerable<Ability> spells, Vector2 position, HeroId heroId, float mana, Vector2 extraSize);
}