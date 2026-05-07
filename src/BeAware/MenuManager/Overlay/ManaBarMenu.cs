namespace BeAware.MenuManager.Overlay;

using Divine.Menu.Items;

internal class ManaBarMenu
{
    public ManaBarMenu(Menu overlayMenu)
    {
        var manaBarMenu = overlayMenu.AddMenu("Mana Bar");
        ManaBarItem = manaBarMenu.AddSwitcher("Enable");
        ManaBarValueItem = manaBarMenu.AddSwitcher("Value", false);
    }

    public MenuSwitcher ManaBarItem { get; }

    public MenuSwitcher ManaBarValueItem { get; }
}