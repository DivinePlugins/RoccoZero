namespace BeAware.MenuManager.ShowMeMore.MoreInformation;

using Divine.Entity.Entities.Abilities.Components;
using Divine.Menu;
using Divine.Menu.Items;

internal sealed class WindrunnerPowershotMenu
{
    public WindrunnerPowershotMenu(Menu moreInformationMenu)
    {
        var windrunnerPowershotMenu = moreInformationMenu.AddMenu("Windrunner Powershot").SetImage(AbilityId.windrunner_powershot);
        EnableItem = windrunnerPowershotMenu.AddSwitcher("Enable");
        RedItem = windrunnerPowershotMenu.AddSlider("Red:", 255, 0, 255);
        GreenItem = windrunnerPowershotMenu.AddSlider("Green:", 0, 0, 255);
        BlueItem = windrunnerPowershotMenu.AddSlider("Blue:", 0, 0, 255);
        WhenIsVisibleItem = windrunnerPowershotMenu.AddSwitcher("When Is Visible", false);
        SideMessageItem = windrunnerPowershotMenu.AddSwitcher("Side Message", false);
        SoundItem = windrunnerPowershotMenu.AddSwitcher("Play Sound", false);
        OnMinimapItem = windrunnerPowershotMenu.AddSwitcher("Draw On Minimap");
        OnWorldItem = windrunnerPowershotMenu.AddSwitcher("Draw On World");
    }

    public MenuSwitcher EnableItem { get; }

    public MenuSlider RedItem { get; }

    public MenuSwitcher WhenIsVisibleItem { get; }

    public MenuSlider GreenItem { get; }

    public MenuSlider BlueItem { get; }

    public MenuSwitcher SideMessageItem { get; }

    public MenuSwitcher SoundItem { get; }

    public MenuSwitcher OnMinimapItem { get; }

    public MenuSwitcher OnWorldItem { get; }
}