namespace BeAware.MenuManager.Overlay;

using Divine.Helpers;
using Divine.Menu.Items;

internal sealed class NetworthPanelMenu
{
    public NetworthPanelMenu(Menu overlayMenu)
    {
        var networthPanelMenu = overlayMenu.AddMenu("Networth Panel");
        EnableItem = networthPanelMenu.AddSwitcher("Enable", false);
        SizeItem = networthPanelMenu.AddSlider("Size:", 0, -20, 150);
        MoveItem = networthPanelMenu.AddSwitcher("Move", false);
        PositionXItem = networthPanelMenu.AddSlider("Position X:", (int)(HUDInfo.ScreenSize.X - 800), 0, 10000);
        PositionYItem = networthPanelMenu.AddSlider("Position Y:", (int)(HUDInfo.ScreenSize.Y - 240), 0, 10000);
    }

    public MenuSwitcher EnableItem { get; }

    public MenuSlider SizeItem { get; }

    public MenuSwitcher MoveItem { get; }

    public MenuSlider PositionXItem { get; }

    public MenuSlider PositionYItem { get; }
}