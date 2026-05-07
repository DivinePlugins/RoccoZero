namespace BeAware.MenuManager.Overlay;

using Divine.Menu.Items;

internal class HpBarMenu
{
    public HpBarMenu(Menu overlayMenu)
    {
        var hpBarMenu = overlayMenu.AddMenu("Hp Bar");
        HpBarValueItem = hpBarMenu.AddSwitcher("Value", false);
    }

    public MenuSwitcher HpBarValueItem { get; }
}