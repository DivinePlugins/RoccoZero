namespace BeAware.MenuManager.ShowMeMore.MoreInformation;

using Divine.Entity.Entities.Abilities.Components;
using Divine.Menu;
using Divine.Menu.Items;

internal sealed class PudgeHookMenu
{
    public PudgeHookMenu(Menu moreInformationMenu)
    {
        var pudgeHookMenu = moreInformationMenu.AddMenu("Pudge Hook").SetImage(AbilityId.pudge_meat_hook);
        EnableItem = pudgeHookMenu.AddSwitcher("Enable");
        RedItem = pudgeHookMenu.AddSlider("Red:", 255, 0, 255);
        GreenItem = pudgeHookMenu.AddSlider("Green:", 0, 0, 255);
        BlueItem = pudgeHookMenu.AddSlider("Blue:", 0, 0, 255);
        LineRedItem = pudgeHookMenu.AddSlider("Line Red:", 139, 0, 255);
        LineGreenItem = pudgeHookMenu.AddSlider("Line Green:", 0, 0, 255);
        LineBlueItem = pudgeHookMenu.AddSlider("Line Blue:", 0, 0, 255);
        WhenIsVisibleItem = pudgeHookMenu.AddSwitcher("When Is Visible", false);
        SideMessageItem = pudgeHookMenu.AddSwitcher("Side Message", false);
        SoundItem = pudgeHookMenu.AddSwitcher("Play Sound", false);
        OnMinimapItem = pudgeHookMenu.AddSwitcher("Draw On Minimap");
        OnWorldItem = pudgeHookMenu.AddSwitcher("Draw On World");
    }

    public MenuSwitcher EnableItem { get; }

    public MenuSlider RedItem { get; }

    public MenuSlider GreenItem { get; }

    public MenuSlider BlueItem { get; }

    public MenuSlider LineRedItem { get; }

    public MenuSlider LineGreenItem { get; }

    public MenuSlider LineBlueItem { get; }

    public MenuSwitcher WhenIsVisibleItem { get; }

    public MenuSwitcher SideMessageItem { get; }

    public MenuSwitcher SoundItem { get; }

    public MenuSwitcher OnMinimapItem { get; }

    public MenuSwitcher OnWorldItem { get; }
}