namespace BeAware.MenuManager.Overlay;

using Divine.Menu.Items;

internal class UltimateBarMenu
{
    public UltimateBarMenu(Menu topPanelMenu)
    {
        var ultimateBarMenu = topPanelMenu.AddMenu("Ultimate Bar");

        UltimateBarAllyItem = ultimateBarMenu.AddSwitcher("Ally");
        UltimateBarEnemyItem = ultimateBarMenu.AddSwitcher("Enemy");
    }

    public MenuSwitcher UltimateBarAllyItem { get; set; }

    public MenuSwitcher UltimateBarEnemyItem { get; set; }
}