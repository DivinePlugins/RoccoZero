namespace BeAware.MenuManager.Overlay;

using Divine.Helpers;
using Divine.Menu.Items;

internal class EmemyItemPanelMenu
{
    public EmemyItemPanelMenu(Menu overlayMenu)
    {
        var ememyItemsPanelMenu = overlayMenu.AddMenu("Ememy Items Panel");
        EmemyItemPanelItem = ememyItemsPanelMenu.AddSwitcher("Enable", false);
        SizeItem = ememyItemsPanelMenu.AddSlider("Size:", 0, -20, 150);
        MoveItem = ememyItemsPanelMenu.AddSwitcher("Move", false);
        PositionXItem = ememyItemsPanelMenu.AddSlider("Position X:", (int)(HUDInfo.ScreenSize.X - 800), 0, 10000);
        PositionYItem = ememyItemsPanelMenu.AddSlider("Position Y:", (int)(HUDInfo.ScreenSize.Y - 240), 0, 10000);
    }

    public MenuSwitcher EmemyItemPanelItem { get; }

    public MenuSlider SizeItem { get; }

    public MenuSwitcher MoveItem { get; }

    public MenuSlider PositionXItem { get; }

    public MenuSlider PositionYItem { get; }
}