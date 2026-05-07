namespace BeAware.MenuManager.ShowMeMore.MoreInformation;

using Divine.Entity.Entities.Abilities.Components;
using Divine.Menu;
using Divine.Menu.Items;

internal sealed class LinaLightStrikeArrayMenu
{
    public LinaLightStrikeArrayMenu(Menu moreInformationMenu)
    {
        var linaLightStrikeArrayMenu = moreInformationMenu.AddMenu("Lina Light Strike Array").SetImage(AbilityId.lina_light_strike_array);
        EnableItem = linaLightStrikeArrayMenu.AddSwitcher("Enable");
        RedItem = linaLightStrikeArrayMenu.AddSlider("Red:", 255, 0, 255);
        GreenItem = linaLightStrikeArrayMenu.AddSlider("Green:", 0, 0, 255);
        BlueItem = linaLightStrikeArrayMenu.AddSlider("Blue:", 0, 0, 255);
        WhenIsVisibleItem = linaLightStrikeArrayMenu.AddSwitcher("When Is Visible", false);
        SideMessageItem = linaLightStrikeArrayMenu.AddSwitcher("Side Message", false);
        SoundItem = linaLightStrikeArrayMenu.AddSwitcher("Play Sound", false);
        OnMinimapItem = linaLightStrikeArrayMenu.AddSwitcher("Draw On Minimap");
        OnWorldItem = linaLightStrikeArrayMenu.AddSwitcher("Draw On World");
    }

    public MenuSwitcher EnableItem { get; }

    public MenuSlider RedItem { get; }

    public MenuSlider GreenItem { get; }

    public MenuSlider BlueItem { get; }

    public MenuSwitcher WhenIsVisibleItem { get; }

    public MenuSwitcher SideMessageItem { get; }

    public MenuSwitcher SoundItem { get; }

    public MenuSwitcher OnMinimapItem { get; }

    public MenuSwitcher OnWorldItem { get; }
}