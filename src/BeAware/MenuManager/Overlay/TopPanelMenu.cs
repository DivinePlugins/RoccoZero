namespace BeAware.MenuManager.Overlay;

using Divine.Menu.Items;

internal class TopPanelMenu
{
    public TopPanelMenu(Menu overlayMenu)
    {
        var topPanelMenu = overlayMenu.AddMenu("Top Panel");

        VisibleStatusMenu = new VisibleStatusMenu(topPanelMenu);
        UltimateBarMenu = new UltimateBarMenu(topPanelMenu);

        HealthManaBarItem = topPanelMenu.AddSwitcher("Health and Mana Bar");
    }

    public VisibleStatusMenu VisibleStatusMenu { get; }

    public UltimateBarMenu UltimateBarMenu { get; }

    public MenuSwitcher HealthManaBarItem { get; }
}