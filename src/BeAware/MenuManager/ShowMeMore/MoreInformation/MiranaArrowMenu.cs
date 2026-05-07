namespace BeAware.MenuManager.ShowMeMore.MoreInformation;

using Divine.Entity.Entities.Abilities.Components;
using Divine.Menu;
using Divine.Menu.Items;

internal class MiranaArrowMenu
{
    public MiranaArrowMenu(Menu moreInformationMenu)
    {
        var miranaArrowMenu = moreInformationMenu.AddMenu("Mirana Arrow").SetImage(AbilityId.mirana_arrow);
        EnableItem = miranaArrowMenu.AddSwitcher("Enable");
        RedItem = miranaArrowMenu.AddSlider("Red:", 255, 0, 255);
        GreenItem = miranaArrowMenu.AddSlider("Green:", 0, 0, 255);
        BlueItem = miranaArrowMenu.AddSlider("Blue:", 0, 0, 255);
        LineRedItem = miranaArrowMenu.AddSlider("Line Red:", 139, 0, 255);
        LineGreenItem = miranaArrowMenu.AddSlider("Line Green:", 0, 0, 255);
        LineBlueItem = miranaArrowMenu.AddSlider("Line Blue:", 0, 0, 255);
        SideMessageItem = miranaArrowMenu.AddSwitcher("Side Message");
        SoundItem = miranaArrowMenu.AddSwitcher("Play Sound");
        OnMinimapItem = miranaArrowMenu.AddSwitcher("Draw On Minimap");
        OnWorldItem = miranaArrowMenu.AddSwitcher("Draw On World");
        WriteOnChatItem = miranaArrowMenu.AddSwitcher("Write On Chat", false);
    }

    public MenuSwitcher EnableItem { get; }

    public MenuSlider RedItem { get; }

    public MenuSlider GreenItem { get; }

    public MenuSlider BlueItem { get; }

    public MenuSlider LineRedItem { get; }

    public MenuSlider LineGreenItem { get; }

    public MenuSlider LineBlueItem { get; }

    public MenuSwitcher SideMessageItem { get; }

    public MenuSwitcher SoundItem { get; }

    public MenuSwitcher OnMinimapItem { get; }

    public MenuSwitcher OnWorldItem { get; }

    public MenuSwitcher WriteOnChatItem { get; }
}