namespace BeAware.MenuManager.ShowMeMore.MoreInformation;

using Divine.Entity.Entities.Abilities.Components;
using Divine.Menu;
using Divine.Menu.Items;

internal sealed class LeshracSplitEarthMenu
{
    public LeshracSplitEarthMenu(Menu moreInformationMenu)
    {
        var leshracSplitEarthMenu = moreInformationMenu.AddMenu("Leshrac Split Earth").SetImage(AbilityId.leshrac_split_earth);
        EnableItem = leshracSplitEarthMenu.AddSwitcher("Enable");
        RedItem = leshracSplitEarthMenu.AddSlider("Red:", 255, 0, 255);
        GreenItem = leshracSplitEarthMenu.AddSlider("Green:", 0, 0, 255);
        BlueItem = leshracSplitEarthMenu.AddSlider("Blue:", 0, 0, 255);
        WhenIsVisibleItem = leshracSplitEarthMenu.AddSwitcher("When Is Visible", false);
        SideMessageItem = leshracSplitEarthMenu.AddSwitcher("Side Message", false);
        SoundItem = leshracSplitEarthMenu.AddSwitcher("Play Sound", false);
        OnMinimapItem = leshracSplitEarthMenu.AddSwitcher("Draw On Minimap");
        OnWorldItem = leshracSplitEarthMenu.AddSwitcher("Draw On World");
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