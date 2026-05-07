namespace BeAware.MenuManager.Overlay;

using Divine.Menu.Items;

internal sealed class VisibleStatusMenu
{
    public VisibleStatusMenu(Menu topPanelMenu)
    {
        var visibleStatusMenu = topPanelMenu.AddMenu("Visible Status");
        VisibleStatusAllyItem = visibleStatusMenu.AddSwitcher("Ally", false);
        VisibleStatusEnemyItem = visibleStatusMenu.AddSwitcher("Enemy", false);
        visibleStatusMenu.AddText("");
        EnemyNotVisibleTimeItem = visibleStatusMenu.AddSwitcher("Enemy Not Visible Time", false);
        SizeItem = visibleStatusMenu.AddSlider("Size:", 20, 5, 50);
        RedItem = visibleStatusMenu.AddSlider("Red:", 255, 0, 255);
        GreenItem = visibleStatusMenu.AddSlider("Green:", 255, 0, 255);
        BlueItem = visibleStatusMenu.AddSlider("Blue:", 0, 0, 255);
    }

    public MenuSwitcher VisibleStatusAllyItem { get; }

    public MenuSwitcher VisibleStatusEnemyItem { get; }

    public MenuSwitcher EnemyNotVisibleTimeItem { get; }

    public MenuSlider SizeItem { get; }

    public MenuSlider RedItem { get; }

    public MenuSlider GreenItem { get; }

    public MenuSlider BlueItem { get; }
}