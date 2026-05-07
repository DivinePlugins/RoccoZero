namespace BeAware.MenuManager.PartialMapHack;

using Divine.Menu;
using Divine.Menu.Items;

internal class PartialMapHackMenu
{
    public PartialMapHackMenu(Menu rootMenu)
    {
        var partialMapHackMenu = rootMenu.AddMenu("Partial MapHack");
        SpellsItem = partialMapHackMenu.AddSwitcher("Spells");
        ItemsItem = partialMapHackMenu.AddSwitcher("Items");
        SmokeItem = partialMapHackMenu.AddSwitcher("Smoke");
        TeleportItem = partialMapHackMenu.AddSwitcher("Teleport");
        ModifersItem = partialMapHackMenu.AddSwitcher("Modifers");
        WhenIsVisibleItem = partialMapHackMenu.AddSwitcher("When Is Visible").SetTooltip("Dangerous Ability When Is Visible");
        SideMessageItem = partialMapHackMenu.AddSwitcher("Side Message").SetTooltip("Dangerous Ability Side Message");
        SoundItem = partialMapHackMenu.AddSwitcher("Play Sound").SetTooltip("Dangerous Ability Play Sound");
        OnMinimapItem = partialMapHackMenu.AddSwitcher("Draw On Minimap");
        OnWorldItem = partialMapHackMenu.AddSwitcher("Draw On World");
        ReduceTheNumberOfIconsItem = partialMapHackMenu.AddSwitcher("Reduce The Number Of Icons", false);
        DrawTimerItem = partialMapHackMenu.AddSlider("Draw Timer", 5, 1, 9);
        SizeScale = partialMapHackMenu.AddSlider ( "Size Scale For Drawing", 1.0f, 0.1f, 10f, true, 0.1f );
    }

    public MenuSwitcher SpellsItem { get; }

    public MenuSwitcher ItemsItem { get; }

    public MenuSwitcher SmokeItem { get; }

    public MenuSwitcher TeleportItem { get; }

    public MenuSwitcher ModifersItem { get; }

    public MenuSwitcher WhenIsVisibleItem { get; }

    public MenuSwitcher SideMessageItem { get; }

    public MenuSwitcher SoundItem { get; }

    public MenuSwitcher OnMinimapItem { get; }

    public MenuSwitcher OnWorldItem { get; }

    public MenuSwitcher ReduceTheNumberOfIconsItem { get; }

    public MenuSlider DrawTimerItem { get; }

    public MenuSlider SizeScale { get; }
}