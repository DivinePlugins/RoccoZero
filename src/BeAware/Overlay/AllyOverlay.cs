namespace BeAware.Overlay;

using System.Collections.Generic;
using System.Linq;

using Divine.Entity;
using Divine.Entity.Entities.Units.Heroes;
using Divine.Extensions;

internal sealed class AllyOverlay : Overlay
{
    public AllyOverlay(Common common) : base(common)
    {
    }

    protected override IEnumerable<Hero> Heroes
    {
        get
        {
            return EntityManager.GetEntities<Hero>().Where(x => x.IsAlly(LocalHero) && !x.IsIllusion);
        }
    }

    protected override bool HeroIsVisible(Hero hero)
    {
        if (!VisibleStatusMenu.VisibleStatusAllyItem)
        {
            return false;
        }

        return hero.IsVisibleToEnemies;
    }

    protected override bool TopBarUltimateOverlay
    {
        get
        {
            return TopPanelMenu.UltimateBarMenu.UltimateBarAllyItem;
        }
    }

    protected override bool SpellsOverlay
    {
        get
        {
            return SpellsMenu.AllyOverlayItem;
        }
    }

    protected override bool ItemsOverlay
    {
        get
        {
            return ItemsMenu.AllyOverlayItem;
        }
    }

    protected override bool TownPortalScrollOverlay
    {
        get
        {
            return TownPortalScrollMenu.AllyItem;
        }
    }
}