namespace BeAware.MenuManager.Overlay;

using Divine.Menu.Items;

internal sealed class TownPortalScrollMenu
{
    public TownPortalScrollMenu(Menu overlayMenu)
    {
        var townPortalScrollMenu = overlayMenu.AddMenu("Town Portal Scroll");
        AllyItem = townPortalScrollMenu.AddSwitcher("Ally");
        EnemyItem = townPortalScrollMenu.AddSwitcher("Enemy");
    }

    public MenuSwitcher AllyItem { get; }

    public MenuSwitcher EnemyItem { get; }
}