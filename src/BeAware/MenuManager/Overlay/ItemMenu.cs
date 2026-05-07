namespace BeAware.MenuManager.Overlay;

using Divine.Menu.Items;

internal class ItemsMenu
{
    public ItemsMenu(Menu overlayMenu)
    {
        var itemsMenu = overlayMenu.AddMenu("Items");
        AllyOverlayItem = itemsMenu.AddSwitcher("Ally", false);
        EnemyOverlayItem = itemsMenu.AddSwitcher("Enemy", true);
        ExtraSizeItem = itemsMenu.AddSlider("Extra Size:", 0, -10, 10);
        ExtraPosXItem = itemsMenu.AddSlider("Extra Pos X:", 0, -150, 150);
        ExtraPosYItem = itemsMenu.AddSlider("Extra Pos Y:", 0, -150, 150);
    }

    public MenuSwitcher AllyOverlayItem { get; }

    public MenuSwitcher EnemyOverlayItem { get; }

    public MenuSlider ExtraSizeItem { get; }

    public MenuSlider ExtraPosXItem { get; }

    public MenuSlider ExtraPosYItem { get; }
}